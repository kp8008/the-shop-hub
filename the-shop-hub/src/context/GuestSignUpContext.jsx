import { createContext, useContext, useState, useCallback } from 'react'
import { useNavigate } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import { X } from 'lucide-react'

const GuestSignUpContext = createContext(null)

export function GuestSignUpProvider({ children }) {
  const [open, setOpen] = useState(false)
  const navigate = useNavigate()

  const showSignUpPrompt = useCallback(() => {
    setOpen(true)
  }, [])

  const close = useCallback(() => setOpen(false), [])

  const handleSkip = useCallback(() => {
    close()
  }, [close])

  const handleSignUp = useCallback(() => {
    close()
    navigate('/register')
  }, [close, navigate])

  const handleLogin = useCallback(() => {
    close()
    navigate('/login')
  }, [close, navigate])

  return (
    <GuestSignUpContext.Provider value={{ showSignUpPrompt }}>
      {children}
      <AnimatePresence>
        {open && (
          <>
            <motion.div
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className="fixed inset-0 bg-black/50 z-[9998]"
              onClick={close}
            />
            <motion.div
              initial={{ opacity: 0, scale: 0.95 }}
              animate={{ opacity: 1, scale: 1 }}
              exit={{ opacity: 0, scale: 0.95 }}
              className="fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-full max-w-md bg-white border border-gray-200 shadow-xl z-[9999] p-8"
              onClick={e => e.stopPropagation()}
            >
              <button
                type="button"
                onClick={close}
                className="absolute right-4 top-4 p-1 text-gray-400 hover:text-[#111]"
                aria-label="Close"
              >
                <X className="w-5 h-5" />
              </button>
              <h3 className="font-marcellus text-2xl text-[#111] mb-2">Sign up to continue</h3>
              <p className="font-jost text-gray-600 mb-6">
                Create an account to add items to cart, save wishlist, and checkout.
              </p>
              <div className="flex flex-col gap-3">
                <button
                  type="button"
                  onClick={handleSignUp}
                  className="w-full bg-[#111] text-white py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-800"
                >
                  Sign Up
                </button>
                <button
                  type="button"
                  onClick={handleLogin}
                  className="w-full border border-[#111] text-[#111] py-3 text-sm font-jost uppercase tracking-wide hover:bg-gray-50"
                >
                  Login
                </button>
                <button
                  type="button"
                  onClick={handleSkip}
                  className="w-full text-gray-500 py-2 text-sm font-jost hover:text-[#111]"
                >
                  Maybe later
                </button>
              </div>
            </motion.div>
          </>
        )}
      </AnimatePresence>
    </GuestSignUpContext.Provider>
  )
}

export function useGuestSignUpPrompt() {
  const ctx = useContext(GuestSignUpContext)
  return ctx
}
