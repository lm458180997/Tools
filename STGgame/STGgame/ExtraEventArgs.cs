using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace stg
{
    public class UpdateEventArgs : EventArgs
    {
        private float delttime;
        public float Delttime { get { return delttime; } set { delttime = value; } }
        public UpdateEventArgs(float t)
        {
            Delttime = t;
        }
    }
    public class DamageEventArgs : EventArgs
    {
        private int demage;
        public int Demage { get { return demage; } set { demage = value; } }
        public DamageEventArgs(int Power)
        {
            Demage = Power;
        }
    }
}
