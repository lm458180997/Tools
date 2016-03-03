using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.enemy
{
   public class Enemy_NormalEnemy:Enemy
    {
       public Enemy_NormalEnemy(Point p)
       {
            this.coordinate.X = p.X;
            this.coordinate.Y = p.Y;
            this.speed = 50;                            //初始速度设为30
            this.direction = new Vector(0, 1);          //初始移动方向为正前
            this.radius = 20;
            disappeared = false;                        //消失状态表示为否
            my_texturebrush = new TextureBrush(images.enemy_littleblue_1);
       }
       public Enemy_NormalEnemy(Point p,Vector v)
       {
            this.coordinate.X = p.X;
            this.coordinate.Y = p.Y;
            this.speed = 50;                            //初始速度设为30
            this.direction = new Vector(0, 1);          //初始移动方向为正前
            this.radius = 20;
            disappeared = false;                        //消失状态表示为否
            my_texturebrush = new TextureBrush(images.enemy_littleblue_1);

            this.direction.X = v.X;                     //设置方向
            this.direction.Y = v.Y;
       }
       public override void Draw(Graphics g)
       {

           if (direction.X != 0)                              //根据不同的飞行状态，贴上不同的图
           {
               my_texturebrush = new TextureBrush(stg.Properties.Resources.enemy_littleblue_2);
           }
           else
           {
               my_texturebrush = new TextureBrush(stg.Properties.Resources.enemy_littleblue_1);
           }
           my_texturebrush.TranslateTransform(coordinate.X - 25, coordinate.Y - 25);              //绘制敌人，将画刷平移到点上
           g.FillRectangle(my_texturebrush, coordinate.X - 25, coordinate.Y - 25, 50, 50);
           my_texturebrush.TranslateTransform(-(coordinate.X - 25), -(coordinate.Y - 25));
          
       }

        float lastick = 0;           //用于实现子弹的连续发射,部分逻辑更新时会用到多个
        float lastick1 = 0;          //部分enemy发射会用到的
        float Ticks = 0;
        public override void Update(float t)      //逻辑更新
        {

            Time_cacullate += t;
            if (Time_cacullate < 1)
            {
                coordinate.X += direction.X * speed * t;
                coordinate.Y += direction.Y * speed * t;
            }
           
            if (Time_cacullate < 4 && Time_cacullate > 1)
            {
                if (Time_cacullate - lastick1 > 0.2f)
                {
                    Fire();
                    lastick1 = Time_cacullate;
                }
            }

            if (Time_cacullate > 4 && Time_cacullate < 5)
            {
                direction -= new Vector(-0.01, 0);
                coordinate.X += direction.X * speed * t;
                coordinate.Y += direction.Y * speed * t;
                Ticks += t;                          //执行间隔时间计算
                if (Ticks - lastick >= 0.5f)
                {
                    Fire();
                    lastick = Ticks;
                }
            }
            if (Time_cacullate > 5)
            {
                Ticks += t;                          //执行间隔时间计算
                if (Ticks - lastick >= 0.2f)
                {
                    if (!disappeared)                 //如果没消失，则还可以发射
                        Fire();
                    lastick = Ticks;
                }
                coordinate.X += direction.X * speed * t;
                coordinate.Y += direction.Y * speed * t;
            }

            //如果机体飞出了界外则判定为消失，  disappeared = true;
            if ((coordinate.Y + radius) < -5 || (coordinate.Y - radius > Game.Game_Height + 5) || (coordinate.X + radius < -5) || (coordinate.X - radius > Game.Game_Width + 5))
                disappeared = true;
        }
        public override bool Collision(Player e) //判断对于玩家的碰撞
        {
            return false;
        }
        protected override void Fire()                           //敌机的开火（无方向参数）
        {
            Bullet b;
            b = new stg.bullet.Knife_gray(coordinate.X, coordinate.Y + 25f);
            Game.Bulltes_toAdd.Add(b);
        }
    }
}
