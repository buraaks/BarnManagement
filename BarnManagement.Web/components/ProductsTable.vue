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
