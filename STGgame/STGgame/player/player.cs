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
    public class Player : Entity
    {
        protected int level = 1;                   //人物等级
        protected bool flyslow = false;            //飞行属性，是否为慢速飞行

        public int Hited_count = 0;                //被击中的次数 

        public bool Hited = false;                 //表示是否被击中
        public float unmatchedtime = 3;            //无敌的时间间隔 

        public float fire_cd;                      //发射子弹的冷却时间
        protected bool allowfire = true;           //表示能否发射子弹
        protected float cd_calculate = 0;          //对cd进行计算，判断是否过了cd阶段，从而判断是否能引发新一次射击
        public List<Bullet> Player_Bullets;        //玩家的子弹集合
        public  List<Bullet> Bullet_toremove;      //将要移除的子弹的集合
        public TextureBrush my_texturebrush;       //用于绘制角色的画刷

        public Item Item_L;                        //左边道具
        public Item Item_R;                        //右边道具
             
        public Player(){}
        public virtual int Level
        {get { return level; }set { level = value; }}

        public Enemy Follow_enemy;

        public virtual bool FlySlow                //慢速飞行属性
        { get { return flyslow; }set{ flyslow = value;}}

        public override void Draw(Graphics g) { }        //根据不同的角色制定不同的绘画
        public override void Update(float t) { }      //根据不同的角色制定不同的游戏逻辑
        //开火
        public virtual void Fire() { }              //根据不同等级，创造不同的子弹与子弹数量
        
    }
}
