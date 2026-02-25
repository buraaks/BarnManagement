<template>
  <div class="main-container">
    <AppHeader :balance="user?.balance ?? 0" :is-dark="isDark" @logout="handleLogout" @toggle-theme="toggleTheme" />

    <div v-if="gameLoading" class="game-layout">
      <div class="grids-section">
        <div class="skeleton-card">
          <div class="skeleton-card-header">
            Animals
          </div>
          <SkeletonLoader :rows="4" />
        </div>
        <div class="skeleton-card">
          <div class="skeleton-card-header">
            Products
          </div>
          <SkeletonLoader :rows="2" />
        </div>
      </div>
    </div>

    <div v-else class="game-layout">
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
        @sell-products="showSellProductModal = true"
        @reset="handleReset"
      />
    </div>

    <BuyAnimalModal
      :visible="showBuyModal"
      @close="showBuyModal = false"
      @buy="handleBuyAnimal"
    />

    <SellProductModal
      :visible="showSellProductModal"
      :products="groupedProducts"
      @close="showSellProductModal = false"
      @sell="handleSellProduct"
    />

    <ConfirmModal
      :visible="confirmState.visible"
      :title="confirmState.title"
      :message="confirmState.message"
      @confirm="confirmState.onConfirm"
      @cancel="cancelConfirm"
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
  loading: gameLoading,
  toggleAnimalSelection,
  init,
  buyAnimal,
  sellAnimals,
  sellAllProducts,
  sellProduct,
  resetGame,
  startAutoRefresh,
  stopAutoRefresh,
} = useGameState()

const { showToast } = useToast()
const { state: confirmState, confirm, cancel: cancelConfirm } = useConfirmModal()
const { logout } = useAuth()
const { isDark, toggleTheme } = useTheme()

const showBuyModal = ref(false)
const showSellProductModal = ref(false)

function handleLogout() {
  stopAutoRefresh()
  logout()
}

async function handleBuyAnimal(data: BuyAnimalRequest) {
  const success = await buyAnimal(data)
  if (success) showBuyModal.value = false
}

function handleSellAnimals() {
  if (selectedAnimalIds.value.length === 0) {
    showToast('Please select at least one animal first', 'error')
    return
  }

  const count = selectedAnimalIds.value.length
  const message = count > 1
    ? `Are you sure you want to sell ${count} selected animals?`
    : 'Are you sure you want to sell this animal?'

  confirm('Sell Animals', message, sellAnimals)
}

async function handleSellProduct(type: string, quantity: number) {
  const success = await sellProduct(type, quantity)
  if (success) showSellProductModal.value = false
}

function handleReset() {
  confirm('Reset Game', 'CRITICAL: This will delete ALL progress. Continue?', resetGame)
}

onMounted(async () => {
  await init()
  startAutoRefresh()
})

onUnmounted(() => {
  stopAutoRefresh()
})
</script>

<style scoped>
.main-container {
  max-width: 1100px;
  margin: 2rem auto;
  padding: 0 1.5rem;
  animation: fadeIn 0.5s ease-out;
}

.game-layout {
  display: grid;
  grid-template-columns: 1fr 240px;
  gap: 2rem;
}

.grids-section {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.skeleton-card {
  background: var(--card-bg);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  border: 1px solid var(--border-color);
  overflow: hidden;
}

.skeleton-card-header {
  padding: 1.25rem;
  font-family: "Outfit", sans-serif;
  font-weight: 600;
  font-size: 1.1rem;
  border-bottom: 1px solid var(--border-color);
  background: rgba(0, 0, 0, 0.02);
}

@media (max-width: 850px) {
  .game-layout {
    grid-template-columns: 1fr;
  }
}

@media (max-width: 768px) {
  .main-container {
    margin: 1rem auto;
    padding: 0 1rem;
  }

  .game-layout {
    gap: 1.25rem;
  }

  .grids-section {
    gap: 1.25rem;
  }
}

@media (max-width: 480px) {
  .main-container {
    margin: 0.5rem auto;
    padding: 0 0.75rem;
  }

  .game-layout {
    gap: 1rem;
  }

  .grids-section {
    gap: 1rem;
  }
}
</style>
