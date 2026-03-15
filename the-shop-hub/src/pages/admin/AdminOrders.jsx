import { useState, useEffect } from 'react'
import { motion, AnimatePresence } from 'framer-motion'
import { Eye, Edit, Trash2, X, Search, Filter, Package, Truck, CheckCircle, Clock, Star, MessageSquare } from 'lucide-react'
import api, { getImageUrl, getErrorMessage, getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import toast from 'react-hot-toast'
import { downloadInvoicePdf } from '../../utils/invoicePdf'

const AdminOrders = () => {
  const [orders, setOrders] = useState([])
  const [showModal, setShowModal] = useState(false)
  const [selectedOrder, setSelectedOrder] = useState(null)
  const [searchTerm, setSearchTerm] = useState('')
  const [statusFilter, setStatusFilter] = useState('')
  const [orderDetails, setOrderDetails] = useState([])

  const orderStatuses = [
    { value: 'pending', label: 'Pending', color: 'bg-yellow-100 text-yellow-700', icon: Clock },
    { value: 'processing', label: 'Processing', color: 'bg-blue-100 text-blue-700', icon: Package },
    { value: 'shipped', label: 'Shipped', color: 'bg-purple-100 text-purple-700', icon: Truck },
    { value: 'delivered', label: 'Delivered', color: 'bg-green-100 text-green-700', icon: CheckCircle }
  ]

  useEffect(() => {
    fetchOrders()
  }, [])

  const fetchOrders = async () => {
    try {
      const response = await api.get('/Order')
      const normalized = response.data.map(o => ({
        orderId: o.orderId ?? o.orderID ?? o.OrderID,
        orderNo: o.orderNo ?? o.OrderNo,
        userName: o.userName ?? o.UserName,
        userEmail: o.userEmail ?? o.UserEmail,
        userPhone: o.userPhone ?? o.UserPhone ?? o.phone ?? o.Phone ?? '',
        orderDate: o.orderDate ?? o.OrderDate,
        deliveryDate: o.deliveryDate ?? o.DeliveryDate,
        totalAmount: o.netAmount ?? o.NetAmount ?? o.totalAmount ?? o.TotalAmount,
        status: (o.status ?? o.Status ?? 'pending').toLowerCase()
      }))
      setOrders(normalized)
    } catch (error) {
      toast.error(getErrorMessage(error) || 'Failed to load orders')
    }
  }

  const updateOrderStatus = async (orderId, newStatus) => {
    try {
      await api.put(`/Order/${orderId}/Status`, { status: newStatus })
      toast.success('Order status updated successfully!')
      fetchOrders()
    } catch (error) {
      toast.error(getErrorMessage(error) || 'Failed to update order status')
    }
  }

  const handleDelete = async (id) => {
    if (window.confirm('Are you sure you want to delete this order?')) {
      try {
        await api.delete(`/Order/${id}`)
        toast.success('Order deleted successfully!')
        fetchOrders()
      } catch (error) {
        toast.error(getErrorMessage(error) || 'Failed to delete order')
      }
    }
  }

  const openModal = async (order) => {
    try {
      const response = await api.get(`/Order/${order.orderId}`)
      const data = response.data

      const payment = data.payment ?? data.Payment
      const normalizedOrder = {
        orderId: data.orderId ?? data.orderID ?? data.OrderID ?? order.orderId,
        orderNo: data.orderNo ?? data.OrderNo ?? order.orderNo,
        userName: data.userName ?? data.UserName ?? order.userName,
        userEmail: data.userEmail ?? data.UserEmail ?? order.userEmail,
        userPhone:
          data.userPhone ??
          data.UserPhone ??
          data.phone ??
          data.Phone ??
          order.userPhone ??
          '',
        orderDate: data.orderDate ?? data.OrderDate ?? order.orderDate,
        deliveryDate: data.deliveryDate ?? data.DeliveryDate ?? order.deliveryDate,
        totalAmount:
          data.netAmount ??
          data.NetAmount ??
          data.totalAmount ??
          data.TotalAmount ??
          order.totalAmount,
        status: (data.status ?? data.Status ?? order.status ?? 'pending').toLowerCase(),
        shippingAddress: data.shippingAddress ?? data.ShippingAddress ?? null,
        payment: payment ? {
          paymentModeName: payment.paymentModeName ?? payment.PaymentModeName ?? 'N/A',
          paymentReference: payment.paymentReference ?? payment.PaymentReference ?? '',
          paymentStatus: payment.paymentStatus ?? payment.PaymentStatus ?? '',
          totalPayment: payment.totalPayment ?? payment.TotalPayment ?? 0,
          transactionID: payment.transactionID ?? payment.TransactionID ?? ''
        } : null
      }

      const itemsRaw = data.orderItems ?? data.OrderItems ?? []
      const normalizedItems = itemsRaw.map((it, index) => ({
        orderDetailId:
          it.orderDetailId ?? it.orderDetailID ?? it.OrderDetailID ?? index,
        productId: it.productId ?? it.productID ?? it.ProductID,
        productName: it.productName ?? it.ProductName ?? 'Product',
        productImage: it.productImage ?? it.ProductImage ?? '',
        quantity: it.quantity ?? it.Quantity ?? 0,
        amount: it.amount ?? it.Amount ?? 0,
        netAmount: it.netAmount ?? it.NetAmount ?? it.amount ?? it.Amount ?? 0,
        review: it.review ?? it.Review ?? null
      }))

      setSelectedOrder(normalizedOrder)
      setOrderDetails(normalizedItems)
      setShowModal(true)
    } catch (error) {
      toast.error(getErrorMessage(error) || 'Failed to load order details')
    }
  }

  const closeModal = () => {
    setShowModal(false)
    setSelectedOrder(null)
    setOrderDetails([])
  }

  const getStatusInfo = (status) => {
    return orderStatuses.find(s => s.value === status) || orderStatuses[0]
  }

  const filteredOrders = orders.filter(order => {
    const matchesSearch =
      order.orderNo?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      order.userName?.toLowerCase().includes(searchTerm.toLowerCase())
    const matchesStatus =
      statusFilter === '' || order.status === statusFilter
    return matchesSearch && matchesStatus
  })

  return (
    <div>
      <div className="flex items-center justify-between mb-8">
        <h1 className="font-marcellus text-3xl text-[#111]">Orders Management</h1>
        <div className="flex items-center space-x-4">
          <div className="text-sm text-gray-600">
            Total Orders: <span className="font-semibold">{orders.length}</span>
          </div>
        </div>
      </div>

      {/* Search and Filter */}
      <div className="bg-white rounded-2xl p-6 shadow-md mb-6">
        <div className="flex flex-col md:flex-row gap-4">
          <div className="flex-1 relative">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <input
              type="text"
              placeholder="Search by order number or customer..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none pl-10"
            />
          </div>
          <div className="relative">
            <Filter className="absolute left-3 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira outline-none pl-10 pr-4"
            >
              <option value="">All Statuses</option>
              {orderStatuses.map(status => (
                <option key={status.value} value={status.value}>
                  {status.label}
                </option>
              ))}
            </select>
          </div>
        </div>
      </div>

      {/* Orders Table */}
      <div className="bg-white border border-gray-200 overflow-hidden">
        <div className="overflow-x-auto">
          <table className="w-full">
            <thead className="bg-gray-50 border-b">
              <tr>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Order #</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Customer</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Date</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Total</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Status</th>
                <th className="px-6 py-4 text-left text-sm font-semibold text-gray-700">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y">
              {filteredOrders.map((order) => {
                const statusInfo = getStatusInfo(order.status)
                const StatusIcon = statusInfo.icon
                
                return (
                  <tr key={order.orderId} className="hover:bg-gray-50 transition-colors">
                    <td className="px-6 py-4 font-medium">#{order.orderNo}</td>
                    <td className="px-6 py-4">
                      <div>
                        <p className="font-medium">{order.userName}</p>
                        <p className="text-sm text-gray-500">{order.userEmail}</p>
                      </div>
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-600">
                      {new Date(order.orderDate).toLocaleDateString()}
                    </td>
                    <td className="px-6 py-4 font-jost font-semibold text-[#111]">
                      {formatPrice(order.totalAmount)}
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center space-x-2">
                        <select
                          value={order.status || 'pending'}
                          onChange={(e) => updateOrderStatus(order.orderId, e.target.value)}
                          className={`px-3 py-1 rounded-full text-sm font-medium border-0 ${statusInfo.color}`}
                        >
                          {orderStatuses.map(status => (
                            <option key={status.value} value={status.value}>
                              {status.label}
                            </option>
                          ))}
                        </select>
                      </div>
                    </td>
                    <td className="px-6 py-4">
                      <div className="flex items-center space-x-2">
                        <motion.button
                          whileHover={{ scale: 1.1 }}
                          whileTap={{ scale: 0.9 }}
                          onClick={() => openModal(order)}
                          className="p-2 text-blue-600 hover:bg-blue-50 rounded-lg transition-colors"
                        >
                          <Eye className="w-5 h-5" />
                        </motion.button>
                        <motion.button
                          whileHover={{ scale: 1.1 }}
                          whileTap={{ scale: 0.9 }}
                          onClick={() => handleDelete(order.orderId)}
                          className="p-2 text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        >
                          <Trash2 className="w-5 h-5" />
                        </motion.button>
                      </div>
                    </td>
                  </tr>
                )
              })}
            </tbody>
          </table>
        </div>
      </div>

      {/* Order Details Modal */}
      <AnimatePresence>
        {showModal && selectedOrder && (
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
              className="bg-white rounded-2xl p-8 max-w-4xl w-full max-h-[90vh] overflow-y-auto"
            >
              <div className="flex items-center justify-between mb-6">
                <h2 className="font-marcellus text-2xl text-[#111]">Order Details - #{selectedOrder.orderNo}</h2>
                <button onClick={closeModal} className="p-2 hover:bg-gray-100 rounded-lg">
                  <X className="w-6 h-6" />
                </button>
              </div>

              <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
                {/* Order Info */}
                <div className="space-y-6">
                  <div className="bg-gray-50 rounded-xl p-6">
                    <h3 className="text-lg font-semibold mb-4">Order Information</h3>
                    <div className="space-y-3">
                      <div className="flex justify-between">
                        <span className="text-gray-600">Order Date:</span>
                        <span className="font-medium">
                          {new Date(selectedOrder.orderDate).toLocaleDateString()}
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">Status:</span>
                        <span className={`px-3 py-1 rounded-full text-sm font-medium ${getStatusInfo(selectedOrder.status).color}`}>
                          {getStatusInfo(selectedOrder.status).label}
                        </span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">Total Amount:</span>
                        <span className="font-marcellus font-bold text-[#111]">
                          {formatPrice(selectedOrder.totalAmount)}
                        </span>
                      </div>
                    </div>
                  </div>

                  {selectedOrder.payment && (
                    <div className="bg-gray-50 rounded-xl p-6">
                      <h3 className="text-lg font-semibold mb-4">Payment Information</h3>
                      <div className="space-y-3">
                        <div className="flex justify-between">
                          <span className="text-gray-600">Payment Mode:</span>
                          <span className="font-medium">{selectedOrder.payment.paymentModeName}</span>
                        </div>
                        {selectedOrder.payment.paymentReference && (
                          <div className="flex justify-between">
                            <span className="text-gray-600">Reference:</span>
                            <span className="font-medium text-sm">{selectedOrder.payment.paymentReference}</span>
                          </div>
                        )}
                        <div className="flex justify-between">
                          <span className="text-gray-600">Status:</span>
                          <span className="font-medium">{selectedOrder.payment.paymentStatus}</span>
                        </div>
                        {selectedOrder.payment.transactionID && (
                          <div className="flex justify-between">
                            <span className="text-gray-600">Transaction ID:</span>
                            <span className="font-medium text-sm">{selectedOrder.payment.transactionID}</span>
                          </div>
                        )}
                        <div className="flex justify-between">
                          <span className="text-gray-600">Amount Paid:</span>
                          <span className="font-marcellus font-semibold text-[#111]">
                            {formatPrice(selectedOrder.payment.totalPayment)}
                          </span>
                        </div>
                      </div>
                    </div>
                  )}

                  <div className="bg-gray-50 rounded-xl p-6">
                    <h3 className="text-lg font-semibold mb-4">Customer Information</h3>
                    <div className="space-y-3">
                      <div className="flex justify-between">
                        <span className="text-gray-600">Name:</span>
                        <span className="font-medium">{selectedOrder.userName}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">Email:</span>
                        <span className="font-medium">{selectedOrder.userEmail}</span>
                      </div>
                      <div className="flex justify-between">
                        <span className="text-gray-600">Phone:</span>
                        <span className="font-medium">{selectedOrder.userPhone}</span>
                      </div>
                    </div>
                  </div>

                  <div className="bg-gray-50 rounded-xl p-6">
                    <h3 className="text-lg font-semibold mb-4">Shipping Address</h3>
                    {selectedOrder.shippingAddress ? (
                      <div className="space-y-2 text-sm text-gray-700">
                        {selectedOrder.shippingAddress.receiverName && (
                          <div className="font-semibold">
                            {selectedOrder.shippingAddress.receiverName}
                          </div>
                        )}
                        <div>
                          {selectedOrder.shippingAddress.fullAddress ??
                            selectedOrder.shippingAddress.FullAddress ??
                            `${selectedOrder.shippingAddress.addressLine1 ?? ''}${
                              selectedOrder.shippingAddress.city
                                ? `, ${selectedOrder.shippingAddress.city}`
                                : ''
                            }${
                              selectedOrder.shippingAddress.state
                                ? `, ${selectedOrder.shippingAddress.state}`
                                : ''
                            }${
                              selectedOrder.shippingAddress.pincode
                                ? ` - ${selectedOrder.shippingAddress.pincode}`
                                : ''
                            }`}
                        </div>
                        {selectedOrder.shippingAddress.phone && (
                          <div className="text-sm text-gray-600">
                            Phone: {selectedOrder.shippingAddress.phone}
                          </div>
                        )}
                      </div>
                    ) : (
                      <p className="text-sm text-gray-500">No shipping address available.</p>
                    )}
                  </div>
                </div>

                {/* Order Items */}
                <div>
                  <h3 className="text-lg font-semibold mb-4">Order Items</h3>
                  <div className="space-y-4">
                    {orderDetails.map((item, index) => (
                      <div key={item.orderDetailId ?? item.orderDetailID ?? index} className="bg-gray-50 rounded-xl p-4">
                        <div className="flex items-center space-x-4">
                          <img
                            src={getImageUrl(item.productImage) || getPlaceholderImageUrl(80, 80)}
                            alt={item.productName}
                            className="w-16 h-16 object-cover rounded-lg"
                          />
                          <div className="flex-1">
                            <h4 className="font-semibold">{item.productName}</h4>
                            <div className="flex justify-between items-center mt-2">
                              <span className="text-gray-600">Qty: {item.quantity}</span>
                              <div className="text-right">
                                <p className="text-sm text-gray-600">
                                  {formatPrice(item.amount)} × {item.quantity}
                                </p>
                                <p className="font-jost font-semibold text-[#111]">
                                  {formatPrice(item.netAmount)}
                                </p>
                              </div>
                            </div>
                            {item.review && (
                              <div className="mt-3 pt-3 border-t border-gray-200">
                                <div className="flex items-center gap-2 text-sm font-medium text-gray-700 mb-1">
                                  <MessageSquare className="w-4 h-4 text-kaira" />
                                  Review by customer
                                </div>
                                <div className="flex items-center gap-1 text-yellow-500 mb-1">
                                  {[1, 2, 3, 4, 5].map((star) => (
                                    <Star
                                      key={star}
                                      className={`w-4 h-4 ${star <= (item.review.rating ?? item.review.Rating) ? 'fill-current' : ''}`}
                                    />
                                  ))}
                                  <span className="text-gray-600 text-sm ml-1">
                                    ({item.review.rating ?? item.review.Rating}/5)
                                  </span>
                                </div>
                                <p className="font-medium text-gray-800 text-sm">
                                  {item.review.title ?? item.review.Title}
                                </p>
                                <p className="text-gray-600 text-sm mt-0.5">
                                  {item.review.comment ?? item.review.Comment}
                                </p>
                                {(item.review.image ?? item.review.Image) && (
                                  <img
                                    src={getImageUrl(item.review.image ?? item.review.Image) || (item.review.image ?? item.review.Image)}
                                    alt="Review"
                                    className="mt-2 w-20 h-20 object-cover rounded-lg border"
                                  />
                                )}
                                <p className="text-xs text-gray-400 mt-1">
                                  {new Date(item.review.created ?? item.review.Created).toLocaleDateString()}
                                </p>
                              </div>
                            )}
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              </div>

              <div className="flex gap-4 pt-6 mt-6 border-t">
                <button onClick={closeModal} className="border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors flex-1">
                  Close
                </button>
                <button
                  onClick={() => downloadInvoicePdf(selectedOrder, orderDetails, 'admin')}
                  className="bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex-1"
                >
                  Print Order
                </button>
              </div>
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  )
}

export default AdminOrders
