using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Browser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SQLiteHandler.GetInstance().CoonectToDb("Keywords.db");
        }

        private async Task<bool> IsBlocked(string url)
        {
            List<string> keywords = await SQLiteHandler.GetInstance().GetAllKeywords();

            foreach (string keyword in keywords)
            {
                if (url.ToLower().Contains(keyword.ToLower()))
                {
                    return true;
                }
            }

            return false;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string url = SearchBar.Text;

            if (await IsBlocked(url))
            {
                MessageBox.Show("Site blocked", "Warning");
            }
            else
            {
                webBrowser1.Navigate(url);
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            webBrowser1.GoHome();
        }

        //orice browser are nevoie de buton de refresh
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            webBrowser1.Refresh();
        }

        private async void SearchBar_KeyDown(object sender, KeyEventArgs e)
        {
            string url = SearchBar.Text;
            if (e.KeyCode == Keys.Enter)
            {
                if (await IsBlocked(url))
                {
                    MessageBox.Show("Site blocked", "Warning");
                }
                else
                {
                    webBrowser1.Navigate(url);
                }
            }

        }

        // partea asta nu stiu daca asa trebuia facuta dar mi s-a parut cel mai usor mod
        private async void menuKeywordViewDelete_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.Text = "View / Delete Keywords";
            f.Width = 300;
            f.Height = 180;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;

            Label lbl = new Label();
            lbl.BackColor = Color.DarkSeaGreen;
            lbl.Text = "Keywords:";
            lbl.Left = 10;
            lbl.Top = 19;
            lbl.Width = 70;
            lbl.Height = 19;

            ComboBox combo = new ComboBox();
            combo.Left = 90;
            combo.Top = 18;
            combo.Width = 170;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;

            Button btnDelete = new Button();
            btnDelete.BackColor = Color.Green;
            btnDelete.Text = "Delete";
            btnDelete.Left = 90;
            btnDelete.Top = 60;
            btnDelete.Width = 90;

            Button btnCancel = new Button();
            btnCancel.BackColor = Color.Red;
            btnCancel.Text = "Cancel";
            btnCancel.Left = 200;
            btnCancel.Top = 60;
            btnCancel.Width = 60;

            List<string> keywords = await SQLiteHandler.GetInstance().GetAllKeywords();
            foreach (string keyword in keywords)
            {
                combo.Items.Add(keyword);
            }
            // functie lamba cool, adauga event handler nou (functie inline?)
            btnDelete.Click += (s, ev) =>
            {
                if (combo.SelectedItem == null)
                {
                    MessageBox.Show("Choose a keyword");
                    return;
                }

                string k = combo.SelectedItem.ToString();

                SQLiteHandler.GetInstance().DeleteKeyword(k);

                MessageBox.Show("Keyword deleted");
                combo.Items.Remove(k);
            };

            btnCancel.Click += (s, ev) =>
            {
                f.Close();
            };

            f.Controls.Add(lbl);
            f.Controls.Add(combo);
            f.Controls.Add(btnDelete);
            f.Controls.Add(btnCancel);

            f.ShowDialog();
        }

        private void menuKeywordAdd_Click(object sender, EventArgs e)
        {
            Form f = new Form();
            f.Text = "Add Keyword";
            f.Width = 300;
            f.Height = 180;
            f.FormBorderStyle = FormBorderStyle.FixedDialog;
            f.MaximizeBox = false;

            Label lbl = new Label();
            lbl.BackColor = Color.DarkSeaGreen;
            lbl.Text = "Keyword:";
            lbl.Left = 10;
            lbl.Top = 20;
            lbl.Width = 70;
            lbl.Height = 16;

            TextBox txt = new TextBox();
            txt.Left = 90;
            txt.Top = 18;
            txt.Width = 170;

            Button btnOk = new Button();
            btnOk.BackColor = Color.Green;
            btnOk.Text = "OK";
            btnOk.Left = 90;
            btnOk.Top = 60;
            btnOk.Width = 60;

            Button btnCancel = new Button();
            btnCancel.BackColor = Color.Red;
            btnCancel.Text = "Cancel";
            btnCancel.Left = 200;
            btnCancel.Top = 60;
            btnCancel.Width = 60;

            btnOk.Click += (s, ev) =>
            {
                string keyword = txt.Text.Trim();

                if (keyword == "")
                {
                    MessageBox.Show("Enter a keyword");
                    return;
                }

                if (keyword.Length > 64)
                {
                    MessageBox.Show("Keyword too long");
                    return;
                }

                if (SQLiteHandler.GetInstance().KeywordExists(keyword))
                {
                    MessageBox.Show("Already exists");
                    return;
                }

                SQLiteHandler.GetInstance().AddKeyword(keyword);

                MessageBox.Show("Keyword added");
            };

            btnCancel.Click += (s, ev) =>
            {
                f.Close();
            };

            f.Controls.Add(lbl);
            f.Controls.Add(txt);
            f.Controls.Add(btnOk);
            f.Controls.Add(btnCancel);

            f.ShowDialog();
        }
    }
}
