import axios from 'axios'

// Backend API URL – use .env VITE_API_BASE_URL if set, else default (must match backend port e.g. 7077 or 7877)
const API_BASE_URL = import.meta.env.VITE_API_BASE_URL || 'https://localhost:7077/api'

const api = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
})

// Request interceptor to add token and fix FormData Content-Type
api.interceptors.request.use(
  (config) => {
    // When sending FormData, remove Content-Type so browser sets multipart/form-data with boundary
    if (config.data instanceof FormData) {
      delete config.headers['Content-Type']
    }
    const authStorage = localStorage.getItem('auth-storage')
    if (authStorage) {
      const { state } = JSON.parse(authStorage)
      if (state?.token) {
        config.headers.Authorization = `Bearer ${state.token}`
      }
    }
    return config
  },
  (error) => {
    console.error('Request error:', error)
    return Promise.reject(error)
  }
)

// Public endpoints that must not trigger redirect on 401 (guest can browse)
const isPublicGet = (config) => {
  const url = (config?.url || '').toLowerCase()
  return config?.method === 'get' && (
    url.includes('/product') || url.includes('/category')
  )
}

// Response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message)
    
    if (error.response?.status === 401) {
      localStorage.removeItem('auth-storage')
      // Don't redirect to login for public GET (Product/Category) so guest can see products
      if (!isPublicGet(error.config)) {
        window.location.href = '/login'
      }
    }
    
    // Network error
    if (!error.response) {
      console.error('Network Error: Cannot connect to backend API at', API_BASE_URL)
      console.error('Make sure your .NET backend is running on https://localhost:7077')
    }
    
    return Promise.reject(error)
  }
)

// Inline placeholder image (no network request) - use when image is missing or fails to load
const SVG_PLACEHOLDER = (w = 100, h = 100, text = 'No Image') =>
  `data:image/svg+xml,${encodeURIComponent(`<svg xmlns="http://www.w3.org/2000/svg" width="${w}" height="${h}"><rect fill="#f0f0f0" width="100%" height="100%"/><text fill="#999" x="50%" y="50%" dominant-baseline="middle" text-anchor="middle" font-size="14" font-family="sans-serif">${text}</text></svg>`)}`
export const getPlaceholderImageUrl = (width = 300, height = 300, text = 'No Image') =>
  SVG_PLACEHOLDER(width, height, text)

// Helper to build full image URL from backend-returned path (e.g. "Files/Products/xxx.jpg")
// For Cloudinary URLs, optional width (default 800) requests that size for sharp display
export const getImageUrl = (path, width = 800) => {
  if (!path) return null
  
  // Handle if path is an object with an imageUrl property (as returned by some API endpoints)
  const imagePath = typeof path === 'object' ? (path.imageUrl || path.ImageUrl || path.image || path.Image) : path
  
  if (!imagePath || typeof imagePath !== 'string') return null
  
  if (imagePath.startsWith('http://') || imagePath.startsWith('https://')) {
    return getSharpImageUrl(imagePath, width)
  }
  const base = API_BASE_URL.replace('/api', '')
  const normalizedPath = imagePath.replace(/\\/g, '/')
  return `${base}/${normalizedPath}`
}

/**
 * For Cloudinary URLs: inject quality/scale params so images render sharp (not blurry).
 * Uses q_auto:good, c_scale, and width so customer-side product images stay crisp.
 */
function getSharpImageUrl(url, width = 800) {
  if (!url || typeof url !== 'string') return url
  if (!url.includes('cloudinary.com') || !url.includes('/upload/')) return url
  // Insert after "upload/": c_scale,w_800,q_auto:good,f_auto/ (keep version if present)
  return url.replace(/\/upload\//, `/upload/c_scale,w_${width},q_auto:good,f_auto/`)
}

// Helper to extract error message from API response
export const getErrorMessage = (error) => {
  const data = error?.response?.data
  if (typeof data?.message === 'string') return data.message
  if (data?.errors) {
    if (Array.isArray(data.errors)) {
      return data.errors.map(e => e.error || e).join(', ')
    }
    // ASP.NET validation format: { "FieldName": ["error1", "error2"] }
    if (typeof data.errors === 'object') {
      const msgs = Object.values(data.errors).flat()
      return msgs.join(', ')
    }
  }
  return error?.message || 'Operation failed'
}

export default api
