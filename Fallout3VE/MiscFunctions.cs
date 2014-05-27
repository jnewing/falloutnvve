using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fallout3VE.MiscFunctions
{
    class MFunctions
    {

        public static bool valid_file_header(byte[] header_data)
        {
            byte[] valid_header = { 0x46, 0x4F, 0x33, 0x53, 0x41, 0x56, 0x45, 0x47, 0x41, 0x4D, 0x45 };
            bool match = true;
            for (int x = 0; x < valid_header.Length; x++)
            {
                if (header_data[x] != valid_header[x])
                    match = false;

                if (!match)
                    break;
            }
            
            return match;
        }


        public static int[] tokenize(byte[] token_data, char delim)
        {
            int count = 0;
            for (int x = 0; x < token_data.Length; x++)
                if (token_data[x] == delim)
                    count++;

            int[] tokens = new int[count];
            for (int x = 0, tok = 0; x < token_data.Length; x++)
            {
                if (token_data[x] == delim)
                {
                    tokens[tok] = x;
                    tok++;
                }
            }

            return tokens;
        }

    }
}
