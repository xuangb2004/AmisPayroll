using System;

namespace AmisPayroll.Entities.Base
{
    public abstract class BaseEntity
    {
        public abstract Guid GetId();
        public abstract void SetId(Guid id);
    }
}