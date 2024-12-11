using Npgsql;
using System;
using System.Windows.Forms;
using DotNetEnv;

namespace PersonalTrackingAutomation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtPassword.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string connectionString = "Host=localhost;Port=5432;Database=PersonalTrackingAutomation;Username=postgres;Password=123456;";

            try
            {

                string username = txtUsername.Text;
                string password = txtPassword.Text;


                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show("PostgreSQL Connecting is Success!");


                    string query = "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {

                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password);


                        int userExists = Convert.ToInt32(cmd.ExecuteScalar());
                        if (userExists > 0)
                        {
                            MessageBox.Show("Login Success!");

                            //open form2
                            Form2 form2 = new Form2();
                            form2.Show();

                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show("Usarname or Password Wrong!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Bağlantı hatası: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "User Login";
            DotNetEnv.Env.Load();
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }
    }
}
