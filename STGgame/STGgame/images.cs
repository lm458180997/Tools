using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Drawing.Drawing2D;
using System.Drawing;

namespace stg   
{
    public static class images     //图片类
    {
        //temporary
       public static Bitmap arrow_blue = new Bitmap(stg.Properties.Resources.arrow_blue);
       public static Bitmap arrow_gold = new Bitmap(stg.Properties.Resources.arrow_gold);
       public static Bitmap bigjade_white = new Bitmap(stg.Properties.Resources.bigjade_white);
       public static Bitmap ellipse_blue = new Bitmap(stg.Properties.Resources.ellipse_blue);
       public static Bitmap enemy_bigblack = new Bitmap(stg.Properties.Resources.enemy_bigblack);
       public static Bitmap enemy_black_1 = new Bitmap(stg.Properties.Resources.enemy_black_1);
       public static Bitmap enemy_black_2 = new Bitmap(stg.Properties.Resources.enemy_black_2);
       public static Bitmap enemy_littleblue_1 = new Bitmap(stg.Properties.Resources.enemy_littleblue_1);
       public static Bitmap enemy_littleblue_2 = new Bitmap(stg.Properties.Resources.enemy_littleblue_2);
       public static Bitmap forest = new Bitmap(stg.Properties.Resources.forest);
       public static Bitmap knife_blue = new Bitmap(stg.Properties.Resources.knife_blue);
       public static Bitmap knife_gray = new Bitmap(stg.Properties.Resources.knife_gray);
       public static Bitmap knife_lightblue = new Bitmap(stg.Properties.Resources.knife_lightblue);
       public static Bitmap knife_red = new Bitmap(stg.Properties.Resources.knife_red);
       public static Bitmap knife_white = new Bitmap(stg.Properties.Resources.knife_white);
       public static Bitmap Lin_bullet_small = new Bitmap(stg.Properties.Resources.Lin_bullet_small);
       public static Bitmap Lin_forward = new Bitmap(stg.Properties.Resources.Lin_forward);
       public static Bitmap rice_blue = new Bitmap(stg.Properties.Resources.rice_blue);
       public static Bitmap rice_white = new Bitmap(stg.Properties.Resources.rice_white);
       public static Bitmap Reim = new Bitmap(stg.Properties.Resources.pl00);
       public static Bitmap Reim1 = new Bitmap(stg.Properties.Resources.player00);
       public static Bitmap Reimu_effect = new Bitmap(stg.Properties.Resources.eff_sloweffect);

       //bullets
       public static Bitmap bullet1 = new Bitmap(stg.Properties.Resources.bullet1);
       public static Bitmap bullet2 = new Bitmap(stg.Properties.Resources.bullet2);
       public static Bitmap bullet3 = new Bitmap(stg.Properties.Resources.bullet3);

       //background
       public static Bitmap stage01a = new Bitmap(stg.Properties.Resources.stage01a);
       public static Bitmap stage01e = new Bitmap(stg.Properties.Resources.stage01e);
       public static Bitmap stage01f = new Bitmap(stg.Properties.Resources.stage01f);
       public static Bitmap stage02a = new Bitmap(stg.Properties.Resources.stage02a);
       public static Bitmap stage02b = new Bitmap(stg.Properties.Resources.stage02b);
       public static Bitmap stage02c = new Bitmap(stg.Properties.Resources.stage02c);


        //enemies
       public static Bitmap enemies1 = new Bitmap(stg.Properties.Resources.enemys1);
       public static Bitmap enemies2 = new Bitmap(stg.Properties.Resources.enemy);
       public static Bitmap enemies3 = new Bitmap(stg.Properties.Resources.enemy2);
       public static Bitmap enemies3b = new Bitmap(stg.Properties.Resources.enemy2);

        //initialize
       public static void init() 
       {
           enemies3b.RotateFlip(RotateFlipType.RotateNoneFlipX);  // 无旋转的水平翻转
           //enemies3b.MakeTransparent(Color.White);     将某种颜色透明化
       }
       
    }
}
