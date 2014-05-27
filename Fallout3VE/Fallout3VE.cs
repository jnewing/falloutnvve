using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Forms;
using Fallout3VE.Database;
using Fallout3VE.Items;
using Fallout3VE.Offset;
using Fallout3VE.SaveGame;
using Fallout3VE.FormID;
using XLib.Other;
using XLib.Converters;
using XLib.STFS;
using DevExpress.XtraEditors;

namespace Fallout3VE
{
   
    class Fallout3VE
    {
        frmMain _form;
        
        public DatabaseClass db;
        OffsetsClass offsets;
        Savegame savegame;
        FormIDTable fid;
        ItemsClass items;
        string file, tmpfile;
        STFS xpack;
        
        int[] lookup_table;

        public Fallout3VE() { }
        ~Fallout3VE() 
        {
            if (File.Exists(tmpfile))
            {
                File.Delete(tmpfile);
            }
        }

        public Fallout3VE(frmMain frm)
        {
            _form = frm;
            
            // check the read me file to piss off x
            /**
             * SHA1CryptoServiceProvider shash = new SHA1CryptoServiceProvider();
             * FileStream checkf = new FileStream(Application.StartupPath + "\\readme.txt", FileMode.Open, FileAccess.Read);
             * byte[] check_buffer = shash.ComputeHash(checkf);
             * 
             * if (Convert.ToBase64String(check_buffer) != "xIcaxeHX9pNcHVEv7ax+eSvMiFQ=")
             * {
             *      XtraMessageBox.Show("File corruption, please re-download from http://zeropair.com", "Sanity Check: Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
             *      Application.Exit();
             *  }
             */
        }
        
        public void load_database()
        {
            db = new DatabaseClass(savegame.savegame_loadorder);
        }

        public string[] get_loadorder()
        {
            return savegame.savegame_loadorder;
        }

        public bool load_file(string filename)
        {
            _form.barStatus.Caption = "Status: Reading...";
            _form.pBar.Properties.Maximum = 5;

            // setup file and tempfile
            tmpfile = Path.GetTempFileName();
            file = filename;

            _form.progressbar_inc();

            FileStream fsOut = new FileStream(tmpfile, FileMode.Create, FileAccess.Write);

            // get & witeout clean file data
            xpack = new STFS(filename);

            byte[] cleanbuffer = xpack.extractFile();
            _form.progressbar_inc();
            
            fsOut.Write(cleanbuffer, 0, cleanbuffer.Length);
            fsOut.Close();
            _form.progressbar_inc();

            offsets = new OffsetsClass(tmpfile);
            _form.progressbar_inc();

            savegame = new Savegame(tmpfile, offsets);
            _form.progressbar_inc();

            _form.barStatus.Caption = "Status: Done";

            return true;
        }

        public void load_fidtable()
        {
            _form.barStatus.Caption = "Status: FormID...";
            _form.pBar.Properties.Maximum = 2;

            fid = new FormIDTable(savegame.savegame_fidtable);
            _form.progressbar_inc();

            lookup_table = fid.fid_table;
            _form.progressbar_inc();

            _form.barStatus.Caption = "Status: Done";
        }

        public void load_skills()
        {
            _form.barStatus.Caption = "Status: Skills...";
            _form.pBar.Properties.Maximum = 1;

            // load skills to interface
            _form.numBarter.Value = (int)Conv.ToSingle(savegame.savegame_player_barter, false);
            _form.numEWeapons.Value = (int)Conv.ToSingle(savegame.savegame_player_eweapons, false);
            _form.numExplosives.Value = (int)Conv.ToSingle(savegame.savegame_player_explo, false);
            _form.numGuns.Value = (int)Conv.ToSingle(savegame.savegame_player_guns, false);
            _form.numLockpick.Value = (int)Conv.ToSingle(savegame.savegame_player_lockpick, false);
            _form.numMedicine.Value = (int)Conv.ToSingle(savegame.savegame_player_medicine, false);
            _form.numMelee.Value = (int)Conv.ToSingle(savegame.savegame_player_melee, false);
            _form.numRepair.Value = (int)Conv.ToSingle(savegame.savegame_player_repair, false);
            _form.numScience.Value = (int)Conv.ToSingle(savegame.savegame_player_science, false);
            _form.numSneak.Value = (int)Conv.ToSingle(savegame.savegame_player_sneak, false);
            _form.numSpeech.Value = (int)Conv.ToSingle(savegame.savegame_player_speech, false);
            _form.numSurvival.Value = (int)Conv.ToSingle(savegame.savegame_player_survival, false);
            _form.numUnarmed.Value = (int)Conv.ToSingle(savegame.savegame_player_unarmed, false);
        }

