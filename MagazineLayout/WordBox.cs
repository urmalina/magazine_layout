using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagazineLayout
{
    class WordBox
    {
        private Rectangle rectangle;
        public Rectangle Rectangle
        {
            get { return rectangle; }
            set { rectangle = value; }

        }
        private string wordValue;
        public string WordValue
        {
            get { return wordValue; }
            set { wordValue = value; }
        }

        private Font wordFont;
        public Font WordFont
        {
            get { return wordFont; }
            set { wordFont = value; }
        }

        public void IncreaseFontSize(int add)
        {
            this.WordFont = new Font(this.WordFont.Name, this.WordFont.Size + add);
        }
        //private List<Rectangle> intersectRectangles = new List<Rectangle>();
        //public List<Rectangle> IntersectRectangles
        //{
        //    get { return intersectRectangles; }
        //    set { intersectRectangles = value; }
        //}
        //public void AddIntersectRectangle(Rectangle rectangle)
        //{
        //    this.IntersectRectangles.Add(rectangle);
        //}
        public WordBox(string wordValue, Rectangle rectangle, Font font)
        {
            this.wordValue = wordValue;
            this.rectangle = rectangle;
            this.wordFont = font;
        }

    }
}
