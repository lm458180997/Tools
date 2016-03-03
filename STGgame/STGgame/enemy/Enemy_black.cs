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
    public class Enemy_black : Enemy
    {
        public Enemy_black(PointF p)       //提供坐标,生成enemy
        {
            this.coordinate.X = p.X;
            this.coordinate.Y = p.Y;

            this.speed = 50;
            this.HP = 100000;
            this.direction = new Vector(0, 1);
            this.radius = 20;

            this.My_Servitors = new List<Enemy>();      // 创建使魔列表
            this.My_Servitors_toAdd = new List<Enemy>();
            this.My_Servitors_toremove = new List<Enemy>();//使魔列表的移除
            this.HaveServitors = true;                   //标记为拥有使魔

            this.myrect = new RectangleF(0, 32, 32, 32);  //选择区域
            disappeared = false;
            my_texturebrush = new TextureBrush(images.enemy_black_1);
        }

        public override void Draw(Graphics g)
        {
            if (My_Servitors != null)              //绘制出每一个使魔(前提是此enemy拥有使魔)
            {
                foreach (Enemy en in My_Servitors)
                    en.Draw(g);
            }
            if (disappeared)           //如果已经消失了的话就不画出来
                return;
         
            g.DrawImage(images.enemies1, new RectangleF(coordinate.X - 16, coordinate.Y - 16, 32, 32),
                myrect, GraphicsUnit.Pixel);
        }

        float lastick = 0;           //用于实现子弹的连续发射,部分逻辑更新时会用到多个
        float lastick1 = 0;          //部分enemy发射会用到的
        float lastick2 = 0;          //用于实现最后逃离时的动画
        public override void Update(float t)      //逻辑更新
        {
            Time_cacullate += t;
            animation(t);
            if (!disappeared)
            {
                if(Time_cacullate>4)
                    AppointedEvent(1);           //执行扣除血量的操作（仅一次）
                foreach (DamageEventArgs re in My_Damages_ToRemove)  //丢弃抛弃列表中的成员
                    My_Damages.Remove(re);
                My_Damages_ToRemove.Clear();
                int i = My_Damages.Count;

                foreach (DamageEventArgs damage in My_Damages)       //击中时的处理
                {
                   this.HP = this.HP - damage.Demage < 0 ? 0 : this.HP - damage.Demage;
                   My_Damages_ToRemove.Add(damage);                 //处理结束后，就将其放入抛弃列表
                }
                if (HP == 0)
                {
                    this.disappeared = true;
                    foreach (Enemy e in My_Servitors)
                    {
                        e.disappeared = true; e.working = false;
                    }
                    this.working = false;
                }
                if (Time_cacullate < 3)
                {
                    if (Time_cacullate - lastick > 2.1f)    //实际上只能执行一次
                    {
                        Vector v = new Vector(-1, 0);
                        float r = (float)(18 / ((30f / 180f) * PI));
                        //添加使魔1
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new Point(Game.Game_X + 100, Game.Game_Y + 200),
                            Enemy_black_Servitors.Enemy_black_Servitor, v));
                        v.rotate(120);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 100 + r * ((float)Math.Cos((210f / 180) * PI)),
                            Game.Game_Y + 200 - r + r * ((float)Math.Sin((210f / 180) * PI))), Enemy_black_Servitors.Enemy_black_Servitor, v));
                        v.rotate(120);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 100 + r * ((float)Math.Cos((330f / 180) * PI)),
                            Game.Game_Y + 200 - r + r * ((float)Math.Sin((330f / 180) * PI))), Enemy_black_Servitors.Enemy_black_Servitor, v));

                        //添加使魔2
                        v.X = 1; v.Y = 0;
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new Point(Game.Game_X + 300, Game.Game_Y + 200),
                            Enemy_black_Servitors.Enemy_black_Servitor2, v));
                        v.rotate(120);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 300 - r * 0.86625f, Game.Game_Y + 200 - 1.5f * r),
                                                         Enemy_black_Servitors.Enemy_black_Servitor2, v));
                        v.rotate(120);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 300 + r * 0.86625f, Game.Game_Y + 200 - 1.5f * r),
                                                         Enemy_black_Servitors.Enemy_black_Servitor2, v));
                        lastick = Time_cacullate;

                        //中下部分的使魔集合
                        v.X = 1; v.Y = 0;
                        v.rotate(90);      
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 200 + r * ((float)Math.Cos((0f / 180) * PI)),
                                              Game.Game_Y + 270 + r * ((float)Math.Sin((0f / 180) * PI))), Enemy_black_Servitors.Enemy_black_Servitor, v));
                        v.rotate(90);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 200 + r * ((float)Math.Cos((90f / 180) * PI)),
                                             Game.Game_Y + 270 + r * ((float)Math.Sin((90f / 180) * PI))), Enemy_black_Servitors.Enemy_black_Servitor2, v.opposite()));
                        v.rotate(90);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 200 + r * ((float)Math.Cos((180f / 180) * PI)),
                                             Game.Game_Y + 270 + r * ((float)Math.Sin((180f / 180) * PI))), Enemy_black_Servitors.Enemy_black_Servitor, v));
                        v.rotate(90);
                        My_Servitors_toAdd.Add(new Enemy_black_Servitor(new PointF(Game.Game_X + 200 + r * ((float)Math.Cos((270f / 180) * PI)),
                                              Game.Game_Y + 270 + r * ((float)Math.Sin((270f / 180) * PI))), Enemy_black_Servitors.Enemy_black_Servitor2, v.opposite()));
                    }
                }
                if (Time_cacullate < 30)
                {
                    if (Time_cacullate - lastick1 > 3)
                    {
                        Fire();
                        lastick1 = Time_cacullate;
                    }
                    if (Time_cacullate - lastick2 > 25)
                    {
                        new Animation.Escape(this, coordinate.X, coordinate.Y, coordinate.X, -120, 3).Act_Run();
                    }
                }

            }

            //以下是所有敌机总体的逻辑更新

            //如果机体飞出了界外则判定为消失(包括使魔)，  disappeared = true;
            if ((coordinate.Y + radius) < Game.Game_Y - 100 || (Game.Game_Y + coordinate.Y - radius > Game.Game_Height + 5) || (coordinate.X + radius < -5) || (coordinate.X - radius > Game.Game_Width + 5))
            {
                disappeared = true;
                foreach (Enemy e in My_Servitors)
                    e.disappeared = true;
            }

            //使魔列表的逻辑更新（若没有使魔则直接跳过）
            foreach (Enemy a in My_Servitors)
            {
                if (a.disappeared == true) //如果使魔的子弹全跑去了外面而且自己也已经消失，则把这使魔给除去
                {
                    My_Servitors_toremove.Add(a);
                }
            }
            foreach (Enemy a in My_Servitors_toremove)
            {
                My_Servitors.Remove(a);
            }
            My_Servitors_toremove.Clear();
            foreach (Enemy a in My_Servitors_toAdd)
            {
                My_Servitors.Add(a);
            }
            My_Servitors_toAdd.Clear();
            foreach (Enemy a in My_Servitors)             //对每一个使魔都进行逻辑更新
            {
                a.Update(t);
            }
        }
        public override bool Collision(Player e) //判断对于玩家的碰撞
        {
            return false;
        }

        int x;    //敌机动画
        float thelasttick1;
        public void animation(float t)
        {
            if (Time_cacullate - thelasttick1 > 0.08f)
            {
                x = x == 1 ? 0 : x + 1;

                myrect.X = x * 32;
                thelasttick1 = Time_cacullate;
            }

        }
        protected override void Fire()                           //敌机的开火（无方向参数）
        {
            Game.Bulltes_toAdd.Add(new stg.bullet.Bigjade_white(coordinate.X, coordinate.Y + 25));
        }

        protected override void AppointedEvent(int times)       //扣除血量的操作（形成前4秒类似无敌的效果）
        {
            if (eventcount < times)
            {
                this.HP /= 50;
            }
            eventcount++;
        }
    }
}
