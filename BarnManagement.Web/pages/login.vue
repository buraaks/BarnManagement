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
