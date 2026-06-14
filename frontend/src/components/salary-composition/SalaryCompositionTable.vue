<template>
  <div class="page-wrapper">
    
    <div class="layout-container" ref="layoutContainerRef">
      
      <div class="toolbar-layer" :class="{ 'is-open': isFilterPanelOpen }">
        <div class="toolbar-left">
          <div class="toolbar-search">
            <i class="misa-icon mi-search"></i>
            <input v-model="searchText" type="text" placeholder="Tìm kiếm" @input="handleSearchInput" @keydown.enter="applySearch" @focus="handleSearchInput" />
            <ul v-if="isSearchDropdownOpen && searchSuggestions.length > 0" class="search-suggestions">
              <li v-for="item in searchSuggestions" :key="item.compositionId" @click="selectSuggestion(item)">
                <span class="suggest-name">{{ item.compositionName }}</span>
                <span class="suggest-dash"> - </span>
                <span class="suggest-code">{{ item.compositionCode }}</span>
              </li>
            </ul>
          </div>

          <template v-if="selectedRowKeys.length === 0">
            <div class="toolbar-dropdown dropdown-status" @click="toggleStatusDropdown">
              <span><span class="label-text">Trạng thái:</span> <strong>{{ currentStatusLabel }}</strong></span>
              <i class="misa-icon mi-chevron-down"></i>
              <ul v-if="isStatusDropdownOpen" class="custom-filter-list">
                <li :class="{ active: selectedStatusFilter === 0 }" @click.stop="selectStatusFilter(0, 'Tất cả')"><i class="misa-icon mi-check" v-if="selectedStatusFilter === 0"></i> Tất cả</li>
                <li :class="{ active: selectedStatusFilter === 1 }" @click.stop="selectStatusFilter(1, 'Đang theo dõi')"><i class="misa-icon mi-check" v-if="selectedStatusFilter === 1"></i> Đang theo dõi</li>
                <li :class="{ active: selectedStatusFilter === 2 }" @click.stop="selectStatusFilter(2, 'Ngừng theo dõi')"><i class="misa-icon mi-check" v-if="selectedStatusFilter === 2"></i> Ngừng theo dõi</li>
              </ul>
            </div>
            <div class="toolbar-dropdown dropdown-unit">
              <span><span class="label-text">Tất cả đơn vị</span></span>
              <i class="misa-icon mi-chevron-down"></i>
            </div>
          </template>

          <template v-if="selectedRowKeys.length > 0">
            <div class="batch-info">
              <span>Đã chọn <strong>{{ selectedRowKeys.length }}</strong></span>
              <span class="btn-deselect" @click="deselectAll">Bỏ chọn</span>
            </div>
            <div class="batch-buttons">
              <button v-if="hasTrackingStatus" class="batch-btn btn-batch-warning" @click="handleUpdateStatusBulk(2)"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="8" y1="12" x2="16" y2="12"></line></svg> Ngừng theo dõi</button>
              <button v-if="hasStoppedStatus" class="batch-btn btn-batch-tracking" @click="handleUpdateStatusBulk(1)"><i class="misa-icon mi-circle-check-green"></i> Đang theo dõi</button>
              <button class="batch-btn btn-batch-danger" @click="handleDeleteBulk"><i class="misa-icon mi-trash-red"></i> Xóa</button>
            </div>
          </template>
        </div>

        <div class="toolbar-right" v-if="selectedRowKeys.length === 0">
          <button class="icon-btn" :class="{ 'is-filtered': appliedFilters.length > 0 }" title="Bộ lọc" @click="toggleFilterPanel" draggable="false">
            <i v-if="appliedFilters.length === 0" class="misa-icon mi-filter"></i>
            <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="#00ab6b" xmlns="http://www.w3.org/2000/svg">
              <path d="M4 3H20C20.5523 3 21 3.44772 21 4V6.58579C21 6.851 20.8946 7.10536 20.7071 7.29289L14 14V21C14 21.5523 13.5523 22 13 22H11C10.4477 22 10 21.5523 10 21V14L3.29289 7.29289C3.10536 7.10536 3 6.851 3 6.58579V4C3 3.44772 3.44772 3 4 3Z"/>
            </svg>
          </button>
          
          <button class="icon-btn" title="Cài đặt" draggable="false">
            <i class="misa-icon mi-setting"></i>
          </button>
        </div>
      </div>

      <div class="grid-layer" :class="{ 'is-resizing': isResizing }" :style="gridLayerStyle">
        
        <div class="applied-filters-container" v-if="appliedFilters.length > 0">
          <div class="filter-tag" v-for="(filter, index) in appliedFilters" :key="index">
            <span class="tag-label">{{ filter.label }}</span>
            <span class="tag-operator">{{ getOperatorLabel(filter.operator) }}</span>
            <span class="tag-value" v-if="!['isnull', 'isnotnull'].includes(filter.operator)">
                {{ (filter.value && filter.value.trim() !== '') ? filter.value : '"Rỗng"' }}
            </span>
            <span @click="removeAppliedFilter(index, filter.id)" class="tag-close">
              <svg width="14" height="14" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
                <path d="M18 6L6 18M6 6L18 18" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"/>
              </svg>
            </span>
          </div>
        </div>

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
          :load-panel="{ enabled: false }"
        >
          <DxSelection mode="multiple" show-check-boxes-mode="always" />

          <DxColumn data-field="compositionCode" caption="Mã thành phần" width="130" />
          <DxColumn data-field="compositionName" caption="Tên thành phần" min-width="200" /> 
          <DxColumn data-field="organizationId" caption="Đơn vị áp dụng" width="180" cell-template="unitTemplate" />
          <template #unitTemplate="{ data }"><span>{{ getUnitName(data.value) }}</span></template>
          <DxColumn data-field="compositionType" caption="Loại thành phần" width="150" :customize-text="formatType" />
          <DxColumn data-field="compositionNature" caption="Tính chất" width="120" :customize-text="formatNature" />
          <DxColumn data-field="valueType" caption="Kiểu giá trị" width="120" :customize-text="formatValueType" />
          <DxColumn data-field="amount" caption="Giá trị" width="120" />
          <DxColumn data-field="sourceType" caption="Nguồn tạo" width="120" :customize-text="formatSource" />
          
          <DxColumn data-field="status" caption="Trạng thái" width="130" cell-template="statusTemplate" />
          <template #statusTemplate="{ data }">
            <div v-if="data.value === 1" class="status-badge status-tracking"><div class="status-dot"></div><span>Đang theo dõi</span></div>
            <div v-else-if="data.value === 2" class="status-badge status-stopped"><div class="status-dot"></div><span>Ngừng theo dõi</span></div>
          </template>

          <DxColumn cell-template="actionTemplate" css-class="column-actions" :width="60" />
          <template #actionTemplate="{ data }">
            <div class="row-actions">
              <button v-if="data.data.status === 1 || data.data.Status === 1" class="action-btn btn-warning" title="Ngừng theo dõi" @click="handleUpdateStatusSingle(data.key, 2, data.data)"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"></circle><line x1="8" y1="12" x2="16" y2="12"></line></svg></button>
              <button v-if="data.data.status === 2 || data.data.Status === 2" class="action-btn" style="color: #00ab6b;" title="Đang theo dõi" @click="handleUpdateStatusSingle(data.key, 1, data.data)"><i class="misa-icon mi-circle-check-green"></i></button>
              <button class="action-btn btn-normal" title="Nhân bản"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><rect x="9" y="9" width="13" height="13" rx="2" ry="2"></rect><path d="M5 15H4a2 2 0 0 1-2-2V4a2 2 0 0 1 2-2h9a2 2 0 0 1 2 2v1"></path></svg></button>
              <button class="action-btn btn-normal" title="Sửa"><i class="misa-icon mi-pencil"></i></button>
              <button class="action-btn btn-danger" title="Xóa" @click="handleDeleteSingle(data.data)"><i class="misa-icon mi-trash-red"></i></button>
            </div>
          </template>

          <DxPaging :page-size="pageSize" />
          <DxPager :visible="false" />
        </DxDataGrid>

        <div class="misa-empty-state" v-if="totalRecords === 0 && !isLoading"><i class="ms-table__icon_nodata"></i><span>Không có dữ liệu</span></div>

        <div class="misa-pagination">
          <div class="pagination-left">Tổng số: <strong>{{ totalRecords }}</strong></div>
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
            <span class="page-range"><strong>{{ startRecord }} - {{ endRecord }}</strong></span>
            <div class="page-navigation">
              <button class="nav-btn" :disabled="currentPage === 1" @click="changePage(1)"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M19 12H5M5 12L12 19M5 12L12 5M2 5V19"/></svg></button>
              <button class="nav-btn" :disabled="currentPage === 1" @click="changePage(currentPage - 1)"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M15 18l-6-6 6-6"/></svg></button>
              <button class="nav-btn" :disabled="currentPage === totalPages || totalRecords === 0" @click="changePage(currentPage + 1)"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M9 18l6-6-6-6"/></svg></button>
              <button class="nav-btn" :disabled="currentPage === totalPages || totalRecords === 0" @click="changePage(totalPages)"><svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M5 12H19M19 12L12 5M19 12L12 19M22 5V19"/></svg></button>
            </div>
          </div>
        </div>
      </div>

      <Transition name="fade">
        <div class="fake-gap" v-if="isFilterPanelOpen" :style="{ right: filterWidth + 'px' }" :class="{ 'is-resizing': isResizing }"></div>
      </Transition>

      <Transition name="slide-right">
        <div v-if="isFilterPanelOpen" class="filter-layer" :style="{ width: filterWidth + 'px' }" :class="{ 'is-resizing': isResizing }">
          
          <div class="resize-handle" @mousedown="startResize">
            <div class="resize-icon"></div>
          </div>

          <div class="filter-header">
            <h3 class="filter-title">Bộ lọc</h3>
            <button class="btn-close-filter" @click="toggleFilterPanel" aria-label="Đóng">
              <svg width="24" height="24" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M18 6L6 18M6 6L18 18" stroke="#666666" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
            </button>
          </div>

          <div class="filter-body">
            <div class="filter-search-box">
              <i class="misa-icon mi-search"></i>
              <input type="text" placeholder="Tìm kiếm" v-model="filterSearchText" />
            </div>

            <ul class="filter-list">
              <li v-for="field in filteredFields" :key="field.id" class="filter-item" :class="{ 'is-active': field.isActive }">
                
                <div class="filter-item-header" @click="toggleFilterField(field)">
                  <div class="misa-checkbox" :class="{ 'checked': field.isActive }">
                    <svg v-if="field.isActive" width="12" height="12" viewBox="0 0 12 12" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M2.5 6L5 8.5L9.5 3.5" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
                  </div>
                  <span class="filter-label">{{ field.label }}</span>
                </div>

                <div class="filter-item-content" v-if="field.isActive">
                  <div class="custom-dropdown-wrapper">
                    <div class="misa-filter-select" @click.stop="toggleOperatorDropdown(field)">
                      <span>{{ getOperatorLabel(field.operator) }}</span>
                      <svg class="chevron-icon" :class="{ 'is-rotated': field.isOperatorOpen }" width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M6 9L12 15L18 9" stroke="#666666" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
                    </div>
                    <ul v-if="field.isOperatorOpen" class="custom-operator-list">
                      <li @click.stop="selectOperator(field, 'contains')" :class="{ active: field.operator === 'contains' }">Chứa</li>
                      <li @click.stop="selectOperator(field, 'notcontains')" :class="{ active: field.operator === 'notcontains' }">Không chứa</li>
                      <li @click.stop="selectOperator(field, 'equals')" :class="{ active: field.operator === 'equals' }">Bằng</li>
                      <li @click.stop="selectOperator(field, 'startswith')" :class="{ active: field.operator === 'startswith' }">Bắt đầu bằng</li>
                      <li @click.stop="selectOperator(field, 'endswith')" :class="{ active: field.operator === 'endswith' }">Kết thúc bằng</li>
                      <li @click.stop="selectOperator(field, 'isnull')" :class="{ active: field.operator === 'isnull' }">Trống</li>
                      <li @click.stop="selectOperator(field, 'isnotnull')" :class="{ active: field.operator === 'isnotnull' }">Không trống</li>
                    </ul>
                  </div>

                  <input v-if="!['isnull', 'isnotnull'].includes(field.operator)" type="text" v-model="field.value" class="misa-filter-input" />
                </div>

              </li>
            </ul>
          </div>

          <div class="filter-footer">
            <button class="btn-secondary" @click="clearAllFilters">Bỏ lọc</button>
            <button class="btn-primary" @click="applyFilter">Áp dụng</button>
          </div>
        </div>
      </Transition>

    </div> 

    <div v-if="isShowDeleteConfirm" class="misa-modal-overlay">
      <div class="misa-modal-confirm" :style="{ transform: `translate(${popupPosition.x}px, ${popupPosition.y}px)` }">
        <div class="modal-header" @mousedown="startDragModal">
          <h3 class="modal-title">Thông báo</h3>
          <button class="btn-close-modal" @click="cancelDelete" aria-label="Đóng">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M18 6L6 18M6 6L18 18" stroke="#666666" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </button>
        </div>
        <div class="modal-body confirm-body">
          <div class="confirm-message">
            <span v-if="!isBulkAction">Bạn có chắc chắn muốn xóa thành phần lương <strong>{{ itemToDeleteName }}</strong> không?</span>
            <span v-else>Bạn có chắc chắn muốn xóa các thành phần lương đã chọn không?</span>
          </div>
        </div>
        <div class="modal-footer confirm-footer">
          <button class="btn-secondary" @click="cancelDelete">Hủy</button>
          <button class="btn-primary btn-danger" @click="confirmDelete">Xóa</button>
        </div>
      </div>
    </div>

    <div v-if="isShowStatusConfirm" class="misa-modal-overlay">
      <div class="misa-modal-confirm" :style="{ transform: `translate(${popupPosition.x}px, ${popupPosition.y}px)` }">
        <div class="modal-header" @mousedown="startDragModal">
          <h3 class="modal-title">Chuyển trạng thái</h3>
          <button class="btn-close-modal" @click="cancelChangeStatus" aria-label="Đóng">
            <svg width="18" height="18" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M18 6L6 18M6 6L18 18" stroke="#666666" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
          </button>
        </div>
        <div class="modal-body confirm-body">
          <div class="confirm-message">
            <span v-if="!isBulkAction">Bạn có chắc chắn muốn chuyển trạng thái thành phần lương <strong>{{ itemToChangeStatusName }}</strong> sang {{ targetStatusText }} không?</span>
            <span v-else>Bạn có chắc chắn muốn chuyển trạng thái các thành phần lương đã chọn sang {{ targetStatusText }} không?</span>
          </div>
        </div>
        <div class="modal-footer confirm-footer">
          <button class="btn-secondary" @click="cancelChangeStatus">Hủy</button>
          <button class="btn-primary" @click="confirmChangeStatus">Đồng ý</button>
        </div>
      </div>
    </div>

    <Transition name="toast-slide">
      <div v-if="isShowToast" class="misa-toast-success">
        <div class="toast-left">
          <svg class="toast-icon" width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><circle cx="12" cy="12" r="10" stroke="white" stroke-width="2"/><path d="M8 12.5L11 15.5L16 9" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
          <span class="toast-text">{{ toastMessage }}</span>
        </div>
        <button class="toast-close" @click="closeToast" aria-label="Đóng thông báo">
          <svg width="16" height="16" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg"><path d="M18 6L6 18M6 6L18 18" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"/></svg>
        </button>
      </div>
    </Transition>

  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue';
