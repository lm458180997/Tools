using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace 获取IP地址
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Net.IPHostEntry myEntry = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName());
            string ipAddress = myEntry.AddressList[0].ToString();
            this.Text = ipAddress;

            label1.Text = "";

            //查询到内网的上ip地址
            GetIP gip = new GetIP();
            ipAddress = gip.GetLocalIPv4().ToString();
            this.Text = ipAddress;
            textBox1.Text = ipAddress;

            label1.Text = "内网Ip是：" + ipAddress;

            ipAddress = GetIP();
            label1.Text += ("\n\n外网ip是：" + ipAddress);
            textBox2.Text = ipAddress;
            

        }

        //查询到外网上的ip地址
        private static string GetIP()
        {
            string tempip = "";
            try
            {
                //需要透过“镜子”才能看到自己的外网ip
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据
                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            return tempip;
        } 
    }

    public class GetIP   
    {     
        private IPAddress IPv4 , IPv6;  
        public GetIP()//构造函数。    搜索  
        {          
            getAllIP();     
        }       
        private void getAllIP()   
        {          
            IPAddress [] ipList= Dns.GetHostAddresses(Dns.GetHostName());      
            foreach (IPAddress ip in ipList)      
            {               
                //获得IPv4                
                if (ip.AddressFamily == AddressFamily.InterNetwork) 
                    IPv4 = ip;                
                //获得IPv6               
                if (ip.AddressFamily == AddressFamily.InterNetworkV6)                  
                    IPv6 = ip;           
            }       
        }       
        public IPAddress GetLocalIPv4()//通过这个public函数获取ipv4        
        {       
            try         
            {               
                if (IPv4 != null)                    
                    return IPv4;                
                else                    
                    return null;           
            }           
            catch (Exception error)         
            {               
                MessageBox.Show(" GetLocalIpv4 Error: " + error.Message);  
                return null;          
            }     
        }       
        public IPAddress GetLocalIPv6()//通过这个public函数获取ipv6       
        {         
            try       
            {             
                if (IPv6 != null)          
                    return IPv6;              
                else                 
                    return null;         
            }           
            catch (Exception error)       
            {               
                MessageBox.Show(" GetLocalIpv6 Error: " + error.Message);  
                return null;           
            }      
        }   
    }





}
