<template>
  <header class="app-header">
    <div class="brand">
      <h1><i class="fa-solid fa-cow" /> Barn Management</h1>
      <p class="subtitle">
        Professional Simulation Dashboard
      </p>
    </div>
    <div class="header-actions">
      <div class="cash-card">
        <i class="fa-solid fa-wallet" /> {{ balance.toFixed(2) }} TL
      </div>
      <button class="theme-toggle" :title="isDark ? 'Light Mode' : 'Dark Mode'" @click="$emit('toggleTheme')">
        <i :class="isDark ? 'fa-solid fa-sun' : 'fa-solid fa-moon'" />
      </button>
      <button class="logout-btn" title="Logout" @click="$emit('logout')">
        <i class="fa-solid fa-right-from-bracket" />
      </button>
    </div>
  </header>
</template>

<script setup lang="ts">
defineProps<{
  balance: number
  isDark: boolean
}>()

defineEmits<{
  logout: []
  toggleTheme: []
}>()
</script>

<style scoped>
.app-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2.5rem;
  padding: 1.5rem;
  background: var(--card-bg);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  border: 1px solid var(--border-color);
}

h1 {
  font-family: "Outfit", sans-serif;
  font-size: 1.75rem;
  font-weight: 700;
  background: linear-gradient(to right, var(--primary), #818cf8);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.subtitle {
  color: var(--text-muted);
  font-size: 0.85rem;
}

.header-actions {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.cash-card {
  font-family: "Outfit", sans-serif;
  font-weight: 700;
  font-size: 1.25rem;
  color: var(--success);
  background: rgba(16, 185, 129, 0.1);
  padding: 0.5rem 1.25rem;
  border-radius: 999px;
  border: 1px solid rgba(16, 185, 129, 0.2);
}

.theme-toggle,
.logout-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-color);
  background: var(--card-bg);
  color: var(--text-muted);
  cursor: pointer;
  transition: var(--transition);
  font-size: 1rem;
}

.theme-toggle:hover,
.logout-btn:hover {
  color: var(--primary);
  border-color: var(--primary);
}

.logout-btn:hover {
  color: var(--danger);
  border-color: var(--danger);
}

@media (max-width: 768px) {
  .app-header {
    flex-direction: column;
    gap: 0.75rem;
    padding: 1rem;
    margin-bottom: 1.5rem;
    text-align: center;
  }

  .brand .subtitle {
    display: none;
  }

  h1 {
    font-size: 1.35rem;
  }

  .cash-card {
    font-size: 1.1rem;
    padding: 0.4rem 1rem;
  }
}

@media (max-width: 480px) {
  .app-header {
    padding: 0.875rem;
    margin-bottom: 1rem;
    border-radius: var(--radius-sm);
  }

  h1 {
    font-size: 1.15rem;
  }

  .cash-card {
    font-size: 1rem;
    padding: 0.35rem 0.875rem;
  }
}
</style>