import axios from 'axios';
import CustomStore from 'devextreme/data/custom_store';
import notify from 'devextreme/ui/notify';
import { globalLoading } from '../../utils/loading.js'; 
import { DxDataGrid, DxColumn, DxSelection, DxPaging, DxPager } from 'devextreme-vue/data-grid';

const API_URL = 'http://localhost:5094/api/v1/SalaryCompositions';

const dataGridRef = ref(null);
const totalRecords = ref(0); 
const isLoading = ref(false); 
const currentPage = ref(1);
const pageSize = ref(15); 

// Biến lưu trữ các thẻ (Tags) bộ lọc đang được áp dụng
const appliedFilters = ref([]); 

const totalPages = computed(() => Math.ceil(totalRecords.value / pageSize.value));
const startRecord = computed(() => totalRecords.value === 0 ? 0 : (currentPage.value - 1) * pageSize.value + 1);
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
// KẾT NỐI API 
// ==========================================
const dataSource = new CustomStore({
  key: 'compositionId', 
  load: async (loadOptions) => {
    isLoading.value = true; 
    try {
      const params = { 
        skip: loadOptions.skip || 0, 
        take: loadOptions.take || pageSize.value, 
        searchValue: searchText.value.trim() 
      };
      
      if (selectedStatusFilter.value !== 0) {
        params.status = selectedStatusFilter.value;
      }
      
      // ==========================================
      // XỬ LÝ ĐẨY BỘ LỌC NÂNG CAO XUỐNG BACKEND
      // ==========================================
      if (appliedFilters.value.length > 0) {
        
        // CÁCH 1: TRUYỀN THÀNH QUERY PARAMS (Cách phổ biến nhất cho API REST cơ bản)
        // Kết quả gửi đi sẽ có dạng: ?compositionCode=abc&compositionNature=Thu nhập
        appliedFilters.value.forEach(f => {
          if (f.value && f.value.trim() !== '') {
            params[f.id] = f.value.trim();
          }
        });

        // -------------------------------------------------------------
        // CÁCH 2: NẾU BACKEND CỦA BẠN DÙNG THƯ VIỆN CHUẨN DEVEMENTREME (C# DataSourceLoader)
        // (Hãy mở comment đoạn code dưới đây và xóa Cách 1 đi)
        /*
        let dxFilters = [];
        appliedFilters.value.forEach((f, index) => {
          if (index > 0) dxFilters.push("and");
          dxFilters.push([f.id, f.operator, f.value]);
        });
        params.filter = JSON.stringify(dxFilters);
        */
        // -------------------------------------------------------------
        // CÁCH 3: NẾU BACKEND YÊU CẦU MỘT CHUỖI JSON CUSTOM CỦA RIÊNG BẠN
        /*
        const customFilters = appliedFilters.value.map(f => ({
          FieldName: f.id,
          Operator: f.operator,
          Value: f.value
        }));
        params.advancedFilters = JSON.stringify(customFilters);
        */
      }

      // CHÚ Ý: Nếu Backend của bạn bắt buộc dùng method POST để lọc, hãy sửa dòng dưới thành axios.post(...)
      const response = await axios.get(API_URL, { params });
      
      const dataArray = response.data.data; 
      if (!Array.isArray(dataArray)) return { data: [], totalCount: 0 };
      
      const total = response.data.totalRecord || response.data.TotalRecord || dataArray.length;
      totalRecords.value = total;
      
      return { data: dataArray, totalCount: total };
    } catch (error) {
      console.error("Lỗi:", error); 
      throw 'Data Loading Error';
    } finally { 
      isLoading.value = false; 
    }
  },
  remove: async (key) => { 
    try { await axios.delete(`${API_URL}/${key}`); } 
    catch (error) { throw 'Delete Error'; } 
  }
});

