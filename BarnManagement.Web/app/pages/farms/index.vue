<template>
  <div class="farms-container">
    <div class="farms-header">
      <div>
        <h2><i class="fa-solid fa-tractor" /> My Farms</h2>
        <p class="subtitle">
          Manage your farms
        </p>
      </div>
      <div class="farms-header-actions">
        <button class="btn-back" @click="navigateTo('/')">
          <i class="fa-solid fa-arrow-left" /> Back
        </button>
        <button class="btn-create" @click="showCreateModal = true">
          <i class="fa-solid fa-plus" /> New Farm
        </button>
      </div>
    </div>

    <div v-if="loading" class="loading-state">
      Loading farms...
    </div>

    <div v-else-if="farms.length === 0" class="empty-farms">
      <i class="fa-solid fa-seedling" />
      <p>No farms yet. Create your first farm!</p>
    </div>

    <div v-else class="farms-grid">
      <div v-for="farm in farms" :key="farm.id" class="farm-card">
        <div class="farm-card-header">
          <h3>{{ farm.name }}</h3>
          <span class="farm-id">{{ farm.id.substring(0, 8) }}</span>
        </div>
        <div class="farm-card-actions">
          <button class="btn-edit" @click="startEdit(farm)">
            <i class="fa-solid fa-pen" /> Rename
          </button>
          <button class="btn-delete" @click="confirmDelete(farm)">
            <i class="fa-solid fa-trash" /> Delete
          </button>
        </div>
      </div>
    </div>

    <div v-if="showCreateModal" class="modal-overlay" @click.self="showCreateModal = false">
      <div class="modal-content">
        <div class="modal-header">
          <span>{{ editingFarm ? 'Rename Farm' : 'Create Farm' }}</span>
          <button class="close-btn" @click="closeModal">
            &times;
          </button>
        </div>
        <div class="modal-body">
          <div class="form-group">
            <label>Farm Name:</label>
            <input v-model="farmName" type="text" placeholder="Enter farm name..." @keyup.enter="handleSave">
          </div>
        </div>
        <div class="modal-footer">
          <button class="win-btn btn-success" @click="handleSave">
            {{ editingFarm ? 'Save' : 'Create' }}
          </button>
          <button class="win-btn btn-cancel" @click="closeModal">
            Cancel
          </button>
        </div>
      </div>
    </div>

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
import type { Farm } from '~/types'

definePageMeta({
  middleware: 'auth',
})

const { request } = useApi()
const { showToast } = useToast()
const { state: confirmState, confirm, cancel: cancelConfirm } = useConfirmModal()

const farms = ref<Farm[]>([])
const loading = ref(true)
const showCreateModal = ref(false)
const farmName = ref('')
const editingFarm = ref<Farm | null>(null)

async function fetchFarms() {
  loading.value = true
  const data = await request<Farm[]>('/farms')
  if (data) farms.value = data
  loading.value = false
}

function startEdit(farm: Farm) {
  editingFarm.value = farm
  farmName.value = farm.name
  showCreateModal.value = true
}

function closeModal() {
  showCreateModal.value = false
  editingFarm.value = null
  farmName.value = ''
}

async function handleSave() {
  if (!farmName.value.trim()) {
    showToast('Farm name is required', 'error')
    return
  }

  if (editingFarm.value) {
    const result = await request(`/farms/${editingFarm.value.id}`, {
      method: 'PUT',
      body: JSON.stringify({ name: farmName.value }),
    })
    if (result) {
      showToast('Farm renamed successfully!', 'success')
      closeModal()
      await fetchFarms()
    }
  }
  else {
    const result = await request<Farm>('/farms', {
      method: 'POST',
      body: JSON.stringify({ name: farmName.value }),
    })
    if (result) {
      showToast('Farm created successfully!', 'success')
      closeModal()
      await fetchFarms()
    }
  }
}

function confirmDelete(farm: Farm) {
  confirm('Delete Farm', `Are you sure you want to delete "${farm.name}"? This cannot be undone.`, async () => {
    const result = await request(`/farms/${farm.id}`, { method: 'DELETE' })
    if (result) {
      showToast('Farm deleted.', 'success')
      await fetchFarms()
    }
  })
}

onMounted(fetchFarms)
</script>

<style scoped>
.farms-container {
  max-width: 800px;
  margin: 2rem auto;
  padding: 0 1.5rem;
  animation: fadeIn 0.5s ease-out;
}

.farms-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding: 1.5rem;
  background: var(--card-bg);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  border: 1px solid var(--border-color);
}

