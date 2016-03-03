using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.player
{
    public class Reimu : Player
    {

        public RectangleF myrect = new RectangleF(0, 0, 32, 48);
        private Items.YinYangyu_L YinYang_L;
        private Items.YinYangyu_R YinYang_R;
        public Reimu()
        {
            this.coordinate.X = Game.Game_X + Game.Game_Width / 2;
            this.coordinate.Y = Game.Game_Y + Game.Game_Height - 70;
            this.radius = 1;                         //初始化半径
            this.speed = 260;
            this.Level = 4;
            this.radius = 2;
            this.my_texturebrush = new TextureBrush(images.Lin_forward);
            this.direction = new Vector(0, 0);       //方向初值为0
            Player_Bullets = new List<Bullet>();      //创建子弹集合
            Bullet_toremove = new List<Bullet>();    //创建将要移除的子弹的集合

            YinYang_L = new Items.YinYangyu_L(this); //添加阴阳玉
            YinYang_R = new Items.YinYangyu_R(this);

            new Animation.Reimu_effect(this).Fore_Run();  //给自己添加动画
        }

        public override int Level
        {
            get { return level; }
            set
            {
                level = value;
                switch (value)         //根据不同等级，制定不同的自机属性,cd以及攻击力等
                {
                    case 1:
                        fire_cd = 0.1f;
                        break;
                    case 2:
                        fire_cd = 0.1f;
                        break;
                    case 3:
                        fire_cd = 0.1f;
                        break;
                    case 4:
                        fire_cd = 0.1f;
                        break;
                }
            }
        }
        public override bool FlySlow            //慢速飞行属性
        {
            get { return flyslow; }
            set
            {
                flyslow = value;
                if (value == true)
                {
                    speed = 150;
                }
                else
                {
                    speed = 260;
                }
            }
        }


        public override void Draw(Graphics g)        //根据不同的角色制定不同的绘画
        {
            foreach (Bullet a in Player_Bullets)     //对每一个子弹都进行绘制
            {
                a.Draw(g);
            }
            g.DrawImage(images.Reim, new RectangleF(coordinate.X - 16, coordinate.Y - 21, 32, 48),
                myrect, GraphicsUnit.Pixel);
            YinYang_L.Draw(g);
            YinYang_R.Draw(g);
            //g.FillRectangle(Brushes.Yellow, coordinate.X - radius, coordinate.Y - radius, 2 * radius, 2 * radius);
        }

        int x = 0;   //向前方向的人物动画
        int x1 = 0;   //向左方向的人物动画
        int x2 = 0;  //向右方向的人物动画
        float Time_caculate;
        float lasttick1;
        public override void Update(float t)         //根据不同的角色制定不同的游戏逻辑
        {
            Time_caculate += t;
            animation();
            YinYang_L.Update(t);
            YinYang_R.Update(t);
            if (allowfire == false)                 //如果不允许射击（cd期间）,则cd_calculat开始计时
            {
                cd_calculate += t;
                if (cd_calculate >= fire_cd)        //如果计时超过了cd，则allowfire打开，允许射击
                {
                    allowfire = true;
                    cd_calculate = 0f;              //cd的计时复位为0
                }
            }
            float m_x = coordinate.X, m_y = coordinate.Y;    //利用m_x,m_y来模拟逻辑
            //把向量规范成单位长度：
            direction.Normalize();

            //实现相应的移动
            m_x += speed * direction.X * t;
            m_y += speed * direction.Y * t;
            if ((m_x - radius) >= Game.Game_X + 5 && (m_x + radius) <= Game.Game_X + Game.Game_Width - 5)  //符合条件则把值返回给真实值
                coordinate.X = m_x;
            if ((m_y - radius) >= Game.Game_Y + 5 && (m_y + radius) <= Game.Game_Y + Game.Game_Height - 7)
                coordinate.Y = m_y;

            if (unmatchedtime > 0)
                unmatchedtime -= t;
            if (Hited == true)
            {
                Hited = false;
                unmatchedtime = 3;          //重置无敌时间 , 默认无敌时间都是3s
                Hited_count++;              //被击中次数加1
                float len;
                foreach (Bullet b in Game.Bulltes)
                {
                    len = (b.coordinate.X - coordinate.X) * (b.coordinate.X - coordinate.X)
                        + (b.coordinate.Y - coordinate.Y) * (b.coordinate.Y - coordinate.Y);
                    if (len < 40000)               //r = 200
                        b.disappeared = true;      //在其半径范围内的全部子弹消失
                }
            }
            //对每一个子弹都进行逻辑更新
            foreach (Bullet a in Player_Bullets)
            {
                foreach (Enemy em in Game.Enemys)
                {
                    if (em.HaveServitors == true)   //如果有使魔则对它的每一个使魔都进行判定
                    {
                        foreach (Enemy em2 in em.My_Servitors)  //迭代每一个使魔
                        {
                            if (a.Collision(em2))
                            {
                                em2.Hited = true;   //(击中后子弹会自动消失，这里不必深入思考原理，想了解就看bullet类的collision方法)
                            }
                        }
                    }
                    if (a.Collision(em))              // 对每一个敌人进行子弹的判定
                    {
                        em.Hited = true;   //(击中后子弹会自动消失，这里不必深入思考原理，想了解就看bullet类的collision方法)
                    }
                }
            }
            foreach (Bullet a in Player_Bullets)
            {
                a.Update(t);                     //更新逻辑
                //除去屏幕以外或已经消失了的弹幕
                if (a.isoutofForm() || a.disappeared == true)
                    Bullet_toremove.Add(a);
            }
            foreach (Bullet a in Bullet_toremove)//将需要移除的移除
            {
                Player_Bullets.Remove(a);
            }
            Bullet_toremove.Clear();             //清空释放列表
        }
        private void animation()
        {
            //人物动画
            if (direction.X == 0)
            {
                if (Time_caculate - lasttick1 > 0.1f)
                {
                    if (x == 8)
                        x = 0;
                    else
                        x++;
                    if (x < 8)
                    {
                        myrect.X = x * 32;
                        myrect.Y = 0;
                    }
                    else
                    {
                        myrect.X = 0;
                        myrect.Y = 48;
                    }
                    lasttick1 = Time_caculate;
                }
            }
            else if (direction.X < 0)
            {
                if (flyslow)
                {
                    if (Time_caculate - lasttick1 > 0.1f)
                    {
                        if (x1 == 1)
                            x1 = 0;
                        else
                            x1++;
                        myrect.X = (x1 + 1) * 32;
                        myrect.Y = 48;
                        lasttick1 = Time_caculate;
                    }
                }
                else
                {
                    if (Time_caculate - lasttick1 > 0.1f)
                    {
                        if (x2 == 4)
                            x2 = 0;
                        else
                            x2++;
                        myrect.X = (x2 + 3) * 32;
                        myrect.Y = 48;
                        lasttick1 = Time_caculate;
                    }
                }
            }
            else if (direction.X > 0)
            {
                if (flyslow)
                {
                    if (Time_caculate - lasttick1 > 0.1f)
                    {
                        if (x1 == 1)
                            x1 = 0;
                        else
                            x1++;
                        myrect.X = (x1 + 1) * 32;
                        myrect.Y = 96;
                        lasttick1 = Time_caculate;
                    }
                }
                else
                {
                    if (Time_caculate - lasttick1 > 0.1f)
                    {
                        if (x2 == 4)
                            x2 = 0;
                        else
                            x2++;
                        myrect.X = (x2 + 3) * 32;
                        myrect.Y = 96;
                        lasttick1 = Time_caculate;
                    }
                }
            }
        }

        //开火
        public override void Fire()              //根据不同等级，创造不同的子弹与子弹数量
        {
            if (allowfire == false)     //如果还不允许发射，就结束发射命令
                return;
            switch (Level)
            {
                case 1:                 //等级1时的子弹
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y - 12,20));
                    break;
                case 2:
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X - 5, coordinate.Y ,15));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X + 5, coordinate.Y ,15));
                    break;
                case 3:
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y ,10,900, new Vector(-0.15f, -1).GetNormalize()));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y ));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y ,10,900, new Vector(0.15f, -1).GetNormalize()));
                    break;
                case 4:
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y, 8, 900, new Vector(-0.3f, -1).GetNormalize()));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y, 10, 900, new Vector(-0.1f, -1).GetNormalize()));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y ,20));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y, 10, 900, new Vector(0.1f, -1).GetNormalize()));
                    Player_Bullets.Add(new stg.bullet.Reimu_Bullet_small(coordinate.X, coordinate.Y, 8, 900, new Vector(0.3f, -1).GetNormalize()));
                    YinYang_R.Fire();
                    YinYang_L.Fire();
                    break;
            }
            allowfire = false;          //发射后进入冷却阶段，在fire_cd内不允许再发射
        }
    }
}
