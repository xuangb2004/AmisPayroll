<template>
  <div class="white-card-form">
    
    <div class="form-row">
      <div class="form-label">Tên thành phần <span class="required">*</span></div>
      <div class="form-control">
        <DxTextBox 
          v-model="form.name" 
          value-change-event="keyup input"
          :class="{ 'has-error': errors.name }"
          @input="handleNameInput" 
        />
        <div class="error-text" v-if="errors.name">Không được để trống.</div>
      </div>
    </div>

    <div class="form-row">
      <div class="form-label">Mã thành phần <span class="required">*</span></div>
      <div class="form-control">
        <DxTextBox 
          v-model="form.code" 
          placeholder="Nhập mã viết liền" 
          value-change-event="keyup input"
          :class="{ 'has-error': errors.code }"
          @input="handleCodeInput" 
        />
        <div class="error-text" v-if="errors.code">Không được để trống.</div>
      </div>
    </div>

    <div class="form-row align-top" style="margin-top: 4px;">
      <div class="form-label" style="margin-top: 8px;">Đơn vị áp dụng <span class="required">*</span></div>
      <div class="form-control">
        <DxDropDownBox
          v-model="form.units"
          v-model:opened="isUnitDropdownOpen"
          :data-source="unitData"
          value-expr="id"
          display-expr="name"
          class="misa-dropdownbox"
          :class="{ 'has-error': errors.units, 'is-opened': isUnitDropdownOpen }"
          field-template="customTagTemplate" 
          :show-drop-down-button="false"
        >
          <template #customTagTemplate>
            <div class="custom-tag-field">
              <DxTextBox class="hidden-dx-textbox" :read-only="true" />
              
              <div v-if="form.units.length === 0" class="tag-placeholder">Chọn đơn vị</div>
              <div v-else class="tags-wrapper">
                <span class="misa-tag" v-for="id in form.units" :key="id" @click.stop>
                  {{ getUnitName(id) }}
                  <span class="tag-remove" @click.stop="removeUnit(id)">×</span>
                </span>
              </div>
              
              <div class="mi-chevron-down Express-dropdown-arrow"></div>
            </div>
          </template>

          <template #content>
            <DxTreeView
              ref="treeViewRef"
              :data-source="unitData"
              data-structure="plain"
              key-expr="id"
              parent-id-expr="parentId"
              display-expr="name"
              show-check-boxes-mode="normal"
              selection-mode="multiple"
              :select-by-click="true" 
              @item-selection-changed="onUnitSelectionChanged"
              class="misa-treeview"
            />
          </template>
        </DxDropDownBox>
        <div class="error-text" v-if="errors.units">Không được để trống.</div>
      </div>
    </div>

    <div class="form-row">
      <div class="form-label">Loại thành phần <span class="required">*</span></div>
      <div class="form-control">
        <DxSelectBox 
          v-model="form.type" 
          :data-source="['Lương', 'Phụ cấp', 'Phúc lợi']" 
          class="misa-selectbox"
        />
      </div>
    </div>

    <div class="form-row">
      <div class="form-label">Tính chất <span class="required">*</span></div>
      <div class="form-control nature-group">
        <DxSelectBox 
          v-model="form.nature" 
          :data-source="['Thu nhập', 'Khấu trừ', 'Khác']" 
          class="misa-selectbox nature-select"
        />
        
        <div v-if="form.nature === 'Thu nhập'" class="dynamic-options inline-radios">
          <label class="misa-radio"><input type="radio" value="Chịu thuế" v-model="form.taxOption"> <span class="radio-mark"></span> Chịu thuế</label>
          <label class="misa-radio"><input type="radio" value="Miễn thuế toàn phần" v-model="form.taxOption"> <span class="radio-mark"></span> Miễn thuế toàn phần</label>
          <label class="misa-radio"><input type="radio" value="Miễn thuế một phần" v-model="form.taxOption"> <span class="radio-mark"></span> Miễn thuế một phần</label>
        </div>

        <div v-else-if="form.nature === 'Khấu trừ'" class="dynamic-options">
          <label class="misa-checkbox">
            <input type="checkbox" v-model="form.isTaxDeduction"> 
            <span class="checkbox-mark"></span> Giảm trừ khi tính thuế
          </label>
        </div>
      </div>
    </div>

    <template v-if="form.nature === 'Thu nhập' || form.nature === 'Khấu trừ'">
      <div class="form-row align-top">
        <div class="form-label" style="margin-top: 8px;">Định mức</div>
        <div class="form-control">
          <textarea 
            v-model="form.norm" 
            class="misa-textarea formula-box" 
            placeholder="Tự động gợi ý công thức và tham số khi gõ" 
            rows="3"
          ></textarea>
          
          <div class="mt-2" style="display: flex; align-items: center; gap: 8px;">
            <label class="misa-checkbox">
              <input type="checkbox" v-model="form.allowExceedNorm"> 
              <span class="checkbox-mark"></span> Cho phép giá trị tính vượt quá định mức
            </label>
            <i class="misa-icon-info" title="Thông tin chi tiết">i</i>
          </div>
        </div>
      </div>
    </template>

    <div class="form-row mt-2">
      <div class="form-label">Kiểu giá trị</div>
      <div class="form-control">
        <DxSelectBox v-model="form.valueType" :data-source="['Tiền tệ', 'Phần trăm', 'Hệ số']" />
      </div>
    </div>

    <div class="form-row align-top">
      <div class="form-label" style="margin-top: 8px;">Giá trị</div>
      <div class="form-control">
        <div class="value-options">
          <label class="misa-radio block-radio">
            <input type="radio" :value="1" v-model="form.valueOption"> 
            <span class="radio-mark"></span> Tự động cộng tổng giá trị của các nhân viên
          </label>
          <div class="indent-block" v-if="form.valueOption === 1">
            <div style="display: flex; align-items: center; gap: 8px;">
              <DxSelectBox :data-source="['Trong cùng đơn vị công tác', 'Khác']" v-model="form.valueScope" width="220"/>
              <i class="misa-icon-info">i</i>
              <DxTextBox v-model="form.valueInput" width="100%"/>
            </div>
          </div>
          <label class="misa-radio block-radio mt-2">
            <input type="radio" :value="2" v-model="form.valueOption"> 
            <span class="radio-mark"></span> Tính theo công thức tự đặt</label>
          <div class="indent-block" v-if="form.valueOption === 2">
            <div class="formula-container">
              <textarea v-model="form.customFormula" class="misa-textarea formula-box" placeholder="Tự động gợi ý công thức và tham số khi gõ" rows="3"></textarea>
              <div class="ai-icon">🤖</div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <div class="form-row align-top mt-2">
      <div class="form-label" style="margin-top: 8px;">Mô tả</div>
      <div class="form-control">
        <textarea v-model="form.description" class="misa-textarea resize-y" rows="3"></textarea>
      </div>
    </div>

    <div class="form-row">
      <div class="form-label">Hiển thị trên phiếu lương</div>
      <div class="form-control inline-radios">
        <label class="misa-radio"><input type="radio" value="Có" v-model="form.showOnPayslip"> <span class="radio-mark"></span> Có</label>
        <label class="misa-radio"><input type="radio" value="Không" v-model="form.showOnPayslip"> <span class="radio-mark"></span> Không</label>
        <label class="misa-radio"><input type="radio" value="Chỉ khi khác 0" v-model="form.showOnPayslip"> <span class="radio-mark"></span> Chỉ hiển thị nếu giá trị khác 0</label>
      </div>
    </div>

    <div class="form-row">
      <div class="form-label">Nguồn tạo</div>
      <div class="form-control">
        <input type="text" class="misa-readonly-input" value="Tự thêm" readonly />
      </div>
    </div>

  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue';
