import { useState, useEffect, useRef } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { useLocation } from 'react-router-dom'
import { Plus, Edit, Trash2, X, Upload, Search, Filter } from 'lucide-react'
import api, { getImageUrl, getErrorMessage, getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import toast from 'react-hot-toast'

const AdminProducts = () => {
  const [products, setProducts] = useState([])
  const [categories, setCategories] = useState([])
  const [showModal, setShowModal] = useState(false)
  const [editingProduct, setEditingProduct] = useState(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [categoryFilter, setCategoryFilter] = useState('')
  const [uploading, setUploading] = useState(false)
  const [submitting, setSubmitting] = useState(false)
  const fileInputRef = useRef(null)
  const [formData, setFormData] = useState({
    productName: '',
    productCode: '',
    description: '',
    price: '',
    stockQuantity: '',
    categoryId: '',
    image: null,
    imageUrl: '',
    additionalImages: [],    // New files to upload
    existingAdditionalImages: [], // Existing image objects {ProductImageID, ImageUrl}
    imagesToDelete: []        // IDs of existing images to delete
  })

  const location = useLocation()

  useEffect(() => {
    fetchData()
    if (location.state?.openModal) {
      openModal()
    }
  }, [location.state])

  const fetchData = async () => {
    try {
      const [productsRes, categoriesRes] = await Promise.all([
        api.get('/Product/admin'),
        api.get('/Category')
      ])
      
      // Debug logging
      console.log('=== PRODUCTS API RESPONSE ===')
      console.log('First product:', productsRes.data[0])
      console.log('StockQuantity field:', productsRes.data[0]?.StockQuantity)
      console.log('stockQuantity field:', productsRes.data[0]?.stockQuantity)
      console.log('All fields:', Object.keys(productsRes.data[0] || {}))
      console.log('============================')
      
      setProducts(productsRes.data)
      setCategories(categoriesRes.data)
    } catch (error) {
      console.error('API Error:', error.response?.data || error.message)
      toast.error('Failed to load data')
    }
  }

  const handleImageUpload = async (file) => {
    if (!file) return null
    
    setUploading(true)
    const formData = new FormData()
    formData.append('file', file)
    
    try {
      const response = await api.post('/Upload', formData, {
        headers: { 'Content-Type': 'multipart/form-data' }
      })
      return response.data.imageUrl
    } catch (error) {
      toast.error('Failed to upload image')
      return null
    } finally {
      setUploading(false)
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    if (submitting) return
    setSubmitting(true)
    try {
      // Create FormData for file upload
      const formDataToSend = new FormData()
      
      // Add all fields with correct backend field names (PascalCase)
      formDataToSend.append('ProductName', formData.productName)
      formDataToSend.append('ProductCode', formData.productCode || `PROD-${Date.now()}`) // Generate code if not provided
      formDataToSend.append('Price', formData.price)
      formDataToSend.append('StockQuantity', formData.stockQuantity || '0')
      formDataToSend.append('CategoryID', formData.categoryId)
      formDataToSend.append('IsActive', 'true')
      formDataToSend.append('Image', formData.imageUrl || '') // Send current URL or empty string
      
      // Add file - use formData.image or get from file input ref (handles React state timing)
      const fileToUpload = formData.image ?? fileInputRef.current?.files?.[0]
      if (fileToUpload) {
        formDataToSend.append('DocumentFile', fileToUpload)
      }

      // Add additional images
      if (formData.additionalImages && formData.additionalImages.length > 0) {
        formData.additionalImages.forEach(file => {
           formDataToSend.append('AdditionalImages', file)
        })
      }

      if (editingProduct) {
        formDataToSend.append('ProductID', editingProduct.productID)
        
        // Add image IDs to delete
        if (formData.imagesToDelete && formData.imagesToDelete.length > 0) {
          formData.imagesToDelete.forEach(id => {
            formDataToSend.append('ImagesToDelete', id)
          })
        }
        
        // Don't set Content-Type - axios sets multipart/form-data with boundary automatically
        // Longer timeout when uploading multiple images (2 min)
        const opts = { timeout: 120000 }
        await api.put(`/Product/${editingProduct.productID}`, formDataToSend, opts)
        toast.success('Product updated successfully!')
      } else {
        const opts = { timeout: 120000 }
        await api.post('/Product', formDataToSend, opts)
        toast.success('Product added successfully!')
      }
      fetchData()
      closeModal()
    } catch (error) {
      console.error('Submit Error:', error.response?.data || error.message)
      toast.error(getErrorMessage(error))
    } finally {
      setSubmitting(false)
    }
  }

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this product?')) {
      try {
        await api.delete(`/Product/${id}`)
        toast.success('Product deleted successfully!')
        fetchData()
      } catch (error) {
        console.error('Delete Error:', error.response?.data || error.message)
        toast.error(getErrorMessage(error) || 'Failed to delete product')
      }
    }
  }

  const openModal = (product = null) => {
    if (product) {
      setEditingProduct(product)
      setFormData({
        productName: product.productName || product.ProductName,
        productCode: product.productCode || product.ProductCode || '',
        description: product.description || '',
        price: (product.price || product.Price || 0).toString(),
        stockQuantity: (product.stockQuantity || product.StockQuantity || 0).toString(),
        categoryId: (product.categoryID || product.CategoryID || '').toString(),
        image: null,
        imageUrl: product.image || product.Image || '',
        additionalImages: [],
        existingAdditionalImages: product.images || product.Images || [],
        imagesToDelete: []
      })
    } else {
      setEditingProduct(null)
      setFormData({
        productName: '',
        productCode: '',
        description: '',
        price: '',
        stockQuantity: '0',
        categoryId: '',
        image: null,
        imageUrl: '',
        additionalImages: [],
        existingAdditionalImages: []
      })
    }
    setShowModal(true)
  }

  const closeModal = () => {
    setShowModal(false)
    setEditingProduct(null)
  }

  const filteredProducts = products.filter(product => {
    const productName = product.productName || product.ProductName || ''
    const categoryID = product.categoryID || product.CategoryID
    const matchesSearch = productName.toLowerCase().includes(searchTerm.toLowerCase())
    const matchesCategory = categoryFilter === '' || categoryID?.toString() === categoryFilter
    return matchesSearch && matchesCategory
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <h1 className="font-marcellus text-3xl text-[#111]">Products Management</h1>
        <motion.button
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
          onClick={() => openModal()}
          className="bg-[#111] text-white px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Add Product</span>
        </motion.button>
      </div>

      {/* Search and Filter */}
      <div className="bg-white rounded-2xl p-6 shadow-md mb-6">
        <div className="flex flex-col md:flex-row gap-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search products..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none pl-10"
            />
          </div>
          <div className="relative">
            <Filter className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <select
              value={categoryFilter}
              onChange={(e) => setCategoryFilter(e.target.value)}
              className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none pl-10 pr-4"
            >
              <option value="">All Categories</option>
              {categories.map(cat => (
                <option key={cat.categoryID} value={cat.categoryID}>
                  {cat.categoryName}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Products Table */}
      <div className="bg-white border border-gray-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Image</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Name</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Category</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Price</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Stock</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Status</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {filteredProducts.map((product) => (
                <tr key={product.productID} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4">
                    <img
                      src={getImageUrl(product.image) || getPlaceholderImageUrl(50, 50)}
                      alt={product.productName}
                      className="w-12 h-12 object-cover rounded-lg bg-gray-100"
                      onError={(e) => { e.target.onerror = null; e.target.src = getPlaceholderImageUrl(50, 50) }}
                    />
                  </td>
                  <td className="px-6 py-4">
                    <div>
                      <p className="font-medium">{product.productName}</p>
                      <p className="text-sm text-gray-500 line-clamp-1">{product.productCode}</p>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <span className="px-3 py-1 bg-kaira-light text-kaira-dark text-sm font-jost font-medium">
                      {categories.find(c => c.categoryID === product.categoryID)?.categoryName || 'N/A'}
                    </span>
                  </td>
                  <td className="px-6 py-4 font-jost font-semibold text-[#111]">{formatPrice(product.price)}</td>
                  <td className="px-6 py-4">
                    <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                      (product.stockQuantity || product.StockQuantity || 0) > 10 
                        ? 'bg-green-100 text-green-700' 
                        : (product.stockQuantity || product.StockQuantity || 0) > 0 
                        ? 'bg-yellow-100 text-yellow-700' 
                        : 'bg-red-100 text-red-700'
                    }`}>
                      {product.stockQuantity || product.StockQuantity || 0} units
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                      product.isActive 
                        ? 'bg-green-100 text-green-700' 
                        : 'bg-gray-100 text-gray-700'
                    }`}>
                      {product.isActive ? 'Active' : 'Inactive'}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-2">
                      <motion.button
                        whileHover={{ scale: 1.1 }}
                        whileTap={{ scale: 0.9 }}
                        onClick={() => openModal(product)}
                        className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                      >
                        <Edit className="w-5 h-5" />
                      </motion.button>
                      <motion.button
                        whileHover={{ scale: 1.1 }}
                        whileTap={{ scale: 0.9 }}
                        onClick={() => handleDelete(product.productID)}
                        className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                      >
                        <Trash2 className="w-5 h-5" />
                      </motion.button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal */}
      <AnimatePresence>
        {showModal && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4"
            onClick={closeModal}
          >
            <motion.div
              initial={{ scale: 0.9, opacity: 0 }}
              animate={{ scale: 1, opacity: 1 }}
              exit={{ scale: 0.9, opacity: 0 }}
              onClick={(e) => e.stopPropagation()}
              className="bg-white rounded-2xl p-8 max-w-2xl w-full max-h-[90vh] overflow-y-auto"
            >
              <div className="flex items-center justify-between mb-6">
                <h2 className="font-marcellus text-2xl text-[#111]">
                  {editingProduct ? 'Edit Product' : 'Add New Product'}
                </h2>
                <button onClick={closeModal} className="p-2 hover:bg-gray-100 rounded-lg">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-semibold mb-2">Product Name</label>
                  <input
                    type="text"
                    required
                    value={formData.productName}
                    onChange={(e) => setFormData({ ...formData, productName: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold mb-2">Product Code</label>
                  <input
                    type="text"
                    value={formData.productCode}
                    onChange={(e) => setFormData({ ...formData, productCode: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    placeholder="Leave empty to auto-generate"
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold mb-2">Price</label>
                    <input
                      type="number"
                      step="0.01"
                      required
                      value={formData.price}
                      onChange={(e) => setFormData({ ...formData, price: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold mb-2">Stock Quantity</label>
                    <input
                      type="number"
                      required
                      min="0"
                      value={formData.stockQuantity}
                      onChange={(e) => setFormData({ ...formData, stockQuantity: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-semibold mb-2">Category</label>
                  <select
                    required
                    value={formData.categoryId}
                    onChange={(e) => setFormData({ ...formData, categoryId: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  >
                    <option value="">Select Category</option>
                    {categories.map(cat => (
                      <option key={cat.categoryID} value={cat.categoryID}>
                        {cat.categoryName}
                      </option>
                    ))}
                  </select>
                </div>

                <div>
                  <label className="block text-sm font-semibold mb-2">Product Images</label>
                  <p className="text-xs text-gray-500 mb-3">First image will be the main product image</p>
                  <div className="space-y-4">
                    <div className="flex items-center justify-center w-full">
                      <label className="flex flex-col items-center justify-center w-full h-32 border-2 border-gray-300 border-dashed rounded-lg cursor-pointer bg-gray-50 hover:bg-gray-100">
                        <div className="flex flex-col items-center justify-center pt-5 pb-6">
                          <Upload className="w-8 h-8 mb-4 text-gray-500" />
                          <p className="mb-2 text-sm text-gray-500">
                            <span className="font-semibold">Click to select multiple images</span>
                          </p>
                          <p className="text-xs text-gray-500">PNG, JPG or JPEG (MAX. 5MB each)</p>
                        </div>
                        <input
                          ref={fileInputRef}
                          type="file"
                          className="hidden"
                          accept="image/*"
                          multiple
                          onChange={(e) => {
                            if (e.target.files && e.target.files.length > 0) {
                              const files = Array.from(e.target.files);
                              // First file is main image, rest are additional
                              const mainImage = files[0];
                              const additionalFiles = files.slice(1);
                              setFormData(prev => ({ 
                                ...prev, 
                                image: mainImage,
                                additionalImages: [...prev.additionalImages, ...additionalFiles]
                              }));
                            }
                          }}
                        />
                      </label>
                    </div>

                    {/* All Images Preview */}
                    <div className="flex flex-wrap gap-4">
                      {/* Main Image */}
                      {(formData.image || formData.imageUrl) && (
                        <div className="relative group">
                          <div className="w-20 h-20 relative">
                            <img
                              src={formData.image ? URL.createObjectURL(formData.image) : formData.imageUrl}
                              alt="Main"
                              className="w-full h-full object-cover border-2 border-kaira"
                            />
                            <span className="absolute -top-2 -left-2 bg-[#111] text-white text-xs font-jost px-1.5 py-0.5">Main</span>
                          </div>
                          <button
                            type="button"
                            onClick={() => setFormData({ ...formData, image: null, imageUrl: '' })}
                            className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 shadow-md hover:bg-red-600"
                          >
                            <X className="w-3 h-3" />
                          </button>
                        </div>
                      )}

                      {/* Existing Additional Images */}
                      {formData.existingAdditionalImages && formData.existingAdditionalImages.map((img, idx) => (
                        <div key={`exist-${idx}`} className="relative group w-20 h-20">
                          <img src={getImageUrl(img.imageUrl)} alt={`Existing ${idx}`} className="w-full h-full object-cover rounded-lg border border-gray-200" />
                          <button
                            type="button"
                            onClick={() => {
                              setFormData(prev => ({
                                ...prev,
                                existingAdditionalImages: prev.existingAdditionalImages.filter((_, i) => i !== idx),
                                imagesToDelete: [...prev.imagesToDelete, img.productImageID]
                              }))
                            }}
                            className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 shadow-md hover:bg-red-600"
                          >
                            <X className="w-3 h-3" />
                          </button>
                        </div>
                      ))}
                      
                      {/* New Additional Images */}
                      {formData.additionalImages && formData.additionalImages.map((file, idx) => (
                        <div key={`new-${idx}`} className="relative group w-20 h-20">
                          <img src={URL.createObjectURL(file)} alt={`New ${idx}`} className="w-full h-full object-cover rounded-lg border border-gray-300" />
                          <button
                            type="button"
                            onClick={() => setFormData(prev => ({
                              ...prev,
                              additionalImages: prev.additionalImages.filter((_, i) => i !== idx)
                            }))}
                            className="absolute -top-2 -right-2 bg-red-500 text-white rounded-full p-1 shadow-md hover:bg-red-600"
                          >
                            <X className="w-3 h-3" />
                          </button>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>

                <div className="flex gap-4 pt-4">
                  <button type="button" onClick={closeModal} className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1">
                    Cancel
                  </button>
                  <button 
                    type="submit" 
                    disabled={uploading || submitting}
                    className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {submitting ? (editingProduct ? 'Updating...' : 'Adding...') : uploading ? 'Uploading...' : (editingProduct ? 'Update' : 'Add') + ' Product'}
                  </button>
                </div>
              </form>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}

export default AdminProducts
