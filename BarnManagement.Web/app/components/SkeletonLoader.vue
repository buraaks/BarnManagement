<template>
  <div class="skeleton-container">
    <div v-for="i in rows" :key="i" class="skeleton-row">
      <div class="skeleton-line" :style="{ width: getWidth(i) }" />
    </div>
  </div>
</template>

<script setup lang="ts">
const props = withDefaults(defineProps<{
  rows?: number
}>(), {
  rows: 3,
})

function getWidth(index: number): string {
  const widths = ['100%', '85%', '70%', '90%', '75%']
  return widths[(index - 1) % widths.length]
}
</script>

<style scoped>
.skeleton-container {
  padding: 1.25rem;
}

.skeleton-row {
  margin-bottom: 1rem;
}

.skeleton-row:last-child {
  margin-bottom: 0;
}

.skeleton-line {
  height: 16px;
  background: linear-gradient(90deg, var(--border-color) 25%, transparent 50%, var(--border-color) 75%);
  background-size: 200% 100%;
  border-radius: 4px;
  animation: shimmer 1.5s infinite;
}

@keyframes shimmer {
  0% {
    background-position: 200% 0;
  }
  100% {
    background-position: -200% 0;
  }
}
</style>
