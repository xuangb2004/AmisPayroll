<template>
  <div class="grid-wrapper">
    <DxDataGrid
      :data-source="dataSource"
      ref="dataGridRef"
      v-model:selected-row-keys="selectedRowKeys"
      @selection-changed="onSelectionChanged"
      :show-borders="false"
      :show-row-lines="true"
      :column-auto-width="true"
      :hover-state-enabled="true"
      height="100%"
      width="100%"
    >
      <DxSelection mode="multiple" show-check-boxes-mode="always" />

      <DxToolbar>
        <DxItem location="before">
          <template #default>
            <div class="toolbar-search">
              <i class="misa-icon mi-search"></i>
              <input
                v-model="searchText"
                type="text"
                placeholder="Tìm kiếm"
                @input="handleSearchInput"
                @keydown.enter="applySearch"
                @focus="handleSearchInput" 
              />
              <ul v-if="isSearchDropdownOpen && searchSuggestions.length > 0" class="search-suggestions">
                <li 
                  v-for="item in searchSuggestions" 
                  :key="item.compositionId" 
                  @click="selectSuggestion(item)"
                >
                  <span class="suggest-name">{{ item.compositionName }}</span>
                  <span class="suggest-dash"> - </span>
                  <span class="suggest-code">{{ item.compositionCode }}</span>
                </li>
              </ul>
            </div>
          </template>
        </DxItem>
        <DxItem location="before" v-if="selectedRowKeys.length === 0">
          <template #default>
            <div class="toolbar-dropdown dropdown-status">
              <span>Trạng thái: <strong>Tất cả</strong></span>
              <i class="misa-icon mi-chevron-down"></i>
            </div>
          </template>
        </DxItem>
        <DxItem location="before" v-if="selectedRowKeys.length === 0">
          <template #default>
            <div class="toolbar-dropdown dropdown-unit">
              <span>Tất cả đơn vị</span>
              <i class="misa-icon mi-chevron-down"></i>
            </div>
          </template>
        </DxItem>

        <DxItem location="before" v-if="selectedRowKeys.length > 0">
          <template #default>
            <div class="batch-info">
              <span>Đã chọn <strong>{{ selectedRowKeys.length }}</strong></span>
              <span class="btn-deselect" @click="deselectAll">Bỏ chọn</span>
            </div>
          </template>
        </DxItem>
        
        <DxItem location="before" v-if="selectedRowKeys.length > 0">
          <template #default>
            <div class="batch-buttons">
              <button v-if="hasTrackingStatus" class="batch-btn btn-batch-warning">
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="8" y1="12" x2="16" y2="12"></line></svg>
                Ngừng theo dõi
              </button>

              <button v-if="hasStoppedStatus" class="batch-btn btn-batch-tracking">
                <i class="misa-icon mi-circle-check-green"></i>
                Đang theo dõi
              </button>

              <button class="batch-btn btn-batch-danger">
                <i class="misa-icon mi-trash-red"></i>
                Xóa
              </button>
            </div>
          </template>
        </DxItem>
        <DxItem location="after" v-if="selectedRowKeys.length === 0">
          <template #default>
            <button class="icon-btn" title="Bộ lọc"><i class="misa-icon mi-filter"></i></button>
          </template>
        </DxItem>
        <DxItem location="after" v-if="selectedRowKeys.length === 0">
          <template #default>
            <button class="icon-btn" title="Cài đặt"><i class="misa-icon mi-setting"></i></button>
          </template>
        </DxItem>
      </DxToolbar>

      <DxColumn data-field="compositionCode" caption="Mã thành phần" width="130" />
      <DxColumn data-field="compositionName" caption="Tên thành phần" min-width="200" /> 
      
      <DxColumn data-field="organizationId" caption="Đơn vị áp dụng" width="180" cell-template="unitTemplate" />
      <template #unitTemplate="{ data }">
        <span>{{ getUnitName(data.value) }}</span>
      </template>

      <DxColumn data-field="compositionType" caption="Loại thành phần" width="150" :customize-text="formatType" />
      <DxColumn data-field="compositionNature" caption="Tính chất" width="120" :customize-text="formatNature" />
      <DxColumn data-field="valueType" caption="Kiểu giá trị" width="120" :customize-text="formatValueType" />
      
      <DxColumn data-field="amount" caption="Giá trị" width="120" />
      <DxColumn data-field="sourceType" caption="Nguồn tạo" width="120" :customize-text="formatSource" />
      
      <DxColumn data-field="status" caption="Trạng thái" width="130" cell-template="statusTemplate" />
      <template #statusTemplate="{ data }">
        <div v-if="data.value === 1" class="status-badge status-tracking">
          <div class="status-dot"></div> 
          <span>Đang theo dõi</span>
        </div>
        <div v-else-if="data.value === 2" class="status-badge status-stopped">
          <div class="status-dot"></div>
          <span>Ngừng theo dõi</span>
        </div>
      </template>

      <DxColumn 
        cell-template="actionTemplate" 
        css-class="column-actions"
        :width="60"
      />
      <template #actionTemplate="{ data }">
        <div class="row-actions">
          
          <button 
            v-if="data.data.status === 1 || data.data.Status === 1" 
            class="action-btn btn-warning" 
            title="Ngừng theo dõi"
            @click="handleUpdateStatusSingle(data.key, 2, data.data)" 
          >
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="8" y1="12" x2="16" y2="12"></line></svg>
          </button>

          <button 
            v-if="data.data.status === 2 || data.data.Status === 2" 
            class="action-btn" style="color: #00ab6b;"
            title="Đang theo dõi"
            @click="handleUpdateStatusSingle(data.key, 1, data.data)"
          >
            <i class="misa-icon mi-circle-check-green"></i>
          </button>

          <button class="action-btn btn-normal" title="Nhân bản">
            <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path></svg>
          </button>
          
          <button class="action-btn btn-normal" title="Sửa">
            <i class="misa-icon mi-pencil"></i>
          </button>
          
          <button class="action-btn btn-danger" title="Xóa" @click="handleDeleteSingle(data.key)">
            <i class="misa-icon mi-trash-red"></i>
          </button>
        </div>
      </template>

      <DxPaging :page-size="pageSize" />
      <DxPager :visible="false" />
    </DxDataGrid>

    <div class="misa-empty-state" v-if="totalRecords === 0 && !isLoading">
      <i class="ms-table__icon_nodata"></i>
      <span>Không có dữ liệu</span>
    </div>


  <div class="misa-pagination">
    <div class="pagination-left">
      Tổng số: <strong>{{ totalRecords }}</strong>
    </div>
    
    <div class="pagination-right">
      <span class="page-size-label">Số dòng/trang</span>
      
      <div class="page-size-selector">
        <select v-model="pageSize" @change="onPageSizeChange">
          <option :value="15">15</option>
          <option :value="25">25</option>
          <option :value="50">50</option>
          <option :value="100">100</option>
        </select>
      </div>
      
      <span class="page-range">
        <strong>{{ startRecord }} - {{ endRecord }}</strong>
      </span>
      
      <div class="page-navigation">
        <button class="nav-btn" :disabled="currentPage === 1" @click="changePage(1)">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M19 12H5M5 12L12 19M5 12L12 5M2 5V19"/></svg>
        </button>
        <button class="nav-btn" :disabled="currentPage === 1" @click="changePage(currentPage - 1)">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M15 18l-6-6 6-6"/></svg>
        </button>
        <button class="nav-btn" :disabled="currentPage === totalPages || totalRecords === 0" @click="changePage(currentPage + 1)">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 18l6-6-6-6"/></svg>
        </button>
        <button class="nav-btn" :disabled="currentPage === totalPages || totalRecords === 0" @click="changePage(totalPages)">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12H19M19 12L12 5M19 12L12 19M22 5V19"/></svg>
        </button>
      </div>
    </div>
  </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import axios from 'axios';
