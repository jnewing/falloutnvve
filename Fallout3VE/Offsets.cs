using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using Fallout3VE.MiscFunctions;
using XLib.Converters;
using XLib.Other;
using DevExpress.XtraEditors;

namespace Fallout3VE.Offset
{
    /*
    | ===============================================================
    |  Offsets Storage
    | ===============================================================
    | 
    */
    class SGOffsets
    {
        int level, level2;
        int name_first, name_last;
        int player_data_offset;
        int sstr, sper, send, scha, sint, sagi, sluc;
        int fid_start, fid_size, fid_end;
        int inv_start, inv_fitem, inv_eitem, inv_end;
        int load_order;
        int experiance;
        int carry_weight;
        int health;
        int actionpoints;
        int damageres;
        int crit;
        int karma;
        int rad_resist;
        int barter, eweapons, explo, guns, lockpick, medicine, melee, repair, science, sneak, speech, survival, unarmed;
        int statstart;
        int speed;
        int reload;
        int tdistance;
        int msize;
        int h20;
        int sleep;
        int damagetol;
        int names;

        public int stat_start_offset { get { return statstart; } set { statstart = value; } }
        public int pdata_offset { get { return player_data_offset; } set { player_data_offset = value; } }
        public int plevel { get { return level; } set { level = value; } }
        public int plevel2 { get { return level2; } set { level2 = value; } }
        public int pname_first { get { return name_first; } set { name_first = value; } }
        public int pname_last { get { return name_last; } set { name_last = value; } }
        public int pnames { get { return names; } set { names = value; } }
        
        public int special_str { get { return sstr; } set { sstr = value; } }
        public int special_per { get { return sper; } set { sper = value; } }
        public int special_end { get { return send; } set { send = value; } }
        public int special_cha { get { return scha; } set { scha = value; } }
        public int special_int { get { return sint; } set { sint = value; } }
        public int special_agi { get { return sagi; } set { sagi = value; } }
        public int special_luc { get { return sluc; } set { sluc = value; } }
        
        public int formid_start { get { return fid_start; } set { fid_start = value; } }
        public int formid_size { get { return fid_size; } set { fid_size = value; } }
        public int formid_end { get { return fid_end; } set { fid_end = value; } }
        
        public int pinv_start { get { return inv_start; } set { inv_start = value; } }
        public int pinv_fitem { get { return inv_fitem; } set { inv_fitem = value; } }
        public int pinv_eitem { get { return inv_eitem; } set { inv_eitem = value; } }
        public int pinv_end { get { return inv_end; } set { inv_end = value; } }

        public int load_offs { get { return load_order; } set { load_order = value; } }

        public int pdamaget { get { return damagetol; } set { damagetol = value; } }
        public int psleep { get { return sleep; } set { sleep = value; } }
        public int ph20 { get { return h20; } set { h20 = value; } }
        public int psize { get { return msize; } set { msize = value; } }
        public int pthrow { get { return tdistance; } set { tdistance = value; } }
        public int preload { get { return reload; } set { reload = value; } }
        public int pspeed { get { return speed; } set { speed = value; } }
        public int pexperiance { get { return experiance; } set { experiance = value; } }
        public int pcweight { get { return carry_weight; } set { carry_weight = value; } }
        public int phealth { get { return health; } set { health = value; } }
        public int pactionp { get { return actionpoints; } set { actionpoints = value; } }
        public int pdamager { get { return damageres; } set { damageres = value; } }
        public int pcrit { get { return crit; } set { crit = value; } }
        public int pkarma { get { return karma; } set { karma = value; } }
        public int pradresist { get { return rad_resist; } set { rad_resist = value; } }

        public int s_barter { get { return barter; } set { barter = value; } }
        public int s_eweapons { get { return eweapons; } set { eweapons = value; } }
        public int s_explo { get { return explo; } set { explo = value; } }
        public int s_guns { get { return guns; } set { guns = value; } }
        public int s_lockpick { get { return lockpick; } set { lockpick = value; } }
        public int s_medicine { get { return medicine; } set { medicine = value; } }
        public int s_melee { get { return melee; } set { melee = value; } }
        public int s_repair { get { return repair; } set { repair = value; } }
        public int s_science { get { return science; } set { science = value; } }
        public int s_sneak { get { return sneak; } set { sneak = value; } }
        public int s_speech { get { return speech; } set { speech = value; } }
        public int s_survival { get { return survival; } set { survival = value; } }
        public int s_unarmed { get { return unarmed; } set { unarmed = value; } }
        
    }
    // ==============================================================


