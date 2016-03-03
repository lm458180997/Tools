using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing;

namespace stg
{
    public class Bullet : Entity     //子弹
    {
        public float Time_calculate=0;    //记录时间，根据进行的时间进行特殊的事件处理

        public Bullet() { }

        public override void Draw(Graphics g) { } //根据不同的名字制定不同的绘画

        public override void Update(float t) { }  //根据不同的名字制定不同的逻辑更新

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

        public override bool Collision(Enemy e) { return false; }       //根据不同的名字制定不同的判断逻辑,判断是否与敌机相撞
        
        public virtual Vector getdirection()                //返回子弹指向玩家的方向
        {
            Vector v = new Vector();
            v.X = (Game.player.coordinate.X - coordinate.X);
            v.Y = (Game.player.coordinate.Y - coordinate.Y);
            v.Normalize();                          //获取指向自机方向的向量
            return v;
        }
        public virtual bool isoutofForm()
        {
            if (coordinate.X + radius < Game.Game_X - 10 || coordinate.X - radius > Game.Game_X + Game.Game_Width + 10 ||
                coordinate.Y + radius < Game.Game_Y - 10 || coordinate.Y - radius > Game.Game_Y + Game.Game_Height + 10)
                return true;
            return false;
        }
    }
}
