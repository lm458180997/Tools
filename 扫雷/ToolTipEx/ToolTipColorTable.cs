using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace CSharpWin
{
   /* 作者：Starts_2000
    * 日期：2010-01-09
    * 网站：http://www.csharpwin.com CS 程序员之窗。
    * 你可以免费使用或修改以下代码，但请保留版权信息。
    * 具体请查看 CS程序员之窗开源协议（http://www.csharpwin.com/csol.html）。
    */

    public class ToolTipColorTable
    {
        private static readonly Color _base = Color.FromArgb(105, 200, 254);
        private static readonly Color _border = Color.FromArgb(204, 153, 51);
        private static readonly Color _backNormal = Color.FromArgb(250, 250, 250);
        private static readonly Color _backHover = Color.FromArgb(255, 180, 105);
        private static readonly Color _backPressed = Color.FromArgb(226, 176, 0);
        private static readonly Color _titleFore = Color.Brown;
        private static readonly Color _tipFore = Color.Chocolate;

        public ToolTipColorTable() { }

        public virtual Color Base
        {
            get { return _base; }
        }

        public virtual Color Border
        {
            get { return _border; }
        }

        public virtual Color BackNormal
        {
            get { return _backNormal; }
        }

        public virtual Color BackHover
        {
            get { return _backHover; }
        }

        public virtual Color BackPressed
        {
            get { return _backPressed; }
        }

        public virtual Color TitleFore
        {
            get { return _titleFore; }
        }

        public virtual Color TipFore
        {
            get { return _tipFore; }
        }
    }
}
