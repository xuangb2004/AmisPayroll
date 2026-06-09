using System;
using AmisPayroll.Entities.Base;
using AmisPayroll.Entities.Enum;

namespace AmisPayroll.Entities.Entities
{
    public class SalaryComposition : BaseEntity
    {
        public Guid CompositionId { get; set; }
        public Guid OrganizationId { get; set; }
        
        public string CompositionCode { get; set; }
        public string CompositionName { get; set; }
        public int CompositionType { get; set; } // Mã loại thành phần 
        public CompositionNature CompositionNature { get; set; }
        public TaxNature? TaxNature { get; set; } // Cho phép null
        
        public string NormFormula { get; set; }
        public int IsAllowExceedNorm { get; set; } 
        
        public ValueTypeEnum ValueType { get; set; }
        public decimal Amount { get; set; }
        public string CalculationFormula { get; set; }
        
        public string Description { get; set; }
        public int IsDisplayOnPayslip { get; set; } 
        
        public SourceType SourceType { get; set; }
        public Status Status { get; set; }

        public override Guid GetId() => CompositionId;
        public override void SetId(Guid id) => CompositionId = id;
    }
}