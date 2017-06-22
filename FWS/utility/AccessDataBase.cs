using System;
using System.Data;
using System.Data.OleDb;
using System.Configuration;
using System.Collections.Generic;


namespace FWS.utility
{
    public class AccessDataBase
    {
        /// <summary>
        /// 连接数据库字符串
        /// </summary>
        private string connectionString;

        /// <summary>
        /// 存储数据库连接（保护类，只有由它派生的类才能访问）
        /// </summary>
        protected OleDbConnection Connection;

        /// <summary>
        /// 构造函数：数据库的默认连接
        /// </summary>
        public AccessDataBase()
{

            
            string connStr;
 
            connStr = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.ToString();
 //connStr = System.Configuration.con.AppSettings["ConnectionString"].ToString(); //从web.config配置中读取

            connectionString = connStr;
//connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + HttpContext.Current.Request.PhysicalapplicationPath + connStr;
// connectionString = System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"].ToString();
//

            Connection = new OleDbConnection(connectionString);
}

        /// <summary>
        /// 构造函数：带有参数的数据库连接
        /// </summary>
        /// <param name="newConnectionString"></param>
        public AccessDataBase(string newConnectionString)
        {
            //connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + HttpContext.Current.Request.PhysicalApplicationPath + newConnectionString;
            connectionString = newConnectionString;
            Connection = new OleDbConnection(connectionString);
        }

        /// <summary>
        /// 获得连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }


        /// <summary>
        /// 执行SQL语句没有返回结果，如：执行删除、更新、插入等操作
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>操作成功标志</returns>
        public bool ExeSQL(string strSQL)
        {
            bool resultState = false;

            Connection.Open();
            OleDbTransaction myTrans = Connection.BeginTransaction();
            OleDbCommand command = new OleDbCommand(strSQL, Connection, myTrans);

            try
            {
                command.ExecuteNonQuery();
                myTrans.Commit();
                resultState = true;
               
            }
            catch
            {
                myTrans.Rollback();
                resultState = false;
            }
            finally
            {
                Connection.Close();
            }
            return resultState;
        }

        /// <summary>
        /// 执行SQL语句返回结果到DataReader中
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>dataReader</returns>
        private OleDbDataReader ReturnDataReader(string strSQL)
        {
            Connection.Open();
            OleDbCommand command = new OleDbCommand(strSQL, Connection);
            OleDbDataReader dataReader = command.ExecuteReader();
            Connection.Close();

            return dataReader;
        }

        /// <summary>
        /// 执行SQL语句返回结果到DataSet中
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>DataSet</returns>
        public DataSet ReturnDataSet(string strSQL)
        {
            Connection.Open();
            DataSet dataSet = new DataSet();
            OleDbDataAdapter OleDbDA = new OleDbDataAdapter(strSQL, Connection);
            OleDbDA.Fill(dataSet, "objDataSet");

            Connection.Close();
            return dataSet;
        }

        /// <summary>
        /// 执行一查询语句，同时返回查询结果数目
        /// </summary>
        /// <param name="strSQL"></param>
        /// <returns>sqlResultCount</returns>
        public int ReturnSqlResultCount(string strSQL)
        {
            int sqlResultCount = 0;

            try
            {
                Connection.Open();
                OleDbCommand command = new OleDbCommand(strSQL, Connection);
                OleDbDataReader dataReader = command.ExecuteReader();

                while (dataReader.Read())
                {
                    sqlResultCount++;
                }
                dataReader.Close();
            }
            catch
            {
                sqlResultCount = 0;
            }
            finally
            {
                Connection.Close();
            }
            return sqlResultCount;
        }

        public void coloseCon()
        {
         
            Connection.Close();
        }

       /// <summary>
       /// 批量数据
       /// </summary>
       /// <param name="sqlArray"></param>

        public void insertToAccessByBatch(List<string> sqlArray)//String[]
        {

            try
            {



                //OleDbConnection aConnection = new OleDbConnection(DB.getConnectStr());

                //aConnection.Open();
                Connection.Open();
                OleDbTransaction transaction = Connection.BeginTransaction();
                OleDbCommand aCommand = new OleDbCommand();
                aCommand.Connection = Connection;

                aCommand.Transaction = transaction;
                int count = sqlArray.Count;
                for (int i = 0; i < count; i++)
                {

                    aCommand.CommandText = sqlArray[i];
                    aCommand.ExecuteNonQuery();

              //      LogHelper.log(Convert.ToString(i));

                }

                transaction.Commit();

                Connection.Close();

            }

            catch (Exception e)
            {

              //  LogHelper.log(e.Message);
           
            }

        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="sqlStr"></param>
        public void deleteDt(string sqlStr)
        {
            Connection.Open();
            OleDbCommand odc = new OleDbCommand(sqlStr, Connection);
            odc.ExecuteNonQuery();
            Connection.Close();
        }

    }


}
