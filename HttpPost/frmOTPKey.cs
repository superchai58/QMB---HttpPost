using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace HttpPost
{
    
    public partial class frmOTPKey : Form
    {
        [DllImport("kernel32.dll")]
        public static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        public static string sPath = Application.StartupPath + "\\OLEODBC.INI";
        public frmOTPKey()
        {
            InitializeComponent();
        }

        private void frmOTPKey_Load(object sender, EventArgs e)
        {
            BRLibrary.EncryptStr myEncryptStr = new BRLibrary.EncryptStr();
            gParameters.strCnn_BK = myEncryptStr.OpenINI("Connect_SEL_BK", sPath);

            Dblibrary.OTPKey OTPKey = new Dblibrary.OTPKey();
            DataTable dt = OTPKey.GetOTPKey_PythonPath();
            this.dgOTPKey_Python.DataSource = dt;
            Application.DoEvents();
            
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            this.btnRun.Enabled = false;
            this.Cursor = Cursors.WaitCursor;
            string strPlatForm_Name = "", strURL = "", strRequest = "";
            HttpWebRequest req;

            for (int i = 0; i < this.dgOTPKey_Python.Rows.Count; i++)
            {
                strPlatForm_Name = "";
                strURL = "";
                strRequest = "";

                strPlatForm_Name = this.dgOTPKey_Python.Rows[i].Cells[0].Value.ToString();
                strURL = this.dgOTPKey_Python.Rows[i].Cells[1].Value.ToString();
                strRequest = this.dgOTPKey_Python.Rows[i].Cells[2].Value.ToString();

                this.post(strPlatForm_Name, strURL, strRequest);
            }

            this.Cursor = Cursors.Default;
            this.btnRun.Enabled = true;
            this.Close();
            Application.Exit();
        }

        private void post(string PlatForm_name, string url, string str)
        {
            string result = "";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(str);//把字符串转换为字节

            req.ContentLength = data.Length; //请求长度
            //textBox1.AppendText(req.ToString() + "\n");
            using (Stream reqStream = req.GetRequestStream()) //获取
            {
                reqStream.Write(data, 0, data.Length);//向当前流中写入字节
                reqStream.Close(); //关闭当前流
            }
            HttpWebResponse resp = (HttpWebResponse)req.GetResponse(); //响应结果
            Stream stream = resp.GetResponseStream();
            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                result = reader.ReadToEnd();
            }
         
            Result r = JsonConvert.DeserializeObject<Result>(result);
            Dblibrary.OTPKey OTPKey = new Dblibrary.OTPKey();
          
            OTPKey.InsertOTPKey_Rport_WebPath(PlatForm_name, Convert.ToInt32(r.response[0].available.ToString()));
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.btnRun.PerformClick();
        }

        public class Result
        {
            public response[] response;
            //public response[] response;
            public string result;
            public string reason;
        }

        public class response
        {
            public string quantity;
            public string used;
            public string available;
        }
    }
}
