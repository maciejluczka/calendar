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
    public partial class Login : Form
    {
        Database database;
        public Login()
        {
            InitializeComponent();
            database = Database.Initialize();
            bool connect = database.Connect();
            if (!connect)
            {
                LoginAlert.Text = "Nie udało się połaczyć z bazą!";
                RegisterAlert.Text = "Nie udało się połaczyć z bazą!";
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Login_Load(object sender, EventArgs e)
        {

        }

        Form1 form = new Form1();
        private void button1_Click(object sender, EventArgs e)
        {
            if (database.Login(LoginLogin.Text, LoginPassword.Text)) {
                this.Hide();
                form.Show();
            } else
            {
                LoginAlert.Text = "Niepoprawny login";
                
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {




        }

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            database.Close();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (RegisterLogin.Text != "" && RegisterPass.Text != "")
            {
                bool userExists = database.UserExists(RegisterLogin.Text);
                if (!userExists)
                {
                    database.Register(RegisterLogin.Text, RegisterPass.Text);
                    RegisterAlert.Text = "Pomyślnie zajerestrowano";
                }
                else
                {
                    RegisterAlert.Text = "Ten użytkownik już istnieje";
                }


            }
        }
    }
}
