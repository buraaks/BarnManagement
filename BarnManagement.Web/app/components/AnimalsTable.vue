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

tbody tr {
  cursor: pointer;
  transition: var(--transition);
}

tbody tr:hover {
  background-color: rgba(79, 70, 229, 0.04);
}

.selected-row {
  background-color: rgba(79, 70, 229, 0.08) !important;
  position: relative;
}

.selected-row::before {
  content: "";
  position: absolute;
  left: 0;
  top: 0;
  bottom: 0;
  width: 4px;
  background: var(--primary);
}

@media (max-width: 768px) {
  .grid-container {
    border-radius: var(--radius-sm);
  }

  .grid-container :deep(table) {
    display: block;
    overflow-x: auto;
    -webkit-overflow-scrolling: touch;
  }

  .grid-container :deep(thead),
  .grid-container :deep(tbody) {
    display: table;
    width: 100%;
    table-layout: fixed;
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

@media (max-width: 480px) {
  table thead {
    display: none;
  }

  table,
  table tbody {
    display: block;
  }

  table tbody tr {
    display: flex;
    flex-direction: column;
    padding: 0.875rem;
    border-top: 1px solid var(--border-color);
    gap: 0.35rem;
    position: relative;
  }

  table tbody tr:first-child {
    border-top: none;
  }

  table tbody tr td {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 0.25rem 0;
    border-top: none;
    font-size: 0.875rem;
    white-space: normal;
  }

  table tbody tr td::before {
    content: attr(data-label);
    font-weight: 600;
    font-size: 0.75rem;
    text-transform: uppercase;
    letter-spacing: 0.04em;
    color: var(--text-muted);
    flex-shrink: 0;
    margin-right: 0.75rem;
  }

  .selected-row {
    border-left: 4px solid var(--primary) !important;
    padding-left: calc(0.875rem - 4px) !important;
    background-color: rgba(79, 70, 229, 0.06) !important;
    border-radius: var(--radius-sm);
  }

  .selected-row::before {
    display: none;
  }
}

@media (hover: none) and (pointer: coarse) {
  tbody tr:hover {
    background-color: transparent;
  }

  tbody tr:active {
    background-color: rgba(79, 70, 229, 0.06);
  }

  .grid-container:hover {
    box-shadow: var(--shadow);
  }
}
</style>
