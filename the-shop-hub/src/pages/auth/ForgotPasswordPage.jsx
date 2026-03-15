import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { motion } from 'framer-motion'
import { Mail, Lock, Eye, EyeOff, ShoppingCart } from 'lucide-react'
import api from '../../utils/api'
import toast from 'react-hot-toast'

const ForgotPasswordPage = () => {
  const [step, setStep] = useState(1)
  const [email, setEmail] = useState('')
  const [otp, setOtp] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [confirmPassword, setConfirmPassword] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const navigate = useNavigate()

  const handleSendOtp = async (e) => {
    e.preventDefault()
    if (!email.trim()) {
      toast.error('Enter your email')
      return
    }
    setLoading(true)
    try {
      await api.post('/Auth/request-password-reset-otp', { email: email.trim() })
      toast.success('OTP sent to your email. Check inbox.')
      setStep(2)
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to send OTP')
    } finally {
      setLoading(false)
    }
  }

  const handleResetPassword = async (e) => {
    e.preventDefault()
    if (newPassword.length < 6) {
      toast.error('Password must be at least 6 characters')
      return
    }
    if (newPassword !== confirmPassword) {
      toast.error('Passwords do not match')
      return
    }
    if (!otp.trim() || otp.trim().length !== 6) {
      toast.error('Enter valid 6-digit OTP')
      return
    }
    setLoading(true)
    try {
      await api.post('/Auth/verify-otp-reset-password', {
        email: email.trim(),
        otp: otp.trim(),
        newPassword
      })
      toast.success('Password reset successfully. Sign in with new password.')
      navigate('/login', { replace: true })
    } catch (err) {
      toast.error(err.response?.data?.message || 'Failed to reset password')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="min-h-screen flex flex-col lg:flex-row bg-[#fafafa]">
      {/* LEFT: Logo + description */}
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
            Reset your password using the OTP we send to your registered email.
          </motion.p>
        </Link>
      </div>

      {/* RIGHT: Form */}
      <div className="lg:w-[55%] flex items-center justify-center py-12 px-6 lg:pl-8">
        <motion.div
          initial={{ opacity: 0, x: 16 }}
          animate={{ opacity: 1, x: 0 }}
          className="w-full max-w-xl"
        >
          <div className="bg-white border border-gray-200 rounded-3xl shadow-sm p-8 relative">
            <div className="text-center mb-6">
              <h1 className="font-marcellus text-3xl text-[#111] mb-2">Reset Password</h1>
              <p className="font-jost text-gray-600">
                {step === 1 ? 'Enter your email to receive OTP' : 'Enter OTP and new password'}
              </p>
            </div>

            {step === 1 ? (
              <form onSubmit={handleSendOtp} className="space-y-5">
                <div>
                  <label className="block text-sm font-semibold mb-2">Email</label>
                  <div className="relative">
                    <Mail className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                    <input
                      type="email"
                      required
                      value={email}
                      onChange={(e) => setEmail(e.target.value)}
                      className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                      placeholder="your@email.com"
                    />
                  </div>
                </div>
                <motion.button
                  whileHover={{ scale: 1.02 }}
                  whileTap={{ scale: 0.98 }}
                  type="submit"
                  disabled={loading}
                  className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-800 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                >
                  {loading ? 'Sending...' : 'Send OTP'}
                </motion.button>
              </form>
            ) : (
              <form onSubmit={handleResetPassword} className="space-y-5">
                <div>
                  <label className="block text-sm font-semibold mb-2">Email</label>
                  <input
                    type="email"
                    value={email}
                    readOnly
                    className="w-full px-4 py-3 border border-gray-200 font-jost bg-gray-50 text-gray-600"
                  />
                </div>
                <div>
                  <label className="block text-sm font-semibold mb-2">OTP (6 digits)</label>
                  <input
                    type="text"
                    required
                    maxLength={6}
                    value={otp}
                    onChange={(e) => setOtp(e.target.value.replace(/\D/g, ''))}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none"
                    placeholder="123456"
                  />
                </div>
                <div>
                  <label className="block text-sm font-semibold mb-2">New Password</label>
                  <div className="relative">
                    <Lock className="absolute left-4 top-1/2 -translate-y-1/2 w-5 h-5 text-gray-400" />
                    <input
                      type={showPassword ? 'text' : 'password'}
                      required
                      minLength={6}
                      value={newPassword}
                      onChange={(e) => setNewPassword(e.target.value)}
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
                <div>
                  <label className="block text-sm font-semibold mb-2">Confirm Password</label>
                  <input
                    type="password"
                    required
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    className="w-full px-4 py-3 border border-gray-200 font-jost focus:border-kaira focus:ring-1 focus:ring-kaira outline-none pl-12"
                    placeholder="••••••••"
                  />
                </div>
                <div className="flex gap-3">
                  <button
                    type="button"
                    onClick={() => setStep(1)}
                    className="flex-1 border-2 border-gray-300 text-gray-700 py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-50"
                  >
                    Back
                  </button>
                  <motion.button
                    whileHover={{ scale: 1.02 }}
                    whileTap={{ scale: 0.98 }}
                    type="submit"
                    disabled={loading}
                    className="flex-1 bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-800 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    {loading ? 'Resetting...' : 'Reset Password'}
                  </motion.button>
                </div>
              </form>
            )}

            <p className="text-center mt-5 font-jost text-gray-600">
              Remember password?{' '}
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

export default ForgotPasswordPage
