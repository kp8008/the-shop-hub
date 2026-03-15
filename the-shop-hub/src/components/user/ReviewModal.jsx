import { useState, useEffect } from 'react'
import { Star, X, ImagePlus, Trash2 } from 'lucide-react'
import api, { getErrorMessage, getImageUrl } from '../../utils/api'
import toast from 'react-hot-toast'
import useAuthStore from '../../store/useAuthStore'

const ReviewModal = ({ isOpen, onClose, productId, productName, existingReview: existingReviewProp, onSuccess }) => {
  const { user } = useAuthStore()
  const [rating, setRating] = useState(0)
  const [hoverRating, setHoverRating] = useState(0)
  const [title, setTitle] = useState('')
  const [comment, setComment] = useState('')
  const [imageFile, setImageFile] = useState(null)
  const [imagePreview, setImagePreview] = useState(null)
  const [existingImageUrl, setExistingImageUrl] = useState(null)
  const [submitting, setSubmitting] = useState(false)
  const [deleting, setDeleting] = useState(false)
  const [existingReviewFetched, setExistingReviewFetched] = useState(null)

  const effectiveReview = existingReviewProp || existingReviewFetched
  const isEditMode = !!effectiveReview

  useEffect(() => {
    if (!isOpen) {
      setExistingReviewFetched(null)
      return
    }
    if (existingReviewProp || !productId || !user) return
    const fetchMyReview = async () => {
      try {
        const res = await api.get(`/ProductReview/product/${productId}`)
        const reviews = res.data?.reviews ?? []
        const userId = user?.userId ?? user?.userID
        const myReview = reviews.find(r => (r.userID ?? r.UserID) === userId)
        if (myReview) {
          setExistingReviewFetched({
            reviewID: myReview.reviewID ?? myReview.ReviewID,
            productID: productId,
            rating: myReview.rating ?? myReview.Rating,
            title: myReview.title ?? myReview.Title,
            comment: myReview.comment ?? myReview.Comment,
            image: myReview.image ?? myReview.Image
          })
        } else {
          setExistingReviewFetched(null)
        }
      } catch {
        setExistingReviewFetched(null)
      }
    }
    fetchMyReview()
  }, [isOpen, productId, user, existingReviewProp])

  useEffect(() => {
    if (!isOpen) return
    if (effectiveReview) {
      setRating(effectiveReview.rating ?? 0)
      setTitle(effectiveReview.title ?? '')
      setComment(effectiveReview.comment ?? '')
      const img = effectiveReview.image ?? null
      setExistingImageUrl(img)
      setImagePreview(img ? (getImageUrl(img) || img) : null)
      setImageFile(null)
    } else {
      setRating(0)
      setTitle('')
      setComment('')
      setExistingImageUrl(null)
      setImagePreview(null)
      setImageFile(null)
    }
  }, [isOpen, effectiveReview])

  const resetForm = () => {
    setRating(0)
    setHoverRating(0)
    setTitle('')
    setComment('')
    setImageFile(null)
    setImagePreview(null)
    setExistingImageUrl(null)
    setExistingReviewFetched(null)
  }

  const handleClose = () => {
    resetForm()
    onClose()
  }

  const handleImageChange = (e) => {
    const file = e.target.files?.[0]
    if (!file) return
    const allowed = ['image/jpeg', 'image/png', 'image/gif', 'image/webp']
    if (!allowed.includes(file.type)) {
      toast.error('Only JPG, PNG, GIF, WebP allowed')
      return
    }
    if (file.size > 5 * 1024 * 1024) {
      toast.error('Max 5MB')
      return
    }
    setImageFile(file)
    setImagePreview(URL.createObjectURL(file))
  }

  const removeImage = () => {
    setImageFile(null)
    if (imagePreview && imagePreview.startsWith('blob:')) URL.revokeObjectURL(imagePreview)
    setImagePreview(null)
    setExistingImageUrl(null)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (rating < 1 || rating > 5) {
      toast.error('Please select a rating (1-5 stars)')
      return
    }
    if (!title.trim()) {
      toast.error('Please enter a title')
      return
    }
    if (!comment.trim()) {
      toast.error('Please enter your review')
      return
    }
    setSubmitting(true)
    try {
      let imageUrl = existingImageUrl || null
      if (imageFile) {
        const formData = new FormData()
        formData.append('file', imageFile)
        const uploadRes = await api.post('/Upload/ReviewImage', formData)
        imageUrl = uploadRes.data?.imageUrl || null
      }
      if (isEditMode && effectiveReview?.reviewID) {
        await api.put(`/ProductReview/${effectiveReview.reviewID}`, {
          reviewID: effectiveReview.reviewID,
          productID: productId ?? effectiveReview.productID,
          rating,
          title: title.trim(),
          comment: comment.trim(),
          image: imageUrl || undefined
        })
        toast.success('Review updated successfully!')
      } else {
        await api.post('/ProductReview', {
          productID: productId,
          rating,
          title: title.trim(),
          comment: comment.trim(),
          image: imageUrl || undefined
        })
        toast.success('Review submitted successfully!')
      }
      handleClose()
      onSuccess?.()
    } catch (err) {
      toast.error(getErrorMessage(err))
    } finally {
      setSubmitting(false)
    }
  }

  const handleDelete = async () => {
    if (!isEditMode || !effectiveReview?.reviewID) return
    if (!window.confirm('Delete your review? You can write a new one after deleting.')) return
    setDeleting(true)
    try {
      await api.delete(`/ProductReview/${effectiveReview.reviewID}`)
      toast.success('Review deleted. You can write a new review now.')
      handleClose()
      onSuccess?.()
    } catch (err) {
      toast.error(getErrorMessage(err))
    } finally {
      setDeleting(false)
    }
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50" onClick={handleClose}>
      <div
        className="bg-white rounded-2xl shadow-xl max-w-md w-full max-h-[90vh] overflow-y-auto"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-center justify-between p-4 border-b">
          <h3 className="text-xl font-bold">{isEditMode ? 'Edit your review' : 'Write a review'}</h3>
          <button type="button" onClick={handleClose} className="p-2 hover:bg-gray-100 rounded-full">
            <X className="w-5 h-5" />
          </button>
        </div>
        {productName && (
          <p className="px-4 pb-2 text-gray-600 text-sm">Product: {productName}</p>
        )}
        <form onSubmit={handleSubmit} className="p-4 space-y-4">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">Rating (1-5 stars) *</label>
            <div className="flex gap-1">
              {[1, 2, 3, 4, 5].map((star) => (
                <button
                  key={star}
                  type="button"
                  className="p-1 focus:outline-none"
                  onMouseEnter={() => setHoverRating(star)}
                  onMouseLeave={() => setHoverRating(0)}
                  onClick={() => setRating(star)}
                >
                  <Star
                    className={`w-8 h-8 ${
                      (hoverRating || rating) >= star
                        ? 'text-yellow-500 fill-yellow-500'
                        : 'text-gray-300'
                    }`}
                  />
                </button>
              ))}
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Title *</label>
            <input
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              maxLength={100}
              placeholder="Short summary"
              className="w-full px-3 py-2 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Review note *</label>
            <textarea
              value={comment}
              onChange={(e) => setComment(e.target.value)}
              maxLength={500}
              rows={3}
              placeholder="Share your experience..."
              className="w-full px-3 py-2 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
              required
            />
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-1">Product image (optional)</label>
            <div className="flex items-center gap-2">
              <label className="flex items-center gap-2 px-3 py-2 border rounded-lg cursor-pointer hover:bg-gray-50">
                <ImagePlus className="w-5 h-5 text-gray-500" />
                <span className="text-sm">Choose image</span>
                <input
                  type="file"
                  accept="image/jpeg,image/png,image/gif,image/webp"
                  className="hidden"
                  onChange={handleImageChange}
                />
              </label>
              {(imagePreview || existingImageUrl) && (
                <div className="relative inline-block">
                  <img src={imagePreview || getImageUrl(existingImageUrl) || existingImageUrl} alt="Preview" className="w-16 h-16 object-cover rounded" />
                  <button
                    type="button"
                    onClick={removeImage}
                    className="absolute -top-1 -right-1 w-5 h-5 bg-red-500 text-white rounded-full flex items-center justify-center text-xs"
                  >
                    ×
                  </button>
                </div>
              )}
            </div>
          </div>
          <div className="flex gap-2 pt-2">
            {isEditMode && (
              <button
                type="button"
                onClick={handleDelete}
                disabled={deleting}
                className="flex items-center gap-1 px-3 py-2 text-red-600 border border-red-200 rounded-lg hover:bg-red-50 disabled:opacity-50"
              >
                <Trash2 className="w-4 h-4" /> {deleting ? 'Deleting...' : 'Delete review'}
              </button>
            )}
            <button
              type="button"
              onClick={handleClose}
              className="flex-1 py-2 border rounded-lg font-medium hover:bg-gray-50"
            >
              Cancel
            </button>
            <button
              type="submit"
              disabled={submitting}
              className="flex-1 py-2 bg-[#111] text-white text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors disabled:opacity-50"
            >
              {submitting ? (isEditMode ? 'Updating...' : 'Submitting...') : (isEditMode ? 'Update review' : 'Submit review')}
            </button>
          </div>
        </form>
      </div>
    </div>
  )
}

export default ReviewModal
