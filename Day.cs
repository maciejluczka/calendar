using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Calendar
{
    internal class Day : Clickable, IPainter
    {
        private Rectangle rectangle;

        public string name, description;
        public int day, month, year;
        Pen pen = new Pen(Color.Black);

        public Day(int _day, int _firstDayOfWeek, int _month, int _year)
        {
            day = _day;
            month = _month;
            year = _year;
            rectangle = getPosition(_firstDayOfWeek);

            //Clickable base
            x = rectangle.X;
            y = rectangle.Y;
            size = Constants.daySize;
        }

        public void Draw(Graphics graphics)
        {
            //Draw day square
            graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);


            //Draw day number
            const int padding = 4;
            using (Font smallFont = new Font(SystemFonts.DefaultFont.FontFamily, 8f))
            {
                string cornerText = day.ToString();   
                                                       
                SizeF textSize = graphics.MeasureString(cornerText, smallFont);

                float tx = rectangle.Right - textSize.Width - padding;
                float ty = rectangle.Top + padding;

                graphics.DrawString(cornerText, smallFont, Brushes.Black, tx, ty);
            }

            //Draw name text in middle
            if(name != null)
            {
                using (Font font = new Font(SystemFonts.DefaultFont.FontFamily, 10f))
                {
                    SizeF textSize = graphics.MeasureString(name, font);

                    float tx = rectangle.Left + (rectangle.Width - textSize.Width) / 2f;
                    float ty = rectangle.Top + (rectangle.Height - textSize.Height) / 2f;

                    graphics.DrawString(name, font, Brushes.Red, tx, ty);
                }
            }

        }
        private Rectangle getPosition(int firstDayOfWeek)
        {
            // calculate month offset + day index
            int index = firstDayOfWeek + (day - 1);

            int col = index % 7;
            int row = index / 7;
            if (row >= 5) row = 5 - 1;  // clamp overflow into last row

            int x = Constants.leftMargin + col * Constants.daySize;
            int y = Constants.topMargin + row * Constants.daySize;

            return new Rectangle(x, y, Constants.daySize, Constants.daySize);
        }

        public override void onClick()
        {
            Sidebar sidebar = Sidebar.Instance;
            sidebar.SetSidebar(this);
        }
    }
}
