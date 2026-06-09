namespace AmisPayroll.Application.DTOs
{
    public class SalaryCompositionDto
    {
        public Guid CompositionId { get; set; }
        public Guid OrganizationId { get; set; }
        
        public string OrganizationName { get; set; } = string.Empty;

        public string CompositionCode { get; set; } = string.Empty;
        public string CompositionName { get; set; } = string.Empty;
        public int CompositionType { get; set; }
        public int CompositionNature { get; set; }
        public int? TaxNature { get; set; }
        public string? NormFormula { get; set; }
        public int IsAllowExceedNorm { get; set; }
        public int ValueType { get; set; }
        public decimal Amount { get; set; }
        public string? CalculationFormula { get; set; }
        public string? Description { get; set; }
        public int IsDisplayOnPayslip { get; set; }
        public int SourceType { get; set; }
        public int Status { get; set; }
    }
}