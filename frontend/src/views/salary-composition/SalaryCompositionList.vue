<template>
  <div class="salary-page-container">
    <template v-if="!isAddingNew">
      <div class="page-header">
        <h1 class="page-title">Thành phần lương</h1>
        <div class="page-actions">
          <button class="btn-outline">
            <i class="misa-icon mi-rule"></i> Danh mục của hệ thống
          </button>
          <div class="btn-group-primary">
            <button class="btn-primary" @click="openAddForm">
              <i class="dx-icon-plus"></i> Thêm
            </button>
            <button class="btn-primary-dropdown">
              <i class="misa-icon mi-chevron-down-white"></i>
            </button>
          </div>
        </div>
      </div>
      <div class="page-content-grid">
        <SalaryCompositionTable ref="tableRef" />
      </div>
    </template>

    <template v-else>
      <div class="add-page-header">
        <div class="header-left">
          <button class="btn-icon-back" @click="requestCloseAddForm" aria-label="Thoát thêm thành phần">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#444" stroke-width="2.5"><path d="M19 12H5M12 19l-7-7 7-7"/></svg>
          </button>
          <h1 class="add-page-title">Thêm thành phần</h1>
        </div>
      </div>

      <div class="add-page-content">
        <AddSalaryComposition 
          ref="formRef" 
          @close="requestCloseAddForm" 
          @save-success="handleSaveSuccess"
        />
      </div>

      <div class="add-page-footer">
        <button class="btn-cancel" @click="requestCloseAddForm">Hủy bỏ</button>
        <div class="footer-right">
          <button class="btn-outline-primary" @click="triggerSaveAndAdd">Lưu và thêm</button>
          <button class="btn-primary" @click="triggerSave">Lưu</button>
        </div>
      </div>
    </template>

    <div v-if="isExitConfirmVisible" class="exit-confirm-overlay" @click.self="stayOnAddForm">
      <div 
        class="exit-confirm-dialog" 
        role="dialog" 
        aria-modal="true" 
        aria-labelledby="exit-confirm-title"
        :style="dialogStyle"
        @mousedown="startDrag"
      >
        <button class="exit-confirm-close" @click="stayOnAddForm" aria-label="Đóng">×</button>
        <h2 id="exit-confirm-title">Thoát và không lưu?</h2>
        <p>Nếu bạn thoát, các dữ liệu đang nhập liệu sẽ không được lưu lại.</p>
        <div class="exit-confirm-actions">
          <button class="btn-stay" @click="stayOnAddForm">Ở lại</button>
          <button class="btn-exit" @click="closeAddForm">Thoát, không lưu</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, computed } from 'vue';
import SalaryCompositionTable from '../../components/salary-composition/SalaryCompositionTable.vue';
import AddSalaryComposition from './components/AddSalaryComposition.vue';

const isAddingNew = ref(false);
const isExitConfirmVisible = ref(false);
const formRef = ref(null);
const tableRef = ref(null); 

// ==========================================
// LOGIC KÉO THẢ (DRAG & DROP) CHO THÔNG BÁO
// ==========================================
const isDragging = ref(false);
const position = ref({ x: 0, y: 0 }); // Vị trí hiện tại
const dragStart = ref({ x: 0, y: 0 }); // Điểm bắt đầu kéo

// Tính toán style di chuyển và con trỏ chuột
const dialogStyle = computed(() => ({
  transform: `translate(${position.value.x}px, ${position.value.y}px)`,
  cursor: isDragging.value ? 'grabbing' : 'grab'
}));

const startDrag = (event) => {
  if (event.target.tagName.toLowerCase() === 'button') return;

  isDragging.value = true;
  dragStart.value = {
    x: event.clientX - position.value.x,
    y: event.clientY - position.value.y
  };
  document.addEventListener('mousemove', onDrag);
  document.addEventListener('mouseup', stopDrag);
};

const onDrag = (event) => {
  if (!isDragging.value) return;
  position.value = {
    x: event.clientX - dragStart.value.x,
    y: event.clientY - dragStart.value.y
  };
};

const stopDrag = () => {
  isDragging.value = false;
  document.removeEventListener('mousemove', onDrag);
  document.removeEventListener('mouseup', stopDrag);
};

// ==========================================
// ĐIỀU HƯỚNG & HÀNH ĐỘNG CỦA FORM
// ==========================================
const openAddForm = () => { isAddingNew.value = true; };

const requestCloseAddForm = () => { 
  // Reset vị trí popup về căn giữa màn hình mỗi khi mở lên
  position.value = { x: 0, y: 0 }; 
  isExitConfirmVisible.value = true; 
};

const stayOnAddForm = () => { isExitConfirmVisible.value = false; };
const closeAddForm = () => {
  isExitConfirmVisible.value = false;
  isAddingNew.value = false;
};

const triggerSave = () => { if (formRef.value) formRef.value.save(); };
const triggerSaveAndAdd = () => { if (formRef.value) formRef.value.saveAndAdd(); };

const handleSaveSuccess = (newData) => {
  isAddingNew.value = false; 
  if (tableRef.value) {
    if (typeof tableRef.value.loadData === 'function') {
      tableRef.value.loadData();
    } else if (typeof tableRef.value.addRecord === 'function') {
      tableRef.value.addRecord(newData);
    }
  }
};
</script>

<style scoped>
/* Tổng thể trang */
.salary-page-container {
  display: flex; flex-direction: column; height: 100%; width: 100%;
  background-color: #f4f5f8; 
  overflow: hidden;
}

