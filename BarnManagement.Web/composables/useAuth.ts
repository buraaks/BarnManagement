import type { AuthResponse, LoginRequest, RegisterRequest } from '~/types'

export function useAuth() {
  const { request, setToken, removeToken } = useApi()
  const { showToast } = useToast()

  async function login(data: LoginRequest): Promise<boolean> {
    const result = await request<AuthResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify(data),
    })

    if (result?.token) {
      setToken(result.token)
      return true
    }
    return false
  }

  async function register(data: RegisterRequest): Promise<boolean> {
    const result = await request<AuthResponse>('/auth/register', {
      method: 'POST',
      body: JSON.stringify(data),
    })

    if (result?.token) {
      setToken(result.token)
      showToast('Account created successfully!', 'success')
      return true
    }
    return false
  }

  function logout() {
    removeToken()
    navigateTo('/login')
  }

  return {
    login,
    register,
    logout,
  }
}
