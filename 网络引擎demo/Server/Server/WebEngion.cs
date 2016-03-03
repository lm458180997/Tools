using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;


namespace Chat
{
    //用于聊天的协议，它包括一个用户名和聊天类容
    [System.Serializable]
    public class ChatProto
    {
        //用户名
        public string userName;
        //聊天内容
        public string chatMsg;
    }

    [System.Serializable]
    public class FileProto
    {
        public string username;
        public string filename;
        public byte[] filedata;
    }


}
namespace FileIO
{
    [System.Serializable]
    public class FileProto
    {
        public string username;
        public string filename;
        public byte[] filedata;
    }
}
//这种云存储方式由于要开启等量内存的缓冲区，因此不适宜大文件的传输
namespace WebEngion
{

    public class NetworkManager
    {

        //逻辑处理使用一个独立线程，与网络的线程分开
        System.Threading.Thread myThread;                      //单开线程
        //代理回调函数
        public delegate void OnReceive(NetPacket packet);      //回调结构
        //每个消息对应一个OnReceive函数
        public Dictionary<string, OnReceive> handles;          //回调键值对
        //储存网络数据的队列
        private System.Collections.Queue Packets = new System.Collections.Queue();  //数据包

        public NetworkManager()
        {
            handles = new Dictionary<string, OnReceive>();
            //注册连接成功，丢失连接信息
            AddHandler("OnAccepted", OnAccepted);
            AddHandler("OnConnected", OnConnected);
            AddHandler("OnConnectFailed", OnConnectFailed);
            AddHandler("OnLost", OnLost);
        }

        //注册消息
        public void AddHandler(string msgid, OnReceive handler)
        {
            handles.Add(msgid, handler);
        }

        //数据包入队
        public void AddPacket(NetPacket packet)
        {
            lock (Packets)
            {
                Packets.Enqueue(packet);
            }
        }

        //数据包出队
        public NetPacket GetPaket()
        {
            lock (Packets)
            {
                if (Packets.Count == 0)
                {
                    return null;
                }
                return (NetPacket)Packets.Dequeue();
            }
        }

        //开始执行另一个线程处理逻辑
        public void StartThreadUpdate()
        {
            //为逻辑部分建立新的线程
            myThread = new System.Threading.Thread(new System.Threading.ThreadStart(ThreadUpdate));
            runUpdate = true;
            myThread.Start();
        }
        public void StopThreadUpdate()
        {
            runUpdate = false;
        }
        bool runUpdate = true;        //是否执行更新
        //逻辑线程
        public void ThreadUpdate()
        {
            while (runUpdate)
            {
                //为了节约cpu，每循环暂停30桢
                System.Threading.Thread.Sleep(30);
                Update();
            }
        }

        //处理数据包，更新逻辑
        public void Update()
        {
            NetPacket packet = null;
            for (packet = GetPaket(); packet != null; )
            {
                string msg = "";
                //获得信息标识符
                packet.BeginRead(out msg);
                OnReceive handler = null;
                if (handles.TryGetValue(msg, out handler))
                {
                    //根据消息标识符找到相应的OnReceive代理函数
                    if (handler != null)
                        handler(packet);
                }
                packet = null;
            }
        }

        //处理服务器接受客户端的连接
        public virtual void OnAccepted(NetPacket packet)
        {
        }

        //处理客户端取得与服务器的连接
        public virtual void OnConnected(NetPacket packet)
        {
        }

        //处理客户端取得与服务器连接失败
        public virtual void OnConnectFailed(NetPacket packet)
        {
        }

        //处理丢失连接
        public virtual void OnLost(NetPacket packet)
        {
        }


    }

}

namespace WebEngion
{
    public class TCPPeer
    {

        //是否是服务器
        public bool isServer { set; get; }
        //使用的socket
        public Socket socket;
        //网络管理器
        NetworkManager networkMgr;

        public TCPPeer(NetworkManager netMgr)
        {
            networkMgr = netMgr;
        }

        //添加内部消息(用于向NetWorkManager对象添加一些内部消息，不是从网络得到的，例如连接成功，连接失败之类)
        private void AddInternalPacket(string msg, Socket sk)
        {
            //通知丢失连接
            NetPacket p = new NetPacket();
            p.socket = sk;
            p.BeginWrite(msg);
            networkMgr.AddPacket(p);
        }

