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
        private List<int> eachLineSpaces;
        private List<List<WordBox>> lines;
        private List<List<WordBox>> linesWithSpaces;

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
            FindFontParameters();
            wordBoxes = GenerateWordRectangles();

            CorrectFontSize();
            wordBoxes = GenerateWordRectangles();
            CorrectWordSpace();
            //foreach (WordBox wordBox in wordBoxes)
            //{
            //    g.DrawString(wordBox.WordValue, font, Brushes.Black, wordBox.Rectangle.Location);
            //}
            foreach (List<WordBox> line in linesWithSpaces)
            {
                foreach (WordBox wordBox in line)
                {
                    g.DrawString(wordBox.WordValue, font, Brushes.Black, wordBox.Rectangle.Location);
                }
            }   
        }

        private void FindFontParameters()
        {
            int squarePictureBox = mainRectangle.Width * mainRectangle.Height;
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
            this.font = new Font(font.Name, font.Size);
            
        }

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
            //illustrations.Sort((r1, r2) => r1.Y.CompareTo(r2.Y));
            int lineHeight = TextRenderer.MeasureText("A", font).Height;
            string[] words = text.Split(' ');

            List<WordBox> wordBoxes1 = new List<WordBox>();
            lines = new List<List<WordBox>>();
            List<WordBox> line = new List<WordBox>();

            // Рисование текста
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
                WordBox word1 = new WordBox(word, wordRect, font);
                wordBoxes1.Add(word1);

                if (y == word1.Rectangle.Y)
                {
                    line.Add(word1);
                }
                else
                {
                    List<WordBox> line1 = line;
                    lines.Add(line1);
                    line.Clear();
                    line.Add(word1);
                }

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
            linesWithSpaces = new List<List<WordBox>>();
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
            if (lines != null)
            {
                foreach (List<WordBox> line in lines)
                {
                    if (line != null)
                    {
                        int lineSpaceLength = this.spaceLength;
                        List<WordBox> testLine = line;
                        if (line.Count > 1)
                        {
                            while (!LineIntersectsRectangle(testLine) && testLine[testLine.Count - 1].Rectangle.Right < mainRectangle.Width)
                            {
                                testLine = GenerateLineWithSpaces(line, lineSpaceLength);
                                lineSpaceLength++;

                            }
                            //testLine = GenerateLineWithSpaces(line, lineSpaceLength );
                        }

                        //eachLineSpaces.Add(lineSpaceLength--);
                        List<WordBox> list = testLine;
                        linesWithSpaces.Add(list);
                    }
                    
                }
            }
            
        }
        private bool LineIntersectsRectangle(List<WordBox> line)
        {
            foreach (WordBox wordBox in line)
            {
                if (IsRectangleCollision(wordBox.Rectangle, illustrations))
                {
                    return true;
                }
                
            }
            return false;
        }

        private List<WordBox> GenerateLineWithSpaces(List<WordBox> line, int space)
        {
            List<WordBox> newLine = new List<WordBox>();
            WordBox newWordBox;
            for (int i = 1; i < line.Count; i++)
            {
                Rectangle newRectangle = new Rectangle(line[i].Rectangle.X + space, line[i].Rectangle.Y, line[i].Rectangle.Width, line[i].Rectangle.Height);
                newWordBox = new WordBox(line[i].WordValue, newRectangle, font);
                newLine.Add(newWordBox);
            }
            return newLine;
        }

    }
}
