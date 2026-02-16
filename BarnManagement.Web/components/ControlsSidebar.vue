<template>
  <div class="controls-section">
    <button class="btn" @click="$emit('buy')">
      <i class="fa-solid fa-cart-plus" /> Buy Animal
    </button>
    <button class="btn" @click="$emit('sell')">
      <i class="fa-solid fa-money-bill-transfer" />
      {{ selectedCount > 1 ? `Sell Selected (${selectedCount})` : 'Sell Animal' }}
    </button>
    <button class="btn" @click="$emit('sellProducts')">
      <i class="fa-solid fa-box-open" /> Sell Products
    </button>
    <button class="btn btn-danger" @click="$emit('reset')">
      <i class="fa-solid fa-rotate-left" /> Reset Game
    </button>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  selectedCount: number
}>()

defineEmits<{
  buy: []
  sell: []
  sellProducts: []
  reset: []
}>()
</script>
