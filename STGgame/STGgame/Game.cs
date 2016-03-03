using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

//Bitmap有一个MakeTransparent方法，可以使某些颜色成为透明，不妨试试这个方法
namespace stg
{
    public enum BossName
    {
        black,
        black_Syren
    }
    public enum Gamestate   //游戏状态的枚举值
    {
        Title,
        Gaming,
        Stop,
        Lose,
        Over
    }
    public enum Gamedifficulty
    {
        Easy,
        Normal,
        Hard,
        Lunatic
    }
    public partial class Game : Form
    {
        const int WM_NCHITTEST = 0x0084;
        const int HT_LEFT = 10;
        const int HT_RIGHT = 11;
        const int HT_TOP = 12;
        const int HT_TOPLEFT = 13;
        const int HT_TOPRIGHT = 14;
        const int HT_BOTTOM = 15;
        const int HT_BOTTOMLEFT = 16;
        const int HT_BOTTOMRIGHT = 17;
        const int HT_CAPTION = 2;

        public Game()
        {
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            initialize();
            images.init();
            init();                              //游戏初始化，调试用
            this.DoubleBuffered = true;
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;           //禁用窗口最大化
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.Move += (s, e) => { Invalidate(); };
            this.Left = 0; this.Top = 0;
        }

        const int UP = 0, DOWN = 1, LEFT = 2, RIGHT = 3, Z = 4;    //用于判断按下的键位
        public static Player player;              //创建玩家
        public static List<Enemy> Enemys;                     //保存游戏中的所有对象
        public static List<Enemy> Enemy_toadd;                       //将要添加的对象
        static List<Enemy> Enemy_toremove;                    //将移除的对象

        public static List<Bullet> Bulltes;              // 保存游戏中敌人所有的子弹
        public static List<Bullet> Bulltes_toAdd;        // 增加列表
        public static List<Bullet> Bulltes_toRemove;     // 移除列表

        public static int Game_Width = 400;       //游戏区域的宽度
        public static int Game_Height = 480;      //游戏区域的高度
        public static int Game_X = 10;            //游戏坐标
        public static int Game_Y = 10;
        public static RectangleF Game_rect;      //游戏区域

        public bool Form_Gaming = true;            //表示是否正常运行（鼠标拖动窗体时逻辑更新会暂停）

        private float time_calculate = 0;           //记录游戏进行的时间

        static Stage game_stage;

        private static Gamedifficulty difficulty = Gamedifficulty.Hard;    //游戏难度，默认为Hard
        public static Gamedifficulty Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }


        bool[] keyState = new bool[10];           //所有按键状态，true表示按下
        public Panel drawpanel = new Panel();               //  界面的绘制面板
        void initialize()                         //窗口初始化
        {
            this.ClientSize = new Size(Game_Width + 200, Game_Height + 20);
        }

        void init()                               //玩家初始化
        {
            Game_rect = new RectangleF(Game_X, Game_Y, Game_Width, Game_Height);
            Enemys = new List<Enemy>();         //所有实体对象的集合 
            Enemy_toadd = new List<Enemy>();
            Enemy_toremove = new List<Enemy>();
            Bulltes = new List<Bullet>();
            Bulltes_toAdd = new List<Bullet>();
            Bulltes_toRemove = new List<Bullet>();
            player = new stg.player.Reimu();//创建Linm角色

            game_stage = new stages.Stage1();  //Stage 1
        }

        long lastTick;                              //用于计时
        float dt;                                   //用于更新游戏逻辑
        long tick;                                  //记录当前ticks
        float lastdt;
        long fps;
        protected override void WndProc(ref Message m)    //这是个好东西，用于处理底层信号
        {
            if (m.Msg == 0xa1 && (int)m.WParam == 0x2)
            {
                Invalidate();
            }
            base.WndProc(ref m);
        }
        protected override void OnPaint(PaintEventArgs e)
        {

            tick = DateTime.Now.Ticks;
            if (lastTick != 0)
            {
                dt = (tick - lastTick) / 10000000.0f;
                lastdt = dt;
            }
            lastTick = tick;
            if (lastdt != 0)
                fps = (long)(1 / lastdt);

            //逻辑更新
            if (Form_Gaming)
                Update(dt);                             //根据间隔时间 dt 来更新游戏逻辑


            //动画更新
            Draw(e.Graphics);
            Application.DoEvents();                 //由于这里是死循环，故使用doevent来解除循环

            if (fps > 100)
                Thread.Sleep(5);
            Invalidate(false);
            //this.OnPaint(e);
        }

        protected override void OnKeyDown(KeyEventArgs e) //记录已按下的键
        {
            base.OnKeyDown(e);               //SHIFT,UP,DOWN等分别设为了常量，便于理解
            switch (e.KeyCode)
            {
                case Keys.Left:
                    keyState[LEFT] = true;
                    break;
                case Keys.Right:
                    keyState[RIGHT] = true;
                    break;
                case Keys.Up:
                    keyState[UP] = true;
                    break;
                case Keys.Down:
                    keyState[DOWN] = true;
                    break;
                case Keys.Z:
                    keyState[Z] = true;
                    break;
            }
        }
        protected override void OnKeyUp(KeyEventArgs e)   //记录已经松开的键
        {
            base.OnKeyUp(e);
            switch (e.KeyCode)
            {
                case Keys.Left:
                    keyState[LEFT] = false;
                    break;
                case Keys.Right:
                    keyState[RIGHT] = false;
                    break;
                case Keys.Up:
                    keyState[UP] = false;
                    break;
                case Keys.Down:
                    keyState[DOWN] = false;
                    break;
                case Keys.Z:
                    keyState[Z] = false;
                    break;
            }
        }
        protected bool IsKeydown(Keys key)      //判断键是否按下
        {
            switch (key)                        //根据数组中的
            {
                case Keys.Up:
                    return keyState[UP];
                case Keys.Down:
                    return keyState[DOWN];
                case Keys.Left:
                    return keyState[LEFT];
                case Keys.Right:
                    return keyState[RIGHT];
                case Keys.Z:
                    return keyState[Z];
                default:
                    return false;
            }
        }
        void keyinit()                      //键位复位
        {
            for (int i = 0; i < 10; i++)
                keyState[i] = false;
        }
        private void AddEntity(Enemy e)    //添加敌人
        {
            Enemys.Add(e);
        }

