<template>
  <div class="progress-bar-container">
    <div class="progress-bar" :style="{ width: `${value}%` }" />
  </div>
</template>

<script setup lang="ts">
defineProps<{
  value: number
}>()
</script>

<style scoped>
.progress-bar-container {
  width: 120px;
  height: 8px;
  background: var(--border-color);
  border-radius: 999px;
  overflow: hidden;
}

.progress-bar {
  height: 100%;
  background: linear-gradient(90deg, var(--success), #34d399);
  transition: width 0.3s ease;
}

@media (max-width: 768px) {
  .progress-bar-container {
    width: 80px;
  }
}

@media (max-width: 480px) {
  .progress-bar-container {
    width: 100%;
    max-width: 120px;
  }
}
</style>