        public void load_stats()
        {
            _form.barStatus.Caption = "Status: Stats...";
            _form.pBar.Properties.Maximum = 4;
            
            // load stats
            _form.txtName.Text = Encoding.ASCII.GetString(savegame.savegame_player_name);
            _form.txtLoc.Text = Encoding.ASCII.GetString(savegame.savegame_player_location);
            _form.txtKarma.Text = Encoding.ASCII.GetString(savegame.savegame_player_karma);
            _form.txtTime.Text = Encoding.ASCII.GetString(savegame.savegame_player_time);
            _form.progressbar_inc();

            _form.numLvl.Value = Conv.ToInt32(savegame.savegame_player_level, false);
            _form.progressbar_inc();

            // load special
            if (Conv.ToInt16(savegame.savegame_special_str, true) > 10 ||
                Conv.ToInt16(savegame.savegame_special_per, true) > 10 ||
                Conv.ToInt16(savegame.savegame_special_end, true) > 10 ||
                Conv.ToInt16(savegame.savegame_special_cha, true) > 10 ||
                Conv.ToInt16(savegame.savegame_special_int, true) > 10 ||
                Conv.ToInt16(savegame.savegame_special_agi, true) > 10 ||
                Conv.ToInt16(savegame.savegame_special_luc, true) > 10)
            {
                if (XtraMessageBox.Show("It looks as though your S.P.E.C.I.A.L. data is corrupt or missing, I can try to read the rest of the file if you like, however S.P.E.C.A.I.L. stats editing will be disabled.", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Hand) == DialogResult.OK)
                {
                    // yes was cliked
                    _form.numStr.Enabled = false;
                    _form.numPer.Enabled = false;
                    _form.numEnd.Enabled = false;
                    _form.numCha.Enabled = false;
                    _form.numInt.Enabled = false;
                    _form.numAgi.Enabled = false;
                    _form.numLuc.Enabled = false;
                    _form.progressbar_inc();
                }
                else
                {
                    Application.Exit();
                }
            }
            else
            {
                _form.numStr.Value = Conv.ToInt16(savegame.savegame_special_str, true);
                _form.numPer.Value = Conv.ToInt16(savegame.savegame_special_per, true);
                _form.numEnd.Value = Conv.ToInt16(savegame.savegame_special_end, true);
                _form.numCha.Value = Conv.ToInt16(savegame.savegame_special_cha, true);
                _form.numInt.Value = Conv.ToInt16(savegame.savegame_special_int, true);
                _form.numAgi.Value = Conv.ToInt16(savegame.savegame_special_agi, true);
                _form.numLuc.Value = Conv.ToInt16(savegame.savegame_special_luc, true);
                _form.progressbar_inc();

            }

            // load xp
            _form.numXP.Value = (int)BitConverter.ToSingle(savegame.savegame_player_exp, 0);

            // load extra carry weight
            if (savegame.savegame_player_weight.Length > 0)
                _form.numWeight.Value = (int)BitConverter.ToSingle(savegame.savegame_player_weight, 0);

            // load exta hp
            if (savegame.savegame_player_hp.Length > 0)
                _form.numHP.Value = (int)BitConverter.ToSingle(savegame.savegame_player_hp, 0);

            // load ap
            if (savegame.savegame_player_ap.Length > 0)
                _form.numAP.Value = (int)BitConverter.ToSingle(savegame.savegame_player_ap, 0);

            // load dmg res
            if (savegame.savegame_player_dmg.Length > 0)
                _form.numDR.Value = (int)BitConverter.ToSingle(savegame.savegame_player_dmg, 0);

            // load karma
            if (savegame.savegame_player_karma_num.Length > 0)
                _form.numKarma.Value = (int)BitConverter.ToSingle(savegame.savegame_player_karma_num, 0);

            // load rad res
            if (savegame.savegame_player_radres.Length > 0)
                _form.numRads.Value = (int)BitConverter.ToSingle(savegame.savegame_player_radres, 0);

            // load speed
            if (savegame.savegame_player_speed.Length > 0)
                _form.numSpeed.Value = (int)BitConverter.ToSingle(savegame.savegame_player_speed, 0);

            // crit?
            if (savegame.savegame_player_crit.Length > 0)
                _form.numCrit.Value = (int)BitConverter.ToSingle(savegame.savegame_player_crit, 0);

            // reload
            if (savegame.savegame_player_reload.Length > 0)
                _form.numReload.Value = (int)BitConverter.ToSingle(savegame.savegame_player_reload, 0);

            // throw
            if (savegame.savegame_player_throw.Length > 0)
                _form.numThrow.Value = (int)BitConverter.ToSingle(savegame.savegame_player_throw, 0);

            // player size
            if (savegame.savegame_player_size.Length > 0)
                _form.numSize.Value = (int)BitConverter.ToSingle(savegame.savegame_player_size, 0);

            // player h20
            if (savegame.savegame_player_h20.Length > 0)
                _form.numH2O.Value = (int)BitConverter.ToSingle(savegame.savegame_player_h20, 0);

            // player sleep
            if (savegame.savegame_player_sleep.Length > 0)
                _form.numSleep.Value = (int)BitConverter.ToSingle(savegame.savegame_player_sleep, 0);

            // player DT
            if (savegame.savegame_player_dmgt.Length > 0)
                _form.numDT.Value = (int)BitConverter.ToSingle(savegame.savegame_player_dmgt, 0);

            // dlc stuff
            _form.txtDLC.Text = savegame.load_seq.Length.ToString();
            for (int x = 0; x < savegame.load_seq.Length; x++)
            {
                _form.listboxDLC.Items.Add("[" + x.ToString("D2") + "] " + savegame.load_seq[x]);
            }

            _form.progressbar_inc();
            _form.barStatus.Caption = "Status: Done";
        }

