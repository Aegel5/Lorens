using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Lorens
{
    /// <summary>
    /// Interaction logic for LorensCalc.xaml
    /// </summary>
    public partial class LorensCalc : Window
    {
        public LorensCalc()
        {
            InitializeComponent();
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CalcKoord();
        }



        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {
            CalcKoord();
        }

        private void TextBox_TextChanged_2(object sender, TextChangedEventArgs e)
        {
            CalcKoord();
        }

        private void TextBox_TextChanged_3(object sender, TextChangedEventArgs e)
        {
            CalcKoord();
        }

        void CalcKoord()
        {
            var resXs = "err";
            var resTs = "err";

            try
            {
                if (!Utils.TryParse(txtX.Text, out double x))
                    return;
                if (!Utils.TryParse(txtT.Text, out double t))
                    return;
                if (!Utils.TryParse(txtC.Text, out double c))
                    return;
                if (!Utils.TryParse(txtVs.Text, out double vs))
                    return;

                var a1 = x - vs * t;
                var a2 = Math.Sqrt(1 - (vs * vs) / (c * c));

                var xs = a1 / a2;

                var b1 = t - vs * x / (c * c);
                var ts = b1 / a2;

                resXs = Utils.DoubleToString(xs);
                resTs = Utils.DoubleToString(ts);
            }
            catch (Exception ex)
            {

            }
            finally
            {
                if (txtXs != null)
                    txtXs.Text = resXs;
                if (txtTs != null)
                    txtTs.Text = resTs;
            }
        }

        private void txtXs_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
