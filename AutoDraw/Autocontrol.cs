using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace AutoDraw
{
    [Serializable]
    public struct SavePos
    {
        //客户端总配置文件
        public string Black_x;                       //电脑名
        public string Black_y;                       //电脑名
        public string Red_x;                       //电脑名
        public string Red_y;                       //电脑名
        public string Green_x;                       //电脑名
        public string Green_y;                       //电脑名
        public string Blue_x;                       //电脑名
        public string Blue_y;                       //电脑名
        public string Yellow_x;                       //电脑名
        public string Yellow_y;                       //电脑名

        public int x_pixel;
        public int y_pixel;

        public SavePos(int type)
        {
            Black_x = "";
            Black_y = "";
            Red_x = "";
            Red_y = "";
            Green_x = "";
            Green_y = "";
            Blue_x = "";
            Blue_y = "";
            Yellow_x = "";
            Yellow_y = "";
            x_pixel = 100;
            y_pixel = 100;
        }

    }

    class Autocontrol
    {

    }

    class Device_control
    {
        public static int g_x_offset = 0;
        public static int g_y_offset = 0;


        const uint KEYEVENTF_EXTENDEDKEY = 0x1;
        const uint KEYEVENTF_KEYUP = 0x2;

        [System.Runtime.InteropServices.DllImport("user32")]
        static extern int GetKeyState(int nVirtKey);
        [System.Runtime.InteropServices.DllImport("user32")]
        static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);
        public enum VirtualKeys : byte
        {
            VK_NUMLOCK = 0x90, //数字锁定键
            VK_SCROLL = 0x91, //滚动锁定 
            VK_CAPITAL = 0x14, //大小写锁定
            VK_A = 65,           //A(10进制)
            VK_RETURN = 13,     //ENTER
            VK_ESCAPE = 27,      //传说中的救命ESC
            VK_Q = 81           //A(10进制)
        }
        public static bool GetState(VirtualKeys Key)
        {
            int tempkey = (GetKeyState((int)Key));
            return ((tempkey < 0)); //当检测到虚拟按键不为0时(似乎可能为0(未按下)或者-127(按下))
        }
        public static void SetState(VirtualKeys Key, bool State)
        {
            if (State != GetState(Key))
            {
                keybd_event((byte)Key, 0x45, KEYEVENTF_EXTENDEDKEY | 0, 0);
                keybd_event((byte)Key, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);//获取鼠标焦点
        [DllImport("user32.dll")]
        public static extern void SetCursorPos(int x, int y);//设置鼠标焦点

        public static void keyboard_test(byte keynum)
        {

            if ((keynum >= 0) && (keynum <= 9))
            {
                keybd_event((byte)(keynum + 48), 0, 0, 0);         //按下某个数字键
                                                                   //System.Threading.Thread.Sleep(20);
                keybd_event((byte)(keynum + 48), 0, 2, 0);        //放开那个数字键               
            }
            else
            {
                int ztest = keynum % 10;

                keybd_event((byte)(49), 0, 0, 0);
                keybd_event((byte)(49), 0, 2, 0);

                System.Threading.Thread.Sleep(200);

                keybd_event((byte)(ztest + 48), 0, 0, 0);
                keybd_event((byte)(ztest + 48), 0, 2, 0);

                return;
            }

        }

        public static void mouse_click_fast(Double[,] imgintsum, int[,] colors, SavePos poss)
        {
            Point curpoint = new Point(0, 0);

            System.Threading.Thread.Sleep(2000);

            int x_max = imgintsum.GetLength(0);
            int y_max = imgintsum.GetLength(1);
            int resolution_pixel = 2;
            Double curcolor=0;
            Double corxnearcolor = 0;
            Double corynearcolor = 0;
            Double xdiffer = 0;
            Double ydiffer = 0;

            int pixel_differ = 255;
            int pixel_differ_low = 85;

            int counter = 0;

            GetCursorPos(out curpoint);
            Point Initpoint = curpoint;

            ////首先画轮廓
            /////移到黑色
            SetCursorPos(Convert.ToInt32(poss.Black_x), Convert.ToInt32(poss.Black_y));
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0002, 0, 0, 1, 0);
            //System.Threading.Thread.Sleep(20);
            mouse_event(0x0004, 0, 0, 1, 0);

            while (counter<10000)
            {
                //回到初始点
                SetCursorPos(Initpoint.X, Initpoint.Y);
                for (int i = 0; i < x_max; i++)
                {
                    for (int j = 0; j < y_max; j++)
                    {
                        GetCursorPos(out curpoint);
                        SetCursorPos(curpoint.X + resolution_pixel, curpoint.Y);

                        curcolor = imgintsum[j, i];
                        if (j > 0 && i > 0)
                        {
                            corxnearcolor = imgintsum[j - 1, i];
                            corynearcolor = imgintsum[j, i - 1];
                            xdiffer = Math.Abs(curcolor - corxnearcolor);
                            ydiffer = Math.Abs(curcolor - corynearcolor);
                        }
                        else
                        {
                            xdiffer = 0;
                            ydiffer = 0;
                        }

                        //intdiffer > 35 && curcolor<40
                        if ((pixel_differ_low < xdiffer && xdiffer <= pixel_differ) ||
                            (pixel_differ_low < ydiffer && ydiffer <= pixel_differ))
                        {
                            mouse_event(0x0002, 0, 0, 1, 0);
                            //System.Threading.Thread.Sleep(20);
                            mouse_event(0x0004, 0, 0, 1, 0);
                            counter++;
                            GetCursorPos(out curpoint);
                            if (curpoint.X < (Initpoint.X - 10) || curpoint.Y < (Initpoint.Y - 10))
                            {
                                return;
                            }
                        }

                        if (counter > 10000)
                        {
                            break;
                        }
                        else if (counter==3000)
                        {
                            System.Threading.Thread.Sleep(2000);
                            counter++;
                        }

                    }


                    if (counter > 10000)
                    {
                        break;
                    }
                    GetCursorPos(out curpoint);
                    SetCursorPos(curpoint.X - x_max * resolution_pixel, curpoint.Y + resolution_pixel);

                }

                pixel_differ = pixel_differ_low;
                pixel_differ_low -= 5;

                if (pixel_differ_low <= 0)
                {
                    return;
                }
            }
 

            int curprintcolor = 0;
            for (int k = 1; k < 5; k++)
            {
                //移动到对应颜色取色
                if (k == 1)
                {
                    SetCursorPos(Convert.ToInt32(poss.Red_x), Convert.ToInt32(poss.Red_y));
                    System.Threading.Thread.Sleep(40);

                    //poss.Red_x
                }
                else if (k == 2)
                {
                    SetCursorPos(Convert.ToInt32(poss.Green_x), Convert.ToInt32(poss.Green_y));
                    System.Threading.Thread.Sleep(40);
                }
                else if (k == 3)
                {
                    SetCursorPos(Convert.ToInt32(poss.Blue_x), Convert.ToInt32(poss.Blue_y));
                    System.Threading.Thread.Sleep(40);
                }
                else if (k == 4)
                {
                    SetCursorPos(Convert.ToInt32(poss.Yellow_x), Convert.ToInt32(poss.Yellow_y));
                    System.Threading.Thread.Sleep(40);
                }
                mouse_event(0x0002, 0, 0, 1, 0);
                //System.Threading.Thread.Sleep(20);
                mouse_event(0x0004, 0, 0, 1, 0);


                //回到初始点
                SetCursorPos(Initpoint.X, Initpoint.Y);
                curprintcolor = k;
                //开始涂色逻辑
                for (int i = 0; i < x_max; i++)
                {
                    for (int j = 0; j < y_max; j++)
                    {
                        GetCursorPos(out curpoint);
                        SetCursorPos(curpoint.X + resolution_pixel, curpoint.Y);

                        curcolor = colors[j, i];

                        //intdiffer > 35 && curcolor<40
                        if (curcolor == curprintcolor)
                        {
                            mouse_event(0x0002, 0, 0, 1, 0);
                            //System.Threading.Thread.Sleep(20);
                            mouse_event(0x0004, 0, 0, 1, 0);
                            counter++;
                            GetCursorPos(out curpoint);
                            if (curpoint.X < (Initpoint.X - 10) || curpoint.Y < (Initpoint.Y - 10))
                            {
                                return;
                            }
                        }

                        if (counter > 10000)
                        {
                            break;
                        }

                    }
                    if (counter > 10000)
                    {
                        break;
                    }
                    GetCursorPos(out curpoint);
                    SetCursorPos(curpoint.X - x_max * resolution_pixel, curpoint.Y + resolution_pixel);

                }
            }

            counter++;
        }

        public static void mouse_autoinput1(int value)
        {
            //先直接找位置算了，等等再找相对位置
            //System.Threading.Thread.Sleep(1000);
            System.Threading.Thread.Sleep(20);
            SetCursorPos(1234 + g_x_offset, 390 + g_y_offset);    //自定义加价
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(20);

            SetCursorPos(1234 + g_x_offset, 390 + g_y_offset);    //自定义加价
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);

            System.Threading.Thread.Sleep(20);
            keybd_event(8, 0, 0, 0);         //按下backspace                                                              
            keybd_event(8, 0, 2, 0);        //放开那个数字键 

            System.Threading.Thread.Sleep(20);

            keyboard_test((byte)value);
            System.Threading.Thread.Sleep(20);
            keyboard_test(0);
            System.Threading.Thread.Sleep(20);
            keyboard_test(0);

            System.Threading.Thread.Sleep(60);
            SetCursorPos(1300 + g_x_offset, 385 + g_y_offset);    //自定义加价
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);

            System.Threading.Thread.Sleep(20);
            SetCursorPos(1306 + g_x_offset, 499 + g_y_offset);    //出价
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);

            System.Threading.Thread.Sleep(20);
            SetCursorPos(1233 + g_x_offset, 497 + g_y_offset);    //点到框上面
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);
        }
        public static void mouse_autoinput2()
        {
            //先直接找位置算了，等等再找相对位置
            System.Threading.Thread.Sleep(20);
            SetCursorPos(1053 + g_x_offset, 573 + g_y_offset);    //确定
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);

        }
        public static void mouse_autoinput3()
        {
            //点到刷新验证码
            System.Threading.Thread.Sleep(20);
            SetCursorPos(1053 + g_x_offset, 498 + g_y_offset);    //确定
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);

            System.Threading.Thread.Sleep(20);
            SetCursorPos(1233 + g_x_offset, 497 + g_y_offset);    //
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(40);
            mouse_event(0x0004, 0, 0, 1, 0);
        }
        public static void mouse_autoinput4()
        {
            //取消
            System.Threading.Thread.Sleep(20);
            SetCursorPos(1159 + g_x_offset, 562 + g_y_offset);
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(20);
            mouse_event(0x0004, 0, 0, 1, 0);
            //两个位置的取消
            System.Threading.Thread.Sleep(20);
            SetCursorPos(1207 + g_x_offset, 577 + g_y_offset);
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(20);
            mouse_event(0x0004, 0, 0, 1, 0);

        }
        public static void mouse_autoinput5()
        {
            //最终确认
            //System.Threading.Thread.Sleep(1000);
            System.Threading.Thread.Sleep(200);
            SetCursorPos(1234 + g_x_offset, 390 + g_y_offset);    //自定义加价

            SetCursorPos(1300 + g_x_offset, 385 + g_y_offset);    //自定义加价
            System.Threading.Thread.Sleep(200);
            SetCursorPos(1234 + g_x_offset, 390 + g_y_offset);    //自定义加价

            System.Threading.Thread.Sleep(200);

            SetCursorPos(1300 + g_x_offset, 385 + g_y_offset);    //自定义加价
            System.Threading.Thread.Sleep(200);
            SetCursorPos(1234 + g_x_offset, 390 + g_y_offset);    //自定义加价
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(200);
            mouse_event(0x0004, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(200);

            SetCursorPos(1234 + g_x_offset, 390 + g_y_offset);    //自定义加价
            mouse_event(0x0002, 0, 0, 1, 0);
            System.Threading.Thread.Sleep(200);
            mouse_event(0x0004, 0, 0, 1, 0);

            System.Threading.Thread.Sleep(200);
            keybd_event(8, 0, 0, 0);         //按下backspace        
            System.Threading.Thread.Sleep(200);
            keybd_event(8, 0, 2, 0);        //放开那个数字键 
            System.Threading.Thread.Sleep(200);
        }

        /// <summary>
        /// 已存在配置文件目录返回true 不存在则创建目录
        /// </summary>
        /// <returns></returns>
        public static bool SettingExist()
        {
            string curuserpath ="Settings\\";
            if (Directory.Exists(curuserpath))
            {
                return true;
            }
            else
            {
                //没有则新建setting
                System.IO.Directory.CreateDirectory(curuserpath);
                System.IO.Directory.CreateDirectory("CurCut\\");

                //List<string> initlist = new List<string>();
                SavePos zz = new SavePos(0);
                //Savesettings.Add(textBox10.Text);
                //initlist.Add("0");
                SaveSetting(zz);

                return false;
            }

        }

        public static SavePos LoadSetting()
        {
            string curpath = "Settings\\setting";

            SavePos test = new SavePos();
            try
            {
                test = (SavePos)LoadObj(curpath);

            }
            catch
            {
                //copy_setting();
                //test = (SavePos)LoadObj(curpath);
            }

            return test;
        }
        public static bool SaveSetting(SavePos SettingInput)
        {
            string curpath = "Settings\\setting";
            try
            {
                SaveObj(SettingInput, curpath);

            }
            catch
            {
                //写入失败
                return false;
            }

            SavePos test = new SavePos();
            try
            {
                test = (SavePos)LoadObj(curpath);
                //resave_setting();
                return true;
            }
            catch
            {

            }

            return false;
        }
        public static void SaveObj(object obj, string filename)   //序列化保存
        {
            try
            {
                FileStream fs = new FileStream(filename, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, obj);
                fs.Close();

            }
            catch (Exception)
            {

            }
        }
        public static object LoadObj(string filename)            //序列化读取文件
        {
            object Obj = new object();
            FileStream fs;
            try
            {
                fs = new FileStream(filename, FileMode.Open);
                try
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    Obj = bf.Deserialize(fs);
                }
                catch
                {

                }
                fs.Close();
            }
            catch (Exception)
            {

            }


            return Obj;
        }

    }
}
