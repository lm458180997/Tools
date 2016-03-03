using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;


namespace stg
{
    public static class Utils
    {
        #region Round Rectangle
        static Random random = new Random();
        private static GraphicsPath GetRoundRect(float x, float y, float w, float h, float radius)
        {
            if (w < 0 || h < 0) return null;        //长宽不符合条件
            if (radius < 0) radius = 0;
            GraphicsPath p = new GraphicsPath();
            if (radius == 0)                        //半径为0则直添加矩形
                p.AddRectangle(new RectangleF(x, y, w, h));
            else
            {
                radius = Math.Min(Math.Min(w, h) / 2, radius);  //防止直径比长宽还长
                p.AddArc(new RectangleF(x, y, radius * 2, radius * 2), 180, 90);
                p.AddArc(new RectangleF(x + w - radius * 2, y, radius * 2, radius * 2), 270, 90);
                p.AddArc(new RectangleF(x + w - radius * 2, y + h - radius * 2, radius * 2, radius * 2), 0, 90);
                p.AddArc(new RectangleF(x, y + h - radius * 2, radius * 2, radius * 2), 90, 90);
                p.CloseAllFigures();     //把上面的每条弧线都连接起来
            }
            return p;
        }
        public static void DrawRoundRect( Graphics g, Pen pen, RectangleF r, float radius)
        {
            DrawRoundRect(g, pen, r.X, r.Y, r.Width, r.Height, radius);
        }
        public static void DrawRoundRect( Graphics g, Pen pen, float x, float y, float w, float h, float radius)
        {
            g.DrawPath(pen, GetRoundRect(x, y, w, h, radius));
        }
        public static void FillRoundRect( Graphics g, Brush brush, RectangleF r, float radius)
        {
            FillRoundRect(g, brush, r.X, r.Y, r.Width, r.Height, radius);
        }
        public static void FillRoundRect(Graphics g, Brush brush, float x, float y, float w, float h, float radius)
        {
            g.FillPath(brush, GetRoundRect(x, y, w, h, radius));
        }
        public static int RandomInt(int min = 0, int max = int.MaxValue) // 获得两个数值间的随机数
        {
            return random.Next(min, max);
        }
        #endregion
        public static double Getdistance( float x ,float y)             //获得坐标与参数点间的距离
        {
            return Math.Sqrt((Game.player.coordinate.X-x)*(Game.player.coordinate.X-x)+
                (Game.player.coordinate.Y-y)*(Game.player.coordinate.Y-y));
        }
    }
}
