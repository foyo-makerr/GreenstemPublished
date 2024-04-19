using System;
using System.Data;
using System.Data.SqlClient;
//using System.Windows.Forms;
//using DevExpress.DataAccess.ConnectionParameters;
//using DevExpress.DataAccess.Sql;
//using DevExpress.XtraSplashScreen;


namespace GreenStem.ClassModules
{

    public class clsConnection
    {
        public object strDbFilename = "";
        public object strDbFilenameLocal = "";

        public static string strConnectionADODB;
        public static SqlTransaction objSQLTransaction;
        public static SqlConnection objSQLConnection;


        public static string TempSQLServerName;
      

        public static DataTable dt = new DataTable(), dt1 = new DataTable(), dt2 = new DataTable(), dt3 = new DataTable(), dt4 = new DataTable(), dt5 = new DataTable(), dtComp = new DataTable(), dtCode = new DataTable(), dtF1 = new DataTable();
        public static DataTable dtStock1 = new DataTable(), dtStock2 = new DataTable(), dtStock3 = new DataTable();
        public static DataTable dtCust1 = new DataTable(), dtCust2 = new DataTable(), dtCust3 = new DataTable();
        public static DataTable dtSupp1 = new DataTable(), dtSupp2 = new DataTable(), dtSupp3 = new DataTable();
        public static DataRow dr, dr1, dr2, dr3, dr4, dr5;

        public static DataSet ds1;
        public static SqlCommand sqlCmd = new SqlCommand();
        public static SqlDataAdapter da;

    

        public static void gf_CommitTrans()
        {
            objSQLTransaction.Commit();
            objSQLConnection.Close();
            objSQLTransaction.Dispose();
        }

        public static void gf_RollbackTrans()
        {
            objSQLTransaction.Rollback();
            objSQLConnection.Close();
            objSQLTransaction.Dispose();
        }

  

    
     
    }
}