        public void Update(float t)             //更新游戏逻辑
        {
            game_stage.Update(t);       // 关卡逻辑更新
            time_calculate += t;
            this.Text = "刷新率:" + fps.ToString() + " 碰撞次数：" + player.Hited_count.ToString() + "   已用时间 : " + ((int)time_calculate).ToString();
            Vector v = new Vector(0, 0);          //创建Vector对象用于更新逻辑
            //移动
            if (IsKeydown(Keys.Up))
                v.Y--;
            if (IsKeydown(Keys.Down))
                v.Y++;
            if (IsKeydown(Keys.Left))
                v.X--;
            if (IsKeydown(Keys.Right))
                v.X++;
            v.Normalize();
            player.direction.X = v.X;                 //玩家的方向确定
            player.direction.Y = v.Y;
            //由于shift键比较特殊，需要用Control.ModifierKeys特别判断
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                player.FlySlow = true;
            else
                player.FlySlow = false;

            #region 玩家逻辑
            if (IsKeydown(Keys.Z))                //玩家发射子弹
            {
                player.Fire();
            }
            player.Update(t);                    //玩家的逻辑更新
            #endregion
            #region 敌机逻辑
            foreach (Enemy e in Enemy_toadd)           //将待定添加的内容添入进去
            {
                Enemys.Add(e);
            }
            Enemy_toadd.Clear();
            foreach (Enemy e in Enemys)          //对每一个实体进行逻辑更新
            {
                e.Update(t);
            }

            foreach (Enemy e in Enemys)          //将逃出游戏区域的敌人移除()
            {
                if (e.disappeared == true)       //当敌机状态为消失后，则整个移除
                {
                    Enemy_toremove.Add(e);
                }
            }

            foreach (Enemy e in Enemy_toremove)        //移除需要移除的对象
            {
                Enemys.Remove(e);
            }
            Enemy_toremove.Clear();
            foreach (Enemy e in Enemy_toadd)        //添加需要添加的对象
            {
                Enemys.Add(e);
            }
            Enemy_toadd.Clear();
            #endregion
            #region  敌机子弹逻辑
            foreach (Bullet b in Bulltes)
            {
                if (b.disappeared == true)
                    Bulltes_toRemove.Add(b);
            }
            foreach (Bullet b in Bulltes_toRemove)
            {
                Bulltes.Remove(b);
            }
            Bulltes_toRemove.Clear();
            foreach (Bullet b in Bulltes_toAdd)
            {
                Bulltes.Add(b);
            }
            Bulltes_toAdd.Clear();
            foreach (Bullet b in Bulltes)
            {
                b.Update(t);
            }
            #endregion
            #region 中弹逻辑
            if (Game.player.unmatchedtime <= 0)     //如果在无敌时间以外，则对每一个子弹进行碰撞判定
            {
                foreach (Bullet a in Bulltes)
                {
                    if (a.Collision(Game.player))   //如果发生了碰撞，则把player的Hited标记为true
                        Game.player.Hited = true;
                }
            }
            #endregion
            #region 动画更新
            Animation.Animation.Check();
            Animation.Animation._Update(new UpdateEventArgs(t));
            #endregion
        }                                      
        public void Draw(Graphics g)              //对每一个实体对象都进行绘制
        {
            //g.FillRectangle(mapbrush, this.ClientRectangle);
            game_stage.Draw(g);           //关卡更新
            foreach (Animation.Animation a in Animation.Animation.Back_Animations)
                a.Draw(g);
            //绘制每一个对象
            Application.DoEvents();                 //由于这里是死循环，故使用doevent来解除循环

            player.Draw(g);                   // 对玩家（和玩家的子弹）进行绘制
            foreach (Enemy r in Enemys)       // 对所有敌人进行绘制
            {
                r.Draw(g);
            }
            foreach (Bullet b in Bulltes)     // 对敌人的子弹进行绘制
            {
                b.Draw(g);
            }
            foreach (Animation.Animation a in Animation.Animation.Fore_Animations)
                a.Draw(g);
            g.FillRectangle(Brushes.Black, 0, -5, Game_X + Game.Game_Width, Game.Game_Y + 6);
            g.FillRectangle(Brushes.Black, 0, Game.Game_Y, Game.Game_X, Game.Game_Height);
            g.FillRectangle(Brushes.Black, 0, Game.Game_Y + Game.Game_Height - 1, Game.Game_X + Game_Width, Game_Y + 5);
            g.FillRectangle(Brushes.Black, Game_X + Game_Width - 5, 0, 200, this.Height + 10);
        }

    }
}




