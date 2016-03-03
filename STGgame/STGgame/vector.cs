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
    public class Vector      //表示方向的一个类
    {
        private float x, y;
        public float X       //方向属性用X,Y来表示
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        public Vector() : this(0, 0) { }
        public Vector(float x, float y)  //float精度
        {
            this.x = x;
            this.y = y;
        }
        public Vector(float x)           //用角度来获得方向,正右方向，顺时针旋转 起
        {
            this.x = (float)Math.Acos(x);
            this.y = (float)Math.Asin(x);
        }
        public Vector(double x, double y)//double精度
        {
            this.x = (float)x;
            this.y = (float)y;
        }
        public Vector(PointF p)            //点的构造方法
        {
            this.x = p.X;
            this.y = p.Y;
        }
        public PointF ToPointF()           //提供一个可返回方向内容的点
        {
            return new PointF(x, y);
        }
        public float ToRadian()            //返回其方向的弧度数
        {
            if(x!=0)
            return (float)Math.Atan2(this.y, this.x)*180/(float)Math.PI;
            else if (y == -1)
                return 1.5f * (float)Math.PI;
            else if (y == 1)
                return 0.5f * (float)Math.PI;
            else
                return 0;             
        }
        public void rotate(float a)               //根据角度，方向进行旋转，顺时针起
        {  
            float x =this.x;
            float y = this.y;
            this.x = (float)(x * Math.Cos(a / 180 * Math.PI) - y * Math.Sin(a / 180 * Math.PI));         //根据新角度重新定义其向量
            this.y = (float)(x * Math.Sin(a / 180 * Math.PI) + y * Math.Cos(a / 180 * Math.PI));
            Normalize();
        }
        //operator的用法
        //打个比方说，你知道1+1=2，电脑也知道1+1=2，这个时候，+号就不需要重载。
        //但你知道（x+y)*(x-y),电脑是不会明白这种复杂的计算的，所以这个时候，*号就需要重载。
        //重载的目的，就是告诉电脑，(x+y)*(x-y)的算法是：x*x+x*y-x*y-y*y。
        //也就是*号在这个运算中的所有用处都化成电脑明白的运算方式。
        //其实也就是自定义的符号运算 ， 例下面就是自定义v1+v2的 + 运算
        public static Vector operator +(Vector v1, Vector v2)  //向量叠加
        {
            return new Vector(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector operator -(Vector v1, Vector v2)  //向量叠减
        {
            return new Vector(v1.x - v2.x, v1.y - v2.y);
        }
        public static Vector operator *(Vector v1, Vector v2)  //向量相乘
        {
            return new Vector(v1.x * v2.x, v1.y * v2.y);
        }
        public static Vector operator *(Vector v1, float s)    //向量数乘
        {
            return new Vector(v1.x * s, v1.y * s);
        }
        //public static bool operator ==(Vector v1, Vector v2)   //相等判断
        //{
        //    return v1.x == v2.y && v1.y == v2.y;
        //}
        //public static bool operator !=(Vector v1, Vector v2)   //不等判断
        //{
        //    return !(v1 == v2);
        //}
        public float Length()                                  //返回长度
        {
            return (float)Math.Sqrt(x * x + y * y);
        }
        public Vector Cross(Vector v2)
        {
            return new Vector();                   //清空方向
        }
        public void Normalize()     //将方向数值单位化
        {
            if (Length() == 0)      //排出意外情况
                return;
            float l = Length();
            x /= l;
            y /= l;
        }
        public float getcurve()     //获得方向的角度度(仅对于敌人)
        {
            return (float)(Math.Atan2(y, x) * 180 / Math.PI) - 90;
        }
        public Vector opposite()
        {
            return new Vector(-x, -y);
        }
        public Vector GetNormalize()       // 返回一个标准化的矢量
        {
            Normalize();
            return this;
        }
    }
}
