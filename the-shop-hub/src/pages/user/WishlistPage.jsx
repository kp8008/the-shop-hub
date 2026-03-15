import { useState, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { motion } from 'framer-motion'
import { Heart, ShoppingCart, Trash2, ArrowLeft } from 'lucide-react'
import useFavoriteStore from '../../store/useFavoriteStore'
import useCartStore from '../../store/useCartStore'
import useAuthStore from '../../store/useAuthStore'
import { formatPrice } from '../../utils/formatPrice'
import { getImageUrl, getPlaceholderImageUrl } from '../../utils/api'
import toast from 'react-hot-toast'

const WishlistPage = () => {
  const navigate = useNavigate()
  const { isAuthenticated } = useAuthStore()
  const { favorites, loading, fetchFavorites, removeFavorite } = useFavoriteStore()
  const { addItem } = useCartStore()
  const [removingId, setRemovingId] = useState(null)

  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login')
      return
    }
    fetchFavorites()
  }, [isAuthenticated])

  const handleRemove = async (productId) => {
    setRemovingId(productId)
    try {
      await removeFavorite(productId)
      toast.success('Removed from wishlist')
    } catch (error) {
      toast.error('Failed to remove from wishlist')
    } finally {
      setRemovingId(null)
    }
  }

  const handleAddToCart = (item) => {
    const product = {
      productId: item.productID,
      productName: item.productName,
      price: item.productPrice,
      imageUrl: getImageUrl(item.productImage)
    }
    addItem(product, 1)
    toast.success('Added to cart!')
  }

  if (loading) {
    return (
      <div className="py-16 min-h-screen bg-kaira-light">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <div className="animate-pulse space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white border border-gray-200 p-6 flex gap-6">
                <div className="w-32 h-32 bg-gray-200"></div>
                <div className="flex-1 space-y-3">
                  <div className="h-6 bg-gray-200 rounded w-1/3"></div>
                  <div className="h-4 bg-gray-200 rounded w-1/4"></div>
                  <div className="h-8 bg-gray-200 rounded w-24"></div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    )
  }

  return (
    <div className="py-16 min-h-screen bg-kaira-light">
      <div className="max-w-[1800px] mx-auto px-4 md:px-6">
        {/* Header - Kaira */}
        <div className="flex items-center justify-between mb-8">
          <div>
            <Link 
              to="/products" 
              className="inline-flex items-center font-jost text-gray-600 hover:text-kaira mb-4 nav-link-kaira"
            >
              <ArrowLeft className="w-4 h-4 mr-2" />
              Continue Shopping
            </Link>
            <h1 className="font-marcellus text-3xl text-[#111] flex items-center gap-3">
              <Heart className="w-8 h-8 text-kaira fill-current" />
              My Wishlist
              <span className="text-lg font-normal font-jost text-gray-500">
                ({favorites.length} {favorites.length === 1 ? 'item' : 'items'})
              </span>
            </h1>
          </div>
        </div>

        {/* Empty State */}
        {favorites.length === 0 ? (
          <motion.div
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            className="text-center py-16"
          >
            <Heart className="w-24 h-24 text-gray-300 mx-auto mb-6" />
            <h2 className="font-marcellus text-2xl text-[#111] mb-4">Your wishlist is empty</h2>
            <p className="font-jost text-gray-600 mb-8">
              Start adding products you love to your wishlist!
            </p>
            <Link
              to="/products"
              className="inline-flex items-center px-8 py-3 bg-[#111] text-white text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
            >
              Browse Products
            </Link>
          </motion.div>
        ) : (
          <div className="space-y-4">
            {favorites.map((item, index) => (
              <motion.div
                key={item.favoriteID}
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ delay: index * 0.1 }}
                className="bg-white border border-gray-200 p-6 hover:border-kaira transition-colors"
              >
                <div className="flex flex-col sm:flex-row gap-6">
                  {/* Product Image */}
                  <Link to={`/products/${item.productID}`} className="flex-shrink-0">
                    <div className="w-32 h-32 overflow-hidden bg-kaira-light">
                      <img
                        src={getImageUrl(item.productImage) || getPlaceholderImageUrl(128, 128)}
                        alt={item.productName}
                        className="w-full h-full object-cover hover:scale-105 transition-transform"
                        onError={(e) => { e.target.onerror = null; e.target.src = getPlaceholderImageUrl(128, 128) }}
                      />
                    </div>
                  </Link>

                  {/* Product Info */}
                  <div className="flex-1 flex flex-col justify-between">
                    <div>
                      <Link 
                        to={`/products/${item.productID}`}
                        className="text-xl font-marcellus text-[#111] hover:text-kaira transition-colors"
                      >
                        {item.productName}
                      </Link>
                      {item.categoryName && (
                        <p className="text-sm font-jost text-gray-500 mt-1">{item.categoryName}</p>
                      )}
                      {item.productCode && (
                        <p className="text-xs font-jost text-gray-400 mt-1">Code: {item.productCode}</p>
                      )}
                    </div>
                    <p className="font-jost text-xl font-medium text-[#111] mt-2">
                      {formatPrice(item.productPrice)}
                    </p>
                  </div>

                  {/* Actions */}
                  <div className="flex flex-col sm:flex-row sm:items-center gap-3">
                    <button
                      onClick={() => handleAddToCart(item)}
                      className="flex items-center justify-center gap-2 px-6 py-3 bg-[#111] text-white text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
                    >
                      <ShoppingCart className="w-5 h-5" />
                      Add to Cart
                    </button>
                    <button
                      onClick={() => handleRemove(item.productID)}
                      disabled={removingId === item.productID}
                      className={`flex items-center justify-center gap-2 px-4 py-3 border-2 border-red-500 text-red-500 rounded-lg font-semibold hover:bg-red-500 hover:text-white transition-colors ${
                        removingId === item.productID ? 'opacity-50 cursor-not-allowed' : ''
                      }`}
                    >
                      <Trash2 className="w-5 h-5" />
                      Remove
                    </button>
                  </div>
                </div>
              </motion.div>
            ))}
          </div>
        )}
      </div>
    </div>
  )
}

export default WishlistPage
