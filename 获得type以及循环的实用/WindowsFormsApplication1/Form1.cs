using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
                var name = "Bugs Bunny";       /*var是预判断类型,会根据情形成为相应格式*/
                var age = 25;
                var isRabbit = true;
                Type nameType = name.GetType();
                Type ageType = age.GetType();
                Type isRabvitType = isRabbit.GetType();
                label1.Text = "name is type" + nameType.ToString();
            /*是获得了nameType的格式，并将它转换为字符串型 */
                label2.Text = "age is type" + ageType.ToString();
                label3.Text="isRabbit is type" + isRabvitType.ToString();
                label4.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for( int i =0;i<10;i++)
            {
                label4.Text += i;
            }
            for (int i = 10; i >= 0; i--)
            { label4.Text += i; }
        }
    }
}
