using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Windows.Forms;
using XLib.Other;

namespace Fallout3VE
{
    class SaveDL
    {
        WebClient webClient;
        string _saves;

        public SaveDL() 
        { 
            webClient = new WebClient();
        }
        
        ~SaveDL() { }

        public string saves_txt { get { return _saves; } }

        public void get_saves()
        {
            byte[] saves_data = webClient.DownloadData("http://epicgeeks.net/fo3saves/saves.dat");
            _saves = Encoding.ASCII.GetString(saves_data);

            //string[] parts = Regex.Split(_saves, "");
        }

    }
}
