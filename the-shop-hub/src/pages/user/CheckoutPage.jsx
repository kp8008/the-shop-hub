import { useState, useEffect } from 'react'
import { useNavigate } from 'react-router-dom'
import { motion } from 'framer-motion'
import { CreditCard, MapPin, Check, Smartphone, Wallet } from 'lucide-react'
import useCartStore from '../../store/useCartStore'
import useAuthStore from '../../store/useAuthStore'
import api, { getErrorMessage, getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import toast from 'react-hot-toast'

const CheckoutPage = () => {
  const { items, getTotal, clearCart } = useCartStore()
  const { user } = useAuthStore()
  const navigate = useNavigate()

  const [step, setStep] = useState(1)
  const [loading, setLoading] = useState(false)
  const [showSuccessModal, setShowSuccessModal] = useState(false)
  const [addresses, setAddresses] = useState([])
  const [addressesLoading, setAddressesLoading] = useState(true)
  const [selectedAddressId, setSelectedAddressId] = useState(null)
  const [useNewAddress, setUseNewAddress] = useState(false)

  const [shippingInfo, setShippingInfo] = useState({
    address: '',
    city: '',
    state: '',
    zipCode: '',
    country: 'India'
  })

  const [paymentInfo, setPaymentInfo] = useState({
    method: 'card',
    cardNumber: '',
    cardName: '',
    expiryDate: '',
    cvv: '',
    upiId: ''
  })

  const total = getTotal()
  const shipping = total > 500 ? 0 : 50
  const tax = total * 0.1
  const grandTotal = total + shipping + tax

  useEffect(() => {
    fetchAddresses()
  }, [])

  const fetchAddresses = async () => {
    setAddressesLoading(true)
    try {
      const response = await api.get('/Address/MyAddresses')
      const mapped = response.data.map((a) => ({
        addressId: a.addressId ?? a.addressID ?? a.AddressId ?? a.AddressID,
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
      if (mapped.length > 0) {
        const defaultAddress = mapped.find((a) => a.isDefault) || mapped[0]
        setSelectedAddressId(defaultAddress.addressId)
        setUseNewAddress(false)
        setShippingInfo({
          address: defaultAddress.addressLine1,
          city: defaultAddress.city,
          state: defaultAddress.state,
          zipCode: defaultAddress.pincode,
          country: defaultAddress.country || 'India'
        })
      } else {
        setUseNewAddress(true)
      }
    } catch (error) {
      setUseNewAddress(true)
    } finally {
      setAddressesLoading(false)
    }
  }

  const validateShipping = () => {
    if (!useNewAddress && selectedAddressId && addresses.length > 0) {
      return true
    }

    if (
      !shippingInfo.address ||
      !shippingInfo.city ||
      !shippingInfo.state ||
      !shippingInfo.zipCode ||
      !shippingInfo.country
    ) {
      toast.error('Please fill all shipping details')
      return false
    }

    const cityPattern = /^[A-Za-z]{2,8}$/
    if (!cityPattern.test(shippingInfo.city.trim())) {
      toast.error('City must be 2–8 letters (A–Z only)')
      return false
    }

    const pinPattern = /^\d{6}$/
    if (!pinPattern.test(shippingInfo.zipCode.trim())) {
      toast.error('Please enter a valid 6-digit PIN code')
      return false
    }

    return true
  }

  const validatePayment = () => {
    if (paymentInfo.method === 'card') {
      const cardNumberValid = /^\d{16}$/.test(paymentInfo.cardNumber)
      const expiryValid = /^(0[1-9]|1[0-2])\/\d{2}$/.test(paymentInfo.expiryDate)
      const cvvValid = /^\d{3}$/.test(paymentInfo.cvv)
      if (!cardNumberValid) {
        toast.error('Please enter a valid 16-digit card number')
        return false
      }
      if (!paymentInfo.cardName.trim()) {
        toast.error('Please enter the cardholder name')
        return false
      }
      if (!expiryValid) {
        toast.error('Please enter a valid expiry date in MM/YY format')
        return false
      }
      if (!cvvValid) {
        toast.error('Please enter a valid 3-digit CVV')
        return false
      }
    }

    if (paymentInfo.method === 'upi') {
      if (!paymentInfo.upiId.trim()) {
        toast.error('Please enter your UPI ID')
        return false
      }
      const upiPattern = /^[a-zA-Z0-9.\-_]{2,}@[a-zA-Z]{2,}$/
      if (!upiPattern.test(paymentInfo.upiId)) {
        toast.error('Please enter a valid UPI ID (example: name@bank)')
        return false
      }
    }

    return true
  }

  const handleReviewOrder = () => {
    if (!validateShipping()) {
      return
    }
    if (!validatePayment()) {
      return
    }
    setStep(3)
  }

  const handlePlaceOrder = async () => {
    if (!items.length) {
      toast.error('Your cart is empty')
      return
    }

    if (!validateShipping()) {
      return
    }

    if (!validatePayment()) {
      return
    }

    setLoading(true)
    try {
      const receiverName = `${user.firstName || ''} ${user.lastName || ''}`.trim() || user.email || 'Customer'
      const phoneFromUser = user.mobile && /^\d{10,15}$/.test(user.mobile) ? user.mobile : '9999999999'
      let addressId = selectedAddressId

      if (!addressId || useNewAddress || addresses.length === 0) {
        const addressPayload = {
          addressID: 0,
          userID: user.userId,
          receiverName,
          phone: phoneFromUser,
          addressLine1: shippingInfo.address,
          landmark: '',
          city: shippingInfo.city,
          state: shippingInfo.state,
          country: shippingInfo.country,
          pincode: shippingInfo.zipCode,
          isDefault: addresses.length === 0
        }

        const addressResponse = await api.post('/Address', addressPayload)
        addressId =
          addressResponse.data.addressId ??
          addressResponse.data.AddressId ??
          addressResponse.data.AddressID
      }

      const orderResponse = await api.post('/Order', {
        orderId: 0,
        userId: user.userId,
        orderNo: 'TEMP',
        deliveryDate: null,
        addressId,
        totalAmount: total,
        couponDiscount: 0,
        netAmount: grandTotal,
        status: 'pending'
      })
      const orderId = orderResponse.data.orderId

      for (const item of items) {
        await api.post('/OrderDetail', {
          orderId,
          orderDetailId: 0,
          productId: item.productId,
          addressId,
          quantity: item.quantity,
          amount: item.price,
          discount: 0,
          netAmount: item.price * item.quantity
        })
      }

      const paymentMethodLabel = paymentInfo.method === 'card'
        ? 'CARD'
        : paymentInfo.method === 'upi'
        ? 'UPI'
        : 'COD'

      const paymentModeId =
        paymentInfo.method === 'card'
          ? 1
          : paymentInfo.method === 'upi'
          ? 3
          : 4

      await api.post('/Payment', {
        PaymentID: 0,
        OrderID: orderId,
        PaymentModeID: paymentModeId,
        TotalPayment: grandTotal,
        PaymentReference: `${paymentMethodLabel}-${Date.now()}`,
        PaymentStatus: 'Completed'
      })

      clearCart()
      toast.success('Payment successful')
      setShowSuccessModal(true)
    } catch (error) {
      console.error('Error placing order:', error.response?.data || error.message)
      toast.error(getErrorMessage(error))
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="py-16 min-h-screen bg-kaira-light">
      <div className="max-w-6xl mx-auto px-4 md:px-6">
        <h1 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-8">Checkout</h1>

        {/* Progress Steps - Kaira */}
        <div className="flex items-center justify-center mb-12">
          {[1, 2, 3].map((s) => (
            <div key={s} className="flex items-center">
              <motion.div
                animate={{ scale: step >= s ? 1 : 0.8 }}
                className={`w-12 h-12 rounded-full flex items-center justify-center font-jost font-semibold ${
                  step >= s
                    ? 'bg-[#111] text-white'
                    : 'bg-gray-200 text-gray-400'
                }`}
              >
                {step > s ? <Check className="w-6 h-6" /> : s}
              </motion.div>
              {s < 3 && (
                <div className={`w-24 h-1 mx-2 ${step > s ? 'bg-[#111]' : 'bg-gray-200'}`} />
              )}
            </div>
          ))}
        </div>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Forms */}
          <div className="lg:col-span-2 space-y-6">
                {/* Shipping Information */}
            {step === 1 && (
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                className="bg-white border border-gray-200 p-6"
              >
                <div className="flex items-center space-x-3 mb-6">
                  <MapPin className="w-6 h-6 text-kaira" />
                  <h2 className="font-marcellus text-xl text-[#111]">Shipping Information</h2>
                </div>

                <div className="space-y-6">
                  <div>
                    <div className="flex items-center justify-between mb-3">
                      <h3 className="text-lg font-semibold">Choose delivery address</h3>
                    </div>

                    {addressesLoading ? (
                      <p className="text-sm text-gray-500">Loading saved addresses...</p>
                    ) : addresses.length === 0 ? (
                      <p className="text-sm text-gray-500">
                        No saved addresses yet. Add a new delivery address below.
                      </p>
                    ) : (
                      <div className="space-y-3">
                        {addresses.map((address) => {
                          const isSelected =
                            !useNewAddress && selectedAddressId === address.addressId
                          return (
                            <button
                              key={address.addressId}
                              type="button"
                              onClick={() => {
                                setUseNewAddress(false)
                                setSelectedAddressId(address.addressId)
                                setShippingInfo({
                                  address: address.addressLine1,
                                  city: address.city,
                                  state: address.state,
                                  zipCode: address.pincode,
                                  country: address.country || 'India'
                                })
                              }}
                              className={`w-full text-left border p-4 flex items-start justify-between gap-3 ${
                                isSelected
                                  ? 'border-kaira bg-kaira-light'
                                  : 'border-gray-200 hover:border-kaira'
                              }`}
                            >
                              <div>
                                <p className="text-sm font-semibold text-gray-900">
                                  {address.receiverName}
                                </p>
                                <p className="text-sm text-gray-700">
                                  {address.addressLine1}
                                  {address.landmark && `, ${address.landmark}`}
                                </p>
                                <p className="text-sm text-gray-700">
                                  {address.city}, {address.state} - {address.pincode}
                                </p>
                                <p className="text-sm text-gray-600">+91 {address.phone}</p>
                              </div>
                              <div className="flex flex-col items-end gap-2">
                                {address.isDefault && (
                                  <span className="px-2 py-1 text-xs font-jost font-semibold bg-kaira-light text-kaira-dark">
                                    Default
                                  </span>
                                )}
                                {isSelected && (
                                  <Check className="w-5 h-5 text-kaira" />
                                )}
                              </div>
                            </button>
                          )
                        })}
                      </div>
                    )}
                  </div>

                  <div className="flex items-center justify-between">
                    <h3 className="text-lg font-semibold">Add new address</h3>
                    <label className="flex items-center gap-2 text-sm text-gray-700">
                      <input
                        type="checkbox"
                        checked={useNewAddress || addresses.length === 0}
                        onChange={(e) => {
                          const shouldUseNew = e.target.checked || addresses.length === 0
                          setUseNewAddress(shouldUseNew)
                          if (!shouldUseNew && addresses.length > 0) {
                            const defaultAddress =
                              addresses.find((a) => a.isDefault) || addresses[0]
                            setSelectedAddressId(defaultAddress.addressId)
                            setShippingInfo({
                              address: defaultAddress.addressLine1,
                              city: defaultAddress.city,
                              state: defaultAddress.state,
                              zipCode: defaultAddress.pincode,
                              country: defaultAddress.country || 'India'
                            })
                          }
                        }}
                        className="w-4 h-4 text-kaira border-gray-300 rounded"
                      />
                      <span>Use new address for this order</span>
                    </label>
                  </div>

                  {(useNewAddress || addresses.length === 0) && (
                    <div className="space-y-4">
                      <input
                        type="text"
                        placeholder="Street Address"
                        value={shippingInfo.address}
                        onChange={(e) =>
                          setShippingInfo({ ...shippingInfo, address: e.target.value })
                        }
                        className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                        required
                      />
                      <div className="grid grid-cols-2 gap-4">
                        <input
                          type="text"
                          placeholder="City"
                          value={shippingInfo.city}
                          onChange={(e) =>
                            setShippingInfo({
                              ...shippingInfo,
                              city: e.target.value.replace(/[^A-Za-z]/g, '').slice(0, 8)
                            })
                          }
                          className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                          maxLength="8"
                          required
                        />
                        <select
                          value={shippingInfo.state}
                          onChange={(e) =>
                            setShippingInfo({ ...shippingInfo, state: e.target.value })
                          }
                          className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
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
                          <option value="Andaman and Nicobar Islands">
                            Andaman and Nicobar Islands
                          </option>
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
                      <div className="grid grid-cols-2 gap-4">
                        <input
                          type="text"
                          placeholder="ZIP Code"
                          value={shippingInfo.zipCode}
                          onChange={(e) =>
                            setShippingInfo({
                              ...shippingInfo,
                              zipCode: e.target.value.replace(/\D/g, '').slice(0, 6)
                            })
                          }
                          className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                          maxLength="6"
                          required
                        />
                        <input
                          type="text"
                          value="India"
                          readOnly
                          className="w-full px-4 py-3 border border-gray-200 font-jost bg-gray-100 cursor-not-allowed"
                        />
                      </div>
                    </div>
                  )}
                </div>

                <button
                  onClick={() => {
                    if (validateShipping()) {
                      setStep(2)
                    }
                  }}
                  className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors w-full mt-6"
                >
                  Continue to Payment
                </button>
              </motion.div>
            )}

            {step === 2 && (
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                className="bg-white rounded-2xl p-6 shadow-md"
              >
                <div className="flex items-center space-x-3 mb-6">
                  <CreditCard className="w-6 h-6 text-kaira" />
                  <h2 className="text-2xl font-bold">Payment Information</h2>
                </div>

                <div className="space-y-4">
                  <div className="grid grid-cols-3 gap-3">
                    <button
                      type="button"
                      onClick={() => setPaymentInfo({ ...paymentInfo, method: 'card' })}
                      className={`flex flex-col items-center justify-center gap-2 border rounded-xl py-3 px-4 text-sm font-medium transition-colors ${
                        paymentInfo.method === 'card'
                          ? 'border-kaira bg-kaira-light text-kaira-dark'
                          : 'border-gray-200 text-gray-600 hover:border-kaira hover:text-kaira'
                      }`}
                    >
                      <CreditCard className="w-5 h-5" />
                      <span>Card</span>
                    </button>
                    <button
                      type="button"
                      onClick={() => setPaymentInfo({ ...paymentInfo, method: 'upi' })}
                      className={`flex flex-col items-center justify-center gap-2 border rounded-xl py-3 px-4 text-sm font-medium transition-colors ${
                        paymentInfo.method === 'upi'
                          ? 'border-kaira bg-kaira-light text-kaira-dark'
                          : 'border-gray-200 text-gray-600 hover:border-kaira hover:text-kaira'
                      }`}
                    >
                      <Smartphone className="w-5 h-5" />
                      <span>UPI</span>
                    </button>
                    <button
                      type="button"
                      onClick={() => setPaymentInfo({ ...paymentInfo, method: 'cod' })}
                      className={`flex flex-col items-center justify-center gap-2 border rounded-xl py-3 px-4 text-sm font-medium transition-colors ${
                        paymentInfo.method === 'cod'
                          ? 'border-kaira bg-kaira-light text-kaira-dark'
                          : 'border-gray-200 text-gray-600 hover:border-kaira hover:text-kaira'
                      }`}
                    >
                      <Wallet className="w-5 h-5" />
                      <span>Cash on Delivery</span>
                    </button>
                  </div>

                  {paymentInfo.method === 'card' && (
                    <>
                      <input
                        type="text"
                        placeholder="Card Number"
                        value={paymentInfo.cardNumber}
                        onChange={(e) =>
                          setPaymentInfo({
                            ...paymentInfo,
                            cardNumber: e.target.value.replace(/\D/g, '').slice(0, 16)
                          })
                        }
                        className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                        maxLength="16"
                        required
                      />
                      <input
                        type="text"
                        placeholder="Cardholder Name"
                        value={paymentInfo.cardName}
                        onChange={(e) => setPaymentInfo({ ...paymentInfo, cardName: e.target.value })}
                        className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                        required
                      />
                      <div className="grid grid-cols-2 gap-4">
                        <input
                          type="text"
                          placeholder="MM/YY"
                          value={paymentInfo.expiryDate}
                          onChange={(e) =>
                            setPaymentInfo({ ...paymentInfo, expiryDate: e.target.value })
                          }
                          className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                          maxLength="5"
                          required
                        />
                        <input
                          type="text"
                          placeholder="CVV"
                          value={paymentInfo.cvv}
                          onChange={(e) =>
                            setPaymentInfo({
                              ...paymentInfo,
                              cvv: e.target.value.replace(/\D/g, '').slice(0, 3)
                            })
                          }
                          className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                          maxLength="3"
                          required
                        />
                      </div>
                    </>
                  )}

                  {paymentInfo.method === 'upi' && (
                    <input
                      type="text"
                      placeholder="Enter UPI ID (example: name@bank)"
                      value={paymentInfo.upiId}
                      onChange={(e) => setPaymentInfo({ ...paymentInfo, upiId: e.target.value })}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                      required
                    />
                  )}

                  {paymentInfo.method === 'cod' && (
                    <p className="text-gray-600 text-sm">
                      Pay safely with cash when your order is delivered to you.
                    </p>
                  )}
                </div>

                <div className="flex gap-4 mt-6">
                  <button onClick={() => setStep(1)} className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1">
                    Back
                  </button>
                  <button onClick={handleReviewOrder} className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1">
                    Review Order
                  </button>
                </div>
              </motion.div>
            )}

            {/* Review Order */}
            {step === 3 && (
              <motion.div
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                className="bg-white rounded-2xl p-6 shadow-md"
              >
                <h2 className="text-2xl font-bold mb-6">Review Your Order</h2>

                <div className="space-y-4 mb-6">
                  {items.map((item) => (
                    <div key={item.productId} className="flex items-center space-x-4 pb-4 border-b">
                      <img
                        src={item.imageUrl || getPlaceholderImageUrl(80, 80)}
                        alt={item.productName}
                        className="w-20 h-20 object-cover rounded-lg"
                      />
                      <div className="flex-1">
                        <h3 className="font-semibold">{item.productName}</h3>
                        <p className="text-gray-600">Qty: {item.quantity}</p>
                      </div>
                      <p className="font-bold">{formatPrice(item.price * item.quantity)}</p>
                    </div>
                  ))}
                </div>

                <div className="flex gap-4">
                  <button onClick={() => setStep(2)} className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1">
                    Back
                  </button>
                  <button
                    onClick={handlePlaceOrder}
                    disabled={loading}
                    className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1 disabled:opacity-50"
                  >
                    {loading ? 'Processing...' : 'Place Order'}
                  </button>
                </div>
              </motion.div>
            )}
          </div>

          {/* Order Summary */}
          <div className="lg:col-span-1">
            <div className="bg-white rounded-2xl p-6 shadow-md sticky top-24">
              <h2 className="text-2xl font-bold mb-6">Order Summary</h2>

              <div className="space-y-4 mb-6">
                <div className="flex justify-between text-gray-600">
                  <span>Subtotal ({items.length} items)</span>
                  <span className="font-semibold">{formatPrice(total)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Shipping</span>
                  <span className="font-semibold">
                    {shipping === 0 ? 'FREE' : formatPrice(shipping)}
                  </span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Tax</span>
                  <span className="font-semibold">{formatPrice(tax)}</span>
                </div>
                <div className="border-t pt-4">
                  <div className="flex justify-between text-xl font-bold">
                    <span>Total</span>
                    <span className="text-[#111] font-marcellus">{formatPrice(grandTotal)}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>

      {showSuccessModal && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 backdrop-blur-sm">
          <div
            className="absolute inset-0"
            onClick={() => setShowSuccessModal(false)}
          />
          <motion.div
            initial={{ opacity: 0, scale: 0.9, y: 20 }}
            animate={{ opacity: 1, scale: 1, y: 0 }}
            className="relative z-10 mx-4 w-full max-w-md overflow-hidden rounded-3xl bg-white p-8 text-center shadow-2xl"
          >
            <div className="pointer-events-none absolute inset-0 bg-kaira-light" />
            <div className="relative flex flex-col items-center">
              <div className="mb-6 flex h-20 w-20 items-center justify-center rounded-full bg-[#111] text-white">
                <Check className="h-10 w-10" />
              </div>
              <h2 className="text-2xl font-extrabold tracking-tight text-gray-900">
                Payment successful
              </h2>
              <p className="mt-3 text-sm font-jost font-medium uppercase tracking-[0.2em] text-kaira-dark">
                Thank you for shopping
              </p>
              <p className="mt-4 text-base text-gray-600">
                Have a great day. Your order is on the way to you.
              </p>
              <div className="mt-8 flex flex-col gap-3 sm:flex-row">
                <button
                  onClick={() => {
                    setShowSuccessModal(false)
                    navigate('/orders')
                  }}
                  className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1"
                >
                  View my orders
                </button>
                <button
                  onClick={() => {
                    setShowSuccessModal(false)
                    navigate('/')
                  }}
                  className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1"
                >
                  Continue shopping
                </button>
              </div>
            </div>
          </motion.div>
        </div>
      )}
    </div>
  )
}

export default CheckoutPage
