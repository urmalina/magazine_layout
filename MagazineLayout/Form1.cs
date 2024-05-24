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

namespace MagazineLayout
{
    public partial class Form1 : Form
    {
        bool isDrawing = false;
        private Rectangle currentRectangle;
        private Point startPoint;
        private List<Rectangle> illustrations = new List<Rectangle>();
        private List<Rectangle> Rectangles = new List<Rectangle>();
        private OpenFileDialog openFileDialog = new OpenFileDialog();
        public Form1()
        {
            InitializeComponent();

        }        

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //при клике начинается отрисовка прямоугольника
            //при втором клике отрисовка завершается
            startPoint = e.Location;
            if (isDrawing)
            {
                isDrawing = false;
                illustrations.Add(currentRectangle);
                pictureBox1.Invalidate();
            }
            else
            {
                isDrawing = true;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawing)
            {
                var endPoint = e.Location;
                currentRectangle = new Rectangle(
                    Math.Min(startPoint.X, endPoint.X),
                    Math.Min(startPoint.Y, endPoint.Y),
                    Math.Abs(startPoint.X - endPoint.X),
                    Math.Abs(startPoint.Y - endPoint.Y));
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            foreach (var rect in illustrations)
            {
                g.FillRectangle(Brushes.Gray, rect);
            }
            if (isDrawing)
            {
                g.FillRectangle(Brushes.Gray, currentRectangle);
            }
        }

        private void chooseFile_Click(object sender, EventArgs e)
        {
            openFileDialog.InitialDirectory = "c:\\";
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void generateLayout_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text) || !File.Exists(textBox1.Text))
            {
                MessageBox.Show("Выберите корректный файл с текстом.");
                return;
            }

            string text = File.ReadAllText(textBox1.Text);
            Graphics g = pictureBox1.CreateGraphics();
            
            LayoutMaker layoutMaker = new LayoutMaker(g, pictureBox1, text, illustrations);
            layoutMaker.DrawTextOnLayout();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            illustrations.Clear();
            pictureBox1.Invalidate();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (illustrations.Count > 0)
            {
                illustrations.Remove(currentRectangle);
                pictureBox1.Invalidate();
            }

        }

       
    }
}
