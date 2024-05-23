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
        private Rectangle rectangle { get; set; }
        private string wordValue { get; set; }

        private Font font { get; set; }

        public WordBox(string wordValue, Rectangle rectangle, Font font)
        {
            this.wordValue = wordValue;
            this.rectangle = rectangle;
            this.font = font;
        }

    }
}
