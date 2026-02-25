export function useSse() {
  const eventSource = ref<EventSource | null>(null)

  function connect(endpoint: string, onMessage: (data: string) => void, onEvent?: Record<string, (data: string) => void>) {
    if (import.meta.server) return

    if (eventSource.value) {
      eventSource.value.close()
    }

    // Since useApi uses '/api', we use the same convention.
    // Standard EventSource doesn't easily support Authorization headers.
    // For now, we assume the backend doesn't require JWT for SSE or handles it via cookies/query.
    // Given the current setup, we'll try connection without token first.
    const es = new EventSource(`/api${endpoint}`)

    es.onmessage = (event) => {
      onMessage(event.data)
    }

    if (onEvent) {
      Object.entries(onEvent).forEach(([name, callback]) => {
        es.addEventListener(name, (event: any) => {
          callback(event.data)
        })
      })
    }

    es.onerror = () => {
      console.error('SSE Connection failed. Reconnecting in 5s...')
      es.close()
      setTimeout(() => connect(endpoint, onMessage, onEvent), 5000)
    }

    eventSource.value = es
  }

  function disconnect() {
    if (eventSource.value) {
      eventSource.value.close()
      eventSource.value = null
    }
  }

  onUnmounted(() => {
    disconnect()
  })

  return {
    connect,
    disconnect,
  }
}
