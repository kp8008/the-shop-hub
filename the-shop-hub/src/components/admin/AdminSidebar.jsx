import { Link, useLocation } from 'react-router-dom'
import { motion } from 'framer-motion'
import { 
  LayoutDashboard, 
  Package, 
  FolderTree, 
  ShoppingCart, 
  Users,
  ShoppingBag
} from 'lucide-react'

const AdminSidebar = () => {
  const location = useLocation()

  const menuItems = [
    { path: '/admin', icon: <LayoutDashboard />, label: 'Dashboard', exact: true },
    { path: '/admin/products', icon: <Package />, label: 'Products' },
    { path: '/admin/categories', icon: <FolderTree />, label: 'Categories' },
    { path: '/admin/orders', icon: <ShoppingCart />, label: 'Orders' },
    { path: '/admin/users', icon: <Users />, label: 'Users' },
  ]

  const isActive = (path, exact) => {
    if (exact) {
      return location.pathname === path
    }
    return location.pathname.startsWith(path)
  }

  return (
    <div className="w-64 bg-gray-900 text-white flex flex-col">
      {/* Logo */}
      <div className="p-6 border-b border-gray-800">
        <Link to="/admin" className="flex items-center space-x-3">
          <div className="w-10 h-10 bg-kaira rounded-lg flex items-center justify-center text-white">
            <ShoppingBag className="w-6 h-6" />
          </div>
          <div>
            <h1 className="font-marcellus text-xl text-white">The Shop Hub</h1>
            <p className="text-xs font-jost text-gray-400">Admin Panel</p>
          </div>
        </Link>
      </div>

      {/* Menu */}
      <nav className="flex-1 p-4 space-y-2">
        {menuItems.map((item) => (
          <Link key={item.path} to={item.path}>
            <motion.div
              whileHover={{ x: 5 }}
              className={`flex items-center space-x-3 px-4 py-3 rounded-lg transition-colors font-jost ${
                isActive(item.path, item.exact)
                  ? 'bg-kaira text-white'
                  : 'text-gray-300 hover:bg-gray-800'
              }`}
            >
              <span className="w-5 h-5">{item.icon}</span>
              <span className="font-medium">{item.label}</span>
            </motion.div>
          </Link>
        ))}
      </nav>

      {/* Admin Info */}
      <div className="p-4 border-t border-gray-800">
        <div className="text-center text-sm text-gray-400">
          <p className="font-semibold text-white mb-1">Admin Panel</p>
          <p>Manage your store</p>
        </div>
      </div>
    </div>
  )
}

export default AdminSidebar
