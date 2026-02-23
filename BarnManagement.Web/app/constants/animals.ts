import type { AnimalSpecies } from '~/types'

export interface AnimalConfig {
  species: AnimalSpecies
  price: number
  productionInterval: number
  icon: string
}

export const ANIMAL_CONFIGS: Record<AnimalSpecies, AnimalConfig> = {
  Cow: { species: 'Cow', price: 500, productionInterval: 30, icon: 'fa-cow' },
  Sheep: { species: 'Sheep', price: 200, productionInterval: 20, icon: 'fa-sheep' },
  Chicken: { species: 'Chicken', price: 30, productionInterval: 10, icon: 'fa-egg' },
}

export const ANIMAL_SPECIES_LIST: AnimalSpecies[] = ['Cow', 'Sheep', 'Chicken']
