using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AmisPayroll.Application.DTOs;
using AmisPayroll.Application.DTOs.SalaryComposition;
using AmisPayroll.Application.Exceptions;
using AmisPayroll.Application.Interfaces.Repositories;
using AmisPayroll.Application.Interfaces.Services;
using AmisPayroll.Entities.Entities;
using AmisPayroll.Entities.Enum;

namespace AmisPayroll.Application.Services
{
    public class SalaryCompositionService : ISalaryCompositionService
    {
        private readonly ISalaryCompositionRepository _repository;

        public SalaryCompositionService(ISalaryCompositionRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// API BƯỚC 1: Lấy danh sách phân trang, tìm kiếm và JOIN tên đơn vị
        /// </summary>
        public async Task<(IEnumerable<SalaryCompositionDto> Data, int TotalRecord)> GetPagingAsync(int skip, int take, string? searchValue)
        {
            var (rawData, totalCount) = await _repository.GetPagingAsync(skip, take, searchValue);

            var dtoList = new List<SalaryCompositionDto>();
            foreach (var item in rawData)
            {
                dtoList.Add(new SalaryCompositionDto
                {
                    CompositionId = item.composition_id is string strId ? Guid.Parse(strId) : (Guid)item.composition_id,
                    OrganizationId = item.organization_id is string strOrgId ? Guid.Parse(strOrgId) : (Guid)item.organization_id,
                    OrganizationName = item.OrganizationName ?? "Tất cả đơn vị", 
                    CompositionCode = item.composition_code,
                    CompositionName = item.composition_name,
                    CompositionType = (int)item.composition_type,
                    CompositionNature = (int)item.composition_nature,
                    TaxNature = item.tax_nature != null ? (int)item.tax_nature : null,
                    NormFormula = item.norm_formula,
                    IsAllowExceedNorm = (int)item.is_allow_exceed_norm,
                    ValueType = (int)item.value_type,
                    Amount = (decimal)item.amount,
                    CalculationFormula = item.calculation_formula,
                    Description = item.description,
                    IsDisplayOnPayslip = (int)item.is_display_on_payslip,
                    SourceType = (int)item.source_type,
                    Status = (int)item.status
                });
            }

            return (dtoList, totalCount);
        }

        /// <summary>
        /// Lấy tất cả thành phần lương (Không phân trang)
        /// </summary>
        public async Task<IEnumerable<SalaryCompositionDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            
            return entities.Select(e => new SalaryCompositionDto
            {
                CompositionId = e.CompositionId,
                OrganizationId = e.OrganizationId,
                OrganizationName = "Tất cả đơn vị",
                CompositionCode = e.CompositionCode,
                CompositionName = e.CompositionName,
                CompositionType = (int)e.CompositionType,
                CompositionNature = (int)e.CompositionNature,
                TaxNature = e.TaxNature != null ? (int)e.TaxNature : null,
                NormFormula = e.NormFormula,
                IsAllowExceedNorm = e.IsAllowExceedNorm,
                ValueType = (int)e.ValueType,
                Amount = e.Amount,
                CalculationFormula = e.CalculationFormula,
                Description = e.Description,
                IsDisplayOnPayslip = e.IsDisplayOnPayslip,
                SourceType = (int)e.SourceType,
                Status = (int)e.Status
            });
        }

        /// <summary>
        /// Lấy chi tiết thành phần lương theo ID
        /// </summary>
        public async Task<SalaryCompositionDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new ValidateException("Thành phần lương không tồn tại trong hệ thống.");

            return new SalaryCompositionDto
            {
                CompositionId = entity.CompositionId,
                OrganizationId = entity.OrganizationId,
                CompositionCode = entity.CompositionCode,
                CompositionName = entity.CompositionName,
                CompositionType = (int)entity.CompositionType,
                CompositionNature = (int)entity.CompositionNature,
                TaxNature = entity.TaxNature != null ? (int)entity.TaxNature : null,
                NormFormula = entity.NormFormula,
                IsAllowExceedNorm = entity.IsAllowExceedNorm,
                ValueType = (int)entity.ValueType,
                Amount = entity.Amount,
                CalculationFormula = entity.CalculationFormula,
                Description = entity.Description,
                IsDisplayOnPayslip = entity.IsDisplayOnPayslip,
                SourceType = (int)entity.SourceType,
                Status = (int)entity.Status
            };
        }

