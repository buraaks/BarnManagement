const API_URL = '/api'

const token = ref<string | null>(null)
const apiLoading = ref(false)

function isTokenExpired(jwt: string): boolean {
  try {
    const payload = JSON.parse(atob(jwt.split('.')[1]))
    return payload.exp * 1000 < Date.now()
  }
  catch {
    return true
  }
}

function initToken() {
  if (import.meta.client) {
    const stored = localStorage.getItem('token')
    if (stored && !isTokenExpired(stored)) {
      token.value = stored
    }
    else if (stored) {
      localStorage.removeItem('token')
      token.value = null
    }
  }
}

export function useApi() {
  const { showToast } = useToast()

  if (import.meta.client && token.value === null) {
    initToken()
  }

  function getToken(): string | null {
    if (token.value && isTokenExpired(token.value)) {
      removeToken()
      return null
    }
    return token.value
  }

  function setToken(newToken: string) {
    token.value = newToken
    if (import.meta.client) {
      localStorage.setItem('token', newToken)
    }
  }

  function removeToken() {
    token.value = null
    if (import.meta.client) {
      localStorage.removeItem('token')
    }
  }

  function isAuthenticated(): boolean {
    return !!getToken()
  }

  async function request<T>(endpoint: string, options: RequestInit = {}): Promise<T | undefined> {
    const currentToken = getToken()
    const headers: Record<string, string> = {
      'Content-Type': 'application/json',
      ...(currentToken ? { Authorization: `Bearer ${currentToken}` } : {}),
      ...(options.headers as Record<string, string> || {}),
    }

    apiLoading.value = true
    try {
      const response = await fetch(`${API_URL}${endpoint}`, { ...options, headers })

      if (response.status === 401) {
        removeToken()
        navigateTo('/login')
        return undefined
      }

      if (!response.ok) {
        const errorText = await response.text()
        let errorMsg = errorText
        try {
          const errorJson = JSON.parse(errorText)
          errorMsg = errorJson.message || errorJson.title || errorMsg
        }
        catch {}
        throw new Error(errorMsg || `Request failed (${response.status})`)
      }

      if (response.status === 204) return true as T
      return await response.json() as T
    }
    catch (err: unknown) {
      const message = err instanceof Error ? err.message : 'An error occurred'
      showToast(message, 'error')
      return undefined
    }
    finally {
      apiLoading.value = false
    }
  }

  return {
    request,
    getToken,
    setToken,
    removeToken,
    isAuthenticated,
    token: readonly(token),
    apiLoading: readonly(apiLoading),
  }
}
