using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using DVLD_BusinessLayer;

namespace DVLD_PresentationLayer
{
    public partial class ucPeople : UserControl
    {
        public ucPeople()
        {
            InitializeComponent();
        }

        private void ucPeople_Load(object sender, EventArgs e)
        {
            DataTable _dtAllPeople = clsPerson.GetPeople();
            if (_dtAllPeople != null)
            {
                _dtAllPeople.Columns.Add("PERSON", typeof(string), "FirstName + ' ' + SecondName + ' ' + ThirdName + ' ' + LastName");
            }

            _dtAllPeople.Columns.Add("GenderText", typeof(string));
            foreach (DataRow row in _dtAllPeople.Rows)
            {
                string originalGender = row["Gendor"].ToString().Trim();

                if (originalGender == "0" || originalGender == "Male")
                {
                    row["GenderText"] = "Male";
                }
                else
                {
                    row["GenderText"] = "Female";
                }
            }

            guna2DataGridView1.DataSource = _dtAllPeople;
            if (guna2DataGridView1.Rows.Count > 0)
            {
                guna2DataGridView1.Columns["PersonID"].Visible = false;
                guna2DataGridView1.Columns["FirstName"].Visible = false;
                guna2DataGridView1.Columns["SecondName"].Visible = false;
                guna2DataGridView1.Columns["ThirdName"].Visible = false;
                guna2DataGridView1.Columns["LastName"].Visible = false;
                guna2DataGridView1.Columns["Gendor"].Visible = false;
                guna2DataGridView1.Columns["GenderText"].HeaderText = "Gender";
                guna2DataGridView1.Columns["GenderText"].DisplayIndex = 1;
                guna2DataGridView1.Columns["NationalNo"].HeaderText = "NATIONAL ID";
                guna2DataGridView1.Columns["DateOfBirth"].DefaultCellStyle.Format = "MMM dd, yyyy";
                guna2DataGridView1.Columns["DateOfBirth"].HeaderText = "DATE OF BIRTH";
                guna2DataGridView1.Columns["Person"].DisplayIndex = 0;
                guna2DataGridView1.Columns["Person"].Width = 250;

            }
        }
    }
}
