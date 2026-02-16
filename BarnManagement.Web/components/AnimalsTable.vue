<template>
  <div class="grid-container">
    <div class="grid-header">
      <i class="fa-solid fa-list-check" /> Animals
    </div>
    <table>
      <thead>
        <tr>
          <th>ID</th>
          <th>Name</th>
          <th>Age</th>
          <th>Type</th>
          <th>Production Progress</th>
        </tr>
      </thead>
      <tbody>
        <tr
          v-for="animal in animals"
          :key="animal.id"
          :class="{ 'selected-row': selectedIds.includes(animal.id) }"
          @click="$emit('select', animal.id)"
        >
          <td>{{ animal.id.substring(0, 8) }}</td>
          <td>{{ animal.name }}</td>
          <td>{{ getAge(animal.birthDate) }} Years</td>
          <td>{{ animal.species }}</td>
          <td>
            <ProgressBar :value="getProgress(animal)" />
          </td>
        </tr>
        <tr v-if="animals.length === 0">
          <td colspan="5" class="empty-state">
            No animals yet. Buy your first animal!
          </td>
        </tr>
      </tbody>
    </table>
  </div>
</template>

<script setup lang="ts">
import type { Animal } from '~/types'

defineProps<{
  animals: readonly Animal[]
  selectedIds: string[]
}>()

defineEmits<{
  select: [id: string]
}>()

function fixDate(dateStr: string): Date {
  return new Date(dateStr.endsWith('Z') ? dateStr : dateStr + 'Z')
}

function getAge(birthDate: string): number {
  const birth = fixDate(birthDate)
  const now = new Date()
  const diffSeconds = Math.max(0, (now.getTime() - birth.getTime()) / 1000)
  return Math.floor(diffSeconds / 30)
}

function getProgress(animal: Animal): number {
  if (!animal.nextProductionAt || animal.productionInterval <= 0) return 0

  const now = new Date()
  const nextProd = fixDate(animal.nextProductionAt)

  if (nextProd <= now) return 100

  const remainingSeconds = (nextProd.getTime() - now.getTime()) / 1000
  const ratio = 1.0 - remainingSeconds / animal.productionInterval
  return Math.max(0, Math.min(100, Math.floor(ratio * 100)))
}
</script>
