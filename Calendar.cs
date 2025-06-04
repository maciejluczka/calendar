using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    internal class Calendar : IPainter
    {
        //next/prev month button
        public class MonthButton : Clickable, IPainter
        {
            private string imgPath;
            private bool next;
            private Calendar calendar;

            public MonthButton(string _imgPath, bool _next, Calendar _calendar, int _x, int _y)
            {
                imgPath = _imgPath;
                next = _next;
                calendar = _calendar;
                x = _x; 
                y = _y;
                size = Image.FromFile(imgPath).Width;
            }

            public void Draw(Graphics g)
            {
                Image img = Image.FromFile(imgPath);
                g.DrawImage(img, x, y);
            }

            public override void onClick()
            {
                int month = calendar.selectedMonth.month;
                int year = calendar.selectedMonth.year;
                if (next)
                {
                    if (month == 12)
                        calendar.ChangeMonth(1, year + 1);
                    else
                        calendar.ChangeMonth(month + 1, year);
                }
                else
                {
                    if (month == 1)
                        calendar.ChangeMonth(12, year - 1);
                    else
                        calendar.ChangeMonth(month - 1, year);
                }
            }
        
        }


        public Month selectedMonth;
        public MonthButton prevMonth, nextMonth;

        private Form1 form;
        public Calendar(Form1 _form)
        {
            form = _form;

            ChangeMonth(DateTime.Now.Month, DateTime.Now.Year);


            //Create next/prev month button
            int width = Image.FromFile("..//..//img//leftArrow.png").Width;
            prevMonth = new MonthButton("..//..//img//leftArrow.png", false, this, Constants.leftMargin, 10);
            nextMonth = new MonthButton("..//..//img//rightArrow.png", true, this, Constants.leftMargin + Constants.daySize * 7 - width, 10);
        }

        public void Draw(Graphics g)
        {
            DrawDayLabels(g);
            DrawDate(g);
        }

        private void DrawDayLabels(Graphics g)
        {
            string[] dayNames = { "Poniedziałek", "Wtorek", "Środa", "Czwartek", "Piątek", "Sobota", "Niedziela" };

            int padding = 4;
            using (Font smallFont = new Font(SystemFonts.DefaultFont.FontFamily, 12f))
            {
                //iterate through week
                for (int i = 0; i < 7; i++)
                {
                    string dayName = dayNames[i];

                    SizeF textSize = g.MeasureString(dayName, smallFont);

                    float tx = Constants.leftMargin + i * Constants.daySize + (Constants.daySize - textSize.Width) / 2;
                    float ty = Constants.topMargin - textSize.Height - padding;

                    g.DrawString(dayName, smallFont, Brushes.Black, tx, ty);

                }
            }
        }

        private void DrawDate(Graphics g)
        {
            using (Font smallFont = new Font(SystemFonts.DefaultFont.FontFamily, 16f, FontStyle.Bold))
            {
                string[] months = { "Styczeń", "Luty", "Marzec", "Kwiecień", "Maj", "Czerwiec", "Lipiec", "Sierpień", "Wrzesień", "Pażdziernik", "Listopad", "Grudzień" };

                string cornerText = $"{months[selectedMonth.month-1]} {selectedMonth.year}";

                SizeF textSize = g.MeasureString(cornerText, smallFont);

                float calendarAreaWidth = Constants.windowWidth - Constants.sidebarWidth;

                float tx = (calendarAreaWidth - textSize.Width + Constants.leftMargin) / 2;
                float ty = 30;

                g.DrawString(cornerText, smallFont, Brushes.Black, tx, ty);
            }
        }

        public void ChangeMonth(int month, int year)
        {
            
            selectedMonth = new Month(month, year);
            Sidebar sidebar = Sidebar.Instance;
            sidebar.SetEmptySidebar();

            Database database = Database.Instance;
            string[] t = database.LoadDays(month, year);
            for(int i = 0; i < t.Length; i ++) // 3 values per event
            {
               string[] data = t[i].Split(',');
               Day matchingDay = selectedMonth.days.FirstOrDefault(day => day.day == Int32.Parse(data[0]));
               matchingDay.name = data[1];
               matchingDay.description = data[2];

            }

            form.Invalidate();
            
        }



    }
}
