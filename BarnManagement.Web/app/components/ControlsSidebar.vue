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

<style scoped>
.btn {
  display: block;
  width: 100%;
  padding: 0.875rem;
  border-radius: var(--radius-sm);
  font-family: "Inter", sans-serif;
  font-weight: 600;
  font-size: 0.95rem;
  cursor: pointer;
  transition: var(--transition);
  border: 1px solid var(--border-color);
  background: var(--card-bg);
  color: var(--text-main);
  margin-bottom: 0.75rem;
  text-align: center;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
  border-color: var(--primary);
  color: var(--primary);
}

.btn:active {
  transform: translateY(0);
}

.btn-danger {
  background: var(--danger);
  color: white;
  border: none;
}

.btn-danger:hover {
  background: #dc2626;
  color: white;
  filter: brightness(1.1);
}

@media (max-width: 850px) {
  .controls-section {
    order: -1;
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
    gap: 1rem;
  }
}

@media (max-width: 768px) {
  .controls-section {
    grid-template-columns: 1fr 1fr;
    gap: 0.75rem;
  }

  .btn {
    padding: 1rem;
    font-size: 1rem;
    margin-bottom: 0.5rem;
    -webkit-tap-highlight-color: transparent;
  }

  .btn:hover {
    transform: none;
  }
}

@media (max-width: 480px) {
  .controls-section {
    grid-template-columns: 1fr 1fr;
    gap: 0.5rem;
  }

  .btn {
    padding: 0.75rem;
    font-size: 0.85rem;
    min-height: 44px;
    display: flex;
    align-items: center;
    justify-content: center;
    gap: 0.4rem;
  }
}

@media (hover: none) and (pointer: coarse) {
  .btn:hover {
    transform: none;
    box-shadow: none;
  }

  .btn:active {
    transform: scale(0.97);
    opacity: 0.85;
  }
}
</style>
