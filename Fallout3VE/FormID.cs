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


namespace Fallout3VE.FormID
{
    class FormIDTable
    {
        byte[] form_id_table_data;
        int[] form_id_table;

        public FormIDTable() { }
        ~FormIDTable() { }

        public FormIDTable(byte[] fiddata)
        {
            form_id_table_data = fiddata;
            build_fid_table();
        }

        
        public int[] fid_table { get { return form_id_table; } }

        
        protected void build_fid_table()
        {   
            
            int identifiers = BitConverter.ToInt32(form_id_table_data, 0);
            form_id_table = new int[identifiers];

            for (int x = 0; x < form_id_table.Length; x++)
            {
                form_id_table[x] = BitConverter.ToInt32(form_id_table_data, (x * 0x4));
            }
        }

    }
}
