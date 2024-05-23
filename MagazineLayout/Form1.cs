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

        private void Form1_Load(object sender, EventArgs e)
        {

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
            Font font = findFontParameters(g, pictureBox1, illustrations, text);
            List<WordBox> wordBoxes = GenerateWordRectangles(g, text, font, pictureBox1.DisplayRectangle, illustrations, 10);

            
            foreach (WordBox wordBox in wordBoxes)
            {
                g.DrawString(wordBox.WordValue, font, Brushes.Black, wordBox.Rectangle.Location);
            }
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

        //private List<Rectangle> DividePageBySquares(PictureBox pictureBox, List<Rectangle> illustrations)
        //{
        //    List<Rectangle> result = new List<Rectangle>();
        //    illustrations.Sort((r1, r2) => r1.Y.CompareTo(r2.Y));
        //    Point startDividing = new Point(0, 0);
        //    foreach (Rectangle rect in illustrations)
        //    {
        //        //отделяем прямоугольник до начала рисунка
        //        result.Add(new Rectangle(startDividing, new Size(pictureBox.Width, rect.Y - startDividing.Y)));
        //        startDividing.Y = rect.Y;

        //        //отделяем прямоугольник слева от рисунка
        //        result.Add(new Rectangle(startDividing, new Size(rect.X,)
        //    }
        //}

        private Font findFontParameters(Graphics g, PictureBox pictureBox, List<Rectangle> illustrations, string text)
        {
            int squarePictureBox = pictureBox.Width * pictureBox.Height;
            int squareOfAllRectangles = 0;
            foreach (Rectangle rect in illustrations)
            {
                squareOfAllRectangles += rect.Height * rect.Width;
            }
            int squareForText = squarePictureBox - squareOfAllRectangles;

            float fontSquare = 0; //подбираем размер шрифта чтобы занималась максимальная доступная площадь


            Font font = new Font(FontFamily.GenericSansSerif, 1);
            while (fontSquare < squareForText)
            {
                font = new Font(font.Name, font.Size + 1);
                fontSquare = TextRenderer.MeasureText(text, font).Width * TextRenderer.MeasureText(text, font).Height;

            }
            font = new Font(font.Name, font.Size);
            return font;
        }

        private List<WordBox> GenerateWordRectangles(Graphics g, string text, Font font, Rectangle area, List<Rectangle> rectangles, int spaceLength)
        {
            rectangles.Sort((r1, r2) => r1.Y.CompareTo(r2.Y));
            int lineHeight = TextRenderer.MeasureText("A", font).Height;
            string[] words = text.Split(' ');

            List<WordBox> wordBoxes = new List<WordBox>();

            // Рисование текста
            int x = area.Left;
            int y = area.Top;
            Rectangle wordRect = new Rectangle();
            foreach (string word in words)
            {
                Size wordSize = TextRenderer.MeasureText(word, font);

                wordRect = new Rectangle(x, y, wordSize.Width + spaceLength, wordSize.Height);
                if (wordRect.Right > area.Right)
                {
                    wordRect.Y += lineHeight;
                    wordRect.X = area.Left;
                }
                while (IsRectangleCollision(wordRect, rectangles))
                {
                    foreach (var rect in rectangles)
                    {
                        while (wordRect.IntersectsWith(rect))
                        {
                            wordRect.X = rect.Right;
                            if (wordRect.Right > area.Right)
                            {
                                wordRect.Y += lineHeight;
                                wordRect.X = area.Left;
                            }
                        }


                    }


                }
                wordBoxes.Add(new WordBox(word, wordRect, font));

                y = wordRect.Y;
                x = wordRect.Right;

            }
            return wordBoxes;

        }

        private bool IsRectangleCollision(Rectangle textRect, List<Rectangle> rectangles)
        {
            foreach (var rect in rectangles)
            {
                if (rect.IntersectsWith(Rectangle.Round(textRect)))
                {
                    return true;
                }
            }
            return false;
        }

        //private List<WordBox> FontCorrector(Graphics g, List<WordBox> wordBoxes, Rectangle rectangle)
        //{
        //    while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y < rectangle.Y - 2* wordBoxes[wordBoxes.Count - 1].WordFont.Height)
        //    {
        //        wordBoxes[wordBoxes.Count - 1].WordFont = new Font ()
        //        wordBoxes = GenerateWordRectangles()
        //    }
        //}

        //private void DrawWithSpaces(Graphics g, Font font, List<WordBox> wordBoxes, Rectangle rect, List<Rectangle> illustrations)
        //{
        //    var groupedByY = wordBoxes
        //    .GroupBy(wb => wb.Rectangle.Y)
        //    .OrderBy(x => x.Key)
        //    .ToList();
        //    foreach (var group in groupedByY)
        //    {
        //        //Console.WriteLine($"Group Y={group.Key}");
        //        Rectangle lineRectangle = new Rectangle(0, group.Key, rect.Width, TextRenderer.MeasureText("A", font).Height);
        //        int IntersectLength = 0;
        //        if (IsRectangleCollision(lineRectangle, illustrations))
        //        {
        //            foreach (Rectangle illustration in illustrations)
        //            {
        //                if (lineRectangle.IntersectsWith(illustration))
        //                {
        //                    IntersectLength += illustration.Width;
        //                }
        //            }
        //        }
        //        int freeSpace = 
        //        foreach (var wordBox in group)
        //        {
        //            //Console.WriteLine($"  {wordBox}");                    
                    
                    

        //        }
        //    }

            //}
            //private void DrawJustifiedLine(Graphics g, string line, Font font, Rectangle rect)
            //{
            //    string[] words = line.Split(' ');
            //    if (words.Length == 1)
            //    {
            //        g.DrawString(line, font, Brushes.Black, rect.Left, rect.Top);
            //        return;
            //    }

            //    // Calculate total width of the words
            //    float wordsWidth = 0;
            //    foreach (string word in words)
            //    {
            //        wordsWidth += TextRenderer.MeasureText(word, font).Width;
            //    }

            //    // Calculate the width of the spaces
            //    float spaceWidth = TextRenderer.MeasureText(" ", font).Width;
            //    float extraSpace = rect.Width - wordsWidth;
            //    float spaceBetweenWords = spaceWidth + (extraSpace / (words.Length - 1));

            //    float x = rect.Left;
            //    foreach (string word in words)
            //    {
            //        g.DrawString(word, font, Brushes.Black, x, rect.Top);
            //        x += TextRenderer.MeasureText(word, font).Width + spaceBetweenWords;
            //    }
            //}



        //}
    }
}