const searchText = ref('');
const searchSuggestions = ref([]);
const isSearchDropdownOpen = ref(false);
let searchTimer = null;

const isStatusDropdownOpen = ref(false);
const selectedStatusFilter = ref(0); 
const currentStatusLabel = ref('Tất cả');

const handleSearchInput = () => {
  clearTimeout(searchTimer);
  searchTimer = setTimeout(async () => {
    const keyword = searchText.value.trim();
    if (keyword.length > 0) {
      try {
        const params = { skip: 0, take: 5, searchValue: keyword };
        if (selectedStatusFilter.value !== 0) params.status = selectedStatusFilter.value;
        const response = await axios.get(API_URL, { params });
        searchSuggestions.value = response.data.data || [];
        isSearchDropdownOpen.value = true;
      } catch (error) {}
    } else { searchSuggestions.value = []; isSearchDropdownOpen.value = false; applySearch(); }
  }, 300); 
};

const selectSuggestion = (item) => { searchText.value = item.compositionName; isSearchDropdownOpen.value = false; applySearch(); };
const applySearch = () => { currentPage.value = 1; isSearchDropdownOpen.value = false; if (dataGridRef.value) { dataGridRef.value.instance.pageIndex(0); dataGridRef.value.instance.refresh(); } };
const toggleStatusDropdown = (e) => { e.stopPropagation(); isStatusDropdownOpen.value = !isStatusDropdownOpen.value; isSearchDropdownOpen.value = false; };
const selectStatusFilter = (statusValue, statusLabel) => { selectedStatusFilter.value = statusValue; currentStatusLabel.value = statusLabel; isStatusDropdownOpen.value = false; applySearch(); };
const closeDropdowns = (event) => { 
  if (!event.target.closest('.toolbar-search')) isSearchDropdownOpen.value = false; 
  if (!event.target.closest('.dropdown-status')) isStatusDropdownOpen.value = false; 
  
  if (!event.target.closest('.custom-dropdown-wrapper')) {
    filterFields.value.forEach(f => f.isOperatorOpen = false);
  }
};
// ==========================================
// LOGIC BỘ LỌC NÂNG CAO & KÉO THẢ 
// ==========================================
const isFilterPanelOpen = ref(false);
const filterSearchText = ref('');
const layoutContainerRef = ref(null);
const gridWrapperRef = ref(null);
const filterWidth = ref(280); 
const isResizing = ref(false); 

