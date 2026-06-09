using AmisPayroll.Application.DTOs;
using AmisPayroll.Application.DTOs.SalaryComposition;
using AmisPayroll.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace AmisPayroll.Api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]s")]
    public class SalaryCompositionController : ControllerBase
    {
        private readonly ISalaryCompositionService _salaryCompositionService;

        public SalaryCompositionController(ISalaryCompositionService salaryCompositionService)
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalaryCompositionDto dto)
        {
            var result = await _salaryCompositionService.UpdateAsync(id, dto);

            return Ok(new
            {
                Success = true,
                Data = result,
                Message = "Cập nhật thành công."
            });
        }
    }
}
