using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartPointSwitch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private DataPoint firstP;
        private DataPoint secondP;
        private bool selFist = false;
        private bool selSecond = false;
        private ToolTip tip = new ToolTip();

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();

            var rnd = new Random();

            for (int i = 0; i < 3; i++)
            {
                var serie = new Series();
                serie.Name = i.ToString();
                serie.ChartType = SeriesChartType.Line;
                serie.MarkerStyle = MarkerStyle.Circle;
                serie.MarkerSize = 7;

                for (int j = 0; j < 10; j++)
                {
                    serie.Points.AddXY(j, rnd.Next(0,10000) / 1000.0);
                }

                chart1.Series.Add(serie);
            }
        }

        private void chart1_MouseClick(object sender, MouseEventArgs e)
        {
            var hit = chart1.HitTest(e.X, e.Y);

            if (hit.ChartElementType == ChartElementType.DataPoint)
            {
                var p = (DataPoint)hit.Object;

                if (selFist == false)
                {
                    firstP = p;
                    selFist = true;
                }
                else if (p != firstP)
                {
                    secondP = p;
                    selSecond = true;
                }

                if (selFist == true && selSecond == true)
                {
                    var x = secondP.XValue;
                    var y = secondP.YValues[0];

                    secondP.XValue = firstP.XValue;
                    secondP.YValues[0] = firstP.YValues[0];

                    firstP.XValue = x;
                    firstP.YValues[0] = y;

                    chart1.Invalidate();

                    selFist = false;
                    selSecond = false;
                }
            }
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            var hit = chart1.HitTest(e.X, e.Y);

            if (hit.ChartElementType != ChartElementType.DataPoint)
            {
                tip.RemoveAll();
                tip.Tag = null;
                return;
            }

            var p = (DataPoint)hit.Object;
            if (tip.Tag == p)
                return;
            var x = p.XValue;
            var y = p.YValues[0];
            var txt = string.Format("X={0}\nY={1}", x, y);
            tip.Tag = p;
            tip.Show(txt, chart1, e.Location.X + 10, e.Location.Y + 10);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Title = "Save to file";
            sfd.Filter = "Text file|*.txt";
            
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                SaveSeriesToFile(sfd.FileName);
            }
        }

        private void SaveSeriesToFile(string fileName)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < chart1.Series.Count; i++)
            {
                sb.Append(i + 1);
                for (int j = 0; j < chart1.Series[i].Points.Count; j++)
                {
                    sb.Append("\t" + chart1.Series[i].Points[j].YValues[0]);
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText(fileName, sb.ToString());
        }
    }
}
