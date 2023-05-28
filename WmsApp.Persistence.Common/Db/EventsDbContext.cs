using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using WmsApp.Domain.Common.Enities.Events;
using WmsApp.Domain.Common.StatusValidator;
using WmsApp.Persistence.Common.Db.EventRunner;

namespace WmsApp.Persistence.Common.Db
{
    public abstract class EventsDbContext<T> : DbContext
        where T : DbContext
    {
        private IEventRunner _eventRunner;


        public EventsDbContext(DbContextOptions<T> options, IEventRunner eventRunner) : base(options)
        {
            _eventRunner = eventRunner;
        }

        public EventsDbContext() : base()
        {

        }

        public override int SaveChanges()
        {
            if(_eventRunner is null)
                return base.SaveChanges();

            return _eventRunner.RunEvents(this, () => base.SaveChanges());
        }
    }
}
