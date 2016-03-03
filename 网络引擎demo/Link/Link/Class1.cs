using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Link
{
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

        //文件传输包
        [System.Serializable]
        public class FileProto
        {
            public string username;
            public string filename;
            public byte[] filedata;
        }

        //用户提交上传申请报告
        [System.Serializable]
        public class UserCall
        {
            public string username;       //用户名
            public string password;       //用户密码
            public string filename;       //文件名字
            public int filelenth;         //文件长度
        }

        //服务器返回的允许上传报告
        [System.Serializable]
        public class CallResult
        {
            public int callID;            //申请任务所分配的ID号
            public int needChangeName;    //是否需要改名字存储
            public string TestName;       //任务名
        }


    }
}
