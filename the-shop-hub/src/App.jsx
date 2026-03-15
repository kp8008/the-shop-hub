import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom'
import { Toaster } from 'react-hot-toast'
import useAuthStore from './store/useAuthStore'
import ScrollToTop from './components/ScrollToTop'

// User Pages
import HomePage from './pages/user/HomePage'
import ProductsPage from './pages/user/ProductsPage'
import ProductDetailPage from './pages/user/ProductDetailPage'
import CartPage from './pages/user/CartPage'
import CheckoutPage from './pages/user/CheckoutPage'
import WishlistPage from './pages/user/WishlistPage'
import LoginPage from './pages/auth/LoginPage'
import RegisterPage from './pages/auth/RegisterPage'
import ForgotPasswordPage from './pages/auth/ForgotPasswordPage'
import ProfilePage from './pages/user/ProfilePage'
import OrdersPage from './pages/user/OrdersPage'
import DebugAuth from './pages/DebugAuth'

// Admin Pages
import AdminDashboard from './pages/admin/AdminDashboard'
import AdminProducts from './pages/admin/AdminProducts'
import AdminCategories from './pages/admin/AdminCategories'
import AdminOrders from './pages/admin/AdminOrders'
import AdminUsers from './pages/admin/AdminUsers'

// Layouts
import UserLayout from './layouts/UserLayout'
import AdminLayout from './layouts/AdminLayout'

// For admin-only routes (admin dashboard)
const ProtectedRoute = ({ children, requireAdmin = false }) => {
  const { isAuthenticated, user } = useAuthStore()
  if (!isAuthenticated || !user) return <Navigate to="/login" replace />
  if (requireAdmin && user.userTypeId !== 1) return <Navigate to="/" replace />
  if (!requireAdmin && user.userTypeId === 1) return <Navigate to="/admin" replace />
  return children
}

// For customer-only routes (cart, wishlist, etc.) – guest is redirected to login
const RequireCustomerAuth = ({ children }) => {
  const { isAuthenticated, user } = useAuthStore()
  if (!isAuthenticated || !user) return <Navigate to="/login" replace />
  if (user.userTypeId === 1) return <Navigate to="/admin" replace />
  return children
}

function App() {
  const { isAuthenticated, user } = useAuthStore()

  return (
    <Router>
      <ScrollToTop />
      <Toaster 
        position="top-right"
        toastOptions={{
          duration: 3000,
          style: {
            background: '#363636',
            color: '#fff',
          },
          success: {
            duration: 3000,
            iconTheme: {
              primary: '#10b981',
              secondary: '#fff',
            },
          },
          error: {
            duration: 4000,
            iconTheme: {
              primary: '#ef4444',
              secondary: '#fff',
            },
          },
        }}
      />
      
      <Routes>
        {/* Debug Route - Accessible without auth */}
        <Route path="/debug-auth" element={<DebugAuth />} />
        
        {/* Public Routes - Only Login/Register */}
        <Route path="/login" element={
          isAuthenticated ? (
            user?.userTypeId === 1 ? <Navigate to="/admin" replace /> : <Navigate to="/" replace />
          ) : (
            <LoginPage />
          )
        } />
        <Route path="/register" element={
          isAuthenticated ? (
            user?.userTypeId === 1 ? <Navigate to="/admin" replace /> : <Navigate to="/" replace />
          ) : (
            <RegisterPage />
          )
        } />
        <Route path="/forgot-password" element={
          isAuthenticated ? (
            user?.userTypeId === 1 ? <Navigate to="/admin" replace /> : <Navigate to="/" replace />
          ) : (
            <ForgotPasswordPage />
          )
        } />

        {/* User Routes - Guest can browse home/shop; cart/wishlist/etc require login */}
        <Route path="/" element={<UserLayout />}>
          <Route index element={<HomePage />} />
          <Route path="products" element={<ProductsPage />} />
          <Route path="products/:id" element={<ProductDetailPage />} />
          <Route path="cart" element={<RequireCustomerAuth><CartPage /></RequireCustomerAuth>} />
          <Route path="checkout" element={<RequireCustomerAuth><CheckoutPage /></RequireCustomerAuth>} />
          <Route path="wishlist" element={<RequireCustomerAuth><WishlistPage /></RequireCustomerAuth>} />
          <Route path="profile" element={<RequireCustomerAuth><ProfilePage /></RequireCustomerAuth>} />
          <Route path="orders" element={<RequireCustomerAuth><OrdersPage /></RequireCustomerAuth>} />
        </Route>

        {/* Admin Routes - Protected, Admin Only */}
        <Route path="/admin" element={
          <ProtectedRoute requireAdmin={true}>
            <AdminLayout />
          </ProtectedRoute>
        }>
          <Route index element={<AdminDashboard />} />
          <Route path="products" element={<AdminProducts />} />
          <Route path="categories" element={<AdminCategories />} />
          <Route path="orders" element={<AdminOrders />} />
          <Route path="users" element={<AdminUsers />} />
        </Route>

        {/* Catch all - guest goes to home, logged-in to dashboard */}
        <Route path="*" element={
          <Navigate to={
            isAuthenticated 
              ? (user?.userTypeId === 1 ? "/admin" : "/")
              : "/"
          } replace />
        } />
      </Routes>
    </Router>
  )
}

export default App
