using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.enemy
{
    public class Stage1_2 : Enemy
    {
        private RectangleF myrect1 = new RectangleF(0, 64, 32, 32);
        private RectangleF myrect2 = new RectangleF(128, 64, 32, 32);
        private RectangleF myrect3 = new RectangleF(352, 64, 32, 32);
        float distance;
        float fly_time;                           //出现时的飞行时间
        float Add_speed;                          //进入时的加速度
        float Add_speed2;                         //离开时的加速度
        public Stage1_2(float x, float y)
       {
            this.coordinate.X = x;
            this.coordinate.Y = y;
            this.radius = 20;
            disappeared = false;                        //消失状态表示为否
            distance =(float) Utils.Getdistance(coordinate.X, coordinate.Y)*0.5f; // 3/5个点到player的距离
            fly_time = 3;
            this.HP = 200;
       }

       int change_X;
       public override void Draw(Graphics g)
       {
           if (this.disappeared || !this.working)
               return;
           if(direction.X < 0&&speed>30)
           {
               g.DrawImage(images.enemies3,
                   new RectangleF(coordinate.X - 16, coordinate.Y - 16, 32, 32), myrect2, GraphicsUnit.Pixel);
           }
           else if (direction.X > 0 && speed > 30)
           {
               g.DrawImage(images.enemies3b,
                   new RectangleF(coordinate.X - 16, coordinate.Y - 16, 32, 32), myrect3, GraphicsUnit.Pixel);
           }
           else
           {
               g.DrawImage(images.enemies3,
                   new RectangleF(coordinate.X - 16, coordinate.Y - 16, 32, 32), myrect1, GraphicsUnit.Pixel);
           }
       }

        float lastick = 0;                
        float lastick2 = 0;          
        public override void Update(float t)      
        {
            Time_cacullate += t;

            Damage_process();                              // 血量处理

            if (Time_cacullate < fly_time)
            {
                this.speed -= Add_speed * t;
            }
            else
            {
                AppointedEvent(1);                          //执行一次反向操作
                this.speed += Add_speed2 * t;
            }
            if (Time_cacullate > 1.2f)
            {
                if (Time_cacullate - lastick >1f)
                {
                    Fire();
                    lastick = Time_cacullate;
                }
            }

            #region     动画换帧效果
            if (Time_cacullate - lastick2 > 0.1f)               
            {
                change_X = change_X + 1 > 3 ? 0 : change_X + 1;
                myrect1.X = change_X * 32;
                lastick2 = Time_cacullate;
            }
            #endregion
            #region 移动
                coordinate.X += direction.X * speed * t;
                coordinate.Y += direction.Y * speed * t;
            #endregion
           
            if (OutOfFrame())
                disappeared = true;
        }
        public override bool Collision(Player e)
        {
            return false;
        }
        protected override void Fire()                        
        {
           // Game.Bulltes_toAdd.Add(new bullet.Stage1_1(coordinate.X,coordinate.Y+8,getdirection(),150)); 
            Game.Bulltes_toAdd.Add(new bullet.Stage1_2(coordinate.X, coordinate.Y + 8, getdirection(), 150));
        }

        protected override void AppointedEvent(int times)       //指定次数的任务
        {
            if(eventcount<times)
               this.direction = this.direction.opposite();
            eventcount += 1;
        }
    }
}
