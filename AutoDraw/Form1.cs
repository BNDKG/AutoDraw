using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AutoDraw
{
    public partial class Form1 : Form
    {
        SavePos curz = new SavePos();

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //timer1.Stop();
            //相对路径，和程序exe同目录下
            String Pathcut = "";
            try
            {
                DirectoryInfo dir = new DirectoryInfo(@"CurCut");
                FileInfo[] fileInfo = dir.GetFiles();
                List<string> fileNames = new List<string>();
                String finalcut = "";
                foreach (FileInfo item in fileInfo)
                {
                    fileNames.Add(item.Name);

                    finalcut = item.Name;
                }
                Pathcut = @"CurCut\" + finalcut;
            }
            catch
            {
                label1.Text = "请先截图";
                return;
            }

            Bitmap myBitmap = new Bitmap(Pathcut);

            Bitmap myBitmap2 = new Bitmap(myBitmap, curz.x_pixel, curz.x_pixel);
            myBitmap.Dispose();

            Double[,] gray= GetImagePixel_change(myBitmap2);
            int[,] colors = GetImagePixel_changecolors(myBitmap2);

            int zfe = 1;
            zfe++;

            curz.Red_x = textBox1.Text;
            curz.Red_y = textBox2.Text;
            curz.Green_x = textBox3.Text;
            curz.Green_y = textBox4.Text;
            curz.Blue_x = textBox5.Text;
            curz.Blue_y = textBox6.Text;
            curz.Yellow_x = textBox7.Text;
            curz.Yellow_y = textBox8.Text;
            curz.Black_x = textBox9.Text;
            curz.Black_y = textBox10.Text;

            Device_control.mouse_click_fast(gray,colors, curz);

            //if (System.IO.File.Exists(Pathcut))
            //{
            //    System.IO.File.Delete(Pathcut);
            //}
            //timer1.Start();
        }
        public static byte[] GetImagePixel(Bitmap img)
        {
            byte[] result = new byte[img.Width * img.Height * 3];
            int n = 0;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    result[n] = img.GetPixel(i, j).R;
                    result[n + 1] = img.GetPixel(i, j).G;
                    result[n + 2] = img.GetPixel(i, j).B;
                    n += 3;
                }
            }
            return result;
        }

        public static Double[,] GetImagePixel_change(Bitmap img)
        {
            //byte[] result = new byte[img.Width * img.Height * 3];
            Double[,] result= new Double[img.Width, img.Height];

            int n = 0;
            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    //result[i,j] = (Double)img.GetPixel(i, j).R*0.3+ img.GetPixel(i, j).G*0.59+ img.GetPixel(i, j).B*0.11;
                    result[i, j] = (Double)img.GetPixel(i, j).R * 0.3 + img.GetPixel(i, j).G * 0.3 + img.GetPixel(i, j).B * 0.3;
                }
            }
            return result;
        }

        public static int[,] GetImagePixel_changecolors(Bitmap img)
        {
            //byte[] result = new byte[img.Width * img.Height * 3];
            int[,] result = new int[img.Width, img.Height];

            int n = 0;
            int RR = 0;
            int GG = 0;
            int BB = 0;

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    result[i, j] = 0;
                    RR = img.GetPixel(i, j).R;
                    GG = img.GetPixel(i, j).G;
                    BB = img.GetPixel(i, j).B;


                    if (RR > 150 && GG < 100 && BB< 100)
                    {
                        //红色 200 50 50
                        result[i, j] = 1;
                    }
                    else if (RR < 120 && GG > 120 && BB < 120)
                    {
                        //绿色 75 200 75
                        result[i, j] = 2;
                    }
                    else if (RR < 100 && GG < 100 && BB > 160)
                    {
                        //蓝色 200 200 75
                        result[i, j] = 3;
                    }
                    else if (RR > 160 && GG > 160 && BB < 90)
                    {
                        //黄色 200 200 75
                        result[i, j] = 4;
                    }

                }
            }
            return result;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Point curpointz = new Point(0, 0);
            Device_control.GetCursorPos(out curpointz);

            label9.Text = curpointz.X.ToString();
            label10.Text = curpointz.Y.ToString();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //读取配置
            if (Device_control.SettingExist())
            {
                //已有配置
            }
            else
            {
                //初次配置
                //SetMeAutoStart(true);
                //Form3 frm3 = new Form3();
                //frm3.ShowDialog();  
                
            }

            curz = Device_control.LoadSetting();
            //timer1.Start();

            //todo添加判断计算机名重复

            textBox1.Text = curz.Red_x;
            textBox2.Text = curz.Red_y;
            textBox3.Text = curz.Green_x;
            textBox4.Text = curz.Green_y;
            textBox5.Text = curz.Blue_x;
            textBox6.Text = curz.Blue_y;
            textBox7.Text = curz.Yellow_x;
            textBox8.Text = curz.Yellow_y;
            textBox9.Text = curz.Black_x;
            textBox10.Text = curz.Black_y;

            textBox11.Text = curz.x_pixel.ToString();
            //textBox12.Text = curz.y_pixel.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);

            Point curpointz = new Point(0, 0);
            Device_control.GetCursorPos(out curpointz);

            textBox1.Text = curpointz.X.ToString();
            textBox2.Text = curpointz.Y.ToString();
            saveforms();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);

            Point curpointz = new Point(0, 0);
            Device_control.GetCursorPos(out curpointz);

            textBox3.Text = curpointz.X.ToString();
            textBox4.Text = curpointz.Y.ToString();
            saveforms();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);

            Point curpointz = new Point(0, 0);
            Device_control.GetCursorPos(out curpointz);

            textBox5.Text = curpointz.X.ToString();
            textBox6.Text = curpointz.Y.ToString();
            saveforms();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);

            Point curpointz = new Point(0, 0);
            Device_control.GetCursorPos(out curpointz);

            textBox7.Text = curpointz.X.ToString();
            textBox8.Text = curpointz.Y.ToString();
            saveforms();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            saveforms();
        }
        private void saveforms()
        {
            curz.Red_x = textBox1.Text;
            curz.Red_y = textBox2.Text;
            curz.Green_x = textBox3.Text;
            curz.Green_y= textBox4.Text;
            curz.Blue_x = textBox5.Text;
            curz.Blue_y = textBox6.Text;
            curz.Yellow_x = textBox7.Text;
            curz.Yellow_y = textBox8.Text;
            curz.Black_x = textBox9.Text;
            curz.Black_y = textBox10.Text;

            curz.x_pixel = Convert.ToInt32(textBox11.Text);
            curz.y_pixel = Convert.ToInt32(textBox11.Text);


            Device_control.SaveSetting(curz);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            System.Threading.Thread.Sleep(2000);

            Point curpointz = new Point(0, 0);
            Device_control.GetCursorPos(out curpointz);

            textBox9.Text = curpointz.X.ToString();
            textBox10.Text = curpointz.Y.ToString();
            saveforms();
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveforms();
        }
    }
}
