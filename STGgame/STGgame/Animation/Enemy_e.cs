using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.Animation
{
    public class Enemy_e :Animation
    {
        private Enemy enemy;
        private RectangleF myrect = new RectangleF(96, 224, 32, 32); //图片中的选择区域
        private float rotation = 0;                           //旋转角度
        public Enemy_e(Enemy e)
        {
            this.enemy = e;
        }
        public override void Update(UpdateEventArgs e)
        {
            if (!enemy.disappeared && working)
            {
                rotation += (e.Delttime * 120) % 360;           //每秒旋转120度
            }
            else
                working = false;
        }

        public override void Draw(Graphics g)
        {
            if (!enemy.disappeared && working)    //慢速下才形成动画
            {
                //旋转
                g.TranslateTransform(enemy.coordinate.X, enemy.coordinate.Y);
                g.RotateTransform(rotation);
                g.TranslateTransform(-enemy.coordinate.X, -enemy.coordinate.Y);

                //绘制
                g.DrawImage(images.enemies1,
                    new RectangleF(enemy.coordinate.X - 16, enemy.coordinate.Y - 16, 32, 32), myrect, GraphicsUnit.Pixel);

                //复位
                g.TranslateTransform(enemy.coordinate.X, enemy.coordinate.Y);
                g.RotateTransform(-rotation);
                g.TranslateTransform(-enemy.coordinate.X, -enemy.coordinate.Y);
            }
        }
    }
}