    /*
    | ===============================================================
    |  Offsets Class
    | ===============================================================
    | 
    */
    class OffsetsClass
    {
        SGOffsets offsets = new SGOffsets();
        byte[] savedata;

        public OffsetsClass() {}

        public OffsetsClass(string filename)
        {
            try
            {
                using (FileStream io = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader bRead = new BinaryReader(io);
                    savedata = new byte[io.Length];
                    bRead.Read(savedata, 0, savedata.Length);
                    bRead.Close();
                    io.Close();
                }

                load_offsets();
            }
            catch(System.Exception ex)
            {
                XtraMessageBox.Show(ex.ToString(),  "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        public OffsetsClass(byte[] sdata)
        {
            savedata = new byte[sdata.Length];
            savedata = sdata;

            load_offsets();
        }

        public int player_data { get { return offsets.pdata_offset; } }
        public int player_level { get { return offsets.plevel; } }
        public int player_level2 { get { return offsets.plevel2; } }
        public int name_first { get { return offsets.pname_first; } }
        public int name_last { get { return offsets.pname_last; } }
        public int special_str { get { return offsets.special_str; } }
        public int special_per { get { return offsets.special_per; } }
        public int special_end { get { return offsets.special_end; } }
        public int special_cha { get { return offsets.special_cha; } }
        public int special_int { get { return offsets.special_int; } }
        public int special_agi { get { return offsets.special_agi; } }
        public int special_luc { get { return offsets.special_luc; } }
        public int begin_formid { get { return offsets.formid_start; } }
        public int size_formid { get { return offsets.formid_size; } }
        public int end_formid { get { return offsets.formid_end; } }
        public int inventory_begin { get { return offsets.pinv_start; } }
        public int inventory_fitem { get { return offsets.pinv_fitem; } }
        public int inventory_eitem { get { return offsets.pinv_eitem; } }
        public int inventory_end { get { return offsets.pinv_end; } }
        public int load_order_offset { get { return offsets.load_offs; } }
        public int experiance { get { return offsets.pexperiance; } }
        public int cweight { get { return offsets.pcweight; } }
        public int health { get { return offsets.phealth; } }
        public int actionpoints { get { return offsets.pactionp; } }
        public int damage_res { get { return offsets.pdamager; } }
        public int crit { get { return offsets.pcrit; } }
        public int karma { get { return offsets.pkarma; } }
        public int rad_resist { get { return offsets.pradresist; } }
        public int player_damagetol { get { return offsets.pdamaget; } }
        public int player_sleep { get { return offsets.psleep; } }
        public int player_h20 { get { return offsets.ph20; } }
        public int player_speed { get { return offsets.pspeed; } }
        public int player_reload { get { return offsets.preload; } }
        public int player_throw { get { return offsets.pthrow; } }
        public int player_size { get { return offsets.psize; } }
        public int barter { get { return offsets.s_barter; } }
        public int eweapons { get { return offsets.s_eweapons; } }
        public int explo { get { return offsets.s_explo; } }
        public int guns { get { return offsets.s_guns; } }
        public int lockpick { get { return offsets.s_lockpick; } }
        public int medicine { get { return offsets.s_medicine; } }
        public int melee { get { return offsets.s_melee; } }
        public int repair { get { return offsets.s_repair; } }
        public int science { get { return offsets.s_science; } }
        public int sneak { get { return offsets.s_sneak; } }
        public int speech { get { return offsets.s_speech; } }
        public int survival { get { return offsets.s_survival; } }
        public int unarmed { get { return offsets.s_unarmed; } }

        protected void load_offsets()
        {
            // set player level
            set_level_offset();
            
            // set name offsets
            set_name_offsets();

            // set load orders
            set_load_offsets();

            // set the stat start offset
            set_statstart();

            // set s.p.e.c.i.a.l. offsets
            set_special_offests();

            // set fid table offsets
            set_fid_offests();

            // set inv offsets
            set_inv_offsets();

            // set exp offset
            set_exp_offset();

            // set weight offset;
            set_weight_offset();

            // set hp offset
            set_health_offset();

            // set ap offset
            set_action_offset();

            // set damage resitance offset
            set_damage_offset();

            // set crit offset
            set_crit_offset();

            // set karma offset
            set_karma_offset();

            // set rad_resist offset
            set_rad_offset();

            // set player speed offset
            set_speed_offset();

            // set reload speed offset
            set_reload_offset();

            // set throwing distance
            set_throw_offset();

            // set player size
            set_player_size();

            // set skills offsets
            set_skills_offsets();

            // set the second lvl offset
            set_level2_offset();

            // set h20
            set_player_h20();

            // set sleep
            set_player_sleep();

            // set damage tol
            set_player_damaget();

            // set player names
        }

        protected void set_statstart()
        {
            offsets.stat_start_offset = ByteFunctions.ByteSearchFirst(savedata, new byte[] { 0x7C, 0xF4, 0x0F, 0x49, 0x40, 0x7C }, 0);
        }

        protected void set_skills_offsets()
        {
            /*
            offsets.s_barter = offsets.stat_start_offset - 0x0282;
            offsets.s_eweapons = offsets.stat_start_offset - 0x0278;
            offsets.s_explo = offsets.stat_start_offset - 0x0273;
            offsets.s_lockpick = offsets.stat_start_offset - 0x026e;
            offsets.s_medicine = offsets.stat_start_offset - 0x0269;
            offsets.s_melee = offsets.stat_start_offset - 0x0264;
            offsets.s_repair = offsets.stat_start_offset - 0x025f;
            offsets.s_science = offsets.stat_start_offset - 0x025a;
            offsets.s_guns = offsets.stat_start_offset - 0x0255;
            offsets.s_sneak = offsets.stat_start_offset - 0x0250;
            offsets.s_speech = offsets.stat_start_offset - 0x024b;
            offsets.s_survival = offsets.stat_start_offset - 0x0246;
            offsets.s_unarmed = offsets.stat_start_offset - 0x0241;
            */

            offsets.s_barter = offsets.stat_start_offset - 0x0298;
            offsets.s_eweapons = offsets.stat_start_offset - 0x03a8;
            offsets.s_explo = offsets.stat_start_offset - 0x0273;
            offsets.s_lockpick = offsets.stat_start_offset - 0x026e;
            offsets.s_medicine = offsets.stat_start_offset - 0x0269;
            offsets.s_melee = offsets.stat_start_offset - 0x0264;
            offsets.s_repair = offsets.stat_start_offset - 0x025f;
            offsets.s_science = offsets.stat_start_offset - 0x025a;
            offsets.s_guns = offsets.stat_start_offset - 0x0255;
            offsets.s_sneak = offsets.stat_start_offset - 0x0250;
            offsets.s_speech = offsets.stat_start_offset - 0x024b;
            offsets.s_survival = offsets.stat_start_offset - 0x0246;
            offsets.s_unarmed = offsets.stat_start_offset - 0x0241;

        }

        protected void set_player_names_offsets()
        {
            
        }

        protected void set_level_offset()
        {
            offsets.plevel = 0;
            for (int y = 0; y < 9; offsets.plevel++)
            {
                if (savedata[offsets.plevel] == 0x7C)
                    y++;
            }
        }

        protected void set_level2_offset()
        {
            byte[] temp_bytel = ByteFunctions.BytePeice(savedata, offsets.plevel, 0x4);
            int tmp_intl = BitConverter.ToInt32(temp_bytel, 0);

            //offsets.plevel2 = ByteFunctions.ByteSearchFirstReverse(savedata, ByteFunctions.SwapEndian(temp_bytel), offsets.pname_last);
            offsets.plevel2 = ByteFunctions.ByteSearchFirstReverse(savedata, ByteFunctions.SwapEndian(temp_bytel), offsets.pexperiance);
        }

        protected void set_player_h20()
        {
            //offsets.ph20 = offsets.stat_start_offset - 0x01b5;
            offsets.ph20 = offsets.stat_start_offset - 0x01b3;
        }

        protected void set_player_sleep()
        {
            //offsets.psleep = offsets.stat_start_offset - 0x01ab;
            offsets.psleep = offsets.stat_start_offset - 0x01a9;
        }

        protected void set_player_damaget()
        {
            //offsets.pdamaget = offsets.stat_start_offset - 0x01a6;
            offsets.pdamaget = offsets.stat_start_offset - 0x01a4;
        }

        protected void set_player_size()
        {
            //offsets.psize = offsets.stat_start_offset - 0x019;
            offsets.psize = offsets.stat_start_offset - 0x02f;
        }

        protected void set_throw_offset()
        {
            //offsets.pthrow = offsets.stat_start_offset - 0x0309;
            offsets.pthrow = offsets.stat_start_offset - 0x0307;
        }

        protected void set_reload_offset()
        {
            //offsets.preload = offsets.stat_start_offset - 0x02f0;
            offsets.preload = offsets.stat_start_offset - 0x02ee;
        }

        protected void set_speed_offset()
        {
            //offsets.pspeed = offsets.stat_start_offset - 0x02b9;
            offsets.pspeed = offsets.stat_start_offset - 0x02b7;
        }

        protected void set_rad_offset()
        {
            //offsets.pradresist = offsets.stat_start_offset - 0x02be;
            offsets.pradresist = offsets.stat_start_offset - 0x02bc;
        }

        protected void set_karma_offset()
        {
            //offsets.pkarma = offsets.stat_start_offset - 0x02af;
            offsets.pkarma = offsets.stat_start_offset - 0x02ad;
        }

        protected void set_crit_offset()
        {
            //offsets.pcrit = offsets.stat_start_offset - 0x02d7;
            offsets.pcrit = offsets.stat_start_offset - 0x02d5;
        }

        protected void set_damage_offset()
        {
            //offsets.pdamager = offsets.stat_start_offset - 0x02c8;
            offsets.pdamager = offsets.stat_start_offset - 0x02c6;
        }

        protected void set_action_offset()
        {
            //offsets.pactionp = offsets.stat_start_offset - 0x02e6;
            offsets.pactionp = offsets.stat_start_offset - 0x02e4;
        }

        protected void set_health_offset()
        {
            //offsets.phealth = offsets.stat_start_offset - 0x02d2;
            offsets.phealth = offsets.stat_start_offset - 0x02d0;
        }

        protected void set_weight_offset()
        {
            //offsets.pcweight = offsets.stat_start_offset - 0x02e1;
            offsets.pcweight = offsets.stat_start_offset - 0x02df;
        }

        protected void set_load_offsets()
        {
            offsets.load_offs = ByteFunctions.ByteSearchFirst(savedata, new byte[] { 0x7C, 0x0D, 0x00, 0x7C, 0x46, 0x61, 0x6C, 0x6C, 0x6F, 0x75, 0x74, 0x4E, 0x56, 0x2E, 0x65, 0x73, 0x6D, 0x7C }, 0);      
        }

        protected void set_exp_offset()
        {
            offsets.pexperiance = offsets.stat_start_offset - 0x02ac;
        }

        protected void set_inv_offsets()
        {   
            byte[] noff = ByteFunctions.HexToBytes("7C" + fid_array().ToString("X6") + "7C");
            offsets.pinv_end = ByteFunctions.ByteSearchLast(savedata, noff, 0);
            offsets.pinv_start = ByteFunctions.ByteSearchFirstReverse(savedata, new byte[] { 0x7C, 0x00, 0x00, 0x80, 0x3F }, offsets.pinv_end);
            
            // find the first item
            for (int x = offsets.pinv_start; x < savedata.Length; x++)
            {
                if (savedata[x] == 0x7C && savedata[x + 4] == 0x7C && savedata[x + 9] == 0x7C)
                {
                    offsets.pinv_fitem = x;
                    break;
                }
            }
            
            // find the last item
            offsets.pinv_eitem = ByteFunctions.ByteSearchFirst(savedata, new byte[] { 0x7C, 0x00, 0x00, 0x00, 0x7C }, offsets.pinv_fitem) - 0x15;
        }

        protected int fid_array()
        {
            int identifiers = BitConverter.ToInt32(savedata, offsets.formid_size);
            byte[] fid_data = ByteFunctions.BytePeice(savedata, offsets.formid_size, offsets.formid_end - offsets.formid_size);
            int[] fid_array = new int[identifiers];

            for (int x = 0; x < fid_array.Length; x++)
                fid_array[x] = BitConverter.ToInt32(fid_data, (x * 0x4));

            if (XtraMessageBox.Show("Are you currently using the Pimp-Boy?", "Pimp-Boy?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // find 0x0011BACB (pimpboy)
                for (int x = 0; x < fid_array.Length; x++)
                    if (fid_array[x] == 0x0011BACB)
                        return x;
            }
            else
            {
                // find 0x00015038 (pipboy)
                for (int x = 0; x < fid_array.Length; x++)
                    if (fid_array[x] == 0x00015038)
                        return x;
            }

            return 0;
        }


        /*
        | -------------------------------------
        |  load_formid_table()
        | -------------------------------------
        | 
        */
        protected void set_fid_offests()
        {
            int head_offset = ByteFunctions.ByteSearchFirst(savedata, new byte[] { 0x7C, 0x0D, 0x00, 0x7C, 0x46, 0x61, 0x6C, 0x6C, 0x6F, 0x75, 0x74, 0x4E, 0x56, 0x2E, 0x65, 0x73, 0x6D, 0x7C }, 0);
            int addon_count = Conv.ToInt32(ByteFunctions.BytePeice(savedata, (head_offset - 0x1), 1), true);

            for (int x = 0; x < ((addon_count * 2) + 1); head_offset++)
            {
                if (savedata[head_offset] == 0x7C)
                    x++;
            }

            offsets.formid_size = Conv.ToInt32(ByteFunctions.BytePeice(savedata, head_offset, 0x4), false);
            offsets.formid_start = offsets.formid_size + 0x4;
            
            // # of itentifiers
            int identifiers = BitConverter.ToInt32(savedata, offsets.formid_size);

            // end of form id table
            offsets.formid_end = offsets.formid_start + (identifiers * 0x4);

            int temp_pdoffset = head_offset + 0xC;
            offsets.pdata_offset = Conv.ToInt32(ByteFunctions.BytePeice(savedata, temp_pdoffset, 0x4), false);
        }
        // ------------------------------------


        protected void set_special_offests()
        {
            byte[] special_array = new byte[7];
            int[] special = new int[7];
            int[] special_offset = new int[7];

            // get the bytes used for S.P.E.C.I.A.L
            for (int x = (offsets.pname_last - 0x0000000b), y = 0; x < (offsets.pname_last - 0x0b) + special_array.Length; x++, y++)
            {
                special_offset[y] = x;
                special_array[y] = savedata[x];
            }

            // offsets for S.P.E.C.I.A.L.
            offsets.special_str = special_offset[0];
            offsets.special_per = special_offset[1];
            offsets.special_end = special_offset[2];
            offsets.special_cha = special_offset[3];
            offsets.special_int = special_offset[4];
            offsets.special_agi = special_offset[5];
            offsets.special_luc = special_offset[6];
        }


        protected void set_name_offsets()
        {
            // get name
            string name = get_name();

            // get the offsets
            List<int> n_offsets = ByteFunctions.ByteSearchAll(savedata, name, 0);

            offsets.pname_first = n_offsets[0];
            offsets.pname_last = n_offsets[1];
        }


        protected string get_name()
        {
            int start = 0, stop = 0, count = 0;
            for (int x = 0; x < 500; x++)
            {
                if (savedata[x] == 0x7C)
                    count++;

                if (count == 5)
                    start = x;

                if (count == 6)
                    stop = x;
            }

            return Encoding.Default.GetString(ByteFunctions.BytePeice(savedata, start + 2, stop - start -1));
        }

    }
    // ==============================================================
}
