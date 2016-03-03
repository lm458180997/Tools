using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WebEngion;
using FileIO;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
namespace Server
{
    public partial class Form1 : Form
    {
        public static string label1text = "";
        public static string label2text = "";
        public float percent;
        FileServer server;

        public Form1()
        {
            InitializeComponent();


            Timer timer = new Timer();
            timer.Interval = 10;
            timer.Tick += (s, e) =>
                {
                    percent = WebEngion.TCPPeer.percent;
                    this.label1.Text = label1text;
                    this.label2.Text = label2text;
                    this.progressBar1.Value = (int)(this.progressBar1.Maximum * percent);
                };
            timer.Start();

            Timer timer2 = new Timer();
            timer2.Interval = 30;
            timer2.Tick += (s, e) =>
            {
                if (WebEngion.TCPPeer.id != 0)
                {
                   //UploadTest t = Server.FileTestsControler.FindTest(WebEngion.TCPPeer.id);
                   //label1text = t.TestName;
                   //Server.FileTestsControler.UpdateData(WebEngion.TCPPeer.id, WebEngion.TCPPeer.downlenth, WebEngion.TCPPeer.lenth);
                }
            };
            timer2.Start();


        }

        private void button2_Click(object sender, EventArgs e)
        {
            //"192.168.2.103"
            string text = textBox1.Text;


            server = new FileServer();
            server.StartServer(text, 21555);
        }








    }



    public class FileServer : NetworkManager
    {
        //数据库工具
        FileTestsControler databaseTool;
        //保存所有的客户端连接
        List<Socket> peerList;
        //清楚无效的socket列表
        List<Socket> peerList_ToRemove = new List<Socket>();
        //服务器
        TCPPeer server;

        public FileServer()
        {
            //创建一个列表保存每个客户端的Socket
            peerList = new List<Socket>();
            databaseTool = new FileTestsControler();
        }

        //启动服务器
        public void StartServer(string ip, int port)
        {
            //注册事件，此处只有一个消息
            AddHandler("FileSaveCall", OnCall);
            AddHandler("FileSave", OnSave);           
            server = new TCPPeer(this);
            server.Listen(ip, port);
            //启动另一个线程处理消息队列
            this.StartThreadUpdate();
            Debug.Print("启动服务器");                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                       
            Form1.label2text += "启动服务器\n";
        }

        //处理服务器接受客户端的连接
        public override void OnAccepted(NetPacket packet)
        {
            Debug.Print("接受新的连接");
            peerList.Add(packet.socket);

            Form1.label2text += "接受新的连接\n";
        }
        //处理丢失连接
        public override void OnLost(NetPacket packet)
        {
            Debug.Print("丢失连接");
            peerList.Remove(packet.socket);
        }
        //处理上传逻辑
        public void OnSave(NetPacket packet)
        {
            Form1.label2text += "成功下载到用户的上传资源。。\n";

            Form1.label2text += "文件本地装载中。。\n";

            Link.Chat.FileProto proto = packet.ReadObject<Link.Chat.FileProto>();
            if (proto != null)
                Debug.Print(proto.filename + "  ->  " + proto.username);

            string filename = proto.filename;
            byte[] bts = proto.filedata;
            Stream stream = BytesToStream(bts);
            StreamToFile(stream, filename);

            Form1.label2text += "装载成功，提示用户上传已成功。。\n";

            foreach (Socket sk in peerList)
            {
                if (!sk.Connected)
                    peerList_ToRemove.Add(sk);
            }

            //删除无效连接
            foreach (Socket sk in peerList_ToRemove)
            {
                peerList.Remove(sk);
            }
            peerList_ToRemove.Clear();

            /////////---------------发送返回信息----------------------/////////////
            NetPacket npt = new NetPacket();
            npt.packageId = 0;
            npt.BeginWrite("FileSaveResult");
            Link.Chat.CallResult rs = new Link.Chat.CallResult();
            rs.callID = 0;
            rs.needChangeName = 0;
            rs.TestName = proto.filename;
            npt.WriteObject<Link.Chat.CallResult>(rs);
            npt.EncodeHeader();
            if (packet.socket.Connected)
            {
                server.Send(packet.socket, npt);
            }
            /////////---------------发送返回信息----------------------/////////////

        }

        //回应下载任务请求
        public void OnCall(NetPacket packet)
        {
            Link.Chat.UserCall proto = packet.ReadObject<Link.Chat.UserCall>();
            //此处可以验证用户信息
            //-------------------

            Form1.label2text += "接受到来自" + proto.username + "的上传请求信息\n";

            //Server.FileTestsControler.ConnectDatabase();
            //添加一行数据，并且获得id
            //int id = Server.FileTestsControler.AddNewTest(proto.filename, proto.filelenth);
            int id = 1;
            Form1.label2text += "请求成功,数据库追加行，返回任务id："+id.ToString()+"\n";

            NetPacket npt = new NetPacket();
            npt.packageId = 0;
            npt.BeginWrite("FileCallResult");
            Link.Chat.CallResult rs= new Link.Chat.CallResult();
            rs.callID = id;
            rs.needChangeName = 0;
            rs.TestName = proto.filename;
            npt.WriteObject<Link.Chat.CallResult>(rs);
            npt.EncodeHeader();

            Form1.label2text += "反馈给请求用户下载信息\n";
            if (packet.socket.Connected)
            {
                server.Send(packet.socket, npt);
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