import axios from 'axios';
import { DxTextBox } from 'devextreme-vue/text-box';
import { DxSelectBox } from 'devextreme-vue/select-box';
import { DxDropDownBox } from 'devextreme-vue/drop-down-box';
import { DxTreeView } from 'devextreme-vue/tree-view';

const emit = defineEmits(['close']); 

// ==========================
// 1. STATE QUẢN LÝ FORM
// ==========================
const isManualCode = ref(false);
const isUnitDropdownOpen = ref(false);
const treeViewRef = ref(null);

const form = ref({
  name: '',
  code: '',
  units: [],
  type: null,
  nature: 'Thu nhập', 
  taxOption: 'Chịu thuế',
  isTaxDeduction: false,
  norm: '', 
  allowExceedNorm: false, 
  valueType: 'Tiền tệ',
  valueOption: 2, 
  valueScope: 'Trong cùng đơn vị công tác',
  valueInput: '',
  customFormula: '',
  description: '',
  showOnPayslip: 'Có'
});

// Trạng thái báo lỗi
const errors = ref({
  name: false,
  code: false,
  units: false
});

// ==========================
// 2. LOGIC TẠO MÃ TỰ ĐỘNG VÀ BÁO LỖI THỜI GIAN THỰC
// ==========================
const removeVietnameseTones = (str) => {
  str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
  str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
  str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
  str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
  str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
  str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
  str = str.replace(/đ/g, "d");
  str = str.replace(/À|Á|Ạ|Ả|Ã|Â|Ầ|Ấ|Ậ|Ẩ|Ẫ|Ă|Ằ|Ắ|Ặ|Ẳ|Ẵ/g, "A");
  str = str.replace(/È|É|Ẹ|Ẻ|Ẽ|Ê|Ề|Ế|Ệ|Ể|Ễ/g, "E");
  str = str.replace(/Ì|Í|Ị|Ỉ|Ĩ/g, "I");
  str = str.replace(/Ò|Ó|Ọ|Ỏ|Õ|Ô|Ồ|Ố|Ộ|Ổ|Ỗ|Ơ|Ờ|Ớ|Ợ|Ở|Ỡ/g, "O");
  str = str.replace(/Ù|Ú|Ụ|Ủ|Ũ|Ư|Ừ|Ứ|Ự|Ử|Ữ/g, "U");
  str = str.replace(/Ỳ|Ý|Ỵ|Ỷ|Ỹ/g, "Y");
  str = str.replace(/Đ/g, "D");
  return str;
};

