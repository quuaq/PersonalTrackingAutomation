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
            DotNetEnv.Env.Load();
            txtPassword.PasswordChar = '*';
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    MessageBox.Show("PostgreSQL Connecting is Success!");

                    // Kullanıcıyı ve tc_number bilgisini sorgula
                    string query = "SELECT tc_number FROM users WHERE username = @username AND password = @password;";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", password);

                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string tcNumber = reader["tc_number"].ToString();

                                MessageBox.Show("Login Success!");

                                // Log kaydı ekle
                                reader.Close(); // Reader kapatılır çünkü aynı bağlantıda log ekleyeceğiz
                                string logQuery = "INSERT INTO logs (tc_number, action) VALUES (@tc_number, @action);";
                                using (var logCmd = new NpgsqlCommand(logQuery, connection))
                                {
                                    logCmd.Parameters.AddWithValue("tc_number", tcNumber);
                                    logCmd.Parameters.AddWithValue("action", "User logged in.");
                                    logCmd.ExecuteNonQuery();
                                }

                                // Form2'yi aç
                                Form2 form2 = new Form2();
                                form2.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Username or Password Wrong!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection error: {ex.Message}");
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Text = "User Login";
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
