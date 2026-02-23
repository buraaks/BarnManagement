interface ConfirmModalState {
  visible: boolean
  title: string
  message: string
  onConfirm: () => void
}

const state = reactive<ConfirmModalState>({
  visible: false,
  title: '',
  message: '',
  onConfirm: () => {},
})

export function useConfirmModal() {
  function confirm(title: string, message: string, onConfirm: () => void) {
    state.title = title
    state.message = message
    state.onConfirm = () => {
      state.visible = false
      onConfirm()
    }
    state.visible = true
  }

  function cancel() {
    state.visible = false
  }

  return {
    state: readonly(state),
    confirm,
    cancel,
  }
}
