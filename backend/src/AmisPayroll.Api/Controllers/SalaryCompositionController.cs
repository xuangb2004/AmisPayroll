using AmisPayroll.Api.Controllers.Base;
using AmisPayroll.Application.DTOs;
using AmisPayroll.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using AmisPayroll.Application.DTOs.SalaryComposition;
using System.Threading.Tasks;

namespace AmisPayroll.Api.Controllers
{
    public class SalaryCompositionController : BaseController<SalaryCompositionDto, CreateSalaryCompositionDto, UpdateSalaryCompositionDto>
    {
        private readonly ISalaryCompositionService _salaryCompositionService;

        public SalaryCompositionController(ISalaryCompositionService salaryCompositionService) 
            : base(salaryCompositionService)
        {
            _salaryCompositionService = salaryCompositionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaging([FromQuery] int skip = 0, [FromQuery] int take = 15, [FromQuery] string? searchValue = null)
        {
            var (data, totalRecord) = await _salaryCompositionService.GetPagingAsync(skip, take, searchValue);
            
            return Ok(new
            {
                data = data,
                totalRecord = totalRecord
            });
        }
        
    }
}