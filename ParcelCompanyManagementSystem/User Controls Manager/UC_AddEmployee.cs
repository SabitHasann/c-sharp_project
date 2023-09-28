﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace ParcelCompanyManagementSystem.User_Controls
{
    public partial class UC_AddEmployee : UserControl
    {
        private DataAccess Da { get; set; }
        private string NewID { get; set; }
        private string NewPassword { get; set; }

        private static Random random = new Random();

        public UC_AddEmployee()
        {
            InitializeComponent();
            this.Da = new DataAccess();
        }

        private bool IsDataValidToSave()
        {
            if (String.IsNullOrWhiteSpace(this.txtName.Text) || String.IsNullOrWhiteSpace(this.txtCellPhone.Text) ||
                String.IsNullOrWhiteSpace(this.txtAddress.Text) || String.IsNullOrWhiteSpace(this.txtEmail.Text) || 
                String.IsNullOrWhiteSpace(this.cmbMaritalStatus.Text) || String.IsNullOrWhiteSpace(this.cmbGender.Text) || 
                String.IsNullOrWhiteSpace(this.cmbRank.Text))
                return false;
            else
                return true;
        }

        private void AutoIdGenerate()
        {
            try
            {
                if (this.cmbRank.Text == "Operator")
                {
                    var query = @"select EmployeeID from Login where EmployeeID like 'O-%' order by EmployeeID desc;";
                    var dt = this.Da.ExecuteQueryTable(query);

                    if(dt.Rows.Count == 0)
                    {
                        var num = 0;
                        this.NewID = "O-" + (++num).ToString("d3");

                    }
                    else
                    {
                        string oldID = dt.Rows[0]["EmployeeID"].ToString();
                        string[] temp = oldID.Split('-');
                        var num = Convert.ToInt32(temp[1]);
                        this.NewID = "O-" + (++num).ToString("d3");
                    }   
                }
                else if (this.cmbRank.Text == "Accountant")
                {
                    var query = @"select EmployeeID from Login where EmployeeID like 'A-%' order by EmployeeID desc;";
                    var dt = this.Da.ExecuteQueryTable(query);

                    if(dt.Rows.Count == 0)
                    {
                        var num = 0;
                        this.NewID = "A-" + (++num).ToString("d3");
                    }
                    else
                    {
                        string oldID = dt.Rows[0]["EmployeeID"].ToString();
                        string[] temp = oldID.Split('-');
                        var num = Convert.ToInt32(temp[1]);
                        this.NewID = "A-" + (++num).ToString("d3");
                    }                   
                }
                else
                {
                    MessageBox.Show("An error has been occured, please try again later.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("An error has been occured, please try again later.\n" + exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AutoPasswordGenerate()
        {
            this.NewPassword = Convert.ToString(random.Next(1000,99999));
        }

        private void ClearAll()
        {
            this.txtName.Clear();
            this.txtCellPhone.Clear();
            this.txtAddress.Clear();
            this.txtEmail.Clear();
            this.cmbMaritalStatus.SelectedIndex = -1;
            this.cmbGender.SelectedIndex = -1;
            this.cmbRank.SelectedIndex = -1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (!IsDataValidToSave())
                {
                    MessageBox.Show("Please fill all the Data Field.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                this.AutoIdGenerate();
                this.AutoPasswordGenerate();

                DialogResult dialogResult = MessageBox.Show("Are you sure want to add this Employee?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if(dialogResult == DialogResult.Yes)
                {
                    var queryEmployee = @"insert into Employee values('" + this.NewID + @"', 
                                        '" + this.txtName.Text + "', '" + this.txtCellPhone.Text + @"', 
                                        '" + this.txtAddress.Text + "', '" + this.txtEmail.Text + @"', 
                                        '" + this.cmbMaritalStatus.Text + "', '" + this.cmbGender.Text + @"', 
                                        '" + this.dtpBirthDate.Text + "', '" + this.dtpJoinDate.Text + "', '" + this.cmbRank.Text + "');";
                    var queryLogin = @"insert into Login values('" + this.NewID + "','" + this.NewPassword + "','" + this.cmbRank.Text + "');";
                    
                    int count = this.Da.ExecuteDMLQuery(queryEmployee);
                    count += this.Da.ExecuteDMLQuery(queryLogin);

                    if (count == 2)
                    {
                        MessageBox.Show("Data Added properly.\nYour Username = " + this.NewID + "\nand Password = " + this.NewPassword + "\nPlease, do remember this and use this for future login.", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.ClearAll();
                    }   
                    else
                        MessageBox.Show("Data insertion failed! Please, try again later.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }

            catch (Exception exc)
            {
                MessageBox.Show("An error has been occured, please try again later.\n" + exc.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UC_AddEmployee_Leave(object sender, EventArgs e)
        {
            this.ClearAll();
        }

        private void txtCellPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }
        }
    }
}
