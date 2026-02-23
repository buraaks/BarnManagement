<template>
  <div class="login-container">
    <h2>{{ isLogin ? 'Login' : 'Register' }}</h2>
    <form @submit.prevent="handleSubmit">
      <div v-if="!isLogin" class="form-group">
        <label>Username</label>
        <input v-model="username" type="text">
      </div>
      <div class="form-group">
        <label>Email</label>
        <input v-model="email" type="email" required>
      </div>
      <div class="form-group">
        <label>Password</label>
        <input v-model="password" type="password" required>
      </div>
      <button type="submit" class="login-btn" :disabled="loading">
        {{ loading ? 'Please wait...' : (isLogin ? 'Login' : 'Register') }}
      </button>
    </form>
    <div class="auth-switch">
      <span>{{ isLogin ? "Don't have an account? " : 'Already have an account? ' }}</span>
      <a @click="toggleMode">{{ isLogin ? 'Register' : 'Login' }}</a>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({
  layout: 'auth',
  middleware: 'auth',
})

const { login, register } = useAuth()

const isLogin = ref(true)
const email = ref('')
const password = ref('')
const username = ref('')
const loading = ref(false)

function toggleMode() {
  isLogin.value = !isLogin.value
}

async function handleSubmit() {
  loading.value = true
  try {
    let success: boolean
    if (isLogin.value) {
      success = await login({ email: email.value, password: password.value })
    }
    else {
      success = await register({
        username: username.value,
        email: email.value,
        password: password.value,
      })
    }

    if (success) {
      navigateTo('/')
    }
  }
  finally {
    loading.value = false
  }
}
</script>

<style scoped>
.login-container {
  max-width: 420px;
  margin: 100px auto;
  padding: 2.5rem;
  background: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: var(--radius);
  box-shadow: var(--shadow-lg);
}

.login-container h2 {
  font-family: "Outfit", sans-serif;
  font-weight: 700;
  text-align: center;
  margin-bottom: 1.5rem;
  background: linear-gradient(to right, var(--primary), #818cf8);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.auth-switch {
  margin-top: 1.25rem;
  text-align: center;
  font-size: 0.9rem;
  color: var(--text-muted);
}

.auth-switch a {
  color: var(--primary);
  cursor: pointer;
  text-decoration: underline;
  font-weight: 500;
}

.auth-switch a:hover {
  color: var(--primary-hover);
}

.login-btn {
  display: block;
  width: 100%;
  padding: 0.875rem;
  border-radius: var(--radius-sm);
  font-family: "Inter", sans-serif;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  transition: var(--transition);
  border: none;
  background: var(--primary);
  color: white;
  margin-top: 0.5rem;
}

.login-btn:hover {
  background: var(--primary-hover);
  transform: translateY(-1px);
  box-shadow: 0 4px 12px rgba(79, 70, 229, 0.3);
}

.login-btn:disabled {
  opacity: 0.7;
  cursor: not-allowed;
  transform: none;
}

@media (max-width: 768px) {
  .login-container {
    margin: 2rem auto;
    padding: 1.5rem;
    max-width: calc(100vw - 2rem);
    border-radius: var(--radius-sm);
  }

  .login-btn {
    padding: 1rem;
    font-size: 1rem;
  }
}

@media (max-width: 480px) {
  .login-container {
    margin: 1rem auto;
    padding: 1.25rem;
  }

  .login-container h2 {
    font-size: 1.5rem;
    margin-bottom: 1.25rem;
  }
}

@media (hover: none) and (pointer: coarse) {
  .login-btn:hover {
    transform: none;
  }

  .login-btn:active {
    transform: scale(0.98);
    opacity: 0.9;
  }
}
</style>
