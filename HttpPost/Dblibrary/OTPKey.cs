using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace HttpPost.Dblibrary
{
    class OTPKey : QMSSDK.Db.WinForm
    {

        public OTPKey()
        {
            QMSSDK.Db.Connections.CreateCn(gParameters.strCnn_BK);
        }

        public DataTable GetOTPKey_PythonPath()
        {
            string strSQL = "Select * From OTPKey_WebPath";
            DataTable dt = this.Execute(strSQL);
            return dt;
        }

        public void InsertOTPKey_Rport_WebPath(string PlatForm_Name, int OTPKey_Count)
        {
            DataSet ds;
            SqlParameter[] sp = new SqlParameter[2];
            sp[0] = new SqlParameter("@PlatForm_Name", SqlDbType.VarChar);
            sp[0].Value = PlatForm_Name;
            sp[1] = new SqlParameter("@OTPKey_Count", SqlDbType.Int);
            sp[1].Value = OTPKey_Count;
            this.spName = "InsertOTPKey_Rport_WebPath";
            this.ExecuteReturnless(sp);
            QMSSDK.Db.Connections.CloseCn();
        }
    }
}
