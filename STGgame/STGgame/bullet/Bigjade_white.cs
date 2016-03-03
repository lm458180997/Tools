using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    public class Bigjade_white:Bullet
    {

        private TextureBrush my_texturebrush;//子弹的纹理画刷，用于绘制子弹

        public Bigjade_white(float x ,float y) 
        {
            this.coordinate.X = x;
            this.coordinate.Y = y;
            
            this.direction = getdirection();
            this.radius = 9;
            this.speed = 50;
            my_texturebrush = new TextureBrush(images.bigjade_white);
        }

        public override void Draw(Graphics g)   //根据不同的名字制定不同的绘画
        {
            //GraphicsState s;
            float angle = direction.getcurve();

            g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
            g.RotateTransform(angle);//旋转角度
            g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位

            my_texturebrush.TranslateTransform(coordinate.X - 25f, coordinate.Y - 25f);
            g.FillRectangle(my_texturebrush, coordinate.X - 25f, coordinate.Y - 25f, 50f, 50f);
            my_texturebrush.TranslateTransform(-(coordinate.X - 25f), -(coordinate.Y - 25f));

            g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
            g.RotateTransform(-angle);//旋转角度
            g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位
        }

        public override void Update(float t)   //根据不同的名字制定不同的逻辑更新
        {
            coordinate.X += speed * direction.X * t;
            coordinate.Y += speed * direction.Y * t;
            if (this.isoutofForm())
                this.disappeared = true;
        }
        float len;
        public override bool Collision(Player p)       //根据不同的名字制定不同的判断逻辑
        {
            len = (float)Math.Sqrt(((coordinate.X - Game.player.coordinate.X) * (coordinate.X - Game.player.coordinate.X) + 
                (coordinate.Y - Game.player.coordinate.Y) * (coordinate.Y - Game.player.coordinate.Y)));
            if (len < radius + Game.player.radius)
                return true;
            return false;
        }
    }
}
