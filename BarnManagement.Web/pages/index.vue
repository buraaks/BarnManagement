<template>
  <div class="main-container">
    <AppHeader :balance="user?.balance ?? 0" />

    <div class="game-layout">
      <div class="grids-section">
        <AnimalsTable
          :animals="animals"
          :selected-ids="selectedAnimalIds"
          @select="toggleAnimalSelection"
        />
        <ProductsTable :products="groupedProducts" />
      </div>

      <ControlsSidebar
        :selected-count="selectedAnimalIds.length"
        @buy="showBuyModal = true"
        @sell="handleSellAnimals"
        @sell-products="handleSellProducts"
        @reset="handleReset"
      />
    </div>

    <BuyAnimalModal
      :visible="showBuyModal"
      @close="showBuyModal = false"
      @buy="handleBuyAnimal"
    />

    <ConfirmModal
      :visible="confirmModal.visible"
      :title="confirmModal.title"
      :message="confirmModal.message"
      @confirm="confirmModal.onConfirm"
      @cancel="confirmModal.visible = false"
    />
  </div>
</template>

<script setup lang="ts">
import type { BuyAnimalRequest } from '~/types'

definePageMeta({
  middleware: 'auth',
})

const {
  user,
  animals,
  groupedProducts,
  selectedAnimalIds,
  toggleAnimalSelection,
  init,
  buyAnimal,
  sellAnimals,
  sellAllProducts,
  resetGame,
  startAutoRefresh,
  stopAutoRefresh,
} = useGameState()

const showBuyModal = ref(false)

const confirmModal = reactive({
  visible: false,
  title: '',
  message: '',
  onConfirm: () => {},
})

function showConfirm(title: string, message: string, onConfirm: () => void) {
  confirmModal.title = title
  confirmModal.message = message
  confirmModal.onConfirm = () => {
    confirmModal.visible = false
    onConfirm()
  }
  confirmModal.visible = true
}

async function handleBuyAnimal(data: BuyAnimalRequest) {
  const success = await buyAnimal(data)
  if (success) showBuyModal.value = false
}

function handleSellAnimals() {
  if (selectedAnimalIds.value.length === 0) {
    const { showToast } = useToast()
    showToast('Please select at least one animal first', 'error')
    return
  }

  const count = selectedAnimalIds.value.length
  const message = count > 1
    ? `Are you sure you want to sell ${count} selected animals?`
    : 'Are you sure you want to sell this animal?'

  showConfirm('Sell Animals', message, sellAnimals)
}

function handleSellProducts() {
  showConfirm(
    'Sell All Products',
    'Sell all products in storage?',
    sellAllProducts,
  )
}

function handleReset() {
  showConfirm(
    'Reset Game',
    'CRITICAL: This will delete ALL progress. Continue?',
    resetGame,
  )
}

onMounted(async () => {
  await init()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})
</script>
