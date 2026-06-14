using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AmisPayroll.Entities.Entities;
using AmisPayroll.Application.DTOs.SalaryComposition;
using AmisPayroll.Application.DTOs;

namespace AmisPayroll.Application.Interfaces.Repositories
{
    public interface ISalaryCompositionRepository : IBaseRepository<SalaryComposition>
    {
        Task<bool> CheckDuplicateCodeAsync(string code);
        Task<bool> CheckDuplicateCodeAsync(string code, Guid excludeId);
        Task<(IEnumerable<dynamic> Data, int TotalRecord)> GetPagingAsync(int skip, int take, string? searchValue,int? status = null);
    }
}
