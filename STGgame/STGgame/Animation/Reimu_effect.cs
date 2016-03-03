using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.Animation
{
    public class Reimu_effect:Animation
    {
        Player player;
        private RectangleF myrect= new RectangleF(0,0,64,64); //图片中的选择区域
        private float rotation = 0;                           //旋转角度
        public Reimu_effect(Player player)
        {
            this.player = player;
        }
        public override void Update(UpdateEventArgs e)
        {
            if (!player.disappeared && working)
            {
                rotation += (e.Delttime * 90)%360;           //每秒旋转90度
            }
            else
                working = false;
        }
        public override void Draw(Graphics g)
        {
            if (player.FlySlow && !player.disappeared &&working)    //慢速下才形成动画
            {
                //旋转
                g.TranslateTransform(player.coordinate.X, player.coordinate.Y);
                g.RotateTransform(rotation);
                g.TranslateTransform(-player.coordinate.X, -player.coordinate.Y);

                //绘制
                g.DrawImage(images.Reimu_effect, 
                    new RectangleF(player.coordinate.X - 32, player.coordinate.Y - 32, 64, 64), myrect, GraphicsUnit.Pixel);
               
                //复位
                g.TranslateTransform(player.coordinate.X, player.coordinate.Y);
                g.RotateTransform(-rotation);
                g.TranslateTransform(-player.coordinate.X, -player.coordinate.Y);
            }
        }
    }
}
