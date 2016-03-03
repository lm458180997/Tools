using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.enemy
{
    class Enemy_LittleSyren:Enemy
    {
        public Enemy_LittleSyren(PointF p)       //提供坐标,生成enemy
        {
            this.coordinate.X = p.X;
            this.coordinate.Y = p.Y;
            this.speed = 50;
            this.radius = 20;
            this.direction = new Vector(0, 1);
            this.HP = 200;
            disappeared = false;                        //消失状态表示为否
            Fire_direction = new Vector(0, 1);          //子弹方向初始为正前
            Fire_direction1 = new Vector(0, -1);         //第二种状态的子弹初始为正前
            my_texturebrush = new TextureBrush(images.enemy_littleblue_1);
        }
        
        public override void Draw(Graphics g)
        {
            my_texturebrush.TranslateTransform(coordinate.X - 25, coordinate.Y - 25);                   //绘制敌人
            g.FillRectangle(my_texturebrush, coordinate.X - 25, coordinate.Y - 25, 50, 50);
            my_texturebrush.TranslateTransform(-(coordinate.X - 25), -(coordinate.Y - 25));
        }

        float lastick = 0;           //用于实现子弹的连续发射,部分逻辑更新时会用到多个
        float Ticks = 0;
        public override void Update(float t)      //逻辑更新
        {
            Time_cacullate += t;
            Ticks = Time_cacullate;
            
            //hit判定
            if (Hited == true)    //被打中一次减少10血
            {
                HP -= 10;
                this.Hited = false;    //复位为未击中状态
            }
            if (HP < 0)           //当血空后就消失（毁灭）掉
            {
                HP = 0;
                this.disappeared = true;
            }

            //动作逻辑
            if (Ticks - lastick >= 0.1f)
            {
                int i = new Random().Next(10);
                if (i > 7)                           //随机位置发生偏移
                    Fire_direction.rotate(-1);
                Fire_direction.rotate(-10);
                Fire_direction1.rotate(10);
                Fire(Fire_direction, Fire_direction1);
                Fire(Fire_direction.opposite(), Fire_direction1.opposite());  //获得与之相反的方向
                lastick = Ticks;
            }
            
            //如果机体飞出了界外则判定为消失，  disappeared = true;
            if ((coordinate.Y + radius) < -5 || (coordinate.Y - radius > Game.Game_Height + 5) || (coordinate.X + radius < -5) || (coordinate.X - radius > Game.Game_Width + 5))
                disappeared = true;
        }
        public override bool Collision(Player e) //判断对于玩家的碰撞
        {
            return false;
        }

        protected override void Fire(Vector v1, Vector v2)        //带有两个方向参数的开火方法
        {
            Game.Bulltes_toAdd.Add(new stg.bullet.knife_red(coordinate.X, coordinate.Y, v1));
            Game.Bulltes_toAdd.Add(new stg.bullet.knife_blue(coordinate.X, coordinate.Y, v2));
        }
    }
}
