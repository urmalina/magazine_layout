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
        private List<int> letterSpaces;
        private List<List<WordBox>> lines;
        
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
            GenerateWordRectangles();

            CorrectFontSize();
            //wordBoxes = GenerateWordRectangles();
            CorrectWordSpace();
            foreach (WordBox wordBox in wordBoxes)
            {
                g.DrawString(wordBox.WordValue, font, Brushes.Black, wordBox.Rectangle.Location);
                //foreach ()
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

        private void GenerateWordRectangles()
        {   wordBoxes = new List<WordBox>();
            illustrations.Sort((r1, r2) => r1.Y.CompareTo(r2.Y));
            //int lineHeight = TextRenderer.MeasureText("A", font).Height;
            string[] words = text.Split(' ', '\n');

            //List<WordBox> wordBoxes1 = new List<WordBox>();
            //lines = new List<List<WordBox>>();
            // Рисование текста
            int x = mainRectangle.Left;
            int y = mainRectangle.Top;
            //List<WordBox> line = new List<WordBox>();
            //Rectangle wordRect = new Rectangle();
            foreach (string word in words)
            {
                //int indexOfletter = 0;
                //int numberOfLetters = word.Length;
                //List<WordBox> letters = GenerateLetterRectangles(word, x, y);
                //foreach(letter in letters)
                GenerateLetterRectangles(word, x, y);
                x = wordBoxes[wordBoxes.Count - 1].Rectangle.Right;
                y = wordBoxes[wordBoxes.Count - 1].Rectangle.Y;
            }
            //return wordBoxes1;

        }

        private void GenerateLetterRectangles(string word, int x, int y)
        {
            int X = x;
            int Y = y;
            int lineHeight = TextRenderer.MeasureText("A", font).Height;
            //int indexOfletter = 0;
            int numberOfLetters = word.Length;
            List<WordBox> letters = new List<WordBox>();
            for (int i = 0; i < word.Length; i++)            
            {                
                Size rectSize = TextRenderer.MeasureText(word[i].ToString(), font);
                Rectangle letterRect;
                if (i == numberOfLetters - 1)
                {
                    letterRect = new Rectangle(X, Y, rectSize.Width + spaceLength, rectSize.Height);
                }
                else
                {
                    letterRect = new Rectangle(X, Y, rectSize.Width, rectSize.Height);
                }
                //wordRect = new Rectangle(x, y, rectSize.Width + spaceLength, rectSize.Height);
                if (letterRect.Right > mainRectangle.Right)
                {
                    
                    Y += lineHeight;
                    X = mainRectangle.Left;
                    letters.Clear();                   
                    GenerateLetterRectangles(word, X, Y);
                    break;
                }
                if (IsRectangleCollision(letterRect, illustrations))
                {
                    foreach (var rect in illustrations)
                    {
                        if (letterRect.IntersectsWith(rect))
                        {
                            X = rect.Right;
                            Y = y;
                            letters.Clear();
                            GenerateLetterRectangles(word, X, Y);
                            break;
                        }


                    }


                }
                WordBox letter = new WordBox(word[i].ToString(), letterRect, font);
                letters.Add(letter);
                X = letterRect.Right;
                Y = letterRect.Y;
            }
            if (letters.Count > 0)
            {
                foreach (WordBox letter in letters)
                {
                    wordBoxes.Add(letter);
                }
            }
            

        }
        private void CorrectFontSize()
        {
            
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y < mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {
                Font font = new Font(FontFamily.GenericSansSerif, this.font.Size + 1);
                this.font = font;
                GenerateWordRectangles();
            }
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y > mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {

                Font font = new Font(FontFamily.GenericSansSerif, this.font.Size - 1);
                this.font = font;
                GenerateWordRectangles();
            }

        }

        private void CorrectWordSpace()
        {
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y < mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {
                this.spaceLength++;
                GenerateWordRectangles();
            }
            while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y > mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            {

                this.spaceLength--;
                GenerateWordRectangles();
            }
        }

        private void DivideOnChars()
        {
            //var groupedByY = wordBoxes
            //.GroupBy(wb => wb.Rectangle.Y)
            //.OrderBy(g => g.Key)
            //.ToList();

            //foreach (var group in groupedByY)
            //{
            //    //Console.WriteLine($"Group Y={group.Key}");
            //    foreach (var wordBox in group)
            //    {
            //        //Console.WriteLine($"  {wordBox}");
            //    }
            //}

            //while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y < mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            //{
            //    this.spaceLength++;
            //    wordBoxes = GenerateWordRectangles();
            //}
            //while (wordBoxes[wordBoxes.Count - 1].Rectangle.Y > mainRectangle.Height - 2 * TextRenderer.MeasureText("A", font).Height)
            //{

            //    this.spaceLength--;
            //    wordBoxes = GenerateWordRectangles();
            //}

            foreach (List<WordBox> line in lines)
            {
                int letterSpace = 1;
                //while ()
                //foreach (WordBox word in line)
                //{ 
                    
                //}
            }
        }
    }
}
