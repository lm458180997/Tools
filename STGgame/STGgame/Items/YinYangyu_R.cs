using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.Items
{
    public class YinYangyu_R : Item
    {
         private Player player;
        private float Rotate;
        private RectangleF myrect = new RectangleF(80, 144, 16, 16);
        private float Time_Caculate = 0;
        public YinYangyu_R(Player P)
        {
            this.player = P;
            this.coordinate.X = P.coordinate.X + 30;
            this.coordinate.Y = P.coordinate.Y + 5;
        }

        float OffRotate = 360;   //设置位置的偏转（慢速时移动至前端）
        public override void Update(float t)
        {
            Time_Caculate += t;
            Rotate += t * 360;           //1秒钟转一圈
            if (player.FlySlow)
            {
                OffRotate = OffRotate - t * 360 < 285 ? 285 : OffRotate - t * 360;
            }
            else
                OffRotate = OffRotate + t * 360 > 360 ? 360 : OffRotate + t * 360;
            this.coordinate.X = player.coordinate.X + 35 * (float)Math.Cos((OffRotate / 180) * PI);
            this.coordinate.Y = player.coordinate.Y+5 + 35 * (float)Math.Sin((OffRotate / 180) * PI);
        }

        public override void Draw(System.Drawing.Graphics g)
        {
            //旋转效果
            if (player.Level > 1)
            {
                g.TranslateTransform(coordinate.X, coordinate.Y);
                g.RotateTransform(Rotate);
                g.TranslateTransform(-coordinate.X, -coordinate.Y);
                g.DrawImage(images.Reim, new RectangleF(coordinate.X - 8, coordinate.Y - 8, 16, 16), myrect, GraphicsUnit.Pixel);
                g.TranslateTransform(coordinate.X, coordinate.Y);
                g.RotateTransform(-Rotate);
                g.TranslateTransform(-coordinate.X, -coordinate.Y);
            }
        }

        float Fire_cd1;
        float Fire_cd2;
        float Fire_cd3;
        public void Fire()
        {
            switch (player.Level)
            {
                case 1: break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    player.Player_Bullets.Add(new bullet.Reimu_bullet_fire(coordinate.X-10, coordinate.Y));
                    player.Player_Bullets.Add(new bullet.Reimu_bullet_fire(coordinate.X , coordinate.Y-5));
                    player.Player_Bullets.Add(new bullet.Reimu_bullet_fire(coordinate.X + 10, coordinate.Y));
                    break;
            }
        }
    }
}
