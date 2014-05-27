using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Fallout3VE.MiscFunctions;
using Fallout3VE.Offset;
using XLib.Other;
using XLib.Converters;

namespace Fallout3VE.SaveGame
{
    class Savegame
    {
        OffsetsClass _offsets;

        byte[] savedata;

        byte[] header;
        byte[] formid_table;
        byte[] player_inventory;
        byte[] player_special;
        public byte[] load_orders;
        public string[] load_seq;

        byte[] player_name;
        byte[] player_location;
        byte[] player_time;
        byte[] player_karma;
        byte[] player_level;
        byte[] player_exp;
        byte[] player_weight;
        byte[] player_hp;
        byte[] player_ap;
        byte[] player_dmg;
        byte[] player_crit;
        byte[] player_karma_num;
        byte[] player_rad;
        byte[] player_speed;
        byte[] player_reload;
        byte[] player_throw;
        byte[] player_size;
        byte[] player_h20;
        byte[] player_sleep;
        byte[] player_dmgt;

        byte[] player_str;
        byte[] player_per;
        byte[] player_end;
        byte[] player_cha;
        byte[] player_int;
        byte[] player_agi;
        byte[] player_luc;

        byte[] player_barter, player_eweapons, player_explo, player_guns, player_lockpick, player_medicine,
            player_melee, player_repair, player_science, player_sneak, player_speech, player_survival, player_unarmed;

        public Savegame(byte[] sdata, OffsetsClass of)
        {
            _offsets = of;
            savedata = sdata;
            break_down(sdata);
        }

        public Savegame(string filename, OffsetsClass of)
        {
            _offsets = of;

            using (FileStream io = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                BinaryReader bRead = new BinaryReader(io);
                savedata = new byte[io.Length];
                bRead.Read(savedata, 0, savedata.Length);
                bRead.Close();
                io.Close();
                break_down(savedata);
            }
        }

        public void savedata_rewrite(byte[] newdata)
        {
            savedata = new byte[newdata.Length];
            savedata = newdata;
        }

        public byte[] savegame_header { get { return header; } set { header = value; } }
        public byte[] savegame_fidtable { get { return formid_table; } set { formid_table = value; } }
        public byte[] savegame_pinventory { get { return player_inventory; } set { player_inventory = value; } }
        public byte[] savegame_psepcial { get { return player_special; } set { player_special = value; } }
        public byte[] savegame_savedata { get { return savedata; } set { savedata = value; } }
        
        public byte[] savegame_player_name { get { return player_name; } }
        public byte[] savegame_player_location { get { return player_location; } }
        public byte[] savegame_player_karma { get { return player_karma; } }
        public byte[] savegame_player_time { get { return player_time; } }
        public byte[] savegame_player_level { get { return player_level; } }
        public byte[] savegame_player_exp { get { return player_exp; } }
        public byte[] savegame_player_weight { get { return player_weight; } }
        public byte[] savegame_player_hp { get { return player_hp; } }
        public byte[] savegame_player_ap { get { return player_ap; } }
        public byte[] savegame_player_dmg { get { return player_dmg; } }
        public byte[] savegame_player_crit { get { return player_crit; } }
        public byte[] savegame_player_karma_num { get { return player_karma_num; } }
        public byte[] savegame_player_radres { get { return player_rad; } }
        public byte[] savegame_player_speed { get { return player_speed; } }
        public byte[] savegame_player_reload { get { return player_reload; } }
        public byte[] savegame_player_throw { get { return player_throw; } }
        public byte[] savegame_player_size { get { return player_size; } }
        public byte[] savegame_player_h20 { get { return player_h20; } }
        public byte[] savegame_player_sleep { get { return player_sleep; } }
        public byte[] savegame_player_dmgt { get { return player_dmgt; } }

        public byte[] savegame_special_str { get { return player_str; } }
        public byte[] savegame_special_per { get { return player_per; } }
        public byte[] savegame_special_end { get { return player_end; } }
        public byte[] savegame_special_cha { get { return player_cha; } }
        public byte[] savegame_special_int { get { return player_int; } }
        public byte[] savegame_special_agi { get { return player_agi; } }
        public byte[] savegame_special_luc { get { return player_luc; } }

        public byte[] savegame_player_barter { get { return player_barter; } }
        public byte[] savegame_player_eweapons { get { return player_eweapons; } }
        public byte[] savegame_player_explo { get { return player_explo; } }
        public byte[] savegame_player_guns { get { return player_guns; } }
        public byte[] savegame_player_lockpick { get { return player_lockpick; } }
        public byte[] savegame_player_medicine { get { return player_medicine; } }
        public byte[] savegame_player_melee { get { return player_melee; } }
        public byte[] savegame_player_repair { get { return player_repair; } }
        public byte[] savegame_player_science { get { return player_science; } }
        public byte[] savegame_player_sneak { get { return player_sneak; } }
        public byte[] savegame_player_speech { get { return player_speech; } }
        public byte[] savegame_player_survival { get { return player_survival; } }
        public byte[] savegame_player_unarmed { get { return player_unarmed; } }

