using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Form1 : Form
    {
        public Form1()
        {     
            InitializeComponent();
            Images.init();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            start();
        }
        public void start()
        {
            snakes = new List<snake>();
            foods = new List<food>();
            walls = new List<wall>();
            toremoves = new List<snake>();
            toremovef = new List<food>();
            toremovew = new List<wall>();
            Row = 20;
            Col = 20;
            arr = new int[row, col];
            Mwidth = 30;
            dir = direction.right;
            for (int i = 0; i < row; i++)
            {
                walls.Add(new wall(new Point(i, 0)));
                walls.Add(new wall(new Point(i, col - 1)));
            }
            for (int j = 1; j < col - 1; j++)
            {
                walls.Add(new wall(new Point(0, j)));
                walls.Add(new wall(new Point(row - 1, j)));
            }
            Invalidate();
            Score = 0;
            snakes.Add(new snake(new Point(Row / 2, Col / 2), snake.snakeType.Head));
            addfoods();                                      //添加第一个食物                                                                                                                  
            for (int i = 1; i < 4; i++)
            {
                snakes.Add(new snake(new Point(Row / 2 - i, Col / 2)));
            }
            timer = new Timer();
            timer.Start();
            toremovef.Clear();
            toremoves.Clear();
            toremovew.Clear();
            
            timer.Interval =100;
            timer.Tick += (s, e) => 
            {
                move();
                getfoods();
                addwalls();
                Remove();
                dbCode = false;
                Invalidate();
            };
        }
        public void move()
        {
            Point p = snakes[0].location;
            Boolean losed=false;
            switch (dir)
            {
                case direction.up:
                    p.Y--;
                    if (judge(p) == false)
                    {
                        timer.Stop();
                        losed = true;
                    }
                    break;
                case direction.down:
                    p.Y++;
                    if (judge(p) == false)
                    {
                        timer.Stop();
                        losed = true;
                    }
                    break;
                case direction.left: ;
                    p.X--;
                    if (judge(p) == false)
                    {
                        timer.Stop();
                        losed = true;
                    }
                    break;
                case direction.right:
                    p.X++;
                    if (judge(p) == false)
                    {
                        timer.Stop();
                        losed = true;
                    }
                    break;
            }
            if (losed == true)
            {
                start();
                return;
            }
            for (int i = snakes.Count - 1; i >= 1; i--)  // 后一位的坐标往前移动一位
                snakes[i].location = snakes[i - 1].location;
            snakes[0].location = p;
            Invalidate();
        }
        Boolean judge(Point p)               //判断某点是否在身体或墙上
        {
            foreach (wall w in walls)
            {
                if (p == w.location)
                {
                    return false;
                }
            }
            for(int i=0;i<=snakes.Count-1;i++)
                if (p == snakes[i].location)
                {
                    return false;
                }
            return true;
        }        
        public void getfoods()         // 判断是否获得食物
        {
            Boolean geted = false;
            foreach (food a in foods)
            {
                if (a.location == snakes[0].location)
                {
                    if (a.Type == food.foodType.buff)
                    {
                        snakes.Add(new snake(snakes[snakes.Count - 1].location));  //新增的身体放入到最后一位
                        toremovef.Add(a);
                        geted = true;
                        Score += ((snakes.Count - 4) * (snakes.Count - 4));
                    }
                }
            }
            if (geted == true)
            {
                addfoods();
            }
        }
        public void addfoods()          //添加食物
        {
            int x, y;
            Boolean add =true;
            x = rnd.Next(1, Row + 1);
            y = rnd.Next(1, Col + 1);
            foreach (wall w in walls)
                if (w.location == new Point(x, y))
                    add = false;
            foreach (snake s in snakes)
                if (s.location == new Point(x, y))
                    add = false;
            foreach (food f in foods)
                if (f.location == new Point(x, y))
                    add = false;
            if (add == true) { foods.Add(new food(new Point(x, y))); }
            else addfoods();
        }
        public void addfooddebuff() { }     //生成减益效果的食物
        public void addwalls()      //添加墙壁
        {
            int i = rnd.Next(1, 100);
            Boolean added = false;
            if (i > 98)
            {
                while (added == false)
                {
                    Boolean add = true;
                    int x, y;
                    x = rnd.Next(1, Row + 1);
                    y = rnd.Next(1, Col + 1);
                    foreach (wall w in walls)
                        if (w.location == new Point(x, y))
                            add = false;
                    foreach (snake s in snakes)
                        if (s.location == new Point(x, y))
                            add = false;
                    foreach (food f in foods)
                        if (f.location == new Point(x, y))
                            add = false;
                    if (add == true) { walls.Add(new wall(new Point(x, y))); added = true; }
                }
            }
        }
        public void Remove()  //抛出
        {
            foreach (food d in toremovef)
                foods.Remove(d);
            foreach (wall w in toremovew)
                walls.Remove(w);
            foreach (snake s in toremoves)
                snakes.Remove(s);
        }

        private List<snake> snakes=new List<snake>();
        private List<food> foods = new List<food>();
        private List<wall> walls = new List<wall>();
        private List<snake> toremoves = new List<snake>();
        private List<food> toremovef = new List<food>();
        private List<wall> toremovew = new List<wall>();

        public enum direction { up, down, left, right } //运动方向
        public direction dir = direction.right;
        private Timer timer =new Timer();
        private int row,col;                            //row代表横坐标，col代表纵坐标
        private int[,] arr;
        public static int mwidth;                       //格子的单位宽度
        private Random rnd=new Random();
        private Boolean dbCode = false;                 //防止按键同时触发而引发的bug
        public int Row { get { return row-2; } set { row = value+2; } }
        public int Col { get { return col-2; } set { col = value+2; } }
        public int Mwidth
        { 
            get { return mwidth; } 
            set { mwidth = value; this.ClientSize = new Size(mwidth * row, mwidth * col); }
        }
        public int score=0;
        public int Score
        {
            get { return score; }
            set { score = value; this.Text = "Score:" + score.ToString(); }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            foreach (snake s in snakes)
                s.Draw(g);
            foreach (food f in foods)
                f.Draw(g);
            foreach (wall w in walls)
                w.Draw(g);
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if( dbCode==true)       //如果发生同时按键盘，则返回
            return;
            dbCode = true;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if(dir!=direction.down)
                    dir = direction.up;
                    break;
                case Keys.Down:
                    if (dir != direction.up)
                    dir = direction.down;
                    break;
                case Keys.Left:
                    if (dir != direction.right)
                    dir = direction.left;
                    break;
                case Keys.Right:
                    if (dir != direction.left)
                    dir = direction.right;
                    break;
            }

        }
    }
    class snake
    {
        public enum snakeType { Body, Head };
        public Point location;
        protected snakeType type;

        public snakeType Type
        {
            get { return type; }
        }
        public snake(Point location, snakeType type = snakeType.Body)
        {
            this.location = location;
            this.type = type;
        }
        public virtual void Draw(Graphics g)
        {
            switch (type)
            {
                case snakeType.Body:
                 //   g.FillRectangle(Brushes.LightBlue, location.X * Form1.mwidth, location.Y * Form1.mwidth, Form1.mwidth, Form1.mwidth);
                    g.DrawImage(Snake.Properties.Resources.apple, location.X * Form1.mwidth, location.Y * Form1.mwidth, Form1.mwidth, Form1.mwidth);
                 break;
                case snakeType.Head:
                 //   g.FillRectangle(Brushes.Black, location.X * Form1.mwidth, location.Y * Form1.mwidth, Form1.mwidth, Form1.mwidth);
                 g.DrawImage(Snake.Properties.Resources.dingzi, location.X * Form1.mwidth, location.Y * Form1.mwidth, Form1.mwidth, Form1.mwidth);
                    break;
            }
        }
    }
    class item
    {
        public Point location;
        protected  int w = Form1.mwidth;
    }
    class food : item
    {
        public enum foodType{buff,debuff};
        private foodType foodtype;
        public foodType Type
        {
            get { return foodtype; }
        }
        public food(Point point, foodType type=foodType.buff)
        {
            this.location = point;
            this.foodtype = type;
        }
        public void Draw(Graphics g)
        {
            switch (foodtype)
            {
                case foodType.buff:
                    g.DrawImage(Images.Apples, location.X * w, location.Y * w, w, w);
                    break;
                case foodType.debuff:
                    break;
            }
        }
    }
    class wall : item
    {
        public enum wallType { normal, special };
        private wallType walltype;
        public wallType Type
        {
            get { return walltype; }
        }
        public wall(Point point, wallType type=wallType.normal)
        {
            this.location = point;
            this.walltype = type;
        }
        public void Draw(Graphics g)
        {
            switch(walltype)
            {
                case wallType.normal:
                    g.DrawImage(Images.Wall, location.X * w, location.Y * w, w, w);
                    break;
                case wallType.special:
                    break;
            }
        }
    }
    public static class Images
    {
        public static Bitmap Apples,Wall,Body,Head;
        public static void init()
        {
            Apples = Snake.Properties.Resources.apple;
            Wall = Snake.Properties.Resources.wall;
        }
    }
}
