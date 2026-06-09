using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<SalaryCompositionDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            
            return entities.Select(e => new SalaryCompositionDto
            {
                CompositionId = e.CompositionId,
                OrganizationId = e.OrganizationId,
                CompositionCode = e.CompositionCode,
                CompositionName = e.CompositionName,
                CompositionType = e.CompositionType,
                CompositionNature = e.CompositionNature,
                TaxNature = e.TaxNature,
                Amount = e.Amount,
                Status = e.Status,
                SourceType = e.SourceType,
                IsDisplayOnPayslip = e.IsDisplayOnPayslip
            });
        }

        public async Task<SalaryCompositionDto> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new ValidateException("Thành phần lương không tồn tại trong hệ thống.");

            return new SalaryCompositionDto
            {
                CompositionId = entity.CompositionId,
                CompositionCode = entity.CompositionCode,
                CompositionName = entity.CompositionName
            };
        }

        public async Task<int> InsertAsync(CreateSalaryCompositionDto dto)
        {
            // 1. Validate
            if (string.IsNullOrWhiteSpace(dto.CompositionCode))
                throw new ValidateException("Mã thành phần lương không được để trống.");
                
            if (string.IsNullOrWhiteSpace(dto.CompositionName))
                throw new ValidateException("Tên thành phần lương không được để trống.");

            var isDuplicate = await _repository.CheckDuplicateCodeAsync(dto.CompositionCode);
            if (isDuplicate)
                throw new ValidateException($"Mã thành phần lương <{dto.CompositionCode}> đã tồn tại.");

            var entity = new SalaryComposition
            {
                CompositionId = Guid.NewGuid(),
                OrganizationId = dto.OrganizationId,
                CompositionCode = dto.CompositionCode,
                CompositionName = dto.CompositionName,
                CompositionType = dto.CompositionType,
                CompositionNature = dto.CompositionNature,
                TaxNature = dto.TaxNature,
                Amount = dto.Amount,
                SourceType = SourceType.Custom, 
                Status = Status.Tracking
            };

            return await _repository.InsertAsync(entity);
        }

        public async Task<int> UpdateAsync(Guid id, UpdateSalaryCompositionDto dto)
        {
            var existEntity = await _repository.GetByIdAsync(id);
            if (existEntity == null)
                throw new ValidateException("Thành phần lương không tồn tại.");

            var isDuplicate = await _repository.CheckDuplicateCodeAsync(dto.CompositionCode, id);
            if (isDuplicate)
                throw new ValidateException($"Mã thành phần lương <{dto.CompositionCode}> đã tồn tại.");

            existEntity.CompositionCode = dto.CompositionCode;
            existEntity.CompositionName = dto.CompositionName;
            existEntity.CompositionNature = dto.CompositionNature;
            existEntity.Amount = dto.Amount;

            return await _repository.UpdateAsync(existEntity);
        }

        public async Task<int> DeleteAsync(Guid id)
        {
            var existEntity = await _repository.GetByIdAsync(id);
            if (existEntity == null)
                throw new ValidateException("Thành phần lương không tồn tại.");
                
            if (existEntity.SourceType == SourceType.System)
                throw new ValidateException("Đây là thành phần lương mặc định của hệ thống nên không thể xóa.");

            return await _repository.DeleteAsync(id);
        }
    }
}