import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vite.dev/config/
export default defineConfig({
  plugins: [vue()],
  base: '/BarnManagement/', // Eğer repo adınız farklıysa burayı güncelleyin
})
