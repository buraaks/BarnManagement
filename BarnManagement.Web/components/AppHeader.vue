<template>
  <header>
    <div class="brand">
      <h1><i class="fa-solid fa-cow" /> Barn Management</h1>
      <p style="color: var(--text-muted); font-size: 0.85rem">
        Professional Simulation Dashboard
      </p>
    </div>
    <div class="cash-card">
      <i class="fa-solid fa-wallet" /> {{ balance.toFixed(2) }} TL
    </div>
  </header>
</template>

<script setup lang="ts">
defineProps<{
  balance: number
}>()
</script>