.farms-header h2 {
  font-family: "Outfit", sans-serif;
  font-weight: 700;
  background: linear-gradient(to right, var(--primary), #818cf8);
  -webkit-background-clip: text;
  background-clip: text;
  -webkit-text-fill-color: transparent;
}

.subtitle {
  color: var(--text-muted);
  font-size: 0.85rem;
}

.farms-header-actions {
  display: flex;
  gap: 0.75rem;
}

.btn-back,
.btn-create {
  padding: 0.625rem 1.25rem;
  border-radius: var(--radius-sm);
  font-weight: 600;
  font-size: 0.9rem;
  cursor: pointer;
  transition: var(--transition);
  border: 1px solid var(--border-color);
  background: var(--card-bg);
  color: var(--text-main);
  font-family: inherit;
}

.btn-create {
  background: var(--primary);
  color: white;
  border: none;
}

.btn-back:hover {
  border-color: var(--primary);
  color: var(--primary);
}

.btn-create:hover {
  filter: brightness(1.1);
}

.farms-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
  gap: 1.25rem;
}

.farm-card {
  background: var(--card-bg);
  border-radius: var(--radius);
  box-shadow: var(--shadow);
  border: 1px solid var(--border-color);
  padding: 1.5rem;
  transition: var(--transition);
}

.farm-card:hover {
  box-shadow: var(--shadow-lg);
}

.farm-card-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 1rem;
}

.farm-card-header h3 {
  font-family: "Outfit", sans-serif;
  font-weight: 600;
  font-size: 1.15rem;
}

.farm-id {
  font-size: 0.75rem;
  color: var(--text-muted);
  font-family: monospace;
}

.farm-card-actions {
  display: flex;
  gap: 0.75rem;
}

.btn-edit,
.btn-delete {
  flex: 1;
  padding: 0.5rem;
  border-radius: var(--radius-sm);
  font-weight: 500;
  font-size: 0.85rem;
  cursor: pointer;
  transition: var(--transition);
  border: 1px solid var(--border-color);
  background: var(--card-bg);
  color: var(--text-main);
  font-family: inherit;
}

.btn-edit:hover {
  border-color: var(--primary);
  color: var(--primary);
}

.btn-delete:hover {
  border-color: var(--danger);
  color: var(--danger);
}

.loading-state,
.empty-farms {
  text-align: center;
  padding: 4rem 2rem;
  color: var(--text-muted);
}

.empty-farms i {
  font-size: 3rem;
  margin-bottom: 1rem;
  display: block;
}

/* Modal styles */
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(15, 23, 42, 0.6);
  backdrop-filter: blur(4px);
  z-index: 1000;
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 1rem;
}

.modal-content {
  background: var(--card-bg);
  border-radius: var(--radius);
  width: 100%;
  max-width: 400px;
  box-shadow: 0 25px 50px -12px rgba(0, 0, 0, 0.25);
  border: 1px solid var(--border-color);
  overflow: hidden;
  animation: modalPop 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
}

@keyframes modalPop {
  from {
    transform: scale(0.9) translateY(20px);
    opacity: 0;
  }
  to {
    transform: scale(1) translateY(0);
    opacity: 1;
  }
}

.modal-header {
  padding: 1.25rem;
  background: var(--primary);
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-family: "Outfit", sans-serif;
  font-weight: 600;
}

.modal-body {
  padding: 1.5rem;
}

.modal-footer {
  padding: 1.25rem;
  background: rgba(0, 0, 0, 0.02);
  border-top: 1px solid var(--border-color);
  display: flex;
  gap: 1rem;
}

.win-btn {
  flex: 1;
  padding: 0.75rem;
  border-radius: var(--radius-sm);
  font-weight: 600;
  cursor: pointer;
  border: none;
  transition: var(--transition);
  font-family: inherit;
  font-size: 0.95rem;
}

.btn-success {
  background: var(--success);
  color: white;
}

.btn-success:hover {
  filter: brightness(1.1);
}

.btn-cancel {
  background: var(--danger);
  color: white;
}

.btn-cancel:hover {
  filter: brightness(1.1);
}

.close-btn {
  background: none;
  border: none;
  color: white;
  font-size: 1.5rem;
  cursor: pointer;
  line-height: 1;
  opacity: 0.8;
  transition: opacity 0.2s;
}

.close-btn:hover {
  opacity: 1;
}

@media (max-width: 768px) {
  .farms-container {
    padding: 0 1rem;
    margin: 1rem auto;
  }

  .farms-header {
    flex-direction: column;
    gap: 1rem;
    text-align: center;
  }
}
</style>
