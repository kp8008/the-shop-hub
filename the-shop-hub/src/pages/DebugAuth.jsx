import { useEffect, useState } from 'react'
import useAuthStore from '../store/useAuthStore'

const DebugAuth = () => {
  const { user, token, isAuthenticated } = useAuthStore()
  const [localStorageData, setLocalStorageData] = useState(null)

  useEffect(() => {
    // Read localStorage directly
    const stored = localStorage.getItem('auth-storage')
    if (stored) {
      try {
        setLocalStorageData(JSON.parse(stored))
      } catch (e) {
        setLocalStorageData({ error: 'Failed to parse', raw: stored })
      }
    }
  }, [])

  const clearAuth = () => {
    localStorage.removeItem('auth-storage')
    window.location.reload()
  }

  return (
    <div style={{ padding: '40px', fontFamily: 'monospace', maxWidth: '800px', margin: '0 auto' }}>
      <h1 style={{ color: '#667eea' }}>🔍 Auth Debug Page</h1>
      
      <div style={{ marginTop: '30px' }}>
        <h2>Zustand Store State:</h2>
        <div style={{ background: '#f5f5f5', padding: '20px', borderRadius: '8px', marginTop: '10px' }}>
          <p><strong>isAuthenticated:</strong> {String(isAuthenticated)}</p>
          <p><strong>token:</strong> {token ? `${token.substring(0, 20)}...` : 'null'}</p>
          <p><strong>user:</strong></p>
          <pre style={{ background: '#fff', padding: '10px', borderRadius: '4px', overflow: 'auto' }}>
            {JSON.stringify(user, null, 2)}
          </pre>
          {user && (
            <>
              <p style={{ marginTop: '10px' }}><strong>user.userTypeId:</strong> {user.userTypeId}</p>
              <p><strong>Type of userTypeId:</strong> {typeof user.userTypeId}</p>
              <p><strong>Is Admin? (userTypeId === 1):</strong> {String(user.userTypeId === 1)}</p>
            </>
          )}
        </div>
      </div>

      <div style={{ marginTop: '30px' }}>
        <h2>LocalStorage Raw Data:</h2>
        <div style={{ background: '#f5f5f5', padding: '20px', borderRadius: '8px', marginTop: '10px' }}>
          <pre style={{ background: '#fff', padding: '10px', borderRadius: '4px', overflow: 'auto' }}>
            {JSON.stringify(localStorageData, null, 2)}
          </pre>
        </div>
      </div>

      <div style={{ marginTop: '30px' }}>
        <h2>Actions:</h2>
        <button 
          onClick={clearAuth}
          style={{
            padding: '12px 24px',
            background: '#e74c3c',
            color: 'white',
            border: 'none',
            borderRadius: '8px',
            cursor: 'pointer',
            fontSize: '16px'
          }}
        >
          Clear Auth & Reload
        </button>
      </div>

      <div style={{ marginTop: '30px', background: '#fff3cd', padding: '20px', borderRadius: '8px', border: '1px solid #ffc107' }}>
        <h3>Expected Values for Admin:</h3>
        <ul>
          <li><code>user.userTypeId</code> should be <strong>1</strong> (number)</li>
          <li><code>typeof user.userTypeId</code> should be <strong>"number"</strong></li>
          <li><code>user.userTypeId === 1</code> should be <strong>true</strong></li>
        </ul>
        
        <h3 style={{ marginTop: '20px' }}>Expected Values for Customer:</h3>
        <ul>
          <li><code>user.userTypeId</code> should be <strong>2</strong> (number)</li>
          <li><code>typeof user.userTypeId</code> should be <strong>"number"</strong></li>
          <li><code>user.userTypeId === 2</code> should be <strong>true</strong></li>
        </ul>
      </div>
    </div>
  )
}

export default DebugAuth