import CustomStore from 'devextreme/data/custom_store';
import notify from 'devextreme/ui/notify';

import {
  DxDataGrid, DxColumn, DxSelection,
  DxPaging, DxPager, DxToolbar, DxItem
} from 'devextreme-vue/data-grid';

// Biến Popup
const isShowAddPopup = ref(false);
const handleOpenAddPopup = () => { isShowAddPopup.value = true; };

// Biến cấu hình Grid
const dataGridRef = ref(null);
const totalRecords = ref(0); 

// Biến Search Dropdown & Loading
const searchText = ref('');
const searchSuggestions = ref([]);
const isSearchDropdownOpen = ref(false);
const isLoading = ref(false); 
let searchTimer = null;

// ==========================================
// 1. KẾT NỐI BACKEND C#/.NET BẰNG CUSTOM STORE
// ==========================================
const API_URL = 'http://localhost:5094/api/v1/SalaryCompositions';

const dataSource = new CustomStore({
  key: 'compositionId', 

  load: async (loadOptions) => {
    isLoading.value = true; // Bật loading để ẩn Empty State tạm thời
    try {
      const params = {
        skip: loadOptions.skip || 0,
        take: loadOptions.take || pageSize.value,
        searchValue: searchText.value.trim()
      };
      
      const response = await axios.get(API_URL, { params });
      const dataArray = response.data.data; 
      
      if (!Array.isArray(dataArray)) {
          return { data: [], totalCount: 0 };
      }

      // Cập nhật tổng số dòng
      const total = response.data.totalRecord || response.data.TotalRecord || dataArray.length;
      totalRecords.value = total;

      return {
        data: dataArray,
        totalCount: total
      };

    } catch (error) {
      console.error("Lỗi khi load dữ liệu:", error);
      throw 'Data Loading Error';
    } finally {
      isLoading.value = false; 
    }
  },

  remove: async (key) => {
    try {
      await axios.delete(`${API_URL}/${key}`);
    } catch (error) {
      throw 'Delete Error';
    }
  }
});


