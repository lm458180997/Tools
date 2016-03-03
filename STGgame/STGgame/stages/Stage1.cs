using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.stages
{
    public class Stage1 : Stage            //里面装填进各种逻辑命令
    {
        private Random random = new Random();
        private RectangleF myrect1 = new RectangleF(0,0,32,32);
        private RectangleF myrect2 = new RectangleF(0,0,32,32);
        public Stage1()
        {
            this.Backgrounda = new System.Drawing.TextureBrush(images.stage02b);
            this.Backgrounde = new System.Drawing.TextureBrush(images.stage01e);
            this.Backgroundf = new System.Drawing.TextureBrush(images.stage01f);
            Backgrounda.TranslateTransform(-48, 0);
            this.Offset_ay = 1;
            this.Offset_ey = 1;
            this.Offset_fy = 1;
        }
        float BLACK;
        float A;
        float s_1;
        public override void Update(float t)
        {
            Time_Caculate += t;
            #region 地图
            if (Time_Caculate - lastick1 > 0.05f)
            {
                this.backgrounda.TranslateTransform(0, Offset_ay);
                this.TotalOffset_ay += Offset_ay;
                lastick1 += 0.05f;
            }
            #endregion
            #region 过程
            if (Time_Caculate < 3)
            {
            }
            else if (Time_Caculate < 5)
            {
                if (Time_Caculate - s_1 > 0.2f)
                {
                    Game.Enemy_toadd.Add(new enemy.Stage1_1(Game.Game_X + random.Next(Game.Game_Width / 3),
                        Game.Game_Y));
                    s_1 = Time_Caculate;
                }
            }
            else if (Time_Caculate < 7)
            {
                if (Time_Caculate - s_1 > 0.2f)
                {
                    Game.Enemy_toadd.Add(new enemy.Stage1_1(Game.Game_X + Game.Game_Width - random.Next(Game.Game_Width / 3),
                        Game.Game_Y));
                    s_1 = Time_Caculate;
                }
            }
            else if (Time_Caculate < 9)
            {
                if (Time_Caculate - s_1 > 0.2f)
                {
                    Game.Enemy_toadd.Add(new enemy.Stage1_1(Game.Game_X + random.Next(Game.Game_Width / 3),
                        Game.Game_Y));
                    s_1 = Time_Caculate;
                }
            }
            else if (Time_Caculate < 11)
            {
                if (Time_Caculate - s_1 > 0.2f)
                {
                    Game.Enemy_toadd.Add(new enemy.Stage1_1(Game.Game_X + Game.Game_Width - random.Next(Game.Game_Width / 3),
                        Game.Game_Y));
                    s_1 = Time_Caculate;
                }
            }

            if (Time_Caculate > 3 && Time_Caculate < 5 && Time_Caculate - BLACK > 4)
            {
                Enemy e = new enemy.Enemy_black(new System.Drawing.PointF(
                    Game.Game_X + Game.Game_Width / 2, Game.Game_Y - 50));
                //给Enemy_black添加出场动画
                new Animation.Show_enemy(e, Game.Game_X + Game.Game_Width / 2, Game.Game_Y - 50
                , Game.Game_X + Game.Game_Width / 2, Game.Game_Y + 150, 2).Act_Run();
                Game.Enemy_toadd.Add(e);
                BLACK = Time_Caculate;
            }
            #endregion
        }
        public override void Draw(System.Drawing.Graphics g)
        {
            g.FillRectangle(Backgrounda, Game.Game_rect);
           // g.FillRectangle(Backgrounde, Game.Game_rect);
            //g.FillRectangle(Backgroundf, Game.Game_rect);
        }
        
    }
}
