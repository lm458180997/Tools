using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            this.DoubleBuffered = true;
            arr = new int[num, num];
            arr_2 = new int[(num - 2) * (num - 2)];
            computer = new Computer(arr,player.player2);
            EventHandler h = (s, e) => { initialize(); this.Invalidate(); };
            button.Click += h;
          //  button.Click += new System.EventHandler(this.button_Click);
          //  private void button1_Click(object sender, EventArgs e){}
            InitializeComponent();
        }
        private Button button= new Button();
        private int num = 16;
        static int[,] arr;     //0表示没有棋子，1表示有黑棋，2表示有白棋
        public static int[] arr_2;    //用一维数组表示
        private int mwidth = 26;
        private bool vscomputer = true;
        static Computer computer ;
    
        private player condition ; 

        public enum player
        { player1=1, player2=2}

        private void Form1_Load(object sender, EventArgs e)
        {
            this.ClientSize = new Size(mwidth * num + 100, mwidth * num);
            condition = player.player1;

            this.Controls.Add(button);

            button.Text = "刷新";
            button.Location = new Point((num-2) * mwidth + 30, 100);
            button.Size = new Size(50, 20);
            button.Show();
        }
        public  void initialize()
        {
            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < num; j++)
                {
                    if (i == 0 || i == 15 || j == 0 || j == 15)
                    {
                        arr[i, j] = 3;
                    }
                    else
                        arr[i, j] = 0;
                }
            }
            for (int i = 0; i < 196; i++)
                arr_2[i] = 0;
            condition = player.player1;
        }

        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            SolidBrush tb = new SolidBrush(Color.LightBlue);
            SolidBrush tb1 = new SolidBrush(Color.Black);
            SolidBrush tb2 = new SolidBrush(Color.White);
            Pen pen = new Pen(Color.Black, 1);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            for (int i = 1; i <= 14; i++)
            {
                for (int j = 1; j <=14; j++)
                {
                    g.DrawLine(pen, new Point(i * mwidth + mwidth / 2, 1 * mwidth + mwidth / 2), new Point(i * mwidth + mwidth / 2, (num-2) * mwidth + mwidth / 2));
                    g.DrawLine(pen, new Point(1 * mwidth + mwidth / 2, j * mwidth + mwidth / 2), new Point((num-2) * mwidth + mwidth / 2, j * mwidth + mwidth / 2));
                }
            }
            for (int i = 1; i <= 14; i++) //黑棋
            {
                for (int j = 1; j <= 14; j++)
                {
                    if (arr[i,j]==1)
                        g.FillEllipse(tb1, i * mwidth + mwidth * 0.1f, j * mwidth + mwidth * 0.1f, mwidth - mwidth * 0.2f, mwidth - mwidth * 0.2f);
                }
            }
            for (int i = 1; i <= 14; i++) //白棋
            {
                for (int j = 1; j <= 14; j++)
                {
                    if (arr[i,j]==-1)
                        g.FillEllipse(tb2, i * mwidth + mwidth * 0.1f, j * mwidth + mwidth * 0.1f, mwidth - mwidth * 0.2f, mwidth - mwidth * 0.2f);
                }
            }
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            this.Text = condition.ToString();
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (e.X > mwidth && e.X < mwidth * 15 && e.Y > mwidth && e.Y < mwidth * 15)
                {
                    int x = e.X / mwidth;
                    int y = e.Y / mwidth;
                    switch (condition)
                    {
                        case player.player1:
                            if (arr[x, y] == 0)
                            {
                                arr[x, y] = 1;
                                arr_2[(y - 1) * 14 + x-1] = 1;
                                Invalidate();
                                if (judgewin(x, y, selcetplayer) == true)
                                {
                                    MessageBox.Show("黑棋胜利");
                                }
                                condition = player.player2;
                                if (vscomputer == true)     //模拟电脑下棋
                                {
                                    int gg = computer.getx();
                                    int tt = computer.gety();
                                    float ss = computer.myscorearray[gg, tt];
                                    this.Text = gg.ToString() +" "+ tt.ToString()+" "+ss.ToString();
                                    arr[gg, tt] = -1;
                                    arr_2[(tt - 1) * 14 + gg-1] = -1;
                                    Invalidate();
                                    condition = player.player1;
                                }
                            }
                            break;
                        case player.player2:
                            if (condition == player.player2)
                            {
                                if (arr[x, y] == 0)
                                {
                                    arr[x, y] = 2;
                                    Invalidate();
                                    if (judgewin(x, y, selcetplayer) == true)
                                    {
                                        MessageBox.Show("白棋胜利");
                                    }
                                    condition = player.player1;
                                }
                            }
                            break;
                    }
                }
            }
        }
        public delegate int select(player p);
        public static event select selcetplayer = (p) =>           //实现了委托的方法
        {
            if (p == player.player1)
            { return 1; }
            else
            { return -1; }
        };
        public Boolean judgewin(int x, int y, select selected)   //需要导入一个实现该委托的方法
        {
            int number = 0;
            for (int i = Math.Max(1, x - 4); i <= Math.Min(x + 4, 14); i++)
            {
                if (arr[i, y] == selected(condition))
                {
                    number++;
                    if (number >= 5)
                        return true;
                }
                else
                    number = 0;
            }
            number = 0;
            for (int i = Math.Max(1, y - 4); i <= Math.Min(y + 4, 14); i++)
            {
                if (arr[x, i] == selected(condition))
                {
                    number++;
                    if (number >= 5)
                        return true;
                }
                else
                    number = 0;
            }
            number = 0;
            for (int i = x-4, j = y - 4; i <= x+4 && j <= y + 4; i++, j++)
            {
                if (j >= 1 && j <= 14&& i>=1 && i<=14)
                {
                    if (arr[i, j] == selected(condition))
                    {
                        number++;
                        if (number >= 5)
                            return true;
                    }
                    else
                        number = 0;
                }
            }
            number = 0;
            for (int i = x-4, j = y + 4; i <= x + 4 && j >= y - 4; i++, j--)
            {
                if (j >= 1 && j <= 14 && i >= 1 && i <= 14)
                {
                    if (arr[i, j] == selected(condition))
                    {
                        number++;
                        if (number >= 5)
                            return true;
                    }
                    else
                        number = 0;
                }
            }
            return false;
        }


    }
}
