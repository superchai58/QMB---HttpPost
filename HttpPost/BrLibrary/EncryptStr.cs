using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace BRLibrary
{
    public class EncryptStr
    {
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        public string OpenINI(string key, string sPath)
        {

            string section = "ConnectionList";
            string connectionString = "";
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            // section=配置节，key=键名，temp=上面，path=路径
            GetPrivateProfileString(section, key, "", temp, 255, sPath);
            
            if (temp.ToString() == "")
            {
                MessageBox.Show(sPath.ToString());
                MessageBox.Show("INI配置错误，请联系QMS！");
                return "";
            }

            //解密 密钥（ojVuhWn0dcN9f7KbGZJPwiaStqCe6DAMRp5BOYT1Im3UL8vyzs2kXlFQgxrHE4）
            connectionString = DecryptDES(temp.ToString(), "ojVuhWn0dcN9f7KbGZJPwiaStqCe6DAMRp5BOYT1Im3UL8vyzs2kXlFQgxrHE4");
            return connectionString;
        }

        private string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                string hKey = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                string OpenKey = "";
                string ServerIP = "", UID = "", PWD = "", DBName = "";
                string tvl;
                int tax, tbx, tcx = 0;

                //因为目前VB,C#共用INI文档，采用相同的加解密方式
                int decryptString_len = decryptString.Length;
                int decryptKey_len = decryptKey.Length;
                for (int i = 1; i <= decryptString_len; i++)
                {
                    tvl = decryptString.Substring(i - 1, 1);
                    tax = decryptKey.IndexOf(tvl) + 1;
                    tbx = tax;
                    if (tax == 0)
                    {
                        OpenKey = OpenKey + tvl;
                    }
                    else
                    {
                        tax = (tax - i - decryptString_len - tcx) % decryptKey_len;
                        if (tax < 0)
                            tax = tax + decryptKey_len;
                        if (tax == 0)
                            tax = decryptKey_len;
                        OpenKey = OpenKey + hKey.Substring(tax - 1, 1);
                    }
                    tcx = tbx + 1;
                }

                //解密后：Provider=SQLOLEDB.1;Password=pwd;Persist Security Info=True;User ID=sa;Initial Catalog=SPART;Data Source=10.;Network Library=DBMSSOCN
                //因为C#不能通过如上的VB连接字符串进行连接DB，故更改字符串的格式
                ServerIP = OpenKey.Substring(OpenKey.IndexOf("Source") + 6, OpenKey.IndexOf(";Network") - OpenKey.IndexOf("Source") - 6);
                DBName = OpenKey.Substring(OpenKey.IndexOf("Catalog") + 7, OpenKey.IndexOf(";Data") - OpenKey.IndexOf("Catalog") - 7);
                UID = OpenKey.Substring(OpenKey.IndexOf(" ID") + 3, OpenKey.IndexOf(";Initial") - OpenKey.IndexOf(" ID") - 3);
                PWD = OpenKey.Substring(OpenKey.IndexOf("Password") + 8, OpenKey.IndexOf(";Persist") - OpenKey.IndexOf("Password") - 8);
                OpenKey = "SERVER" + ServerIP + ";DATABASE" + DBName + ";UID" + UID + ";PWD" + PWD;
                return OpenKey;
            }
            catch
            {
                return "INI配置文档错误，请找QMS.";
            }
        }
    }
}
