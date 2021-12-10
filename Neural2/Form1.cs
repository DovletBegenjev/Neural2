using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neural2
{
    public partial class Form1 : Form
    {
        int imgCount = 0, nj = 0;
        static int NN = 3; // number of images
        int[,] w;
        int[][] S; // array of images in binary form
        char[] imgName = new char[NN]; // array of letters, that defines what letter was on image
        int[] SumOfNeyrons = new int[NN]; // array of neyrons summary for each letter, used to define letters
        public int[] signal1 = new int[NN]; // array of signals of each letter

        public sbyte[] ConvertToByte(Bitmap img)
        {
            int i = 0, j = 0;
            int N = img.Height * img.Width;
            sbyte[] texto = new sbyte[N];
            for (i = 0; i < img.Height; ++i)
            {
                for (j = 0; j < img.Width; ++j)
                {
                    if (img.GetPixel(j, i).A.ToString() == "255" && img.GetPixel(j, i).B.ToString() == "255" && img.GetPixel(j, i).G.ToString() == "255" && img.GetPixel(j, i).R.ToString() == "255")
                    {
                        texto[i + j * img.Height] = 1;
                    }
                    else
                    {
                        texto[i + j * img.Height] = 0;
                    }
                    //textBox1.AppendText(Convert.ToString(texto[i + j * img.Height]));
                }
                //textBox1.AppendText("\n");
            }
            return texto;
        }

        public int[] HebbAlg(sbyte[][] arr, int arrSize, int[] signal, int neuron)
        {
            int i = 0, j = 0;
            int S = 0;
            int[] dw = new int[arrSize + 1];
            int[] y = new int[NN];
            int[,] x = new int[NN, arrSize + 1];

            for (j = 0; j < NN; ++j)
            {
                x[j, 0] = 1;
                for (i = 1; i <= arrSize; ++i)
                {
                    x[j, i] = arr[j][i - 1];
                }
            }

            for (j = 0; j < NN; ++j)
            {
                for (i = 0; i <= arrSize; ++i)
                {
                    if (x[j, i] * signal[j] == 1)
                        dw[i] = 1;
                    else if (x[j, i] == 0)
                        dw[i] = 0;
                    else if (x[j, i] != 0 && signal[j] == 0)
                        dw[i] = -1;

                    w[neuron, i] = w[neuron, i] + dw[i];
                    S += x[j, i] * w[neuron, i];
                }
                S += w[neuron, 0];
                //if (S <= 0) y[j] = 0;
                //else if (S > 0) y[j] = 1;
                y[j] = S;
                SumOfNeyrons[j] = S;
                S = 0;
            }
            return y;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            //opf.Multiselect = true;
            string fp = "";
            if (opf.ShowDialog() == DialogResult.OK)
            {
                fp = opf.FileName;
                pictureBox1.Image = Image.FromFile(fp);
            }

            Bitmap img;
            img = new Bitmap(opf.FileName.ToString());

            int N = img.Height * img.Width;
            bool flag = false;

            sbyte[] textoF = new sbyte[N];
            textoF = ConvertToByte(img);
            for (int nj = 0; nj < NN; ++nj)
            {
                int SS = 0;
                for (int i = 1; i < N; ++i)
                {
                    SS += textoF[i] * w[nj, i]; 
                }
                SS += w[nj, 0]; 
                //textBox1.Clear();
                textBox1.AppendText(Convert.ToString($"Это буква {SS}\n")); 
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox2.Text == "") MessageBox.Show("Укажите, какие буквы будут на изображениях!");
            else
            {
                OpenFileDialog opf = new OpenFileDialog();
                string fp = "";
                PictureBox[] pics = { pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5 };
                Bitmap img;
                int N = 0;
                sbyte[][] texto1 = new sbyte[NN][];
                S = new int[NN][];

                for (int i = 0; i < NN; ++i)
                {
                    if (opf.ShowDialog() == DialogResult.OK)
                    {
                        fp = opf.FileName;
                        pics[i].Image = Image.FromFile(fp);
                    }

                    img = new Bitmap(opf.FileName.ToString());

                    N = img.Height * img.Width; // image size

                    imgName[i] = textBox2.Text[i];

                    texto1[i] = ConvertToByte(img); // array of bit representation of images
                }
                // Алгоритм Хеба
                for (int i = 0; i < NN; ++i)
                {
                    signal1[i] = 1;
                    if (i > 0) signal1[i - 1] = 0;
                    if (imgCount == 0)
                    {
                        w = new int[NN, N + 1];
                        ++imgCount;
                        //textBox1.AppendText(Convert.ToString(S1));
                    }
                    S[i] = HebbAlg(texto1, N, signal1, i);
                    //while (S[i][i] <= 0)
                    //{
                    //    //S[i][i] *= -1;
                    //    S[i] = HebbAlg(texto1, N, signal1, i); // train neuron until right signal returns
                    //}
                }

                for (int i = 0; i < NN; ++i)
                {
                    for (int j = 0; j < NN; ++j)
                    {
                        textBox1.AppendText($"{Convert.ToString(S[i][j])} ");
                    }
                }
                textBox1.AppendText("   ");
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
