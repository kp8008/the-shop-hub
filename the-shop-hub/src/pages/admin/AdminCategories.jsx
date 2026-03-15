import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Plus, Edit, Trash2, X } from 'lucide-react'
import api, { getErrorMessage } from '../../utils/api'
import toast from 'react-hot-toast'

const AdminCategories = () => {
  const [categories, setCategories] = useState([])
  const [showModal, setShowModal] = useState(false)
  const [editingCategory, setEditingCategory] = useState(null)
  const [formData, setFormData] = useState({
    categoryName: '',
    description: ''
  })

  useEffect(() => {
    fetchCategories()
  }, [])

  const fetchCategories = async () => {
    try {
      const response = await api.get('/Category')
      setCategories(response.data)
    } catch (error) {
      console.error('API Error:', error.response?.data || error.message)
      toast.error('Failed to load categories')
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      const categoryData = {
        CategoryName: formData.categoryName,
        IsActive: true
      }

      if (editingCategory) {
        categoryData.CategoryID = editingCategory.categoryID
        await api.put(`/Category/${editingCategory.categoryID}`, categoryData)
        toast.success('Category updated successfully!')
      } else {
        await api.post('/Category', categoryData)
        toast.success('Category added successfully!')
      }
      fetchCategories()
      closeModal()
    } catch (error) {
      console.error('Submit Error:', error.response?.data || error.message)
      toast.error(getErrorMessage(error))
    }
  }

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this category?')) {
      try {
        await api.delete(`/Category/${id}`)
        toast.success('Category deleted successfully!')
        fetchCategories()
      } catch (error) {
        console.error('Delete Error:', error.response?.data || error.message)
        toast.error(getErrorMessage(error) || 'Failed to delete category')
      }
    }
  }

  const openModal = (category = null) => {
    if (category) {
      setEditingCategory(category)
      setFormData({
        categoryName: category.categoryName,
        description: category.description || ''
      })
    } else {
      setEditingCategory(null)
      setFormData({ categoryName: '', description: '' })
    }
    setShowModal(true)
  }

  const closeModal = () => {
    setShowModal(false)
    setEditingCategory(null)
  }

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <h1 className="font-marcellus text-3xl text-[#111]">Categories Management</h1>
        <motion.button
          whileHover={{ scale: 1.05 }}
          whileTap={{ scale: 0.95 }}
          onClick={() => openModal()}
          className="bg-[#111] text-white px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex items-center space-x-2"
        >
          <Plus className="w-5 h-5" />
          <span>Add Category</span>
        </motion.button>
      </div>

      {/* Categories Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        {categories.map((category, index) => (
          <motion.div
            key={category.categoryID}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.05 }}
            className="bg-white rounded-2xl p-6 shadow-md hover:shadow-lg transition-shadow"
          >
            <div className="flex items-start justify-between mb-4">
              <div className="w-16 h-16 bg-kaira-light flex items-center justify-center">
                <span className="font-marcellus text-3xl text-kaira">
                  {category.categoryName.charAt(0)}
                </span>
              </div>
              <div className="flex items-center space-x-2">
                <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.9 }}
                  onClick={() => openModal(category)}
                  className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                >
                  <Edit className="w-5 h-5" />
                </motion.button>
                <motion.button
                  whileHover={{ scale: 1.1 }}
                  whileTap={{ scale: 0.9 }}
                  onClick={() => handleDelete(category.categoryID)}
                  className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                >
                  <Trash2 className="w-5 h-5" />
                </motion.button>
              </div>
            </div>
            <h3 className="text-xl font-bold mb-2">{category.categoryName}</h3>
            <p className="text-gray-600 text-sm">{category.description || 'No description'}</p>
          </motion.div>
        ))}
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
              className="bg-white rounded-2xl p-8 max-w-md w-full"
            >
              <div className="flex items-center justify-between mb-6">
                <h2 className="font-marcellus text-2xl text-[#111]">
                  {editingCategory ? 'Edit Category' : 'Add New Category'}
                </h2>
                <button onClick={closeModal} className="p-2 hover:bg-gray-100 rounded-lg">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div>
                  <label className="block text-sm font-semibold mb-2">Category Name</label>
                  <input
                    type="text"
                    required
                    value={formData.categoryName}
                    onChange={(e) => setFormData({ ...formData, categoryName: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold mb-2">Description</label>
                  <textarea
                    value={formData.description}
                    onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    rows="3"
                  />
                </div>

                <div className="flex gap-4 pt-4">
                  <button type="button" onClick={closeModal} className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1">
                    Cancel
                  </button>
                  <button type="submit" className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1">
                    {editingCategory ? 'Update' : 'Add'} Category
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

export default AdminCategories
