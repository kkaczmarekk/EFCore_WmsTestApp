using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WmsApp.Domain.Common.DateTimeGenerator
{
    public class FixedDateTimeGenerator : IActualDateTime
    {
        public FixedDateTimeGenerator(int day, int month, int year)
        {
            Day = day;
            Month = month;
            Year = year;
        }

        public int Day { get; }
        public int Month { get; }
        public int Year { get; }

        public DateTime GetActualDateTime()
        {
            return new DateTime(Year, Month, Day);
        }
    }
}
