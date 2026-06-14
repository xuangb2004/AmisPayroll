<template>
  <router-view />

  <div v-if="globalLoading" class="misa-loading-overlay">
    <div class="misa-spinner"></div>
  </div>
</template>

<script setup>
import { globalLoading } from './utils/loading.js'; 
</script>

<style>
.misa-loading-overlay {
  position: fixed;
  top: 0; left: 0; right: 0; bottom: 0;
  background-color: rgba(255, 255, 255, 0.6); 
  z-index: 99999; 
  display: flex;
  justify-content: center;
  align-items: center;
}

.misa-spinner {
  width: 40px; height: 40px;
  border: 4px solid #e0e0e0;
  border-top: 4px solid #170589; 
  border-radius: 50%; 
  animation: spin 0.8s linear infinite; 
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}
</style>