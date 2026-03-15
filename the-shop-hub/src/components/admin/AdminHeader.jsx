import { Bell, User, LogOut } from 'lucide-react'
import { useNavigate } from 'react-router-dom'
import useAuthStore from '../../store/useAuthStore'
import toast from 'react-hot-toast'

const AdminHeader = () => {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    toast.success('Logged out successfully')
    navigate('/login')
  }

  return (
    <header className="bg-white border-b border-gray-200 px-6 py-4">
      <div className="flex items-center justify-between">
        <div>
          <h2 className="font-marcellus text-2xl text-[#111]">Admin Dashboard</h2>
          <p className="font-jost text-gray-600 text-sm">Manage your e-commerce store</p>
        </div>

        <div className="flex items-center space-x-4">
          <button className="relative p-2 text-gray-600 hover:bg-kaira-light transition-colors">
            <Bell className="w-6 h-6" />
            <span className="absolute top-1 right-1 w-2 h-2 bg-red-500 rounded-full"></span>
          </button>

          <div className="flex items-center space-x-3 pl-4 border-l border-gray-200">
            <div className="w-10 h-10 bg-kaira-light rounded-full flex items-center justify-center text-kaira">
              <User className="w-6 h-6" />
            </div>
            <div>
              <p className="font-jost font-semibold text-[#111]">{user?.firstName} {user?.lastName}</p>
              <p className="text-xs font-jost text-gray-600">Administrator</p>
            </div>
            <button
              onClick={handleLogout}
              className="ml-2 p-2 text-gray-600 hover:bg-red-50 hover:text-red-600 rounded-lg transition-colors"
              title="Logout"
            >
              <LogOut className="w-5 h-5" />
            </button>
          </div>
        </div>
      </div>
    </header>
  )
}

export default AdminHeader
