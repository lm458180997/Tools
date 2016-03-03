using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using WebEngion;
using FileIO;
using System.Diagnostics;
namespace Customer
{


    public class FileManager : NetworkManager
    {
        public bool Listening = false;

        public bool Connected = false;

        TCPPeer client;

        public void Start(string ip, int port)
        {
            //连接到服务器
            client = new TCPPeer(this);
            try
            {
                client.Connect(ip, port);
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
            this.StartThreadUpdate();
            Debug.Print("打开信息监听");
            Form1.label2text += "打开监听器\n";
        }

        //发送聊天信息
        public bool Send(NetPacket packet)
        {
            if (client.socket.Connected)
            {
                client.Send(client.socket, packet);
                return true;                    //正确连接上
            }
            return false;
        }

        //处理丢失连接
        public override void OnLost(NetPacket packet)
        {
            Connected = false;
            Debug.Print("丢失与服务器的连接");
        }

        //处理客户端取得与服务器的连接
        public override void OnConnected(NetPacket packet)
        {
            Connected = true;
            Debug.Print("成功连接到服务器");
        }

        //处理客户端与服务器连接失败
        public override void OnConnectFailed(NetPacket packet)
        {
            Connected = false;
            Debug.Print("连接服务器失败，请重新再试");
        }


    }
}
