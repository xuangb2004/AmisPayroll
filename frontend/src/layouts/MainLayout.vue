<template>
  <div class="amis-layout">
    <AppHeader />
    <div class="amis-body">
      <AppSidebar />
      <main class="amis-content">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script setup>
import AppHeader from '../components/layout/AppHeader.vue'
import AppSidebar from '../components/layout/AppSidebar.vue'
</script>

<style scoped>
.amis-layout {
  display: flex;
  flex-direction: column;
  height: 100vh;
  width: 100vw;
  overflow: hidden;
  background-color: #f4f5f8; 
}

.amis-body {
  display: flex;
  flex: 1;
  overflow: hidden;
}

.amis-content {
  flex: 1;
  padding: 8px 16px;
  overflow: hidden; 
  display: flex;
  flex-direction: column;
}
</style>