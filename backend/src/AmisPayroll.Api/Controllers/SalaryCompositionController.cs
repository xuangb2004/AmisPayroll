using AmisPayroll.Api.Controllers.Base;
using AmisPayroll.Application.DTOs.SalaryComposition;
using AmisPayroll.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AmisPayroll.Api.Controllers
{
    public class SalaryCompositionsController : BaseController<SalaryCompositionDto, CreateSalaryCompositionDto, UpdateSalaryCompositionDto>
    {
        private readonly ISalaryCompositionService _salaryService;

        public SalaryCompositionsController(ISalaryCompositionService salaryService) : base(salaryService)
        {
            _salaryService = salaryService;
        }
    }
}