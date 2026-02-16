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