// ==========================================
// 2. LOGIC THANH TÌM KIẾM (SEARCH DROPDOWN)
// ==========================================
const handleSearchInput = () => {
  clearTimeout(searchTimer);
  searchTimer = setTimeout(async () => {
    const keyword = searchText.value.trim();
    if (keyword.length > 0) {
      try {
        const response = await axios.get(`${API_URL}?skip=0&take=5&searchValue=${keyword}`);
        searchSuggestions.value = response.data.data || [];
        isSearchDropdownOpen.value = true;
      } catch (error) {
        console.error("Lỗi lấy gợi ý:", error);
      }
    } else {
      searchSuggestions.value = [];
      isSearchDropdownOpen.value = false;
      applySearch(); // Tải lại bảng rỗng
    }
  }, 300); 
};

const selectSuggestion = (item) => {
  searchText.value = item.compositionName; // Hiển thị nguyên Tên
  isSearchDropdownOpen.value = false;
  applySearch(); // Search thẳng vào lưới DataGrid
};

const closeSearchDropdown = (event) => {
  if (!event.target.closest('.toolbar-search')) {
    isSearchDropdownOpen.value = false;
  }
};

const applySearch = () => {
  currentPage.value = 1;
  isSearchDropdownOpen.value = false;
  if (dataGridRef.value && dataGridRef.value.instance) {
    dataGridRef.value.instance.pageIndex(0);
    dataGridRef.value.instance.refresh();
  }
};

// Đăng ký event click để đóng dropdown search
onMounted(() => {
  document.addEventListener('click', closeSearchDropdown);
});

onUnmounted(() => {
  document.removeEventListener('click', closeSearchDropdown);
});


// ==========================================
// 3. LOGIC CHỌN HÀNG LOẠT & THAO TÁC CƠ BẢN
// ==========================================
const selectedRowKeys = ref([]);
const selectedRowsData = ref([]);

const hasStoppedStatus = computed(() => selectedRowsData.value.some(row => row.Status === 'Ngừng sử dụng' || row.Status === 'Ngừng theo dõi' || row.status === 2));
const hasTrackingStatus = computed(() => selectedRowsData.value.some(row => row.Status === 'Đang theo dõi' || row.status === 1));

const onSelectionChanged = (e) => {
  selectedRowKeys.value = e.selectedRowKeys;
  selectedRowsData.value = e.selectedRowsData;
};

const deselectAll = () => {
  selectedRowKeys.value = [];
  selectedRowsData.value = [];
};

