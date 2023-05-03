using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.Enities.Interfaces
{
    public interface ICreateAudit
    {
        public string CreatedByUserName { get; }
        public DateTime CreatedDateUtc { get; }
    }
}
