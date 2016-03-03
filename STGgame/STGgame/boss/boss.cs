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
    public class Boss : Entity
    {
        const float PI = (float)Math.PI;    //指定一个PI便于计算

        protected BossName name;                   //boss的名字
        public List<Bullet> Enemy_Bullets;         //boss的子弹集合
        protected List<Bullet> Bullets_toAdd;      //boss的子弹添加列表
        protected List<Bullet> Bullets_toremove;   //boss的将要除去的子弹集合
        protected List<Enemy> My_Servitors;        //boss的使魔列表（部分敌人）
        protected List<Enemy> My_Servitors_toAdd;
        protected List<Enemy> My_Servitors_toremove;//敌人使魔的丢弃列表（把其中disappeared的使魔除去）

        protected float Time_cacullate = 0;         //Time_cacullate记录从出现开始到结束的时间，并相应的引起事件
        protected float lastick =0;
        //形成自定义的敌人出场效果
        private TextureBrush my_texturebrush;      //定义画出敌机的


        public BossName Name
        {
            get { return name; }
            set 
            { 
                name = value;
                switch (name)
                {
                    case BossName.black_Syren:
                        break;
                }
            }
        }
        public Boss( BossName name)
        {
            
        }

        public override void Draw(System.Drawing.Graphics g)
        {
           
        }
        public override void Update(float t)
        {
        }

    }
}
