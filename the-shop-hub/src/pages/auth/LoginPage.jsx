import { useState, useRef, useEffect } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import { Mail, Lock, Eye, EyeOff, ShoppingCart, X } from 'lucide-react'
import { GoogleLogin } from '@react-oauth/google'
import { jwtDecode } from 'jwt-decode'
import api from '../../utils/api'
import useAuthStore from '../../store/useAuthStore'
import toast from 'react-hot-toast'

const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID

const LoginPage = () => {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
    selectedRole: 'customer'
  })
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const [showSignUpChoice, setShowSignUpChoice] = useState(false)
  const googleBtnRef = useRef(null)
  const [googleBtnWidth, setGoogleBtnWidth] = useState(320)

  useEffect(() => {
    if (!googleBtnRef.current) return
    const ro = new ResizeObserver(() => {
      if (googleBtnRef.current) setGoogleBtnWidth(googleBtnRef.current.offsetWidth)
    })
    ro.observe(googleBtnRef.current)
    setGoogleBtnWidth(googleBtnRef.current.offsetWidth)
    return () => ro.disconnect()
  }, [showSignUpChoice])

  const navigate = useNavigate()
  const { login } = useAuthStore()

  const handleSubmit = async (e) => {
    e.preventDefault()
    setLoading(true)

    try {
      const response = await api.post('/Auth/login', {
        email: formData.email,
        password: formData.password
      })

      const data = response.data

      const userTypeId = Number(
        data.UserTypeID ??
        data.userTypeID ??
        data.userTypeId ??
        data.usertypeid ??
        data.USERTYPEID
      )

      const userId = Number(
        data.UserID ??
        data.userID ??
        data.userId ??
        data.userid
      )

      const userName = data.UserName ?? data.userName ?? data.username ?? 'User'
      const email = data.Email ?? data.email ?? formData.email
      const token = data.Token ?? data.token

      if (isNaN(userTypeId) || userTypeId === 0) {
        toast.error('Server returned invalid user type. Please contact support.')
        setLoading(false)
        return
      }

      const expectedUserTypeId = formData.selectedRole === 'admin' ? 1 : 2
      if (userTypeId !== expectedUserTypeId) {
        const actualRole = userTypeId === 1 ? 'Admin' : userTypeId === 2 ? 'Customer' : 'Unknown'
        toast.error(`This account is registered as ${actualRole}, but you selected ${formData.selectedRole}. Please select the correct role.`)
        setLoading(false)
        return
      }

      const user = {
        userId: userId,
        firstName: userName.split(' ')[0] || userName,
        lastName: userName.split(' ')[1] || '',
        email: email,
        userTypeId: userTypeId,
        mobile: data.Phone ?? data.phone ?? ''
      }

      login(user, token)

      await new Promise(resolve => setTimeout(resolve, 150))

      toast.success(`Welcome back, ${user.firstName}!`)

      if (userTypeId === 1) {
        setTimeout(() => navigate('/admin', { replace: true }), 200)
      } else if (userTypeId === 2) {
        setTimeout(() => navigate('/', { replace: true }), 200)
      } else {
        toast.error('Unknown user type')
      }
    } catch (error) {
      toast.error(error.response?.data?.message || 'Invalid credentials')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex flex-col lg:flex-row bg-[#fafafa]">
      {/* LEFT: Store name, logo, description - very big */}
      <div className="lg:w-[45%] flex flex-col justify-center items-center lg:items-end lg:pr-16 py-12 px-6 text-center lg:text-right">
        <Link to="/" className="inline-flex flex-col items-center lg:items-end">
          <motion.div
            initial={{ opacity: 0, y: 12 }}
            animate={{ opacity: 1, y: 0 }}
            className="flex items-center gap-4 mb-8"
          >
            <motion.div
              whileHover={{ rotate: 360 }}
              transition={{ duration: 0.5 }}
              className="w-24 h-24 bg-[#111] rounded-2xl flex items-center justify-center"
            >
              <ShoppingCart className="w-14 h-14 text-white" />
            </motion.div>
            <span className="font-marcellus text-5xl lg:text-6xl text-[#111]">The Shop Hub</span>
          </motion.div>
          <motion.p
            initial={{ opacity: 0, y: 8 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ delay: 0.1 }}
            className="font-jost text-gray-600 text-xl lg:text-2xl max-w-md lg:max-w-lg leading-relaxed"
          >
            Your one-stop destination for quality products, great prices, and seamless shopping. Sign in to explore deals and track your orders.
          </motion.p>
        </Link>
      </div>

      {/* RIGHT: Login box - white with rounded edges, dark accents */}
      <div className="lg:w-[55%] flex items-center justify-center py-12 px-6 lg:pl-8">
        <motion.div
          initial={{ opacity: 0, x: 16 }}
          animate={{ opacity: 1, x: 0 }}
          className="w-full max-w-xl"
        >
          <div className="bg-white border border-gray-200 rounded-3xl shadow-sm p-8 relative">
          <button
            type="button"
            onClick={() => navigate('/', { replace: true })}
            className="absolute top-6 right-6 font-jost text-gray-500 hover:text-[#111] hover:underline text-sm"
          >
            Skip
          </button>
          <div className="text-center mb-6">
            <h1 className="font-marcellus text-3xl text-[#111] mb-2">Welcome Back!</h1>
            <p className="font-jost text-gray-600">Sign in to continue shopping</p>
          </div>

          <form onSubmit={handleSubmit} className="space-y-5">
            <div>
              <label className="block text-sm font-semibold mb-2">Login as</label>
              <div className="grid grid-cols-2 gap-3">
                <motion.button
                  type="button"
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  onClick={() => setFormData({ ...formData, selectedRole: 'customer' })}
                  className={`px-4 py-3 border-2 font-jost transition-all duration-200 ${
                    formData.selectedRole === 'customer'
                      ? 'border-kaira bg-kaira-light text-kaira-dark'
                      : 'border-gray-200 hover:border-gray-300 text-gray-700'
                  }`}
                >
                  Customer
                </motion.button>
                <motion.button
                  type="button"
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  onClick={() => setFormData({ ...formData, selectedRole: 'admin' })}
                  className={`px-4 py-3 border-2 font-jost transition-all duration-200 ${
                    formData.selectedRole === 'admin'
                      ? 'border-kaira bg-kaira-light text-kaira-dark'
                      : 'border-gray-200 hover:border-gray-300 text-gray-700'
                  }`}
                >
                  Admin
                </motion.button>
              </div>
            </div>

            <div>
              <label className="block text-sm font-semibold mb-2">Email</label>
              <div className="relative">
                <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type="email"
                  required
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                  placeholder="your@email.com"
                />
              </div>
            </div>

            <div>
              <label className="block text-sm font-semibold mb-2">Password</label>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type={showPassword ? 'text' : 'password'}
                  required
                  value={formData.password}
                  onChange={(e) => setFormData({ ...formData, password: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12 pr-12"
                  placeholder="••••••••"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-4 top-1/2 -translate-y-1/2 text-gray-400 hover:text-gray-600"
                >
                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
            </div>

            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center space-x-2 cursor-pointer">
                <input type="checkbox" className="rounded text-kaira focus:ring-kaira" />
                <span className="font-jost">Remember me</span>
              </label>
              <Link to="/forgot-password" className="font-jost text-kaira hover:underline">
                Forgot password?
              </Link>
            </div>

            <motion.button
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
              type="submit"
              disabled={loading}
              className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? 'Signing in...' : 'Sign In'}
            </motion.button>
          </form>

          <div className="relative my-5">
            <div className="absolute inset-0 flex items-center">
              <div className="w-full border-t border-gray-200" />
            </div>
            <div className="relative flex justify-center text-sm">
              <span className="px-4 bg-white text-gray-500">Other options</span>
            </div>
          </div>

          {googleClientId ? (
            <div ref={googleBtnRef} className="w-full flex justify-center">
              <GoogleLogin
                onSuccess={(credentialResponse) => {
                  try {
                    const decoded = jwtDecode(credentialResponse.credential)
                    navigate('/register', {
                      replace: true,
                      state: {
                        fromGoogle: true,
                        googleEmail: decoded.email || '',
                        googleFirstName: decoded.given_name || '',
                        googleLastName: decoded.family_name || ''
                      }
                    })
                    toast.success('Fill password and mobile to complete sign up')
                  } catch (e) {
                    toast.error('Could not get Google profile')
                  }
                }}
                onError={() => toast.error('Google sign-in failed')}
                useOneTap={false}
                theme="outline"
                size="large"
                type="standard"
                shape="rectangular"
                text="continue_with"
                width={typeof googleBtnWidth === 'number' && googleBtnWidth > 0 ? googleBtnWidth : 320}
              />
            </div>
          ) : (
            <button
              type="button"
              disabled
              className="w-full flex items-center justify-center gap-3 px-4 py-3 border-2 border-gray-200 rounded-lg opacity-60 cursor-not-allowed font-jost"
            >
              <img src="https://www.google.com/favicon.ico" alt="Google" className="w-5 h-5" />
              <span>Google (set VITE_GOOGLE_CLIENT_ID)</span>
            </button>
          )}

          <p className="text-center mt-5 font-jost text-gray-600">
            Don't have an account?{' '}
            <button
              type="button"
              onClick={() => setShowSignUpChoice(true)}
              className="text-kaira font-semibold hover:underline"
            >
              Sign Up
            </button>
          </p>
        </div>

        <AnimatePresence>
          {showSignUpChoice && (
            <>
              <motion.div
                initial={{ opacity: 0 }}
                animate={{ opacity: 1 }}
                exit={{ opacity: 0 }}
                className="fixed inset-0 bg-black/50 z-[9998]"
                onClick={() => setShowSignUpChoice(false)}
              />
              <div className="fixed inset-0 z-[9999] flex items-center justify-center p-4 pointer-events-none">
                <motion.div
                  initial={{ opacity: 0, scale: 0.95 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0.95 }}
                  className="relative w-full max-w-[420px] bg-white border border-gray-200 shadow-xl rounded-2xl p-8 pointer-events-auto"
                  onClick={(e) => e.stopPropagation()}
                >
                  <button
                    type="button"
                    onClick={() => setShowSignUpChoice(false)}
                    className="absolute right-4 top-4 p-1.5 text-gray-400 hover:text-[#111]"
                    aria-label="Close"
                  >
                    <X className="w-5 h-5" />
                  </button>
                  <h3 className="font-marcellus text-2xl text-[#111] mb-6 pr-10">Create an account?</h3>
                  <div className="flex flex-col gap-3">
                    <button
                      type="button"
                      onClick={() => {
                        setShowSignUpChoice(false)
                        navigate('/', { replace: true })
                      }}
                      className="w-full border-2 border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-50"
                    >
                      Skip for now, do it later
                    </button>
                    <button
                      type="button"
                      onClick={() => {
                        setShowSignUpChoice(false)
                        navigate('/register', { replace: true })
                      }}
                      className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-800"
                    >
                      Sign Up
                    </button>
                  </div>
                </motion.div>
              </div>
            </>
          )}
        </AnimatePresence>
        </motion.div>
      </div>
    </div>
  )
}

export default LoginPage