const handleDeleteSingle = async (compositionId) => {
  const isConfirm = confirm(`Bạn có chắc chắn muốn xóa thành phần "${compositionId}" không?`);
  if (isConfirm) {
    try {
      await dataSource.remove(compositionId);
      notify("Xóa bản ghi thành công", "success", 3000);
      if(dataGridRef.value) dataGridRef.value.instance.refresh();
    } catch (error) {
      notify("Có lỗi xảy ra khi xóa", "error", 3000);
    }
  }
};

const handleUpdateStatusSingle = async (compositionId, newStatus, rowData) => {
  try {
    const updatedPayload = {
      OrganizationId: rowData.organizationId || rowData.OrganizationId,
      CompositionCode: rowData.compositionCode || rowData.CompositionCode,
      CompositionName: rowData.compositionName || rowData.CompositionName,
      CompositionType: rowData.compositionType || rowData.CompositionType,
      CompositionNature: rowData.compositionNature || rowData.CompositionNature,
      TaxNature: rowData.taxNature || rowData.TaxNature || null,
      NormFormula: rowData.normFormula || rowData.NormFormula || "",
      IsAllowExceedNorm: rowData.isAllowExceedNorm || rowData.IsAllowExceedNorm || 0,
      ValueType: rowData.valueType || rowData.ValueType,
      Amount: rowData.amount || rowData.Amount || 0,
      CalculationFormula: rowData.calculationFormula || rowData.CalculationFormula || "",
      Description: rowData.description || rowData.Description || "",
      IsDisplayOnPayslip: rowData.isDisplayOnPayslip || rowData.IsDisplayOnPayslip || 0,
      SourceType: rowData.sourceType || rowData.SourceType,
      Status: newStatus
    };

    await axios.put(`${API_URL}/${compositionId}`, updatedPayload);
    const statusText = newStatus === 1 ? 'Đang theo dõi' : 'Ngừng theo dõi';
    notify(`Đã chuyển sang trạng thái: ${statusText}`, "success", 3000);
    if(dataGridRef.value) dataGridRef.value.instance.refresh();
  } catch (error) {
    notify("Lỗi khi cập nhật trạng thái", "error", 3000);
  }
};


// ==========================================
// 4. LOGIC PHÂN TRANG CUSTOM
// ==========================================
const currentPage = ref(1);
const pageSize = ref(15); 

const totalPages = computed(() => Math.ceil(totalRecords.value / pageSize.value));

const startRecord = computed(() => {
  if (totalRecords.value === 0) return 0;
  return (currentPage.value - 1) * pageSize.value + 1;
});

const endRecord = computed(() => {
  const end = currentPage.value * pageSize.value;
  return end > totalRecords.value ? totalRecords.value : end;
});

const changePage = (newPage) => {
  if (newPage >= 1 && newPage <= totalPages.value) {
    currentPage.value = newPage;
    if(dataGridRef.value) dataGridRef.value.instance.pageIndex(newPage - 1); 
  }
};

const onPageSizeChange = (event) => {
  currentPage.value = 1;
  if (event && event.target) event.target.blur();
  if(dataGridRef.value) dataGridRef.value.instance.pageSize(pageSize.value);
};


// ==========================================
// 5. HÀM EXPOSE CHO COMPONENT CHA GỌI
// ==========================================
const loadData = () => {
  if (dataGridRef.value && dataGridRef.value.instance) {
    dataGridRef.value.instance.refresh(); 
  }
};
defineExpose({ loadData });


// ==========================================
// 6. FORMAT DỮ LIỆU TỪ SỐ SANG CHỮ
// ==========================================
const formatType = (c) => c.value === 1 ? 'Lương' : (c.value === 2 ? 'Phụ cấp' : (c.value === 3 ? 'Phúc lợi' : c.valueText));
const formatNature = (c) => c.value === 1 ? 'Thu nhập' : (c.value === 2 ? 'Khấu trừ' : (c.value === 3 ? 'Khác' : c.valueText));
const formatValueType = (c) => c.value === 1 ? 'Tiền tệ' : (c.value === 2 ? 'Phần trăm' : (c.value === 3 ? 'Hệ số' : c.valueText));
const formatSource = (c) => c.value === 1 ? 'Hệ thống' : (c.value === 2 ? 'Tự thêm' : c.valueText);
const getUnitName = (id) => id === '11111111-1111-1111-1111-111111111111' ? 'Công ty Tổng Test' : 'Tất cả đơn vị'; 
</script>