// Tính toán khoảng cách cạnh phải cho Bảng 
const gridLayerStyle = computed(() => {
  if (isFilterPanelOpen.value) {
    return { right: (filterWidth.value + 12) + 'px' };
  }
  return { right: '0px' };
});

const filterFields = ref([
  { id: 'compositionCode', label: 'Mã thành phần', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'compositionName', label: 'Tên thành phần', isActive: false, operator: 'contains', value: '' ,isOperatorOpen: false},
  { id: 'compositionType', label: 'Loại thành phần', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'compositionNature', label: 'Tính chất', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'taxNature', label: 'Chịu thuế', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'taxDeduction', label: 'Giảm trừ khi tính thuế', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'normFormula', label: 'Định mức', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'valueType', label: 'Kiểu giá trị', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'amount', label: 'Giá trị', isActive: false, operator: 'equals', value: '',isOperatorOpen: false },
  { id: 'description', label: 'Mô tả', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'sourceType', label: 'Nguồn tạo', isActive: false, operator: 'contains', value: '',isOperatorOpen: false },
  { id: 'isDisplayOnPayslip', label: 'Hiển thị trên phiếu lương', isActive: false, operator: 'equals', value: '',isOperatorOpen: false },
]);

const filteredFields = computed(() => {
  if (!filterSearchText.value) return filterFields.value;
  const keyword = filterSearchText.value.toLowerCase();
  return filterFields.value.filter(f => f.label.toLowerCase().includes(keyword));
});

const toggleFilterPanel = () => { isFilterPanelOpen.value = !isFilterPanelOpen.value; };

const toggleFilterField = (field) => { 
  field.isActive = !field.isActive; 
  if (!field.isActive) { 
    field.operator = 'contains'; 
    field.value = ''; 
  } 
};

// Dịch Operator sang Tiếng Việt cho thẻ Tag hiển thị
const getOperatorLabel = (val) => {
  const map = {
    'contains': 'Chứa', 'notcontains': 'Không chứa', 'equals': 'Bằng',
    'startswith': 'Bắt đầu bằng', 'endswith': 'Kết thúc bằng',
    'isnull': 'Trống', 'isnotnull': 'Không trống'
  };
  return map[val] || 'Chứa';
};
const toggleOperatorDropdown = (field) => {
  filterFields.value.forEach(f => { if (f.id !== field.id) f.isOperatorOpen = false; });
  field.isOperatorOpen = !field.isOperatorOpen;
};
const selectOperator = (field, val) => {
  field.operator = val; 
  field.isOperatorOpen = false;
};
// Xử lý khi nhấn nút "Áp dụng"
const applyFilter = () => { 
  appliedFilters.value = filterFields.value.filter(f => f.isActive).map(f => ({...f}));
  isFilterPanelOpen.value = false; 
  if (dataGridRef.value) {
    dataGridRef.value.instance.pageIndex(0);
    dataGridRef.value.instance.refresh(); 
  }
};

// Xóa 1 thẻ Tag đang áp dụng
const removeAppliedFilter = (index, fieldId) => {
  appliedFilters.value.splice(index, 1);
  const field = filterFields.value.find(f => f.id === fieldId);
  if (field) { field.isActive = false; field.value = ''; field.operator = 'contains'; }
  if (dataGridRef.value) dataGridRef.value.instance.refresh();
};

// Xóa toàn bộ các thẻ Tag
const clearAllFilters = () => { 
  appliedFilters.value = [];
  filterFields.value.forEach(f => { f.isActive = false; f.operator = 'contains'; f.value = ''; }); 
  filterSearchText.value = ''; 
  if (dataGridRef.value) dataGridRef.value.instance.refresh(); 
};

// --- Logic Kéo Thu Phóng ---
const startResize = (e) => {
  e.preventDefault(); isResizing.value = true;
  document.body.style.cursor = 'col-resize'; document.body.style.userSelect = 'none';
  document.addEventListener('mousemove', onResize); document.addEventListener('mouseup', stopResize);
};
const onResize = (e) => {
  if (!isResizing.value || !layoutContainerRef.value) return;
  const containerRect = layoutContainerRef.value.getBoundingClientRect();
  let newWidth = containerRect.right - e.clientX;
  
  const minWidth = 280;
  const maxWidth = containerRect.width * 0.5;
  if (newWidth < minWidth) newWidth = minWidth;
  if (newWidth > maxWidth) newWidth = maxWidth;
  filterWidth.value = newWidth;
};
const stopResize = () => { isResizing.value = false; document.body.style.cursor = ''; document.body.style.userSelect = ''; document.removeEventListener('mousemove', onResize); document.removeEventListener('mouseup', stopResize); };

const selectedRowKeys = ref([]);
const selectedRowsData = ref([]);
const isBulkAction = ref(false);
const hasStoppedStatus = computed(() => selectedRowsData.value.some(row => row.Status === 'Ngừng sử dụng' || row.Status === 'Ngừng theo dõi' || row.status === 2));
const hasTrackingStatus = computed(() => selectedRowsData.value.some(row => row.Status === 'Đang theo dõi' || row.status === 1));
const onSelectionChanged = (e) => { selectedRowKeys.value = e.selectedRowKeys; selectedRowsData.value = e.selectedRowsData; };
const deselectAll = () => { selectedRowKeys.value = []; selectedRowsData.value = []; };