        public void load_items()
        {   
            // load items
            items = new ItemsClass(savegame.savegame_pinventory, offsets.inventory_fitem, lookup_table);

            _form.pBar.Properties.Maximum = items.inventory.Count;
            _form.barStatus.Caption = "Status: Inventory...";
            _form.Update();

            ArrayList datalist = new ArrayList();
            foreach (SGItems item in items.inventory)
            {
                _form.pBar.Increment(1);
                _form.pBar.Update();
                datalist.Add(new SGItems(db.get_type(item.fid), item.raw, item.extra, item.offset, item.sid, item.fid, item.old_fid, item.qty, item.cdn, db.get_description(item.fid)));
            }

            _form.gridControl1.DataSource = datalist;
        }

        public void make_backup(string filename)
        {
            _form.barStatus.Caption = "Backup...";
            _form.pBar.Properties.Maximum = 2;

            // make file backup
            if (!Directory.Exists(Application.StartupPath + "\\backups"))
                Directory.CreateDirectory(Application.StartupPath + "\\backups");

            _form.progressbar_inc();

            string[] parts = filename.Split('\\');
            File.Copy(filename, String.Format("{0}\\backups\\{1}", Application.StartupPath, parts[parts.Length - 1]), true);

            _form.progressbar_inc();
            _form.barStatus.Caption = "Status: Done";
        }
        
