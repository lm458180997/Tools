using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace Server
{
    public class FileTestsControler
    {

       static MySqlConnection mycon;

        public FileTestsControler() { }

        public static void ConnectDatabase()
        {
            string constr = "server=203.195.179.183;port=11567;"
            + "User Id=root;password=leileimin318;Database=FileSave";
            mycon = new MySqlConnection(constr);
        }

        public static UploadTest FindTest(int id)
        {
            UploadTest test = new UploadTest();
            MySqlCommand mycmd = new MySqlCommand("select * from UploadTests where TestID="
                + id.ToString(), mycon);
            mycon.Open();
            MySqlDataReader reader = mycmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.HasRows)
                {

                    test.TestID = reader.GetInt32(0);
                    test.TestName = reader.GetString(1);
                    test.IsBegin = reader.GetInt32(2);
                    test.Filelenth = reader.GetInt32(3);
                    test.IsUploadOver = reader.GetInt32(4);
                    test.DownloadLenth = reader.GetInt32(5);
                }
            }
            mycon.Close();
            return test;
        }

        public static UploadTest UpdateData(int id,int read , int len)
        {
            UploadTest test = new UploadTest();
            MySqlCommand mycmd = new MySqlCommand("update UploadTests set FileLenth="+ len.ToString()+
               ",DownloadLenth=" + read.ToString() + " where TestID="
                + id.ToString(), mycon);
            mycon.Open();
            mycmd.ExecuteReader();
            mycon.Close();
            return test;
        }

        public static int AddNewTest(string fileneme, int filelenth)
        {
            int id = 0;
            MySqlCommand mycmd =
                new MySqlCommand("insert into UploadTests(TestName,IsBegin,FileLenth,IsUploadOver,DownloadLenth)"
            + "values ('" + fileneme + "',0,"+filelenth.ToString()+",0,0);", mycon);
            mycon.Open();
            try
            {
                mycmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                String message = ex.Message;
                Console.WriteLine("插入数据失败了！" + message);
            }
            /////Insert/////
            mycon.Close();
            mycmd = new MySqlCommand("select max(TestID) from UploadTests;", mycon);
            mycon.Open();
            MySqlDataReader reader = mycmd.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetInt32(0);
            }
            mycon.Close();

            return id;
        }


    }

    public class UploadTest
    {
        public int TestID;
        public string TestName;
        public int IsBegin;
        public int Filelenth;
        public int IsUploadOver;
        public int DownloadLenth;
        public UploadTest() { }
        public UploadTest(int _id, string name, int isbegin, int len, int isover, int dwldlen)
        {
            TestID = _id;
            TestName = name;
            IsBegin = isbegin;
            Filelenth = len;
            IsUploadOver = isover;
            DownloadLenth = dwldlen;
        }
    }
}
