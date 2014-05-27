using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Fallout3VE.Database;
using Fallout3VE.SaveGame;

namespace Fallout3VE
{
    public partial class frmSearch : DevExpress.XtraEditors.XtraForm
    {
        DatabaseClass sdb;
        bool no_load = true;
        string[] order;

        public frmSearch()
        {
            InitializeComponent();
        }

        public frmSearch(string[] lo)
        {
            InitializeComponent();
            order = lo;
        }

        public frmSearch(bool nol)
        {
            InitializeComponent();
            no_load = nol;
        }

        public void frmLoad(string[] lo)
        {
            InitializeComponent();
            order = lo;
        }

        public void frmLoad(bool nol)
        {
            InitializeComponent();
            no_load = nol;
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            // diable em
            barCheckItem2.Enabled = false;
            barCheckItem3.Enabled = false;
            barCheckItem4.Enabled = false;
            barCheckItem5.Enabled = false;
            barCheckItem6.Enabled = false;

            if (no_load)
            {
                foreach (string str in order)
                {
                    switch (str)
                    {
                        case "TribalPack.esm":
                            barCheckItem2.Enabled = true;
                            barCheckItem2.Checked = true;
                            break;

                        case "ClassicPack.esm":
                            barCheckItem3.Enabled = true;
                            barCheckItem3.Checked = true;
                            break;

                        case "CaravanPack.esm":
                            barCheckItem4.Enabled = true;
                            barCheckItem4.Checked = true;
                            break;

                        case "MercenaryPack.esm":
                            barCheckItem5.Enabled = true;
                            barCheckItem5.Checked = true;
                            break;

                        case "DeadMoney.esm":
                            barCheckItem6.Enabled = true;
                            barCheckItem6.Checked = true;
                            break;
                    }

                    sdb = new DatabaseClass(order);
                }
            }
            else
            {
                sdb = new DatabaseClass();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {   
            if (lbFoundItems.Items.Count > 0)
                lbFoundItems.Items.Clear();

            if (txtSearchText.Text.Length > 0)
            {
                if (cbRegex.Checked)
                {
                    // do regex search
                    List<string[]> found = sdb.item_search_regex(txtSearchText.Text);

                    if (found.Count > 0)
                        foreach (string[] fitem in found)
                            lbFoundItems.Items.Add(String.Format("{0}:\t0x{1}\t{2}", fitem[0], fitem[1].Replace("(", "").Replace(")", ""), fitem[2]));
                    else
                        XtraMessageBox.Show("No items found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    // do normal search
                    List<string[]> found = sdb.item_search(txtSearchText.Text);

                    if (found.Count > 0)
                        foreach (string[] fitem in found)
                            lbFoundItems.Items.Add(String.Format("{0}:\t0x{1}\t{2}", fitem[0], fitem[1].Replace("(", "").Replace(")", ""), fitem[2]));
                    else
                        XtraMessageBox.Show("No items found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void lbFoundItems_DoubleClick(object sender, EventArgs e)
        {
            if (lbFoundItems.SelectedItem != null)
            {
                string[] parts = lbFoundItems.SelectedItem.ToString().Split('\t');
                Clipboard.SetText(parts[1].Remove(1, 2));
            }
        }

        private void txtSearchText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar == (char)13))
            {
                btnSearch_Click(null, null);
            }
        }
        
    }
}