using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Calendar
{
    internal class Month
    {
        public int year;
        public int month;
        public List<Day> days;

        public Month(int _month, int _year)
        {
            month = _month;
            year = _year;
            days = new List<Day>();
            Setup();

        }

        private void Setup()
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            DayOfWeek dow = new DateTime(year, month, 1).DayOfWeek;
            int firstDayOfWeek = ((int)dow + 6) % 7;
            //fill days list
            for (int i = 0; i < daysInMonth; i++)
            {
                Day d = new Day(i + 1, firstDayOfWeek, month, year);
                days.Add(d);
            }

        }

    }
}
