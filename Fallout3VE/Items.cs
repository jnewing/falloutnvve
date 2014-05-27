using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Threading;
using XLib.Other;
using XLib.Converters;

namespace Fallout3VE.Items
{
    class SGItems
    {
        bool updated_qty = false;
        bool updated_fid = false;
        string item_type, item_description;
        byte[] item_raw, item_extra;
        int item_offset, item_sid, item_fid, item_old_fid, item_qty;
        float item_cnd;
        
        public SGItems() { }
        public SGItems(string itype, byte[] iraw, byte[] iextra, int ioff, int isid, int ifid, int ioldfid, int iqty, float icnd, string idesc)
        {
            item_type = itype;
            item_raw = iraw;
            item_extra = iextra;
            item_offset = ioff;
            item_sid = isid;
            item_fid = ifid;
            item_old_fid = ioldfid;
            item_qty = iqty;
            item_cnd = icnd;
            item_description = idesc;
        }

        ~SGItems() { }

        public bool update_qty { get { return updated_qty; } set { updated_qty = value; } }
        public bool update_fid { get { return updated_fid; } set { updated_fid = value; } }
        public string type { get { return item_type; } set { item_type = value; } }
        public byte[] raw { get { return item_raw; } set { item_raw = value; } }
        public byte[] extra { get { return item_extra; } set { item_extra = value; } }
        public int offset { get { return item_offset; } set { item_offset = value; } }
        public int sid { get { return item_sid; } set { item_sid = value; } }
        public int fid { get { return item_fid; } set { item_fid = value; } }
        public int old_fid { get { return item_old_fid; } set { item_old_fid = value; } }
        public int qty { get { return item_qty; } set { item_qty = value; } }
        public float cdn { get { return item_cnd; } set { item_cnd = value; } }
        public string descrip { get { return item_description; } set { item_description = value; } }

    }

    
    class ItemsClass
    {
        List<SGItems> items = new List<SGItems>();
        int[] lookup_table;
        byte[] itemdata;
        int inv_offset;

        public ItemsClass() { }
        ~ItemsClass() { }

        public ItemsClass(byte[] idata, int ioffset, int[] lut)
        {
            itemdata = new byte[idata.Length];
            itemdata = idata;
            inv_offset = ioffset;
            lookup_table = lut;

            // do teh verk!
            process_inventory();
        }
        
        public List<SGItems> inventory { get { return items; } }
        

        protected void process_inventory()
        {   
            // tokenize
            int[] tokens = tokenize_items(itemdata);

            for (int x = 0; x < (tokens.Length - 1); x++)
            {
                int i_off = inv_offset + tokens[x];
                byte[] item = ByteFunctions.BytePeice(itemdata, tokens[x], tokens[x + 1] - tokens[x]);

                if (item.Length > 9)
                {
                    // parse the item
                    int[] idata = parse_item(item);

                    if (idata[0] != 0x00 && idata[0] < lookup_table.Length)
                    {
                        // make new item instance
                        SGItems _item = new SGItems();
                        _item.raw = item;
                        _item.offset = i_off;
                        _item.sid = idata[0];
                        _item.qty = idata[1];
                        _item.cdn = idata[2];
                        _item.fid = lookup_table[idata[0]];
                        items.Add(_item);
                    }
                }
            }
        }

        public void parse_chunk(int[] tokens, int start, int stop)
        {
            for (int x = start; x < (stop - 1); x++)
            {
                int i_off = inv_offset + tokens[x];
                byte[] item = ByteFunctions.BytePeice(itemdata, tokens[x], tokens[x + 1] - tokens[x]);

                if (item.Length > 9)
                {
                    // parse the item
                    int[] idata = parse_item(item);

                    if (idata[0] != 0x00)
                    {
                        // make new item instance
                        SGItems _item = new SGItems();
                        _item.raw = item;
                        _item.offset = i_off;
                        _item.sid = idata[0];
                        _item.qty = idata[1];
                        _item.cdn = idata[2];
                        _item.fid = lookup_table[idata[0]];
                        items.Add(_item);
                    }
                }
            }
        }

        protected int[] parse_item(byte[] sitem)
        {
            int[] iinfo = new int[3];
            
            // process fid
            iinfo[0] = Conv.ToInt32(ByteFunctions.BytePeice(sitem, 1, 3), true);

            // process amount
            bool chk = true;
            for (int x = 5; x < 9; x++)
            {
                if (sitem[x] == 0x7C)
                    chk = false;

                if (!chk)
                    break;
            }

            if (chk)
                iinfo[1] = Conv.ToInt32(ByteFunctions.BytePeice(sitem, 5, 4), false);
            else
                iinfo[1] = -1;

            // check for condition
            bool cdnchk = false;
            int cdnoff = 0;
            if (sitem.Length > 15)
            {
                for (int x = 10; x < sitem.Length - 1; x++)
                {
                    if ((sitem[x] == 0x7C && sitem[x + 1] == 0x25) && (sitem.Length >= x + 5))
                    {
                        cdnchk = true;
                        cdnoff = x + 3;
                        break;
                    }
                }
            }

            if (cdnchk)
            {
                byte[] fval = ByteFunctions.BytePeice(sitem, cdnoff, 4);
                iinfo[2] = (int)Conv.ToSingle(fval, false);
            }
            else
            {
                iinfo[2] = -1;
            }


            return iinfo;
        }


        /*
        | -------------------------------------
        |  tokenize_items ( byte[], byte[] )
        | -------------------------------------
        |
        |
        */
        protected int[] tokenize_items(byte[] token_data)
        {
            int count = 0;
            for (int x = 0; x < token_data.Length; x++)
            {
                if (token_data.Length > x + 4)
                {
                    if (token_data[x] == 0x7c && token_data[x + 1] != 0x7c && token_data[x + 2] != 0x7c && token_data[x + 3] != 0x7c && token_data[x + 4] == 0x7c)
                    {
                        count++;
                        x += 4;
                    }
                }
                else
                {
                    count++;
                    x += 4;
                }
            }

            int[] tokens = new int[count];
            for (int x = 0, tok = 0; x < token_data.Length; x++)
            {
                if (token_data.Length > x + 4)
                {
                    if (token_data[x] == 0x7c && token_data[x + 1] != 0x7c && token_data[x + 2] != 0x7c && token_data[x + 3] != 0x7c && token_data[x + 4] == 0x7c)
                    {
                        tokens[tok] = x;
                        tok++;
                        x += 4;
                    }
                }
                else
                {
                    tokens[tok] = token_data.Length;
                    tok++;
                    x += 4;
                }
            }

            return tokens;
        }
        // ------------------------------------

        /*
        | -------------------------------------
        |  form_id_index ( int )
        | -------------------------------------
        |
        |
        */
        private int form_id_index(int fid)
        {
            for (int x = 0; x < lookup_table.Length; x++)
            {
                if (lookup_table[x] == fid)
                    return x;
            }

            return 0;
        }
        // ------------------------------------
    }
}
