import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
    plugins: [react()],

    // Build optimizations
    build: {
        // Enable code splitting
        rollupOptions: {
            output: {
                manualChunks: (id) => {
                    // Vendor chunks - only include packages that actually exist
                    if (id.includes('node_modules')) {
                        if (id.includes('react') || id.includes('react-dom')) {
                            return 'vendor';
                        }
                        if (id.includes('react-router-dom')) {
                            return 'router';
                        }
                        if (id.includes('axios')) {
                            return 'utils';
                        }
                        return 'vendor';
                    }
                },
            },
        },
        // Optimize chunk size
        chunkSizeWarningLimit: 1000,
        // Enable minification
        minify: 'esbuild',
        // Source maps for production debugging (optional)
        sourcemap: false,
    },

    // Development optimizations
    server: {
        // Enable HMR
        hmr: true,
        // Port configuration
        port: 3000,
        // Open browser automatically
        open: true,
    },

    // Optimize dependencies - only include packages that exist
    optimizeDeps: {
        include: [
            'react',
            'react-dom',
            'react-router-dom',
            'axios',
        ],
    },

    // Enable source maps for development
    css: {
        devSourcemap: true,
    },
});