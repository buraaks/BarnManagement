interface Toast {
  id: number
  message: string
  type: 'info' | 'error' | 'success'
  fadingOut: boolean
}

const toasts = ref<Toast[]>([])
let nextId = 0

export function useToast() {
  function showToast(message: string, type: Toast['type'] = 'info') {
    const id = nextId++
    toasts.value.push({ id, message, type, fadingOut: false })

    setTimeout(() => {
      const toast = toasts.value.find(t => t.id === id)
      if (toast) toast.fadingOut = true
      setTimeout(() => {
        toasts.value = toasts.value.filter(t => t.id !== id)
      }, 300)
    }, 3000)
  }

  return {
    toasts: readonly(toasts),
    showToast,
  }
}
