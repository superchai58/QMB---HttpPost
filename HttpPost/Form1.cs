using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Collections;
using Newtonsoft.Json;
using System.Diagnostics;

namespace HttpPost
{
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
             string url1 = "http://10.97.1.53:8887/nest_inventory";
             string req1 = "{\"config\":\"dvt9.1_b4\",\"project\":\"prince\",\"manufacturer\":\"quanta\",\"site\":\"bk\"}";
             string url2 = "http://10.97.1.53:8889/nest_inventory";
             string req2 = "{\"config\":\"pvt_b4\",\"project\":\"sabrina\",\"manufacturer\":\"quanta\",\"site\":\"bk\"}";
             string ret1 = post(url1, req1);
             string ret2 = post(url2, req2);

          /*  string strJSON = "{\"response\":[{\"quantity\":362500,\"used\":109892,\"available\":252608}],\"result\":\"PASS\",\"reason\":\"\"}";
            Result r = JsonConvert.DeserializeObject<Result>(strJSON);
            this.textBox2.AppendText(r.response[0].quantity.ToString()+"\n");
            */
        }

        private string post(string url,string str)
        {
            textBox1.AppendText("url:" + url + " data:" + str + "\n");
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
            textBox2.AppendText(result + "\n");
            textBox2.AppendText("quantity =" + r.response[0].quantity.ToString() + "\n");
            textBox2.AppendText("used =" + r.response[0].used.ToString() + "\n");
            textBox2.AppendText("available =" + r.response[0].available.ToString() + "\n");
            MessageBox.Show(r.response[0].available.ToString());
            /*  DataSet ds = JsonConvert.DeserializeObject<DataSet>(result);
              DataTable dt = ds.Tables["response"];
              textBox2.AppendText(dt.Rows.Count.ToString() + "\n");
              textBox2.AppendText(result + "\n");*/
            return result;
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
