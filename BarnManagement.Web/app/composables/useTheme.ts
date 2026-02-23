type Theme = 'light' | 'dark'

const theme = ref<Theme>('light')

function applyTheme(t: Theme) {
  if (import.meta.client) {
    document.documentElement.setAttribute('data-theme', t)
    localStorage.setItem('theme', t)
  }
}

export function useTheme() {
  if (import.meta.client && !document.documentElement.hasAttribute('data-theme')) {
    const stored = localStorage.getItem('theme') as Theme | null
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches
    theme.value = stored || (prefersDark ? 'dark' : 'light')
    applyTheme(theme.value)
  }

  const isDark = computed(() => theme.value === 'dark')

  function toggleTheme() {
    theme.value = theme.value === 'dark' ? 'light' : 'dark'
    applyTheme(theme.value)
  }

  return {
    theme: readonly(theme),
    isDark,
    toggleTheme,
  }
}
