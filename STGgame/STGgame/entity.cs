using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;

using System.Windows.Forms;
using System.Drawing.Drawing2D;


namespace stg
{
    public abstract class Entity         //定义一个抽象的实体，其详细由子类来实现
    {
        public float speed;             //速度
        public RectangleF bounds;       //位置，大小
        public Vector direction;        //表示方向
        public PointF coordinate;       //表示坐标   
        public float radius;            //半径      
        public bool disappeared = false;           //表示是否已经消失
        public bool working = true;                //表示子弹是否进行工作（与disappeared不同）
        protected Entity() { }                     //定义一个空的构造函数用于子类写更符合的构造函数
        public virtual void Draw(Graphics g) { }   //由子类来完成draw的操作

        // 以下为不得已的方法，一般情况最好别这样

 
        public virtual void Update(float t)      //t表示经过的时间
        {
            float length = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);
            if (length == 0)
                return;
            direction.X /= length;
            direction.Y /= length;
            //以上把向量规范成了单位长度
            bounds.Offset(speed * direction.X * t, speed * direction.Y * t);
            //实现相应的移动
        }
        public virtual bool allBullets_out() { return true; }  //  主要是用于判断是否子弹全跑出（对于enemy和player而言）

        public virtual bool Collision(Player e)     // 判断碰撞
        {
            return false;
        }
        public virtual bool Collision(Enemy e)     // 判断碰撞
        {
            return false;
        }
    }
}
