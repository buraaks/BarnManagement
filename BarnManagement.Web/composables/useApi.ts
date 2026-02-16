const API_URL = '/api'

export function useApi() {
  const { showToast } = useToast()

  function getToken(): string | null {
    if (import.meta.client) {
      return localStorage.getItem('token')
    }
    return null
  }

  function setToken(token: string) {
    if (import.meta.client) {
      localStorage.setItem('token', token)
    }
  }

  function removeToken() {
    if (import.meta.client) {
      localStorage.removeItem('token')
    }
  }

  function isAuthenticated(): boolean {
    return !!getToken()
  }

  async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T | undefined> {
    const token = getToken()
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...(options.headers as Record<string, string> || {}),
    }

    try {
      const response = await fetch(`${API_URL}${endpoint}`, { ...options, headers })

      if (response.status === 401 || response.status === 404) {
        removeToken()
        navigateTo('/login')
        return undefined
      }

      if (!response.ok) {
        const errorText = await response.text()
        let errorMsg = errorText
        try {
          const errorJson = JSON.parse(errorText)
          errorMsg = errorJson.message || errorJson
        }
        catch {}
        throw new Error(errorMsg || 'Request failed')
      }

      if (response.status === 204) return true as T
      return await response.json() as T
    }
    catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'An error occurred'
      console.error('API Error:', err)
      showToast(message, 'error')
      return undefined
    }
  }

  return {
    request,
    getToken,
    setToken,
    removeToken,
    isAuthenticated,
  }
}
