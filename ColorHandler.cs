using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using System.Windows.Documents;

namespace lab
{
    internal class ColorHandler : ColorProps
    {
        public ColorHandler(SolidColorBrush shapeColor) : base(shapeColor)
        {
        }

        public void SetShapeBrushColor()
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                _shapeColor = new SolidColorBrush(selectedColor);
            }
        }

        public static System.Windows.Media.Brush ChangeTextColor()
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Drawing.Color selectedColor = colorDialog.Color;
                System.Windows.Media.Color wpfColor = System.Windows.Media.Color.FromArgb(selectedColor.A, selectedColor.R, selectedColor.G, selectedColor.B);
                SolidColorBrush brush = new SolidColorBrush(wpfColor);
                return brush;
            }
            return null;
        }
        
        public static System.Windows.Media.Brush ChangeHightlightColor()
        {
            System.Windows.Forms.ColorDialog colorDialog = new System.Windows.Forms.ColorDialog();
            if (colorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color selectedColor = System.Windows.Media.Color.FromArgb(colorDialog.Color.A, colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B);
                SolidColorBrush brush = new SolidColorBrush(selectedColor);
                return brush;
            }
            return null;
        }
        
    }
}