/* Header & Content của màn hình danh sách */
.page-header { display: flex; justify-content: space-between; align-items: center; padding: 0 0 8px 0; }
.page-title { font-size: 16px; font-weight: 700; color: #212121; }
.page-actions { display: flex; gap: 12px; align-items: center; }
.page-content-grid { flex: 1; background: #fff; border-radius: 4px; overflow: hidden; }

/* GIAO DIỆN KHI NHẤN "THÊM" */
.add-page-header { padding: 16px 0; flex-shrink: 0; }
.header-left { display: flex; align-items: center; }
.btn-icon-back {
  background-color: transparent; border: none; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  width: 24px; height: 24px; border-radius: 0; margin-right: 12px;
  padding: 0;
}
.btn-icon-back:hover svg { stroke: #111111; }
.add-page-title { font-size: 20px; font-weight: 700; color: #111; }

.add-page-content { flex: 1; overflow-y: auto; }

.add-page-footer {
  display: flex; justify-content: flex-end; align-items: center;
  padding: 16px 0; flex-shrink: 0;
}
.footer-right { display: flex; gap: 12px; }

/* CÁC NÚT BẤM (BUTTONS) */
.btn-primary { 
  background: #00ab6b; border: none; color: #fff; padding: 0 16px; 
  height: 32px; border-radius: 4px; cursor: pointer; font-weight: 600; 
  display: flex; align-items: center; justify-content: center; gap: 6px;
  transition: background-color 0.2s;
}
.btn-primary:hover { background: #00995f; }

.btn-outline-primary { 
  background: #fff; border: 1px solid #00ab6b; color: #00ab6b; 
  padding: 0 16px; height: 32px; border-radius: 4px; cursor: pointer; font-weight: 500; 
  transition: all 0.2s;
}
.btn-outline-primary:hover { background: #e5f6ed; }

.btn-cancel { 
  background: #fff; border: 1px solid transparent; 
  cursor: pointer; padding: 0 16px; height: 32px; color: #111; border-radius: 4px; margin-right: 12px;
}
.btn-cancel:hover { background: #e0e0e0; }

.btn-group-primary {
  display: flex; border-radius: 8px; overflow: hidden; height: 32px; box-shadow: 0 1px 2px rgba(0,0,0,0.1);
}
.btn-group-primary .btn-primary { height: 100%; position: relative; border-radius: 0; }
.btn-group-primary .btn-primary::after {
  content: ""; position: absolute; right: 0; top: 20%; height: 60%; width: 1px; background: rgba(255, 255, 255, 0.963);
}

.btn-primary-dropdown { 
  background: #00ab6b; border: none; color: #fff; padding: 0 8px; 
  height: 100%; cursor: pointer; display: flex; align-items: center; justify-content: center;
  transition: background-color 0.2s;
}
.btn-primary-dropdown:hover { background: #00995f; }

.btn-outline { 
  padding: 0 12px; height: 32px; background: #fff; border: 1px solid #e0e0e0; 
  border-radius: 4px; cursor: pointer; display: flex; align-items: center; gap: 6px;
  color: #111; font-weight: 500;
}
.btn-outline:hover { background: #f4f5f8; }


/* =======================================================
   CSS CHO POPUP XÁC NHẬN THOÁT 
   ======================================================= */
.exit-confirm-overlay {
  position: fixed;
  inset: 0;
  z-index: 1000;
  display: flex;
  align-items: flex-start; 
  justify-content: center;
  background: rgba(0, 0, 0, 0.4); 
}

.exit-confirm-dialog {
  position: relative;
  width: 420px; 
  min-height: 160px;
  padding: 20px 24px;
  background: #ffffff;
  border-radius: 12px; 
  margin-top: 35vh; 
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.18);
  color: #111111;
  transition: border-radius 0.2s ease, box-shadow 0.2s ease;
}

.exit-confirm-dialog:hover {
  cursor: grab;
}
.exit-confirm-dialog:active {
  cursor: grabbing;
}

.exit-confirm-dialog h2 {
  margin: 0 28px 10px 0;
  font-size: 16px;
  line-height: 22px;
  font-weight: 700;
}

.exit-confirm-dialog p {
  margin: 0;
  color: #111111;
  font-size: 13px;
  line-height: 18px;
}

.exit-confirm-close {
  position: absolute;
  top: 12px;
  right: 12px;
  width: 24px;
  height: 24px;
  border: none;
  background: transparent;
  color: #666666;
  font-size: 20px;
  line-height: 20px;
  cursor: pointer;
}

.exit-confirm-close:hover {
  color: #111111;
}

/* Các nút bấm dài ra */
.exit-confirm-actions {
  display: flex;
  justify-content: flex-end;
  gap: 16px; 
  margin-top: 24px;
}

.btn-stay,
.btn-exit {
  height: 32px; 
  padding: 0 11px; 
  border-radius: 8px;
  font-size: 13px;
  font-weight: 400;
  cursor: pointer;
  min-width: 70px;
}

.btn-stay {
  background: #ffffff;
  color: #111111;
  border: 1px solid #d9d9d9;
  width: 80px;
}

.btn-stay:hover {
  background: #e2e5e8;
}

.btn-exit {
  background: #019c64;
  color: #ffffff;
  border: 1px solid #00ab6b;
  min-width: 60px;
}

.btn-exit:hover {
  background: #01784a;
  border-color: #00995f;
}
</style>