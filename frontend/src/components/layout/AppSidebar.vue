<template>
  <aside class="amis-sidebar" :class="{ 'is-expanded': isExpanded }">
    
    <div class="menu-list">
      <div class="menu-item">
        <i class="misa-icon mi-home"></i>
        <span class="menu-text" v-show="isExpanded">Tổng quan</span>
      </div>
      
      <div class="menu-item active">
        <i class="misa-icon mi-salary-composition"></i>
        <span class="menu-text" v-show="isExpanded">Thành phần lương</span>
      </div>
      
      <div class="menu-item">
        <i class="misa-icon mi-payroll-template"></i>
        <span class="menu-text" v-show="isExpanded">Mẫu bảng lương</span>
      </div>
      
      <div class="menu-item">
        <i class="misa-icon mi-salary-data"></i>
        <span class="menu-text" v-show="isExpanded">Dữ liệu tính lương</span>
        <i class="misa-icon mi-chevron-right arrow-icon" v-show="isExpanded"></i>
      </div>
      
      <div class="menu-item">
        <i class="misa-icon mi-calculator"></i>
        <span class="menu-text" v-show="isExpanded">Tính lương</span>
        <i class="misa-icon mi-chevron-right arrow-icon" v-show="isExpanded"></i>
      </div>
      
      <div class="menu-item">
        <i class="misa-icon mi-money"></i>
        <span class="menu-text" v-show="isExpanded">Chi trả</span>
        <i class="misa-icon mi-chevron-right arrow-icon" v-show="isExpanded"></i>
      </div>
      
      <div class="menu-item">
        <i class="misa-icon mi-report"></i>
        <span class="menu-text" v-show="isExpanded">Báo cáo</span>
      </div>
    </div>

    <div class="sidebar-footer">
      <div class="toggle-btn" @click="toggleSidebar">
        <i class="misa-icon" :class="isExpanded ? 'mi-arrow-left' : 'mi-arrow-right'"></i>
      </div>
    </div>
  </aside>
</template>

<script setup>
import { ref } from 'vue'

const isExpanded = ref(true)

const toggleSidebar = () => {
  isExpanded.value = !isExpanded.value
}
</script>

<style scoped>
.amis-sidebar {
  width: 64px; 
  background-color: #ffffff;
  border-right: 1px solid #e0e0e0;
  display: flex;
  flex-direction: column;
  justify-content: space-between; 
  transition: width 0.2s ease; 
  overflow: hidden;
  z-index: 5;
}

.amis-sidebar.is-expanded {
  width: 240px; 
}

.menu-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
  padding: 8px;
}

.amis-sidebar:not(.is-expanded) .menu-list {
  padding: 12px 8px;
  align-items: center;
}


.menu-item {
  display: flex;
  align-items: center;
  height: 26px;
  padding: 0 16px;
  border-radius: 8px;
  cursor: pointer;
  color: #111111;
  white-space: nowrap; 
  transition: background-color 0.2s;
}


.amis-sidebar:not(.is-expanded) .menu-item {
  width: 44px;
  justify-content: center;
  padding: 0;
}

.menu-item:hover {
  background-color: #f4f5f8;
}

.menu-item.active {
  background-color: #e5f6ed; 
  color: #00ab6b; 
  font-weight: 500;
}

.menu-text {
  margin-left: 10px;
  flex: 1; 
  font-size: 12px;
}

.arrow-icon {
  width: 16px;
  height: 16px;
  opacity: 0.6;
}
/* 4. FOOTER VÀ NÚT TOGGLE */
.sidebar-footer {
  display: flex;
  justify-content: flex-end; 
  
}


.amis-sidebar:not(.is-expanded) .sidebar-footer {
  justify-content: center; 
}


.toggle-btn {
  width: 36px; 
  height: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  background-color: transparent;
  transition: all 0.2s;
  
 
  border-radius: 0; 
  border: none;
  border-left: 1px solid #e0e0e0;
  border-top: 1px solid #e0e0e0; 
}

/* Nút Toggle ở trạng thái THU GỌN */
.amis-sidebar:not(.is-expanded) .toggle-btn {
  width: 100%; 
  border-left: none; 
  border-top: 1px solid #e0e0e0;
}

.toggle-btn:hover {
  background-color: #f4f5f8;
}
</style>