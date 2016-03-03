using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using WebEngion;
using FileIO;
using System.Diagnostics; 


namespace Customer
{


    public partial class Form1 : Form
    {

        public static string label1text = "";
        public static string label2text = "";
        public float percent;
        public static int Stid=0;

        FileManager clientPeer;

        public Form1()
        {
            InitializeComponent();

            Customer.FileTestsControler fc = new FileTestsControler();
            Customer.FileTestsControler.ConnectDatabase();

            //实例
            clientPeer = new FileManager();

            //注册文件传输协议
            clientPeer.AddHandler("FileCallResult", OnCall);
            //上传文件结果返回
            clientPeer.AddHandler("FileSaveResult", OnSave);

            clientPeer.Start("192.168.2.104", 21555);

            #region timer
            Timer timer = new Timer();
            timer.Interval = 10;

            timer.Tick += (s, e) =>
                {
                    this.Text = result;
                    this.label1.Text = label1text;
                    this.label2.Text = label2text;
                    this.progressBar1.Value = (int)(this.progressBar1.Maximum * percent);
                };
            this.Text = result;
            timer.Start();

            Timer timer2 = new Timer();
            timer2.Interval = 30;
            timer2.Tick += (s, e) =>
            {
                if (Stid != 0)
                {
                    //UploadTest t = Customer.FileTestsControler.FindTest(Stid);
                    //label1text = t.TestName;
                    //percent = t.DownloadLenth / (float)t.Filelenth;
                }
            };
            timer2.Start();
            #endregion
        }

        static string result="";


        //暂时用于保存文件的长度资料（可以后续扩展框架使可同步上传）
        byte[] filedatas;
        string filename;

        void SendFile(string path = "File.rar")
        {
            filename = path;
            Link.Chat.UserCall pu = new Link.Chat.UserCall();
            pu.filename = path;
            pu.password = "l123";
            pu.username = "user";

            StreamReader rd = new StreamReader(path); 
            filedatas = StreamToBytes(rd.BaseStream);
            rd.Close();

            pu.filelenth = filedatas.Length;

            //包体封装
            NetPacket p = new NetPacket();
            p.BeginWrite("FileSaveCall");                        //展开文件储存协议
            p.WriteObject<Link.Chat.UserCall>(pu);
            p.EncodeHeader();

            label2text += "user: " + pu.username + "  password: " + pu.username + "发送请求信息\n";
            if (clientPeer.Send(p)){ }
            else
               result = "发送失败，服务器断开";
        }


        //处理存储
        public void OnCall(NetPacket packet)
        {
            result = "Success";

            //从返回结果中获取任务ID
            Link.Chat.CallResult rs = packet.ReadObject<Link.Chat.CallResult>();
            int id = rs.callID;

            Form1.label2text += "任务请求成功，得到任务ID:"+id.ToString()+" \n";
            Stid = id;

            //文件数据包
            Link.Chat.FileProto proto = new Link.Chat.FileProto();
            proto.username = "leiming";
            proto.filename = filename ;
            proto.filedata = filedatas;

            Form1.label2text += "开始传输文件数据。。。\n";
            //包体封装
            NetPacket p = new NetPacket(512000000);
            p.BeginWrite("FileSave");                        //展开文件储存协议
            p.WriteObject<Link.Chat.FileProto>(proto);
            p.packageId = id;
            p.EncodeHeader();
            if (clientPeer.Send(p))
            {

            }
            else
                result = "发送失败，服务器断开";
        }

        public void OnSave(NetPacket packet)
        {
            result = "Save Success";

            Form1.label2text += "上传成功\n";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            if (File.Exists(str))
            {
                SendFile(str);
            }
            else
            {
                throw new Exception("no file find");
            }
        } 




        /* - - - - - - - - - - - - - - - - - - - - - - - -  
 * Stream 和 byte[] 之间的转换 
 * - - - - - - - - - - - - - - - - - - - - - - - */
        /// <summary> 
        /// 将 Stream 转成 byte[] 
        /// </summary> 
        public byte[] StreamToBytes(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }

        /// <summary> 
        /// 将 byte[] 转成 Stream 
        /// </summary> 
        public Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }


        /* - - - - - - - - - - - - - - - - - - - - - - - -  
         * Stream 和 文件之间的转换 
         * - - - - - - - - - - - - - - - - - - - - - - - */
        /// <summary> 
        /// 将 Stream 写入文件 
        /// </summary> 
        public void StreamToFile(Stream stream, string fileName)
        {
            // 把 Stream 转换成 byte[] 
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            // 设置当前流的位置为流的开始 
            stream.Seek(0, SeekOrigin.Begin);

            // 把 byte[] 写入文件 
            FileStream fs = new FileStream(fileName, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        /// <summary> 
        /// 从文件读取 Stream 
        /// </summary> 
        public Stream FileToStream(string fileName)
        {
            // 打开文件 
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            // 读取文件的 byte[] 
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            // 把 byte[] 转换成 Stream 
            Stream stream = new MemoryStream(bytes);
            return stream;
        }


    }










}
