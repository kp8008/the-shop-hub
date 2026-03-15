import { useState, useEffect } from 'react'
import { motion } from 'framer-motion'
import { User, Mail, Phone, Save, MapPin, Plus, Trash2, Home } from 'lucide-react'
import useAuthStore from '../../store/useAuthStore'
import api, { getErrorMessage } from '../../utils/api'
import toast from 'react-hot-toast'

const ProfilePage = () => {
  const { user, updateUser } = useAuthStore()
  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    email: user?.email || '',
    mobile: user?.mobile || user?.phone || ''
  })
  const [loading, setLoading] = useState(false)
  const [addresses, setAddresses] = useState([])
  const [addressesLoading, setAddressesLoading] = useState(true)
  const [savingAddress, setSavingAddress] = useState(false)
  const [showAddressForm, setShowAddressForm] = useState(false)
  const [editingAddress, setEditingAddress] = useState(null)
  const [addressForm, setAddressForm] = useState({
    receiverName: '',
    phone: '',
    addressLine1: '',
    landmark: '',
    city: '',
    state: '',
    country: 'India',
    pincode: '',
    isDefault: false
  })

  useEffect(() => {
    fetchAddresses()
  }, [])

  const fetchAddresses = async () => {
    setAddressesLoading(true)
    try {
      const response = await api.get('/Address/MyAddresses')
      const mapped = response.data.map((a) => ({
        addressId: a.addressId ?? a.addressID ?? a.AddressId ?? a.AddressID,
        userId: a.userId ?? a.userID ?? a.UserId ?? a.UserID,
        receiverName: a.receiverName ?? a.ReceiverName,
        phone: a.phone ?? a.Phone,
        addressLine1: a.addressLine1 ?? a.AddressLine1,
        landmark: a.landmark ?? a.Landmark ?? '',
        city: a.city ?? a.City,
        state: a.state ?? a.State,
        country: a.country ?? a.Country,
        pincode: a.pincode ?? a.Pincode,
        isDefault: a.isDefault ?? a.IsDefault
      }))
      setAddresses(mapped)
    } catch (error) {
      toast.error('Failed to load addresses')
    } finally {
      setAddressesLoading(false)
    }
  }

  const resetAddressForm = () => {
    setAddressForm({
      receiverName: `${user?.firstName || ''} ${user?.lastName || ''}`.trim() || user?.email || '',
      phone: (formData.mobile || '').replace(/\D/g, ''),
      addressLine1: '',
      landmark: '',
      city: '',
      state: '',
      country: 'India',
      pincode: '',
      isDefault: addresses.length === 0
    })
  }

  const handleAddNewAddress = () => {
    setEditingAddress(null)
    resetAddressForm()
    setShowAddressForm(true)
  }

  const handleEditAddress = (address) => {
    setEditingAddress(address)
    setAddressForm({
      receiverName: address.receiverName,
      phone: address.phone,
      addressLine1: address.addressLine1,
      landmark: address.landmark || '',
      city: address.city,
      state: address.state,
      country: address.country,
      pincode: address.pincode,
      isDefault: address.isDefault
    })
    setShowAddressForm(true)
  }

  const handleDeleteAddress = async (addressId) => {
    const confirmed = window.confirm('Delete this address?')
    if (!confirmed) return
    try {
      await api.delete(`/Address/${addressId}`)
      toast.success('Address deleted')
      fetchAddresses()
    } catch (error) {
      toast.error(getErrorMessage(error))
    }
  }

  const saveAddress = async (override) => {
    setSavingAddress(true)
    try {
      const userId = user?.userId ?? user?.userID ?? 0
      const payload = {
        addressID: editingAddress ? editingAddress.addressId : 0,
        userID: userId,
        receiverName: addressForm.receiverName,
        phone: (addressForm.phone || '').replace(/\D/g, ''),
        addressLine1: addressForm.addressLine1,
        landmark: addressForm.landmark,
        city: addressForm.city,
        state: addressForm.state,
        country: addressForm.country,
        pincode: addressForm.pincode,
        isDefault:
          override?.isDefault ??
          addressForm.isDefault ??
          (editingAddress ? editingAddress.isDefault : addresses.length === 0)
      }

      if (payload.addressID === 0) {
        await api.post('/Address', payload)
        toast.success('Address added')
      } else {
        await api.put(`/Address/${payload.addressID}`, payload)
        toast.success('Address updated')
      }

      setShowAddressForm(false)
      setEditingAddress(null)
      fetchAddresses()
    } catch (error) {
      toast.error(getErrorMessage(error))
    } finally {
      setSavingAddress(false)
    }
  }

  const handleSaveAddress = async (e) => {
    e.preventDefault()
    await saveAddress()
  }

  const handleSetDefault = async (address) => {
    setEditingAddress(address)
    setAddressForm({
      receiverName: address.receiverName,
      phone: address.phone,
      addressLine1: address.addressLine1,
      landmark: address.landmark || '',
      city: address.city,
      state: address.state,
      country: address.country,
      pincode: address.pincode,
      isDefault: true
    })
    await saveAddress({ isDefault: true })
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)

    try {
      const userId = user?.userId ?? user?.userID ?? 0

      const payload = {
        userID: userId,
        userName:
          `${formData.firstName} ${formData.lastName}`.trim() ||
          formData.email ||
          user?.firstName ||
          'Customer',
        address: user?.address || '',
        phone: (formData.mobile || '').replace(/\D/g, ''),
        email: formData.email,
        password: '',
        userTypeID: user?.userTypeId ?? user?.userTypeID ?? 2,
        isActive: true
      }

      await api.put(`/User/${userId}`, payload)

      updateUser({
        ...user,
        firstName: formData.firstName,
        lastName: formData.lastName,
        email: formData.email,
        mobile: payload.phone,
        phone: payload.phone
      })
      toast.success('Profile updated successfully!')
    } catch (error) {
      toast.error(getErrorMessage(error))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="py-16 min-h-screen bg-kaira-light">
      <div className="max-w-4xl mx-auto px-4 md:px-6 space-y-8">
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="bg-white border border-gray-200 p-8"
        >
          <h1 className="font-marcellus text-3xl text-[#111] mb-8">My Profile</h1>

          <form onSubmit={handleSubmit} className="space-y-6">
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-jost font-semibold mb-2">First Name</label>
                <div className="relative">
                  <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                  <input
                    type="text"
                    value={formData.firstName}
                    onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-jost font-semibold mb-2">Last Name</label>
                <input
                  type="text"
                  value={formData.lastName}
                  onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-jost font-semibold mb-2">Email</label>
              <div className="relative">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type="email"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-jost font-semibold mb-2">Mobile</label>
              <div className="relative">
                <Phone className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type="tel"
                  value={formData.mobile}
                  onChange={(e) => setFormData({ ...formData, mobile: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                />
              </div>
            </div>

            <motion.button
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
              type="submit"
              disabled={loading}
              className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex items-center justify-center space-x-2 disabled:opacity-50"
            >
              <Save className="w-5 h-5" />
              <span>{loading ? 'Saving...' : 'Save Changes'}</span>
            </motion.button>
          </form>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="bg-white border border-gray-200 p-8"
        >
          <div className="flex items-center justify-between mb-6">
            <div className="flex items-center gap-3">
              <MapPin className="w-6 h-6 text-kaira" />
              <h2 className="font-marcellus text-2xl text-[#111]">Saved Addresses</h2>
            </div>
            <button
              type="button"
              onClick={handleAddNewAddress}
              className="inline-flex items-center gap-2 px-4 py-2 bg-[#111] text-white text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
            >
              <Plus className="w-4 h-4" />
              <span>Add Address</span>
            </button>
          </div>

          {addressesLoading ? (
            <p className="font-jost text-gray-500 text-sm">Loading addresses...</p>
          ) : addresses.length === 0 ? (
            <p className="font-jost text-gray-500 text-sm">
              No addresses saved yet. Add your delivery address.
            </p>
          ) : (
            <div className="grid gap-4 md:grid-cols-2">
              {addresses.map((address) => (
                <div
                  key={address.addressId}
                  className={`border p-4 space-y-3 ${
                    address.isDefault ? 'border-kaira bg-kaira-light' : 'border-gray-200'
                  }`}
                >
                  <div className="flex items-center justify-between">
                    <div className="flex items-center gap-2">
                      <Home className="w-5 h-5 text-kaira" />
                      <span className="font-jost font-semibold text-sm">{address.receiverName}</span>
                    </div>
                    {address.isDefault && (
                      <span className="px-2 py-1 text-xs font-jost font-semibold bg-kaira-light text-kaira-dark">
                        Default
                      </span>
                    )}
                  </div>
                  <p className="text-sm text-gray-700">
                    {address.addressLine1}
                    {address.landmark && `, ${address.landmark}`}
                  </p>
                  <p className="text-sm text-gray-700">
                    {address.city}, {address.state}, {address.country} - {address.pincode}
                  </p>
                  <p className="text-sm text-gray-600">+91 {address.phone}</p>
                  <div className="flex items-center justify-between pt-2">
                    <div className="flex gap-2">
                      <button
                        type="button"
                        onClick={() => handleEditAddress(address)}
                        className="px-3 py-1 text-xs font-semibold rounded-full border border-gray-300 text-gray-700 hover:bg-gray-100"
                      >
                        Edit
                      </button>
                      <button
                        type="button"
                        onClick={() => handleDeleteAddress(address.addressId)}
                        className="px-3 py-1 text-xs font-semibold rounded-full border border-red-200 text-red-600 hover:bg-red-50 flex items-center gap-1"
                      >
                        <Trash2 className="w-3 h-3" />
                        Delete
                      </button>
                    </div>
                    {!address.isDefault && (
                      <button
                        type="button"
                        onClick={() => handleSetDefault(address)}
                        className="px-3 py-1 text-xs font-jost font-semibold bg-kaira-light text-kaira-dark hover:bg-kaira hover:text-white transition-colors"
                      >
                        Set as default
                      </button>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}

          {showAddressForm && (
            <form onSubmit={handleSaveAddress} className="mt-8 space-y-4 border-t pt-6">
              <h3 className="text-lg font-semibold mb-2">
                {editingAddress ? 'Edit Address' : 'Add New Address'}
              </h3>
              <div className="grid md:grid-cols-2 gap-4">
                <div>
                  <label className="block text-sm font-jost font-semibold mb-2">Receiver Name</label>
                  <input
                    type="text"
                    value={addressForm.receiverName}
                    onChange={(e) =>
                      setAddressForm({ ...addressForm, receiverName: e.target.value })
                    }
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-jost font-semibold mb-2">Phone</label>
                  <input
                    type="tel"
                    value={addressForm.phone}
                    onChange={(e) =>
                      setAddressForm({
                        ...addressForm,
                        phone: e.target.value.replace(/\D/g, '').slice(0, 15)
                      })
                    }
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    required
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-jost font-semibold mb-2">Address Line</label>
                <input
                  type="text"
                  value={addressForm.addressLine1}
                  onChange={(e) =>
                    setAddressForm({ ...addressForm, addressLine1: e.target.value })
                  }
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                  required
                />
              </div>
              <div>
                <label className="block text-sm font-jost font-semibold mb-2">Landmark (optional)</label>
                <input
                  type="text"
                  value={addressForm.landmark}
                  onChange={(e) => setAddressForm({ ...addressForm, landmark: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                />
              </div>
              <div className="grid md:grid-cols-3 gap-4">
                <div>
                  <label className="block text-sm font-jost font-semibold mb-2">City</label>
                  <input
                    type="text"
                    value={addressForm.city}
                    onChange={(e) =>
                      setAddressForm({
                        ...addressForm,
                        city: e.target.value.replace(/[^A-Za-z ]/g, '')
                      })
                    }
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-jost font-semibold mb-2">State</label>
                  <select
                    value={addressForm.state}
                    onChange={(e) => setAddressForm({ ...addressForm, state: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    required
                  >
                    <option value="">Select State</option>
                    <option value="Andhra Pradesh">Andhra Pradesh</option>
                    <option value="Arunachal Pradesh">Arunachal Pradesh</option>
                    <option value="Assam">Assam</option>
                    <option value="Bihar">Bihar</option>
                    <option value="Chhattisgarh">Chhattisgarh</option>
                    <option value="Goa">Goa</option>
                    <option value="Gujarat">Gujarat</option>
                    <option value="Haryana">Haryana</option>
                    <option value="Himachal Pradesh">Himachal Pradesh</option>
                    <option value="Jharkhand">Jharkhand</option>
                    <option value="Karnataka">Karnataka</option>
                    <option value="Kerala">Kerala</option>
                    <option value="Madhya Pradesh">Madhya Pradesh</option>
                    <option value="Maharashtra">Maharashtra</option>
                    <option value="Manipur">Manipur</option>
                    <option value="Meghalaya">Meghalaya</option>
                    <option value="Mizoram">Mizoram</option>
                    <option value="Nagaland">Nagaland</option>
                    <option value="Odisha">Odisha</option>
                    <option value="Punjab">Punjab</option>
                    <option value="Rajasthan">Rajasthan</option>
                    <option value="Sikkim">Sikkim</option>
                    <option value="Tamil Nadu">Tamil Nadu</option>
                    <option value="Telangana">Telangana</option>
                    <option value="Tripura">Tripura</option>
                    <option value="Uttar Pradesh">Uttar Pradesh</option>
                    <option value="Uttarakhand">Uttarakhand</option>
                    <option value="West Bengal">West Bengal</option>
                    <option value="Andaman and Nicobar Islands">Andaman and Nicobar Islands</option>
                    <option value="Chandigarh">Chandigarh</option>
                    <option value="Dadra and Nagar Haveli and Daman and Diu">
                      Dadra and Nagar Haveli and Daman and Diu
                    </option>
                    <option value="Delhi">Delhi</option>
                    <option value="Jammu and Kashmir">Jammu and Kashmir</option>
                    <option value="Ladakh">Ladakh</option>
                    <option value="Lakshadweep">Lakshadweep</option>
                    <option value="Puducherry">Puducherry</option>
                  </select>
                </div>
                <div>
                  <label className="block text-sm font-semibold mb-2">PIN Code</label>
                  <input
                    type="text"
                    value={addressForm.pincode}
                    onChange={(e) =>
                      setAddressForm({
                        ...addressForm,
                        pincode: e.target.value.replace(/\D/g, '').slice(0, 6)
                      })
                    }
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none"
                    required
                  />
                </div>
              </div>
              <div className="grid md:grid-cols-2 gap-4 items-center">
                <div>
                  <label className="block text-sm font-jost font-semibold mb-2">Country</label>
                  <input
                    type="text"
                    value={addressForm.country}
                    readOnly
                    className="w-full px-4 py-3 border border-gray-200 font-jost bg-gray-100 cursor-not-allowed"
                  />
                </div>
                <div className="flex items-center gap-2 mt-6 md:mt-8">
                  <input
                    id="defaultAddress"
                    type="checkbox"
                    checked={addressForm.isDefault}
                    onChange={(e) =>
                      setAddressForm({ ...addressForm, isDefault: e.target.checked })
                    }
                    className="w-4 h-4 text-kaira border-gray-300 rounded focus:ring-kaira"
                  />
                  <label htmlFor="defaultAddress" className="text-sm font-jost text-gray-700">
                    Set as default address
                  </label>
                </div>
              </div>
              <div className="flex justify-end gap-3 pt-2">
                <button
                  type="button"
                  onClick={() => {
                    setShowAddressForm(false)
                    setEditingAddress(null)
                  }}
                  className="border border-[#111] text-[#111] px-6 py-2 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors"
                >
                  Cancel
                </button>
                <button
                  type="submit"
                  disabled={savingAddress}
                  className="bg-[#111] text-white px-6 py-2 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors disabled:opacity-50"
                >
                  {savingAddress ? 'Saving...' : 'Save Address'}
                </button>
              </div>
            </form>
          )}
        </motion.div>
      </div>
    </div>
  )
}

export default ProfilePage