<style scoped>
/* ==========================================
   1. LAYOUT CHUNG & GRID WRAPPER
   ========================================== */
.grid-wrapper {
  height: 100%;
  width: 100%;
  overflow: hidden;
  display: flex;
  flex-direction: column;
  position: relative; /* Rất quan trọng để Empty state nằm đúng chỗ */
}

:deep(.dx-datagrid) {
  flex: 1;
  min-height: 0;
  font-size: 13px; 
}

/* ==========================================
   2. TÙY CHỈNH THANH CUỘN
   ========================================== */
:deep(.dx-scrollbar-vertical),
:deep(.dx-scrollbar-vertical .dx-scrollable-scroll) {
  width: 8px !important;
  border-radius: 8px !important;
}

:deep(.dx-scrollbar-horizontal),
:deep(.dx-scrollbar-horizontal .dx-scrollable-scroll) {
  height: 8px !important;
  border-radius: 8px !important;
}

/* ==========================================
   3. THANH CÔNG CỤ (TOOLBAR)
   ========================================== */
:deep(.dx-toolbar) {
  padding: 8px 16px;
  margin-bottom: 8px; 
}

.toolbar-search { 
  display: flex; align-items: center; border: 1px solid var(--border-color); 
  border-radius: 4px; padding: 4px 8px; width: 250px; 
  position: relative; 
}
.toolbar-search input { border: none; outline: none; margin-left: 8px; width: 100%; }
.toolbar-search:hover { border-color: #00ab6b; }

/* CSS Mới Cho Menu Gợi Ý Search */
.search-suggestions {
  position: absolute;
  top: 100%;
  left: 0;
  width: 100%; 
  min-width: 250px;
  background: #ffffff;
  border: 1px solid #e0e0e0;
  border-radius: 4px;
  box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15);
  list-style: none;
  padding: 4px 0;
  margin: 4px 0 0 0;
  z-index: 1000;
  max-height: 250px;
  overflow-y: auto;
}

.search-suggestions li {
  padding: 8px 12px;
  cursor: pointer;
  display: flex;
  align-items: center;
  font-size: 13px;
  transition: background-color 0.15s ease;
}

.search-suggestions li:hover {
  background: #f4f5f8;
}

.search-suggestions li:hover .suggest-name,
.search-suggestions li:hover .suggest-dash,
.search-suggestions li:hover .suggest-code {
  color: #00ab6b;
}

