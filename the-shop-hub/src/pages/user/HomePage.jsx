import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { motion } from 'framer-motion'
import {
  ArrowRight,
  Truck,
  Shield,
  CreditCard,
  Headphones,
  Star,
  Monitor,
  Shirt,
  BookOpen,
  Home as HomeIcon,
  Watch as WatchIcon,
  Dumbbell,
  Package,
  Heart
} from 'lucide-react'
import api, { getImageUrl, getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import useAuthStore from '../../store/useAuthStore'
import useFavoriteStore from '../../store/useFavoriteStore'
import { useGuestSignUpPrompt } from '../../context/GuestSignUpContext'
import toast from 'react-hot-toast'

const HomePage = () => {
  const [featuredProducts, setFeaturedProducts] = useState([])
  const [categories, setCategories] = useState([])
  const { isAuthenticated, user } = useAuthStore()
  const { fetchFavorites } = useFavoriteStore()

  useEffect(() => {
    fetchData()
  }, [])

  // Fetch favorites when user is authenticated
  useEffect(() => {
    if (isAuthenticated) {
      fetchFavorites()
    }
  }, [isAuthenticated])

  const fetchData = async () => {
    try {
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
      setFeaturedProducts(mappedProducts.slice(0, 8))
      setCategories(mappedCategories.slice(0, 6))
    } catch (error) {
      console.error('Error fetching data:', error)
    }
  }

  const features = [
    {
      icon: <Truck className="w-8 h-8" />,
      title: 'Free Shipping',
      description: 'On orders over ₹500'
    },
    {
      icon: <Shield className="w-8 h-8" />,
      title: 'Secure Payment',
      description: '100% secure transactions'
    },
    {
      icon: <CreditCard className="w-8 h-8" />,
      title: 'Easy Returns',
      description: '30-day return policy'
    },
    {
      icon: <Headphones className="w-8 h-8" />,
      title: '24/7 Support',
      description: 'Dedicated support team'
    }
  ]

  return (
    <div>
      {/* Hero / Billboard - Kaira style */}
      <section id="billboard" className="relative py-12 md:py-16 bg-gradient-to-b from-gray-50 to-kaira-light border-t border-gray-100">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <div className="flex flex-col items-center text-center">
            <motion.h1
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.6 }}
              className="font-marcellus text-4xl md:text-5xl lg:text-6xl text-[#111] mb-4"
            >
              New collections are here!!
            </motion.h1>
            <motion.p
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.2, duration: 0.6 }}
              className="font-jost text-gray-600 max-w-2xl mb-8 text-sm md:text-base leading-relaxed"
            >
              Discover amazing products at unbeatable prices. Your perfect shopping experience starts here.
            </motion.p>
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ delay: 0.3, duration: 0.6 }}
              className="flex flex-wrap justify-center gap-4"
            >
              <Link to="/products">
                <motion.button
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  className="bg-[#111] text-white px-8 py-3 text-sm font-jost uppercase tracking-[0.1em] hover:bg-kaira-dark transition-colors"
                >
                  Shop Now
                </motion.button>
              </Link>
              <Link to="/products?featured=true">
                <motion.button
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  className="border border-[#111] text-[#111] px-8 py-3 text-sm font-jost uppercase tracking-[0.1em] hover:bg-[#111] hover:text-white transition-colors"
                >
                  View Deals
                </motion.button>
              </Link>
            </motion.div>
          </div>
        </div>
      </section>

      {/* Features strip - Kaira style */}
      <section className="features py-10 md:py-12 bg-white border-y border-gray-200">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {features.map((feature, index) => (
              <motion.div
                key={index}
                initial={{ opacity: 0, y: 16 }}
                whileInView={{ opacity: 1, y: 0 }}
                viewport={{ once: true }}
                transition={{ delay: index * 0.1 }}
                className="text-center py-6 md:py-8"
              >
                <div className="w-12 h-12 flex items-center justify-center mx-auto mb-4 text-kaira">
                  {feature.icon}
                </div>
                <h3 className="font-marcellus text-[#111] text-lg mb-2">{feature.title}</h3>
                <p className="font-jost text-gray-600 text-sm">{feature.description}</p>
              </motion.div>
            ))}
          </div>
        </div>
      </section>

      {/* Categories - Kaira style */}
      <section className="py-14 md:py-16 bg-kaira-light">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="text-center mb-10"
          >
            <h2 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-2">Shop by Category</h2>
            <p className="font-jost text-gray-600">Explore our wide range of products</p>
          </motion.div>

          <div className="relative overflow-hidden">
            <motion.div
              className="flex gap-6"
              initial={{ x: 0 }}
              animate={{ x: ['0%', '-50%'] }}
              transition={{ duration: 30, repeat: Infinity, ease: 'linear' }}
            >
              {[...categories, ...categories].map((category, index) => (
                <motion.div
                  key={`${category.categoryId}-${index}`}
                  whileHover={{ y: -4 }}
                  className="min-w-[180px] shrink-0 group"
                >
                  <Link to={`/products?category=${category.categoryId}`}>
                    <div className="bg-white border border-gray-200 p-6 text-center hover:border-kaira transition-all duration-300 cursor-pointer">
                      <div className="w-16 h-16 bg-kaira-light rounded flex items-center justify-center mx-auto mb-4 text-kaira group-hover:bg-kaira group-hover:text-white transition-colors">
                        {(() => {
                          const n = (category.categoryName || '').toLowerCase()
                          const is = (s) => n.includes(s)
                          const props = { className: 'w-8 h-8' }
                          if (is('electronic')) return <Monitor {...props} />
                          if (is('fashion') || is('cloth')) return <Shirt {...props} />
                          if (is('book')) return <BookOpen {...props} />
                          if (is('home') || is('kitchen')) return <HomeIcon {...props} />
                          if (is('watch')) return <WatchIcon {...props} />
                          if (is('sport')) return <Dumbbell {...props} />
                          return <Package {...props} />
                        })()}
                      </div>
                      <h3 className="font-marcellus text-[#111] group-hover:text-kaira transition-colors">
                        {category.categoryName}
                      </h3>
                      <p className="font-jost text-xs text-gray-500 mt-1 uppercase tracking-wide">Explore</p>
                    </div>
                  </Link>
                </motion.div>
              ))}
            </motion.div>
          </div>
        </div>
      </section>

      {/* Featured Products - Kaira style */}
      <section className="py-14 md:py-16 bg-white">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
            className="flex flex-col sm:flex-row sm:justify-between sm:items-end gap-4 mb-10"
          >
            <div>
              <h2 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-1">Featured Products</h2>
              <p className="font-jost text-gray-600">Handpicked items just for you</p>
            </div>
            <Link to="/products" className="nav-link-kaira text-kaira text-sm font-jost uppercase tracking-[0.1em] inline-flex items-center gap-2 w-fit">
              View All <ArrowRight className="w-4 h-4" />
            </Link>
          </motion.div>

          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {featuredProducts.map((product, index) => (
              <ProductCard key={product.productId} product={product} index={index} />
            ))}
          </div>
        </div>
      </section>

      {/* CTA - Kaira style (dark bg) with light background text */}
      <section className="relative py-12 md:py-16 bg-[#111] text-white overflow-hidden">
        {/* Faint background text - reference style */}
        <div className="absolute inset-0 flex flex-wrap items-center justify-center gap-x-8 gap-y-4 p-6 pointer-events-none select-none" aria-hidden>
          {[...Array(12)].map((_, i) => (
            <span
              key={i}
              className="font-marcellus text-[8vw] md:text-[6vw] lg:text-[5vw] font-bold text-white whitespace-nowrap"
              style={{
                opacity: 0.06,
                transform: `rotate(${i % 2 === 0 ? -3 : 2}deg)`,
              }}
            >
              New Collections only on SHOP-HUB!!
            </span>
          ))}
        </div>
        <div className="relative z-10 max-w-[1800px] mx-auto px-4 md:px-6 text-center">
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            whileInView={{ opacity: 1, y: 0 }}
            viewport={{ once: true }}
          >
            {/* Same promo for everyone (like second image); Sign Up only when not logged in */}
            <h2 className="font-marcellus text-4xl md:text-5xl lg:text-6xl mb-4">
              Our 1st Anniversary is here!
            </h2>
            <p className="font-jost text-gray-200 text-xl md:text-2xl lg:text-3xl mb-10 max-w-2xl mx-auto leading-relaxed">
              Get <span className="font-semibold text-white">5% extra off</span> on every order for a limited time.
            </p>
            <div className="flex flex-wrap justify-center gap-4">
              <Link to="/products">
                <motion.button
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  className="bg-white text-[#111] px-8 py-3 text-sm font-jost uppercase tracking-[0.1em] hover:bg-kaira-light hover:text-[#111] transition-colors"
                >
                  Shop Anniversary Deals
                </motion.button>
              </Link>
              {isAuthenticated ? (
                <Link to="/orders">
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    className="border border-white text-white px-8 py-3 text-sm font-jost uppercase tracking-[0.1em] hover:bg-white hover:text-[#111] transition-colors"
                  >
                    Track My Orders
                  </motion.button>
                </Link>
              ) : (
                <Link to="/register">
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    className="border border-white text-white px-8 py-3 text-sm font-jost uppercase tracking-[0.1em] hover:bg-white hover:text-[#111] transition-colors"
                  >
                    Sign Up Now
                  </motion.button>
                </Link>
              )}
            </div>
          </motion.div>
        </div>
      </section>
    </div>
  )
}

