
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

export default defineConfig({
  plugins: [react()],
  base: process.env.GITHUB_PAGES ? `/${process.env.GITHUB_REPOSITORY?.split('/')[1]}/` : '/'
})
