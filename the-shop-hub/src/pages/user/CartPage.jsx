import { Link } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import { Trash2, Plus, Minus, ShoppingBag, ArrowRight } from 'lucide-react'
import useCartStore from '../../store/useCartStore'
import useAuthStore from '../../store/useAuthStore'
import { formatPrice } from '../../utils/formatPrice'
import { getPlaceholderImageUrl } from '../../utils/api'

const CartPage = () => {
  const { items, updateQuantity, removeItem, getTotal } = useCartStore()
  const { isAuthenticated } = useAuthStore()

  const total = getTotal()
  const shipping = total > 500 ? 0 : 50
  const tax = total * 0.1
  const grandTotal = total + shipping + tax

  if (items.length === 0) {
    return (
      <div className="py-16 min-h-screen flex items-center justify-center bg-kaira-light">
        <motion.div
          initial={{ opacity: 0, scale: 0.9 }}
          animate={{ opacity: 1, scale: 1 }}
          className="text-center"
        >
          <div className="w-32 h-32 bg-kaira-light border border-gray-200 flex items-center justify-center mx-auto mb-6 text-kaira">
            <ShoppingBag className="w-16 h-16" />
          </div>
          <h2 className="font-marcellus text-3xl text-[#111] mb-4">Your cart is empty</h2>
          <p className="font-jost text-gray-600 mb-8">Add some products to get started!</p>
          <Link to="/products">
            <motion.button
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
              className="bg-[#111] text-white px-8 py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
            >
              Start Shopping
            </motion.button>
          </Link>
        </motion.div>
      </div>
    )
  }

  return (
    <div className="py-16 min-h-screen bg-kaira-light">
      <div className="max-w-[1800px] mx-auto px-4 md:px-6">
        <h1 className="font-marcellus text-3xl md:text-4xl text-[#111] mb-8">Shopping Cart</h1>

        <div className="grid lg:grid-cols-3 gap-8">
          {/* Cart Items */}
          <div className="lg:col-span-2 space-y-4">
            <AnimatePresence>
              {items.map((item) => (
                <motion.div
                  key={item.productId}
                  initial={{ opacity: 0, x: -20 }}
                  animate={{ opacity: 1, x: 0 }}
                  exit={{ opacity: 0, x: 20 }}
                  className="bg-white border border-gray-200 p-6 hover:border-kaira transition-colors"
                >
                  <div className="flex gap-6">
                    <Link to={`/products/${item.productId}`} className="flex-shrink-0">
                      <img
                        src={item.imageUrl || getPlaceholderImageUrl(150, 150)}
                        alt={item.productName}
                        className="w-32 h-32 object-cover rounded-xl"
                      />
                    </Link>

                    <div className="flex-1">
                      <div className="flex justify-between mb-2">
                        <Link to={`/products/${item.productId}`}>
                          <h3 className="font-marcellus text-xl text-[#111] hover:text-kaira transition-colors">
                            {item.productName}
                          </h3>
                        </Link>
                        <motion.button
                          whileHover={{ scale: 1.1 }}
                          whileTap={{ scale: 0.9 }}
                          onClick={() => removeItem(item.productId)}
                          className="text-red-500 hover:text-red-700"
                        >
                          <Trash2 className="w-5 h-5" />
                        </motion.button>
                      </div>

                      <p className="font-jost text-xl font-medium text-[#111] mb-4">
                        {formatPrice(item.price)}
                      </p>

                      <div className="flex items-center justify-between">
                        <div className="flex items-center space-x-3 bg-gray-100 rounded-lg p-2">
                          <motion.button
                            whileHover={{ scale: 1.1 }}
                            whileTap={{ scale: 0.9 }}
                            onClick={() => updateQuantity(item.productId, item.quantity - 1)}
                            className="w-8 h-8 flex items-center justify-center bg-white rounded-lg hover:bg-gray-200 transition-colors"
                          >
                            <Minus className="w-4 h-4" />
                          </motion.button>
                          <span className="w-12 text-center font-semibold">{item.quantity}</span>
                          <motion.button
                            whileHover={{ scale: 1.1 }}
                            whileTap={{ scale: 0.9 }}
                            onClick={() => updateQuantity(item.productId, item.quantity + 1)}
                            className="w-8 h-8 flex items-center justify-center bg-white rounded-lg hover:bg-gray-200 transition-colors"
                          >
                            <Plus className="w-4 h-4" />
                          </motion.button>
                        </div>

                        <p className="font-jost text-xl font-medium text-[#111]">
                          {formatPrice(item.price * item.quantity)}
                        </p>
                      </div>
                    </div>
                  </div>
                </motion.div>
              ))}
            </AnimatePresence>
          </div>

          {/* Order Summary */}
          <div className="lg:col-span-1">
            <motion.div
              initial={{ opacity: 0, y: 20 }}
              animate={{ opacity: 1, y: 0 }}
              className="bg-white border border-gray-200 p-6 sticky top-24"
            >
              <h2 className="font-marcellus text-xl text-[#111] mb-6">Order Summary</h2>

              <div className="space-y-4 mb-6 font-jost">
                <div className="flex justify-between text-gray-600">
                  <span>Subtotal</span>
                  <span className="font-medium text-[#111]">{formatPrice(total)}</span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Shipping</span>
                  <span className="font-medium text-[#111]">
                    {shipping === 0 ? 'FREE' : formatPrice(shipping)}
                  </span>
                </div>
                <div className="flex justify-between text-gray-600">
                  <span>Tax (10%)</span>
                  <span className="font-medium text-[#111]">{formatPrice(tax)}</span>
                </div>
                <div className="border-t border-gray-200 pt-4">
                  <div className="flex justify-between text-xl font-marcellus text-[#111]">
                    <span>Total</span>
                    <span>{formatPrice(grandTotal)}</span>
                  </div>
                </div>
              </div>

              {shipping > 0 && (
                <div className="bg-kaira-light border border-kaira p-4 mb-6">
                  <p className="font-jost text-sm text-kaira-dark">
                    Add <span className="font-semibold">{formatPrice(500 - total)}</span> more to get FREE shipping!
                  </p>
                </div>
              )}

              {isAuthenticated ? (
                <Link to="/checkout">
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors flex items-center justify-center space-x-2"
                  >
                    <span>Proceed to Checkout</span>
                    <ArrowRight className="w-5 h-5" />
                  </motion.button>
                </Link>
              ) : (
                <Link to="/login">
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
                  >
                    Login to Checkout
                  </motion.button>
                </Link>
              )}

              <Link to="/products">
                <motion.button
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  className="w-full border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors mt-4"
                >
                  Continue Shopping
                </motion.button>
              </Link>
            </motion.div>
          </div>
        </div>
      </div>
    </div>
  )
}

export default CartPage
