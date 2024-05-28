using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MagazineLayout
{
    class LayoutMaker
    {
        private string text;
        public string Text
        {
            get { return text; }
            set { text = value;  }
        }
        private Rectangle mainRectangle;
        private Graphics g;
        private Font font = new Font(FontFamily.GenericSansSerif, 1);
        private List<WordBox> wordBoxes;
        private List<Rectangle> illustrations;
        private int spaceLength;
       
        //public Rectangle MainRectangle { get => mainRectangle; set => mainRectangle = value; }

        //public Graphics G { get => g; set => g = value; }
        //public Font Font { get => font; set => font = value; }
        //public List<WordBox> WordBoxes { get => wordBoxes; set => wordBoxes = value; }
        //public List<Rectangle> Illustrations { get => illustrations; set => illustrations = value; }

        public LayoutMaker(Graphics g, PictureBox pictureBox, string Text, List<Rectangle> illustrations)
        {
            this.g = g;
            mainRectangle = pictureBox.DisplayRectangle;
            this.Text = Text;
            this.illustrations = illustrations;
        }

        public void DrawTextOnLayout()
        {
            //FindFontParameters();
            wordBoxes = GenerateWordRectangles();
            CorrectFontSize();            
            CorrectWordSpace();
            foreach (WordBox wordBox in wordBoxes)
            {
                g.DrawString(wordBox.WordValue, font, Brushes.Black, wordBox.Rectangle.Location);
            }
        }

        //private void FindFontParameters()
        //{
        //    int squarePictureBox = mainRectangle.Width * mainRectangle.Height;
        //    int squareOfAllRectangles = 0;
        //    foreach (Rectangle rect in illustrations)
        //    {
        //        squareOfAllRectangles += rect.Height * rect.Width;
        //    }
        //    int squareForText = squarePictureBox - squareOfAllRectangles;

        //    float fontSquare = 0; //подбираем размер шрифта чтобы занималась максимальная доступная площадь


        //    Font font = new Font(FontFamily.GenericSansSerif, 1);
        //    while (fontSquare < squareForText)
        //    {
        //        font = new Font(font.Name, font.Size + 1);
        //        fontSquare = TextRenderer.MeasureText(text, font).Width * TextRenderer.MeasureText(text, font).Height;

        //    }
        //    this.font = new Font(font.Name, font.Size);
            
        //}

        private bool IsRectangleCollision(Rectangle rectangle, List<Rectangle> rectangles)
        {
            foreach (var rect in rectangles)
            {
                if (rect.IntersectsWith(Rectangle.Round(rectangle)))
                {
                    return true;
                }
            }
            return false;
        }

        private List<WordBox> GenerateWordRectangles()
        {
            
            int lineHeight = TextRenderer.MeasureText("A", font).Height;
            string[] words = text.Split(' ');

            List<WordBox> wordBoxes1 = new List<WordBox>();
                        
            int x = mainRectangle.Left;
            int y = mainRectangle.Top;
            Rectangle wordRect = new Rectangle();
            foreach (string word in words)
            {
                Size wordSize = TextRenderer.MeasureText(word, font);

                wordRect = new Rectangle(x, y, wordSize.Width + spaceLength, wordSize.Height);
                if (wordRect.Right > mainRectangle.Right)
                {
                    wordRect.Y += lineHeight;
                    wordRect.X = mainRectangle.Left;
                }
                while (IsRectangleCollision(wordRect, illustrations))
                {
                    foreach (var rect in illustrations)
                    {
                        while (wordRect.IntersectsWith(rect))
                        {
                            wordRect.X = rect.Right;
                            if (wordRect.Right > mainRectangle.Right)
                            {
                                wordRect.Y += lineHeight;
                                wordRect.X = mainRectangle.Left;
                            }
                        }


                    }


                }
                wordBoxes1.Add(new WordBox(word, wordRect, font));

                y = wordRect.Y;
                x = wordRect.Right;

            }
            return wordBoxes1;

        }
        private void CorrectFontSize()
        {
            
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y < mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {
                Font font = new Font(FontFamily.GenericSansSerif, this.font.Size + 1);
                this.font = font;
                wordBoxes = GenerateWordRectangles();
            }
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y > mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {

                Font font = new Font(FontFamily.GenericSansSerif, this.font.Size - 1);
                this.font = font;
                wordBoxes = GenerateWordRectangles();
            }

        }

        private void CorrectWordSpace()
        {
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y < mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {
                this.spaceLength++;
                wordBoxes = GenerateWordRectangles();
            }
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y > mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {

                this.spaceLength--;
                wordBoxes = GenerateWordRectangles();
            }
        }

    }
}
