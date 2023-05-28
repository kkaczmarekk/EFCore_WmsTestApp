using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Persistence.Common.Db.EventRunner
{
    public interface IEventRunner
    {
        public int RunEvents(DbContext context, Func<int> runBaseSaveChanges);
    }
}
