export type AnimalSpecies = 'Cow' | 'Sheep' | 'Chicken'

export interface Animal {
  id: string
  farmId: string
  species: AnimalSpecies
  name: string
  birthDate: string
  lifeSpanDays: number
  productionInterval: number
  nextProductionAt: string | null
  purchasePrice: number
  sellPrice: number
}

export interface Product {
  id: string
  farmId: string
  productType: string
  quantity: number
  salePrice: number
  producedAt: string
}

export interface User {
  id: string
  email: string
  username: string
  balance: number
}

export interface Farm {
  id: string
  name: string
  ownerId: string
}

export interface AuthResponse {
  token: string
}

export interface BuyAnimalRequest {
  species: AnimalSpecies
  name: string
  purchasePrice: number
  productionInterval: number
}

export interface LoginRequest {
  email: string
  password: string
}

export interface RegisterRequest {
  username: string
  email: string
  password: string
}

export interface GroupedProduct {
  type: string
  quantity: number
  price: number
}

export interface SellAllResponse {
  totalEarnings: number
}

export interface BalanceResponse {
  balance: number
}
