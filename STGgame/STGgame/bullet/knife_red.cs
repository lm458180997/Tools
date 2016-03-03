using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    public class knife_red:Bullet
    {

        private TextureBrush my_texturebrush;//子弹的纹理画刷，用于绘制子弹

        public knife_red(float x ,float y, Vector v)            //构造
        {
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.radius = 1;
            this.speed = 200;
            my_texturebrush = new TextureBrush(images.knife_red);
            //初始化方向,
            this.direction = new Vector(0, 0);
            this.direction.X = v.X;   //this.direction = v; 这样是错的，传过去的是引用，而不是值
            this.direction.Y = v.Y;   //这种错误很容易犯，需要牢记
        }

        public override void Draw(Graphics g)   //根据不同的名字制定不同的绘画
        {

            float angle = direction.getcurve();

            g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
            g.RotateTransform(angle);//旋转角度
            g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位

            my_texturebrush.TranslateTransform(coordinate.X - 12.5f, coordinate.Y - 12.5f);    //将画刷平移到需要的点上
            g.FillRectangle(my_texturebrush, coordinate.X - 12.5f, coordinate.Y - 12.5f, 25f, 25f);
            my_texturebrush.TranslateTransform(-(coordinate.X - 12.5f), -(coordinate.Y - 12.5f));  //将画刷复位

            g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
            g.RotateTransform(-angle);//旋转角度
            g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位

        }

        float time_calculate = 0;              //记录弹幕出现的总时间
        float Ticks = 0;                       //ticks 和 lasttick用于制作事件
        float lasttick = 0;
        public override void Update(float t)   //根据不同的名字制定不同的逻辑更新
        {
            time_calculate += t;
            Ticks = time_calculate;
            if (Ticks - lasttick > 0.1f)
            {
                direction.rotate(-5);
                speed += 10;
                lasttick = Ticks;
            }
            coordinate.X += speed * direction.X * t;
            coordinate.Y += speed * direction.Y * t;
            if (this.isoutofForm())
                this.disappeared = true;

        }
        public override bool Collision(Player p)       //根据不同的名字制定不同的判断逻辑
        {
            float len = (float)Math.Sqrt(((coordinate.X - Game.player.coordinate.X) * (coordinate.X - Game.player.coordinate.X) + 
                (coordinate.Y - Game.player.coordinate.Y) * (coordinate.Y - Game.player.coordinate.Y)));
            return false;
        }
    }
}
