import { create } from 'zustand'
import { persist, createJSONStorage } from 'zustand/middleware'

const useAuthStore = create(
  persist(
    (set, get) => ({
      user: null,
      token: null,
      isAuthenticated: false,
      
      login: (user, token) => {
        console.log('========== AUTH STORE LOGIN ==========')
        console.log('Saving user:', JSON.stringify(user, null, 2))
        console.log('User.userTypeId:', user.userTypeId)
        console.log('Type:', typeof user.userTypeId)
        
        set({ 
          user, 
          token, 
          isAuthenticated: true 
        })
        
        // Verify it was saved
        const state = get()
        console.log('State after set:', JSON.stringify(state, null, 2))
        console.log('======================================')
      },
      
      logout: () => {
        console.log('Logging out...')
        set({ 
          user: null, 
          token: null, 
          isAuthenticated: false 
        })
      },
      
      updateUser: (user) => set({ user }),
    }),
    {
      name: 'auth-storage',
      storage: createJSONStorage(() => localStorage),
      onRehydrateStorage: () => (state) => {
        console.log('========== REHYDRATING AUTH STORE ==========')
        console.log('Rehydrated state:', JSON.stringify(state, null, 2))
        if (state?.user) {
          console.log('User.userTypeId:', state.user.userTypeId)
          console.log('Type:', typeof state.user.userTypeId)
        }
        console.log('============================================')
      }
    }
  )
)

export default useAuthStore
