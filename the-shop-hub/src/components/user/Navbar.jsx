import { useState, useEffect, useMemo } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { motion, AnimatePresence } from 'framer-motion'
import {
  ShoppingCart,
  User,
  Search,
  Menu,
  X,
  Heart,
  LogOut,
  Package,
  Settings
} from 'lucide-react'
import useAuthStore from '../../store/useAuthStore'
import useCartStore from '../../store/useCartStore'
import useFavoriteStore from '../../store/useFavoriteStore'
import { useGuestSignUpPrompt } from '../../context/GuestSignUpContext'
import api from '../../utils/api'

const Navbar = () => {
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false)
  const [isUserMenuOpen, setIsUserMenuOpen] = useState(false)
  const [searchOpen, setSearchOpen] = useState(false)
  const [searchQuery, setSearchQuery] = useState('')
  const [navCategories, setNavCategories] = useState([])

  const { user, isAuthenticated, logout } = useAuthStore()
  const { showSignUpPrompt } = useGuestSignUpPrompt() || {}
  const { getItemCount } = useCartStore()
  const { favorites, fetchFavorites } = useFavoriteStore()
  const navigate = useNavigate()

  const cartItemCount = getItemCount()
  const wishlistCount = favorites.length

  useEffect(() => {
    if (isAuthenticated) {
      fetchFavorites()
    }
  }, [isAuthenticated])

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const res = await api.get('/Category')
        const mapped = res.data.map(c => ({
          id: c.categoryId ?? c.categoryID ?? c.CategoryID,
          name: c.categoryName ?? c.CategoryName
        }))
        setNavCategories(mapped)
      } catch {
        setNavCategories([])
      }
    }
    fetchCategories()
  }, [])

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  const executeSearch = () => {
    const q = searchQuery.trim()
    if (q.length === 0) return
    navigate(`/products?search=${encodeURIComponent(q)}`)
    setSearchOpen(false)
    setSearchQuery('')
    setIsMobileMenuOpen(false)
  }

  const marqueeCategories = useMemo(() => {
    if (!navCategories.length) return []
    return [...navCategories, ...navCategories]
  }, [navCategories])

  return (
    <>
      {/* Kaira-style search overlay */}
      <AnimatePresence>
        {searchOpen && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-white z-[9999] flex items-center justify-center p-4"
            onClick={() => setSearchOpen(false)}
          >
            <div
              className="w-full max-w-2xl text-center"
              onClick={e => e.stopPropagation()}
            >
              <form
                onSubmit={e => {
                  e.preventDefault()
                  executeSearch()
                }}
                className="relative border-b border-gray-300 pb-2"
              >
                <input
                  type="search"
                  placeholder="Type and press enter"
                  value={searchQuery}
                  onChange={e => setSearchQuery(e.target.value)}
                  className="w-full text-2xl md:text-3xl bg-transparent border-0 outline-none text-[#111] placeholder:text-gray-400 py-4"
                  autoFocus
                />
                <button
                  type="submit"
                  className="absolute right-0 top-1/2 -translate-y-1/2 p-2 text-[#111] hover:text-kaira"
                  aria-label="Search"
                >
                  <Search className="w-6 h-6" />
                </button>
              </form>
              <h5 className="text-xs uppercase tracking-widest text-gray-500 mt-8 mb-2 font-jost">
                Browse Categories
              </h5>
              <div className="flex flex-wrap justify-center gap-x-2 gap-y-1">
                {navCategories.map((cat, i) => (
                  <Link
                    key={cat.id}
                    to={`/products?category=${cat.id}`}
                    onClick={() => setSearchOpen(false)}
                    className="text-lg text-[#111] hover:underline font-jost"
                  >
                    {cat.name}
                  </Link>
                ))}
              </div>
              <button
                type="button"
                onClick={() => setSearchOpen(false)}
                className="absolute top-6 right-6 text-[#111] hover:rotate-90 transition-transform"
                aria-label="Close search"
              >
                <X className="w-6 h-6" />
              </button>
            </div>
          </motion.div>
        )}
      </AnimatePresence>

      <motion.nav
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        className="relative w-full overflow-hidden bg-white border-b border-gray-200 shadow-sm transition-all duration-300"
      >
        <div className="max-w-[1800px] mx-auto px-4 md:px-6">
          <div className="flex items-center justify-between h-14 md:h-14 shrink-0">
            {/* Logo - Kaira style */}
            <Link
              to="/"
              className="flex items-center gap-2 text-[#111] font-marcellus text-xl md:text-2xl tracking-wide"
            >
              <span className="font-semibold">The Shop Hub</span>
            </Link>

            {/* Center nav - Desktop */}
            <div className="hidden md:flex items-center gap-6 lg:gap-8">
              <Link
                to="/"
                className="nav-link-kaira text-[#111] text-sm font-jost uppercase tracking-[0.1em]"
              >
                Home
              </Link>
              <Link
                to="/products"
                className="nav-link-kaira text-[#111] text-sm font-jost uppercase tracking-[0.1em]"
              >
                Shop
              </Link>
              <Link
                to="/orders"
                className="nav-link-kaira text-[#111] text-sm font-jost uppercase tracking-[0.1em]"
              >
                Orders
              </Link>
            </div>

            {/* Right: Wishlist, Cart, Search, User */}
            <div className="flex items-center gap-4 md:gap-6">
              {isAuthenticated ? (
                <>
                  <Link
                    to="/wishlist"
                    className="hidden md:flex items-center gap-1 text-[#111] text-sm font-jost uppercase tracking-[0.1em] nav-link-kaira"
                  >
                    Wishlist
                    <span className="text-gray-500 font-normal">({wishlistCount})</span>
                  </Link>
                  <Link
                    to="/cart"
                    className="hidden md:flex items-center gap-1 text-[#111] text-sm font-jost uppercase tracking-[0.1em] nav-link-kaira"
                  >
                    Cart
                    <span className="text-gray-500 font-normal">({cartItemCount})</span>
                  </Link>
                </>
              ) : (
                <>
                  <button
                    type="button"
                    onClick={() => showSignUpPrompt?.()}
                    className="hidden md:flex items-center gap-1 text-[#111] text-sm font-jost uppercase tracking-[0.1em] nav-link-kaira"
                  >
                    Wishlist
                    <span className="text-gray-500 font-normal">(0)</span>
                  </button>
                  <button
                    type="button"
                    onClick={() => showSignUpPrompt?.()}
                    className="hidden md:flex items-center gap-1 text-[#111] text-sm font-jost uppercase tracking-[0.1em] nav-link-kaira"
                  >
                    Cart
                    <span className="text-gray-500 font-normal">(0)</span>
                  </button>
                </>
              )}

              {/* Search icon - opens overlay */}
              <button
                type="button"
                onClick={() => setSearchOpen(true)}
                className="p-2 text-[#111] hover:text-kaira transition-colors"
                aria-label="Search"
              >
                <Search className="w-5 h-5 md:w-6 md:h-6" />
              </button>

              {/* Mobile: Wishlist & Cart icons with badges */}
              {isAuthenticated ? (
                <>
                  <Link to="/wishlist" className="md:hidden relative p-2 text-[#111]">
                    <Heart className="w-5 h-5" />
                    {wishlistCount > 0 && (
                      <span className="absolute -top-0.5 -right-0.5 w-4 h-4 bg-kaira text-white text-[10px] rounded-full flex items-center justify-center font-semibold">
                        {wishlistCount}
                      </span>
                    )}
                  </Link>
                  <Link to="/cart" className="md:hidden relative p-2 text-[#111]">
                    <ShoppingCart className="w-5 h-5" />
                    {cartItemCount > 0 && (
                      <span className="absolute -top-0.5 -right-0.5 w-4 h-4 bg-kaira text-white text-[10px] rounded-full flex items-center justify-center font-semibold">
                        {cartItemCount}
                      </span>
                    )}
                  </Link>
                </>
              ) : (
                <>
                  <button type="button" onClick={() => showSignUpPrompt?.()} className="md:hidden relative p-2 text-[#111]">
                    <Heart className="w-5 h-5" />
                  </button>
                  <button type="button" onClick={() => showSignUpPrompt?.()} className="md:hidden relative p-2 text-[#111]">
                    <ShoppingCart className="w-5 h-5" />
                  </button>
                </>
              )}

              {/* User */}
              {isAuthenticated ? (
                <div className="relative">
                  <div className="flex items-center gap-2">
                    <button
                      type="button"
                      onClick={() => setIsUserMenuOpen(!isUserMenuOpen)}
                      className="flex items-center gap-2 text-[#111] text-sm font-jost uppercase tracking-[0.05em] hover:text-kaira transition-colors"
                    >
                      <User className="w-5 h-5" />
                      <span className="hidden lg:inline">{user?.firstName}</span>
                    </button>
                    <button
                      type="button"
                      onClick={handleLogout}
                      className="p-1.5 text-[#111] hover:text-red-600 transition-colors"
                      aria-label="Logout"
                    >
                      <LogOut className="w-5 h-5" />
                    </button>
                  </div>
                  <AnimatePresence>
                    {isUserMenuOpen && (
                      <>
                        <div
                          className="fixed inset-0 z-10"
                          aria-hidden
                          onClick={() => setIsUserMenuOpen(false)}
                        />
                        <motion.div
                          initial={{ opacity: 0, y: 8 }}
                          animate={{ opacity: 1, y: 0 }}
                          exit={{ opacity: 0, y: 8 }}
                          className="absolute right-0 mt-2 w-52 bg-white border border-gray-200 shadow-lg z-20 py-1"
                        >
                          <Link
                            to="/profile"
                            className="flex items-center gap-2 px-4 py-2.5 text-sm uppercase tracking-wide text-[#111] hover:bg-[#111] hover:text-white transition-colors"
                            onClick={() => setIsUserMenuOpen(false)}
                          >
                            <Settings className="w-4 h-4" />
                            Profile
                          </Link>
                          <Link
                            to="/wishlist"
                            className="flex items-center gap-2 px-4 py-2.5 text-sm uppercase tracking-wide text-[#111] hover:bg-[#111] hover:text-white transition-colors"
                            onClick={() => setIsUserMenuOpen(false)}
                          >
                            <Heart className="w-4 h-4" />
                            Wishlist {wishlistCount > 0 && `(${wishlistCount})`}
                          </Link>
                          <Link
                            to="/orders"
                            className="flex items-center gap-2 px-4 py-2.5 text-sm uppercase tracking-wide text-[#111] hover:bg-[#111] hover:text-white transition-colors"
                            onClick={() => setIsUserMenuOpen(false)}
                          >
                            <Package className="w-4 h-4" />
                            Orders
                          </Link>
                          <hr className="my-1 border-gray-100" />
                          <button
                            type="button"
                            onClick={() => {
                              handleLogout()
                              setIsUserMenuOpen(false)
                            }}
                            className="flex items-center gap-2 px-4 py-2.5 text-sm uppercase tracking-wide text-red-600 hover:bg-red-50 w-full text-left transition-colors"
                          >
                            <LogOut className="w-4 h-4" />
                            Logout
                          </button>
                        </motion.div>
                      </>
                    )}
                  </AnimatePresence>
                </div>
              ) : (
                <Link
                  to="/login"
                  className="flex items-center gap-2 text-[#111] text-sm font-jost uppercase tracking-[0.1em] nav-link-kaira"
                >
                  <User className="w-5 h-5" />
                  <span className="hidden lg:inline">Login</span>
                </Link>
              )}

              {/* Mobile menu toggle */}
              <button
                type="button"
                onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                className="md:hidden p-2 text-[#111]"
                aria-label="Menu"
              >
                {isMobileMenuOpen ? <X className="w-6 h-6" /> : <Menu className="w-6 h-6" />}
              </button>
            </div>
          </div>

          {/* Categories marquee - Desktop only, fixed height so it never overlaps content */}
          <div className="hidden md:block h-10 border-t border-gray-200/80 shrink-0 overflow-hidden">
            <div className="category-marquee-wrapper h-full flex items-center">
              <div className="category-marquee">
                {marqueeCategories.map((category, index) => (
                  <Link
                    key={`${category.id}-${index}`}
                    to={`/products?category=${category.id}`}
                    className="nav-link-kaira text-[#111] text-xs font-jost uppercase tracking-[0.12em] text-gray-600 hover:text-[#111] whitespace-nowrap"
                  >
                    {category.name}
                  </Link>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* Mobile menu */}
        <AnimatePresence>
          {isMobileMenuOpen && (
            <motion.div
              initial={{ opacity: 0, height: 0 }}
              animate={{ opacity: 1, height: 'auto' }}
              exit={{ opacity: 0, height: 0 }}
              transition={{ duration: 0.25 }}
              className="md:hidden bg-white border-t border-gray-200 overflow-hidden"
            >
              <div className="max-w-[1800px] mx-auto px-4 py-4 space-y-1">
                <Link
                  to="/"
                  className="block py-3 text-sm uppercase tracking-[0.1em] text-[#111] font-jost border-b border-gray-100"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Home
                </Link>
                <Link
                  to="/products"
                  className="block py-3 text-sm uppercase tracking-[0.1em] text-[#111] font-jost border-b border-gray-100"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Shop
                </Link>
                <Link
                  to="/orders"
                  className="block py-3 text-sm uppercase tracking-[0.1em] text-[#111] font-jost border-b border-gray-100"
                  onClick={() => setIsMobileMenuOpen(false)}
                >
                  Orders
                </Link>
                <div className="pt-2">
                  <p className="text-xs uppercase tracking-widest text-gray-400 mb-2">Categories</p>
                  {navCategories.map(cat => (
                    <Link
                      key={cat.id}
                      to={`/products?category=${cat.id}`}
                      className="block py-2 text-sm text-[#111] font-jost nav-link-kaira"
                      onClick={() => setIsMobileMenuOpen(false)}
                    >
                      {cat.name}
                    </Link>
                  ))}
                </div>
                {isAuthenticated && (
                  <button
                    type="button"
                    onClick={() => { handleLogout(); setIsMobileMenuOpen(false); }}
                    className="flex items-center gap-2 w-full py-3 text-sm uppercase tracking-[0.1em] text-red-600 font-jost border-b border-gray-100"
                  >
                    <LogOut className="w-4 h-4" />
                    Logout
                  </button>
                )}
                <div className="pt-2 border-t border-gray-100">
                  <input
                    type="search"
                    placeholder="Search products..."
                    value={searchQuery}
                    onChange={e => setSearchQuery(e.target.value)}
                    onKeyDown={e => e.key === 'Enter' && executeSearch()}
                    className="w-full px-3 py-2.5 border border-gray-200 text-sm outline-none focus:border-kaira"
                  />
                  <button
                    type="button"
                    onClick={executeSearch}
                    className="mt-2 w-full py-2.5 bg-[#111] text-white text-sm uppercase tracking-wide font-jost"
                  >
                    Search
                  </button>
                </div>
              </div>
            </motion.div>
          )}
        </AnimatePresence>
      </motion.nav>
    </>
  )
}

export default Navbar
