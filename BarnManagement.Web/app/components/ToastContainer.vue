<template>
  <div class="toast-container">
    <div
      v-for="toast in toasts"
      :key="toast.id"
      class="toast"
      :class="[toast.type, { 'fade-out': toast.fadingOut }]"
    >
      {{ toast.message }}
    </div>
  </div>
</template>

<script setup lang="ts">
const { toasts } = useToast()
</script>

<style scoped>
.toast-container {
  position: fixed;
  bottom: 24px;
  right: 24px;
  z-index: 2000;
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.toast {
  background: var(--card-bg);
  color: var(--text-main);
  padding: 1rem 1.5rem;
  border-radius: var(--radius-sm);
  box-shadow: var(--shadow-lg);
  border-left: 4px solid var(--primary);
  font-weight: 500;
  min-width: 280px;
  animation: slideIn 0.3s ease-out;
}

.toast.error {
  border-left-color: var(--danger);
}

.toast.success {
  border-left-color: var(--success);
}

.toast.fade-out {
  opacity: 0;
  transition: opacity 0.3s ease;
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
    opacity: 0;
  }
  to {
    transform: translateX(0);
    opacity: 1;
  }
}

@media (max-width: 768px) {
  .toast-container {
    left: 1rem;
    right: 1rem;
    bottom: 1rem;
  }

  .toast {
    min-width: unset;
    width: 100%;
    padding: 0.875rem 1rem;
    font-size: 0.9rem;
  }
}
</style>
