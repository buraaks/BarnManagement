import type { Animal, BalanceResponse, BuyAnimalRequest, Farm, GroupedProduct, Product, SellAllResponse, User } from '~/types'

export function useGameState() {
  const { request } = useApi()
  const { showToast } = useToast()

  const user = ref<User | null>(null)
  const animals = ref<Animal[]>([])
  const products = ref<Product[]>([])
  const currentFarmId = ref<string | null>(null)
  const selectedAnimalIds = ref<string[]>([])

  let refreshInterval: ReturnType<typeof setInterval> | null = null

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
    const userData = await request<User>('/users/me')
    if (!userData) return

    user.value = userData

    const farms = await request<Farm[]>('/farms')
    if (farms && farms.length > 0) {
      currentFarmId.value = farms[0].id
      await refreshData()
    }
  }

  async function refreshData() {
    if (!currentFarmId.value) return

    const [animalsData, productsData, balanceData] = await Promise.all([
      request<Animal[]>(`/farms/${currentFarmId.value}/animals`),
      request<Product[]>(`/farms/${currentFarmId.value}/products`),
      request<BalanceResponse>('/users/me/balance'),
    ])

    if (animalsData) animals.value = animalsData
    if (productsData) products.value = productsData
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

    let successCount = 0
    for (const animalId of [...selectedAnimalIds.value]) {
      const result = await request(`/animals/${animalId}/sell`, { method: 'POST' })
      if (result) successCount++
    }

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

  async function resetGame(): Promise<boolean> {
    const result = await request('/users/reset', { method: 'POST' })
    if (result) {
      showToast('Game has been reset.', 'success')
      setTimeout(() => window.location.reload(), 1000)
      return true
    }
    return false
  }

  function startAutoRefresh() {
    stopAutoRefresh()
    refreshInterval = setInterval(refreshData, 1000)
  }

  function stopAutoRefresh() {
    if (refreshInterval) {
      clearInterval(refreshInterval)
      refreshInterval = null
    }
  }

  return {
    user: readonly(user),
    animals: readonly(animals),
    products: readonly(products),
    groupedProducts,
    currentFarmId: readonly(currentFarmId),
    selectedAnimalIds,
    toggleAnimalSelection,
    init,
    refreshData,
    buyAnimal,
    sellAnimals,
    sellAllProducts,
    resetGame,
    startAutoRefresh,
    stopAutoRefresh,
  }
}
