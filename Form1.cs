using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Kresty
{
    public partial class Form1 : Form
    {

        private struct Point
        {
            public double x, u;

            public Point(double x, double u)
            {
                this.x = x;
                this.u = u;
            }
        }

        Dictionary<int, Point> ujminus1;
        Dictionary<int, Point> uj;
        Dictionary<int, Point> ujplus1;
        double a = 100;
        double h = 0.2;
        double T = 0.001;
        double xmax = 10;
        double t = 0;
        int len;

        private double fi(double x)
        {
            return Math.Sin(x);
        }

        private double psi(double x)
        {
            return 0;
        }

        private double mu1(double t)
        {
            return 0;
        }

        private double mu2(double t)
        {
            return 0;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            uj = new Dictionary<int, Point>();
            ujminus1 = new Dictionary<int, Point>();
            ujplus1 = new Dictionary<int, Point>();

            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart1.ChartAreas[0].AxisX.Maximum = xmax;
            chart1.ChartAreas[0].AxisY.Minimum = -1;
            chart1.ChartAreas[0].AxisY.Maximum = 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init_solve();
            t = T;

            timer1.Start();
        }

        private void init_solve()
        {
            ujminus1_start();
            uj_start();
        }

        private void ujminus1_start()
        {
            chart1.Series[0].Points.Clear();

            int c = 0;
            for (double x = 0; x <= xmax; x += h)
            {
                Point p = new Point(x, fi(x));
                ujminus1.Add(c++, p);
                chart1.Series[0].Points.AddXY(x, p.u);
            }
            len = c;
        }

        private void uj_start()
        {
            chart1.Series[0].Points.Clear();

            foreach(KeyValuePair<int, Point> kv in ujminus1) 
            {
                double u = psi(kv.Value.x) * T + kv.Value.u;
                uj.Add(kv.Key, new Point(kv.Value.x, kv.Value.u));
                chart1.Series[0].Points.AddXY(kv.Value.x, kv.Value.u);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            step();
        }

        private void step()
        {
            t += T;
            chart1.Series[0].Points.Clear();
            ujplus1 = new Dictionary<int, Point>();

            double u0 = mu1(t);
            double ul = mu2(t);

            chart1.Series[0].Points.AddXY(0, u0);
            ujplus1.Add(0, new Point(0, u0));

            for (int i = 1; i <= len - 2; i++)
            {
                double u = (Math.Pow(a, 2) * Math.Pow(T, 2) / Math.Pow(h, 2)) * (uj[i + 1].u - 2 * uj[i].u + uj[i - 1].u) + 2 * uj[i].u - ujminus1[i].u;
                double x = uj[i].x;

                Point p = new Point(x, u);
                chart1.Series[0].Points.AddXY(x, u);
                ujplus1.Add(i, p);
            }

            chart1.Series[0].Points.AddXY(xmax, ul);
            ujplus1.Add(len - 1, new Point(xmax, ul));

            ujminus1.Clear();
            ujminus1 = new Dictionary<int, Point>(uj);
            uj = new Dictionary<int, Point>(ujplus1);
            ujplus1.Clear();
        }
    }
}
