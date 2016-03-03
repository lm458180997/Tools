using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.Animation
{
    public class Escape : Animation
    {
        private float x1, y1, x2, y2, time,current;
        private Enemy enemy;
        public Escape(Enemy e, float x, float y, float _x, float _y, float t)
        {
            enemy = e;
            x1 = x; y1 = y; x2 = _x; y2 = _y; time = t;
        }

        public override void Update(UpdateEventArgs e)
        {
            if (working && !enemy.disappeared)
            {
                current += e.Delttime;
                current = current > time ? time : current;   //防止值过大跑出
                pro(current / time);
                if (current == time)
                    this.working = false;
            }
            else
                this.working = false;
        }
        private void pro(float percent)
        {
            float p = percent*percent;            // 变速运动的效果
            enemy.coordinate.X = (x1 + (x2 - x1) * p);
            enemy.coordinate.Y = (y1 + (y2 - y1) * p);
        }
    }
}
