<template>
  <header class="amis-header">
    <div class="header-left">
      <i class="misa-icon mi-ninedot-white"></i>
      <div class="app-logo">
        <div class="logo-icon">
          <img data-v-0c4fc81b="" src="https://testcdnamisapp.misacdn.net/apps/payroll/static/logos/icon-payroll-logo.svg?v=1.0" alt="logo" width="48" height="48">
        </div>
        <span class="app-name">Tiền lương</span>
      </div>
    </div>

    <div class="header-center">
      <div class="search-box">
        <i class="misa-icon mi-search-white"></i>
        <input type="text" placeholder="Tìm kiếm" />
      </div>
    </div>

    <div class="header-right">
      <i class="misa-icon mi-setting-white"></i>
      <div class="agent-box">
        <i id="messagebox-ava-agent"></i>
      </div>
      <i class="misa-icon mi-new-feature-white"></i>
      <i class="misa-icon misa-messbox-icon-message-light"></i>
      
      <div class="notification">
        <i class="misa-icon mnoti-selector-box"></i>
        <span class="badge">28</span>
      </div>
      <i class="misa-icon misa-helpmenu-trigger"></i>
      <i class="misa-icon misa-moremenu"></i>
      <i class="misa-icon misa-knowledge-knowledge"></i>
      <div class="avatar">LV</div>
    </div>
  </header>
</template>

<style scoped>
.amis-header {
  height: 38px;
  background-color: #0e9a62;
  color: white;
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 16px;
  box-shadow: 0 2px 4px rgba(0,0,0,0.08);
  z-index: 10;
}

.header-left, .header-center, .header-right { display: flex; align-items: center; }
.header-left { gap: 16px; width: 250px; }
.header-center { flex: 1; justify-content: flex-end; padding-right: 24px; }
.header-right { gap: 16px; }

.icon-menu {
  width: 24px;
  height: 24px;
  cursor: pointer;
}

.app-logo { display: flex; align-items: center; gap: 8px; cursor: pointer; }
.logo-icon {
  display: flex;
  justify-content: center;
  align-items: center;
}
.logo-icon img { width: 28px; height: 28px; }

.app-name { font-size: 16px; font-weight: 700; }

.search-box {
  display: flex;
  align-items: center;
  background-color: rgba(255, 255, 255, 0.2);
  border-radius: 4px;
  padding: 0 4px;
  width: 250px;
  height: 28px;
}
.search-icon { width: 16px; height: 16px; opacity: 0.8; }
.search-box input { background: transparent; border: none; color: white; margin-left: 8px; width: 100%; outline: none; }
.search-box input::placeholder { color: rgba(255, 255, 255, 0.7); }

.header-icon {
  width: 20px;
  height: 20px;
  cursor: pointer;
  opacity: 0.9;
}
.header-icon:hover { opacity: 1; }

.notification { position: relative; cursor: pointer; }
.badge { position: absolute; top: -6px; right: -8px; background-color: #ef292f; color: white; font-size: 10px; font-weight: bold; padding: 2px 4px; border-radius: 10px; }

.avatar { width: 28px; height: 28px; background-color: #3b82f6; border-radius: 50%; display: flex; justify-content: center; align-items: center; font-size: 12px; font-weight: bold; cursor: pointer; border: 1px solid white; }
</style>