using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmisPayroll.Application.Interfaces.Services
{
    public interface IBaseService<TDto, TCreateDto, TUpdateDto>
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto> GetByIdAsync(Guid id);
        Task<int> InsertAsync(TCreateDto dto);
        Task<int> UpdateAsync(Guid id, TUpdateDto dto);
        Task<int> DeleteAsync(Guid id);
    }
}