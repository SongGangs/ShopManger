using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopMangerDemo
{
    class Database
    {//从Web.config中获取连接字符串，然后实例化SqlConnectio对象
        public static SqlConnection ReturnConn()
        {
            //从Web.config的AppSettings中获取连接字符串
            string strConn = System.Configuration.ConfigurationSettings.AppSettings["UserConnection"];
            //使用连接字符串实例化SqlConnection对象
            SqlConnection Conn = new SqlConnection(strConn);
            //如果当前连接状态为关闭状态则打开连接
            if (Conn.State.Equals(ConnectionState.Closed))
            {
                Conn.Open();
            }
            return Conn;   //返回SqlConnection对象
        }
        //根据存储过程名称和参数数组创建SqlCommand对象
        public static SqlCommand CreatCmd(string procName, SqlParameter[] prams)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = Conn;
            cmd.CommandText = procName;
            if (prams != null)
            {
                foreach (SqlParameter paramenter in prams)
                {
                    if (paramenter != null)
                    {
                        cmd.Parameters.Add(paramenter);
                    }
                }
            }
            return cmd;
        }
        //根据存储过程名称，参数数组和特定的SqlConnection连接返回SqlCommand
        public static SqlCommand CreatCmd(string procName,
            SqlParameter[] prams, SqlConnection Conn)
        {
            SqlConnection SqlConn = Conn;
            if (SqlConn.State.Equals(ConnectionState.Closed))
            {
                SqlConn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = SqlConn;
            cmd.CommandText = procName;
            if (prams != null)
            {
                foreach (SqlParameter paramenter in prams)
                {
                    if (paramenter != null)
                    {
                        cmd.Parameters.Add(paramenter);
                    }
                }
            }
            return cmd;

        }
        //根据存储过程名称和SqlConnection对象返回SqlCommand对象
        public static SqlCommand CreatCmd(string procName, SqlConnection Conn)
        {
            SqlConnection SqlConn = Conn;
            if (SqlConn.State.Equals(ConnectionState.Closed))
            {
                SqlConn.Open();
            }
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = SqlConn;
            cmd.CommandText = procName;
            return cmd;

        }
        //根据存储过程名称返回SqlCommand对象
        public static SqlCommand CreatCmd(string procName)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Connection = Conn;
            cmd.CommandText = procName;
            return cmd;

        }
        //执行存储过程并返回SqlDataReader对象
        public static SqlDataReader RunProcGetReader(string procName, SqlParameter[] prams)
        {
            SqlCommand Cmd = CreatCmd(procName, prams);
            SqlDataReader Dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return Dr;

        }
        //使用指定链接执行存储过程，返回SqlDataReader对象
        public static SqlDataReader RunProcGetReader(string procName,
            SqlParameter[] prams, SqlConnection Conn)
        {
            SqlCommand Cmd = CreatCmd(procName, prams, Conn);
            SqlDataReader Dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return Dr;

        }
        //执行存储过程并返回SqlDataReader对象
        public static SqlDataReader RunProcGetReader(string procName, SqlConnection Conn)
        {
            SqlCommand Cmd = CreatCmd(procName, Conn);
            SqlDataReader Dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return Dr;

        }
        //根据指定存储过程名称执行存储过程，返回SqlDataReader对象
        public static SqlDataReader RunProcGetReader(string procName)
        {
            SqlCommand Cmd = CreatCmd(procName);
            SqlDataReader Dr = Cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return Dr;

        }
        //执行存储过程返回DataTable对象
        public static DataTable RunProcGetTable(string procName,
            SqlParameter[] prams, SqlConnection conn)
        {
            SqlCommand Cmd = CreatCmd(procName, prams, conn);
            SqlDataAdapter Dtr = new SqlDataAdapter();
            DataSet Ds = new DataSet();
            Dtr.SelectCommand = Cmd;
            Dtr.Fill(Ds);
            DataTable Dt = Ds.Tables[0];
            conn.Close();
            return Dt;
        }
        public static DataTable RunProcGetTable(string procName, SqlParameter[] prams)
        {
            SqlConnection conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, prams, conn);
            SqlDataAdapter Dtr = new SqlDataAdapter();
            DataSet Ds = new DataSet();
            Dtr.SelectCommand = Cmd;
            Dtr.Fill(Ds);
            DataTable Dt = Ds.Tables[0];
            conn.Close();
            return Dt;
        }
        public static DataTable RunProcGetTable(string procName)
        {
            SqlConnection conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, conn);
            SqlDataAdapter Dtr = new SqlDataAdapter();
            DataSet Ds = new DataSet();
            Dtr.SelectCommand = Cmd;
            Dtr.Fill(Ds);
            DataTable Dt = Ds.Tables[0];
            conn.Close();
            return Dt;
        }

        //执行存储过程，返回影响的行数
        public static int RunExecute(string procName)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, Conn);
            int intResult = Cmd.ExecuteNonQuery();
            Conn.Close();
            return intResult;
        }
        public static int RunExecute(string procName, SqlParameter[] prams)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, prams, Conn);

            int intResult = Cmd.ExecuteNonQuery();
            Conn.Close();
            return intResult;
        }
        //执行存储过程，返回首行首个字段结果
        public static int RunExecuteScalar(string procName)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, Conn);
            int intResult = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();
            return intResult;
        }
        public static int RunExecuteScalar(string procName, SqlParameter[] prams)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, prams, Conn);
            int intResult = Convert.ToInt32(Cmd.ExecuteScalar());
            Conn.Close();
            return intResult;
        }
        //执行存储过程，返回SqlDataAdapter对象
        public static SqlDataAdapter RunSqlDataAdapter(string procName,
            SqlParameter[] prams)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, prams, Conn);
            SqlDataAdapter Dtr = new SqlDataAdapter();
            Dtr.SelectCommand = Cmd;
            Conn.Close();
            return Dtr;
        }
        public static SqlDataAdapter RunSqlDataAdapter(string procName)
        {
            SqlConnection Conn = ReturnConn();
            SqlCommand Cmd = CreatCmd(procName, Conn);
            SqlDataAdapter Dtr = new SqlDataAdapter();
            Dtr.SelectCommand = Cmd;
            Conn.Close();
            return Dtr;
        }
        //执行SQL语句，返回DataSet对象
        public static DataSet RunDataSet(String sql)
        {
            DataSet ds = new DataSet();
            SqlConnection Con = ReturnConn();
            SqlDataAdapter sqldater = new SqlDataAdapter(sql, Con);
            sqldater.Fill(ds);
            Con.Close();
            return ds;
        }
        //直接执行SQL语句
        public static int ExecuteNonQuery(string sql)
        {
            int i;
            SqlConnection Con = ReturnConn();
            SqlCommand sqlcom = new SqlCommand(sql, Con);
            i = sqlcom.ExecuteNonQuery();
            return i;

        }
        //返回指定SQL语句的SqlDataAdapter对象
        public static SqlDataAdapter RunDataAter(String sql)
        {
            SqlConnection Con = ReturnConn();
            SqlDataAdapter sqldater = new SqlDataAdapter(sql, Con);
            Con.Close();
            return sqldater;
        }
 
        //生成关键字
        public static int GenKey(string tableName, string keyFieldName)
        {
            string sql = "select max(" + keyFieldName + ") from " + tableName;
            DataSet ds = RunDataSet(sql);
            int keyVal;
            try
            {
                keyVal = int.Parse(ds.Tables[0].Rows[0][0].ToString());
            }
            catch (Exception er)
            {
                keyVal = 0;
            }

            return keyVal + 1;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tablename"></param>
        /// <param name="keyField"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static int update(string tablename, string keyField, DataTable dt)
        {
            string sqlStr;
            string keyVal;
            int key = 0;
            SqlDataAdapter da;
            SqlCommandBuilder sb;
            DataSet ds;
            bool isNew = false;
            foreach (DataRow dr in dt.Rows)
            {

                isNew = false;
                //从数据库中取一条记录
                keyVal = dr[keyField].ToString();
                if (keyVal == string.Empty)
                    keyVal = "0";

                sqlStr = "select * from " + tablename + " where " + keyField + "=" + keyVal;
                da = new SqlDataAdapter(sqlStr, ReturnConn());
                ds = new DataSet();
                da.Fill(ds);

                //把当前记录数据，拷贝到ds中
                DataRow drT;
                if (ds.Tables[0].Rows.Count > 0)
                    drT = ds.Tables[0].Rows[0];
                else
                {
                    drT = ds.Tables[0].NewRow();
                    ds.Tables[0].Rows.Add(drT);
                    isNew = true;
                }
                copyRow(dr, drT);
                if (isNew)
                {
                    drT[keyField] = GenKey(tablename, keyField);
                }
                key = int.Parse(drT[keyField].ToString());
                sb = new SqlCommandBuilder(da);

                da.Update(ds);
            }

            return key;
        }

        private static void copyRow(DataRow f, DataRow t)
        {
            foreach (DataColumn dc in f.Table.Columns)
            {
                try
                {
                    t[dc.ColumnName] = f[dc.ColumnName];
                }
                catch (Exception er)
                { }
            }
        }


        public static Int64 update2(string tablename, string keyField, DataTable dt)
        {
            string sqlStr;
            string keyVal;
            Int64 key = 0;
            SqlDataAdapter da;
            SqlCommandBuilder sb;
            DataSet ds;
            bool isNew = false;
            foreach (DataRow dr in dt.Rows)
            {

                isNew = false;
                //从数据库中取一条记录
                keyVal = dr[keyField].ToString();
                if (keyVal == string.Empty)
                    keyVal = "0";

                sqlStr = "select * from " + tablename + " where " + keyField + "=" + keyVal;
                da = new SqlDataAdapter(sqlStr, ReturnConn());
                ds = new DataSet();
                da.Fill(ds);

                //把当前记录数据，拷贝到ds中
                DataRow drT;
                if (ds.Tables[0].Rows.Count > 0)
                    drT = ds.Tables[0].Rows[0];
                else
                {
                    drT = ds.Tables[0].NewRow();
                    ds.Tables[0].Rows.Add(drT);
                    isNew = true;
                }
                copyRow(dr, drT);
                key =Int64.Parse(drT[keyField].ToString());
                sb = new SqlCommandBuilder(da);

                da.Update(ds);
            }

            return key;
        }
    }
}
