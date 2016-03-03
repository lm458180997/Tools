using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    class Reimu_bullet_fire : Bullet
    {
        private RectangleF myrect = new RectangleF(0, 176, 64, 16);
        private int Power;
        public Reimu_bullet_fire(float x, float y, float speed = 1000,int Power = 5)
        {
            this.radius = 2;
            this.speed = speed;
            this.direction = new Vector(0, -1);
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.Power = Power;
        }
        public override void Draw(Graphics g)   //根据不同的名字制定不同的绘画
        {
            g.TranslateTransform(coordinate.X, coordinate.Y);
            g.RotateTransform(-90);
            g.TranslateTransform(-coordinate.X, -coordinate.Y);
            if (Time_calculate > 0.05f)
               g.DrawImage(images.Reim, new RectangleF(coordinate.X -48, coordinate.Y - 8, 48, 16), 
                   myrect, GraphicsUnit.Pixel);
            else
                g.DrawImage(images.Reim, new RectangleF(coordinate.X - 8, coordinate.Y - 8, 24, 16),
                    myrect, GraphicsUnit.Pixel);
            g.TranslateTransform(coordinate.X, coordinate.Y);
            g.RotateTransform(90);
            g.TranslateTransform(-coordinate.X, -coordinate.Y);
            //g.FillRectangle(Brushes.Yellow, coordinate.X - radius, coordinate.Y - radius, 2 * radius, 2 * radius);
        }

        public override void Update(float t)   //根据不同的名字制定不同的逻辑更新
        {
            Time_calculate += t;
            coordinate.X += speed * direction.X * t;
            coordinate.Y += speed * direction.Y * t;
            if (this.isoutofForm())
                this.disappeared = true;
        }
        public override bool Collision(Enemy e)        //根据不同的名字制定不同的判断逻辑,判断是否与敌机相撞
        {
            if (e.disappeared||!e.working)           //如果已经消失或停止工作了就不用再判定了
                return false;
            float len = (float)Math.Sqrt((this.coordinate.X - e.coordinate.X) * (this.coordinate.X - e.coordinate.X) +
                (this.coordinate.Y - e.coordinate.Y) * (this.coordinate.Y - e.coordinate.Y));
            if (len < radius + e.radius)
            {
                e.My_Damages.Add(new DamageEventArgs(Power));    //敌机的击中处理列表添加

                this.disappeared = true;
                this.working = false;          //击中后working结束，并执行动画（动画完毕后disappeared为true）
                return true;
            }
            return false;
        }
    }
}
