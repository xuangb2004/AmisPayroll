using System;
using AmisPayroll.Entities.Base;
using AmisPayroll.Entities.Enum;

namespace AmisPayroll.Entities.Entities
{
    public class SalaryCompositionSystem : BaseEntity
    {
        public Guid SystemCompositionId { get; set; }
        public string SystemCompositionCode { get; set; }
        public string SystemCompositionName { get; set; }
        public int CompositionType { get; set; }
        public CompositionNature CompositionNature { get; set; }
        public ValueTypeEnum ValueType { get; set; }
        public decimal DefaultAmount { get; set; }

        public override Guid GetId() => SystemCompositionId;
        public override void SetId(Guid id) => SystemCompositionId = id;
    }
}