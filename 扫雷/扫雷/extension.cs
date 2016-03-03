using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace 扫雷
{
    public static class GraphicsExtensions
    {
        /// <summary>
        /// 通过指定纹理画刷填充指定矩形
        /// </summary>
        /// <param name="g">绘图对象</param>
        /// <param name="tb">用于绘制的纹理画刷</param>
        /// <param name="x">矩形的x位置</param>
        /// <param name="y">矩形的y位置</param>
        /// <param name="w">矩形的宽度</param>
        /// <param name="h">矩形的高度</param>
        /// <param name="tileX">水平方向上平铺的数量，0为自动</param>
        /// <param name="tileY">竖直方向上平铺的数量，0为自动</param>
        /// <param name="offX">水平方向的平移</param>
        /// <param name="offY">竖直方向的平移</param>
        /// <param name="rotate">旋转角度</param>
        public static void DrawTile(this Graphics g, TextureBrush tb,
            float x, float y, float w, float h, float tileX = 0, float tileY = 0,
            float offX = 0, float offY = 0, float rotate = 0)
        {
            tb.TranslateTransform((x % w) + offX, (y % h) + offY);
            float sx, sy;
            if (tileX <= 0 && tileY <= 0)
                sx = sy = 1;
            else
            {
                sx = w / tileX / tb.Image.Width;
                sy = h / tileY / tb.Image.Height;
                if (tileX <= 0)
                    sx = sy;
                else if (tileY <= 0)
                    sy = sx;
            }
            tb.RotateTransform(rotate);
            tb.ScaleTransform(sx, sy);
            g.FillRectangle(tb, new RectangleF(x, y, w, h));
            tb.ResetTransform();
        }

        /// <summary>
        /// 通过指定纹理画刷填充指定矩形
        /// </summary>
        /// <param name="g">绘图对象</param>
        /// <param name="tb">用于绘制的纹理画刷</param>
        /// <param name="rect">将要绘制的矩形区域</param>
        /// <param name="tileX">水平方向上平铺的数量，0为自动</param>
        /// <param name="tileY">竖直方向上平铺的数量，0为自动</param>
        /// <param name="offX">水平方向的平移</param>
        /// <param name="offY">竖直方向的平移</param>
        /// <param name="rotate">旋转角度</param>
        public static void DrawTile(this Graphics g, TextureBrush tb,
            RectangleF rect, float tileX = 0, float tileY = 0,
            float offX = 0, float offY = 0, float rotate = 0)
        {
            DrawTile(g, tb, rect.Left, rect.Top, rect.Width,
                rect.Height, tileX, tileY, offX, offY, rotate);
        }

        /// <summary>
        /// 根据指定的中心进行旋转变换
        /// </summary>
        /// <param name="g">进行变换的Graphics对象</param>
        /// <param name="angle">旋转角度</param>
        /// <param name="x">中心的x坐标</param>
        /// <param name="y">中心的y坐标</param>
        public static void RotateAt(this Graphics g, float angle, float x = 0, float y = 0)
        {
            g.TranslateTransform(x, y);
            g.RotateTransform(angle);
            g.TranslateTransform(-x, -y);
        }
    }
    public static class ImageExtensions
    {
        /// <summary>
        /// 设置图形边缘半透明
        /// </summary>
        /// <param name="p_Bitmap">图形</param>
        /// <param name="p_CentralTransparent">true中心透明 false边缘透明</param>
        /// <param name="p_Crossdirection">true横 false纵</param>
        /// <returns></returns>
        public static Bitmap BothAlpha(Bitmap p_Bitmap, bool p_CentralTransparent, bool p_Crossdirection)
        {
            Bitmap _SetBitmap = new Bitmap(p_Bitmap.Width, p_Bitmap.Height);
            Graphics _GraphisSetBitmap = Graphics.FromImage(_SetBitmap);
            _GraphisSetBitmap.DrawImage(p_Bitmap, new Rectangle(0, 0, p_Bitmap.Width, p_Bitmap.Height));
            _GraphisSetBitmap.Dispose();
            Bitmap _Bitmap = new Bitmap(_SetBitmap.Width, _SetBitmap.Height);
            Graphics _Graphcis = Graphics.FromImage(_Bitmap);
            Point _Left1 = new Point(0, 0);
            Point _Left2 = new Point(_Bitmap.Width, 0);
            Point _Left3 = new Point(_Bitmap.Width, _Bitmap.Height / 2);
            Point _Left4 = new Point(0, _Bitmap.Height / 2);
            if (p_Crossdirection)
            {
                _Left1 = new Point(0, 0);
                _Left2 = new Point(_Bitmap.Width / 2, 0);
                _Left3 = new Point(_Bitmap.Width / 2, _Bitmap.Height);
                _Left4 = new Point(0, _Bitmap.Height);
            }
            Point[] _Point = new Point[] { _Left1, _Left2, _Left3, _Left4 };
            PathGradientBrush _SetBruhs = new PathGradientBrush(_Point, WrapMode.TileFlipY);
            _SetBruhs.CenterPoint = new PointF(0, 0);
            _SetBruhs.FocusScales = new PointF(_Bitmap.Width / 2, 0);
            _SetBruhs.CenterColor = Color.FromArgb(0, 255, 255, 255);
            _SetBruhs.SurroundColors = new Color[] { Color.FromArgb(255, 255, 255, 255) };
            if (p_Crossdirection)
            {
                _SetBruhs.FocusScales = new PointF(0, _Bitmap.Height);
                _SetBruhs.WrapMode = WrapMode.TileFlipX;
            }
            if (p_CentralTransparent)
            {
                _SetBruhs.CenterColor = Color.FromArgb(255, 255, 255, 255);
                _SetBruhs.SurroundColors = new Color[] { Color.FromArgb(0, 255, 255, 255) };
            }
            _Graphcis.FillRectangle(_SetBruhs, new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height));
            _Graphcis.Dispose();
            BitmapData _NewData = _Bitmap.LockBits(new Rectangle(0, 0, _Bitmap.Width, _Bitmap.Height), ImageLockMode.ReadOnly, _Bitmap.PixelFormat);
            byte[] _NewBytes = new byte[_NewData.Stride * _NewData.Height];
            Marshal.Copy(_NewData.Scan0, _NewBytes, 0, _NewBytes.Length);
            _Bitmap.UnlockBits(_NewData);
            BitmapData _SetData = _SetBitmap.LockBits(new Rectangle(0, 0, _SetBitmap.Width, _SetBitmap.Height), ImageLockMode.ReadWrite, _SetBitmap.PixelFormat);
            byte[] _SetBytes = new byte[_SetData.Stride * _SetData.Height];
            Marshal.Copy(_SetData.Scan0, _SetBytes, 0, _SetBytes.Length);
            int _WriteIndex = 0;
            for (int i = 0; i != _SetData.Height; i++)
            {
                _WriteIndex = i * _SetData.Stride + 3;
                for (int z = 0; z != _SetData.Width; z++)
                {
                    _SetBytes[_WriteIndex] = _NewBytes[_WriteIndex];
                    _WriteIndex += 4;
                }
            }
            Marshal.Copy(_SetBytes, 0, _SetData.Scan0, _SetBytes.Length);
            _SetBitmap.UnlockBits(_SetData);
            return _SetBitmap;
        }
    }
}
