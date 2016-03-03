using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.bullet
{
    public class Arrow_blue : Bullet
    {
        
        public stg.bullet.Bullet_Name name;           //子弹的名字
        private TextureBrush my_texturebrush;//子弹的纹理画刷，用于绘制子弹
        private TextureBrush my_change;      //子弹的变身纹理画刷

        public stg.bullet.Bullet_Name Name               //根据不同的名字，制定不同的初始化方案
        {
            get { return name; }
            set
            {
                name = value;
                switch (value)
                {
                    case stg.bullet.Bullet_Name.Arrow_blue1:          //Arrow_blue子弹
                        if (disappeared == true)
                            break;
                        this.direction = new Vector(0, 0);
                        this.radius = 1.5f;
                        this.speed = 75;
                        my_texturebrush = new TextureBrush(images.arrow_blue);
                        my_change = new TextureBrush(images.ellipse_blue);            // 变身前的纹理
                        break;
                    case stg.bullet.Bullet_Name.Arrow_blue2:          //Arrow_blue2子弹
                        if (disappeared)
                            break;
                        this.direction = new Vector(0, 0);
                        this.radius = 1.5f;
                        this.speed = 75;
                        my_texturebrush = new TextureBrush(images.arrow_blue);
                        my_change = new TextureBrush(images.ellipse_blue);
                        break;
                }
            }
        }

        public Arrow_blue(float x,float y, stg.bullet.Bullet_Name name, float vx,float vy) //多提供一种方向的构造
        {
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.Name = name;
            //初始化方向,
            this.direction.X = vx;   //this.direction = v; 这样是错的，传过去的是引用，而不是值
            this.direction.Y = vy;   //这种错误很容易犯，需要牢记
        }

        public override void Draw(Graphics g)   //根据不同的名字制定不同的绘画
        {
            //GraphicsState s;
            float angle=direction.getcurve();
            switch (name)
            {
                case stg.bullet.Bullet_Name.Arrow_blue1:            //Arrow_blue子弹,用于黑色妖精的使魔1
                        g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
                        g.RotateTransform(angle);//旋转角度
                        g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位

                        if (time_calculate < 1f)                                      //子弹的变身阶段使用另一个纹理
                        {
                            my_change.TranslateTransform(coordinate.X - 10, coordinate.Y - 10);
                            g.FillRectangle(my_change, coordinate.X - 10, coordinate.Y - 10, 20, 20);
                            my_change.TranslateTransform(-(coordinate.X - 10), -(coordinate.Y - 10));
                        }
                        else
                        {
                            my_texturebrush.TranslateTransform(coordinate.X - 15f, coordinate.Y - 15f);
                            g.FillRectangle(my_texturebrush, coordinate.X - 15f, coordinate.Y - 15f, 30f, 30f);
                            my_texturebrush.TranslateTransform(-(coordinate.X - 15f), -(coordinate.Y - 15f));  
                        }
                        g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
                        g.RotateTransform(-angle);//旋转角度
                        g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位
                    break;
                case stg.bullet.Bullet_Name.Arrow_blue2:            //Arrow_blue子弹,用于黑色妖精的使魔2
                        g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
                        g.RotateTransform(angle);//旋转角度
                        g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位

                        if (time_calculate < 1f)                                      //子弹的变身阶段使用另一个纹理
                        {
                            my_change.TranslateTransform(coordinate.X - 10, coordinate.Y - 10);
                            g.FillRectangle(my_change, coordinate.X - 10, coordinate.Y - 10, 20, 20);
                            my_change.TranslateTransform(-(coordinate.X - 10), -(coordinate.Y - 10));
                        }
                        else
                        {
                            my_texturebrush.TranslateTransform(coordinate.X - 15f, coordinate.Y - 15f);
                            g.FillRectangle(my_texturebrush, coordinate.X - 15f, coordinate.Y - 15f, 30f, 30f);
                            my_texturebrush.TranslateTransform(-(coordinate.X - 15f), -(coordinate.Y - 15f));    
                        }

                        g.TranslateTransform(coordinate.X, coordinate.Y);   //平移
                        g.RotateTransform(-angle);//旋转角度
                        g.TranslateTransform(-coordinate.X, -coordinate.Y); //平移复位
                    break;
            }
        }

        float time_calculate = 0;              //记录弹幕出现的总时间
        float lasttick = 0;
        public override void Update(float t)   //根据不同的名字制定不同的逻辑更新
        {
            switch (name)
            {
                case stg.bullet.Bullet_Name.Arrow_blue1:            //Arrow_blue子弹（黑色妖精的使魔1会使用）
                    time_calculate += t;
                    if (time_calculate < 1.2f)
                    {
                        if (time_calculate - lasttick < 0.1f)
                        {
                            direction.rotate(-(time_calculate - lasttick) / 1 * 0);  //1秒正转动0度
                            lasttick = time_calculate;
                        }
                    }
                    else if (time_calculate < 1.8f)
                    {
                        if (time_calculate - lasttick < 0.1f)
                        {
                            direction.rotate(-(time_calculate - lasttick) / 1 * 20);  //1秒正转动20度
                            lasttick = time_calculate;
                        }
                    }
                    coordinate.X += speed * direction.X * t;
                    coordinate.Y += speed * direction.Y * t;
                    if (this.isoutofForm())
                        this.disappeared = true;
                    break;
                case stg.bullet.Bullet_Name.Arrow_blue2:            //Arrow_blue2子弹（黑色妖精的使魔2会使用）
                    time_calculate += t;
                    
                    if (time_calculate < 1.2f)
                    {
                        if (time_calculate - lasttick < 0.1f)
                        {
                            direction.rotate((time_calculate - lasttick) / 1 *0);  //1秒逆转动0度
                            lasttick = time_calculate;
                        }
                    }
                    else if (time_calculate < 1.8f)
                    {
                        if (time_calculate - lasttick < 0.1f)
                        {
                            direction.rotate((time_calculate - lasttick) / 1 * 20);  //1秒正转动20度
                            lasttick = time_calculate;
                        }
                    }
                    coordinate.X += speed * direction.X * t;
                    coordinate.Y += speed * direction.Y * t;
                    if (this.isoutofForm())
                        this.disappeared = true;
                    break;
            }
        }
        public override bool Collision(Player p)       //根据不同的名字制定不同的判断逻辑
        {
            float len = (float)Math.Sqrt(((coordinate.X - Game.player.coordinate.X) *
                (coordinate.X - Game.player.coordinate.X) + (coordinate.Y - Game.player.coordinate.Y) *
                (coordinate.Y - Game.player.coordinate.Y)));
            if (len < radius + Game.player.radius)
            {
                disappeared = true;
                return true;
            }
            return false;
        }
    }
}
