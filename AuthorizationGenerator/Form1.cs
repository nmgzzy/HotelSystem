using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Forms;


namespace AuthorizationGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonEn_Click(object sender, EventArgs e)
        {
            string code;
            List<string> s = MacAddress.GetMacByNetworkInterface();
            if (s.Count>0)
            {
                code = Security.Base64Encode(s[0]);
                this.textBoxMac.Text = code;
            }
            else
            {
                MessageBox.Show("生成失败");
            }
        }
    }

    public static class MacAddress
    {
        /// 通过NetworkInterface读取网卡Mac
        public static List<string> GetMacByNetworkInterface()
        {
            List<string> macs = new List<string>();
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                macs.Add(ni.GetPhysicalAddress().ToString());
            }
            return macs;
        }
    }

    public static class Security
    {
        public static string Base64Encode(string source)
        {
            return Base64Encode(Encoding.UTF8, source);
        }
        public static string Base64Encode(Encoding encodeType, string source)
        {
            string encode = string.Empty;
            byte[] bytes = encodeType.GetBytes(source);
            try
            {
                encode = Convert.ToBase64String(bytes);
            }
            catch
            {
                encode = source;
            }
            return encode;
        }
        public static string Base64Decode(string result)
        {
            return Base64Decode(Encoding.UTF8, result);
        }
        public static string Base64Decode(Encoding encodeType, string result)
        {
            string decode = string.Empty;
            byte[] bytes = Convert.FromBase64String(result);
            try
            {
                decode = encodeType.GetString(bytes);
            }
            catch
            {
                decode = result;
            }
            return decode;
        }
    }

}
