<template>
  <div v-if="visible" class="modal-overlay" @click.self="$emit('cancel')">
    <div class="modal-content">
      <div class="modal-header">
        <span>{{ title }}</span>
        <button class="close-btn" @click="$emit('cancel')">
          &times;
        </button>
      </div>
      <div class="modal-body">
        <p style="margin: 0">
          {{ message }}
        </p>
      </div>
      <div class="modal-footer">
        <button class="win-btn btn-success" @click="$emit('confirm')">
          OK
        </button>
        <button class="win-btn btn-danger" @click="$emit('cancel')">
          Cancel
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
defineProps<{
  visible: boolean
  title: string
  message: string
}>()

defineEmits<{
  confirm: []
  cancel: []
}>()
</script>

<style scoped>
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

.btn-danger {
  background: var(--danger);
  color: white;
}

.btn-danger:hover {
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
  .modal-content {
    max-width: 90vw;
    margin: 0 auto;
  }

  .modal-body {
    padding: 1.25rem;
  }

  .modal-footer {
    padding: 1rem;
  }
}

@media (max-width: 480px) {
  .modal-overlay {
    padding: 0.75rem;
    align-items: flex-end;
  }

  .modal-content {
    max-width: 100%;
    border-radius: var(--radius) var(--radius) 0 0;
    animation: modalSlideUp 0.3s cubic-bezier(0.34, 1.56, 0.64, 1);
  }

  @keyframes modalSlideUp {
    from {
      transform: translateY(100%);
      opacity: 0;
    }
    to {
      transform: translateY(0);
      opacity: 1;
    }
  }

  .modal-body {
    padding: 1rem;
  }

  .modal-footer {
    padding: 0.875rem;
  }

  .win-btn {
    padding: 0.875rem;
    font-size: 1rem;
  }
}
</style>
