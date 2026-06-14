using AmisPayroll.Application.DTOs;
using AmisPayroll.Application.DTOs.SalaryComposition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AmisPayroll.Application.Interfaces.Services
{
    public interface ISalaryCompositionService : IBaseService<SalaryCompositionDto, CreateSalaryCompositionDto, UpdateSalaryCompositionDto>
    {
        Task<(IEnumerable<SalaryCompositionDto> Data, int TotalRecord)> GetPagingAsync(int skip, int take, string? searchValue, int? status = null);
    }
}