        /// <summary>
        /// Thêm mới thành phần lương 
        /// </summary>
        public async Task<int> InsertAsync(CreateSalaryCompositionDto dto)
        {
            // 1. Kiểm tra nghiệp vụ cơ bản
            if (string.IsNullOrWhiteSpace(dto.CompositionCode))
                throw new ValidateException("Mã thành phần lương không được để trống.");
                
            if (string.IsNullOrWhiteSpace(dto.CompositionName))
                throw new ValidateException("Tên thành phần lương không được để trống.");

            // 2. Gọi Repository kiểm tra trùng mã
            var isDuplicate = await _repository.CheckDuplicateCodeAsync(dto.CompositionCode);
            if (isDuplicate)
                throw new ValidateException($"Mã thành phần lương <{dto.CompositionCode}> đã tồn tại trong hệ thống.");

            // 3. Map từ DTO sang Entity để chuẩn bị lưu vào Database
            var entity = new SalaryComposition
            {
                CompositionId = Guid.NewGuid(),
                OrganizationId = dto.OrganizationId,
                CompositionCode = dto.CompositionCode,
                CompositionName = dto.CompositionName,
                CompositionType = dto.CompositionType,
                CompositionNature = (CompositionNature)dto.CompositionNature,
                TaxNature = dto.TaxNature != null ? (TaxNature)dto.TaxNature : null,
                NormFormula = dto.NormFormula,
                IsAllowExceedNorm = dto.IsAllowExceedNorm,
                ValueType = (ValueTypeEnum)dto.ValueType,
                Amount = dto.Amount,
                CalculationFormula = dto.CalculationFormula,
                Description = dto.Description,
                IsDisplayOnPayslip = dto.IsDisplayOnPayslip,
                SourceType = SourceType.Custom, // Mặc định tự thêm mới là Custom
                Status = Status.Tracking      // Mặc định tạo mới là Đang theo dõi
            };

            return await _repository.InsertAsync(entity);
        }

        /// <summary>
        /// Cập nhật thành phần lương (Sửa luôn lỗi bấm Ngừng theo dõi bị đứng im UI)
        /// </summary>
        public async Task<int> UpdateAsync(Guid id, UpdateSalaryCompositionDto dto)
        {
            // 1. Kiểm tra tồn tại bản ghi
            var existEntity = await _repository.GetByIdAsync(id);
            if (existEntity == null)
                throw new ValidateException("Thành phần lương không tồn tại.");

            // 2. Kiểm tra trùng mã với bản ghi khác
            var isDuplicate = await _repository.CheckDuplicateCodeAsync(dto.CompositionCode, id);
            if (isDuplicate)
                throw new ValidateException($"Mã thành phần lương <{dto.CompositionCode}> đã tồn tại.");

            // 3. Cập nhật đầy đủ các trường dữ liệu từ DTO vào Entity hiện tại
            existEntity.OrganizationId = dto.OrganizationId;
            existEntity.CompositionCode = dto.CompositionCode;
            existEntity.CompositionName = dto.CompositionName;
            existEntity.CompositionType = dto.CompositionType;
            existEntity.CompositionNature = (CompositionNature)dto.CompositionNature;
            existEntity.TaxNature = dto.TaxNature != null ? (TaxNature)dto.TaxNature : null;
            existEntity.NormFormula = dto.NormFormula;
            existEntity.IsAllowExceedNorm = dto.IsAllowExceedNorm;
            existEntity.ValueType = (ValueTypeEnum)dto.ValueType;
            existEntity.Amount = dto.Amount;
            existEntity.CalculationFormula = dto.CalculationFormula;
            existEntity.Description = dto.Description;
            existEntity.IsDisplayOnPayslip = dto.IsDisplayOnPayslip;
            
            existEntity.Status = (Status)dto.Status; 

            return await _repository.UpdateAsync(existEntity);
        }

        /// <summary>
        /// Xóa bản ghi (Chống xóa hàng hệ thống)
        /// </summary>
        public async Task<int> DeleteAsync(Guid id)
        {
            var existEntity = await _repository.GetByIdAsync(id);
            if (existEntity == null)
                throw new ValidateException("Thành phần lương không tồn tại.");
                
            // Không cho phép người dùng xóa các danh mục cài đặt sẵn của hệ thống AMIS
            if (existEntity.SourceType == SourceType.System)
                throw new ValidateException("Đây là thành phần lương mặc định của hệ thống nên không thể xóa.");

            return await _repository.DeleteAsync(id);
        }
    }
}