const isShowDeleteConfirm = ref(false);
const itemToDeleteName = ref('');
const itemToDeleteId = ref(null);
const handleDeleteSingle = (compositionData) => { isBulkAction.value = false; itemToDeleteName.value = compositionData.compositionName || compositionData.CompositionName || "N/A"; itemToDeleteId.value = compositionData.compositionId || compositionData.CompositionId; popupPosition.value = { x: 0, y: 0 }; isShowDeleteConfirm.value = true; };
const handleDeleteBulk = () => { isBulkAction.value = true; popupPosition.value = { x: 0, y: 0 }; isShowDeleteConfirm.value = true; };
const cancelDelete = () => { isShowDeleteConfirm.value = false; itemToDeleteId.value = null; itemToDeleteName.value = ''; };
const confirmDelete = async () => {
  globalLoading.value = true;
  try {
    if (isBulkAction.value) { await Promise.all(selectedRowKeys.value.map(id => dataSource.remove(id))); showToast(`Xóa thành công ${selectedRowKeys.value.length} bản ghi`); deselectAll(); } 
    else { if (!itemToDeleteId.value) return; await dataSource.remove(itemToDeleteId.value); showToast("Xóa thành công"); }
    if (dataGridRef.value) dataGridRef.value.instance.refresh(); 
  } catch (error) { notify("Lỗi khi xóa", "error", 3000); } finally { globalLoading.value = false; isShowDeleteConfirm.value = false; }
};

const isShowStatusConfirm = ref(false);
const itemToChangeStatusName = ref('');
const itemToChangeStatusId = ref(null);
const itemToChangeStatusNewValue = ref(null);
const itemToChangeStatusRowData = ref(null);
const targetStatusText = computed(() => itemToChangeStatusNewValue.value === 1 ? 'đang theo dõi' : 'ngừng theo dõi');
const handleUpdateStatusSingle = (compositionId, newStatus, rowData) => { isBulkAction.value = false; itemToChangeStatusName.value = rowData.compositionName || rowData.CompositionName || "N/A"; itemToChangeStatusId.value = compositionId; itemToChangeStatusNewValue.value = newStatus; itemToChangeStatusRowData.value = rowData; popupPosition.value = { x: 0, y: 0 }; isShowStatusConfirm.value = true; };
const handleUpdateStatusBulk = (newStatus) => { isBulkAction.value = true; itemToChangeStatusNewValue.value = newStatus; popupPosition.value = { x: 0, y: 0 }; isShowStatusConfirm.value = true; };
const cancelChangeStatus = () => { isShowStatusConfirm.value = false; itemToChangeStatusId.value = null; itemToChangeStatusRowData.value = null; };
const confirmChangeStatus = async () => {
  globalLoading.value = true;
  try {
    const newStatus = itemToChangeStatusNewValue.value;
    if (isBulkAction.value) { await Promise.all(selectedRowsData.value.map(row => axios.put(`${API_URL}/${row.compositionId || row.CompositionId}`, { ...row, Status: newStatus }))); showToast(`Cập nhật ${selectedRowsData.value.length} bản ghi`); deselectAll(); } 
    else { if (!itemToChangeStatusId.value) return; await axios.put(`${API_URL}/${itemToChangeStatusId.value}`, { ...itemToChangeStatusRowData.value, Status: newStatus }); showToast("Cập nhật thành công"); }
    if(dataGridRef.value) dataGridRef.value.instance.refresh();
  } catch (error) { notify("Lỗi cập nhật", "error", 3000); } finally { globalLoading.value = false; isShowStatusConfirm.value = false; }
};

const popupPosition = ref({ x: 0, y: 0 });
const isDraggingModal = ref(false);
const dragOffset = { x: 0, y: 0 };
const startDragModal = (event) => { if (event.target.closest('button')) return; isDraggingModal.value = true; document.body.style.userSelect = 'none'; dragOffset.x = event.clientX - popupPosition.value.x; dragOffset.y = event.clientY - popupPosition.value.y; document.addEventListener('mousemove', onDragModal); document.addEventListener('mouseup', stopDragModal); };
const onDragModal = (event) => { if (!isDraggingModal.value) return; popupPosition.value.x = event.clientX - dragOffset.x; popupPosition.value.y = event.clientY - dragOffset.y; };
const stopDragModal = () => { isDraggingModal.value = false; document.body.style.userSelect = ''; document.removeEventListener('mousemove', onDragModal); document.removeEventListener('mouseup', stopDragModal); };

const isShowToast = ref(false);
const toastMessage = ref('');
let toastTimer = null;
const showToast = (message) => { toastMessage.value = message; isShowToast.value = true; if (toastTimer) clearTimeout(toastTimer); toastTimer = setTimeout(() => { isShowToast.value = false; }, 3000); };
const closeToast = () => { isShowToast.value = false; if (toastTimer) clearTimeout(toastTimer); };

onMounted(() => document.addEventListener('click', closeDropdowns));
onUnmounted(() => { document.removeEventListener('click', closeDropdowns); stopResize(); stopDragModal(); });

const formatType = (c) => c.value === 1 ? 'Lương' : (c.value === 2 ? 'Phụ cấp' : (c.value === 3 ? 'Phúc lợi' : c.valueText));
const formatNature = (c) => c.value === 1 ? 'Thu nhập' : (c.value === 2 ? 'Khấu trừ' : (c.value === 3 ? 'Khác' : c.valueText));
const formatValueType = (c) => c.value === 1 ? 'Tiền tệ' : (c.value === 2 ? 'Phần trăm' : (c.value === 3 ? 'Hệ số' : c.valueText));
const formatSource = (c) => c.value === 1 ? 'Hệ thống' : (c.value === 2 ? 'Tự thêm' : c.valueText);
const getUnitName = (id) => id === '11111111-1111-1111-1111-111111111111' ? 'Công ty Tổng Test' : 'Tất cả đơn vị'; 

const loadData = () => { if (dataGridRef.value) dataGridRef.value.instance.refresh(); };
defineExpose({ loadData });
</script>

<style scoped>
/* ==========================================
   1. LAYOUT TỔNG & THANH CÔNG CỤ
   ========================================== */
.page-wrapper {
  height: 100%; width: 100%; background-color: #f4f5f8; padding: 0 16px 16px 16px; box-sizing: border-box;
  display: flex; flex-direction: column; 
}

.layout-container { display: block; flex: 1; position: relative; overflow: hidden; width: 100%; margin-top: 0;}

.toolbar-layer {
  position: absolute;
  top: 0; left: 0; right: 0; height: 48px;
  background: #ffffff; border-radius: 8px 8px 0 0;
  padding: 8px 16px; box-sizing: border-box;
  display: flex; justify-content: space-between; align-items: center;
  z-index: 20;
  transition: right 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}

.toolbar-layer.is-open { right: 292px; }

.toolbar-left { display: flex; align-items: center; gap: 12px; }
.toolbar-right { display: flex; align-items: center; gap: 8px; }

