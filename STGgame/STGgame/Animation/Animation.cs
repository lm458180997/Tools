using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


namespace stg.Animation
{
    public abstract class Animation
    {
        public static List<Animation> Fore_Animations = new List<Animation>();
        public static List<Animation> Fore_toAdd = new List<Animation>();
        public static List<Animation> Fore_toRemove = new List<Animation>();

        public static List<Animation> Back_Animations = new List<Animation>();
        public static List<Animation> Back_toAdd = new List<Animation>();
        public static List<Animation> Back_toRemove = new List<Animation>();

        public static List<Animation> Act_Animations = new List<Animation>();
        public static List<Animation> Act_toAdd = new List<Animation>();
        public static List<Animation> Act_toRemove = new List<Animation>();

        public bool working = true;           //是否在正常工作中

        public void Fore_Run()
        {
            Fore_toAdd.Add(this);
        }
        public void Back_Run()
        {
            Back_toAdd.Add(this);
        }
        public void Act_Run()
        {
            Act_toAdd.Add(this);
        }
        public static void Check()
        {
            foreach (Animation a in Act_Animations)
                if (!a.working)
                    Act_toRemove.Add(a);
            foreach (Animation a in Fore_Animations)
                if (!a.working)
                    Fore_toRemove.Add(a);
            foreach (Animation a in Back_Animations)
                if (!a.working)
                    Back_toRemove.Add(a);
        }

        //对所有的动画进行逻辑更新
        public static void _Update(UpdateEventArgs e)      
        {
            foreach (Animation a in Animation.Act_toAdd)
                Animation.Act_Animations.Add(a);
            foreach (Animation a in Animation.Fore_toAdd)
                Animation.Fore_Animations.Add(a);
            foreach (Animation a in Animation.Back_toAdd)
                Animation.Back_Animations.Add(a);
            Animation.Act_toAdd.Clear();
            Animation.Fore_toAdd.Clear();
            Animation.Back_toAdd.Clear();

            foreach (Animation a in Animation.Act_toRemove)
                Animation.Act_Animations.Remove(a);
            foreach (Animation a in Animation.Fore_toRemove)
                Animation.Fore_Animations.Remove(a);
            foreach (Animation a in Animation.Back_toRemove)
                Animation.Back_Animations.Remove(a);
            Animation.Back_toRemove.Clear();
            Animation.Act_toRemove.Clear();
            Animation.Fore_toRemove.Clear();

            foreach (Animation a in Animation.Act_Animations)
                a.Update(e);
            foreach (Animation a in Animation.Back_Animations)
                a.Update(e);
            foreach (Animation a in Animation.Fore_Animations)
                a.Update(e);
        }
        public virtual void Update(UpdateEventArgs e) 
        { }
        
        public virtual void Draw(Graphics g)
        { }
    }
}
