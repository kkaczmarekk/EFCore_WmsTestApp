using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.DateTimeGenerator
{
    public class ActualDateTimeGenerator : IActualDateTime
    {
        public DateTime GetActualDateTime() => DateTime.Now;
    }
}
