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
