using System;
using AmisPayroll.Entities.Enum;

namespace AmisPayroll.Application.DTOs.SalaryComposition
{
    public class CreateSalaryCompositionDto
    {
        public Guid OrganizationId { get; set; }
        public string CompositionCode { get; set; } = string.Empty;
        public string CompositionName { get; set; } = string.Empty;
        public int CompositionType { get; set; }
        public CompositionNature CompositionNature { get; set; }
        public TaxNature? TaxNature { get; set; }
        
        public string NormFormula { get; set; } = string.Empty;
        public int IsAllowExceedNorm { get; set; }
        public ValueTypeEnum ValueType { get; set; }
        public decimal Amount { get; set; }
        public string CalculationFormula { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int IsDisplayOnPayslip { get; set; }
    }
}