        //作为服务器，开始监听
        public void Listen(string ip, int port, int backlog = 1000)
        {
            isServer = true;
            //ip地址
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            //创建socket
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                //将socket绑定到地址上
                socket.Bind(ipe);
                //开始监听（最大容量为1k）
                socket.Listen(backlog);
                //开始异步接受连接
                socket.BeginAccept(new System.AsyncCallback(ListenCallback), socket);
            }
            catch (Exception ex)
            {
            }
        }

        //异步接受一个新的连接
        void ListenCallback(System.IAsyncResult ar)
        {
            //取得服务器socket
            Socket listener = (Socket)ar.AsyncState;
            try
            {
                //获得客户端的socket
                Socket client = listener.EndAccept(ar);
                //通知服务器接受一个新的连接（发现请求后，返回OnAccepted协议）
                AddInternalPacket("OnAccepted", client);
                //创建接受数据的数据包
                NetPacket packet = new NetPacket();
                packet.socket = client;
                //开始接受从来自客户端的数据(将字节流传入包的缓冲区中)
                client.BeginReceive(packet.bytes, 0, NetPacket.headerLength,
                    SocketFlags.None, new System.AsyncCallback(ReceiveHeader), packet);

            }
            catch (Exception ex)
            {

            }
            //继续接受其他连接
            listener.BeginAccept(new System.AsyncCallback(ListenCallback), listener);
        }
        //ListenCallBack是异步监听的回调函数，如果连接成功，取得远程客户端的Socket ， 开始异步接受客户端发来的数据

