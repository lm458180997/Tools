using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    public class Stage1_1:Bullet
    {
        private RectangleF myrect = new RectangleF(0, 240, 8, 8);
        public Stage1_1(float x, float y , float vx,float vy,float speed = 80)
        {
            this.radius = 1;
            this.speed = speed;
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.direction = new Vector(vx, vy);
        }
        public Stage1_1(float x, float y, Vector v ,float speed = 80)
        {
            this.radius = 1;
            this.speed = speed;
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.direction = v;
        }
     
        public override void Draw(Graphics g)   
        {
            g.DrawImage(images.bullet1, new RectangleF(coordinate.X - 3, coordinate.Y - 3, 6, 6), 
                myrect, GraphicsUnit.Pixel);
            //g.FillRectangle(Brushes.Black, coordinate.X - radius, coordinate.Y - radius, 2 * radius, 2 * radius);
        }

        public override void Update(float t)   
        {
            Time_calculate += t;
            coordinate.X += speed * direction.X * t;
            coordinate.Y += speed * direction.Y * t;
            if (this.isoutofForm())
                this.disappeared = true;
        }
    }
}
