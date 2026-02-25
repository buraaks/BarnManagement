<template>
  <div v-if="visible" class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <div class="modal-header">
        <span>Sell Products</span>
        <button class="close-btn" @click="$emit('close')">
          &times;
        </button>
      </div>

      <div class="modal-body">
        <div class="form-group">
          <label>Product Type:</label>
          <select v-model="selectedType" class="form-input">
            <option value="" disabled>Select product</option>
            <option v-for="p in products" :key="p.type" :value="p.type">
              {{ p.type }} ({{ p.quantity }} in stock)
            </option>
          </select>
        </div>

        <div class="form-group">
          <label>Quantity:</label>
          <input
            v-model.number="quantity"
            type="number"
            min="1"
            :max="maxQuantity"
            class="form-input"
            :disabled="!selectedType"
            placeholder="Enter amount"
          />
        </div>
      </div>

      <div class="modal-footer">
        <button
          class="win-btn btn-success"
          :disabled="!isValid"
          @click="handleSell"
        >
          Sell
        </button>
        <button class="win-btn btn-danger" @click="$emit('close')">
          Cancel
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { GroupedProduct } from '~/types'

const props = defineProps<{
  visible: boolean
  products: GroupedProduct[]
}>()

const emit = defineEmits<{
  close: []
  sell: [type: string, quantity: number]
}>()

const selectedType = ref('')
const quantity = ref(1)

const maxQuantity = computed(() => {
  const prod = props.products.find(p => p.type === selectedType.value)
  return prod?.quantity || 0
})

const isValid = computed(() => {
  return selectedType.value !== '' && quantity.value > 0 && quantity.value <= maxQuantity.value
})

function handleSell() {
  if (isValid.value) {
    emit('sell', selectedType.value, quantity.value)
  }
}

// Reset on close/open
watch(() => props.visible, (val) => {
  if (val) {
    selectedType.value = ''
    quantity.value = 1
  }
})
</script>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(4px);
  z-index: 1000;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 1rem;
}

.modal-content {
  background: var(--card-bg);
  border-radius: var(--radius);
  width: 100%;
  max-width: 400px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  border: 1px solid var(--border-color);
  overflow: hidden;
  animation: modalPop 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes modalPop {
  from {
    transform: scale(0.9) translateY(20px);
    opacity: 0;
  }
  to {
    transform: scale(1) translateY(0);
    opacity: 1;
  }
}

.modal-header {
  padding: 1.25rem;
  background: var(--primary);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-family: "Outfit", sans-serif;
  font-weight: 600;
}

.modal-body {
  padding: 1.5rem;
}

.form-group {
  margin-bottom: 1.25rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 600;
  font-size: 0.9rem;
  color: var(--text-main);
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border-radius: var(--radius-sm);
  border: 1px solid var(--border-color);
  background: #ffffff;
  color: #1a1a1a;
  font-family: inherit;
}

.form-input:disabled {
  opacity: 0.6;
  cursor: not-allowed;
  background: #f3f4f6;
}

.modal-footer {
  padding: 1.25rem;
  background: rgba(0, 0, 0, 0.02);
  border-top: 1px solid var(--border-color);
  display: flex;
  gap: 1rem;
}

.win-btn {
  flex: 1;
  padding: 0.75rem;
  border-radius: var(--radius-sm);
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: var(--transition);
  font-family: inherit;
  font-size: 0.95rem;
}

.btn-success {
  background: var(--success);
  color: white;
}

.btn-success:hover:not(:disabled) {
  filter: brightness(1.1);
}

.btn-success:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-danger {
  background: var(--danger);
  color: white;
}

.btn-danger:hover {
  filter: brightness(1.1);
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  line-height: 1;
  opacity: 0.8;
  transition: opacity 0.2s;
}

.close-btn:hover {
  opacity: 1;
}

@media (max-width: 768px) {
  .modal-content {
    max-width: 90vw;
  }
}
</style>
