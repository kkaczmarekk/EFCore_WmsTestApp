using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.StatusValidator;

namespace WmsApp.Domain.Common.Enities.Events
{
    public interface IEntityEventHandler<T> where T : IEntityEvent
    {
        IStatusValidator Handle (object callingEntity, T entityEvent);
    }
}