.suggest-name { font-weight: 500; color: #111111; }
.suggest-dash { margin: 0 4px; color: #666666; }
.suggest-code { color: #666666; }

/* CSS Cho Màn Hình Trống (Empty State) */
.misa-empty-state {
  position: absolute;
  top: 50px; 
  left: 0;
  right: 0;  
  bottom: 0; 
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  
  background-color: transparent; 
  

  pointer-events: none; 
  
  z-index: 5;
}



.misa-empty-state span {
  color: #666666;
  font-size: 13px;
  font-weight: 550;
  font-family: inherit;
  margin-top: 16px;
  pointer-events: auto; 
  user-select: text;
}

:deep(.dx-datagrid-nodata) {
  display: none !important; 
}


.toolbar-dropdown { 
  display: flex; align-items: center; justify-content: space-between; 
  height: 28px; padding: 0 12px; border-radius: 4px; cursor: pointer; transition: all 0.2s ease; 
}
.dropdown-status { min-width: 160px; background-color: transparent; border: 1px solid #e0e0e0; }
.dropdown-status:hover { background-color: #f4f5f8; }

.dropdown-unit { min-width: 200px; background-color: #ffffff; border: 1px solid #e0e0e0; }
.dropdown-unit:hover { border-color: #00ab6b; }

.icon-btn { 
  background: none; border: 1px solid #e0e0e0; border-radius: 4px; 
  font-size: 18px; cursor: pointer; color: var(--text-secondary); margin-left: 2px; 
  display: flex; align-items: center; justify-content: center; width: 28px; height: 28px;
}
.icon-btn:hover { background-color: #f4f5f8; color: var(--text-primary); }

/* ==========================================
   3.1. THANH THAO TÁC HÀNG LOẠT 
   ========================================== */
.batch-info {
  display: flex; align-items: center; gap: 12px; font-size: 13px;
}
.btn-deselect {
  color: #00ab6b; cursor: pointer; font-weight: 500; 
}

.batch-buttons {
  display: flex; align-items: center; gap: 12px; margin-left: 20px;
}
.batch-btn {
  display: flex; align-items: center; gap: 6px; height: 32px; padding: 0 16px; 
  border-radius: 4px; background: #ffffff; font-family: inherit; font-size: 13px; 
  font-weight: 500; cursor: pointer; transition: all 0.2s;
}

.btn-batch-warning { border: 1px solid #f39c12; color: #f39c12; }
.btn-batch-warning:hover { background-color: #fdf2e9; }

.btn-batch-tracking { border: 1px solid #00ab6b; color: #00ab6b; }
.btn-batch-tracking:hover { background-color: #e5f6ed; }

.btn-batch-danger { border: 1px solid #e74c3c; color: #e74c3c; }
.btn-batch-danger:hover { background-color: #fdf0ef; }

/* ==========================================
   4. CỘT TIÊU ĐỀ & ĐƯỜNG KẺ BẢNG 
   ========================================== */
:deep(.dx-datagrid-headers) { 
  background-color: #f4f5f8 !important; color: var(--text-color); font-weight: 600; font-size: 14px; 
}
:deep(.dx-datagrid-borders > .dx-datagrid-headers) { 
  border-bottom: 1px solid var(--border-color); 
}
:deep(.dx-datagrid-headers .dx-header-row > td) { 
  padding: 6px 8px !important; font-size: 13px; font-weight: 600; 
  background-color: #f4f5f8 !important; color: #111111; 
  border-bottom: 1px solid #e0e0e0 !important; border-left: none !important; border-right: none !important; 
}

:deep(.dx-datagrid-headers .dx-header-row > td:not(:last-child)::after) {
  content: ""; position: absolute; right: 0; top: 20%; 
  height: 60%; width: 1px; background-color: #c0c0c0; 
}

:deep(.dx-datagrid .dx-row > td) {
  border-left: none !important; border-right: none !important; position: relative; 
}
:deep(.dx-datagrid .dx-row > td.dx-command-select::after) {
  display: none !important; 
}

/* ==========================================
   5. TRẠNG THÁI HOVER & SELECTED DÒNG
   ========================================== */
:deep(.dx-datagrid-rowsview .dx-row > td) {
  border-bottom: 1px solid #e0e0e0 !important; border-top: none !important; 
}
:deep(.dx-datagrid-rowsview) {
  border-bottom: 1px solid #e0e0e0;
}

:deep(.dx-datagrid-rowsview .dx-data-row:hover > td),
:deep(.dx-datagrid-rowsview .dx-data-row.dx-state-hover > td) {
  background-color: #c1efd7 !important; color: #111111; cursor: pointer; 
}

:deep(.dx-datagrid-rowsview .dx-selection > td) {
  background-color: #c1efd7 !important; color: #111111; 
}

/* ==========================================
   6. CỘT CHỨC NĂNG KHI HOVER
   ========================================== */
.row-actions {
  display: none; 
  position: absolute; 
  right: 12px; 
  top: 50%;
  transform: translateY(-50%); 
  align-items: center;
  gap: 8px;
  background-color: inherit; 
  padding-left: 16px; 
  box-shadow: -15px 0 15px -10px rgba(0, 0, 0, 0.08); 
}
:deep(.dx-datagrid-rowsview .dx-data-row:hover .row-actions){
  display: flex;
}
.action-btn {
  width: 28px; height: 28px; border-radius: 4px; background-color: #ffffff; 
  border: 1px solid #e0e0e0; display: flex; align-items: center; justify-content: center; 
  cursor: pointer; transition: all 0.2s ease;
}
.action-btn:hover { background-color: #f4f5f8; }
.btn-warning { color: #f39c12; }
.btn-warning:hover { border-color: #f39c12; }
.btn-normal { color: #666666; }
.btn-normal:hover { border-color: #00ab6b; color: #00ab6b; }
.btn-danger { color: #e74c3c; }
.btn-danger:hover { border-color: #e74c3c; }

:deep(.dx-datagrid .dx-row > td.column-actions) {
  position: sticky !important;
  right: 0 !important; 
  z-index: 10;
  overflow: visible !important; 
  border-left: none !important;
  background-color: inherit !important; 
}
:deep(.dx-datagrid-headers .dx-header-row > td.column-actions) {
  position: sticky !important;
  right: 0 !important;
  background-color: #f4f5f8 !important;
  z-index: 11;
}

/* ==========================================
   7. TÙY CHỈNH Ô CHECKBOX
   ========================================== */
:deep(.dx-checkbox-checked .dx-checkbox-icon) {
  background-color: #00ab6b !important; border-color: #00ab6b !important; color: #ffffff !important; 
}
:deep(.dx-checkbox-indeterminate .dx-checkbox-icon) {
  background-color: #00ab6b !important; border-color: #00ab6b !important;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23ffffff' stroke-width='4' stroke-linecap='round' stroke-linejoin='round'%3E%3Cline x1='5' y1='12' x2='19' y2='12'%3E%3C/line%3E%3C/svg%3E") !important;
  background-size: 12px; background-position: center; background-repeat: no-repeat;
}
:deep(.dx-checkbox-indeterminate .dx-checkbox-icon::before) {
  display: none !important;
}

/* ==========================================
   8. CỘT TRẠNG THÁI 
   ========================================== */
.status-badge { display: flex; align-items: center; gap: 6px; }
.status-tracking { padding: 2px 8px; border: 1px solid #00ab6b; border-radius: 8px; background-color: #e5f6ed; color: #00ab6b; font-weight: 500; }
.status-stopped { padding: -1px 8px; border: 1px solid #f39c12; border-radius: 8px; background-color: #fdf2e9; color: #f39c12; font-weight: 500; }
.status-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.status-tracking .status-dot { background-color: #00ab6b;  }
.status-stopped .status-dot { background-color: #f39c12; }

/* ==========================================
   9. THANH PHÂN TRANG CUSTOM
   ========================================== */
.misa-pagination {
  display: flex; justify-content: space-between; align-items: center; 
  height: 46px; padding: 0 16px; background-color: #ffffff; 
  border-top: 1px solid #e0e0e0; font-size: 13px; color: #111111; margin-top: auto; 
}
.pagination-right { display: flex; align-items: center; gap: 16px; }

.page-size-selector select {
  height: 28px; padding: 0 24px 0 8px; border: 1px solid #e0e0e0; border-radius: 4px; 
  outline: none; cursor: pointer; font-family: inherit; font-size: 13px; appearance: none;
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23666666' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E");
  background-repeat: no-repeat; background-position: right 4px center; background-size: 14px;
}
.page-size-selector select:focus {
  border-color: #00ab6b; 
  background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23666666' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 15 12 9 18 15'%3E%3C/polyline%3E%3C/svg%3E");
}
.page-size-selector select:hover { border-color: #00ab6b; }
.page-range { min-width: 45px; text-align: center; }

.page-navigation { display: flex; align-items: center; gap: 4px; }
.nav-btn {
  display: flex; justify-content: center; align-items: center; width: 24px; height: 24px; 
  background: none; border: none; border-radius: 4px; cursor: pointer; color: #666666; transition: all 0.2s;
}
/* ==========================================
   TÙY CHỈNH MÀU NỀN KHI BÔI ĐEN VĂN BẢN
   ========================================== */
   :deep(.dx-datagrid) *,
:deep(.dx-datagrid-content) * {
  -webkit-user-select: text !important; 
  -moz-user-select: text !important;    
  user-select: text !important;         
}
::selection {
  background-color: #00ab6b !important; 
  color: #ffffff !important;            
}

::-moz-selection {
  background-color: #00ab6b !important;
  color: #ffffff !important;
}

:deep(::selection) {
  background-color: #00ab6b !important;
  color: #ffffff !important;
}

:deep(::-moz-selection) {
  background-color: #00ab6b !important;
  color: #ffffff !important;
}
.nav-btn:not(:disabled):hover { background-color: #f4f5f8; color: #111111; }
.nav-btn:disabled { color: #cccccc; cursor: not-allowed; }
</style>