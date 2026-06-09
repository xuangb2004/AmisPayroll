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
          <button class="btn-icon-back" @click="closeAddForm">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="#444" stroke-width="2.5"><path d="M19 12H5M12 19l-7-7 7-7"/></svg>
          </button>
          <h1 class="add-page-title">Thêm thành phần</h1>
        </div>
      </div>

      <div class="add-page-content">
        <AddSalaryComposition 
          ref="formRef" 
          @close="closeAddForm" 
          @save-success="handleSaveSuccess"
        />
      </div>

      <div class="add-page-footer">
        <button class="btn-cancel" @click="closeAddForm">Hủy bỏ</button>
        <div class="footer-right">
          <button class="btn-outline-primary" @click="triggerSaveAndAdd">Lưu và thêm</button>
          <button class="btn-primary" @click="triggerSave">Lưu</button>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup>
import { ref } from 'vue';
import SalaryCompositionTable from '../../components/salary-composition/SalaryCompositionTable.vue';
import AddSalaryComposition from './components/AddSalaryComposition.vue';

const isAddingNew = ref(false);
const formRef = ref(null);
const tableRef = ref(null); // BỔ SUNG: Tham chiếu đến bảng

const openAddForm = () => { isAddingNew.value = true; };
const closeAddForm = () => { isAddingNew.value = false; };

const triggerSave = () => { if (formRef.value) formRef.value.save(); };
const triggerSaveAndAdd = () => { if (formRef.value) formRef.value.saveAndAdd(); };

// BỔ SUNG: Hàm xử lý khi form con báo đã lưu thành công
const handleSaveSuccess = (newData) => {
  isAddingNew.value = false; // Đóng form
  
  // Gọi hàm loadData() (hoặc fetchData) được phơi bày (expose) từ file Table
  if (tableRef.value) {
    if (typeof tableRef.value.loadData === 'function') {
      tableRef.value.loadData();
    } else if (typeof tableRef.value.addRecord === 'function') {
      // Nếu đang dùng mảng mock tĩnh, có thể gọi hàm addRecord để nhét data vào bảng
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
  background-color: #e1e4e6; border: none; cursor: pointer;
  display: flex; align-items: center; justify-content: center;
  width: 32px; height: 32px; border-radius: 50%; margin-right: 16px;
}
.btn-icon-back:hover { background-color: #d1d4d6; }
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
</style>