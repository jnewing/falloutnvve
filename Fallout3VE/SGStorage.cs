using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLib.Other;

namespace Fallout3VE
{
    /*
    | ===============================================================
    |  SGData
    | ===============================================================
    | 
    */
    public class SGData
    {
        string p_name, p_location, p_time, p_karma;
        int pstr, pper, pend, pcha, pint, pagi, pluc;
        int p_experiance, p_level;

        public string player_name { get { return p_name; } set { p_name = value; }}
        public string player_location { get { return p_location; } set { p_location = value; }}
        public string player_time { get { return p_time; } set { p_time = value; }}
        public int special_str { get { return pstr; } set { pstr = value; }}
        public int special_per { get { return pper; } set { pper = value; }}
        public int special_end { get { return pend; } set { pend = value; }}
        public int special_cha { get{ return pcha; } set { pcha = value; }}
        public int special_int { get { return pint; } set { pint = value; }}
        public int special_agi { get { return pagi; } set { pagi = value; }}
        public int special_luc { get { return pluc; } set { pluc = value; }}
        public int player_exp { get { return p_experiance; } set { p_experiance = value; }}
        public int player_level { get { return p_level; } set { p_level = value; }}
        public string player_karma{ get { return p_karma; } set { p_karma = value; }}

    }
    // ==============================================================
    
}
