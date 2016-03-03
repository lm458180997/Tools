using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace 扫雷
{
    public partial class Form1 : Form
    {
        private Timer timer = new Timer();        // 用于处理鼠标的点击事件的计时器
        private GlassButton[] glassbutton = new GlassButton[10]; 
        public Form1()
        {
            mat = new int[row, column];
            flag = new Boolean[row, column];
//          img = new Bitmap(扫雷.Properties.Resources._1);
//          img = ImageExtensions.BothAlpha(img, false,true);
//          img = ImageExtensions.BothAlpha(img, false, false);
            this.MaximizeBox = false;

            InitializeComponent();
            this.DoubleBuffered = true;


            //引发双点击事件的timer
            timer.Interval = 100;
            timer.Tick += (s, e) => {together = false;timer.Stop();};

            for (int i = 0; i < 10; i++)
            {
                glassbutton[i] = new GlassButton();
                this.Controls.Add(glassbutton[i]);               
            }

            //重置按钮
            glassbutton[0].Text = "重来";
            glassbutton[0].Click += (s, e) => { inivalize(); addth(boooo); gamestart = true; part5(); };
            //开始按钮
            glassbutton[1].Text = "开始";
            glassbutton[1].Click += (s, e) => { part2(); };
            //帮助按钮
            glassbutton[2].Text = "帮助";
            glassbutton[2].Click += (s, e) => { };
            //排行榜按钮
            glassbutton[3].Text = "排行榜";
            glassbutton[3].Click += (s, e) => { };
            //退出按钮
            glassbutton[4].Text = "退出";
            glassbutton[4].Click += (s, e) => { this.Close(); };
            //初级按钮
            glassbutton[5].Text = "初级";
            glassbutton[5].Click += (s, e) =>
            { Row = 10; Column = 10; inivalize(); boooo = 10; addth(boooo); part5(); gamestart = true; };
            //中级按钮
            glassbutton[6].Text = "中级";
            glassbutton[6].Click += (s, e) =>
            { Row = 15; Column = 15; inivalize(); boooo = 30; addth(boooo); part5(); gamestart = true; };
            //高级按钮
            glassbutton[7].Text = "高级";
            glassbutton[7].Click += (s, e) =>
            { Row = 22; Column = 18; inivalize(); boooo = 50; addth(boooo); part5(); gamestart = true; };
            //胜利/失败按钮
            glassbutton[8].Text = "you win！";
            glassbutton[8].Click += (s, e) =>
            { part4(); };
            //返回按钮
            glassbutton[9].Size = new Size(100, 23);
            glassbutton[9].Text = "回到标题画面";
            glassbutton[9].Click += (s, e) =>
            { part1(); inivalize(); addth(0); Invalidate(); };

            //默认值
            Mwidth = 25;
            Row = 22; Column = 15; 
            this.ClientSize = new Size((int)mwidth * (row - 2), (int)mwidth * (column - 2));
            glassbutton[8].Size = this.ClientSize;   

            //tooltip
            toolTipEx1.Active = false;
            SetToolTip(glassbutton[0],""); //给各个控件的标题和内容进行连接

            InitEvents();
            part1();
            /* 还可以有这些委托  (s,e)是匿名委托（事件）
             * （mousedown/mousemove）事件的监听事件
             *  //     public delegate void MouseEventHandler(object s , MouseEventArgs e);*/
         //   MouseEventHandler h = (s, e) => ((Control)s).Text = e.Location.ToString();
         //   this.MouseDown += h;  // h是后面的委托（指针），(s,e)是一个
         //   this.MouseMove += h;
           /* +=也就是触发一次，mousedown是一个事件，这是由事件触发的事件
             */  
        }
        public void part1()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i >= 1 && i <= 4)
                    glassbutton[i].Show();
                else
                    glassbutton[i].Hide();
            }
        }
        public void part2()
        {
             for (int i = 0; i < 10; i++)
            {
                if ((i >= 5 && i <= 7)||i==9)
                    glassbutton[i].Show();
                else
                    glassbutton[i].Hide();
            }
        }
        public void part3()
        {
            gamestart = false;
            for (int i = 0; i < 10; i++)
            {
                if (i==8)
                    glassbutton[i].Show();
                else
                    glassbutton[i].Hide();
            }
        }
        public void part4()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 0||i==9)
                    glassbutton[i].Show();
                else
                    glassbutton[i].Hide();
            }
        }
        public void part5()
        {
            for (int i = 0; i < 10; i++)        
                 glassbutton[i].Hide();
        }
        int row, column;                                 //row代表x，column代表y
        int[,] mat;                                      //0代表空，-1代表有雷，-2代表未确定(不是雷)，其它数字则代表有雷的数量
        Boolean[,] flag ;                                //f代表没有旗子，t代表有
        Boolean together = false;                        //用来触发鼠标双键击事件
        int boooo=30;                                    //地雷的数量
        private float mwidth = 24;                       //设置格子的宽
        public int Row
        {
            get { return row-2;}
            set
            {
                row = value + 2;
                this.ClientSize = new Size((int)mwidth * (row - 2), (int)mwidth * (column - 2)); mat = new int[row, column];
                flag = new Boolean[row, column];
                glassbutton[0].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 90);
                glassbutton[1].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 150);
                glassbutton[2].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 120);
                glassbutton[3].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 90);
                glassbutton[4].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 60);
                glassbutton[5].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 150);
                glassbutton[6].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 120);
                glassbutton[7].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 90);
                glassbutton[8].Location = new Point(0,0);
                glassbutton[9].Location = new Point((row - 2) * (int)mwidth / 2 - 48, ClientSize.Height - 60);
                glassbutton[8].Size = this.ClientSize;   
            }
        }
        public int Column
        { get { return column - 2; }
            set
            {
                column = value + 2;
                this.ClientSize = new Size((int)mwidth * (row - 2), (int)mwidth * (column - 2));
                mat = new int[row, column];
                flag = new Boolean[row, column];
                glassbutton[0].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 90);
                glassbutton[1].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 150);
                glassbutton[2].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 120);
                glassbutton[3].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 90);
                glassbutton[4].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 60);
                glassbutton[5].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 150);
                glassbutton[6].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 120);
                glassbutton[7].Location = new Point((row - 2) * (int)mwidth / 2 - 35, ClientSize.Height - 90);
                glassbutton[8].Location = new Point(0,0);
                glassbutton[9].Location = new Point((row - 2) * (int)mwidth / 2 - 48, ClientSize.Height - 60);
                glassbutton[8].Size = this.ClientSize;   
            }
        }
        public float Mwidth 
        { 
            get { return mwidth; }
            set
            {
                mwidth = value;
                this.ClientSize = new Size((int)mwidth * (row - 2), (int)mwidth * (column - 2));
            }
        }
        public Boolean gamestart = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            inivalize();
            addth(boooo);
        }

        //初始化
        void inivalize()
        {
            for(int i =0;i<column;i++)
           {
               for (int j = 0; j < row; j++)
               {
                   mat[j, i] = 0;
                   flag[j, i] = false;
               }
            }
            Invalidate();
        }  
        //产生了num个雷
        void addth(int num)
       {   
          int a;
          List<int> list=new List<int>();
          for(int i=0;i<num;i++)
          list.Add(-1);
          for(int i=0;i<=Row*Column-num;i++)
          list.Add(-2);
          Random  rd = new Random();
          for(int i=1;i<=Row;i++)
           { for(int j =1;j<=Column;j++)
             {    a=list[rd.Next(list.Count)];
                  list.Remove(a);
                  mat[i,j]=a;
             }
           }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            TextureBrush tb = new TextureBrush(扫雷.Properties.Resources._3);
          //SolidBrush tb = new SolidBrush(Color.FromArgb(200, Color.LightBlue));
         // SolidBrush tb1 = new SolidBrush(Color.FromArgb(200, Color.DeepSkyBlue));
            TextureBrush tb1 = new TextureBrush(扫雷.Properties.Resources._1);
            SolidBrush tb2 = new SolidBrush(Color.FromArgb(255, Color.White));
            SolidBrush tbstring = new SolidBrush(Color.FromArgb(70, 141, 189, 255));
            SolidBrush tbstring2 = new SolidBrush(Color.FromArgb(140, 141, 189, 255));
            GraphicsPath p =new GraphicsPath();
            for (int i = 1; i < row-1; i++)
            {
                for (int j = 1; j < column-1; j++)
                {
                    if ((mat[i, j] == -2 || mat[i, j] == -1)&&flag[i,j]==false) //原式样貌
                    {
                        g.FillRectangle(tb, (i-1) * mwidth , (j-1) * mwidth , mwidth , mwidth);
                    }
                    else if (flag[i, j] != true) //没插旗则画出数字
                    {
                        g.FillRectangle(tb1, (i-1) * mwidth, (j-1) * mwidth, mwidth, mwidth);
                        if (mat[i, j] != 0)
                        {
                            g.FillRectangle(tbstring, (i-1) * mwidth, (j-1) * mwidth , mwidth, mwidth );
                            g.DrawString(mat[i, j].ToString(), new Font("微软雅黑", 10), tb2, (i-1) * mwidth + mwidth / 2, (j-1) * mwidth + mwidth / 2, new StringFormat()
                            {
                                Alignment = StringAlignment.Center,
                                LineAlignment = StringAlignment.Center
                            });
                        /*    p.AddString(mat[i, j].ToString(),new FontFamily("微软雅黑"),(int)FontStyle.Regular,20,new PointF(i * mwidth+mwidth/2, j * mwidth+mwidth/2),new StringFormat()
                            {
                                Alignment=StringAlignment.Center,
                              LineAlignment=StringAlignment.Center
                            });
                            g.DrawPath(new Pen(Color.FromArgb(50,Color.Black),1),p);
                            g.FillPath(tb2,p);
                         */
                        }
                    }
                    if (flag[i, j] == true)
                    {
                        g.FillRectangle(tb1, (i-1) * mwidth, (j-1) * mwidth, mwidth, mwidth);
                        g.FillRectangle(tbstring2, (i-1) * mwidth, (j-1) * mwidth, mwidth, mwidth);
                        g.SmoothingMode = SmoothingMode.AntiAlias;
                        g.DrawImage(扫雷.Properties.Resources.flag, (i-1) * mwidth, (j-1) * mwidth,mwidth,mwidth);
                       // g.DrawString("◇", new Font("宋体", 10), tb2, i * mwidth + 4, j * mwidth + (mwidth - 10) / 2);
                    }
                }
            }
        }

        //连击（同时按键）方法
        public  void togetherdown(MouseEventArgs e) 
        {
            if (e.X > 0 && e.X < mwidth * (row-2) && e.Y > 0 && e.Y < mwidth * (column-2))
            {
                int x = e.X / (int)mwidth+1;
                int y = e.Y / (int)mwidth+1;
                if (mat[x, y] != -1 && mat[x, y] != -2 && flag[x, y] == false)//只有为确认状态且没插旗的方块可进行事件
                {
                    if (bjudge(x, y))
                    {
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            if ((mat[i, y + 1] == -2 || mat[i, y + 1] == -1) && flag[i, y + 1] == false)
                            {
                                if (judge(i, y + 1) == true)   //进行了judge函数，并返回是否有雷，true为有雷
                                {
                                    glassbutton[8].Text = "you lose";
                                    part3();
                                    this.Invalidate();
                                    return;
                                }
                            }
                        }
                        for (int i = x - 1; i <= x + 1; i++)
                        {
                            if ((mat[i, y - 1] == -2 || mat[i, y - 1] == -1) && flag[i, y - 1] == false)
                            {
                                if (judge(i, y - 1) == true)   //进行了judge函数，并返回是否有雷，true为有雷
                                {
                                    glassbutton[8].Text = "you lose";
                                    part3();
                                    this.Invalidate();
                                    return;
                                }
                            }
                        }
                        if ((mat[x - 1, y] == -2 || mat[x - 1, y] == -1) && flag[x - 1, y] == false)
                        {
                            if (judge(x - 1, y))
                            {
                                glassbutton[8].Text = "you lose";
                                part3();
                                this.Invalidate();
                                return;
                            };
                        }
                        if ((mat[x + 1, y] == -2 || mat[x + 1, y] == -1) && flag[x + 1, y] == false)
                        {
                            if (judge(x + 1, y))
                            {
                                glassbutton[8].Text = "you lose";
                                part3();
                                this.Invalidate();
                                return;
                            };
                        }
                        this.Invalidate();
                    }
                }
                if (judgewin())
                {
                    glassbutton[8].Text = "you win";
                    part3(); 
                    this.Invalidate();
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (gamestart == false) return;
            if (e.X > 0 && e.X < mwidth * (row-2) && e.Y > 0 && e.Y < mwidth*(column-2))
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    if (together == true)
                    {
                        togetherdown(e);
                        return;
                    }
                    together = true;
                    timer.Start();
                    if (e.X > 0 && e.X < mwidth * (row-2) && e.Y > 0 && e.Y < mwidth * (column-2))
                    {
                        int x =e.X / (int)mwidth+1;
                        int y = e.Y / (int)mwidth+1;
                        if (flag[x, y] == false && (mat[x, y] == -2 || mat[x, y] == -1))  //有地雷和没地雷时的外貌是一样的
                        {                                                            //仅当没插旗时点击有效
                            if (judge(x, y) == true)
                            {
                                {
                                    glassbutton[8].Text = "you lose";
                                    part3(); ;
                                    this.Invalidate();
                                }
                            }
                            else
                                this.Invalidate();
                        }
                    }
                }
                else if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    if (together == true)
                    {
                        togetherdown(e);
                        return;
                    }
                    together = true;
                    timer.Start();
                    if (e.X > 0 && e.X < mwidth * (row-2) && e.Y > 0 && e.Y < mwidth*(column-2))
                    {
                        int x = e.X / (int)mwidth+1;
                        int y = e.Y / (int)mwidth+1;
                        if (mat[x, y] == -2 || mat[x, y] == -1)  //当是未确定状态时才可以插旗  
                        {
                            if (flag[x, y] == false)
                                flag[x, y] = true;
                            else
                                flag[x, y] = false;
                            this.Invalidate();
                        }
                    }
                }
                if (judgewin())
                {
                    glassbutton[8].Text = "you win";
                    part3();
                    this.Invalidate();
                }
            }
        }
        //判断能否执行消去（左右键同时按）
        Boolean bjudge(int x, int y)
        {
            int unfindnum = 0,flagnum=0; //在这里没使用unfindnum（未确定方格）的记数，可扩展方法
            for (int i = x - 1; i <= x + 1; i++)
            { 
                if (mat[i, y - 1] == -2 || mat[i, y - 1] == -1) { unfindnum++; }
                if (flag[i, y - 1] == true) { flagnum++;}
            }
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (mat[i, y + 1] == -2 || mat[i, y + 1] == -1) { unfindnum++; }
                if (flag[i, y + 1] == true) { flagnum++; }
            }
            if (mat[x - 1, y] == -2 || mat[x - 1, y] == -1) { unfindnum++; }
            if (flag[x - 1, y] == true) { flagnum++; }
            if (mat[x + 1, y] == -2 || mat[x - 1, y] == -1) { unfindnum++; }
            if (flag[x + 1, y] == true) { flagnum++; }
            if (flagnum == getnum(x, y)) { return true; }
            return false;
            /*   下面也是消去，但有缺陷，被利用能够作弊
            for (int i = x - 1; i <= x + 1; i++)
            {
                if ((mat[i, y - 1] != -1&&flag[i,y-1]==true)||(mat[i,y-1]==-1&&flag[i,y-1]==false )) //没有雷，却插了旗子
                { return false; }                                                                   //有雷却没有插旗子
            }
            for (int i = x - 1; i <= x + 1; i++)
            {
                if ((mat[i, y + 1] != -1 &&flag[i,y+1]==true)||(mat[i,y+1]==-1&&flag[i,y+1]==false ))
                { return false; }
            }
            if ((mat[x - 1, y] != -1 &&flag[x-1,y]==true )||( mat[x-1,y]==-1&&flag[x-1,y]==false))
            { return false ; }
            if ((mat[x + 1, y] !=-1&&flag[x+1,y]==true) ||(mat[x+1,y]==-1&&flag[x+1,y]==false))
            { return false ; }
             */
        }
        //获得周围雷的数量
        int getnum(int x, int y)
        {
            int num = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (mat[i, y - 1] == -1)
                    num++;
            }
            for (int i = x - 1; i <= x + 1; i++)
            {
                if (mat[i, y + 1] == -1)
                    num++;
            }
            if (mat[x - 1, y] == -1)
                num++;
            if (mat[x + 1, y] == -1)
                num++;
            return num;
        }
        //判断是否有雷，并消去
        Boolean judge(int x, int y)
        {   
            if (mat[x, y] != -1)
            {
                if (getnum(x, y) == 0)
                {
                    mat[x, y] = 0;
                    delete(x, y);  //如果自己不是雷，而且周围都没雷，则从自己开始递归消方块
                }
                else
                { mat[x, y] = getnum(x, y); }
                // 对四周进行判定，如果有空隙（某个方块的周围没有雷），则递归消除那个空隙所连接的方块
                if (mat[x - 1, y] == -2&&getnum(x-1,y)==0)
                { mat[x - 1, y] = 0; delete(x - 1, y); }
                else if (mat[x + 1, y] == -2&&getnum(x+1,y)==0)
                { mat[x + 1, y]=0;delete(x + 1, y); }
                else if (mat[x, y - 1] == -2&&getnum(x,y-1)==0)
                { mat[x, y - 1] = 0; delete(x, y - 1); }
                else if (mat[x, y + 1] == -2&&getnum(x,y+1)==0) 
                { mat[x, y + 1] = 0; delete(x, y + 1); }
                return false;
            }
            return true;
        }
        //向四周消去方块
        void delete(int x, int y)
        {
            //判断周围的八格，看是否存在getnum（x,y）存在为0的格子，并递归消去
            for (int i = -1; i <= 1; i++)
            {
                if ((mat[(x - 1), y+i] == -2) && flag[(x - 1), y+i]==false) //判断是否为未确定状态并未插旗
                {
                    if (getnum(x - 1, y+i) == 0)
                    {
                        mat[x - 1, y+i] = 0;  //先把可删除的清为0，再递归
                        delete(x - 1, y+i);
                    }
                    else
                    { mat[x - 1, y+i] = getnum(x - 1, y+i); }
                }
            }
            for (int i = -1; i <= 1; i++)
            {
                if ((mat[(x + 1), y+i] == -2) && flag[x+1,y+i]==false)
                {
                    if (getnum(x + 1, y+i) == 0)
                    {
                        mat[x + 1, y+i] = 0;
                        delete(x + 1, y+i);
                    }
                    else
                    { mat[x + 1, y+i] = getnum(x + 1, y+i); }
                }
            }
            if ((mat[x, (y - 1)] == -2) && flag[x,y-1]==false )
            {
                if (getnum(x, (y - 1)) == 0)
                {
                    mat[x, y - 1] = 0;
                    delete(x, (y - 1));             
                }
                else
                { mat[x, (y - 1)] = getnum(x, (y - 1)); }
            }
            if ((mat[x, (y + 1)] == -2) && flag[x,y+1]==false)
            {
                if (getnum(x, (y + 1)) == 0)
                {
                    mat[x, y + 1] = 0;
                    delete(x, (y + 1));                
                }
                else
                { mat[x, (y + 1)] = getnum(x, (y + 1)); }
            }
        }
        //判断是否已消完
        Boolean judgewin()
        {
            for (int i = 1; i < row-1; i++)
            {
                for (int j = 1; j <column-1; j++)
                { if (mat[i, j] == -2) { return false; } } //如果有一个还处于未判定的话则为假
            }
            return true;
        }
        //tooltip的使用
        private void InitEvents()
        {
            glassbutton[0].MouseEnter += delegate(object sender, EventArgs e)
            {
                _toolTip = " 点击后可以重置画面{0}。\r\n" +
            "    ——Statts_2000\r\n" +
            "     2014.03.28";
                SetToolTip(glassbutton[0], "");
                toolTipEx1.Image  = 扫雷.Properties.Resources.a;
                toolTipEx1.Opacity = 0.8D;
                toolTipEx1.ImageSize = new Size(24, 24);
                toolTipEx1.ToolTipTitle = "  重来";      
                toolTipEx1.Active = true;
            };
            glassbutton[0].MouseLeave += delegate(object sender, EventArgs e)
            {
                ResetToolTip();
            };
            glassbutton[1].MouseEnter += delegate(object sender, EventArgs e)
            {
                _toolTip = " 点击后开始游戏{0}。\r\n" +
            "    ——Statts_2000\r\n" +
            "     2014.03.28";
                SetToolTip(glassbutton[1], "");
                toolTipEx1.Image = 扫雷.Properties.Resources.a;
                toolTipEx1.Opacity = 0.8D;
                toolTipEx1.ImageSize = new Size(24, 24);
                toolTipEx1.ToolTipTitle = "开始";
                toolTipEx1.Active = true;
            };
            glassbutton[1].MouseLeave += delegate(object sender, EventArgs e)
            {
                ResetToolTip();
            };
            glassbutton[4].MouseEnter += delegate(object sender, EventArgs e)
            {
                _toolTip = " 退出{0}。\r\n" +
         "    ——Statts_2000\r\n" +
         "     2014.03.28";
                SetToolTip(glassbutton[4], "");
                toolTipEx1.Image = 扫雷.Properties.Resources.a;
                toolTipEx1.Opacity = 0.8D;
                toolTipEx1.ImageSize = new Size(24, 24);
                toolTipEx1.ToolTipTitle = "退出";
                toolTipEx1.Active = true;
            };
            glassbutton[4].MouseLeave += delegate(object sender, EventArgs e)
            {
                ResetToolTip();
            };


        }
        //tooltip的复位
        private void ResetToolTip()  
        {
            toolTipEx1.Active = false;
            toolTipEx1.Opacity = 1D;
            toolTipEx1.ImageSize = new Size(16, 16);
            toolTipEx1.Image = null;
            toolTipEx1.ToolTipTitle = "";
        }
        private string _toolTip = " 点击后可以重置画面{0}。\r\n";
        //设置文本，未知原理
        private void SetToolTip(Control control, string tip)
        {
            toolTipEx1.SetToolTip(
                control,
                string.Format(_toolTip, tip));
        }

    }

}