const handleNameInput = (e) => {
  const currentName = e.event ? e.event.target.value : form.value.name;
  form.value.name = currentName;

  if (!currentName || !currentName.trim()) {
    errors.value.name = true;
  } else {
    errors.value.name = false;
  }

  if (!isManualCode.value) {
    let rawCode = removeVietnameseTones(currentName);
    form.value.code = rawCode.toUpperCase().replace(/\s+/g, '_');
    
    if (!form.value.code || !form.value.code.trim()) {
      errors.value.code = true;
    } else {
      errors.value.code = false;
    }
  }
};

const handleCodeInput = (e) => {
  isManualCode.value = true;
  const currentCode = e.event ? e.event.target.value : form.value.code;
  form.value.code = currentCode;

  if (!currentCode || !currentCode.trim()) {
    errors.value.code = true;
  } else {
    errors.value.code = false;
  }
};

// ==========================
// 3. LOGIC ĐƠN VỊ ÁP DỤNG VÀ ĐỒNG BỘ TREE VIEW
// ==========================
const unitData = ref([]);

const fetchUnits = async () => {
  try {
    // DỮ LIỆU GIẢ LẬP ĐỂ TEST
    unitData.value = [
      { id: '1', name: 'Thi công', parentId: null },
      { id: '2', name: 'Phòng Kinh doanh', parentId: null },
      { id: '2_1', name: 'Sản xuất 1', parentId: '2' },
      { id: '2_2', name: 'Nguyên liệu 1', parentId: '2' },
    ];
  } catch (error) {
    console.error("Lỗi khi tải danh sách phòng ban:", error);
  }
};

onMounted(() => {
  fetchUnits();
});

const onUnitSelectionChanged = (e) => {
  const selectedNodes = e.component.getSelectedNodes();
  
  form.value.units = selectedNodes.map(node => node.key);
  
  if (form.value.units.length > 0) {
    errors.value.units = false;
  }
};

const getUnitName = (id) => {
  const unit = unitData.value.find(u => u.id === id);
  return unit ? unit.name : id;
};

const removeUnit = (id) => {
  form.value.units = form.value.units.filter(u => u !== id);
  
  if (treeViewRef.value && treeViewRef.value.instance) {
    treeViewRef.value.instance.unselectItem(id);
  }
};

// ==========================
// 4. KIỂM TRA LỖI & LƯU
// ==========================
const validateForm = () => {
  let isValid = true;
  if (!form.value.name.trim()) { errors.value.name = true; isValid = false; }
  if (!form.value.code.trim()) { errors.value.code = true; isValid = false; }
  if (form.value.units.length === 0) { errors.value.units = true; isValid = false; }
  return isValid;
};

