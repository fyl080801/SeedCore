import { defineConfig } from 'vite'
import vue from '@vitejs/plugin-vue'

// https://vitejs.dev/config/
export default defineConfig({
  base: process.env.SEED_MODULE,
  server: {
    port: process.env.PORT,
  },
  build: {
    manifest: true,
    rollupOptions: {
      input: 'index.js',
    },
  },
  plugins: [vue()],
})
