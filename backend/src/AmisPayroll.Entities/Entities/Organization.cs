using System;
using AmisPayroll.Entities.Base;

namespace AmisPayroll.Entities.Entities
{
    public class Organization : BaseEntity
    {
        public Guid OrganizationId { get; set; }
        public string OrganizationCode { get; set; }
        public string OrganizationName { get; set; }
        public string Description { get; set; }

        public override Guid GetId() => OrganizationId;
        public override void SetId(Guid id) => OrganizationId = id;
    }
}