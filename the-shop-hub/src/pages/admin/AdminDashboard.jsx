import { useState, useEffect } from 'react'
import { motion } from 'framer-motion'
import { useNavigate } from 'react-router-dom'
import { IndianRupee, Package, ShoppingCart, Users, TrendingUp } from 'lucide-react'
import api from '../../utils/api'
import { formatPrice } from '../../utils/formatPrice'

const AdminDashboard = () => {
  const navigate = useNavigate()
  const [stats, setStats] = useState({
    totalRevenue: 0,
    totalProducts: 0,
    totalOrders: 0,
    totalUsers: 0
  })

  useEffect(() => {
    fetchStats()
  }, [])

  const fetchStats = async () => {
    try {
      const [products, orders, users] = await Promise.all([
        api.get('/Product'),
        api.get('/Order'),
        api.get('/User')
      ])

      const revenue = orders.data.reduce((sum, order) => sum + (order.totalAmount || 0), 0)

      setStats({
        totalRevenue: revenue,
        totalProducts: products.data.length,
        totalOrders: orders.data.length,
        totalUsers: users.data.length
      })
    } catch (error) {
      console.error('Error fetching stats:', error)
    }
  }

  const statCards = [
    {
      title: 'Total Revenue',
      value: formatPrice(stats.totalRevenue),
      icon: <IndianRupee className="w-8 h-8" />,
      color: 'bg-green-500',
      trend: '+12.5%'
    },
    {
      title: 'Total Products',
      value: stats.totalProducts,
      icon: <Package className="w-8 h-8" />,
      color: 'bg-blue-500',
      trend: '+5.2%'
    },
    {
      title: 'Total Orders',
      value: stats.totalOrders,
      icon: <ShoppingCart className="w-8 h-8" />,
      color: 'bg-purple-500',
      trend: '+8.1%'
    },
    {
      title: 'Total Users',
      value: stats.totalUsers,
      icon: <Users className="w-8 h-8" />,
      color: 'bg-orange-500',
      trend: '+15.3%'
    }
  ]

  return (
    <div>
      <h1 className="font-marcellus text-3xl text-[#111] mb-8">Dashboard Overview</h1>

      {/* Stats Grid - Kaira */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        {statCards.map((stat, index) => (
          <motion.div
            key={index}
            initial={{ opacity: 0, y: 20 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: index * 0.1 }}
            className="bg-white border border-gray-200 p-6 hover:border-kaira transition-colors"
          >
            <div className="flex items-center justify-between mb-4">
              <div className={`${stat.color} text-white p-3`}>
                {stat.icon}
              </div>
              <div className="flex items-center space-x-1 text-green-600 text-sm font-jost font-semibold">
                <TrendingUp className="w-4 h-4" />
                <span>{stat.trend}</span>
              </div>
            </div>
            <h3 className="font-jost text-gray-600 text-sm mb-1">{stat.title}</h3>
            <p className="font-marcellus text-3xl text-[#111]">{stat.value}</p>
          </motion.div>
        ))}
      </div>

      {/* Quick Actions */}
      <div className="bg-white border border-gray-200 p-6">
        <h2 className="font-marcellus text-2xl text-[#111] mb-6">Quick Actions</h2>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            onClick={() => navigate('/admin/products', { state: { openModal: true } })}
            className="bg-[#111] text-white px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors"
          >
            Add New Product
          </motion.button>
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            onClick={() => navigate('/admin/orders')}
            className="border border-[#111] text-[#111] px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors"
          >
            View Orders
          </motion.button>
          <motion.button
            whileHover={{ scale: 1.02 }}
            whileTap={{ scale: 0.98 }}
            onClick={() => navigate('/admin/users')}
            className="border border-[#111] text-[#111] px-6 py-3 text-sm font-jost uppercase tracking-wide hover:bg-[#111] hover:text-white transition-colors"
          >
            Manage Users
          </motion.button>
        </div>
      </div>
    </div>
  )
}

export default AdminDashboard