        //作为客户端，开始连接服务器
        public void Connect(string ip, int port)
        {
            isServer = false;
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ip), port);
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //开始连接
                socket.BeginConnect(ipe, new System.AsyncCallback(ConnectionCallback), socket);
            }
            catch (Exception ex)
            {
            }
        }

        //异步连接回调
        void ConnectionCallback(System.IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            try
            {
                //与服务器取得连接
                client.EndConnect(ar);
                //通知已经成功连接到服务器
                AddInternalPacket("OnConnected", client);
                //开始异步接收服务器信息
                NetPacket packet = new NetPacket();
                packet.socket = client;
                client.BeginReceive(packet.bytes, 0, NetPacket.headerLength, SocketFlags.None,
                    new System.AsyncCallback(ReceiveHeader), packet);

            }
            catch (System.Exception e)
            {
                AddInternalPacket("OnConnectFailed", client);
            }
        }
        //无论是创建用于监听的服务器Socket还是用于发起连接的客户端的额Socket，最后都会进入接受数据状态

        //接受消息头
        void ReceiveHeader(System.IAsyncResult ar)
        {
            NetPacket packet = (NetPacket)ar.AsyncState;
            try
            {
                //返回网络上接收的数据长度
                int read = packet.socket.EndReceive(ar);
                //已断开连接
                if (read < 1)
                {
                    //通知丢失连接
                    AddInternalPacket("OnLost", packet.socket);
                    return;
                }
                packet.readLength += read;
                //消息头必须读满8个字节
                if (packet.readLength < NetPacket.headerLength)
                {
                    packet.socket.BeginReceive(packet.bytes,
                        packet.readLength,                //存储偏移已读入的长度
                        NetPacket.headerLength - packet.readLength,         //这次只读入剩余的数据
                        SocketFlags.None, new System.AsyncCallback(ReceiveHeader),
                        packet);
                }
                else
                {
                    //获得消息体长度
                    packet.DecodeHeader();

                    System.Diagnostics.Debug.Print("ID:" + packet.packageId.ToString());
                    System.Diagnostics.Debug.Print("bodyLenth" + packet.bodyLength.ToString());

                    id = packet.packageId;

                    //根据取得的id数据，开辟相应的缓冲空间
                    int space = 512;
                    if (packet.packageId != 0)
                    {
                        space = 512000000;
                    }

                    byte[] bt;
                    //动态重新分配缓冲区
                    bt = new byte[space + NetPacket.headerLength];
                    for (int i = 0; i < NetPacket.headerLength; i++)
                    {
                        bt[i] = packet.bytes[i];
                    }
                    packet.bytes = bt;


                    packet.readLength = 0;
                    //开始读取消息体
                    packet.socket.BeginReceive(packet.bytes,
                        NetPacket.headerLength,
                        packet.bodyLength,
                        SocketFlags.None,
                        new System.AsyncCallback(ReceiveBody),
                        packet);
                }
            }
            catch (Exception e)
            {

            }
        }

        public static float percent=0;
        public static int lenth;
        public static int downlenth;
        public static int id;


        //接收体信息（这里可以获取信息体，获取进度百分比等）
        void ReceiveBody(System.IAsyncResult ar)
        {
            NetPacket packet = (NetPacket)ar.AsyncState;
            try
            {
                //返回网络上接收的数据长度
                int read = packet.socket.EndReceive(ar);
                //已断开连接
                if (read < 1)
                {
                    //通知丢失连接
                    AddInternalPacket("OnLost", packet.socket);
                    return;
                }
                packet.readLength += read;
                //消息体必须读满指定的长度
                if (packet.readLength < packet.bodyLength)
                {
                    lenth = packet.bodyLength; downlenth = packet.readLength;
                    percent = packet.readLength / (float)packet.bodyLength;
                    packet.socket.BeginReceive(packet.bytes,
                        NetPacket.headerLength + packet.readLength,
                        packet.bodyLength - packet.readLength,
                        SocketFlags.None,
                        new System.AsyncCallback(ReceiveBody),
                        packet);
                }
                else
                {
                    lenth = packet.bodyLength; downlenth = packet.readLength;
                    percent = 1;
                    //将消息传入到逻辑处理队列
                    networkMgr.AddPacket(packet);
                    //下一个读取
                    packet.Reset();
                    packet.socket.BeginReceive(packet.bytes,
                        0,
                        NetPacket.headerLength,
                        SocketFlags.None,
                        new System.AsyncCallback(ReceiveHeader),
                        packet);
                }
            }
            catch (Exception ex)
            {
            }
        }

        //向远程发送信息
        public void Send(Socket sk, NetPacket packet)
        {
            NetworkStream ns;
            lock (sk)
            {
                ns = new NetworkStream(sk);
                if (ns.CanWrite)
                {
                    try
                    {
                        ns.BeginWrite(packet.bytes, 0, packet.Length,
                            new System.AsyncCallback(SendCallback), ns);
                    }
                    catch (Exception e)
                    {
                    }
                }
            }
        }

        //发送回调
        private void SendCallback(System.IAsyncResult ar)
        {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            try
            {
                ns.EndWrite(ar);
                ns.Flush();
                ns.Close();
            }
            catch (Exception ex)
            {
            }
        }


    }
}


namespace WebEngion
{

    public class NetPacket
    {
        //int32 占4个字节
        public const int INT32_LEN = 4;
        //头 占8个字节 (他的值实际上就是最后bodylength的长度,以及任务所对应的ID，前4字节为长度，后4字节为id)
        public const int headerLength = 8;
        //身体  最大512个节
        public int max_body_length = 512;    //序列与反序列化时，缓冲区大小应该要一致
        //当前数据体长
        public int bodyLength { get; set; }
        //总长
        public int Length
        {
            get { return headerLength + bodyLength; }
        }
        //byte 数组
        public byte[] bytes { get; set; }
        //发送这个数据包的socket
        public Socket socket;
        //从网络上读取到的数据长度（可能一次读取完也可能分几次读取）
        public int readLength { get; set; }

        //数据包记录ID
        public int packageId = 0;                  //默认为0


