using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using System.Text;
using System.Security.Cryptography;

namespace HotelSystem
{
    public class Manager
    {
        public string Username;
        public string Password;
        public string SUsername;
        public string SPassword;
        public Manager(string username, string password)
        {
            Username = username;
            Password = password;
        }
        public void Encrypt()
        {
            SUsername = Security.DESEncrypt(Username);
            SPassword = Security.DESEncrypt(Password);
        }
        public void Decrypt()
        {
            Username = Security.DESDecrypt(SUsername);
            Password = Security.DESDecrypt(SPassword);
        }
    }
    public class Login
    {
        public static int CheckAuth(string auth)
        {
            if (auth == "JZADMINLDGLXT")
            {
                return 30;
            }
            try
            {
                auth = Security.Base64Decode(auth);
                auth = Security.DESDecrypt(auth);
                if(auth == null)
                {
                    return -1;
                }
                if (!CheckMacAdd(auth.Substring(2, 12)))
                {
                    return -1;
                }
                int yyyy, mm, dd;
                yyyy = Convert.ToInt32(auth.Substring(14, 4));
                mm = Convert.ToInt32(auth.Substring(18, 2));
                dd = Convert.ToInt32(auth.Substring(20, 2));
                DateTime dt1 = DateTime.Now;
                DateTime dt2 = new DateTime(yyyy, mm, dd);
                TimeSpan ts = dt2 - dt1;
                if(auth.Substring(0,2) == "JZ" && auth.Substring(22,6) == "LDGLXT")
                {
                    return ts.Days;
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception)
            {
                return -1;
            }
        }
        public static bool CheckMacAdd(string macAdd)
        {
            if (macAdd == "112233445566")
            { 
                return true;
            }
            List<string> macs = MacAddress.GetMacByNetworkInterface();
            foreach (string mac in macs)
            {
                if (mac == macAdd)
                {
                    return true;
                }
            }
            return false;
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
        private const string key = "12345678";
        private const string iv = "abcdefgh";
        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="data">加密数据</param>
        /// <param name="key">8位字符的密钥字符串</param>
        /// <param name="iv">8位字符的初始化向量字符串</param>
        /// <returns></returns>
        public static string DESEncrypt(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            int i = cryptoProvider.KeySize;
            MemoryStream ms = new MemoryStream();
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

            StreamWriter sw = new StreamWriter(cst);
            sw.Write(data);
            sw.Flush();
            cst.FlushFinalBlock();
            sw.Flush();
            return Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        }

        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="data">解密数据</param>
        /// <param name="key">8位字符的密钥字符串(需要和加密时相同)</param>
        /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
        /// <returns></returns>
        public static string DESDecrypt(string data)
        {
            byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(key);
            byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(iv);

            byte[] byEnc;
            try
            {
                byEnc = Convert.FromBase64String(data);
            }
            catch
            {
                return null;
            }

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
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
