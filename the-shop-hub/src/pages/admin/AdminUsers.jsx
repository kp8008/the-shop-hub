import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Edit, Trash2, X, Search, Filter, UserCheck, UserX } from 'lucide-react'
import api, { getErrorMessage } from '../../utils/api'
import toast from 'react-hot-toast'

const AdminUsers = () => {
  const [users, setUsers] = useState([])
  const [userTypes, setUserTypes] = useState([])
  const [showModal, setShowModal] = useState(false)
  const [editingUser, setEditingUser] = useState(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [filterType, setFilterType] = useState('')
  const [formData, setFormData] = useState({
    userName: '',
    email: '',
    password: '',
    address: '',
    phone: '',
    userTypeId: 2
  })

  useEffect(() => {
    fetchData()
  }, [])

  const fetchData = async () => {
    try {
      const [usersRes, userTypesRes] = await Promise.all([
        api.get('/User'),
        api.get('/UserType')
      ])
      setUsers(usersRes.data)
      setUserTypes(userTypesRes.data)
    } catch (error) {
      toast.error('Failed to load data')
    }
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    try {
      if (editingUser) {
        const updateData = { ...formData }
        if (!updateData.password) {
          delete updateData.password
        }
        await api.put(`/User/${editingUser.userId}`, updateData)
        toast.success('User updated successfully!')
      } else {
        await api.post('/User', formData)
        toast.success('User created successfully!')
      }
      fetchData()
      closeModal()
    } catch (error) {
      toast.error(getErrorMessage(error))
    }
  }

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this user?')) {
      try {
        await api.delete(`/User/${id}`)
        toast.success('User deleted successfully!')
        fetchData()
      } catch (error) {
        toast.error(getErrorMessage(error) || 'Failed to delete user')
      }
    }
  }

  const openModal = (user = null) => {
    if (user) {
      setEditingUser(user)
      setFormData({
        userName: user.userName,
        email: user.email,
        password: '',
        address: user.address,
        phone: user.phone,
        userTypeId: user.userTypeId
      })
    } else {
      setEditingUser(null)
      setFormData({
        userName: '',
        email: '',
        password: '',
        address: '',
        phone: '',
        userTypeId: 2
      })
    }
    setShowModal(true)
  }

  const closeModal = () => {
    setShowModal(false)
    setEditingUser(null)
  }

  const filteredUsers = users.filter(user => {
    const matchesSearch = user.userName.toLowerCase().includes(searchTerm.toLowerCase()) ||
                         user.email.toLowerCase().includes(searchTerm.toLowerCase())
    const matchesFilter = filterType === '' || user.userTypeId.toString() === filterType
    return matchesSearch && matchesFilter
  })

  return (
    <div>
      <div className="mb-8">
        <h1 className="font-marcellus text-3xl text-[#111]">Users Management</h1>
      </div>

      {/* Search and Filter */}
      <div className="bg-white rounded-2xl p-6 shadow-md mb-6">
        <div className="flex flex-col md:flex-row gap-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search users..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none pl-10"
            />
          </div>
          <div className="relative">
            <Filter className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <select
              value={filterType}
              onChange={(e) => setFilterType(e.target.value)}
              className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none pl-10 pr-4"
            >
              <option value="">All User Types</option>
              {userTypes.map((type, idx) => (
                <option key={type.userTypeID ?? type.userTypeId ?? idx} value={type.userTypeID ?? type.userTypeId}>
                  {type.userTypeName}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Users Table */}
      <div className="bg-white border border-gray-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">User</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Email</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Phone</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Type</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Status</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {filteredUsers.map((user, idx) => (
                <tr key={user.userId ?? user.userID ?? idx} className="hover:bg-gray-50 transition-colors">
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-3">
                      <div className="w-10 h-10 bg-kaira-light rounded-full flex items-center justify-center">
                        <span className="font-jost font-semibold text-kaira">
                          {user.userName.charAt(0).toUpperCase()}
                        </span>
                      </div>
                      <div>
                        <p className="font-medium">{user.userName}</p>
                        <p className="text-sm text-gray-500">{user.address}</p>
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4">{user.email}</td>
                  <td className="px-6 py-4">{user.phone}</td>
                  <td className="px-6 py-4">
                    <span className={`px-3 py-1 rounded-full text-sm font-medium ${
                      user.userTypeId === 1 
                        ? 'bg-purple-100 text-purple-700' 
                        : 'bg-blue-100 text-blue-700'
                    }`}>
                      {userTypes.find(t => t.userTypeID === user.userTypeId)?.userTypeName || 'Unknown'}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-1">
                      <UserCheck className="w-4 h-4 text-green-500" />
                      <span className="text-sm text-green-600">Active</span>
                    </div>
                  </td>
                  <td className="px-6 py-4">
                    <div className="flex items-center space-x-2">
                      <motion.button
                        whileHover={{ scale: 1.1 }}
                        whileTap={{ scale: 0.9 }}
                        onClick={() => openModal(user)}
                        className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                      >
                        <Edit className="w-5 h-5" />
                      </motion.button>
                      <motion.button
                        whileHover={{ scale: 1.1 }}
                        whileTap={{ scale: 0.9 }}
                        onClick={() => handleDelete(user.userId)}
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
                  {editingUser ? 'Edit User' : 'Add New User'}
                </h2>
                <button onClick={closeModal} className="p-2 hover:bg-gray-100 rounded-lg">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <form onSubmit={handleSubmit} className="space-y-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold mb-2">Username</label>
                    <input
                      type="text"
                      required
                      value={formData.userName}
                      onChange={(e) => setFormData({ ...formData, userName: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold mb-2">Email</label>
                    <input
                      type="email"
                      required
                      value={formData.email}
                      onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    />
                  </div>
                </div>

                <div>
                  <label className="block text-sm font-semibold mb-2">
                    Password {editingUser && '(leave blank to keep current)'}
                  </label>
                  <input
                    type="password"
                    required={!editingUser}
                    value={formData.password}
                    onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  />
                </div>

                <div>
                  <label className="block text-sm font-semibold mb-2">Address</label>
                  <textarea
                    value={formData.address}
                    onChange={(e) => setFormData({ ...formData, address: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    rows="3"
                  />
                </div>

                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-semibold mb-2">Phone</label>
                    <input
                      type="tel"
                      value={formData.phone}
                      onChange={(e) => setFormData({ ...formData, phone: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-semibold mb-2">User Type</label>
                    <select
                      required
                      value={formData.userTypeId}
                      onChange={(e) => setFormData({ ...formData, userTypeId: parseInt(e.target.value) })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    >
                      {userTypes.map((type, idx) => (
                        <option key={type.userTypeID ?? type.userTypeId ?? idx} value={type.userTypeID ?? type.userTypeId}>
                          {type.userTypeName}
                        </option>
                      ))}
                    </select>
                  </div>
                </div>

                <div className="flex gap-4 pt-4">
                  <button type="button" onClick={closeModal} className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1">
                    Cancel
                  </button>
                  <button type="submit" className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1">
                    {editingUser ? 'Update' : 'Create'} User
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

export default AdminUsers