        //构造函数
        public NetPacket()
        {
            readLength = 0;
            bodyLength = 0;
            bytes = new byte[headerLength + max_body_length];
        }
        public NetPacket(int lenth)
        {
            max_body_length = lenth;
            readLength = 0;
            bodyLength = 0;
            bytes = new byte[headerLength + max_body_length];
        }
        //重置参数
        public void Reset()
        {
            readLength = 0;
            bodyLength = 0;
        }
        //序列化对象
        public byte[] Seriallize<T>(T t)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    //创建序列化类
                    BinaryFormatter bf = new BinaryFormatter();
                    //序列化到stream中
                    bf.Serialize(stream, t);
                    stream.Seek(0, SeekOrigin.Begin);
                    return stream.ToArray();
                }
                catch (Exception e)
                {
                    return null;
                }
            }
        }
        //反序列化对象
        public T Deserialize<T>(byte[] bs)
        {
            using (MemoryStream stream = new MemoryStream(bs))
            {
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    T t = (T)bf.Deserialize(stream);
                    return t;
                }
                catch (Exception ex)
                {
                    return default(T);
                }
            }
        }
        //////////////////以下为写数据////////////////////

        //开始写入数据
        public void BeginWrite(string msg)
        {
            //初始化体长为0
            bodyLength = 0;
            WriteString(msg);
        }
        //写整形
        public void WriteInt(int number)
        {
            if (bodyLength + INT32_LEN > max_body_length)
                return;
            byte[] bs = System.BitConverter.GetBytes(number);
            bs.CopyTo(bytes, headerLength + bodyLength);
            bodyLength += INT32_LEN;
        }
        //写字符串
        public void WriteString(string str)
        {
            int len = System.Text.Encoding.UTF8.GetByteCount(str);
            this.WriteInt(len);
            if (bodyLength + len > max_body_length)
                return;
            System.Text.Encoding.UTF8.GetBytes(str, 0, str.Length, bytes, headerLength + bodyLength);
            bodyLength += len;
        }
        //写入byte数组
        public void WriteStream(byte[] bs)
        {
            WriteInt(bs.Length);
            if (bodyLength + bs.Length > max_body_length)
            {
                return;
            }
            //压入数据流
            bs.CopyTo(bytes, headerLength + bodyLength);
            bodyLength += bs.Length;
        }
        //直接写入一个序列化的对象
        public void WriteObject<T>(T t)
        {
            byte[] bs = Seriallize<T>(t);
            WriteStream(bs);
        }
        //将数据长度转换为4个字节存到byte数组的最前面
        //public void EncodeHeader()
        //{
        //    byte[] bs = System.BitConverter.GetBytes(bodyLength);
        //    bs.CopyTo(bytes, 0);
        //}
        public void EncodeHeader()
        {
            byte[] bs = System.BitConverter.GetBytes(bodyLength);
            bs.CopyTo(bytes, 0);
            byte[] b = System.BitConverter.GetBytes(packageId);
            b.CopyTo(bytes, 4);
            int i = 0;
        }


        /////////////////下面的函数主要用于读取，反序列化数据，  与写数据是一一对应的///////////////////
        //开始读取
        public void BeginRead(out string msg)
        {
            bodyLength = 0;
            ReadString(out msg);
        }
        //读int 
        public void ReadInt(out int number)
        {
            number = 0;
            if (bodyLength + INT32_LEN > max_body_length)
                return;
            number = System.BitConverter.ToInt32(bytes, headerLength + bodyLength);
            bodyLength += INT32_LEN;
        }
        //读取一个字符串
        public void ReadString(out string str)
        {
            str = "";
            int len = 0;
            ReadInt(out len);
            if (bodyLength + len > max_body_length)
                return;
            str = Encoding.UTF8.GetString(bytes, headerLength + bodyLength, (int)len);
            bodyLength += len;
        }
        //读取byte数组
        public byte[] ReadStream()
        {
            int size = 0;
            ReadInt(out size);
            if (bodyLength + INT32_LEN > max_body_length)
                return null;
            byte[] bs = new byte[size];
            for (int i = 0; i < size; i++)
            {
                bs[i] = bytes[headerLength + bodyLength + i];
            }
            bodyLength += size;
            return bs;
        }
        //直接将读取的byte数组反序列化
        public T ReadObject<T>()
        {
            byte[] bs = ReadStream();
            int i = 0;
            if (bs == null)
            {
                return default(T);
            }
            return Deserialize<T>(bs);
        }
        //由byte数组最前面4个字节得到数据的长度，可以计算后面的数据长度，再继续接受后面的数据
        public void DecodeHeader()
        {
            byte[] bs = new byte[4];
            for (int i = 0; i < 4; i++)
                bs[i] = bytes[i];
            bodyLength = System.BitConverter.ToInt32(bs, 0);
            for (int i = 4; i < 8; i++)
                bs[i - 4] = bytes[i];
            packageId = System.BitConverter.ToInt32(bs, 0);
        }


    }

}



