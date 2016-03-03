using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    public class Knife_gray : Bullet
    {

        private TextureBrush my_texturebrush;//子弹的纹理画刷，用于绘制子弹

        public Knife_gray()
        {
            this.direction.X = 0;
            this.direction.Y = 1;
            this.direction = getdirection(); //指向玩家的方向
            this.radius = 1;
            this.speed = 300;
            my_texturebrush = new TextureBrush(images.knife_gray);
        }
        public Knife_gray(float x,float y) //用点和半径的方式来表示子弹
        {
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.direction = getdirection(); //指向玩家的方向
            this.radius = 1;
            this.speed = 300;
            my_texturebrush = new TextureBrush(images.knife_gray);
        }
        public Knife_gray(PointF f, Vector v) //多提供一种方向的构造
        {
            this.coordinate.X = f.X;
            this.coordinate.Y = f.Y;
            this.radius = 1;
            this.speed = 300;
            my_texturebrush = new TextureBrush(images.knife_gray);
            //初始化方向,
            this.direction.X = v.X;   //this.direction = v; 这样是错的，传过去的是引用，而不是值
            this.direction.Y = v.Y;   //这种错误很容易犯，需要牢记
        }

        public override void Draw(Graphics g)   //根据不同的名字制定不同的绘画
        {
            float angle = direction.getcurve();

            g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
            g.RotateTransform(angle);//旋转角度
            g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位

            my_texturebrush.TranslateTransform(coordinate.X - 12.5f, coordinate.Y - 12.5f);
            g.FillRectangle(my_texturebrush, coordinate.X - 12.5f, coordinate.Y - 12.5f, 25f, 25f);
            my_texturebrush.TranslateTransform(-(coordinate.X - 12.5f), -(coordinate.Y - 12.5f));

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
        public override bool Collision(Player p)       //根据不同的名字制定不同的判断逻辑
        {
            float len = (float)Math.Sqrt(((coordinate.X - Game.player.coordinate.X) * (coordinate.X - Game.player.coordinate.X) + (coordinate.Y - Game.player.coordinate.Y) * (coordinate.Y - Game.player.coordinate.Y)));
            return false;
        }
    }
}
