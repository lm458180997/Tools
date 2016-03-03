using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg
{
    public abstract class Stage
    {
        //背景图层
        protected TextureBrush backgrounda ;
        protected TextureBrush backgrounde;
        protected TextureBrush backgroundf;
        protected float Offset_ay;                //偏移量
        protected float TotalOffset_ay;
        protected float Offset_ey;                //偏移量
        protected float TotalOffset_ey;
        protected float Offset_fy;                //偏移量
        protected float TotalOffset_fy;
        protected float Offset_ax;                //偏移量
        protected float TotalOffset_ax;
        protected float Offset_ex;                //偏移量
        protected float TotalOffset_ex;
        protected float Offset_fx;                //偏移量
        protected float TotalOffset_fx;
        //计时
        protected float Time_Caculate;        
        protected float lastick1;
        public TextureBrush Backgrounda
        {
            get { return backgrounda; }
            set { backgrounda = value; }
        }
        public TextureBrush Backgrounde
        {
            get { return backgrounde; }
            set { backgrounde = value; }
        }
        public TextureBrush Backgroundf
        {
            get { return backgroundf; }
            set { backgroundf = value; }
        }

        public virtual void Update(float t)  {  }
        public virtual void Draw(Graphics g) {  }

    }
}
