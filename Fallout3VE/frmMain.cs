using System;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using XLib.Other;

namespace Fallout3VE
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        Fallout3VE fo3;
        bool opened;
        GridHitInfo hitInfo;
        frmSearch frmSe;
        //string xbsave, pcsave;
        
        public frmMain()
        {
            InitializeComponent();
            
            // some nice max values
            numStr.Properties.MaxValue = 10;
            numPer.Properties.MaxValue = 10;
            numEnd.Properties.MaxValue = 10;
            numCha.Properties.MaxValue = 10;
            numInt.Properties.MaxValue = 10;
            numAgi.Properties.MaxValue = 10;
            numLuc.Properties.MaxValue = 10;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            fo3 = new Fallout3VE(this);
            disable_elements();
        }

        private void enable_elements()
        {
            // main form
            numLvl.Enabled = true;
            numXP.Enabled = true;
            numStr.Enabled = true;
            numPer.Enabled = true;
            numEnd.Enabled = true;
            numCha.Enabled = true;
            numInt.Enabled = true;
            numAgi.Enabled = true;
            numLuc.Enabled = true;
            btnMax.Enabled = true;
            numHP.Enabled = true;
            numWeight.Enabled = true;
            numAP.Enabled = true;
            numDR.Enabled = true;
            numRads.Enabled = true;
            numKarma.Enabled = true;
            numSpeed.Enabled = true;
            numCrit.Enabled = true;
            numReload.Enabled = true;
            numThrow.Enabled = true;
            listboxDLC.Enabled = true;

            // inventory editor
            btnFID.Enabled = true;
            btnQty.Enabled = true;
            btnCND.Enabled = true;

            // main menu
            barClose.Enabled = true;
            barSave.Enabled = true;
            barExport.Enabled = true;
            barImportI.Enabled = true;
            barImportS.Enabled = true;

            // toolbar menu
            tbarClose.Enabled = true;
            tbarSave.Enabled = true;
            tbarExport.Enabled = true;
            tbarImportI.Enabled = true;
            tbarImportS.Enabled = true;

            // tabs
            xtraTabPage3.PageEnabled = true;
            xtraTabPage2.PageEnabled = true;
        }

        private void disable_elements()
        {
            // main form
            numLvl.Enabled = false;
            numXP.Enabled = false;
            numStr.Enabled = false;
            numPer.Enabled = false;
            numEnd.Enabled = false;
            numCha.Enabled = false;
            numInt.Enabled = false;
            numAgi.Enabled = false;
            numLuc.Enabled = false;
            btnMax.Enabled = false;
            numHP.Enabled = false;
            numWeight.Enabled = false;
            numAP.Enabled = false;
            numDR.Enabled = false;
            numRads.Enabled = false;
            numKarma.Enabled = false;
            numSpeed.Enabled = false;
            numCrit.Enabled = false;
            numReload.Enabled = false;
            numThrow.Enabled = false;
            listboxDLC.Enabled = false;

            // inventory editor
            btnFID.Enabled = false;
            btnQty.Enabled = false;
            btnCND.Enabled = false;
            btnUpdate.Enabled = false;

            // main menu
            barClose.Enabled = false;
            barSave.Enabled = false;
            barExport.Enabled = false;
            barImportI.Enabled = false;
            barImportS.Enabled = false;

            // toolbar menu
            tbarClose.Enabled = false;
            tbarSave.Enabled = false;
            tbarExport.Enabled = false;
            tbarImportI.Enabled = false;
            tbarImportS.Enabled = false;

            // tabs
            xtraTabPage3.PageEnabled = false;
            xtraTabPage2.PageEnabled = false;
        }


        private void btnMax_Click(object sender, EventArgs e)
        {
            numStr.Value = 10;
            numPer.Value = 10;
            numEnd.Value = 10;
            numCha.Value = 10;
            numInt.Value = 10;
            numAgi.Value = 10;
            numLuc.Value = 10;
        }

        private void numLvl_EditValueChanged(object sender, EventArgs e)
        {
            // 66699 max xp
            int lvl = int.Parse(numLvl.Value.ToString());
            if (lvl > 0)
            {
                float xp = (25 * ((3 * lvl) + 2) * (lvl - 1)) - 1;
                numXP.Value = (decimal)xp;
            }
        }

        private void numXP_EditValueChanged(object sender, EventArgs e)
        {

        }

        public void progressbar_inc()
        {
            pBar.Increment(1);
            pBar.Update();
        }
        
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            btnUpdate.Enabled = true;

            btnFID.Enabled = true;
            btnQty.Enabled = true;
            btnCND.Enabled = true;

            hitInfo = gridView1.CalcHitInfo(gridControl1.PointToClient(Control.MousePosition));
            if (hitInfo.HitTest == GridHitTest.RowCell)
            {
                // get parts
                object offset = gridView1.GetRowCellValue(hitInfo.RowHandle, "offset");
                object formID = gridView1.GetRowCellValue(hitInfo.RowHandle, "fid");
                object quantity = gridView1.GetRowCellValue(hitInfo.RowHandle, "qty");
                object condition = gridView1.GetRowCellValue(hitInfo.RowHandle, "cdn");
                
                // set for edit
                if (int.Parse(quantity.ToString()) == -1)
                    btnQty.Enabled = false;

                if (int.Parse(condition.ToString()) == -1)
                    btnCND.Enabled = false;

                btnFID.Text = string.Format("{0:X8}", formID);
                btnQty.Text = quantity.ToString();
                btnCND.Text = condition.ToString();
            }

        }

        private void btnFID_MouseClick(object sender, MouseEventArgs e)
        {  
            btnFID.EditValue = Clipboard.GetText();    
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!hitInfo.IsValid)
            {
                XtraMessageBox.Show("Oops it looks like the sky is falling! or maybe you just forgot to select and item to swap.", "Error");
            }
            
            if (hitInfo.HitTest == GridHitTest.RowCell)
            {
                // get parts
                object offset = gridView1.GetRowCellValue(hitInfo.RowHandle, "offset");
                object formID = gridView1.GetRowCellValue(hitInfo.RowHandle, "fid");

                int ifid = BitConverter.ToInt32(ByteFunctions.SwapEndian(ByteFunctions.HexToBytes(btnFID.Text)), 0);
                int iqty = btnQty.Enabled ? int.Parse(btnQty.Text) : -1;
                int icdn = btnCND.Enabled ? int.Parse(btnCND.Text) : -1;

                // update the saves
                if ((int)formID != ifid)
                    fo3.update_item_fid((int)formID, ifid);
                
                if (iqty != -1)
                    fo3.update_item_qty((int)offset, iqty);

                if (icdn != -1)
                    fo3.update_item_cdn((int)offset, icdn);

                // update the display
                gridView1.SetRowCellValue(hitInfo.RowHandle, "fid", ifid);
                gridView1.SetRowCellValue(hitInfo.RowHandle, "qty", iqty);
                gridView1.SetRowCellValue(hitInfo.RowHandle, "cdn", icdn);

                // re-query the item
                gridView1.SetRowCellValue(hitInfo.RowHandle, "type", fo3.db.get_type(ifid));
                gridView1.SetRowCellValue(hitInfo.RowHandle, "descrip", fo3.db.get_description(ifid));
            }
        }

        private void barOpen_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            using (OpenFileDialog dlg = new OpenFileDialog { Filter = "FalloutNV (*.fxs)|*.fxs|" + "All files (*.*)|*.*" })
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    fo3.load_file(dlg.FileName);
                    fo3.load_database();
                    fo3.load_fidtable();
                    fo3.load_stats();
                    fo3.load_skills();
                    fo3.load_items();
                    opened = true;
                    pBar.EditValue = 0;
                    barStatus.Caption = "Status: Ready";
                    Text = "FalloutNVVE - " + dlg.FileName;
                    enable_elements();
                }
            }
        }

        private void barExit_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            Application.Exit();
        }

        private void barSearch_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            if (opened && fo3.get_loadorder().Length > 0)
            {
                frmSe = new frmSearch(fo3.get_loadorder());
            }
            else if (opened && fo3.get_loadorder().Length <= 0)
            {
                XtraMessageBox.Show("It appears that you have no addons installed, while you can still seach the database please keep in mind that addon's and DLC FormIDs will not be correct", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                frmSe = new frmSearch(false);
            }
            else
            {
                XtraMessageBox.Show("You've opened the database search without loading a file, while you can still seach the database please keep in mind that addon's and DLC FormIDs will not be correct", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                frmSe = new frmSearch(false);
            }

            frmSe.Show();
        }

        private void barSave_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fo3.save_file();
        }

        private void barAbout_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            new frmAbout().Show();
        }

        private void barHelp_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.zeropair.com");
        }

        private void barClose_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            txtName.Text = "";
            txtLoc.Text = "";
            txtKarma.Text = "";
            txtTime.Text = "";

            numLvl.Value = 0;
            numXP.Value = 0;

            numStr.Value = 0;
            numPer.Value = 0;
            numEnd.Value = 0;
            numCha.Value = 0;
            numInt.Value = 0;
            numAgi.Value = 0;
            numLuc.Value = 0;

            Text = "FalloutNV";
            gridControl1.DataSource = null;

            if (frmSe != null)
                frmSe.Dispose();

            listboxDLC.Items.Clear();

            disable_elements();
        }

        private void barExport_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            FolderBrowserDialog fdlg = new FolderBrowserDialog();
            if (fdlg.ShowDialog() == DialogResult.OK)
                fo3.export(fdlg.SelectedPath);

            XtraMessageBox.Show("Game successfully exported.", "Success");
        }

        private void barImportS_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog odlg = new OpenFileDialog();
            if (odlg.ShowDialog() == DialogResult.OK)
                fo3.import_special(odlg.FileName);

            XtraMessageBox.Show("Successfully imported S.P.E.C.I.A.L.", "Success");
        }

        private void barImportI_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            OpenFileDialog odlg = new OpenFileDialog();
            if (odlg.ShowDialog() == DialogResult.OK)
                fo3.import_inventory(odlg.FileName);

            XtraMessageBox.Show("Successfully imported item inventory.", "Success");
        }

        private void tbarPC2XBOX_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fo3.pc_to_xbox360();
        }

        private void tbarXBOX2PC_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            fo3.xbox360_to_pc();
        }

        private void simpleButton1_Click(object sender, EventArgs e)
        {
            numBarter.Value = 100;
            numEWeapons.Value = 100;
            numExplosives.Value = 100;
            numGuns.Value = 100;
            numLockpick.Value = 100;
            numMedicine.Value = 100;
            numMelee.Value = 100;
            numRepair.Value = 100;
            numScience.Value = 100;
            numSneak.Value = 100;
            numSpeech.Value = 100;
            numSurvival.Value = 100;
            numUnarmed.Value = 100;
        }

        private void cbtnLink_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void barUpdate_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            // string with ver

        }
    }
}