        public string[] savegame_loadorder { get { return load_seq; } }

        
        public bool savegame_update(int offset, byte[] data)
        {
            // does the offset exist and is it valid lenght?
            if (savedata.Length < (offset + data.Length))
                return false;

            for (int x = 0; x < data.Length; x++)
                savedata[offset + x] = data[x];

            break_down(savedata);

            return true;
        }

        public bool savegame_update(int offset, byte data)
        {
            // does the offset exist and is it valid lenght?
            if (savedata.Length < (offset + 2))
                return false;

            for (int x = 0; x < 2; x++)
                savedata[offset + x] = data;

            break_down(savedata);

            return true;
        }

        public bool savegame_cdnupdate(int offset, byte[] data)
        {
            // find the 7C 25
            for (int x = offset; x < savedata.Length; x++)
            {
                if (savedata[x] == 0x7C && savedata[x + 1] == 0x25 && savedata[x + 2] == 0x7C)
                {
                    for (int y = 0; y < data.Length; y++)
                    {
                        savedata[x + 3 + y] = data[y];
                    }

                    break;
                }
            }

            break_down(savedata);

            return true;
        }

        public bool savegame_fidupdate(int currentfid, int replacefid)
        {
            byte[] cfid = BitConverter.GetBytes(currentfid);
            byte[] rfid = BitConverter.GetBytes(replacefid);

            int fid_off = ByteFunctions.ByteSearchFirst(savedata, cfid, _offsets.size_formid);

            for (int x = fid_off, y = 0; y < rfid.Length; x++, y++)
                savedata[x] = rfid[y];

            break_down(savedata);

            return true;
        }

        public bool savegame_lvlupdate(int new_lvl)
        {
            byte[] blvl = BitConverter.GetBytes(new_lvl);

            int y = 0;
            for (int x = 0, c = 0; x < header.Length; x++)
            {
                if (header[x] == 0x7C)
                    c++;

                if (c == 8)
                {
                    y = x + 1;
                    break;
                }
            }

            for (int x = 0; x < blvl.Length; x++)
                savedata[y + x] = blvl[x];

            return true;
        }

        protected void read_header()
        {
            string[] player_data = Encoding.ASCII.GetString(header).Split('|');
           
            player_name = Encoding.Default.GetBytes(player_data[6]);
            player_karma = Encoding.Default.GetBytes(player_data[8]);
            player_time = Encoding.Default.GetBytes(player_data[13]);
            player_location = Encoding.Default.GetBytes(player_data[11]);
            player_level = Encoding.Default.GetBytes(player_data[9]);
        }

        protected void read_special()
        {
            player_str = ByteFunctions.BytePeice(savedata, _offsets.special_str, 1);
            player_per = ByteFunctions.BytePeice(savedata, _offsets.special_per, 1);
            player_end = ByteFunctions.BytePeice(savedata, _offsets.special_end, 1);
            player_cha = ByteFunctions.BytePeice(savedata, _offsets.special_cha, 1);
            player_int = ByteFunctions.BytePeice(savedata, _offsets.special_int, 1);
            player_agi = ByteFunctions.BytePeice(savedata, _offsets.special_agi, 1);
            player_luc = ByteFunctions.BytePeice(savedata, _offsets.special_luc, 1);
        }

        protected void read_skills()
        {
            player_barter = ByteFunctions.BytePeice(savedata, _offsets.barter, 4);
            player_eweapons = ByteFunctions.BytePeice(savedata, _offsets.eweapons, 4);
            player_explo = ByteFunctions.BytePeice(savedata, _offsets.explo, 4);
            player_guns = ByteFunctions.BytePeice(savedata, _offsets.guns, 4);
            player_lockpick = ByteFunctions.BytePeice(savedata, _offsets.lockpick, 4);
            player_medicine = ByteFunctions.BytePeice(savedata, _offsets.medicine, 4);
            player_melee = ByteFunctions.BytePeice(savedata, _offsets.melee, 4);
            player_repair = ByteFunctions.BytePeice(savedata, _offsets.repair, 4);
            player_science = ByteFunctions.BytePeice(savedata, _offsets.science, 4);
            player_sneak = ByteFunctions.BytePeice(savedata, _offsets.sneak, 4);
            player_speech = ByteFunctions.BytePeice(savedata, _offsets.speech, 4);
            player_survival = ByteFunctions.BytePeice(savedata, _offsets.survival, 4);
            player_unarmed = ByteFunctions.BytePeice(savedata, _offsets.unarmed, 4);
        }

