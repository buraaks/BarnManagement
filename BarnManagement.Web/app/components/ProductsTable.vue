<template>
  <div class="grid-container">
    <div class="grid-header">
      <i class="fa-solid fa-warehouse" /> Storage / Products
    </div>
    <table>
      <thead>
        <tr>
          <th>Product Type</th>
          <th>Quantity</th>
          <th>Price</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="product in products" :key="product.type">
          <td>{{ product.type }}</td>
          <td>{{ product.quantity }}</td>
          <td>{{ (product.price * product.quantity).toFixed(2) }} TL</td>
        </tr>
        <tr v-if="products.length === 0">
          <td colspan="3" class="empty-state">
            No products in storage yet.
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type { GroupedProduct } from '~/types'

defineProps<{
  products: GroupedProduct[]
}>()
</script>

<style scoped>
.grid-container {
  background: var(--card-bg);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  border: 1px solid var(--border-color);
  overflow: hidden;
  transition: var(--transition);
}

.grid-container:hover {
  box-shadow: var(--shadow-lg);
}

.grid-header {
  padding: 1.25rem;
  font-family: "Outfit", sans-serif;
  font-weight: 600;
  font-size: 1.1rem;
  border-bottom: 1px solid var(--border-color);
  background: rgba(0, 0, 0, 0.02);
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

table {
  width: 100%;
  border-collapse: collapse;
}

th {
  text-align: left;
  padding: 1rem;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: var(--text-muted);
  background: rgba(0, 0, 0, 0.01);
}

td {
  padding: 1rem;
  border-top: 1px solid var(--border-color);
  font-size: 0.95rem;
  vertical-align: middle;
}

@media (max-width: 768px) {
  .grid-container {
    border-radius: var(--radius-sm);
  }

  th,
  td {
    padding: 0.75rem 0.625rem;
    font-size: 0.85rem;
    white-space: nowrap;
  }

  .grid-header {
    padding: 1rem;
    font-size: 1rem;
  }
}

@media (hover: none) and (pointer: coarse) {
  .grid-container:hover {
    box-shadow: var(--shadow);
  }
}
</style>
