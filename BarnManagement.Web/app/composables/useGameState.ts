import type { Animal, BalanceResponse, BuyAnimalRequest, Farm, GroupedProduct, Product, SellAllResponse, User } from '~/types'

const user = ref<User | null>(null)
const animals = ref<Animal[]>([])
const products = ref<Product[]>([])
const currentFarmId = ref<string | null>(null)
const selectedAnimalIds = ref<string[]>([])
const loading = ref(false)

export function useGameState() {
  const { request } = useApi()
  const { showToast } = useToast()

  const groupedProducts = computed<GroupedProduct[]>(() => {
    const grouped: Record<string, GroupedProduct> = {}
    for (const p of products.value) {
      if (!grouped[p.productType]) {
        grouped[p.productType] = { type: p.productType, quantity: 0, price: p.salePrice }
      }
      grouped[p.productType].quantity += p.quantity
    }
    return Object.values(grouped)
  })

  function toggleAnimalSelection(id: string) {
    const index = selectedAnimalIds.value.indexOf(id)
    if (index > -1) {
      selectedAnimalIds.value.splice(index, 1)
    }
    else {
      selectedAnimalIds.value.push(id)
    }
  }

  async function init() {
    loading.value = true
    try {
      const userData = await request<User>('/users/me')
      if (!userData) return

      user.value = userData

      const farms = await request<Farm[]>('/farms')
      if (farms && farms.length > 0) {
        currentFarmId.value = farms[0].id
        await refreshData()
      }
    }
    finally {
      loading.value = false
    }
  }

  async function refreshData() {
    if (!currentFarmId.value) return

    const [animalsData, productsData, balanceData] = await Promise.all([
      request<Animal[]>(`/farms/${currentFarmId.value}/animals`),
      request<Product[]>(`/farms/${currentFarmId.value}/products`),
      request<BalanceResponse>('/users/me/balance'),
    ])

    if (animalsData && JSON.stringify(animalsData) !== JSON.stringify(animals.value)) {
      animals.value = animalsData
    }
    if (productsData && JSON.stringify(productsData) !== JSON.stringify(products.value)) {
      products.value = productsData
    }
    if (balanceData && user.value) user.value.balance = balanceData.balance
  }

  async function buyAnimal(data: BuyAnimalRequest): Promise<boolean> {
    if (!currentFarmId.value) return false

    const result = await request<Animal>(`/farms/${currentFarmId.value}/animals/buy`, {
      method: 'POST',
      body: JSON.stringify(data),
    })

    if (result) {
      showToast(`${data.name || data.species} bought successfully!`, 'success')
      await refreshData()
      return true
    }
    return false
  }

  async function sellAnimals(): Promise<boolean> {
    if (selectedAnimalIds.value.length === 0) {
      showToast('Please select at least one animal first', 'error')
      return false
    }

    const promises = selectedAnimalIds.value.map(id =>
      request(`/animals/${id}/sell`, { method: 'POST' }),
    )
    const results = await Promise.all(promises)
    const successCount = results.filter(Boolean).length

    if (successCount > 0) {
      showToast(`${successCount} animal(s) sold!`, 'success')
      selectedAnimalIds.value = []
      await refreshData()
      return true
    }
    return false
  }

  async function sellAllProducts(): Promise<boolean> {
    if (!currentFarmId.value) return false

    const totalVal = products.value.reduce((sum, p) => sum + p.salePrice * p.quantity, 0)
    if (totalVal <= 0) {
      showToast('No products to sell!', 'info')
      return false
    }

    const result = await request<SellAllResponse>(
      `/farms/${currentFarmId.value}/products/sell-all`,
      { method: 'POST' },
    )

    if (result) {
      showToast(`Sold everything for ${result.totalEarnings.toFixed(2)} TL`, 'success')
      await refreshData()
      return true
    }
    return false
  }

  async function sellProduct(productType: string, quantity: number): Promise<boolean> {
    const product = products.value.find(p => p.productType === productType)
    if (!product) return false

    const result = await request(`/products/${product.id}/sell?quantity=${quantity}`, {
      method: 'POST',
    })

    if (result) {
      showToast(`Sold ${quantity} ${productType} successfully!`, 'success')
      await refreshData()
      return true
    }
    return false
  }

  async function resetGame(): Promise<boolean> {
    const result = await request('/users/reset', { method: 'POST' })
    if (result) {
      showToast('Game has been reset.', 'success')
      user.value = null
      animals.value = []
      products.value = []
      currentFarmId.value = null
      selectedAnimalIds.value = []
      stopAutoRefresh()
      await init()
      startAutoRefresh()
      return true
    }
    return false
  }

  function startAutoRefresh() {
    const { connect } = useSse()

    // Connect to SSE events
    connect('/events', () => {}, {
      refresh: () => {
        console.log('SSE: Refresh event received')
        refreshData()
      }
    })

    if (import.meta.client) {
      document.addEventListener('visibilitychange', handleVisibility)
    }
  }

  function stopAutoRefresh() {
    const { disconnect } = useSse()
    disconnect()

    if (import.meta.client) {
      document.removeEventListener('visibilitychange', handleVisibility)
    }
  }

  function handleVisibility() {
    if (!document.hidden) {
      refreshData()
    }
  }

  return {
    user: readonly(user),
    animals: readonly(animals),
    products: readonly(products),
    groupedProducts,
    currentFarmId: readonly(currentFarmId),
    selectedAnimalIds,
    loading: readonly(loading),
    toggleAnimalSelection,
    init,
    refreshData,
    buyAnimal,
    sellAnimals,
    sellAllProducts,
    sellProduct,
    resetGame,
    startAutoRefresh,
    stopAutoRefresh,
  }
}
