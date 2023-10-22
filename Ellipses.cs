using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace gr6
{
    internal class Ellipses
    {
        public double Width {  get; set; }
        public double Height { get; set; }
        public Thickness Margin { get; set; }
        public Brush Fill { get; set; }
        public string Content { get; set; }
        public double Top { get; set; }
        public double Left { get; set; }
    }
}
