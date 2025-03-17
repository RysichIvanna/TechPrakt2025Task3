using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace lab
{
    internal class ColorProps
    {
        protected SolidColorBrush _shapeColor;

        public ColorProps(SolidColorBrush shapeColor)
        {
            _shapeColor = shapeColor;
        }

        public SolidColorBrush GetShapeColor()
        {
            return _shapeColor;
        }
    }

}
