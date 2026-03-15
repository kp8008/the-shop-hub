import { useState, useEffect } from 'react'
import { motion } from 'framer-motion'
import { Package, Clock, CheckCircle, XCircle, Truck, MessageSquare } from 'lucide-react'
import useAuthStore from '../../store/useAuthStore'
import api, { getPlaceholderImageUrl } from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'
import { downloadInvoicePdf } from '../../utils/invoicePdf'
import ReviewModal from '../../components/user/ReviewModal'
import toast from 'react-hot-toast'

const OrdersPage = () => {
  const { user } = useAuthStore()
  const [orders, setOrders] = useState([])
  const [loading, setLoading] = useState(true)
  const [reviewModal, setReviewModal] = useState({ open: false, productId: null, productName: '' })

  useEffect(() => {
    fetchOrders()
  }, [])

  const fetchOrders = async () => {
    try {
      const response = await api.get('/Order/MyOrders')
      const paymentRaw = (o) => o.payment ?? o.Payment
      const userOrders = response.data.map(o => ({
        orderId: o.orderId ?? o.orderID ?? o.OrderID,
        orderNo: o.orderNo ?? o.OrderNo,
        orderDate: o.orderDate ?? o.OrderDate,
        deliveryDate: o.deliveryDate ?? o.DeliveryDate ?? null,
        totalAmount: o.netAmount ?? o.NetAmount ?? o.totalAmount ?? o.TotalAmount,
        status: (o.status ?? o.Status ?? 'pending').toLowerCase(),
        shippingAddress: o.shippingAddress ?? o.ShippingAddress ?? null,
        items: o.orderItems ?? o.OrderItems ?? [],
        itemsCount: o.orderItemsCount ?? o.OrderItemsCount ?? 0,
        totalQuantity: o.totalQuantity ?? o.TotalQuantity ?? 0,
        payment: paymentRaw(o) ? {
          paymentModeName: paymentRaw(o).paymentModeName ?? paymentRaw(o).PaymentModeName ?? 'Cash on Delivery',
          paymentReference: paymentRaw(o).paymentReference ?? paymentRaw(o).PaymentReference ?? '',
          paymentStatus: paymentRaw(o).paymentStatus ?? paymentRaw(o).PaymentStatus ?? '',
          transactionID: paymentRaw(o).transactionID ?? paymentRaw(o).TransactionID ?? ''
        } : null
      }))
      setOrders(userOrders)
    } catch (error) {
      toast.error('Failed to load orders')
    } finally {
      setLoading(false)
    }
  }

  const getStatusIcon = (status) => {
    switch (status?.toLowerCase()) {
      case 'pending':
        return <Clock className="w-5 h-5 text-yellow-500" />
      case 'processing':
        return <Clock className="w-5 h-5 text-blue-500" />
      case 'shipped':
        return <Truck className="w-5 h-5 text-purple-500" />
      case 'delivered':
        return <CheckCircle className="w-5 h-5 text-green-500" />
      case 'cancelled':
        return <XCircle className="w-5 h-5 text-red-500" />
      default:
        return <Package className="w-5 h-5 text-gray-500" />
    }
  }

  if (loading) {
    return (
      <div className="py-16 min-h-screen flex items-center justify-center bg-kaira-light">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-[#111]"></div>
      </div>
    )
  }

  return (
    <div className="py-16 min-h-screen bg-kaira-light">
      <div className="max-w-[1800px] mx-auto px-4 md:px-6">
        <h1 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-8">My Orders</h1>

        {orders.length === 0 ? (
          <div className="text-center py-20">
            <Package className="w-20 h-20 text-gray-300 mx-auto mb-4" />
            <h2 className="font-marcellus text-2xl text-gray-500 mb-2">No orders yet</h2>
            <p className="font-jost text-gray-500">Start shopping to see your orders here!</p>
          </div>
        ) : (
          <div className="space-y-4">
            {orders.map((order, index) => (
              <motion.div
                key={order.orderId}
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ delay: index * 0.1 }}
                className="bg-white border border-gray-200 p-6 hover:border-kaira transition-colors"
              >
                <div className="flex items-center justify-between mb-4">
                  <div>
                    <h3 className="font-marcellus text-lg text-[#111]">Order #{order.orderNo || order.orderId}</h3>
                    <p className="font-jost text-gray-600 text-sm">
                      {new Date(order.orderDate).toLocaleDateString()}
                    </p>
                    {order.deliveryDate && (
                      <p className="text-gray-500 text-xs">Delivery: {new Date(order.deliveryDate).toLocaleDateString()}</p>
                    )}
                  </div>
                  <div className="flex items-center space-x-2">
                    {getStatusIcon(order.status)}
                    <span className="font-semibold capitalize">{order.status}</span>
                  </div>
                </div>
                <div className="grid md:grid-cols-3 gap-4 pt-4 border-t border-gray-200">
                  <div className="md:col-span-2">
                    <p className="text-sm text-gray-500 mb-2">
                      Items ({order.itemsCount}) • Qty {order.totalQuantity}
                    </p>
                    <div className="space-y-3 max-h-44 overflow-auto pr-2">
                      {order.items?.map((it) => (
                        <div key={it.orderDetailID ?? it.OrderDetailID} className="flex items-center gap-3">
                          <img
                            src={it.productImage || it.ProductImage || getPlaceholderImageUrl(56, 56)}
                            alt={it.productName || it.ProductName}
                            className="w-14 h-14 object-cover rounded"
                          />
                          <div className="flex-1 min-w-0">
                            <div className="font-medium">{it.productName || it.ProductName}</div>
                            <div className="text-xs text-gray-500">Qty: {it.quantity || it.Quantity}</div>
                            {order.status === 'delivered' && (
                              <button
                                type="button"
                                onClick={() => setReviewModal({
                                  open: true,
                                  productId: it.productID ?? it.ProductID,
                                  productName: it.productName || it.ProductName
                                })}
                                className="mt-1 text-xs font-jost text-kaira hover:underline flex items-center gap-1"
                              >
                                <MessageSquare className="w-3.5 h-3.5" /> Review product
                              </button>
                            )}
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                  <div>
                    <p className="text-gray-600">Total Amount</p>
                    <p className="font-marcellus text-xl text-[#111]">
                      {formatPrice(order.totalAmount)}
                    </p>
                    {order.shippingAddress && (
                      <div className="mt-3 text-xs text-gray-600">
                        <div className="font-semibold mb-1">Shipping</div>
                        <div>{order.shippingAddress?.FullAddress || order.shippingAddress?.fullAddress}</div>
                      </div>
                    )}
                    <button
                      onClick={() => downloadInvoicePdf(order, order.items, 'customer')}
                      className="mt-4 text-sm font-jost text-kaira hover:underline"
                    >
                      Download Invoice
                    </button>
                  </div>
                </div>
              </motion.div>
            ))}
          </div>
        )}

        <ReviewModal
          isOpen={reviewModal.open}
          onClose={() => setReviewModal({ open: false, productId: null, productName: '' })}
          productId={reviewModal.productId}
          productName={reviewModal.productName}
          onSuccess={() => setReviewModal({ open: false, productId: null, productName: '' })}
        />
      </div>
    </div>
  )
}

export default OrdersPage
