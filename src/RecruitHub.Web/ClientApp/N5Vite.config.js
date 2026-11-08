import { defineConfig } from "vite";
import { resolve } from "node:path";
export default defineConfig({
  define: { "process.env.NODE_ENV": JSON.stringify("production") },
  build: {
    outDir: "../wwwroot/dashboard",
    emptyOutDir: true,
    lib: {
      entry: resolve(import.meta.dirname, "N5Dashboard.jsx"),
      formats: ["es"],
      fileName: () => "n5-dashboard.js",
    },
    rollupOptions: { output: { assetFileNames: "n5-dashboard.[ext]" } },
  },
});
