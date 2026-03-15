import { useState, useEffect, useRef } from 'react'
import { Link, useNavigate, useLocation } from 'react-router-dom'
import { motion } from 'framer-motion'
import { Mail, Lock, User, Eye, EyeOff, ShoppingCart, Phone } from 'lucide-react'
import { GoogleLogin } from '@react-oauth/google'
import { jwtDecode } from 'jwt-decode'
import api from '../../utils/api'
import toast from 'react-hot-toast'
import useAuthStore from '../../store/useAuthStore'

const googleClientId = import.meta.env.VITE_GOOGLE_CLIENT_ID

const RegisterPage = () => {
  const location = useLocation()
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
    mobile: ''
  })
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const googleBtnRef = useRef(null)
  const [googleBtnWidth, setGoogleBtnWidth] = useState(320)
  const navigate = useNavigate()
  const login = useAuthStore((state) => state.login)

  useEffect(() => {
    if (!googleBtnRef.current) return
    const ro = new ResizeObserver(() => {
      if (googleBtnRef.current) setGoogleBtnWidth(googleBtnRef.current.offsetWidth)
    })
    ro.observe(googleBtnRef.current)
    setGoogleBtnWidth(googleBtnRef.current.offsetWidth)
    return () => ro.disconnect()
  }, [])

  // Pre-fill name and email from Google (when coming from Login page after Google sign-in)
  useEffect(() => {
    const state = location.state
    if (state?.fromGoogle && state?.googleEmail) {
      setFormData(prev => ({
        ...prev,
        email: state.googleEmail || prev.email,
        firstName: state.googleFirstName || prev.firstName,
        lastName: state.googleLastName || prev.lastName
      }))
      toast.success('Name and email filled from Google. Enter password and mobile.')
    }
  }, [location.state])

  const handleSubmit = async (e) => {
    e.preventDefault()

    if (formData.password !== formData.confirmPassword) {
      toast.error('Passwords do not match')
      return
    }

    if (formData.password.length < 6) {
      toast.error('Password must be at least 6 characters')
      return
    }

    const mobileDigits = (formData.mobile || '').replace(/\D/g, '')
    if (mobileDigits.length !== 10) {
      toast.error('Please enter a valid 10 digit mobile number')
      return
    }

    setLoading(true)

    try {
      // Backend expects PascalCase (FirstName, LastName, Email, Password, Mobile)
      const registerPayload = {
        FirstName: formData.firstName.trim(),
        LastName: formData.lastName.trim(),
        Email: formData.email.trim(),
        Password: formData.password,
        Mobile: mobileDigits
      }
      const { data } = await api.post('/Auth/register', registerPayload)
      const token = data.token ?? data.Token
      const userId = data.userID ?? data.UserID
      const userName = data.userName ?? data.UserName ?? ''
      const email = data.email ?? data.Email ?? ''
      const userTypeId = data.userTypeID ?? data.UserTypeID ?? 2
      const user = {
        userId,
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email,
        userTypeId,
        mobile: data.phone ?? data.Phone ?? formData.mobile
      }
      if (token) {
        login(user, token)
        toast.success('Account created! Welcome to The Shop Hub.')
        navigate('/', { replace: true })
      } else {
        toast.success('Account created! Please sign in.')
        navigate('/login')
      }
    } catch (error) {
      if (error.response?.status === 404) {
        toast.error('Cannot reach server. Check your connection or try again later.')
        return
      }
      const msg = error.response?.data?.message || error.message || 'Registration failed'
      const isEmailExists = msg.toLowerCase().includes('already exists') || msg.toLowerCase().includes('already registered')
      toast.error(isEmailExists ? 'This email is already registered. Please sign in or use another email.' : msg)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex flex-col lg:flex-row bg-[#fafafa]">
      {/* LEFT: Store name, logo, description - same as login */}
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
            Your one-stop destination for quality products, great prices, and seamless shopping. Create an account to explore deals and track your orders.
          </motion.p>
        </Link>
      </div>

      {/* RIGHT: Create Account form - white rounded box like login */}
      <div className="lg:w-[55%] flex items-center justify-center py-12 px-6 lg:pl-8">
        <motion.div
          initial={{ opacity: 0, x: 16 }}
          animate={{ opacity: 1, x: 0 }}
          className="w-full max-w-xl"
        >
          <div className="bg-white border border-gray-200 rounded-3xl shadow-sm p-8">
          <div className="text-center mb-6">
            <h1 className="font-marcellus text-3xl text-[#111] mb-2">Create Account</h1>
            <p className="font-jost text-gray-600">Join us and start shopping!</p>
          </div>

          {googleClientId && (
            <div className="mb-6 w-full">
              <p className="text-sm text-gray-500 mb-3 font-jost">Fill name & email from Google, then add password and mobile below.</p>
              <div ref={googleBtnRef} className="w-full flex justify-center">
                <GoogleLogin
                  onSuccess={(credentialResponse) => {
                    try {
                      const decoded = jwtDecode(credentialResponse.credential)
                      setFormData(prev => ({
                        ...prev,
                        email: decoded.email || prev.email,
                        firstName: decoded.given_name || prev.firstName,
                        lastName: decoded.family_name || prev.lastName
                      }))
                      toast.success('Name and email filled. Now enter password and mobile.')
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
                  width={googleBtnWidth}
                />
              </div>
            </div>
          )}

          <form onSubmit={handleSubmit} className="space-y-5">
            {/* Name Fields */}
            <div className="grid grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-semibold mb-2">First Name</label>
                <div className="relative">
                  <User className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                  <input
                    type="text"
                    required
                    value={formData.firstName}
                    onChange={(e) => setFormData({ ...formData, firstName: e.target.value })}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                    placeholder="John"
                  />
                </div>
              </div>
              <div>
                <label className="block text-sm font-semibold mb-2">Last Name</label>
                <input
                  type="text"
                  required
                  value={formData.lastName}
                  onChange={(e) => setFormData({ ...formData, lastName: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                  placeholder="Doe"
                />
              </div>
            </div>

            {/* Email */}
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

            {/* Mobile – India +91 fixed (not editable), only 10 digits in box */}
            <div>
              <label className="block text-sm font-semibold mb-2">Mobile</label>
              <div className="flex border border-gray-200 focus-within:border-kaira focus-within:ring-1 focus-within:ring-kaira rounded-sm overflow-hidden">
                <div className="flex items-center gap-1.5 min-w-[100px] px-4 py-3 bg-gray-100 border-r border-gray-200 text-gray-800 font-jost font-medium select-none shrink-0">
                  <span className="text-lg" aria-hidden>🇮🇳</span>
                  <span>+91</span>
                </div>
                <input
                  type="tel"
                  inputMode="numeric"
                  pattern="[0-9]*"
                  autoComplete="tel-national"
                  required
                  maxLength={10}
                  value={formData.mobile}
                  onInput={(e) => {
                    const v = e.target.value.replace(/\D/g, '').slice(0, 10)
                    setFormData(prev => ({ ...prev, mobile: v }))
                  }}
                  onChange={(e) => {
                    const v = e.target.value.replace(/\D/g, '').slice(0, 10)
                    setFormData(prev => ({ ...prev, mobile: v }))
                  }}
                  className="w-full px-4 py-3 font-jost outline-none"
                  placeholder="9876543210"
                />
              </div>
              <p className="mt-1 text-xs text-gray-500 font-jost">10 digit mobile number only</p>
            </div>

            {/* Password */}
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

            {/* Confirm Password */}
            <div>
              <label className="block text-sm font-semibold mb-2">Confirm Password</label>
              <div className="relative">
                <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                <input
                  type={showPassword ? 'text' : 'password'}
                  required
                  value={formData.confirmPassword}
                  onChange={(e) => setFormData({ ...formData, confirmPassword: e.target.value })}
                  className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                  placeholder="••••••••"
                />
              </div>
            </div>

            {/* Terms */}
            <label className="flex items-start space-x-2 cursor-pointer text-sm">
              <input type="checkbox" required className="mt-1 rounded text-kaira focus:ring-kaira" />
              <span className="text-gray-600">
                I agree to the{' '}
                <Link to="/terms" className="text-kaira hover:underline">Terms of Service</Link>
                {' '}and{' '}
                <Link to="/privacy" className="text-kaira hover:underline">Privacy Policy</Link>
              </span>
            </label>

            {/* Submit Button */}
            <motion.button
              whileHover={{ scale: 1.02 }}
              whileTap={{ scale: 0.98 }}
              type="submit"
              disabled={loading}
              className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-kaira-dark transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {loading ? 'Creating Account...' : 'Create Account'}
            </motion.button>
          </form>

          {/* Sign In Link */}
          <p className="text-center mt-5 font-jost text-gray-600">
            Already have an account?{' '}
            <Link to="/login" className="text-kaira font-semibold hover:underline">
              Sign In
            </Link>
          </p>
          </div>
        </motion.div>
      </div>
    </div>
  )
}

export default RegisterPage
