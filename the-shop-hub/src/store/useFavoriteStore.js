import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'
import api from '../utils/api'

const useFavoriteStore = create(
  persist(
    (set, get) => ({
      favorites: [],
      loading: false,
      
      // Fetch all favorites from server
      fetchFavorites: async () => {
        try {
          set({ loading: true })
          const response = await api.get('/Favorites/MyFavorites')
          set({ favorites: response.data, loading: false })
          return response.data
        } catch (error) {
          console.error('Error fetching favorites:', error)
          set({ loading: false })
          return []
        }
      },
      
      // Check if a product is in favorites
      isFavorite: (productId) => {
        return get().favorites.some(f => f.productID === productId)
      },
      
      // Toggle favorite status
      toggleFavorite: async (productId) => {
        try {
          const response = await api.post(`/Favorites/toggle/${productId}`)
          // Refresh favorites list
          await get().fetchFavorites()
          return response.data
        } catch (error) {
          console.error('Error toggling favorite:', error)
          throw error
        }
      },
      
      // Add to favorites
      addFavorite: async (productId) => {
        try {
          await api.post('/Favorites', { productID: productId })
          await get().fetchFavorites()
        } catch (error) {
          console.error('Error adding favorite:', error)
          throw error
        }
      },
      
      // Remove from favorites
      removeFavorite: async (productId) => {
        try {
          await api.delete(`/Favorites/${productId}?productId=${productId}`)
          await get().fetchFavorites()
        } catch (error) {
          console.error('Error removing favorite:', error)
          throw error
        }
      },
      
      // Get favorites count
      getCount: () => get().favorites.length,
      
      // Clear favorites (local only)
      clearFavorites: () => set({ favorites: [] }),
    }),
    {
      name: 'favorites-storage',
      storage: createJSONStorage(() => localStorage),
    }
  )
)

export default useFavoriteStore
