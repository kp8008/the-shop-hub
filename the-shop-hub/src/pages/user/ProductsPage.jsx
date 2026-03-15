import { useState, useEffect } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import { Filter, Grid, List, Star, Heart, ShoppingCart } from 'lucide-react'
import api, { getImageUrl, getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import useCartStore from '../../store/useCartStore'
import useFavoriteStore from '../../store/useFavoriteStore'
import useAuthStore from '../../store/useAuthStore'
import { useGuestSignUpPrompt } from '../../context/GuestSignUpContext'
import toast from 'react-hot-toast'

const ProductsPage = () => {
  const [products, setProducts] = useState([])
  const [categories, setCategories] = useState([])
  const [loading, setLoading] = useState(true)
  const [viewMode, setViewMode] = useState('grid')
  const [showFilters, setShowFilters] = useState(false)
  const [searchParams] = useSearchParams()
  
  const [filters, setFilters] = useState({
    category: searchParams.get('category') || '',
    search: searchParams.get('search') || '',
    minPrice: '',
    maxPrice: '',
    sortBy: 'newest'
  })

  const { addItem } = useCartStore()
  const { isAuthenticated } = useAuthStore()
  const { showSignUpPrompt } = useGuestSignUpPrompt() || {}
  const { isFavorite, toggleFavorite, fetchFavorites } = useFavoriteStore()

  useEffect(() => {
    fetchData()
    if (isAuthenticated) {
      fetchFavorites()
    }
  }, [filters, isAuthenticated])

  // Sync filters when URL query changes (e.g., clicking top bar links or performing a search)
  useEffect(() => {
    const cat = searchParams.get('category') || ''
    const s = searchParams.get('search') || ''
    setFilters(prev => {
      if (prev.category === cat && prev.search === s) return prev
      return { ...prev, category: cat, search: s }
    })
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchParams])

  const fetchData = async () => {
    try {
      setLoading(true)
      const [productsRes, categoriesRes] = await Promise.all([
        api.get('/Product'),
        api.get('/Category')
      ])

      const mappedProducts = productsRes.data.map(p => ({
        productId: p.productId ?? p.productID ?? p.ProductID,
        productName: p.productName ?? p.ProductName,
        description: p.description ?? p.Description ?? '',
        price: p.price ?? p.Price ?? 0,
        imageUrl: getImageUrl(p.image ?? p.Image),
        categoryId: p.categoryId ?? p.categoryID ?? p.CategoryID
      }))

      const mappedCategories = categoriesRes.data.map(c => ({
        categoryId: c.categoryId ?? c.categoryID ?? c.CategoryID,
        categoryName: c.categoryName ?? c.CategoryName
      }))
      
      let filteredProducts = mappedProducts

      const slugify = (str) =>
        String(str || '')
          .toLowerCase()
          .replace(/&/g, 'and')
          .replace(/[^a-z0-9]+/g, '-')
          .replace(/^-+|-+$/g, '')

      const synonyms = {
        fashion: 'clothing',
        home: 'home-and-kitchen',
        'home-living': 'home-and-kitchen'
      }

      // Apply filters
      if (filters.category) {
        let catId = null

        // If query param is a numeric ID, use it directly
        if (/^\d+$/.test(filters.category)) {
          catId = parseInt(filters.category, 10)
        } else {
          const wantedSlug = slugify(synonyms[filters.category] || filters.category)

          const matched = mappedCategories.find((c) => {
            const catSlug = slugify(c.categoryName)
            return (
              catSlug === wantedSlug ||
              catSlug.includes(wantedSlug) ||
              wantedSlug.includes(catSlug)
            )
          })

          if (matched) {
            catId = matched.categoryId
          }
        }

        if (catId !== null) {
          filteredProducts = filteredProducts.filter((p) => p.categoryId === catId)
        }
      }
      if (filters.search) {
        const term = filters.search.toLowerCase()
        filteredProducts = filteredProducts.filter(p =>
          (p.productName || '').toLowerCase().includes(term) ||
          (p.description || '').toLowerCase().includes(term)
        )
      }
      if (filters.minPrice) {
        filteredProducts = filteredProducts.filter(p => p.price >= parseFloat(filters.minPrice))
      }
      if (filters.maxPrice) {
        filteredProducts = filteredProducts.filter(p => p.price <= parseFloat(filters.maxPrice))
      }

      // Apply sorting
      if (filters.sortBy === 'price-low') {
        filteredProducts.sort((a, b) => a.price - b.price)
      } else if (filters.sortBy === 'price-high') {
        filteredProducts.sort((a, b) => b.price - a.price)
      }

      setProducts(filteredProducts)
      setCategories(mappedCategories)
    } catch (error) {
      console.error('Error fetching products:', error)
      toast.error('Failed to load products')
    } finally {
      setLoading(false)
    }
  }

  const handleAddToCart = (product) => {
    if (!isAuthenticated) {
      showSignUpPrompt?.()
      return
    }
    addItem(product)
    toast.success('Added to cart!')
  }

  const handleToggleFavorite = async (productId) => {
    if (!isAuthenticated) {
      showSignUpPrompt?.()
      return
    }
    
    const wasFavorite = isFavorite(productId)
    try {
      await toggleFavorite(productId)
      toast.success(wasFavorite ? 'Removed from wishlist' : 'Added to wishlist')
    } catch (error) {
      toast.error('Failed to update wishlist')
    }
  }

  return (
    <div className="pb-16 min-h-screen bg-kaira-light">
      <div className="max-w-[1800px] mx-auto px-4 md:px-6">
        {/* Header - Kaira */}
        <div className="mb-8">
          <h1 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-2">All Products</h1>
          <p className="font-jost text-gray-600">Discover our amazing collection</p>
        </div>

        <div className="flex flex-col lg:flex-row gap-8">
          {/* Filters Sidebar */}
          <motion.div
            initial={{ x: -50, opacity: 0 }}
            animate={{ x: 0, opacity: 1 }}
            className={`lg:w-64 ${showFilters ? 'block' : 'hidden lg:block'}`}
          >
            <div className="bg-white border border-gray-200 p-6 sticky top-24">
              <div className="flex items-center justify-between mb-6">
                <h2 className="font-marcellus text-[#111] text-lg">Filters</h2>
                <button
                  onClick={() => setFilters({ category: '', minPrice: '', maxPrice: '', sortBy: 'newest' })}
                  className="text-sm font-jost text-kaira hover:underline"
                >
                  Clear All
                </button>
              </div>

              {/* Category Filter */}
              <div className="mb-6">
                <h3 className="font-marcellus text-[#111] text-sm uppercase tracking-wide mb-3">Category</h3>
                <div className="space-y-2 font-jost">
                  <label className="flex items-center space-x-2 cursor-pointer">
                    <input
                      type="radio"
                      name="category"
                      checked={filters.category === ''}
                      onChange={() => setFilters({ ...filters, category: '' })}
                      className="text-kaira focus:ring-kaira"
                    />
                    <span>All Categories</span>
                  </label>
                  {categories.map(cat => (
                    <label key={cat.categoryId} className="flex items-center space-x-2 cursor-pointer">
                      <input
                        type="radio"
                        name="category"
                        checked={filters.category === cat.categoryId.toString()}
                        onChange={() => setFilters({ ...filters, category: cat.categoryId.toString() })}
                        className="text-kaira focus:ring-kaira"
                      />
                      <span>{cat.categoryName}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* Price Range */}
              <div className="mb-6">
                <h3 className="font-marcellus text-[#111] text-sm uppercase tracking-wide mb-3">Price Range</h3>
                <div className="space-y-3">
                  <input
                    type="number"
                    placeholder="Min Price"
                    value={filters.minPrice}
                    onChange={(e) => setFilters({ ...filters, minPrice: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  />
                  <input
                    type="number"
                    placeholder="Max Price"
                    value={filters.maxPrice}
                    onChange={(e) => setFilters({ ...filters, maxPrice: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  />
                </div>
              </div>

              {/* Sort By */}
              <div>
                <h3 className="font-marcellus text-[#111] text-sm uppercase tracking-wide mb-3">Sort By</h3>
                <select
                  value={filters.sortBy}
                  onChange={(e) => setFilters({ ...filters, sortBy: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                >
                  <option value="newest">Newest</option>
                  <option value="price-low">Price: Low to High</option>
                  <option value="price-high">Price: High to Low</option>
                </select>
              </div>
            </div>
          </motion.div>

          {/* Products Grid */}
          <div className="flex-1">
            {/* Toolbar - Kaira */}
            <div className="bg-white border border-gray-200 p-4 mb-6 flex items-center justify-between">
              <p className="font-jost text-gray-600">
                <span className="font-semibold text-[#111]">{products.length}</span> products found
              </p>
              <div className="flex items-center space-x-4">
                <button
                  onClick={() => setShowFilters(!showFilters)}
                  className="lg:hidden border border-[#111] text-[#111] px-4 py-2 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex items-center space-x-2"
                >
                  <Filter className="w-4 h-4" />
                  <span>Filters</span>
                </button>
                <div className="flex items-center space-x-2">
                  <button
                    onClick={() => setViewMode('grid')}
                    className={`p-2 ${viewMode === 'grid' ? 'bg-kaira-light text-kaira' : 'text-gray-600 hover:bg-gray-100'}`}
                  >
                    <Grid className="w-5 h-5" />
                  </button>
                  <button
                    onClick={() => setViewMode('list')}
                    className={`p-2 ${viewMode === 'list' ? 'bg-kaira-light text-kaira' : 'text-gray-600 hover:bg-gray-100'}`}
                  >
                    <List className="w-5 h-5" />
                  </button>
                </div>
              </div>
            </div>

            {/* Products */}
            {loading ? (
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
                {[...Array(6)].map((_, i) => (
                  <div key={i} className="bg-white border border-gray-200 p-4 animate-pulse">
                    <div className="aspect-square bg-gray-200 mb-4"></div>
                    <div className="h-4 bg-gray-200 rounded mb-2"></div>
                    <div className="h-4 bg-gray-200 rounded w-2/3"></div>
                  </div>
                ))}
              </div>
            ) : (
              <AnimatePresence mode="wait">
                {viewMode === 'grid' ? (
                  <motion.div
                    key="grid"
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    exit={{ opacity: 0 }}
                    className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6"
                  >
                    {products.map((product, index) => (
                      <ProductGridCard
                        key={product.productId}
                        product={product}
                        index={index}
                        onAddToCart={handleAddToCart}
                        onToggleFavorite={handleToggleFavorite}
                        isFavorite={isFavorite(product.productId)}
                      />
                    ))}
                  </motion.div>
                ) : (
                  <motion.div
                    key="list"
                    initial={{ opacity: 0 }}
                    animate={{ opacity: 1 }}
                    exit={{ opacity: 0 }}
                    className="space-y-4"
                  >
                    {products.map((product, index) => (
                      <ProductListCard
                        key={product.productId}
                        product={product}
                        index={index}
                        onAddToCart={handleAddToCart}
                        onToggleFavorite={handleToggleFavorite}
                        isFavorite={isFavorite(product.productId)}
                      />
                    ))}
                  </motion.div>
                )}
              </AnimatePresence>
            )}

            {!loading && products.length === 0 && (
              <div className="text-center py-20">
                <p className="font-jost text-xl text-gray-500 mb-4">No products found</p>
                <button
                  onClick={() => setFilters({ category: '', minPrice: '', maxPrice: '', sortBy: 'newest' })}
                  className="bg-[#111] text-white px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
                >
                  Clear Filters
                </button>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  )
}

const ProductGridCard = ({ product, index, onAddToCart, onToggleFavorite, isFavorite }) => {
  const [isToggling, setIsToggling] = useState(false)

  const handleFavoriteClick = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    setIsToggling(true)
    await onToggleFavorite(product.productId)
    setIsToggling(false)
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: index * 0.05 }}
      className="group bg-white border border-gray-200 overflow-hidden hover:border-kaira hover:shadow-lg transition-all duration-300"
    >
      <Link to={`/products/${product.productId}`}>
        <div className="image-zoom-effect relative overflow-hidden aspect-square">
          <img
            src={product.imageUrl || getPlaceholderImageUrl(300, 300)}
            alt={product.productName}
            className="w-full h-full object-cover"
          />
          <motion.button
            whileHover={{ scale: 1.1 }}
            whileTap={{ scale: 0.9 }}
            onClick={handleFavoriteClick}
            disabled={isToggling}
            className={`absolute top-3 right-3 w-10 h-10 rounded-full flex items-center justify-center transition-all ${
              isFavorite 
                ? 'bg-[#111] text-white' 
                : 'bg-white/90 text-[#111] hover:bg-[#111] hover:text-white'
            } ${isToggling ? 'opacity-50' : ''}`}
          >
            <Heart className={`w-5 h-5 ${isFavorite ? 'fill-current' : ''}`} />
          </motion.button>
        </div>
      </Link>
      
      <div className="p-4">
        <Link to={`/products/${product.productId}`}>
          <h3 className="font-marcellus text-[#111] mb-2 line-clamp-2 group-hover:text-kaira transition-colors">
            {product.productName}
          </h3>
        </Link>
        
        <div className="flex items-center space-x-1 mb-3 text-amber-400">
          {[...Array(5)].map((_, i) => (
            <Star key={i} className="w-4 h-4 fill-current" />
          ))}
          <span className="text-sm text-gray-600 ml-2 font-jost">(4.5)</span>
        </div>
        
        <div className="flex items-center justify-between">
          <p className="font-jost text-lg font-medium text-[#111]">{formatPrice(product.price)}</p>
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            onClick={() => onAddToCart(product)}
            className="bg-[#111] text-white p-2 hover:bg-kaira-dark transition-colors"
          >
            <ShoppingCart className="w-5 h-5" />
          </motion.button>
        </div>
      </div>
    </motion.div>
  )
}

const ProductListCard = ({ product, index, onAddToCart, onToggleFavorite, isFavorite }) => {
  const [isToggling, setIsToggling] = useState(false)

  const handleFavoriteClick = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    setIsToggling(true)
    await onToggleFavorite(product.productId)
    setIsToggling(false)
  }

  return (
    <motion.div
      initial={{ opacity: 0, x: -20 }}
      animate={{ opacity: 1, x: 0 }}
      transition={{ delay: index * 0.05 }}
      className="bg-white border border-gray-200 overflow-hidden hover:border-kaira transition-all duration-300 flex"
    >
      <Link to={`/products/${product.productId}`} className="w-48 flex-shrink-0 relative">
        <img
          src={product.imageUrl || getPlaceholderImageUrl(300, 300)}
          alt={product.productName}
          className="w-full h-full object-cover"
        />
        <button
          onClick={handleFavoriteClick}
          disabled={isToggling}
          className={`absolute top-3 left-3 p-2 rounded-full transition-all ${
            isFavorite 
              ? 'bg-[#111] text-white' 
              : 'bg-white/80 text-[#111] hover:bg-[#111] hover:text-white'
          } ${isToggling ? 'opacity-50' : ''}`}
        >
          <Heart className={`w-5 h-5 ${isFavorite ? 'fill-current' : ''}`} />
        </button>
      </Link>
      
      <div className="flex-1 p-6 flex flex-col justify-between">
        <div>
          <Link to={`/products/${product.productId}`}>
            <h3 className="text-xl font-marcellus text-[#111] mb-2 hover:text-kaira transition-colors">
              {product.productName}
            </h3>
          </Link>
          <p className="font-jost text-gray-600 mb-4 line-clamp-2">{product.description}</p>
          <div className="flex items-center space-x-1 text-amber-400">
            {[...Array(5)].map((_, i) => (
              <Star key={i} className="w-4 h-4 fill-current" />
            ))}
            <span className="text-sm text-gray-600 ml-2 font-jost">(4.5)</span>
          </div>
        </div>
        
        <div className="flex items-center justify-between mt-4">
          <p className="font-jost text-2xl font-medium text-[#111]">{formatPrice(product.price)}</p>
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            onClick={() => onAddToCart(product)}
            className="bg-[#111] text-white px-5 py-2.5 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex items-center space-x-2"
          >
            <ShoppingCart className="w-5 h-5" />
            <span>Add to Cart</span>
          </motion.button>
        </div>
      </div>
    </motion.div>
  )
}

export default ProductsPage
