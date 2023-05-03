using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.Enities.Interfaces
{
    public interface ISoftDeletable
    {
        public bool IsDeleted { get; }
        public string DeletedByUserName { get; }
        public DateTime? DeletedDateUtc { get; }
    }
}
