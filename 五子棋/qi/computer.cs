using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace qi
{
     class Computer
    {
         private int[,] chessarray;
         private List<float> score= new List<float>();   //收集所有的分数，并进行排序和分析
         public float[,] myscorearray;                   //二维数组分别表示每一个点上的分数
         int selectactor = 1;                       //判断是哪种棋子，1代表黑棋，2代表白棋
         public delegate bool myjudge(int a, int b);
         public  myjudge[] jud3_1_4and9_12;
         public  myjudge[] jud3_5_8and13_16;
         public myjudge[] jud3_17_24;
         public myjudge[] jud4_all;
         public myjudge[] jud2_1_4;
         public myjudge[] jud2_5_8;
         public myjudge[] jud2_9_12;

         //电脑通过这些对比棋盘上的棋型来选取权重最高的点落子，你可以优化这些棋型的分值
         //棋型列表
         String[] cl = { "11111", "011110", "11110", "11101", "11011", "011100", "011010", "11100", "10110", "11010", "10101", "001100", "011000", "010100", "01100", "010010", "11000", "10100" };
         int[] cv = { 100000, 10000, 250, 280, 260, 310, 300, 55, 80, 55, 57, 65, 60, 45, 30, 20, 12, 12 };
	     //玩家对应棋型的权重
	     int[] pv = {90000, 9000, 250, 245, 260, 285, 280, 45, 70, 46, 49, 60, 50, 35, 20, 4, 4, 4};


         //转换二维索引为一维索引
         int two_to_one(int x, int y)
         {
             if (x < 0 || y < 0 || x > 13 || y > 13) return 196;
             return x * 14 + y;
         }

         //获取整数的符号
         int sign(int n)
         {
             return n > 0 ? 1 : n < 0 ? -1 : 0;
         }

         //获取一个棋子的某一个方向上的棋子坐标。方向:1-右；2-右下；3-下；4-左下。其它方向负数。
         int Get_otherchess(int i, int d)			//比如(3,4)的右边为(4,4)，不过本函数以一维坐标表示
         {
             int x = i / 14;
             int y = i % 14;
             switch (Math.Abs(d))
             {
                 case 1:
                     return two_to_one(x + sign(d), y);
                 case 2:
                     return two_to_one(x + sign(d), y + sign(d));
                 case 3:
                     return two_to_one(x, y + sign(d));
                 case 4:
                     return two_to_one(x - sign(d), y + sign(d));
                 default:
                     return two_to_one(x, y);
             }
         }

         //返回某点棋子
         int P(int x, int y)
         {
             if (x < 0 || y < 0 || x > 13 || y > 13)		//不合法的索引
                 return 3;
             else
                 return Form1.arr_2[x * 14 + y];
         }

         //返回某点棋子
         int P(int n)
         {
             return P(n / 14, n % 14);
         }

         //判断是己方棋子（1）还是障碍（2，对方棋子或棋盘外围）还是空位（0），a为待判断棋子，p为己方棋子。
         char ge(int a, int p)
         {
             if (a == p)
                 return '1';
             else if (a == -p)
                 return '2';
             else
             {
                 return (char)(a + 48);
             }
         }

         //计数一个落子处某个方向上的权重值。
         int CountV(int i, int d, int pl)     //   i为棋子的索引值，d为方向，pl为 对象
         {
             String str = "";				//用于保存棋型
             int tmp = i;
             for (int j = 1; j < 5; j++)
             {		//前4格
                 tmp = Get_otherchess(tmp, -d);
                char str1 = ge(P(tmp),pl);
                 str = ge(P(tmp), pl) + str;
                 if (P(tmp) == 2) break;		//遇到阻挡的就退出循环
             }
             str += '1';					//自身
             for (int j = 1; j < 5; j++)
             {		//后4格，总共保存该处某个方向上周围的9格，比如011110011，一次和预定义的棋型做对比
                 i = Get_otherchess(i, d);
                 str += ge(P(i), pl);
                 if (P(tmp) == 2) break;
             }
             for (int j = 0; j < 18; j++)
             {		//挨个比较棋型，藉此获得权重值
                 Regex r = new Regex(cl[j]);
                 String ts2 = str;
                 String ts3 = "";
                 foreach (char cha in ts2.ToCharArray())
                     ts3 = cha + ts3;
                 //直接比较或者反过来比较相等即可
                 if (r.IsMatch(ts2) || r.IsMatch(ts3))
                     return pl == 1 ? pv[j] : cv[j];
             }
             return 0;
         }

         //计算一个落子处的价值
         int GetValue(int i)
         {
             int n = 0;
             for (int j = 1; j < 5; j++)
             {                    //包括反方向
                 n += CountV(i, j, 1);		//对玩家的价值
                 n += CountV(i, j, -1);		//对电脑的价值
             }
             return n;				//返回总价值（这样是综合型的下法，可以修改玩家价值和电脑价值的比重
             //来达到进攻型下法或者防守型下法
         }
         int GetValue(int x, int y)
         {
             return GetValue((y - 1) * 14 + (x - 1));
         }


         public Computer(int[,] a, Form1.player p)
         {
             this.chessarray = a;
             myscorearray = new float[15, 15];

             //把多个方法用委托组成为方法数组，便于维护和管理
             jud2_1_4 = new myjudge[4] { jud2_1, jud2_2, jud2_3, jud2_4 };
             jud2_5_8 = new myjudge[4] { jud2_5, jud2_6, jud2_7, jud2_8 };
             jud2_9_12 = new myjudge[4] { jud2_9, jud2_10, jud2_11, jud2_12 };
             jud3_1_4and9_12 = new myjudge[8] { jud3_1, jud3_2, jud3_3, jud3_4, jud3_9, jud3_10, jud3_11, jud3_12 };
             jud3_5_8and13_16 = new myjudge[8] { jud3_5, jud3_6, jud3_7, jud3_8, jud3_13, jud3_14, jud3_15, jud3_16 };
             jud3_17_24 = new myjudge[8] { jud3_17, jud3_18, jud3_19, jud3_20, jud3_21, jud3_22, jud3_23, jud3_24 };
             jud4_all = new myjudge[20] { jud4_1, jud4_2, jud4_3, jud4_4, jud4_5, jud4_6, jud4_7, jud4_8,jud4_9,jud4_10,jud4_11,jud4_12,
                 jud4_13,jud4_14,jud4_15,jud4_16,jud4_17,jud4_18,jud4_19,jud4_20 };
         }
         void init()
         {
             for (int i = 1; i < 15; i++)
                 for (int j = 1; j < 15; j++)
                     myscorearray[i, j] = 0;
         }
        
        private void  getscore()    //获得每一个点的分数
        {          
            init();
            
            for (int i = 1; i < 15; i++)
            {
                for (int j = 1; j < 15; j++)
                {
                    if (chessarray[i, j] == 1 || chessarray[i, j] == -1)  // 当它为黑棋或者白棋时，它们的周围分别加上分数
                    {
                        //int x, y;
                        //x = i - 3; y = j - 3; if (x > 0 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 0.5f; }
                        //x = i - 2; y = j - 2; if (x > 0 && y > 0) { if (chessarray[x, y] == 0) myscorearray[x, y] += 1; }
                        //x = i - 1; y = j - 1; if (x > 0 && y > 0) { if (chessarray[x, y] == 0) myscorearray[x, y] += 1.5f; }
                        //x = i - 3; y = j; if (x > 0 && y > 0) { if (chessarray[x, y] == 0) myscorearray[x, y] += 0.5f; }
                        //x = i - 2; y = j; if (x > 0 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1; }
                        //x = i - 1; y = j; if (x > 0 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1.5f; }
                        //x = i - 3; y = j + 3; if (x > 0 && y < 15) { if (chessarray[x, y] == 0) myscorearray[x, y] += 0.5f; }
                        //x = i - 2; y = j + 2; if (x > 0 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1; }
                        //x = i - 1; y = j + 1; if (x > 0 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1.5f; }
                        //x = i; y = j + 3; if (x > 0 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 0.5f; }
                        //x = i; y = j + 2; if (x > 0 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1; }
                        //x = i; y = j + 1; if (x > 0 && y < 15) { if (chessarray[x, y] == 0) myscorearray[x, y] += 1.5f; }
                        //x = i; y = j - 3; if (x > 0 && y > 0) { if (chessarray[x, y] == 0) myscorearray[x, y] += 0.5f; }
                        //x = i; y = j - 2; if (x > 0 && y > 0) { if (chessarray[x, y] == 0) myscorearray[x, y] += 1; }
                        //x = i; y = j - 1; if (x > 0 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1.5f; }

                        //x = i + 3; y = j - 3; if (x < 15 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 0.5f; }
                        //x = i + 2; y = j - 2; if (x < 15 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1; }
                        //x = i + 1; y = j - 1; if (x < 15 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1.5f; }
                        //x = i + 3; y = j; if (x < 15 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 0.5f; }
                        //x = i + 2; y = j; if (x < 15 && y > 0) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1; }
                        //x = i + 1; y = j; if (x < 15 && y > 0) { if (chessarray[x, y] == 0) myscorearray[x, y] += 1.5f; }
                        //x = i + 3; y = j + 3; if (x < 15 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 0.5f; }
                        //x = i + 2; y = j + 2; if (x < 15 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1; }
                        //x = i + 1; y = j + 1; if (x < 15 && y < 15) { if (chessarray[x, y] == 0)myscorearray[x, y] += 1.5f; }
                    }

                    else
                    {
                        myscorearray[i, j] += GetValue(i,j);
                        selectactor = 1;                       //根据黑棋情况加分
                        for (int num = 0; num < 20; num++)
                        {
                            if (chessarray[i, j] == 0)
                            {
                                if (num < 8)
                                {                                          //  给 3 情形的加分
                                    if (jud3_1_4and9_12[num](i, j))        //对委托的方法每一个都运行，并通过判断返回值执行加分
                                        myscorearray[i, j] += 12;
                                    if (jud3_5_8and13_16[num](i, j))
                                        myscorearray[i, j] += 50;
                                    if(jud3_17_24[num](i,j))
                                        myscorearray[i, j] += 50;
                                }
                                if (num < 4)                              //   给 2 情形的加分
                                {
                                    if (jud2_1_4[num](i, j))
                                        myscorearray[i, j] += 7;
                                    if (jud2_5_8[num](i, j))
                                        myscorearray[i, j] += 7;
                                    if (jud2_9_12[num](i, j))
                                        myscorearray[i, j] += 12;
                                }
                                if (num < 20)                             //给 4 情形的加分
                                {
                                    if (chessarray[i, j] == 0)
                                    {
                                        if (jud4_all[num](i, j))
                                            myscorearray[i, j] += 200;
                                    }
                                }
                            }
                        }
                        selectactor = -1;                                   //根据白棋情况加分
                        for (int num = 0; num < 16; num++)
                        {
                            if (chessarray[i, j] == 0)
                            {
                                if (num < 8)
                                {                                          //  给 3 情形的加分
                                    if (jud3_1_4and9_12[num](i, j))        //对委托的方法每一个都运行，并通过判断返回值执行加分
                                        myscorearray[i, j] += 12;
                                    if (jud3_5_8and13_16[num](i, j))
                                        myscorearray[i, j] += 80;
                                    if (jud3_17_24[num](i, j))
                                        myscorearray[i, j] += 50;
                                }
                                if (num < 4)                              //   给 2 情形的加分
                                {
                                    if (jud2_1_4[num](i, j))
                                        myscorearray[i, j] += 8;
                                    if (jud2_5_8[num](i, j))
                                        myscorearray[i, j] += 8;
                                    if (jud2_9_12[num](i, j))
                                        myscorearray[i, j] += 12;
                                }
                                if (num < 20)                             //给 4 情形的加分
                                {
                                    if (chessarray[i, j] == 0)
                                    {
                                        if (jud4_all[num](i, j))
                                            myscorearray[i, j] += 1000;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public Point get_thebestselect() 
        {
            getscore();
            score.Clear();
            Point point= new Point(0,0);
            for (int i = 1; i < 15; i++)
            {
                for (int j = 1; j < 15; j++)
                {
                    score.Add(myscorearray[i, j]);
                }
            }
            for (int i = 0; i < score.Count - 1; i++)      //列表从大到小排序
            {
                for (int j = i + 1; j < score.Count; j++)
                {
                    if (score[i] < score[j])
                    {
                        float temp = score[i];
                        score[i] = score[j];
                        score[j] = temp;
                    }
                }
            }
            for (int i = 1; i < 15; i++)                   //遍历所有点，找到分数最高的点
            {
                for (int j = 1; j < 15; j++)
                {
                    if (myscorearray[i, j] == score[0])
                    {
                        point.X = i;
                        point.Y = j;
                    }
                }
            }
                return point; 
        }


        public int getx()   //获得电脑所选择的x坐标
        {
            return get_thebestselect().X;
        }
        public int gety()
        {
            return get_thebestselect().Y;
        }

        //以下为 2——4颗时的形状判定
        bool jud2_1(int x, int y)                          //无边界的加成
        {
            if (x - 3 < 1||x+1>14)
                return false;
            if (chessarray[x + 1, y] != 0)
                return false;
            if (chessarray[x - 3,y] != 0)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            return true;
        }
        bool jud2_2(int x, int y)
        {
            if (x + 3 > 14||x-1<1)
                return false;
            if (chessarray[x - 1, y] != 0)
                return false;
            if (chessarray[x + 3, y] != 0)
                return false;
            if (chessarray[x + 2, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            return true;
        }
        bool jud2_3(int x, int y)                          //无边界的加成
        {
            if (y - 3 < 1||y+1>14)
                return false;
            if (chessarray[x, y + 1] != 0)
                return false;
            if (chessarray[x, y - 3] != 0)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud2_4(int x, int y)
        {
            if (y + 3 > 14||y-1<1)
                return false;
            if (chessarray[x, y - 1] != 0)
                return false;
            if (chessarray[x, y + 3] != 0)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud2_5(int x, int y)
        {
            if (x - 3 < 1 || y + 3 > 14||x+1>14||y-1<1)
                return false;
            if (chessarray[x + 1, y - 1] != 0)
                return false;
            if (chessarray[x - 3, y + 3] != 0)
                return false;
            if (chessarray[x - 2, y + 2] != selectactor)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud2_6(int x, int y)
        {
            if (x + 3 > 14 || y - 3 < 1||x-1<1||y+1>14)
                return false;
            if (chessarray[x - 1, y + 1] != 0)
                return false;
            if (chessarray[x + 3, y - 3] != 0)
                return false;
            if (chessarray[x + 2, y - 2] != selectactor)
                return false;
            if (chessarray[x + 1, y - 2] != selectactor)
                return false;
            return false;
        }
        bool jud2_7(int x, int y)
        {
            if (x - 3 < 1 || y - 3 < 1||x+1>14||y+1>14)
                return false;
            if (chessarray[x + 1, y + 1] != 0)
                return false;
            if (chessarray[x - 3, y - 3] != 0)
                return false;
            if (chessarray[x - 2, y - 2] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud2_8(int x, int y)
        {
            if (x + 3 > 14 || y + 3 > 14||x-1<1||y-1<1)
                return false;
            if (chessarray[x - 1, y - 1] != 0)
                return false;
            if (chessarray[x + 3, y + 3] != 0)
                return false;
            if (chessarray[x + 2, y + 2] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud2_9(int x, int y)     //无边界的斜向  中位
        {
            if (x - 2 < 1 || x + 2 > 14 || y - 2 < 1 || y + 2 > 14)
                return false;
            if (chessarray[x - 2, y-2] != 0)
                return false;
            if (chessarray[x - 1, y-1] != selectactor)
                return false;
            if (chessarray[x + 1, y+1] != selectactor)
                return false;
            if (chessarray[x + 2, y + 2] != 0)
                return false;
            return true;
        }
        bool jud2_10(int x, int y)
        {
            if (x - 2 < 1 || y + 2 > 14 || x + 2 > 14 || y - 2 < 1)
                return false;
            if (chessarray[x - 2, y + 2] != 0)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 2, y - 2] != 0)
                return false;
            return true;
        }
        bool jud2_11(int x, int y)                  //和上面两个形式一样，只是换为了行和列
        {
            if (x - 2 < 1 || x + 2 > 14)
                return false;
            if (chessarray[x - 2, y] != 0)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != 0)
                return false;
            return true;
        }
        bool jud2_12(int x, int y)
        {
            if (y - 2 < 1 || y + 2 > 14)
                return false;
            if (chessarray[x, y - 2] != 0)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            if (chessarray[x, y + 2] != 0)
                return false;
            return true;
        }

        bool jud3_1(int x, int y)    //_ooox                 3_1 ——3_4为有边界，3_5——3_8为无边界
        {
            if (x + 4 > 14||x-1<1)
                return false;
            if (chessarray[x - 1, y] != 0)
                return false;
            if (chessarray[x + 3, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            if (chessarray[x + 4, y] == selectactor || chessarray[x + 4, y] == 0)
                return false;
            return true;
        }
        bool jud3_2(int x, int y)    //xooo_
        {
            if (x - 4 < 1||x+1>14)
                return false;
            if (chessarray[x + 1, y] != 0)
                return false;
            if (chessarray[x - 3, y] != selectactor)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x - 4, y] == selectactor || chessarray[x - 4, y] == 0)
                return false;
            return true;
        }
        bool jud3_3(int x, int y) 
        {
            if (y + 4 > 14||y-1<1)
                return false;
            if (chessarray[x, y - 1] != 0)
                return false;
            if (chessarray[x, y + 3] != selectactor)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            if (chessarray[x, y + 4] == selectactor || chessarray[x, y + 4] == 0)
                return false;
            return true;
        }
        bool jud3_4(int x, int y)    
        {
            if (y - 4 < 1||y+1>14)
                return false;
            if (chessarray[x, y + 1] != 0)
                return false;
            if (chessarray[x, y - 3] != selectactor)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y - 4] == selectactor || chessarray[x, y - 4] == 0)
                return false;
            return true;
        }
        bool jud3_5(int x, int y)    //_ooo
        {
            if (x + 4 > 14||x-1<1)
                return false;
            if (chessarray[x - 1, y] != 0)
                return false;
            if (chessarray[x + 3, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            if (chessarray[x + 4, y] !=0)
                return false;
            return true;
        }
        bool jud3_6(int x, int y)    //ooo_
        {
            if (x - 4 < 1||x+1>14)
                return false;
            if (chessarray[x + 1, y] != 0)
                return false;
            if (chessarray[x - 3, y] != selectactor)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x - 4, y] != 0)
                return false;
            return true;
        }
        bool jud3_7(int x, int y)    //
        {
            if (y + 4 > 14||y-1<1)
                return false;
            if (chessarray[x, y - 1] != 0)
                return false;
            if (chessarray[x, y + 3] != selectactor)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            if (chessarray[x, y + 4] != 0)
                return false;
            return true;
        }
        bool jud3_8(int x, int y)    //
        {
            if (y - 4 < 1||y+1>14)
                return false;
            if (chessarray[x, y + 1] != 0)
                return false;
            if (chessarray[x, y - 3] != selectactor)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y - 4] != 0)
                return false;
            return true;
        }
        bool jud3_9(int x, int y)    //_ooox                 
        {
            if (x + 4 > 14|| y-4<1||x-1<1||y+1>14)
                return false;
            if (chessarray[x - 1, y + 1] != 0)
                return false;
            if (chessarray[x + 3, y-3] != selectactor)
                return false;
            if (chessarray[x + 2, y-2] != selectactor)
                return false;
            if (chessarray[x + 1, y-1] != selectactor)
                return false;
            if (chessarray[x + 4, y-4] == selectactor || chessarray[x + 4, y] == 0)
                return false;
            return true;
        }
        bool jud3_10(int x, int y)    //xooo_
        {
            if (x - 4 < 1||y-4<1||x+1>14||y+1>14)
                return false;
            if (chessarray[x + 1, y -1] != 0)
                return false;
            if (chessarray[x - 3, y-3] != selectactor)
                return false;
            if (chessarray[x - 2, y-2] != selectactor)
                return false;
            if (chessarray[x - 1, y-1] != selectactor)
                return false;
            if (chessarray[x - 4, y-4] == selectactor || chessarray[x - 4, y] == 0)
                return false;
            return true;
        }
        bool jud3_11(int x, int y)
        {
            if (y + 4 > 14 || x+4>14||x-1<1||y-1<1)
                return false;
            if (chessarray[x - 1, y - 1] != 0)
                return false;
            if (chessarray[x+3, y + 3] != selectactor)
                return false;
            if (chessarray[x+2, y + 2] != selectactor)
                return false;
            if (chessarray[x+1, y + 1] != selectactor)
                return false;
            if (chessarray[x+4, y + 4] == selectactor || chessarray[x+4, y + 4] == 0)
                return false;
            return true;
        }
        bool jud3_12(int x, int y)
        {
            if (y + 4 > 14||x-4<1||x+1>14||y-1<1)
                return false;
            if (chessarray[x + 1, y - 1] != 0)
                return false;
            if (chessarray[x-3, y + 3] != selectactor)
                return false;
            if (chessarray[x-2, y + 2] != selectactor)
                return false;
            if (chessarray[x-1, y + 1] != selectactor)
                return false;
            if (chessarray[x-4, y + 4] == selectactor || chessarray[x-4, y + 4] == 0)
                return false;
            return true;
        }
        bool jud3_13(int x, int y)    //_ooox                 
        {
            if (x + 4 > 14 || y - 4 < 1 || x - 1 < 1 || y + 1 > 14)
                return false;
            if (chessarray[x - 1, y + 1] != 0)
                return false;
            if (chessarray[x + 3, y - 3] != selectactor)
                return false;
            if (chessarray[x + 2, y - 2] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 4, y - 4] != 0)
                return false;
            return true;
        }
        bool jud3_14(int x, int y)    //xooo_
        {
            if (x - 4 < 1 || y - 4 < 1 || x + 1 > 14 || y + 1 > 14)
                return false;
            if (chessarray[x + 1, y + 1] != 0)
                return false;
            if (chessarray[x - 3, y - 3] != selectactor)
                return false;
            if (chessarray[x - 2, y - 2] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            if (chessarray[x - 4, y - 4] != 0)
                return false;
            return true;
        }
        bool jud3_15(int x, int y)
        {
            if (y + 4 > 14 || x + 4 > 14 || x - 1 < 1 || y - 1 < 1)
                return false;
            if (chessarray[x - 1, y - 1] != 0)
                return false;
            if (chessarray[x + 3, y + 3] != selectactor)
                return false;
            if (chessarray[x + 2, y + 2] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 4, y + 4] != 0)
                return false;
            return true;
        }
        bool jud3_16(int x, int y)
        {
            if (y + 4 > 14 || x - 4 < 1 || x + 1 > 14 || y - 1 < 1)
                return false;
            if (chessarray[x + 1, y - 1] != 0)
                return false;
            if (chessarray[x - 3, y + 3] != selectactor)
                return false;
            if (chessarray[x - 2, y + 2] != selectactor)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            if (chessarray[x - 4, y + 4] != 0)
                return false;
            return true;
        }
       
        bool jud3_17(int x, int y)
        {
            if (x - 2 < 1 || x + 3 > 14)
                return false;
            if (chessarray[x - 2, y] != 0)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != selectactor)
                return false;
            if (chessarray[x + 3, y] != 0)
                return false;
            return true;
        }
        bool jud3_18(int x, int y)
        {
            if (x - 3 < 1 || x + 2 > 14)
                return false;
            if (chessarray[x - 3, y] != 0)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != 0)
                return false;
            return true;
        }
        bool jud3_19(int x, int y)
        {
            if (y - 2 < 1 || y + 3 > 14)
                return false;
            if (chessarray[x, y - 2] != 0)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 3] != 0)
                return false;
            return true;
        }
        bool jud3_20(int x, int y)
        {
            if (y - 3 < 1 || y + 2 > 14)
                return false;
            if (chessarray[x, y - 3] != 0)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            if (chessarray[x, y + 2] != 0)
                return false;
            return true;
        }
        bool jud3_21(int x, int y)
        {
            if (x + 3 > 14 || x - 2 < 1 || y + 2 > 14 || y - 3 < 1)
                return false;
            if (chessarray[x - 2, y + 2] != 0)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 2, y - 2] != selectactor)
                return false;
            if (chessarray[x + 3, y - 3] != 0)
                return false;
            return true;
        }
        bool jud3_22(int x, int y)
        {
            if (x - 3 < 1 || x + 2 > 14 || y + 3 > 14 || y - 2 < 1)
                return false;
            if (chessarray[x - 3, y + 3] != 0)
                return false;
            if (chessarray[x - 2, y + 2] != selectactor)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 2, y - 2] != 0)
                return false;
            return true;
        }
        bool jud3_23(int x, int y)
        {
            if (x - 3 < 1 || y - 3 < 1 || x + 2 > 14 || y + 2 > 14)
                return false;
            if (chessarray[x - 3, y - 3] != 0)
                return false;
            if (chessarray[x - 2, y - 2] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 2, y + 2] != 0)
                return false;
            return true;
        }
        bool jud3_24(int x, int y)
        {
            if (x - 2 < 1 || y - 2 < 1 || x + 3 > 14 || y + 3 > 14)
                return false;
            if (chessarray[x - 2, y - 2] != 0)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 3, y + 3] != 0)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 2, y + 2] != selectactor)
                return false;
            return true;
        }
        bool jud4_1(int x, int y)                           //      oooo_ 的情况
        {
            if (x - 4 < 1)
                return false;
            if (chessarray[x - 4, y] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x - 3, y] != selectactor)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            return true;
        }
        bool jud4_2(int x, int y)                           //  _oooo的情况
        {
            if (x + 4 >14)
                return false;
            if (chessarray[x + 4, y] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x + 3, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            return true;
        }
        bool jud4_3(int x, int y)                           //  
        {
            if (y + 4 > 14)
                return false;
            if (chessarray[x, y + 4] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x, y + 3] != selectactor)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_4(int x, int y)                           //  
        {
            if (y - 4 < 1)
                return false;
            if (chessarray[x, y - 4] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x, y - 3] != selectactor)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_5(int x, int y)                           //  
        {
            if (x + 4 >14|| y-4<1)
                return false;
            if (chessarray[x+4, y - 4] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x+3, y - 3] != selectactor)
                return false;
            if (chessarray[x+2, y - 2] != selectactor)
                return false;
            if (chessarray[x+1, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_6(int x, int y)                           //  
        {
            if (x - 4 <1 || y + 4 > 14)
                return false;
            if (chessarray[x - 4, y + 4] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x - 3, y + 3] != selectactor)
                return false;
            if (chessarray[x - 2, y + 2] != selectactor)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_7(int x, int y)                           //  
        {
            if (x - 4 < 1 || y - 4 < 1)
                return false;
            if (chessarray[x - 4, y - 4] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x - 3, y - 3] != selectactor)
                return false;
            if (chessarray[x - 2, y - 2] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_8(int x, int y)                           //  
        {
            if (x + 4 >14 || y + 4 >14)
                return false;
            if (chessarray[x + 4, y + 4] != selectactor)          //根据选择的棋子换不同的棋
                return false;
            if (chessarray[x + 3, y + 3] != selectactor)
                return false;
            if (chessarray[x + 2, y + 2] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_9(int x, int y)
        {
            if (x - 1 < 1 || x + 3 > 14 || y + 1 > 14 || y - 3 < 1)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 2, y - 2] != selectactor)
                return false;
            if (chessarray[x + 3, y - 3] != selectactor)
                return false;
            return true;
        }
        bool jud4_10(int x, int y)
        {
            if (x - 2 < 1 || x + 2 > 14 || y + 2 > 14 || y - 2 < 1)
                return false;
            if (chessarray[x - 2, y + 2] != selectactor)
                return false;
            if (chessarray[x -1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 2, y - 2] != selectactor)
                return false;
            return true;
        }
        bool jud4_11(int x, int y)
        {
            if (x - 3 < 1 || x + 1 > 14 || y + 3 > 14 || y - 1 < 1)
                return false;
            if (chessarray[x - 3, y + 3] != selectactor)
                return false;
            if (chessarray[x - 2, y + 2] != selectactor)
                return false;
            if (chessarray[x - 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 1, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_12(int x, int y)
        {
            if (x + 1 > 14 || x - 3 < 1 || y - 3 < 1 || y + 1 > 14)
                return false;
            if (chessarray[x - 3, y - 3] != selectactor)
                return false;
            if (chessarray[x - 2, y - 2] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_13(int x, int y)
        {
            if (x + 2 > 14 || y + 2 > 14 || x - 2 < 1 || y - 2 < 1)
                return false;
            if (chessarray[x + 2, y + 2] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            if (chessarray[x - 2, y - 2] != selectactor)
                return false;
            return true;
        }
        bool jud4_14(int x, int y)
        {
            if (x + 3 > 14 || y + 3 > 14 || x - 1 < 1 || y - 1 < 1)
                return false;
            if (chessarray[x + 2, y + 2] != selectactor)
                return false;
            if (chessarray[x + 1, y + 1] != selectactor)
                return false;
            if (chessarray[x + 3, y + 3] != selectactor)
                return false;
            if (chessarray[x - 1, y - 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_15(int x, int y)
        {
            if (x - 1 < 1 || x + 3 > 14)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            if(chessarray[x+2,y]!=selectactor)
                return false;
            if (chessarray[x + 3, y] != selectactor)
                return false;
            return true;
        }
        bool jud4_16(int x, int y)
        {
            if (x - 2 < 1 || x + 2 > 14)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x + 2, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            return true;
        }
        bool jud4_17(int x, int y)
        {
            if (x - 3 < 1 || x + 1 > 14)
                return false;
            if (chessarray[x - 2, y] != selectactor)
                return false;
            if (chessarray[x - 1, y] != selectactor)
                return false;
            if (chessarray[x - 3, y] != selectactor)
                return false;
            if (chessarray[x + 1, y] != selectactor)
                return false;
            return true;
        }
        bool jud4_18(int x, int y)
        {
            if (y - 1 < 1 || y + 3 > 14)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 3] != selectactor)
                return false;
            return true;
        }
        bool jud4_19(int x, int y)
        {
            if (y - 2 < 1 || y + 2 > 14)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y + 2] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            return true;
        }
        bool jud4_20(int x, int y)
        {
            if (y - 3 < 1 || y + 1 > 14)
                return false;
            if (chessarray[x, y - 2] != selectactor)
                return false;
            if (chessarray[x, y - 1] != selectactor)
                return false;
            if (chessarray[x, y - 3] != selectactor)
                return false;
            if (chessarray[x, y + 1] != selectactor)
                return false;
            return true;
        }
        

    }
}
