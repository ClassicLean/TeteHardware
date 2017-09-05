﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;



namespace TeteHardware
{
    public partial class formViews : Form
    {
        public formAfterLogin ReferenceToAfterLogin { get; set; } //Reference formEmployeeManage to this form
        public MySqlConnection conn; //connection
        Test func = new Test();
        string myParent = "";
        string myChild = "";
        string myParentTable = "";
        string myChildTable = "";

        public formViews()
        {
            InitializeComponent();
            conn = new MySqlConnection("Server=localhost;Database=tetehardware;Uid=root;Pwd=root"); //connection
            this.Opacity = 0; //form transition using timer
            timer1.Start(); //form transition using timer
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Opacity += 0.1; //form transition using timer
        }

        bool mouseDown; //boolean for mousedown
        Point lastLocation; //variable for the last location of the mouse
        private void formViews_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true; //sets mousedown to true
            lastLocation = e.Location; //gets the location of the form and sets it to lastlocation
        }

        private void formViews_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown) //if mouseDown is true, point to the last location of the mouse
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y); //gets the coordinates of the location of the mouse
                this.Update(); //updates the location of the mouse
            }
        }

        private void formViews_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false; //sets mousedown to false
        }

        //Hot Keys Handling
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F1)
            {
                MessageBox.Show("You pressed the F1 key");
                return true;    // indicate that you handled this keystroke
            }
            else if (keyData == Keys.Escape)     //Close Window
            {
                ReferenceToAfterLogin.Show();
                this.Dispose();
            }
            // Call the base class
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void formViews_Load(object sender, EventArgs e)
        {
            //set Dates
            txtDateFrom.Text=DateTime.Now.ToString();
            txtDateTo.Text= DateTime.Now.ToString();
            populateComboParent();
        }

        private void txtDateFrom_Enter(object sender, EventArgs e)
        {
            monCalFrom.Location = txtDateFrom.Location;
            monCalFrom.Visible = true;
            monCalFrom.Focus();
        }
        private void txtDateTo_Enter(object sender, EventArgs e)
        {
            monCalTo.Location = txtDateTo.Location;
            monCalTo.Visible = true;
            monCalTo.Focus();
        }
        private void monCalFrom_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDateFrom.Text = monCalFrom.SelectionRange.Start.ToShortDateString();
            txtDateTo.Text = monCalFrom.SelectionRange.Start.ToShortDateString();
            monCalFrom.Visible = false;
        }
        private void monCalTo_DateSelected(object sender, DateRangeEventArgs e)
        {
            txtDateTo.Text = monCalTo.SelectionRange.Start.ToShortDateString();
            monCalTo.Visible = false;
        }

        private void populateComboParent()
        {
            comboParent.Items.Clear();
            comboParent.Items.Add("Category");
            comboParent.Items.Add("Supplier");
            comboParent.Items.Add("Product");
            comboParent.Items.Add("Promo");
            comboParent.SelectedIndex = 0;
        }
        private void comboParent_SelectedIndexChanged(object sender, EventArgs e)
        {
            myParent = comboParent.Text;
            populateComboChild(myParent);
            if(myParent=="Category")
            {
                myParentTable = "tbl_Productcatalog";
            }
            else  if (myParent == "Supplier")

            {
                myParentTable = "tbl_Supplier";
            }
            else if (myParent == "Product")

            {
                myParentTable = "tbl_Product";
            }
            else if (myParent == "Promo")

            {
                myParentTable = "tbl_Promo";
            }

            populatedatagridTable("SELECT * FROM " + myParentTable);
        }
        private void populatedatagridTable(string selectCommand)
        {
            datagridTable.DataSource = null;      //remove datasource link for datagridProduct
            try
            {
                conn.Open(); //opens the connection
                MySqlCommand query = new MySqlCommand(selectCommand, conn); //query to select all entries in tbl_productcatalog
                MySqlDataAdapter adp = new MySqlDataAdapter(query); //adapter for query
                DataTable dt = new DataTable(); //datatable for adapter
                BindingSource bs = new BindingSource();
                dt.Locale = System.Globalization.CultureInfo.InvariantCulture;
                adp.Fill(dt);
                bs.DataSource = dt;
                datagridTable.DataSource = bs;
                conn.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show("Error in populating datagridTable : " + x.ToString());
            }
        }

        private void populateComboChild(string myParent)
        {
            comboChild.Items.Clear();
            if (myParent=="Category")
            {
                comboChild.Items.Add("Products");
                comboChild.Items.Add("Sales");
                comboChild.Items.Add("Deliveries");
            }
            else if (myParent=="Supplier")
            {
                comboChild.Items.Add("Products");
                comboChild.Items.Add("Good Deliveries");
                comboChild.Items.Add("Bad Deliveries");
                comboChild.Items.Add("Returned");
            }
            else if (myParent == "Product")
            {
                comboChild.Items.Add("Sales");
                comboChild.Items.Add("Returns");
            }
            else if (myParent == "Promo")
            {
                comboChild.Items.Add("Sales");
            }
        }
        private void comboChild_SelectedIndexChanged(object sender, EventArgs e)
        {
            myChild = comboChild.Text;
            if (myChild == "Products") 
            {
                myChildTable = "tbl_product";
            }
            else if (myChild == "Sales") 
            {
                myChildTable = "tbl_transact";
            }
            else if (myChild == "Deliveries") 
            {
                myChildTable = "tbl_arr";
            }
            else if (myChild == "Good Deliveries") 
            {
                myChildTable = "tbl_arr";
            }
            else if (myChild == "Bad Deliveries")
            {
                myChildTable = "tbl_arrdef";
            }
            else if (myChild == "Returned")
            {

            }
            else if (myChild == "Returns")
            {

            }
            populatedatagridChild("SELECT * FROM " + myChildTable);
        }

        private void populatedatagridChild(string selectCommand)
        {
            datagridTableChild.DataSource = null;      //remove datasource link for datagridProduct
            try
            {
                conn.Open(); //opens the connection
                MySqlCommand query = new MySqlCommand(selectCommand, conn); //query to select all entries in tbl_productcatalog
                MySqlDataAdapter adp = new MySqlDataAdapter(query); //adapter for query
                DataTable dt = new DataTable(); //datatable for adapter
                BindingSource bs1 = new BindingSource();
                dt.Locale = System.Globalization.CultureInfo.InvariantCulture;
                adp.Fill(dt);
                bs1.DataSource = dt;
                datagridTableChild.DataSource = bs1;
                conn.Close();
            }
            catch (Exception x)
            {
                MessageBox.Show("Error in populating datagridTableChild : " + x.ToString());
            }
        }

        private void datagridTable_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            /*            comboParent.Items.Add("Category");
                        comboParent.Items.Add("Supplier");
                        comboParent.Items.Add("Product");
                        comboParent.Items.Add("Promo");
                        */
            int myRowIndex = datagridTable.CurrentRow.Index;
            string myselComm = "";

            if (myParent == "Category")
            {
                string myCatID = "";
                myCatID = datagridTable.Rows[myRowIndex].Cells["catID"].Value.ToString();
                if (myChild == "Products")
                {
                    myselComm = "SELECT * FROM " + myChildTable + " WHERE LEFT(prodID,2) = '" + myCatID + "'";
                }
/*                else if (myChild == "Sales")
                {
                    myselComm = "SELECT * FROM " + myChildTable + " WHERE LEFT(prodID,2) = '" + myCatID + "'";
                }
                else if (myChild == "Deliveries")
                {
                    myselComm = "SELECT * FROM " + myChildTable + " WHERE LEFT(prodID,2) = '" + myCatID + "'";
                }*/
            }
            else if (myParent=="Supplier")
            {
                string mySupID = "";
                mySupID = datagridTable.Rows[myRowIndex].Cells["supID"].Value.ToString();
                if (myChild=="Products")
                {
                    myselComm = "SELECT * FROM " + myChildTable + " WHERE LEFT(supID,2) = '" + mySupID + "'";
                }
/*                else if (myChild == "Good Deliveries")
                {

                }
                else if (myChild == "Bad Deliveries")
                {

                }
                else if (myChild == "Returned")
                {

                }*/
            }
            else if (myParent == "Product")
            {

            }
            else if (myParent == "Promo")
            {

            }
            populatedatagridChild(myselComm);

        }
    }
}