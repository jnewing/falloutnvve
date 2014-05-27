using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using LumenWorks.Framework.IO.Csv;

namespace Fallout3VE.Database
{
    /*
    | ===============================================================
    |  LTIndex Storage Class
    | ===============================================================
    | 
    */
    class LTIndex
    {
        string item_type, item_desc, item_number, item_cdn;

        public LTIndex() { }
        public LTIndex(string type, string desc, string num)
        {
            item_type = type;
            item_desc = desc;
            item_number = num;
        }

        public LTIndex(string type, string desc, string num, string cdn)
        {
            item_type = type;
            item_desc = desc;
            item_number = num;
            item_cdn = cdn;
        }

        public string i_type { get { return item_type; } set { item_type = value; } }
        public string i_description { get { return item_desc; } set { item_desc = value; } }
        public string i_number { get { return item_number; } set { item_number = value; } }
        public string i_cdn { get { return item_cdn; } set { item_cdn = value; } }
    }
    // ==============================================================


    /*
    | ===============================================================
    |  Database Class
    | ===============================================================
    | 
    */
    class DatabaseClass
    {
        List<LTIndex> _lookuplist = new List<LTIndex>();
        CsvReader fo3_csv;
        string[] _order;
        string deadm, honesth, oldwb, lroad;

        ~DatabaseClass() { }

        public DatabaseClass(string[] ord)
        {
            _order = ord;
            load_database_files();
        }

        public DatabaseClass()
        {
            load_database_files_no_order();
        }

        public List<LTIndex> database { get { return _lookuplist; } }


        public string get_description(int f_id)
        {
            foreach (LTIndex item in _lookuplist)
                if (item.i_number.Contains(f_id.ToString("X8")))
                    return item.i_description;

            return "UNKNOWN";
        }


        public string get_type(int f_id)
        {
            foreach (LTIndex item in _lookuplist)
                if (item.i_number.Contains(f_id.ToString("X8")))
                    return item.i_type;

            return "UK";
        }

        public string get_cdn(int f_id)
        {
            foreach (LTIndex item in _lookuplist)
                if (item.i_number.Contains(f_id.ToString("X8")))
                    return item.i_cdn;

            return "N/A";
        }


        public string[] get_all(int f_id)
        {
            string[] temp = new string[4];

            foreach (LTIndex item in _lookuplist)
            {
                if (item.i_number.Contains(f_id.ToString("X8")))
                {
                    temp[0] = item.i_type;
                    temp[1] = item.i_number;
                    temp[2] = item.i_description;
                    temp[3] = item.i_cdn;
                }
            }

            return temp;
        }

        // normal search
        public List<string[]> item_search(string sterm)
        {
            List<string[]> results = new List<string[]>();

            foreach (LTIndex item in _lookuplist)
            {
                if (item.i_description.ToLower().Contains(sterm.ToLower()))
                {
                    string[] temp = { item.i_type, item.i_number, item.i_description };
                    results.Add(temp);
                }
            }

            return results;
        }

        // regex search
        public List<string[]> item_search_regex(string sterm)
        {
            List<string[]> results = new List<string[]>();
            Regex search = new Regex(sterm);

            foreach (LTIndex item in _lookuplist)
            {
                if (search.IsMatch(item.i_description))
                {
                    string[] temp = { item.i_type, item.i_number, item.i_description };
                    results.Add(temp);
                }
            }

            return results;
        }


        protected void load_database_files()
        {
            // load FalloutNV
            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\falloutnv.db"), false, '\t');
            //fo3_csv.MissingFieldAction = MissingFieldAction.ReplaceByNull;

            while (fo3_csv.ReadNextRecord())
            {
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));
            }

            // loaded addons load order
            for (int x = 0, y = 0; x < _order.Length; x++, y++)
            {
                if (_order[x] == "DeadMoney.esm")
                    deadm = "0" + y.ToString();

                if (_order[x] == "HonestHearts.esm")
                    honesth = "0" + y.ToString();

                if (_order[x] == "OldWorldBlues.esm")
                    oldwb = "0" + y.ToString();

                if (_order[x] == "LonesomeRoad.esm")
                    lroad = "0" + y.ToString();
            }

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\lonesomeroad.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], (lroad != null) ? fo3_csv[0].Replace("01", lroad) : fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\oldworldblues.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], (oldwb != null) ? fo3_csv[0].Replace("01", oldwb) : fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\honesthearts.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], (honesth != null) ? fo3_csv[0].Replace("01", honesth) : fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\deadmoney.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], (deadm != null) ? fo3_csv[0].Replace("01", deadm) : fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\tribal.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\classic.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\caravan.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\mercenary.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

        }
        // ------------------------------------


        protected void load_database_files_no_order()
        {
            // load Fallout new vegas
            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\falloutnv.db"), false, '\t');

            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\lonesomeroad.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\oldworldblues.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\honesthearts.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\deadmoney.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\tribal.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\classic.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\caravan.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));

            fo3_csv = new CsvReader(new StreamReader(Application.StartupPath + "\\itemdb\\mercenary.db"), false, '\t');
            while (fo3_csv.ReadNextRecord())
                _lookuplist.Add(new LTIndex(fo3_csv[2], fo3_csv[4], fo3_csv[0]));
        }
    }
    // ==============================================================

}