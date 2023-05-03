using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.Enities.Interfaces
{
    public interface IFullAuditModel : ICreateAudit, IUpdateAudit, ISoftDeletable
    {
        public new bool IsDeleted { get; }
        public new string DeletedByUserName { get; }
        public new DateTime? DeletedDateUtc { get; }
        public new string UpdatedByUserName { get; }
        public new DateTime? UpdatedDateUtc { get; }
        public new string CreatedByUserName { get; }
        public new DateTime CreatedDateUtc { get; }
    }
}
