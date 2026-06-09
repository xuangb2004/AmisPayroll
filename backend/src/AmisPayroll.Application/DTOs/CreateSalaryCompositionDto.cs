using System;
using AmisPayroll.Entities.Enum;

namespace AmisPayroll.Application.DTOs.SalaryComposition
{
    public class CreateSalaryCompositionDto
    {
        public Guid OrganizationId { get; set; }
        public string CompositionCode { get; set; }
        public string CompositionName { get; set; }
        public int CompositionType { get; set; }
        public CompositionNature CompositionNature { get; set; }
        public TaxNature? TaxNature { get; set; }
        public string NormFormula { get; set; }
        public int IsAllowExceedNorm { get; set; }
        public ValueTypeEnum ValueType { get; set; }
        public decimal Amount { get; set; }
        public string CalculationFormula { get; set; }
        public string Description { get; set; }
        public int IsDisplayOnPayslip { get; set; }
    }
}