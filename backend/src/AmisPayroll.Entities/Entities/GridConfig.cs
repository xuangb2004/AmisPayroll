using System;
using AmisPayroll.Entities.Base;

namespace AmisPayroll.Entities.Entities
{
    public class GridConfig : BaseEntity
    {
        public Guid GridConfigId { get; set; }
        public Guid UserId { get; set; }
        public string GridName { get; set; }
        public string ColumnName { get; set; }
        public int IsVisible { get; set; }
        public int Width { get; set; }
        public int IsPinned { get; set; }

        public override Guid GetId() => GridConfigId;
        public override void SetId(Guid id) => GridConfigId = id;
    }
}