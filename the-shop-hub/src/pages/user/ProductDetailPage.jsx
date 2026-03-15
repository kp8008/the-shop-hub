import { useState, useEffect, useRef } from 'react'
import { useParams, Link, useNavigate } from 'react-router-dom'
import { motion } from 'framer-motion'
import { Star, Heart, ShoppingCart, Truck, Shield, ArrowLeft, Plus, Minus, MessageSquare, Edit3, Trash2 } from 'lucide-react'
import api, { getImageUrl, getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import useCartStore from '../../store/useCartStore'
import useFavoriteStore from '../../store/useFavoriteStore'
import useAuthStore from '../../store/useAuthStore'
import { useGuestSignUpPrompt } from '../../context/GuestSignUpContext'
import ReviewModal from '../../components/user/ReviewModal'
import toast from 'react-hot-toast'

const ProductDetailPage = () => {
  const { id } = useParams()
  const navigate = useNavigate()
  const { showSignUpPrompt } = useGuestSignUpPrompt() || {}
  const [product, setProduct] = useState(null)
  const [selectedImage, setSelectedImage] = useState(null)
  const [currentIndex, setCurrentIndex] = useState(0)
  const [quantity, setQuantity] = useState(1)
  const [loading, setLoading] = useState(true)
  const [relatedProducts, setRelatedProducts] = useState([])
  const [isTogglingFavorite, setIsTogglingFavorite] = useState(false)
  const [reviews, setReviews] = useState([])
  const [averageRating, setAverageRating] = useState(0)
  const [reviewsCount, setReviewsCount] = useState(0)
  const [canReview, setCanReview] = useState(false)
  const [reviewModalOpen, setReviewModalOpen] = useState(false)
  const [reviewModalExistingReview, setReviewModalExistingReview] = useState(null)
  const intervalRef = useRef(null)

  const { addItem } = useCartStore()
  const { isAuthenticated, user } = useAuthStore()
  const { isFavorite, toggleFavorite, fetchFavorites } = useFavoriteStore()

  const userId = user?.userId ?? user?.userID
  const myReview = reviews.find(r => (r.userID ?? r.UserID) === userId)
  const isCustomer = user?.userTypeName?.toLowerCase() === 'customer' || user?.userTypeId === 2

  useEffect(() => {
    fetchProduct()
    if (isAuthenticated) {
      fetchFavorites()
    }
  }, [id, isAuthenticated])

  useEffect(() => {
    if (!id) return
    fetchReviews()
    if (isAuthenticated && isCustomer) {
      checkCanReview()
    }
  }, [id, isAuthenticated, isCustomer])

  const fetchReviews = async () => {
    try {
      const res = await api.get(`/ProductReview/product/${id}`)
      setReviews(res.data?.reviews ?? [])
      setAverageRating(res.data?.averageRating ?? 0)
      setReviewsCount(res.data?.count ?? 0)
    } catch {
      setReviews([])
      setAverageRating(0)
      setReviewsCount(0)
    }
  }

  const checkCanReview = async () => {
    try {
      const res = await api.get(`/ProductReview/CanReview/${id}`)
      setCanReview(!!res.data?.canReview)
    } catch {
      setCanReview(false)
    }
  }

  // Auto-slide images every 2 seconds
  useEffect(() => {
    if (product?.gallery && product.gallery.length > 1) {
      intervalRef.current = setInterval(() => {
        setCurrentIndex((prevIndex) => {
          const nextIndex = (prevIndex + 1) % product.gallery.length
          setSelectedImage(product.gallery[nextIndex])
          return nextIndex
        })
      }, 2000)

      return () => {
        if (intervalRef.current) {
          clearInterval(intervalRef.current)
        }
      }
    }
  }, [product?.gallery])

  // Handle manual image selection
  const handleImageSelect = (img, idx) => {
    // Reset the interval when user manually selects
    if (intervalRef.current) {
      clearInterval(intervalRef.current)
    }
    setSelectedImage(img)
    setCurrentIndex(idx)
    
    // Restart the interval after manual selection
    if (product?.gallery && product.gallery.length > 1) {
      intervalRef.current = setInterval(() => {
        setCurrentIndex((prevIndex) => {
          const nextIndex = (prevIndex + 1) % product.gallery.length
          setSelectedImage(product.gallery[nextIndex])
          return nextIndex
        })
      }, 2000)
    }
  }

  const fetchProduct = async () => {
    try {
      setLoading(true)
      const response = await api.get(`/Product/${id}`)
      const p = response.data
      const mainImage = getImageUrl(p.image ?? p.Image, 1200)

      // Backend returns Images as array of { ProductImageID, ImageUrl } or strings
      const rawImages = p.Images ?? p.images ?? []
      const additionalUrls = rawImages
        .map(item => {
          const raw = typeof item === 'string' ? item : (item?.ImageUrl ?? item?.imageUrl)
          if (!raw || typeof raw !== 'string') return null
          return getImageUrl(raw, 1200) || (raw.startsWith('http') ? raw : null)
        })
        .filter(Boolean)
      
      const mappedProduct = {
        productId: p.productId ?? p.productID ?? p.ProductID,
        productName: p.productName ?? p.ProductName,
        description: p.description ?? p.Description ?? p.productName ?? p.ProductName ?? '',
        price: p.price ?? p.Price ?? 0,
        imageUrl: mainImage,
        images: additionalUrls,
        categoryId: p.categoryId ?? p.categoryID ?? p.CategoryID
      }
      
      // Merge main image with additional images for the gallery (only valid URLs)
      let gallery = []
      if (mappedProduct.imageUrl) {
        gallery.push(mappedProduct.imageUrl)
      }
      const uniqueAdditional = additionalUrls.filter(url => url && url !== mappedProduct.imageUrl)
      gallery = [...gallery, ...uniqueAdditional]
      mappedProduct.gallery = gallery

      setProduct(mappedProduct)
      setSelectedImage(gallery[0] || getPlaceholderImageUrl(600, 600))
      
      // Fetch related products
      const allProducts = await api.get('/Product')
      const mappedRelated = allProducts.data.map(x => ({
        productId: x.productId ?? x.productID ?? x.ProductID,
        productName: x.productName ?? x.ProductName,
        description: x.description ?? x.Description ?? '',
        price: x.price ?? x.Price ?? 0,
        imageUrl: getImageUrl(x.image ?? x.Image),
        categoryId: x.categoryId ?? x.categoryID ?? x.CategoryID
      }))

      const related = mappedRelated
        .filter(x => x.categoryId === mappedProduct.categoryId && x.productId !== mappedProduct.productId)
        .slice(0, 4)
      setRelatedProducts(related)
    } catch (error) {
      console.error('Error fetching product:', error)
      toast.error('Failed to load product')
    } finally {
      setLoading(false)
    }
  }

  const handleAddToCart = () => {
    if (!isAuthenticated) {
      showSignUpPrompt?.()
      return
    }
    addItem(product, quantity)
    toast.success(`Added ${quantity} item(s) to cart!`)
  }

  const handleToggleFavorite = async () => {
    if (!isAuthenticated) {
      showSignUpPrompt?.()
      return
    }
    
    setIsTogglingFavorite(true)
    const wasFavorite = isFavorite(product.productId)
    try {
      await toggleFavorite(product.productId)
      toast.success(wasFavorite ? 'Removed from wishlist' : 'Added to wishlist')
    } catch (error) {
      toast.error('Failed to update wishlist')
    } finally {
      setIsTogglingFavorite(false)
    }
  }

  if (loading) {
    return (
      <div className="py-16 min-h-screen bg-kaira-light">
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <div className="animate-pulse">
            <div className="h-8 bg-gray-200 rounded w-1/4 mb-8"></div>
            <div className="grid lg:grid-cols-2 gap-12">
              <div className="aspect-square bg-gray-200 rounded-2xl"></div>
              <div className="space-y-4">
                <div className="h-8 bg-gray-200 rounded"></div>
                <div className="h-4 bg-gray-200 rounded w-3/4"></div>
                <div className="h-20 bg-gray-200 rounded"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    )
  }

  if (!product) {
    return (
      <div className="pt-[var(--nav-height,7rem)] pb-16 min-h-screen flex items-center justify-center bg-kaira-light">
        <div className="text-center">
          <h2 className="font-marcellus text-2xl text-[#111] mb-4">Product not found</h2>
          <Link to="/products" className="bg-[#111] text-white px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors inline-block">
            Back to Products
          </Link>
        </div>
      </div>
    )
  }

  return (
    <div className="py-16 min-h-screen bg-kaira-light">
      <div className="max-w-[1800px] mx-auto px-4 md:px-6">
        {/* Breadcrumb - Kaira */}
        <Link to="/products" className="inline-flex items-center space-x-2 font-jost text-gray-600 hover:text-kaira mb-8 nav-link-kaira">
          <ArrowLeft className="w-4 h-4" />
          <span>Back to Products</span>
        </Link>

        {/* Product Details */}
        <div className="grid lg:grid-cols-2 gap-12 mb-16">
          {/* Images */}
          <motion.div
            initial={{ opacity: 0, x: -50 }}
            animate={{ opacity: 1, x: 0 }}
            className="space-y-4"
          >
            <div className="aspect-square overflow-hidden bg-white border border-gray-200 group relative">
              <motion.img
                key={selectedImage}
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                transition={{ duration: 0.3 }}
                src={selectedImage || product.imageUrl || getPlaceholderImageUrl(600, 600)}
                alt={product.productName}
                className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                onError={(e) => { e.target.onerror = null; e.target.src = getPlaceholderImageUrl(600, 600) }}
              />
            </div>
            
            {/* Thumbnails */}
            {product.gallery && product.gallery.length > 1 && (
               <div className="flex gap-4 overflow-x-auto py-2 px-1">
                 {product.gallery.filter(Boolean).map((img, idx) => (
                   <div 
                     key={idx} 
                     onClick={() => handleImageSelect(img, idx)}
                     className={`flex-shrink-0 w-20 h-20 overflow-hidden cursor-pointer border-2 transition-all bg-white ${
                       selectedImage === img ? 'border-kaira shadow-sm scale-105' : 'border-transparent hover:border-gray-300 opacity-70 hover:opacity-100'
                     }`}
                   >
                     <img
                       src={img}
                       alt={`${product.productName} - ${idx + 1}`}
                       className="w-full h-full object-cover"
                       onError={(e) => { e.target.onerror = null; e.target.src = getPlaceholderImageUrl(80, 80) }}
                     />
                   </div>
                 ))}
               </div>
            )}
          </motion.div>

          {/* Info */}
          <motion.div
            initial={{ opacity: 0, x: 50 }}
            animate={{ opacity: 1, x: 0 }}
            className="space-y-6"
          >
            <div>
              <h1 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-4">{product.productName}</h1>
              
              <div className="flex items-center space-x-4 mb-4 flex-wrap gap-2">
                <div className="flex items-center space-x-1 text-amber-400">
                  {[1, 2, 3, 4, 5].map((star) => (
                    <Star
                      key={star}
                      className={`w-5 h-5 ${star <= Math.round(averageRating) ? 'fill-current' : ''}`}
                    />
                  ))}
                </div>
                <span className="font-jost text-gray-600">
                  ({averageRating > 0 ? averageRating.toFixed(1) : '0'}) {reviewsCount} review{reviewsCount !== 1 ? 's' : ''}
                </span>
                {isAuthenticated && isCustomer && (canReview || myReview) && (
                  <button
                    type="button"
                    onClick={() => {
                      setReviewModalExistingReview(myReview ? {
                        reviewID: myReview.reviewID ?? myReview.ReviewID,
                        productID: parseInt(id, 10),
                        rating: myReview.rating ?? myReview.Rating,
                        title: myReview.title ?? myReview.Title,
                        comment: myReview.comment ?? myReview.Comment,
                        image: myReview.image ?? myReview.Image
                      } : null)
                      setReviewModalOpen(true)
                    }}
                    className="ml-2 text-sm font-jost text-kaira hover:underline flex items-center gap-1"
                  >
                    <MessageSquare className="w-4 h-4" /> {myReview ? 'Edit your review' : 'Write a review'}
                  </button>
                )}
              </div>

              <p className="font-marcellus text-4xl text-[#111] mb-6">{formatPrice(product.price)}</p>

              <p className="font-jost text-gray-600 text-lg leading-relaxed mb-6">
                {product.description || 'Experience premium quality with this amazing product. Designed with attention to detail and built to last.'}
              </p>
            </div>

            {/* Quantity Selector */}
            <div className="flex items-center space-x-4">
              <span className="font-semibold">Quantity:</span>
              <div className="flex items-center space-x-3 bg-white rounded-lg shadow-md p-2">
                <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.9 }}
                  onClick={() => setQuantity(Math.max(1, quantity - 1))}
                  className="w-8 h-8 flex items-center justify-center bg-gray-100 rounded-lg hover:bg-gray-200"
                >
                  <Minus className="w-4 h-4" />
                </motion.button>
                <span className="w-12 text-center font-semibold">{quantity}</span>
                <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.9 }}
                  onClick={() => setQuantity(quantity + 1)}
                  className="w-8 h-8 flex items-center justify-center bg-gray-100 rounded-lg hover:bg-gray-200"
                >
                  <Plus className="w-4 h-4" />
                </motion.button>
              </div>
            </div>

            {/* Action Buttons - Kaira */}
            <div className="flex flex-wrap gap-4">
              <motion.button
                whileHover={{ scale: 1.02 }}
                whileTap={{ scale: 0.98 }}
                onClick={handleAddToCart}
                className="flex-1 bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex items-center justify-center space-x-2"
              >
                <ShoppingCart className="w-5 h-5" />
                <span>Add to Cart</span>
              </motion.button>
              
              <motion.button
                whileHover={{ scale: 1.05 }}
                whileTap={{ scale: 0.95 }}
                onClick={handleToggleFavorite}
                disabled={isTogglingFavorite}
                className={`w-14 h-14 border-2 flex items-center justify-center transition-colors ${
                  isFavorite(product.productId)
                    ? 'bg-[#111] border-[#111] text-white'
                    : 'bg-white border-gray-200 hover:border-[#111] hover:text-[#111]'
                } ${isTogglingFavorite ? 'opacity-50 cursor-not-allowed' : ''}`}
              >
                <Heart className={`w-6 h-6 ${isFavorite(product.productId) ? 'fill-current' : ''}`} />
              </motion.button>
            </div>

            {/* Features - Kaira */}
            <div className="grid grid-cols-2 gap-4 pt-6 border-t border-gray-200">
              <div className="flex items-center space-x-3">
                <div className="w-12 h-12 bg-kaira-light flex items-center justify-center text-kaira">
                  <Truck className="w-6 h-6" />
                </div>
                <div>
                  <p className="font-marcellus text-[#111]">Free Delivery</p>
                  <p className="text-sm font-jost text-gray-600">On orders over ₹500</p>
                </div>
              </div>
              
              <div className="flex items-center space-x-3">
                <div className="w-12 h-12 bg-kaira-light flex items-center justify-center text-kaira">
                  <Shield className="w-6 h-6" />
                </div>
                <div>
                  <p className="font-marcellus text-[#111]">Secure Payment</p>
                  <p className="text-sm font-jost text-gray-600">100% protected</p>
                </div>
              </div>
            </div>
          </motion.div>
        </div>

        {/* Reviews section - Kaira */}
        <div className="mb-16">
          <h2 className="font-marcellus text-2xl text-[#111] mb-4">Customer reviews</h2>
          {reviews.length === 0 ? (
            <p className="font-jost text-gray-500">No reviews yet. Be the first to review after your order is delivered!</p>
          ) : (
            <div className="space-y-4">
              {myReview && (
                <div className="bg-kaira-light border border-kaira p-4">
                  <div className="flex items-center justify-between mb-2">
                    <span className="text-sm font-jost font-semibold text-kaira-dark">Your review</span>
                    <div className="flex gap-2">
                      <button
                        type="button"
                        onClick={() => {
                          setReviewModalExistingReview({
                            reviewID: myReview.reviewID ?? myReview.ReviewID,
                            productID: parseInt(id, 10),
                            rating: myReview.rating ?? myReview.Rating,
                            title: myReview.title ?? myReview.Title,
                            comment: myReview.comment ?? myReview.Comment,
                            image: myReview.image ?? myReview.Image
                          })
                          setReviewModalOpen(true)
                        }}
                        className="text-sm font-jost text-kaira hover:underline flex items-center gap-1"
                      >
                        <Edit3 className="w-4 h-4" /> Edit
                      </button>
                      <button
                        type="button"
                        onClick={async () => {
                          if (!window.confirm('Delete your review? You can write a new one after.')) return
                          try {
                            await api.delete(`/ProductReview/${myReview.reviewID ?? myReview.ReviewID}`)
                            toast.success('Review deleted.')
                            fetchReviews()
                            checkCanReview()
                          } catch (err) {
                            toast.error(err?.response?.data?.message || 'Failed to delete')
                          }
                        }}
                        className="text-sm font-medium text-red-600 hover:text-red-800 flex items-center gap-1"
                      >
                        <Trash2 className="w-4 h-4" /> Delete
                      </button>
                    </div>
                  </div>
                  <div className="flex text-amber-400 mb-1">
                    {[1, 2, 3, 4, 5].map((s) => (
                      <Star key={s} className={`w-4 h-4 ${s <= (myReview.rating ?? myReview.Rating) ? 'fill-current' : ''}`} />
                    ))}
                  </div>
                  <p className="font-marcellus text-[#111]">{myReview.title ?? myReview.Title}</p>
                  <p className="font-jost text-gray-600 text-sm mt-1">{myReview.comment ?? myReview.Comment}</p>
                  {(myReview.image ?? myReview.Image) && (
                    <img
                      src={getImageUrl(myReview.image ?? myReview.Image) || (myReview.image ?? myReview.Image)}
                      alt="Review"
                      className="w-24 h-24 object-cover rounded-lg mt-2"
                    />
                  )}
                  <p className="text-xs text-gray-500 mt-2">{new Date(myReview.created ?? myReview.Created).toLocaleDateString()}</p>
                </div>
              )}
              {reviews.map((r) => {
                const isMine = (r.userID ?? r.UserID) === userId
                if (isMine) return null
                return (
                  <div key={r.reviewID ?? r.ReviewID} className="bg-white p-4 border border-gray-200">
                    <div className="flex items-center gap-2 mb-2">
                      <div className="flex text-amber-400">
                        {[1, 2, 3, 4, 5].map((s) => (
                          <Star
                            key={s}
                            className={`w-4 h-4 ${s <= (r.rating ?? r.Rating) ? 'fill-current' : ''}`}
                          />
                        ))}
                      </div>
                      <span className="font-marcellus text-[#111]">{r.title ?? r.Title}</span>
                      <span className="font-jost text-gray-400 text-sm">
                        {r.userName ?? r.UserName} · {new Date(r.created ?? r.Created).toLocaleDateString()}
                      </span>
                    </div>
                    <p className="font-jost text-gray-600 text-sm mb-2">{r.comment ?? r.Comment}</p>
                    {(r.image ?? r.Image) && (
                      <img
                        src={getImageUrl(r.image ?? r.Image) || (r.image ?? r.Image)}
                        alt="Review"
                        className="w-24 h-24 object-cover rounded-lg"
                      />
                    )}
                  </div>
                )
              })}
            </div>
          )}
        </div>

        <ReviewModal
          isOpen={reviewModalOpen}
          onClose={() => { setReviewModalOpen(false); setReviewModalExistingReview(null) }}
          productId={parseInt(id, 10)}
          productName={product?.productName}
          existingReview={reviewModalExistingReview}
          onSuccess={() => { fetchReviews(); checkCanReview(); setReviewModalExistingReview(null) }}
        />

        {/* Related Products - Kaira */}
        {relatedProducts.length > 0 && (
          <div>
            <h2 className="font-marcellus text-2xl md:text-3xl text-[#111] mb-8">Related Products</h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
              {relatedProducts.map((relatedProduct, index) => (
                <motion.div
                  key={relatedProduct.productId}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ delay: index * 0.1 }}
                  whileHover={{ y: -6 }}
                >
                  <Link to={`/products/${relatedProduct.productId}`}>
                    <div className="bg-white border border-gray-200 overflow-hidden hover:border-kaira hover:shadow-lg transition-all duration-300">
                      <div className="image-zoom-effect aspect-square overflow-hidden">
                        <img
                          src={relatedProduct.imageUrl || getPlaceholderImageUrl(300, 300)}
                          alt={relatedProduct.productName}
                          className="w-full h-full object-cover"
                        />
                      </div>
                      <div className="p-4">
                        <h3 className="font-marcellus text-[#111] mb-2 line-clamp-2 hover:text-kaira">{relatedProduct.productName}</h3>
                        <p className="font-jost text-lg font-medium text-[#111]">{formatPrice(relatedProduct.price)}</p>
                      </div>
                    </div>
                  </Link>
                </motion.div>
              ))}
            </div>
          </div>
        )}
      </div>
    </div>
  )
}

export default ProductDetailPage
