using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Calendar
{
    internal sealed class Sidebar : IPainter
    {
        private static Sidebar _instance;
        private static Form1 _form;
        public static Sidebar Instance
        {
            get
            {
                if (_instance == null)
                    throw new InvalidOperationException(
                        "Sidebar not initialized – call Sidebar.Initialize(form) first");
                return _instance;
            }
        }

        public static Sidebar Initialize(Form1 form)
        {
            if (_instance != null)
                throw new InvalidOperationException("Sidebar already initialized");
            _form = form ?? throw new ArgumentNullException(nameof(form));
            _instance = new Sidebar(_form);
            return _instance;
        }

        private TextBox name, description;
        private Label date, alert;
        private Button saveButton, deleteButton;
        private readonly int xPos, width;
        private Day selectedDay;

        public Sidebar(Form1 form)
        {
            xPos = Constants.leftMargin + Constants.daySize * 7 + Constants.rightMargin;
            width = Constants.windowWidth - xPos;


            name = new TextBox();
            name.Location = new Point(xPos + 10, 70);
            name.Size = new Size(width - 35, 20);

            date = new Label();
            date.Location = new Point(xPos + 10, 10);
            date.BackColor = Color.DarkGray;
            date.Font = new Font(SystemFonts.DefaultFont.FontFamily, 12f);
            date.Width = 200;


            description = new TextBox();
            description.Location = new Point(xPos + 10, 130);
            description.Size = new Size(width - 35, 200);
            description.Multiline = true;


            alert = new Label();
            alert.Location = new Point(xPos + width/3, 390);
            alert.BackColor = Color.DarkGray;
            alert.Font = new Font(SystemFonts.DefaultFont.FontFamily, 10f);

            Label nameLabel = new Label();
            nameLabel.Location = new Point(xPos + 10, name.Location.Y - 25);
            nameLabel.BackColor = Color.DarkGray;
            nameLabel.Font = new Font(SystemFonts.DefaultFont.FontFamily, 10f);
            nameLabel.Text = "Tytuł";

            Label descriptionLabel = new Label();
            descriptionLabel.Location = new Point(xPos + 10, description.Location.Y - 25);
            descriptionLabel.BackColor = Color.DarkGray;
            descriptionLabel.Font = new Font(SystemFonts.DefaultFont.FontFamily, 10f);
            descriptionLabel.Text = "Opis";

            //Save button
            saveButton = new Button();
            saveButton.Location = new Point(xPos + width/3 - 50, 360);
            saveButton.Click += (sender, args) => SaveData();
            saveButton.Text = "Zapisz";

            //Delete button
            deleteButton = new Button();
            deleteButton.Location = new Point(xPos + width / 3 + 50, 360);
            deleteButton.Click += (sender, args) => DeleteDay();
            deleteButton.Text = "Usuń";

            SetEmptySidebar();

            form.Controls.Add(name);
            form.Controls.Add(description);
            form.Controls.Add(date);
            form.Controls.Add(saveButton);
            form.Controls.Add(deleteButton);
            form.Controls.Add(alert);
            form.Controls.Add(nameLabel);
            form.Controls.Add(descriptionLabel);
        }

        public void SetEmptySidebar()
        {
            deleteButton.Enabled = false;
            saveButton.Enabled = false;
            date.Text = "Kliknij w dzień";
            alert.Text = "";
        }


        public void Draw(Graphics g)
        {
            //dark background
            Brush brush = new SolidBrush(Color.DarkGray);
            g.FillRectangle(brush, xPos, 0, width, Constants.windowHeight);


        }

        public void SetSidebar(Day _selectedDay)
        {
            saveButton.Enabled = true;
            deleteButton.Enabled = true;
            selectedDay = _selectedDay;
            name.Text = selectedDay.name;
            description.Text = selectedDay.description;
            date.Text = $"Wybrany dzień: {selectedDay.day}.{selectedDay.month}.{selectedDay.year}";
            alert.Text = "";

        }

        private void SaveData(){
            if(name.Text != null && name.Text != "")
            {

                selectedDay.name = name.Text;
                selectedDay.description = description.Text;

                Database database = Database.Instance;
                database.InsertEvent(selectedDay);
                alert.Text = "Zapisano";

                _form.Invalidate();
            } else
            {
                alert.Text = "Podaj tytuł wydarzenia";
                _form.Invalidate();
            }
        }

        private void DeleteDay()
        {
            name.Text = "";
            description.Text = "";

            selectedDay.name = name.Text;
            selectedDay.description = description.Text;

            Database database = Database.Instance;
            string date = $"{selectedDay.day}.{selectedDay.month}.{selectedDay.year}";
            database.DeleteDay(date);

            _form.Invalidate();

        }
    }
}
