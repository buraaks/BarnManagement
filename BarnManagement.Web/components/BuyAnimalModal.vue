<template>
  <div v-if="visible" class="modal-overlay" @click.self="$emit('close')">
    <div class="modal-content">
      <div class="modal-header">
        <span>Buy Animal</span>
        <button class="close-btn" @click="$emit('close')">
          &times;
        </button>
      </div>
      <div class="modal-body">
        <div class="form-group">
          <label>Animal Name:</label>
          <input v-model="name" type="text" placeholder="Enter a name...">
        </div>
        <div class="form-group">
          <label>Animal Type:</label>
          <select v-model="species">
            <option value="Cow">
              Cow
            </option>
            <option value="Sheep">
              Sheep
            </option>
            <option value="Chicken">
              Chicken
            </option>
          </select>
        </div>
        <div class="price-info">
          <span>Price: </span>
          <strong>{{ price.toFixed(2) }} TL</strong>
        </div>
      </div>
      <div class="modal-footer">
        <button class="win-btn btn-success" @click="handleBuy">
          Buy
        </button>
        <button class="win-btn btn-danger" @click="$emit('close')">
          Cancel
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import type { AnimalSpecies } from '~/types'

defineProps<{
  visible: boolean
}>()

const emit = defineEmits<{
  close: []
  buy: [data: { species: AnimalSpecies, name: string, purchasePrice: number, productionInterval: number }]
}>()

const name = ref('')
const species = ref<AnimalSpecies>('Cow')

const priceMap: Record<AnimalSpecies, number> = {
  Cow: 500,
  Sheep: 200,
  Chicken: 30,
}

const intervalMap: Record<AnimalSpecies, number> = {
  Cow: 30,
  Sheep: 20,
  Chicken: 10,
}

const price = computed(() => priceMap[species.value] || 0)

function handleBuy() {
  emit('buy', {
    species: species.value,
    name: name.value || species.value,
    purchasePrice: priceMap[species.value],
    productionInterval: intervalMap[species.value],
  })
  name.value = ''
  species.value = 'Cow'
}
</script>
