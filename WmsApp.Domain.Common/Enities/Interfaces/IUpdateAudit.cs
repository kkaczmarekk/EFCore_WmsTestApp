using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.Enities.Interfaces
{
    public interface IUpdateAudit
    {
        public string UpdatedByUserName { get; }
        public DateTime? UpdatedDateUtc { get; }
    }
}