const ProductCard = ({ product, index }) => {
  const { isAuthenticated } = useAuthStore()
  const { showSignUpPrompt } = useGuestSignUpPrompt() || {}
  const { isFavorite, toggleFavorite } = useFavoriteStore()
  const [isToggling, setIsToggling] = useState(false)
  
  const productId = product.productId || product.productID || product.ProductID
  const isFav = isFavorite(productId)

  const handleFavoriteClick = async (e) => {
    e.preventDefault()
    e.stopPropagation()
    
    if (!isAuthenticated) {
      showSignUpPrompt?.()
      return
    }
    
    setIsToggling(true)
    try {
      await toggleFavorite(productId)
      toast.success(isFav ? 'Removed from wishlist' : 'Added to wishlist')
    } catch (error) {
      toast.error('Failed to update wishlist')
    } finally {
      setIsToggling(false)
    }
  }

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      whileInView={{ opacity: 1, y: 0 }}
      viewport={{ once: true }}
      transition={{ delay: index * 0.1 }}
      whileHover={{ y: -6 }}
      className="group"
    >
      <Link to={`/products/${productId}`}>
        <div className="bg-white border border-gray-200 overflow-hidden hover:border-kaira hover:shadow-lg transition-all duration-300">
          <div className="image-zoom-effect relative overflow-hidden aspect-square">
            <img
              src={product.imageUrl || getPlaceholderImageUrl(300, 300)}
              alt={product.productName}
              className="w-full h-full object-cover"
              onError={(e) => { e.target.onerror = null; e.target.src = getPlaceholderImageUrl(300, 300) }}
            />
            <button
              onClick={handleFavoriteClick}
              disabled={isToggling}
              className={`absolute top-3 left-3 p-2 rounded-full transition-all duration-300 ${
                isFav 
                  ? 'bg-[#111] text-white' 
                  : 'bg-white/90 text-[#111] hover:bg-[#111] hover:text-white'
              } ${isToggling ? 'opacity-50 cursor-not-allowed' : ''}`}
            >
              <Heart className={`w-5 h-5 ${isFav ? 'fill-current' : ''}`} />
            </button>
            <div className="absolute top-3 right-3 bg-kaira text-white px-2.5 py-1 text-xs font-jost uppercase tracking-wide">
              Sale
            </div>
          </div>
          <div className="p-4">
            <h3 className="font-marcellus text-[#111] mb-2 line-clamp-2 group-hover:text-kaira transition-colors">
              {product.productName}
            </h3>
            <div className="flex items-center justify-between">
              <p className="font-jost text-lg font-medium text-[#111]">{formatPrice(product.price)}</p>
              <div className="flex items-center gap-1 text-gray-500">
                <Star className="w-4 h-4 fill-current text-amber-400" />
                <span className="text-sm font-jost">4.5</span>
              </div>
            </div>
          </div>
        </div>
      </Link>
    </motion.div>
  )
}

export default HomePage