const save = async () => {
  if (!validateForm()) return;

  try {
    // 1. ÁNH XẠ DỮ LIỆU TỪ FORM VUE SANG DTO CỦA C#
    
    // Map CompositionType (Giả sử: 1 = Lương, 2 = Phụ cấp, 3 = Phúc lợi)
    let typeInt = 1;
    if (form.value.type === 'Phụ cấp') typeInt = 2;
    if (form.value.type === 'Phúc lợi') typeInt = 3;

    // Map CompositionNature (Theo DB: 1 = Thu nhập, 2 = Khấu trừ, 3 = Khác)
    let natureInt = 1;
    if (form.value.nature === 'Khấu trừ') natureInt = 2;
    if (form.value.nature === 'Khác') natureInt = 3;

    // Map ValueType (Giả sử: 1 = Tiền tệ, 2 = Phần trăm, 3 = Hệ số)
    let valueTypeInt = 1;
    if (form.value.valueType === 'Phần trăm') valueTypeInt = 2;
    if (form.value.valueType === 'Hệ số') valueTypeInt = 3;

    // Map Hiển thị phiếu lương (Có = 1, Không = 0)
    let isDisplay = form.value.showOnPayslip === 'Có' ? 1 : 0;
    
    // Map Cho phép vượt định mức
    let allowExceed = form.value.allowExceedNorm ? 1 : 0;

    // 2. TẠO PAYLOAD ĐÚNG CHUẨN CreateSalaryCompositionDto.cs
    const payload = {
      // TẠM THỜI lấy ID đơn vị đầu tiên trong mảng để test API (Cần bạn quyết định logic Vấn đề 1)
      OrganizationId: form.value.units.length > 0 ? form.value.units[0] : null, 
      
      CompositionCode: form.value.code,
      CompositionName: form.value.name,
      CompositionType: typeInt,
      CompositionNature: natureInt,
      TaxNature: null, // Bổ sung logic map Tax nếu cần
      NormFormula: form.value.norm,
      IsAllowExceedNorm: allowExceed,
      ValueType: valueTypeInt,
      Amount: form.value.valueOption === 1 ? (Number(form.value.valueInput) || 0) : 0,
      CalculationFormula: form.value.valueOption === 2 ? form.value.customFormula : null,
      Description: form.value.description,
      IsDisplayOnPayslip: isDisplay
    };

    console.log("Payload gửi lên C#:", payload);

    // 3. GỌI AXIOS
    // Đã đổi port theo file launchSettings.json của bạn (http://localhost:5094)
    await axios.post('http://localhost:5094/api/v1/SalaryCompositions', payload);
    
    console.log("Đã lưu vào DB thành công!");
    emit('save-success'); 

  } catch (error) {
    console.error("Lỗi kết nối API:", error);
    if (error.response) {
      // In ra lỗi cụ thể từ C# (Ví dụ: Lỗi validation 400 Bad Request)
      console.error("Chi tiết lỗi từ C#:", error.response.data);
    }
  }
};

const saveAndAdd = () => {
  if (!validateForm()) return;
  console.log("Lưu và tiếp tục thêm:", form.value);
  form.value = { ...form.value, name: '', code: '', units: [] };
  isManualCode.value = false;
};

defineExpose({
  save,
  saveAndAdd
});
</script>

<style scoped>
/* Tổng thể khối trắng */
.white-card-form {
  background-color: #ffffff;
  padding: 24px;
  border-radius: 4px;
  box-shadow: 0 1px 3px rgba(0,0,0,0.1);
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 20px; 
  margin-bottom: 16px;
  font-family: Roboto, Helvetica, Arial, sans-serif;
  color: #111111;
  font-size: 13px;
}

