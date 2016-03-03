using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace stg.enemy
{
    public class Enemy_black_Servitor : Enemy
    {
        private Enemy_black_Servitors _myname;
        public Enemy_black_Servitors myName                 //根据名字对敌人的属性进行初始化
        {
            get { return _myname; }
            set
            {
                _myname = value;
                switch (value)
                {
                    case Enemy_black_Servitors.Enemy_black_Servitor:            //黑色妖精的使魔1
                        this.speed = 18;
                        this.direction = new Vector(0, 0);
                        this.HP = 1000;

                        this.radius = 20;
                        this.disappeared = false;
                        this.my_texturebrush = new TextureBrush(images.enemy_littleblue_1);
                        this.myrect = new RectangleF(256, 192, 32, 32);
                        break;
                    case Enemy_black_Servitors.Enemy_black_Servitor2:           //使魔2号
                        this.speed = 18;
                        this.direction = new Vector(0, 0);
                        this.HP = 1000;

                        this.radius = 20;
                        this.disappeared = false;
                        this.my_texturebrush = new TextureBrush(images.enemy_littleblue_1);
                        this.myrect = new RectangleF(256, 192, 32, 32);
                        break;
                }
            }
        }
        
        public Enemy_black_Servitor(PointF p, Enemy_black_Servitors name, Vector v)    //可以指定一个方向的构造函数
        {
            this.coordinate.X = p.X;
            this.coordinate.Y = p.Y;

            this.myName = name;

            direction.X = v.X;
            direction.Y = v.Y;

            new Animation.Enemy_e(this).Back_Run();    //给它添加一个动画
        }
        public override void Draw(Graphics g)
        {
            //my_texturebrush.TranslateTransform(coordinate.X - 25, coordinate.Y - 25);                   //绘制敌人
            //g.FillRectangle(my_texturebrush, coordinate.X - 25, coordinate.Y - 25, 50, 50);
            //my_texturebrush.TranslateTransform(-(coordinate.X - 25), -(coordinate.Y - 25));
            g.DrawImage(images.enemies1, new RectangleF(coordinate.X - 16, coordinate.Y - 16, 32, 32),
                myrect, GraphicsUnit.Pixel);
        }

        float lastick = 0;           //用于实现子弹的连续发射,部分逻辑更新时会用到多个
        public override void Update(float t)      //逻辑更新
        {
            Time_cacullate += t;
            animation(t);
            switch (_myname)
            {
                //使魔1
                case Enemy_black_Servitors.Enemy_black_Servitor:
                    if (disappeared == false)
                    {
                        foreach (DamageEventArgs re in My_Damages_ToRemove)  //丢弃抛弃列表中的成员
                            My_Damages.Remove(re);
                        My_Damages_ToRemove.Clear();
                        foreach (DamageEventArgs damage in My_Damages)       //对每一个伤害进行处理
                        {
                            this.HP = this.HP - damage.Demage < 0 ? 0 : this.HP - damage.Demage;
                            My_Damages_ToRemove.Add(damage);                 //处理结束后，就将其放入抛弃列表
                        }
                        if (HP == 0)
                        {
                            this.disappeared = true;
                            this.working = false;
                        }

                        if (Time_cacullate > 2f)
                        {
                            direction.rotate((t / 1f) * 30);          //制定为每秒钟转 30度  , 此处为圆圈走法
                            coordinate.X += direction.X * speed * t;
                            coordinate.Y += direction.Y * speed * t;
                        }
                        if (Time_cacullate - lastick > 0.25f)
                        {
                            Vector v = new Vector(0, 0);
                            v = v += direction;            //两个叠加，需详细信息就看vector类中的介绍
                            v.rotate(30);
                            Fire(stg.bullet.Bullet_Name.Arrow_blue2, v);//设置一个不太一样的子弹
                            v.rotate(60);
                            Fire(v);
                            v.rotate(60);
                            Fire(v);
                            lastick = Time_cacullate;
                        }
                    }
                    break;

                //黑色妖精——使魔2

                case Enemy_black_Servitors.Enemy_black_Servitor2:
                    if (disappeared == false)            //如果没消失的话就执行以下逻辑
                    {
                        if (Hited == true)    //被打中一次减少10血
                        {
                            HP -= 10;
                            this.Hited = false;
                        }
                        if (HP < 0)           //当血空后就消失（毁灭）掉
                        {
                            HP = 0;
                            this.disappeared = true;
                        }
                        if (Time_cacullate > 2f)
                        {
                            direction.rotate(-(t / 1f) * 30);          //制定为每秒钟转 30度  , 此处为圆圈走法
                            coordinate.X += direction.X * speed * t;
                            coordinate.Y += direction.Y * speed * t;
                        }
                        if (Time_cacullate - lastick > 0.25f)
                        {
                            Vector v = new Vector(0, 0);
                            v = v += direction;            //两个叠加，需详细信息就看vector类中的介绍
                            v.rotate(-30);
                            Fire(stg.bullet.Bullet_Name.Arrow_blue1, v);
                            v.rotate(-60);             //获取相反的方向
                            Fire(v);
                            v.rotate(-60);
                            Fire(v);
                            lastick = Time_cacullate;
                        }
                    }
                    break;
            }

            //如果机体飞出了界外则判定为消失，  disappeared = true;
            if ((coordinate.Y + radius) < -5 || (coordinate.Y - radius > Game.Game_Height + 5) || (coordinate.X + radius < -5) || (coordinate.X - radius > Game.Game_Width + 5))
                disappeared = true;
        }
        public override bool Collision(Player e) //判断对于玩家的碰撞
        {
            return false;
        }

        //动画效果
        int x;
        float thelasttick1;
        public void animation(float t)
        {
            if (Time_cacullate - thelasttick1 > 0.1f)
            {
                x = x == 7 ? 0 : x + 1;
                myrect.X = 32 * x + 256;
                thelasttick1 = Time_cacullate;
            }
        }

        protected override void Fire(Vector v)        //带有一个方向参数的开火方法
        {
            switch (_myname)
            {
                case Enemy_black_Servitors.Enemy_black_Servitor: //使魔1 的开火（带有方向）
                    Game.Bulltes_toAdd.Add(new stg.bullet.Arrow_blue(coordinate.X,
                        coordinate.Y, stg.bullet.Bullet_Name.Arrow_blue1, v.X, v.Y));
                    break;
                case Enemy_black_Servitors.Enemy_black_Servitor2://使魔2 的开火
                    Game.Bulltes_toAdd.Add(new stg.bullet.Arrow_blue(coordinate.X, coordinate.Y,
                                             stg.bullet.Bullet_Name.Arrow_blue2, v.X, v.Y));
                    break;
            }
        }
        protected override void Fire(stg.bullet.Bullet_Name name, Vector v)         //若要指定发射什么子弹，并且设定一个初始化的方向，则用此方法
        {
            Game.Bulltes_toAdd.Add(new stg.bullet.Arrow_blue(coordinate.X, coordinate.Y, name, v.X, v.Y));
        }
    }
}
