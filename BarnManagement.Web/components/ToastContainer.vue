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