.form-row { display: flex; align-items: center; }
.form-row.align-top { align-items: flex-start; }
.form-label { width: 180px; min-width: 180px; font-weight: 400; }
.required { color: #e74c3c; margin-left: 2px;}
.form-control { flex: 1; max-width: 700px; position: relative; }

/* Trạng thái lỗi */
.error-text { color: #f23d3d; font-size: 12px; margin-top: 4px; }
.has-error :deep(.dx-texteditor) { border-color: #f23d3d !important; }

/* Ép DevExtreme thành ô box */
:deep(.dx-texteditor) { 
  border: 1px solid #d9d9d9 !important; 
  border-radius: 4px !important; 
  background-color: #ffffff !important;
}

:deep(.dx-texteditor::before),
:deep(.dx-texteditor::after) {
  display: none !important;
}

:deep(.dx-texteditor.dx-state-hover), 
:deep(.dx-texteditor.dx-state-focused) { 
  border-color: #00ab6b !important; 
}

:deep(.dx-texteditor-input) { height: 32px; min-height: 32px; }

/* Custom Tag Field cho Đơn vị áp dụng */
.custom-tag-field {
  display: flex; align-items: center; flex-wrap: wrap; gap: 4px;
  min-height: 32px; padding: 4px 32px 4px 8px; 
  background-color: transparent; cursor: pointer; position: relative;
}
.tag-placeholder { color: #999; padding: 2px 0; }
.tags-wrapper { display: flex; flex-wrap: wrap; gap: 6px; }

.misa-tag {
  display: flex; align-items: center; gap: 4px; background-color: #f0f0f0; 
  padding: 2px 8px; border-radius: 4px; font-size: 13px; color: #111;
  border: 1px solid #e0e0e0;
}
.tag-remove { font-size: 14px; color: #666; cursor: pointer; font-weight: bold; margin-left: 2px; padding: 0 2px; }
.tag-remove:hover { color: #e74c3c; }

.custom-tag-field::after {
  content: ""; position: absolute; right: 10px; top: 50%; transform: translateY(-50%);
  border: 5px solid transparent; border-top-color: #666; margin-top: 3px;
}


/* Giao diện list và tree khi chọn */
:deep(.misa-selectbox .dx-list-item.dx-state-hover),
:deep(.misa-treeview .dx-treeview-node.dx-state-hover) { background-color: #f4f5f8 !important; }

:deep(.misa-selectbox .dx-list-item-selected),
:deep(.misa-treeview .dx-treeview-item-without-checkbox.dx-state-selected) { background-color: #e5f6ed !important; color: #00ab6b !important; }

:deep(.dx-checkbox-checked .dx-checkbox-icon) { background-color: #00ab6b !important; border-color: #00ab6b !important; color: #ffffff !important; }

/* Radio và Checkbox */
.nature-group { display: flex; gap: 16px; align-items: center; }
.nature-select { width: 250px; flex-shrink: 0; }
.dynamic-options { flex: 1; padding-left: 8px; }
.inline-radios { display: flex; gap: 20px; align-items: center; }
.value-options { display: flex; flex-direction: column; gap: 8px; width: 100%; }
.indent-block { padding-left: 24px; margin-top: 4px; margin-bottom: 8px; }

.misa-radio, .misa-checkbox { display: flex; align-items: center; gap: 8px; cursor: pointer; position: relative; }
.misa-radio input, .misa-checkbox input { position: absolute; opacity: 0; cursor: pointer; }

.misa-icon-info {
  display: inline-flex; align-items: center; justify-content: center;
  width: 16px; height: 16px; border-radius: 50%; border: 1px solid #999;
  color: #999; font-size: 10px; font-style: normal; font-weight: bold; cursor: help;
}

.radio-mark { width: 16px; height: 16px; border-radius: 50%; border: 1px solid #bbb; display: inline-block; position: relative; transition: all 0.2s; }
.misa-radio:hover .radio-mark { border-color: #00ab6b; }
.misa-radio input:checked ~ .radio-mark { border-color: #00ab6b; }
.misa-radio input:checked ~ .radio-mark::after { content: ""; position: absolute; top: 3px; left: 3px; width: 8px; height: 8px; border-radius: 50%; background-color: #00ab6b; }

.checkbox-mark { width: 16px; height: 16px; border-radius: 4px; border: 1px solid #bbb; display: inline-block; position: relative; }
.misa-checkbox:hover .checkbox-mark { border-color: #00ab6b; }
.misa-checkbox input:checked ~ .checkbox-mark { background-color: #00ab6b; border-color: #00ab6b; }
.misa-checkbox input:checked ~ .checkbox-mark::after { content: ""; position: absolute; left: 4px; top: 1px; width: 4px; height: 8px; border: solid white; border-width: 0 2px 2px 0; transform: rotate(45deg); }

/* Textarea */
.misa-textarea { 
  width: 100%; border: 1px solid #d9d9d9; border-radius: 4px; 
  padding: 8px 12px; font-family: inherit; font-size: 13px; 
  outline: none; transition: border-color 0.2s; 
}
.resize-y { resize: vertical; }
.misa-textarea:hover, .misa-textarea:focus { border-color: #00ab6b; }
.formula-box { color: #111111; resize: none; background-color: #fff; }
.formula-box::placeholder { color: #cccccc; font-weight: 500; }

.formula-container { position: relative; width: 100%; }
.ai-icon {
  position: absolute; right: 12px; bottom: 12px;
  width: 24px; height: 24px; background: #f0f4ff; border-radius: 50%;
  display: flex; align-items: center; justify-content: center; font-size: 14px;
}
/* =======================================================
   ÉP Ô CHỌN ĐƠN VỊ GIỮ VIỀN XANH KHI ĐANG CHỌN 
   ======================================================= */
:deep(.misa-dropdownbox) {
  border: 1px solid #d9d9d9 !important;
  border-radius: 4px !important;
  background-color: #ffffff !important;
  transition: border-color 0.15s ease;
}

:deep(.misa-dropdownbox.dx-state-hover) {
  border-color: #00ab6b !important;
}
:deep(.misa-dropdownbox .dx-texteditor-input) {
  display: none !important; 
}
.is-opened:deep(.misa-dropdownbox),
:deep(.misa-dropdownbox.dx-state-focused),
:deep(.misa-dropdownbox.dx-dropdowneditor-active) {
  border-color: #00ab6b !important;
  box-shadow: 0 0 0 2px rgba(0, 171, 107, 0.1) !important; /* Hiệu ứng đổ bóng nhẹ chuẩn MISA */
}

/* Ẩn hoàn toàn input thô ẩn bên dưới của thư viện */


/* Khung chứa các thẻ Tag */
.custom-tag-field {
  display: flex;
  align-items: center;
  flex-wrap: wrap;
  gap: 6px;
  min-height: 32px;
  width: 100%;
  padding: 4px 36px 4px 10px; 
  cursor: pointer;
  background-color: transparent;
}

.tag-placeholder {
  color: #999999;
  font-size: 13px;
}

.tags-wrapper {
  display: flex;
  flex-wrap: wrap;
  gap: 6px;
  width: 100%;
}

.misa-tag {
  display: inline-flex;
  align-items: center;
  gap: 4px;
  background-color: #f0f0f0;
  padding: 2px 8px;
  border-radius: 4px;
  font-size: 13px;
  color: #111111;
  border: 1px solid #e0e0e0;
  white-space: nowrap;
}

.tag-remove {
  font-size: 14px;
  color: #666666;
  cursor: pointer;
  font-weight: bold;
}
.tag-remove:hover {
  color: #f23d3d;
}

/* Vẽ mũi tên dropdown góc phải ô chọn */
.misa-dropdown-arrow {
  position: absolute;
  right: 12px;
  top: 50%;
  transform: translateY(-50%);
  width: 0;
  height: 0;
  border-left: 5px solid transparent;
  border-right: 5px solid transparent;
  border-top: 5px solid #666666;
  transition: transform 0.2s ease;
}

/* Xoay ngược mũi tên lên trên khi danh sách đang mở */
.is-opened .misa-dropdown-arrow {
  transform: translateY(-50%) rotate(180deg);
  border-top-color: #00ab6b;
}

/* =======================================================
   GIAO DIỆN DANH SÁCH CÂY BÊN TRONG (TREE VIEW)
   ======================================================= */
:deep(.misa-treeview .dx-treeview-item.dx-state-hover) {
  background-color: #f4f5f8 !important;
}

:deep(.misa-treeview .dx-treeview-node.dx-treeview-node-is-checked > .dx-treeview-item) {
  background-color: #e5f6ed !important;
  color: #00ab6b !important;
}

:deep(.misa-treeview .dx-checkbox-checked .dx-checkbox-icon) {
  background-color: #00ab6b !important;
  border-color: #00ab6b !important;
  color: #ffffff !important;
}
:deep(.misa-treeview .dx-checkbox-checked .dx-checkbox-icon::before) {
  color: #ffffff !important;
}

:deep(.hidden-dx-textbox) {
  position: absolute !important;
  width: 0 !important;
  height: 0 !important;
  opacity: 0 !important;
  overflow: hidden !important;
  pointer-events: none !important;
  border: none !important;
  padding: 0 !important;
  margin: 0 !important;
}
.misa-readonly-input { width: 250px; padding: 6px 12px; border: 1px solid #e0e0e0; background-color: #f4f5f8; color: #666; border-radius: 4px; outline: none; }
.mt-2 { margin-top: 8px; }
</style>