        public void save_file()
        {
            _form.barStatus.Caption = "Saving...";
            _form.pBar.Properties.Maximum = 4;

            // try and update all the S.P.E.C.I.A.L. stats
            byte[] special_array = new byte[0x7];
            byte[] buffer;

            try
            {
                // str
                buffer = BitConverter.GetBytes((short)_form.numStr.Value);
                special_array[0] = buffer[0];

                // per
                buffer = BitConverter.GetBytes((short)_form.numPer.Value);
                special_array[1] = buffer[0];

                // end
                buffer = BitConverter.GetBytes((short)_form.numEnd.Value);
                special_array[2] = buffer[0];

                // cha
                buffer = BitConverter.GetBytes((short)_form.numCha.Value);
                special_array[3] = buffer[0];

                // int
                buffer = BitConverter.GetBytes((short)_form.numInt.Value);
                special_array[4] = buffer[0];

                // agi
                buffer = BitConverter.GetBytes((short)_form.numAgi.Value);
                special_array[5] = buffer[0];

                // luck
                buffer = BitConverter.GetBytes((short)_form.numLuc.Value);
                special_array[6] = buffer[0];

                savegame.savegame_update(offsets.special_str, special_array);
                _form.progressbar_inc();

            } 
            catch(System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // try and update the players skills
            try
            {
                float barter = float.Parse(_form.numBarter.Value.ToString());
                savegame.savegame_update(offsets.barter, BitConverter.GetBytes(barter));

                float eweapons = float.Parse(_form.numEWeapons.Value.ToString());
                savegame.savegame_update(offsets.eweapons, BitConverter.GetBytes(eweapons));

                float explo = float.Parse(_form.numExplosives.Value.ToString());
                savegame.savegame_update(offsets.explo, BitConverter.GetBytes(explo));

                float guns = float.Parse(_form.numGuns.Value.ToString());
                savegame.savegame_update(offsets.guns, BitConverter.GetBytes(guns));

                float lockpick = float.Parse(_form.numLockpick.Value.ToString());
                savegame.savegame_update(offsets.lockpick, BitConverter.GetBytes(lockpick));

                float medicine = float.Parse(_form.numMedicine.Value.ToString());
                savegame.savegame_update(offsets.medicine, BitConverter.GetBytes(medicine));

                float melee = float.Parse(_form.numMelee.Value.ToString());
                savegame.savegame_update(offsets.melee, BitConverter.GetBytes(melee));

                float repair = float.Parse(_form.numRepair.Value.ToString());
                savegame.savegame_update(offsets.repair, BitConverter.GetBytes(repair));

                float science = float.Parse(_form.numScience.Value.ToString());
                savegame.savegame_update(offsets.science, BitConverter.GetBytes(science));

                float sneak = float.Parse(_form.numSneak.Value.ToString());
                savegame.savegame_update(offsets.sneak, BitConverter.GetBytes(sneak));

                float speech = float.Parse(_form.numSpeech.Value.ToString());
                savegame.savegame_update(offsets.speech, BitConverter.GetBytes(speech));

                float survival = float.Parse(_form.numSurvival.Value.ToString());
                savegame.savegame_update(offsets.survival, BitConverter.GetBytes(survival));

                float unarmed = float.Parse(_form.numUnarmed.Value.ToString());
                savegame.savegame_update(offsets.unarmed, BitConverter.GetBytes(unarmed));
            }
            catch (System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // try and update the players stats hp, ap, wg, dr, rrad, karma
            try
            {
                // update wg
                float weight = float.Parse(_form.numWeight.Value.ToString());
                savegame.savegame_update(offsets.cweight, BitConverter.GetBytes(weight));

                // update hp
                float health = float.Parse(_form.numHP.Value.ToString());
                savegame.savegame_update(offsets.health, BitConverter.GetBytes(health));

                // update ap
                float ap = float.Parse(_form.numAP.Value.ToString());
                savegame.savegame_update(offsets.actionpoints, BitConverter.GetBytes(ap));

                // update dr
                float dr = float.Parse(_form.numDR.Value.ToString());
                savegame.savegame_update(offsets.damage_res, BitConverter.GetBytes(dr));

                // update rad resistance
                float rads = float.Parse(_form.numRads.Value.ToString());
                savegame.savegame_update(offsets.rad_resist, BitConverter.GetBytes(rads));

                // update karma num
                float karma = float.Parse(_form.numKarma.Value.ToString());
                savegame.savegame_update(offsets.karma, BitConverter.GetBytes(karma));

                // update crit
                float crit = float.Parse(_form.numCrit.Value.ToString());
                savegame.savegame_update(offsets.crit, BitConverter.GetBytes(crit));

                // update speed
                float speed = float.Parse(_form.numSpeed.Value.ToString());
                savegame.savegame_update(offsets.player_speed, BitConverter.GetBytes(speed));

                // update reload
                float reload = float.Parse(_form.numReload.Value.ToString());
                savegame.savegame_update(offsets.player_reload, BitConverter.GetBytes(reload));

                // update throw
                float throwd = float.Parse(_form.numThrow.Value.ToString());
                savegame.savegame_update(offsets.player_throw, BitConverter.GetBytes(throwd));

                // update player size
                float psize = float.Parse(_form.numSize.Value.ToString());
                savegame.savegame_update(offsets.player_size, BitConverter.GetBytes(psize));

                // update player h20
                float ph20 = float.Parse(_form.numH2O.Value.ToString());
                savegame.savegame_update(offsets.player_h20, BitConverter.GetBytes(ph20));

                // update player sleep
                float psleep = float.Parse(_form.numSleep.Value.ToString());
                savegame.savegame_update(offsets.player_sleep, BitConverter.GetBytes(psleep));

                // update damage toler
                float pdt = float.Parse(_form.numDT.Value.ToString());
                savegame.savegame_update(offsets.player_damagetol, BitConverter.GetBytes(pdt));

            }
            catch (System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // updated the players XP and lvl
            try
            {
                // first lets get the old xp
                int oi_xp = (int)BitConverter.ToSingle(savegame.savegame_player_exp, 0);
                float of_xp = BitConverter.ToSingle(savegame.savegame_player_exp, 0);

                // get whats in the form
                float xp = (float)_form.numXP.Value;

                // are we subing?
                if (xp < 0)
                {
                    xp = of_xp - xp;
                }

                int lvl = int.Parse(_form.numLvl.Value.ToString());
                //float xp = ( 25 * ((3 * lvl)  + 2) * (lvl - 1)) - 1;

                //savegame.savegame_update(offsets.player_level, BitConverter.GetBytes(lvl));
                //savegame.savegame_update((offsets.experiance + 0x2f8), BitConverter.GetBytes(lvl));
                savegame.savegame_update(offsets.experiance, BitConverter.GetBytes(xp));

                // update the 2nd lvl offset
                //savegame.savegame_update(offsets.player_level2, ByteFunctions.SwapEndian(BitConverter.GetBytes(lvl)));

                _form.progressbar_inc();
            }
            catch(System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
            // try and update the savegame data and write out the modified file
            try
            {
                savegame.savegame_savedata = xpack.updateFile(savegame.savegame_savedata);
            
                FileStream fs = new FileStream(file, FileMode.Create, FileAccess.Write);
           
                fs.Write(savegame.savegame_savedata, 0, savegame.savegame_savedata.Length);
                _form.progressbar_inc();
           
                fs.Close();
           
                _form.progressbar_inc();
                _form.barStatus.Caption = "Done!";

                XtraMessageBox.Show("File was successfuly saved, please remember to rehash/resign the file.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool update_item_qty(int item_offset, int qty)
        {
            savegame.savegame_update(item_offset + 0x5, BitConverter.GetBytes(qty));
            
            return true;
        }

        public bool update_item_fid(int item_fid, int item_new_fid)
        {
            savegame.savegame_fidupdate(item_fid, item_new_fid);
            
            return true;
        }

        public bool update_item_cdn(int item_offset, int cdn)
        {
            byte[] item_cdn = Conv.GetBytes((float)cdn, false);
            savegame.savegame_cdnupdate(item_offset, item_cdn);
            
            return true;
        }
        
        public bool export(string filepath)
        {
            BinaryWriter bWrite = new BinaryWriter(new FileStream(filepath + "\\special.bin", FileMode.Create, FileAccess.Write));
            bWrite.Write(savegame.savegame_psepcial);
            bWrite.Close();

            bWrite = new BinaryWriter(new FileStream(filepath + "\\inventory.bin", FileMode.Create, FileAccess.Write));
            bWrite.Write(savegame.savegame_pinventory);
            bWrite.Close();

            bWrite = new BinaryWriter(new FileStream(filepath + "\\formid.bin", FileMode.Create, FileAccess.Write));
            bWrite.Write(savegame.savegame_fidtable);
            bWrite.Close();

            return true;
        }

        public bool import_special(string filename)
        {
            using (FileStream io = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                BinaryReader bRead = new BinaryReader(io);
                byte[] specialdata = new byte[io.Length];
                bRead.Read(specialdata, 0, specialdata.Length);
                bRead.Close();
                if (specialdata.Length == savegame.savegame_psepcial.Length)
                {
                    savegame.savegame_psepcial = specialdata;
                    savegame.savegame_update(offsets.special_str, specialdata);
                    load_stats();
                    return true;
                }
                else
                    XtraMessageBox.Show("S.P.E.C.I.A.L. data length can not be more than " + savegame.savegame_psepcial.Length.ToString("X8"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public bool import_inventory(string filename)
        {
            using (FileStream io = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                BinaryReader bRead = new BinaryReader(io);
                byte[] inventorydata = new byte[io.Length];
                bRead.Read(inventorydata, 0, inventorydata.Length);
                bRead.Close();
                // same size
                if (inventorydata.Length == savegame.savegame_pinventory.Length)
                {
                    savegame.savegame_pinventory = inventorydata;
                    savegame.savegame_update(offsets.inventory_fitem, inventorydata);
                    load_items();
                    return true;
                }
                else
                    XtraMessageBox.Show("New inventory size is not the same and the old one?!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public bool import_fidtable(string filename)
        {
            using (FileStream io = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                BinaryReader bRead = new BinaryReader(io);
                byte[] fiddata = new byte[io.Length];
                bRead.Read(fiddata, 0, fiddata.Length);
                bRead.Close();
                if (fiddata.Length == savegame.savegame_fidtable.Length)
                {
                    savegame.savegame_fidtable = fiddata;
                    savegame.savegame_update(offsets.size_formid, fiddata);
                    load_fidtable();
                    return true;
                }
                else
                    XtraMessageBox.Show("FormID Table data too long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return true;
        }

        public bool pc_to_xbox360()
        {
            STFS xstfs;
            string pcfilename, xfilename;
            byte[] pcfiledata;

            // open the pc save file
            OpenFileDialog odlg = new OpenFileDialog { Filter = "FalloutNV (*.fos)|*.fos|" + "All files (*.*)|*.*", Title = "Select your PC save game..." };
            
            if (odlg.ShowDialog() == DialogResult.OK)
                pcfilename = odlg.FileName;
            else
                return false;

            FileStream fs = new FileStream(pcfilename, FileMode.Open, FileAccess.Read);
            fs.Read((pcfiledata = new byte[fs.Length]), 0, (int)fs.Length);
            fs.Close();

            // open the xbox save file
            OpenFileDialog odlg2 = new OpenFileDialog { Filter = "FalloutNV (*.fxs)|*.fxs|" + "All files (*.*)|*.*", Title = "Select your XBOX save game..." };
            if (odlg2.ShowDialog() == DialogResult.OK)
                xfilename = odlg2.FileName;
            else
                return false;

            xstfs = new STFS(xfilename);
            byte[] wodata = xstfs.updateFile(pcfiledata);

            // backup the old fxs file & write out the new file
            File.Copy(xfilename, xfilename + ".bak");
            FileStream fs2 = new FileStream(xfilename, FileMode.Create, FileAccess.Write);
            fs2.Write(wodata, 0, wodata.Length);
            fs2.Close();

            XtraMessageBox.Show("Success", "File has been successfully saved.\nNOTE: Do NOT resigh/rehash this file! If you do it will NOT work!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            return true;
        }

        public void dump_fid()
        {
            SaveFileDialog sdlg = new SaveFileDialog();
            if (sdlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter sWrite = new StreamWriter(sdlg.FileName);
                for (int x = 0; x < fid.fid_table.Length; x++)
                {
                    sWrite.WriteLine(x.ToString() + "\t: " + fid.fid_table[x].ToString("X6"));
                }

                sWrite.Close();
            }
        }

        public bool xbox360_to_pc()
        {
            STFS xstfs;
            byte[] xfiledata;
            byte[] rawdata;
            List<byte> cleanname;
            string filen;

            // open the 360 save file
            OpenFileDialog odlg = new OpenFileDialog { Filter = "FalloutNV (*.fxs)|*.fxs|" + "All files (*.*)|*.*", Title = "Select your XBOX 360 save game..." };
            if (odlg.ShowDialog() == DialogResult.OK)
            {
                filen = odlg.FileName;
            }
            else
            {
                return false;
            }

            xstfs = new STFS(odlg.FileName);
            xfiledata = xstfs.extractFile();
            rawdata = xstfs.rawFile();

            // get the file name
            int endfn = ByteFunctions.ByteSearchFirst(rawdata, new byte[] { 0x00, 0x00, 0x00, 0x00 }, 0x412);

            byte[] fname = ByteFunctions.BytePeice(rawdata, 0x412, (endfn - 0x412));
            cleanname = new List<byte>();

            // remove all 00's
            for (int i = 0; i < fname.Length; i++)
            {
                if (fname[i] != 0x00)
                    cleanname.Add(fname[i]);
            }

            // write out the save file
            FolderBrowserDialog fdlg = new FolderBrowserDialog { Description = "Select save destination..." };
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(fdlg.SelectedPath + "\\" + Encoding.Default.GetString(cleanname.ToArray()) + ".fos", FileMode.Create, FileAccess.Write);
                fs.Write(xfiledata, 0, xfiledata.Length);
                fs.Close();
            }
            else
            {
                return false;
            }

            XtraMessageBox.Show("File Saved!");

            return true;
        }

    }
    
}
