import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';

// All /api calls are proxied to the ASP.NET Core API during development,
// so the UI never hard-codes data and no CORS issues arise locally.
export default defineConfig({
  plugins: [react()],
  server: {
    port: 5173,
    proxy: { '/api': { target: 'http://localhost:5000', changeOrigin: true } }
  }
});
