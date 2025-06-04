using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    public partial class Form1 : Form
    {
        List<IPainter> painters;
        List<Clickable> clickables;
        Calendar calendar;
        Sidebar sidebar;
        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            sidebar = Sidebar.Initialize(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            painters = new List<IPainter>();
            clickables = new List<Clickable>();
            calendar = new Calendar(this);
            this.Text = "Kalendarz";


        }
        private void Form1_Paint_1(object sender, PaintEventArgs e)
        {
            painters.Clear();
            painters.AddRange(calendar.selectedMonth.days);
            painters.Add(calendar);
            painters.Add(calendar.prevMonth);
            painters.Add(calendar.nextMonth);
            painters.Add(sidebar);

            foreach (IPainter painter in painters)
            {
                painter.Draw(e.Graphics);
            }
        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            clickables.Clear();
            clickables.AddRange(calendar.selectedMonth.days);
            clickables.Add(calendar.prevMonth);
            clickables.Add(calendar.nextMonth);

            foreach (Clickable clickable in clickables)
            {
                if (e.X > clickable.x && e.Y > clickable.y && e.X < clickable.x + clickable.size && e.Y < clickable.y + clickable.size)
                {
                    clickable.onClick();
                }
            }
        }

    }
}
