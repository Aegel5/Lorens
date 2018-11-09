using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lorens
{
    class Utils
    {
        static public bool TryParse(string txtBox, out double val)
        {
            var txt = txtBox.Replace(".", ",");
            return double.TryParse(txt, out val);
        }

        static public  string DoubleToString(double val)
        {
            return Math.Round(val, 8).ToString().Replace(",", ".");
        }

        static public (double xnew, double tnew) CalcLorens(double x, double t, double v, double c)
        {
            var a1 = x - v * t;
            var a2 = Math.Sqrt(1 - (v * v) / (c * c));

            var xnew = a1 / a2;

            var b1 = t - v * x / (c * c);
            var tnew = b1 / a2;

            return (xnew, tnew);

        }
    }
}
