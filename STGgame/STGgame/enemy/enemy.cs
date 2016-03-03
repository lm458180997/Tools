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
    public class Enemy : Entity                         //敌人
    {
        //图形工具
        protected const float PI = (float)Math.PI;      //指定一个PI便于计算

        //使魔属性
        public List<Enemy> My_Servitors= new List<Enemy>();                //敌人的使魔列表（部分敌人）
        protected List<Enemy> My_Servitors_toAdd = new List<Enemy>();       //敌人使魔的添加列表
        protected List<Enemy> My_Servitors_toremove = new List<Enemy>();    //敌人使魔的丢弃列表（把其中disappeared的使魔除去）

        public List<DamageEventArgs> My_Damages = new List<DamageEventArgs>();           //敌机的受攻击处理列表
        public List<DamageEventArgs> My_Damages_ToRemove = new List<DamageEventArgs>();  //攻击处理列表的丢弃列表

        public bool HaveServitors = false;              //是否拥有使魔（默认为无使魔）
        
        //冲击判定
        public bool Hited = false;                      //是否被击中 

        //统计时间
        protected float Time_cacullate = 0;            //Time_cacullate记录从出现开始到结束的时间，并相应的引起事件，形成自定义的敌人出场效果
                                
        //绘图工具                
        protected TextureBrush my_texturebrush;         //定义画出敌机的画刷 (目前画刷矩形都在使用，以后将全过渡为矩形)
        //绘制空间
        protected RectangleF myrect;                    //定义画出敌机的矩形

        //生命值
        protected float hp=100;                         //敌人的hp(默认为100)
        public float HP { get { return hp; } set { hp = value; } }

        //逻辑事件
        public override void Draw(Graphics g) { }     //绘制
        public override void Update(float t) { }      //逻辑更新
        public override bool Collision(Player e) { return false; }    //判断对于玩家的碰撞

        //fire事件
        protected Vector Fire_direction= new Vector(0,0);            //部分子弹会用到，子弹的初始化方向
        protected Vector Fire_direction1 = new Vector(0, 0);         //有些情况下可能会同时用到多种方向的子弹
        protected Vector Fire_direction2 = new Vector(0, 0);         //更会有发射用到三种方位的怪物。。。
        protected virtual void Fire() { }                            //敌机的开火（无方向参数）
        protected virtual void Fire(Vector v) { }                    //带有一个方向参数的开火方法
        protected virtual void Fire(Vector v1, Vector v2) { }        //带有两个方向参数的开火方法
        protected virtual void Fire(stg.bullet.Bullet_Name name) { } //若要指定发射什么子弹，则用此方法
        protected virtual void Fire(stg.bullet.Bullet_Name name, Vector v) { }//若要指定发射什么子弹，并且设定一个初始化的方向，则用此方法

        //执行指定次数的任务
        protected int eventcount;                                //纪录执行了的总次数
        protected virtual void AppointedEvent(int times) { }     //指定任务,提供一个次数

        protected virtual bool OutOfFrame()
        {
            if (this.coordinate.X < Game.Game_X - 100 || this.coordinate.X > Game.Game_X + Game.Game_Width + 100
                || this.coordinate.Y < Game.Game_Y - 100 || this.coordinate.Y > Game.Game_Y + Game.Game_Height + 100)
                return true;
            return false;
        }                   //离开屏幕
        public virtual Vector getdirection()                //返回子弹指向玩家的方向
        {
            Vector v = new Vector();
            v.X = (Game.player.coordinate.X - coordinate.X);
            v.Y = (Game.player.coordinate.Y - coordinate.Y);
            v.Normalize();                          //获取指向自机方向的向量
            return v;
        }
        protected virtual void Damage_process()
        {
            foreach (DamageEventArgs re in My_Damages_ToRemove)  //丢弃抛弃列表中的成员
                My_Damages.Remove(re);
            My_Damages_ToRemove.Clear();
            int i = My_Damages.Count;

            foreach (DamageEventArgs damage in My_Damages)       //击中时的处理
            {
                this.HP = this.HP - damage.Demage < 0 ? 0 : this.HP - damage.Demage;
                My_Damages_ToRemove.Add(damage);                 //处理结束后，就将其放入抛弃列表
            }
            if (HP == 0)
            {
                this.disappeared = true;
                foreach (Enemy e in My_Servitors)
                {
                    e.disappeared = true; e.working = false;
                }
                this.working = false;
            }
        }
    }
}
