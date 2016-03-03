using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    public class Stage1_2 :Bullet
    {

        private RectangleF myrect = new RectangleF(160, 96, 16, 15);
        public Stage1_2(float x, float y , float vx,float vy,float speed = 100)
        {
            this.radius = 2;
            this.speed = speed;
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.direction = new Vector(vx, vy);
        }
        public Stage1_2(float x, float y, Vector v, float speed = 100)
        {
            this.radius = 2;
            this.speed = speed;
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.direction = v;
        }
     
        public override void Draw(Graphics g)   
        {
            float curve = direction.getcurve();
            g.TranslateTransform(coordinate.X, coordinate.Y);
            g.RotateTransform(curve);
            g.TranslateTransform(-coordinate.X, -coordinate.Y);
            g.DrawImage(images.bullet1, new RectangleF(coordinate.X - 8, coordinate.Y - 8, 16, 16), 
                myrect, GraphicsUnit.Pixel);
            g.TranslateTransform(coordinate.X, coordinate.Y);
            g.RotateTransform(-curve);
            g.TranslateTransform(-coordinate.X, -coordinate.Y);
            //g.FillRectangle(Brushes.Black, coordinate.X - radius, coordinate.Y - radius, 2 * radius, 2 * radius);
        }

        public override void Update(float t)   //根据不同的名字制定不同的逻辑更新
        {
            Time_calculate += t;
            coordinate.X += speed * direction.X * t;
            coordinate.Y += speed * direction.Y * t;
            if (this.isoutofForm())
                this.disappeared = true;
        }
    }
}