.toolbar-search { flex-shrink: 0; display: flex; align-items: center; border: 1px solid var(--border-color); border-radius: 4px; padding: 4px 8px; width: 250px; position: relative; }
.toolbar-search input { border: none; outline: none; margin-left: 8px; width: 100%; }
.toolbar-search:hover { border-color: #00ab6b; }
.search-suggestions { position: absolute; top: 100%; left: 0; width: 100%; min-width: 250px; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 4px; box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15); list-style: none; padding: 4px 0; margin: 4px 0 0 0; z-index: 1000; max-height: 250px; overflow-y: auto; }
.search-suggestions li { padding: 8px 12px; cursor: pointer; display: flex; align-items: center; font-size: 13px; transition: background-color 0.15s ease; }
.search-suggestions li:hover { background: #f4f5f8; color: #00ab6b; }
.suggest-name { font-weight: 500; color: #111111; } .suggest-dash { margin: 0 4px; color: #666666; } .suggest-code { color: #666666; }

.toolbar-dropdown { flex-shrink: 0; display: flex; align-items: center; justify-content: space-between; height: 32px; padding: 0 12px; border-radius: 4px; cursor: pointer; transition: all 0.2s ease; border: 1px solid #e0e0e0; background-color: #ffffff; position: relative;}
.dropdown-status { min-width: 160px; } .dropdown-status:hover { border-color: #00ab6b; }
.dropdown-unit { min-width: 300px; } .dropdown-unit:hover { border-color: #00ab6b; }
.label-text { opacity: 0.7; }

.custom-filter-list { position: absolute; top: 100%; left: 0; min-width: 180px; background: #ffffff; border: 1px solid #e0e0e0; border-radius: 4px; box-shadow: 0 4px 16px rgba(0, 0, 0, 0.15); list-style: none; padding: 8px 0; margin: 4px 0 0 0; z-index: 1000; }
.custom-filter-list li { padding: 8px 16px; cursor: pointer; display: flex; align-items: center; font-size: 13px; color: #111111; transition: background-color 0.2s; position: relative; }
.custom-filter-list li:hover { background: #f4f5f8; color: #00ab6b; }
.custom-filter-list li .misa-icon.mi-check { position: absolute; left: 8px; font-size: 14px; }
.custom-filter-list li.active { color: #00ab6b; font-weight: 500; background-color: #e5f6ed; }

.batch-info { flex-shrink: 0; display: flex; align-items: center; gap: 12px; font-size: 13px; }
.btn-deselect { color: #00ab6b; cursor: pointer; font-weight: 500; }
.batch-buttons { flex-shrink: 0; display: flex; align-items: center; gap: 12px; margin-left: 20px; }
.batch-btn { flex-shrink: 0; display: flex; align-items: center; gap: 6px; height: 32px; padding: 0 16px; border-radius: 4px; background: #ffffff; font-family: inherit; font-size: 13px; font-weight: 500; cursor: pointer; transition: all 0.2s; }
.btn-batch-warning { border: 1px solid #f39c12; color: #f39c12; } .btn-batch-warning:hover { background-color: #fdf2e9; }
.btn-batch-tracking { border: 1px solid #00ab6b; color: #00ab6b; } .btn-batch-tracking:hover { background-color: #e5f6ed; }
.btn-batch-danger { border: 1px solid #e74c3c; color: #e74c3c; } .btn-batch-danger:hover { background-color: #fdf0ef; }

.icon-btn { 
  width: 32px; height: 32px; background: none; border: 1px solid #e0e0e0; border-radius: 4px; 
  font-size: 18px; cursor: pointer; color: var(--text-secondary); display: flex; align-items: center; justify-content: center; transition: all 0.2s;
  user-select: none; -webkit-user-drag: none; position: relative;
}
.icon-btn:hover { background-color: #eceef4ba; color: var(--text-primary); }

.filter-badge {
  position: absolute; top: 4px; right: 4px; width: 6px; height: 6px; 
  background-color: #e74c3c; border-radius: 50%; border: 1px solid #ffffff;
}

/* ==========================================
   2. KHỐI BẢNG DỮ LIỆU & PHÂN TRANG (GRID LAYER)
   ========================================== */
.grid-layer { 
  position: absolute; top: 48px; left: 0; bottom: 0; 
  background: #ffffff; border-radius: 0 0 8px 8px; 
  display: flex; flex-direction: column; z-index: 10;
  transition: right 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}
.grid-layer.is-resizing { transition: none; }

/* CSS cho Dải hiển thị Tags (Applied Filters) */
.applied-filters-container {
  display: flex; align-items: center; flex-wrap: wrap; gap: 8px;
  padding: 12px 16px 4px 16px; background: #ffffff;
}
.filter-tag {
  display: inline-flex; align-items: top; gap: 4px;
  padding: 4px 1px; border-radius: 4px;
  background-color: #b3b4b717; font-size: 13px; line-height: 1.2;
}
.tag-label { color: #666666; }
.tag-operator { color: #00ab6b; font-weight: 500; }
.tag-value { color: #111111; font-weight: 400; }
.tag-close { 
  cursor: pointer; color: #666666; margin-left: 4px; transition: color 0.2s; 
  display: flex; align-items: center; justify-content: center;
}
.tag-close:hover { color: #e74c3c; }

.clear-all-filters {
  font-size: 13px; color: #008f59; cursor: pointer; margin-left: 8px; font-weight: 500; transition: color 0.2s;
}
.clear-all-filters:hover { color: #00ab6b; text-decoration: underline; }

:deep(.dx-datagrid) { flex: 1; min-height: 0; font-size: 13px; }
:deep(.dx-scrollbar-vertical), :deep(.dx-scrollbar-vertical .dx-scrollable-scroll) { width: 8px !important; border-radius: 8px !important; }
:deep(.dx-scrollbar-horizontal), :deep(.dx-scrollbar-horizontal .dx-scrollable-scroll) { height: 8px !important; border-radius: 8px !important; }

:deep(.dx-datagrid) *, :deep(.dx-datagrid-content) * { -webkit-user-select: text !important; -moz-user-select: text !important; user-select: text !important; }
::selection { background-color: #00ab6b !important; color: #ffffff !important; }
::-moz-selection { background-color: #00ab6b !important; color: #ffffff !important; }

:deep(.dx-datagrid-headers) { background-color: #f4f5f8 !important; color: var(--text-color); font-weight: 600; font-size: 14px; border-top: 1px solid #e0e0e0; }
:deep(.dx-datagrid-borders > .dx-datagrid-headers) { border-bottom: 1px solid var(--border-color); }
:deep(.dx-datagrid-headers .dx-header-row > td) { padding: 10px 8px !important; font-size: 13px; font-weight: 600; background-color: #f4f5f8 !important; color: #111111; border-bottom: 1px solid #e0e0e0 !important; border-left: none !important; border-right: none !important; }
:deep(.dx-datagrid-headers .dx-header-row > td:not(:last-child)::after) { content: ""; position: absolute; right: 0; top: 20%; height: 60%; width: 1px; background-color: #c0c0c0; }
:deep(.dx-datagrid .dx-row > td) { border-left: none !important; border-right: none !important; position: relative; }
:deep(.dx-datagrid .dx-row > td.dx-command-select::after) { display: none !important; }
:deep(.dx-datagrid-rowsview .dx-row > td) { border-bottom: 1px solid #e0e0e0 !important; border-top: none !important; }
:deep(.dx-datagrid-rowsview) { border-bottom: 1px solid #e0e0e0; }
:deep(.dx-datagrid-rowsview .dx-data-row:hover > td), :deep(.dx-datagrid-rowsview .dx-data-row.dx-state-hover > td) { background-color: #c1efd7 !important; color: #111111; cursor: pointer; }
:deep(.dx-datagrid-rowsview .dx-selection > td) { background-color: #c1efd7 !important; color: #111111; }

.row-actions { display: none; position: absolute; right: 12px; top: 50%; transform: translateY(-50%); align-items: center; gap: 8px; background-color: inherit; padding-left: 16px; box-shadow: -15px 0 15px -10px rgba(0, 0, 0, 0.08); }
:deep(.dx-datagrid-rowsview .dx-data-row:hover .row-actions){ display: flex; }
.action-btn { width: 28px; height: 28px; border-radius: 4px; background-color: #ffffff; border: 1px solid #e0e0e0; display: flex; align-items: center; justify-content: center; cursor: pointer; transition: all 0.2s ease; }
.action-btn:hover { background-color: #f4f5f8; }
.btn-warning { color: #f39c12; } .btn-warning:hover { border-color: #f39c12; }
.btn-normal { color: #666666; } .btn-normal:hover { border-color: #00ab6b; color: #00ab6b; }
.btn-danger { color: #e74c3c; } .btn-danger:hover { border-color: #e74c3c; }

:deep(.dx-datagrid .dx-row > td.column-actions) { position: sticky !important; right: 0 !important; z-index: 10; overflow: visible !important; border-left: none !important; background-color: inherit !important; }
:deep(.dx-datagrid-headers .dx-header-row > td.column-actions) { position: sticky !important; right: 0 !important; background-color: #f4f5f8 !important; z-index: 11; }

:deep(.dx-checkbox-checked .dx-checkbox-icon) { background-color: #00ab6b !important; border-color: #00ab6b !important; color: #ffffff !important; }
:deep(.dx-checkbox-indeterminate .dx-checkbox-icon) { background-color: #00ab6b !important; border-color: #00ab6b !important; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23ffffff' stroke-width='4' stroke-linecap='round' stroke-linejoin='round'%3E%3Cline x1='5' y1='12' x2='19' y2='12'%3E%3C/line%3E%3C/svg%3E") !important; background-size: 12px; background-position: center; background-repeat: no-repeat; }
:deep(.dx-checkbox-indeterminate .dx-checkbox-icon::before) { display: none !important; }

.status-badge { display: flex; align-items: center; gap: 6px; }
.status-tracking { padding: 2px 8px; border: 1px solid #00ab6b; border-radius: 8px; background-color: #e5f6ed; color: #00ab6b; font-weight: 500; }
.status-stopped { padding: -1px 8px; border: 1px solid #f39c12; border-radius: 8px; background-color: #fdf2e9; color: #f39c12; font-weight: 500; }
.status-dot { width: 8px; height: 8px; border-radius: 50%; flex-shrink: 0; }
.status-tracking .status-dot { background-color: #00ab6b;  }
.status-stopped .status-dot { background-color: #f39c12; }

.misa-empty-state { position: absolute; top: 50px; left: 0; right: 0; bottom: 0; display: flex; flex-direction: column; align-items: center; justify-content: center; background-color: transparent; pointer-events: none; z-index: 5; }
.misa-empty-state span { color: #666666; font-size: 13px; font-weight: 550; font-family: inherit; margin-top: 16px; pointer-events: auto; }
:deep(.dx-datagrid-nodata) { display: none !important; }

.misa-pagination {
  display: flex; justify-content: space-between; align-items: center; flex-wrap: wrap; 
  height: 46px; padding: 0 16px; background-color: #ffffff; border-top: 1px solid #e0e0e0; 
  font-size: 13px; color: #111111; margin-top: auto; border-radius: 0 0 8px 8px;
}
.pagination-right { display: flex; align-items: center; gap: 16px; }
.page-size-selector select { height: 28px; padding: 0 24px 0 8px; border: 1px solid #e0e0e0; border-radius: 4px; outline: none; cursor: pointer; font-family: inherit; font-size: 13px; appearance: none; background-image: url("data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' width='16' height='16' viewBox='0 0 24 24' fill='none' stroke='%23666666' stroke-width='2' stroke-linecap='round' stroke-linejoin='round'%3E%3Cpolyline points='6 9 12 15 18 9'%3E%3C/polyline%3E%3C/svg%3E"); background-repeat: no-repeat; background-position: right 4px center; background-size: 14px; }
.page-size-selector select:focus { border-color: #00ab6b; }
.page-range { min-width: 45px; text-align: center; }
.page-navigation { display: flex; align-items: center; gap: 4px; }
.nav-btn { display: flex; justify-content: center; align-items: center; width: 24px; height: 24px; background: none; border: none; border-radius: 4px; cursor: pointer; color: #666666; transition: all 0.2s; }
.nav-btn:not(:disabled):hover { background-color: #f4f5f8; color: #111111; }
.nav-btn:disabled { color: #cccccc; cursor: not-allowed; }

/* ==========================================
   3. FAKE GAP 
   ========================================== */
.fake-gap {
  position: absolute; top: 0; bottom: 0; width: 12px;
  background-color: #f4f5f8; z-index: 90; 
  transition: right 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}
.fake-gap.is-resizing { transition: none; }
.fade-enter-active, .fade-leave-active { transition: opacity 0.3s; }
.fade-enter-from, .fade-leave-to { opacity: 0; }

/* ==========================================
   4. BỘ LỌC NÂNG CAO (GIAO DIỆN NGUYÊN BẢN CỦA BẠN)
   ========================================== */
.filter-layer {
  position: absolute; right: 0; top: 0; bottom: 0; 
  background: #ffffff; border-radius: 8px; 
  box-shadow: -4px 0 16px rgba(0,0,0,0.08); 
  display: flex; flex-direction: column; z-index: 100;
  transition: transform 0.3s cubic-bezier(0.25, 0.8, 0.25, 1), width 0.3s cubic-bezier(0.25, 0.8, 0.25, 1);
}
.filter-layer.is-resizing { transition: none; }

.slide-right-enter-active, .slide-right-leave-active { transition: transform 0.3s cubic-bezier(0.25, 0.8, 0.25, 1); }
.slide-right-enter-from, .slide-right-leave-to { transform: translateX(100%); }

.resize-handle {
  position: absolute; left: -6px; width: 12px; top: 0; bottom: 0; cursor: col-resize; z-index: 101; display: flex; align-items: center; justify-content: center;
}
.resize-icon { width: 3px; height: 32px; background-color: transparent; border-radius: 4px; transition: background-color 0.2s; }
.resize-handle:hover .resize-icon, .is-resizing .resize-handle .resize-icon { background-color: #00ab6b; }

.filter-header { display: flex; justify-content: space-between; align-items: center; padding: 0 16px; border-bottom: 1px solid #e0e0e0; height: 56px; }
.filter-title { margin: 0; font-size: 16px; font-weight: 700; color: #111111; }
.btn-close-filter { background: transparent; border: none; cursor: pointer; padding: 4px; border-radius: 4px; display: flex; align-items: center; justify-content: center; }
.btn-close-filter:hover { background: #f0f0f0; }

.filter-body { flex: 1; overflow-y: auto; padding: 16px 20px; }
.filter-search-box { display: flex; align-items: center; border: 1px solid #e0e0e0; border-radius: 4px; padding: 6px 12px; margin-bottom: 16px; transition: border-color 0.2s; }
.filter-search-box:focus-within { border-color: #00ab6b; }
.filter-search-box input { border: none; outline: none; width: 100%; margin-left: 8px; font-family: inherit; }

.filter-list { list-style: none; padding: 0; margin: 0; }
.filter-item { border-radius: 8px; margin-bottom: 4px; transition: all 0.2s ease; }
.filter-item.is-active { background-color: #e5f6ed; padding-bottom: 12px; }

/* CSS gốc bộ lọc của bạn */
.filter-item-header { display: flex; align-items: center; padding: 10px 12px; cursor: pointer; gap: 12px; }
.misa-checkbox { width: 18px; height: 18px; border: 1px solid #c0c0c0; border-radius: 4px; display: flex; align-items: center; justify-content: center; background-color: #ffffff; transition: all 0.2s; flex-shrink: 0; }
.misa-checkbox.checked { background-color: #00ab6b; border-color: #00ab6b; }
.filter-label { font-size: 14px; color: #111111; font-weight: 500; }

.filter-item-content { padding: 0 12px; display: flex; flex-direction: column; gap: 8px; }
.misa-filter-select, .misa-filter-input { width: 100%; height: 32px; border: 1px solid #c0c0c0; border-radius: 4px; padding: 0 12px; outline: none; font-family: inherit; font-size: 13px; color: #111111; background-color: #ffffff; }
.misa-filter-select:focus, .misa-filter-input:focus { border-color: #00ab6b; }

.filter-footer { display: flex; justify-content: space-between; padding: 16px 20px; border-top: 1px solid #e0e0e0; background: #ffffff; border-radius: 0 0 8px 8px;}
.custom-dropdown-wrapper { position: relative; width: 100%; }

.misa-filter-select { 
  display: flex; justify-content: space-between; align-items: center; width: 100%; height: 32px; 
  border: 1px solid #c0c0c0; border-radius: 4px; padding: 0 12px; font-family: inherit; font-size: 13px; 
  color: #111111; background-color: #ffffff; cursor: pointer; user-select: none; 
}
.misa-filter-select:focus, .misa-filter-select:hover { border-color: #00ab6b; }

.custom-operator-list { 
  position: absolute; top: calc(100% + 4px); left: 0; right: 0; background: #ffffff; 
  border: 1px solid #e0e0e0; border-radius: 4px; box-shadow: 0 4px 16px rgba(0,0,0,0.1); 
  list-style: none; padding: 4px 0; margin: 0; z-index: 1000; 
}

.custom-operator-list li { 
  padding: 8px 12px; font-size: 13px; color: #111111; cursor: pointer; transition: all 0.2s; 
}

.custom-operator-list li:hover { 
  background-color: #c6c9d263; 
}

.custom-operator-list li.active { 
  background-color: #e5f6ed; 
  color: #00ab6b; 
  font-weight: 600; 
}
.icon-btn.is-filtered {
  background-color: transparent !important;
  border-color: #e0e0e0 !important;
}
/* ==========================================
   5. MODAL & TOAST THÔNG BÁO 
   ========================================== */
.misa-modal-overlay { position: fixed; top: 0; left: 0; right: 0; bottom: 0; background-color: rgba(0, 0, 0, 0.4); display: flex; justify-content: center; align-items: center; z-index: 9998; }
.misa-modal-confirm { background: #ffffff; width: 400px; border-radius: 8px; box-shadow: 0 4px 16px rgba(0, 0, 0, 0.16); display: flex; flex-direction: column; }
.modal-header { display: flex; justify-content: space-between; align-items: center; padding: 20px 24px 0 24px; cursor: grab; }
.modal-header:active { cursor: grabbing; }
.modal-title { margin: 0; font-size: 16px; color: #111111; font-weight: 700; }
.btn-close-modal { background: transparent; border: none; cursor: pointer; border-radius: 8px; padding: 8px; display: flex; }
.btn-close-modal:hover{ background-color: #e0e1e4e0; }
.confirm-body { padding: 16px 24px; display: flex; align-items: flex-start; gap: 16px; user-select: text !important; }
.confirm-message { font-size: 14px; color: #111111; line-height: 1.5; }
.confirm-footer { padding: 12px 24px; background: #ffffff; display: flex; justify-content: flex-end; gap: 12px; border-radius: 0 0 8px 8px; }
.btn-secondary { padding: 8px 24px; border: 1px solid #e0e0e0; background: #ffffff; color: #111111; border-radius: 8px; cursor: pointer; font-weight: 500; }
.btn-secondary:hover { background: #f0f0f0; }
.btn-primary { padding: 8px 24px; background: #00ab6b; border: none; color: #ffffff; border-radius: 8px; cursor: pointer; font-weight: 500; }
.btn-primary:hover { background: #008f59; }
.btn-primary.btn-danger { background: #F04438; }
.btn-primary.btn-danger:hover { background: #D92D20; }

.misa-toast-success { position: fixed; top: 32px; left: 50%; transform: translateX(-50%); z-index: 9999; background-color: #00ab6b; padding: 12px 16px; border-radius: 4px; box-shadow: 0 4px 16px rgba(0, 171, 107, 0.25); display: flex; align-items: center; justify-content: space-between; min-width: 320px; }
.toast-left { display: flex; align-items: center; gap: 10px; }
.toast-icon { flex-shrink: 0; }
.toast-text { color: #ffffff; font-size: 14px; font-weight: 500; }
.toast-close { background: transparent; border: none; cursor: pointer; display: flex; align-items: center; justify-content: center; padding: 0; opacity: 0.7; transition: opacity 0.2s ease; }
.toast-close:hover { opacity: 1; }
.toast-slide-enter-active, .toast-slide-leave-active { transition: all 0.4s cubic-bezier(0.25, 0.8, 0.25, 1); }
.toast-slide-enter-from, .toast-slide-leave-to { opacity: 0; transform: translate(-50%, -30px); }
</style>