        protected void read_exp()
        {
            player_exp = ByteFunctions.BytePeice(savedata, _offsets.experiance, 4);
        }

        protected void read_weight()
        {
            player_weight = ByteFunctions.BytePeice(savedata, _offsets.cweight, 4);
        }

        protected void read_h20()
        {
            player_h20 = ByteFunctions.BytePeice(savedata, _offsets.player_h20, 4);
        }

        protected void read_sleep()
        {
            player_sleep = ByteFunctions.BytePeice(savedata, _offsets.player_sleep, 4);
        }

        protected void read_damaget()
        {
            player_dmgt = ByteFunctions.BytePeice(savedata, _offsets.player_damagetol, 4);
        }

        protected void read_playersize()
        {
            player_size = ByteFunctions.BytePeice(savedata, _offsets.player_size, 4);
        }

        protected void read_throw()
        {
            player_throw = ByteFunctions.BytePeice(savedata, _offsets.player_throw, 4);
        }

        protected void read_reload()
        {
            player_reload = ByteFunctions.BytePeice(savedata, _offsets.player_reload, 4);
        }

        protected void read_speed()
        {
            player_speed = ByteFunctions.BytePeice(savedata, _offsets.player_speed, 4);
        }

        protected void read_hp()
        {
            player_hp = ByteFunctions.BytePeice(savedata, _offsets.health, 4);
        }

        protected void read_action()
        {
            player_ap = ByteFunctions.BytePeice(savedata, _offsets.actionpoints, 4);
        }

        protected void read_damage()
        {
            player_dmg = ByteFunctions.BytePeice(savedata, _offsets.damage_res, 4);
        }

        protected void read_crit()
        {
            player_crit = ByteFunctions.BytePeice(savedata, _offsets.crit, 4);
        }

        protected void read_karma()
        {
            player_karma_num = ByteFunctions.BytePeice(savedata, _offsets.karma, 4);
        }

        protected void read_radr()
        {
            player_rad = ByteFunctions.BytePeice(savedata, _offsets.rad_resist, 4);
        }

        protected void read_loadorder()
        {
            int addon_value = Conv.ToInt32(ByteFunctions.BytePeice(savedata, _offsets.load_order_offset - 0x01, 1), true);
            int max_count = addon_value * 2;
            int count = 0, size = 0;

            for (int x = (_offsets.load_order_offset + 0x01); count < max_count; x++, size++)
            {
                if (savedata[x] == 0x7C)
                    count++;
            }

            load_orders = ByteFunctions.BytePeice(savedata, (_offsets.load_order_offset + 0x01), size);
            string[] parts = Encoding.ASCII.GetString(load_orders).Split('|');
            List<string> temp_seq = new List<string>();

            bool odd_iter = false;

            foreach (string str in parts)
            {
                if (odd_iter)
                {
                    temp_seq.Add(str);
                    odd_iter = false;
                }
                else
                {
                    odd_iter = true;
                }
            }

            //temp_seq.Remove("FalloutNV.esm");
            //temp_seq.Remove("update.esp");
            //temp_seq.Remove("TribalPack.esm");
            //temp_seq.Remove("ClassicPack.esm");
            //temp_seq.Remove("CaravanPack.esm");
            //temp_seq.Remove("MercenaryPack.esm");

            load_seq = new string[temp_seq.Count];
            for (int x = 0; x < load_seq.Length; x++)
                load_seq[x] = temp_seq[x];
        }

        protected void break_down(byte[] data)
        {
            // header
            int size = 0;
            for (int x = 0; data[x] != 0xFF; x++)
                size++;

            header = new byte[size];
            for (int x = 0; x < header.Length; x++)
                header[x] = data[x];
            
            // formid_table
            formid_table = ByteFunctions.BytePeice(data, _offsets.size_formid, _offsets.end_formid - _offsets.size_formid);
            
            // player inventory
            player_inventory = ByteFunctions.BytePeice(data, _offsets.inventory_fitem, _offsets.inventory_eitem - _offsets.inventory_fitem);

            // player special
            player_special = ByteFunctions.BytePeice(data, _offsets.special_str, 7);

            // addon load order
            read_loadorder();

            // read the header
            read_header();

            // set the special data
            read_special();

            // read exp
            read_exp();

            // read weight
            read_weight();

            // read hp
            read_hp();

            // read ap
            read_action();

            // read dmg resistance
            read_damage();

            // read crit
            read_crit();

            // read karma
            read_karma();

            // read rad resistance
            read_radr();

            // read the players skills
            read_skills();

            // read the players speed
            read_speed();

            // read the players reload speed
            read_reload();

            // read player throw distance
            read_throw();

            // read the player size
            read_playersize();

            // read the player h20
            read_h20();

            // read the sleep
            read_sleep();

            // read the damage tol
            read_damaget();
        }

    }
}
