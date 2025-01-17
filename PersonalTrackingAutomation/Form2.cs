﻿using Npgsql;
using System;
using DotNetEnv;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Microsoft.VisualBasic;


namespace PersonalTrackingAutomation
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
            DotNetEnv.Env.Load();
            LoadUsers();
            LoadPersonals();
            LoadLogs();
        }


        

        private void LoadLogs()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM logs ORDER BY timestamp DESC;";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dgvLogs.DataSource = dt; // Logs DataGridView'e bağlanıyor
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Loglar yüklenirken hata oluştu: {ex.Message}");
            }
        }


        private void LoadUsers()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM users ORDER BY tc_number ASC;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void LoadPersonals()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM personals ORDER BY tc_number ASC;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataGridView2.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kullanıcılar yüklenirken hata oluştu: {ex.Message}");
            }
        }

        private void tabPage1_clear()
        {
            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();
            textBox5.Clear();
        }
        private void tabPAge2_clear()
        {
            maskedTextBox1.Clear();
            maskedTextBox2.Clear();
            maskedTextBox3.Clear();
            maskedTextBox4.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
        }

        private void RegisterUser()
        {
            bool kayitKontrol = false;
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT * FROM users WHERE tc_number = @tc_number";
                    using (var selectSorgu = new NpgsqlCommand(query, connection))
                    {
                        selectSorgu.Parameters.AddWithValue("@tc_number", textBox1.Text);

                        using (var kayitOkuma = selectSorgu.ExecuteReader())
                        {
                            if (kayitOkuma.HasRows)
                            {
                                kayitKontrol = true; 
                                MessageBox.Show("You have already registered with this TR number!");
                            }
                        }
                    }

                   
                    if (kayitKontrol == false)
                    {
                        
                        if (textBox1.Text.Length < 11 || textBox1.Text == "")
                            label1.ForeColor = Color.Red;
                        else
                            label1.ForeColor = Color.Black;

                        if (textBox2.Text.Length < 2 || textBox2.Text == "")
                            label2.ForeColor = Color.Red;
                        else
                            label2.ForeColor = Color.Black;

                        if (textBox3.Text.Length < 2 || textBox3.Text == "")
                            label3.ForeColor = Color.Red;
                        else
                            label3.ForeColor = Color.Black;

                        if (textBox4.Text == "")
                            label4.ForeColor = Color.Red;
                        else
                            label4.ForeColor = Color.Black;

                        if (textBox5.Text == "")
                            label5.ForeColor = Color.Red;
                        else
                            label5.ForeColor = Color.Black;

                        
                        if (textBox1.Text.Length == 11 && textBox2.Text.Length >= 2 && textBox3.Text.Length >= 2 && textBox4.Text != "" && textBox5.Text != "")
                        {
                            string insertQuery = "INSERT INTO users (tc_number, name, surname, username, password) VALUES (@tc, @name, @surname, @username, @password);";
                            using (var insertSorgu = new NpgsqlCommand(insertQuery, connection))
                            {
                                insertSorgu.Parameters.AddWithValue("tc", textBox1.Text);
                                insertSorgu.Parameters.AddWithValue("name", textBox2.Text);
                                insertSorgu.Parameters.AddWithValue("surname", textBox3.Text);
                                insertSorgu.Parameters.AddWithValue("username", textBox4.Text);
                                insertSorgu.Parameters.AddWithValue("password", textBox5.Text);

                                insertSorgu.ExecuteNonQuery();
                                
                                DialogResult result = MessageBox.Show("New user is registered", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                
                            }   

                            string logQuery = "INSERT INTO logs (tc_number, action) VALUES (@tc_number, @action);";
                            using (var logCmd = new NpgsqlCommand(logQuery, connection))
                            {
                                logCmd.Parameters.AddWithValue("tc_number", textBox1.Text);
                                logCmd.Parameters.AddWithValue("action", "New user added.");
                                logCmd.ExecuteNonQuery();
                            }

                            LoadUsers(); 
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kayıt eklenirken hata oluştu: {ex.Message}");
            }
        }


        private void UpdateUser()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE users SET name = @name, surname = @surname, username = @username, password = @password WHERE tc_number = @tc;";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("tc", textBox1.Text);
                        cmd.Parameters.AddWithValue("name", textBox2.Text);
                        cmd.Parameters.AddWithValue("surname", textBox3.Text);
                        cmd.Parameters.AddWithValue("username", textBox4.Text);
                        cmd.Parameters.AddWithValue("password", textBox5.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("User is updated successfuly!");
                    }

                    string logQuery = "INSERT INTO logs (tc_number, action) VALUES (@tc_number, @action);";
                    using (var logCmd = new NpgsqlCommand(logQuery, connection))
                    {
                        logCmd.Parameters.AddWithValue("tc_number", textBox1.Text);
                        logCmd.Parameters.AddWithValue("action", "User information updated.");
                        logCmd.ExecuteNonQuery();
                    }
                }

                LoadUsers(); // Refreshing the datagridview and database table
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}");
            }

        }

        private void DeleteUser()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            // Ask the user for the name of the person they want to delete
            string userNameToDelete = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter the name of user which you want: ",
                "User delete"
            );

            if (string.IsNullOrWhiteSpace(userNameToDelete))
            {
                MessageBox.Show("You must the enter name");
                return;
            }

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "DELETE from users where name = @name";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("name", userNameToDelete);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"{userNameToDelete} successfuly delete");

                            //DataGridView delete rows
                            foreach(DataGridViewRow row in dataGridView1.Rows)
                            {
                                if (row.Cells["name"].Value.ToString() == userNameToDelete)
                                {
                                    dataGridView1.Rows.Remove(row);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show($"{userNameToDelete} his not found!");
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}");
            }
        }




        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "Personal Application";
            //Personal Applications
            maskedTextBox1.Mask = "00000000000";
            maskedTextBox2.Mask = "LL????????????????????";
            maskedTextBox3.Mask = "LL????????????????????";
            maskedTextBox4.Mask = "0000";
            maskedTextBox4.Text = "0";

            comboBox1.Items.Add("Primary School");
            comboBox1.Items.Add("Middle School");
            comboBox1.Items.Add("High School");
            comboBox1.Items.Add("University");

            comboBox2.Items.Add("Manager");
            comboBox2.Items.Add("Driver");
            comboBox2.Items.Add("Officer");
            comboBox2.Items.Add("Employee");

            comboBox3.Items.Add("Research and Development");
            comboBox3.Items.Add("IT");
            comboBox3.Items.Add("Accounting");
            comboBox3.Items.Add("Production");
            comboBox3.Items.Add("Transport");

            DateTime time = DateTime.Now;
            int year = int.Parse(time.ToString("yyyy"));
            int month = int.Parse(time.ToString("MM"));
            int day = int.Parse(time.ToString("dd"));

            dateTimePicker1.MinDate = new DateTime(1960, 1, 1);
            dateTimePicker1.MaxDate = new DateTime(year - 18, month, day);
            dateTimePicker1.Format = DateTimePickerFormat.Long;

            PopulateUserComboBox();
            LoadTasks();
            FormatTaskGridView();
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        //Search Button
        private void button5_Click(object sender, EventArgs e)
        {
            bool registerSearchStatus = false; //Variable to check whether a record is found or not
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (textBox1.Text.Length == 11)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM users WHERE tc_number = @tc_number";
                        using (var selectSorgu = new NpgsqlCommand(query, connection))
                        {
                            selectSorgu.Parameters.AddWithValue("@tc_number", textBox1.Text);

                            using (var registerReader = selectSorgu.ExecuteReader())
                            {
                                while (registerReader.Read())
                                {
                                    registerSearchStatus = true;

                                    textBox2.Text = registerReader["name"].ToString();
                                    textBox3.Text = registerReader["surname"].ToString();
                                    textBox4.Text = registerReader["username"].ToString();
                                    textBox5.Text = registerReader["password"].ToString();

                                    break;
                                }
                            }
                        }
                    }
                    if (!registerSearchStatus)
                    {
                        MessageBox.Show(
                            "The requested record was not found",
                            "Personal Tracking Automation",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Exclamation
                            );
                    }


                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Hata: {ex.Message}");
                }

            }
            else
            {
                MessageBox.Show(
                    "Please enter your TR number in the correct format.",
                    "Personal Tracking Automation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                    );
            }

        }



        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            bool register_search_status = false;
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");


            if (maskedTextBox1.Text.Length == 11)
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT * FROM personals WHERE tc_number = @tc_number";
                        using (var selectSorgu = new NpgsqlCommand(query, connection))
                        {
                            selectSorgu.Parameters.AddWithValue("@tc_number", long.Parse(maskedTextBox1.Text));
                            using (var registerReader = selectSorgu.ExecuteReader())
                            {
                                while (registerReader.Read())
                                {
                                    register_search_status = true;

                                    maskedTextBox2.Text = registerReader["name"].ToString();
                                    maskedTextBox3.Text = registerReader["surname"].ToString();
                                    maskedTextBox4.Text = registerReader["salary"].ToString();

                                    string gender = registerReader["gender"].ToString();
                                    if (radioButton1 != null)
                                    {
                                        if (gender == "Male")
                                        {
                                            radioButton1.Checked = true;
                                        }
                                        else if (gender == "Female")
                                        {
                                            radioButton2.Checked = true;
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("RadioButton tanımlı değil.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }

                                    comboBox1.Text = registerReader["status"].ToString();
                                    comboBox2.Text = registerReader["mission"].ToString();
                                    comboBox3.Text = registerReader["mission_place"].ToString();

                                    if (DateTime.TryParse(registerReader["date_of_birth"].ToString(), out DateTime date_of_birth))
                                    {
                                        dateTimePicker1.Value = date_of_birth;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Date of birth could not be loaded.", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    }




                                }
                                if(register_search_status == false)
                                {
                                    MessageBox.Show("The requested record was not found", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }


                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured: {ex.Message}", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a correct TR-Number", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegisterUser();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateUser();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DeleteUser();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabPage1_clear();
        }

        private void button6_Click(object sender, EventArgs e)
        {

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

        }

        private void button6_Click_1(object sender, EventArgs e)
        {
            string gender = "";
            bool registerControl = false;

            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    // TC Number'ı long olarak dönüştür
                    if (long.TryParse(maskedTextBox1.Text, out long tcNumber))
                    {
                        NpgsqlCommand selectQuery = new NpgsqlCommand("SELECT * FROM personals WHERE tc_number = @tc_number", connection);
                        selectQuery.Parameters.AddWithValue("@tc_number", tcNumber);

                        using (NpgsqlDataReader registerRead = selectQuery.ExecuteReader())
                        {
                            while (registerRead.Read())
                            {
                                registerControl = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid TC Number!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return; // Geçersiz TC numarası girildiğinde işlemi sonlandırır.
                    }
                }

                if (registerControl)
                {
                    MessageBox.Show("Register already exists!", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (maskedTextBox1.MaskCompleted && maskedTextBox2.MaskCompleted &&
                    maskedTextBox3.MaskCompleted && !string.IsNullOrEmpty(comboBox1.Text) &&
                    !string.IsNullOrEmpty(comboBox2.Text) && !string.IsNullOrEmpty(comboBox3.Text) &&
                    maskedTextBox4.MaskCompleted)
                {
                    gender = radioButton1.Checked ? "Male" : "Female";

                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();
                        string insertQuery = "INSERT INTO personals (tc_number, name, surname, gender, status, date_of_birth, mission, mission_place, salary) " +
                                             "VALUES (@tc, @name, @surname, @gender, @status, @date_of_birth, @mission, @mission_place, @salary)";

                        using (var insertSorgu = new NpgsqlCommand(insertQuery, connection))
                        {
                            // tc_number bigint olduğu için long türünde gönderiyoruz
                            insertSorgu.Parameters.AddWithValue("tc", long.Parse(maskedTextBox1.Text));


                            insertSorgu.Parameters.AddWithValue("name", maskedTextBox2.Text);
                            insertSorgu.Parameters.AddWithValue("surname", maskedTextBox3.Text);
                            insertSorgu.Parameters.AddWithValue("gender", gender);
                            insertSorgu.Parameters.AddWithValue("status", comboBox1.Text);
                            insertSorgu.Parameters.AddWithValue("date_of_birth", dateTimePicker1.Value);
                            insertSorgu.Parameters.AddWithValue("mission", comboBox2.Text);
                            insertSorgu.Parameters.AddWithValue("mission_place", comboBox3.Text);

                            if (int.TryParse(maskedTextBox4.Text, out int salary))
                            {
                                insertSorgu.Parameters.AddWithValue("salary", salary);
                            }
                            else
                            {
                                MessageBox.Show("Invalid salary amount!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return;
                            }

                            insertSorgu.ExecuteNonQuery();
                        }
                        LoadPersonals();
                        tabPAge2_clear();

                    }
                    MessageBox.Show("Registration successful!", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Please fill in all required fields!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }



            }
            catch (Exception ex)
            {
                MessageBox.Show($"Something went wrong: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }



        }

        private void button9_Click(object sender, EventArgs e)
        {
            tabPAge2_clear();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {
            string tc_number = maskedTextBox1.Text;
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (tc_number.Length == 11 && long.TryParse(tc_number, out _))
            {
                try
                {
                    using (var connection = new NpgsqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM personals WHERE tc_number = @tc_number";

                        using (var deleteQuery = new NpgsqlCommand(query, connection))
                        {
                            deleteQuery.Parameters.AddWithValue("@tc_number", long.Parse(tc_number));
                            int rowsfAffected = deleteQuery.ExecuteNonQuery();

                            if(rowsfAffected > 0)
                            {
                                MessageBox.Show("Record deleted successfuly.", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            }
                            else
                            {
                                MessageBox.Show("No record found for the entered TC Number!", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                        }
                        LoadPersonals();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occured: {ex.Message}", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please enter a valid TC Number", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            string tc_number = maskedTextBox1.Text;
            if(tc_number.Length != 11 || !long.TryParse(tc_number, out _))
            {
                MessageBox.Show("Please enter a valid TC number", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(string.IsNullOrWhiteSpace(maskedTextBox1.Text) ||
               string.IsNullOrWhiteSpace(maskedTextBox2.Text) ||
               (!radioButton1.Checked &&  !radioButton2.Checked) ||
                string.IsNullOrWhiteSpace(comboBox1.Text) ||
                string.IsNullOrWhiteSpace(comboBox2.Text) ||
                string.IsNullOrWhiteSpace(comboBox3.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBox4.Text))
                {
                    MessageBox.Show("Please fill in all fields before updating.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

            try
            {
                using(var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();
                    string updatedQuery = "UPDATE personals SET name = @name, surname = @surname, gender = @gender," +
                                          "status = @status, mission = @mission, mission_place = @mission_place, " +
                                          "date_of_birth = @date_of_birth, salary = @salary WHERE tc_number = @tc_number ";
                    using (var cmd = new NpgsqlCommand(updatedQuery, connection))
                    {
                        cmd.Parameters.AddWithValue("@tc_number", long.Parse(tc_number));
                        cmd.Parameters.AddWithValue("@name", maskedTextBox2.Text);
                        cmd.Parameters.AddWithValue("@surname", maskedTextBox3.Text);
                        cmd.Parameters.AddWithValue("@gender", radioButton1.Checked ? "Male" : "Female");
                        cmd.Parameters.AddWithValue("@status", comboBox1.Text);
                        cmd.Parameters.AddWithValue("@mission", comboBox2.Text);
                        cmd.Parameters.AddWithValue("@mission_place", comboBox3.Text);
                        cmd.Parameters.AddWithValue("@date_of_birth", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@salary", int.Parse(maskedTextBox4.Text));

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if(rowsAffected > 0)
                        {
                            MessageBox.Show("Record updated successfuly", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        }
                        else
                        {
                            MessageBox.Show("No record found with the given TC number", "Personal Tracking Automation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
                LoadPersonals();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void PopulateUserComboBox()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT tc_number FROM users ORDER BY tc_number ASC;";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cmbUserTC.Items.Add(reader["tc_number"].ToString());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading TC numbers: {ex.Message}");
            }
        }

        private void AssignTask(string tcNumber, string title, string description, DateTime dueDate)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO tasks (tc_number, task_title, task_description, due_date) VALUES (@tc_number, @task_title, @task_description, @due_date);";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("tc_number", tcNumber);
                        cmd.Parameters.AddWithValue("task_title", title);
                        cmd.Parameters.AddWithValue("task_description", description);
                        cmd.Parameters.AddWithValue("due_date", dueDate);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Task assigned successfully!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while assigning the task: {ex.Message}");
            }
        }


        private void btnRefreshLogs_Click(object sender, EventArgs e)
        {
            LoadLogs();
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }

        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void btnAssignTask_Click(object sender, EventArgs e)
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
            
                if (!long.TryParse(cmbUserTC.Text, out long tcNumber))
                {
                    MessageBox.Show("Please select a valid TC number.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtTaskTitle.Text) || string.IsNullOrWhiteSpace(rtbTaskDescription.Text))
                {
                    MessageBox.Show("Please fill in all the fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO tasks (tc_number, task_title, task_description, due_date) VALUES (@tc_number, @task_title, @task_description, @due_date);";

                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("tc_number", tcNumber); // `long` olarak gönderiliyor
                        cmd.Parameters.AddWithValue("task_title", txtTaskTitle.Text);
                        cmd.Parameters.AddWithValue("task_description", rtbTaskDescription.Text);
                        cmd.Parameters.AddWithValue("due_date", dtpDueDate.Value);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Task assigned successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while assigning the task: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void LoadTasks()
        {
            string connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            try
            {
                using (var connection = new NpgsqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "SELECT task_id, tc_number, task_title, task_description, due_date FROM tasks ORDER BY due_date ASC;";
                    using (var cmd = new NpgsqlCommand(query, connection))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dgvTasks.DataSource = dt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tasks: {ex.Message}");
            }
        }

        private void FormatTaskGridView()
        {
            dgvTasks.Columns["task_id"].Visible = false; // ID'yi gizle
            dgvTasks.Columns["tc_number"].HeaderText = "TC Number";
            dgvTasks.Columns["task_title"].HeaderText = "Task Title";
            dgvTasks.Columns["task_description"].HeaderText = "Description";
            dgvTasks.Columns["due_date"].HeaderText = "Due Date";

            dgvTasks.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void btnRefreshTask_Click(object sender, EventArgs e)
        {
            LoadTasks();
        }
    }
}




