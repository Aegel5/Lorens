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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lorens
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        double xc;
        double yc;
        double w;
        double h;
        Canvas paintPanel = null;
        public MainWindow()
        {
            InitializeComponent();
            paintPanel = canvas;
            

        }

        double ykoeff = 1;
        double koeffScale = 1;
        double IncKoeff(double val)
        {
            return val * koeffScale;
        }
        double DecKoeff(double val)
        {
            return val / koeffScale;
        }
        double xToCanv(double x)
        {
            return  (IncKoeff(x) + xc);
        }

        double yToCanv(double y)
        {
            return ((yc) - IncKoeff(y)/ykoeff);
        }

        double xFromCanv(double x)
        {
            return DecKoeff(x - xc);
        }

        double yFromCanv(double y)
        {
            return DecKoeff(yc - y) * ykoeff;
        }

        bool CheckPointInBorder(double xcanv, double ycanv)
        {
            if (xcanv < 0 || xcanv > w || ycanv < 0 || ycanv > h)
                return false;

            return true;
        }


        List<UIElement> axesElements = new List<UIElement>();
        void DrawAxes()
        {
            void AddToPanel(UIElement el)
            {
                paintPanel.Children.Add(el);
                Canvas.SetZIndex(el, -1);
                axesElements.Add(el);
            }
            w = paintPanel.ActualWidth;
            h = paintPanel.ActualHeight;
            xc = w / 2.0;
            yc = h / 2.0;

            foreach (var el in axesElements)
            {
                paintPanel.Children.Remove(el);
            }
            axesElements.Clear();

            void DrawSimple()
            {
                Line line = new Line();



                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 1;
                line.X1 = 0;
                line.Y1 = yc;
                line.X2 = w;
                line.Y2 = yc;
                AddToPanel(line);

                line = new Line();

                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 1;
                line.X1 = xc;
                line.Y1 = 0;
                line.X2 = xc;
                line.Y2 = h;
                AddToPanel(line);

                int countAx = 15;
                var xpart = w / countAx;
                var ypart = h / countAx;
                var rpart = Math.Max(xpart, ypart);
                for (int i = 0; i < countAx + 1; i++)
                {
                    line = new Line();

                    line.Stroke = new SolidColorBrush(Colors.Gray);
                    line.StrokeThickness = 0.5;
                    line.X1 = 0 + rpart * i;
                    line.Y1 = 0;
                    line.X2 = 0 + rpart * i;
                    line.Y2 = h;
                    AddToPanel(line);
                }

                for (int i = 0; i < countAx + 1; i++)
                {
                    line = new Line();

                    line.Stroke = new SolidColorBrush(Colors.Gray);
                    line.StrokeThickness = 0.5;
                    line.X1 = 0;
                    line.Y1 = 0 + rpart * i;
                    line.X2 = w;
                    line.Y2 = 0 + rpart * i;
                    //if (!CheckPointInBorder(line.X1, line.Y1) || !CheckPointInBorder(line.X2, line.Y2))
                    //    continue;
                    AddToPanel(line);
                }
            }

            DrawSimple();



            // единичные окружности
            void draw(double rad, bool drawY)
            {
                void draw1(bool invertX)
                {
                    void draw2(bool invertY)
                    {
                        PointCollection polygonPoints = new PointCollection();
                        void dd(double x, double y)
                        {
                            if (invertY)
                                y = -y;
                            if (invertX)
                                x = -x;

                            if(drawY)
                            {
                                (x,y)=(y,x);
                            }

                            double xcanv = xToCanv(x);
                            double ycanv = yToCanv(y);

                            if (!CheckPointInBorder(xcanv, ycanv))
                                return;

                            polygonPoints.Add(new Point(xcanv, ycanv));
                        }
                        double xstart = rad;
                        double xend = xFromCanv(w);
                        double inc = 0;
                        if (drawY)
                        {
                            xend = yFromCanv(0);
                        }
                        int countp = 100;
                        inc = (xend - xstart) / countp;
                        double rad2 = rad * rad;
                        for (double i = 0; i <= countp; i++)
                        {
                            double x = xstart + i * inc;
                            if (i == countp)
                                x = xend;
                            if (x < rad)
                                continue;
                            double y = Math.Sqrt(x*x - rad2);

                            dd(x, y);
                        }

                        // рассчитаем последнюю видимую точку
                        double yend = yFromCanv(0);
                        if (drawY)
                        {
                            yend = xFromCanv(w);
                        }
                        xend = Math.Sqrt(yend * yend + rad2);
                        dd(xend, yend);


                        Polyline yellowPolyline = new Polyline();
                        yellowPolyline.Stroke = new SolidColorBrush(Colors.Gray);
                        yellowPolyline.StrokeThickness = 0.5;
                        yellowPolyline.Points = polygonPoints;

                        AddToPanel(yellowPolyline);
                    }

                    draw2(true);
                    draw2(false);
                }

                draw1(true);
                draw1(false);
            }

            int hypcount = 15;
            double radinc = Math.Max(w / hypcount, h / hypcount);
            radinc /= koeffScale;

            for (double i = radinc; i <= w; i += radinc)
            {
                draw(i, false);
            }

            for (double i = radinc * ykoeff; i <= h * ykoeff; i += radinc * ykoeff)
            {
                draw(i, true);
            }

        }
        void HandleAdd(Point cur)
        {
            addLineStep = AddLine.Second;
            lineToAdd = new MyLine();
            var line = lineToAdd.line;

            lineToAdd.endPoint.X = lineToAdd.firstPoint.X = xFromCanv(cur.X);
            lineToAdd.endPoint.Y = lineToAdd.firstPoint.Y = yFromCanv(cur.Y);

            line = lineToAdd.line;
            line.Stroke = SystemColors.WindowFrameBrush;
            line.X1 = cur.X;
            line.Y1 = cur.Y;


            line.X2 = line.X1;
            line.Y2 = line.Y2;

            var ec = lineToAdd.eStart;
            ec.StrokeThickness = 1;
            ec.Fill = Brushes.Yellow;
            ec.Stroke = Brushes.Black;
            ec.Width = prad;
            ec.Height = prad;

            lineToAdd.AdjustEllipses();



            paintPanel.Children.Add(ec);
            paintPanel.Children.Add(lineToAdd.eEnd);


            //wrapLine(line);
            paintPanel.Children.Add(line);
        }
        const int prad = 8;
        private void canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var cur = e.GetPosition((IInputElement)sender);
            if (addLineStep == AddLine.First)
            {
                HandleAdd(cur);
            }
            else if(addLineStep == AddLine.Second)
            {
                addLineStep = AddLine.Null;
                lstLines.Add(lineToAdd);
                Select(lineToAdd);
            }
            else
            {
                // select
                var elem = e.OriginalSource as Shape;
                bool found = false;
                if(elem != null)
                {
                    // fine myline
                    foreach(var el in lstLines)
                    {
                        if(el.line == elem || el.eStart == elem || el.eEnd == elem)
                        {
                            found = true;
                            Select(el);
                            break;
                        }
                    }
                }
                if(!found)
                {
                    HandleAdd(cur);
                }

            }
        }
        MyLine selected = null;
        void Select(MyLine el)
        {
            selected = el;

            var x1 = el.firstPoint.X;
            var t1 = el.firstPoint.Y;
            var x2 = el.endPoint.X;
            var t2 = el.endPoint.Y;
            var dt = t2 - t1;
            var dx = x2 - x1;
            var v = dx / dt;


            txt_x1.Text = Utils.DoubleToString(x1);
            txt_x2.Text = Utils.DoubleToString(x2);
            txt_t1.Text = Utils.DoubleToString(t1);
            txt_t2.Text = Utils.DoubleToString(t2);
            txt_v.Text = Utils.DoubleToString(v);
            txt_dt.Text = Utils.DoubleToString(dt);
            txt_dx.Text = Utils.DoubleToString(dx);
        }

        class MyLine
        {
            // координаты канвы
            public Line line = new Line();
            public Ellipse eStart = new Ellipse();
            public Ellipse eEnd = new Ellipse();

            // координаты графика
            public Point firstPoint = new Point();
            public Point endPoint = new Point();

            public void AdjustEllipses()
            {
                var ec = eStart;
                Canvas.SetTop(ec, line.Y1 - ec.Height / 2);
                Canvas.SetLeft(ec, line.X1 - ec.Width / 2);
                Canvas.SetZIndex(ec, 10);

                ec = eEnd;
                Canvas.SetTop(ec, line.Y2 - ec.Height / 2);
                Canvas.SetLeft(ec, line.X2 - ec.Width / 2);
                Canvas.SetZIndex(ec, 10);
            }
        }

        enum AddLine
        {
            Null,
            First,
            Second
        }



        MyLine lineToAdd = new MyLine();

        List<MyLine> lstLines = new List<MyLine>();

        AddLine addLineStep = AddLine.Null;

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            addLineStep = AddLine.First;
        }


        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            var curP = e.GetPosition((IInputElement)sender);
            var x = xFromCanv(curP.X);
            var y = yFromCanv(curP.Y);

            if (addLineStep == AddLine.Second)
            {
                var line = lineToAdd.line;
                lineToAdd.endPoint.X = x;
                lineToAdd.endPoint.Y = y;
                line.X2 = curP.X;
                line.Y2 = curP.Y;

                var ec = lineToAdd.eEnd;
                ec.StrokeThickness = 1;
                ec.Stroke = Brushes.Black;
                ec.Width = prad;
                ec.Height = prad;
                ec.Fill = Brushes.Yellow;

                lineToAdd.AdjustEllipses();
            }
            else if(addLineStep == AddLine.Null)
            {
                if(e.MouseDevice.LeftButton == MouseButtonState.Pressed)
                {
                    if(selected != null)
                    {
                        var x1 = selected.firstPoint.X;
                        var y1 = selected.firstPoint.Y;

                        var x2 = selected.endPoint.X;
                        var y2 = selected.endPoint.Y;

                        var v = (x - x2) / Math.Abs(y1 - y2);

                        if(Math.Abs(v) > 0 && Math.Abs(v) < 1)
                        {
                            DoLorenc(-v);
                        }
                    }
                }
            }
        }
        void DeleteSelected()
        {
            if (selected != null)
            {
                paintPanel.Children.Remove(selected.line);
                paintPanel.Children.Remove(selected.eStart);
                paintPanel.Children.Remove(selected.eEnd);
                lstLines.Remove(selected);
                selected = null;
            }
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            DeleteSelected();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (selected == null)
                return;
            if (!Utils.TryParse(txt_x1.Text, out double x1))
                return;
            if (!Utils.TryParse(txt_x2.Text, out double x2))
                return;
            if (!Utils.TryParse(txt_t1.Text, out double t1))
                return;
            if (!Utils.TryParse(txt_t2.Text, out double t2))
                return;

            selected.firstPoint.X = (x1);
            selected.firstPoint.Y = (t1);
            selected.endPoint.X = (x2);
            selected.endPoint.Y = (t2);

            ToCanvasKoord(selected);

            Select(selected);
        }

        void DoLorenc(double v)
        {
            if (selected == null)
                return;

            foreach (var el in lstLines)
            {
                var line = el.line;

                var x1 = el.firstPoint.X;
                var t1 = el.firstPoint.Y;
                var x2 = el.endPoint.X;
                var t2 = el.endPoint.Y;

                var (x1new, t1new) = Utils.CalcLorens(x1, t1, v, 1);
                var (x2new, t2new) = Utils.CalcLorens(x2, t2, v, 1);

                el.firstPoint.X = (x1new);
                el.firstPoint.Y = (t1new);
                el.endPoint.X = (x2new);
                el.endPoint.Y = (t2new);

                ToCanvasKoord(el);
            }

            Select(selected);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (selected == null)
                return;

            var cur_x1 = selected.firstPoint.X;
            var cur_t1 = selected.firstPoint.Y;
            var cur_x2 = selected.endPoint.X;
            var cur_t2 = selected.endPoint.Y;
            var cur_v = (cur_x2 - cur_x1) / (cur_t2 - cur_t1);

            if(Math.Abs(cur_v) >= 1)
            {
                MessageBox.Show($"Невозможно преобразовать, так как скорость больше или равна скорости света. c=1, v={cur_v}");
                return;
            }

            DoLorenc(cur_v);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
            {
                //addLineStep = AddLine.Null;
            }
            else if(e.Key == Key.Delete)
            {
                DeleteSelected();
            }
        }

        void ToCanvasKoord(MyLine el)
        {
            el.line.X1 = xToCanv(el.firstPoint.X);
            el.line.Y1 = yToCanv(el.firstPoint.Y);
            el.line.X2 = xToCanv(el.endPoint.X);
            el.line.Y2 = yToCanv(el.endPoint.Y);
            el.AdjustEllipses();
        }
        
        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            addLineStep = AddLine.Null;

            double kchange = Math.Abs(e.Delta / 100.0);

            bool fCtrl = Keyboard.Modifiers == ModifierKeys.Control;

                if (e.Delta > 0)
            {
                if (fCtrl)
                {
                    ykoeff *= kchange;
                }
                else
                {
                    koeffScale *= kchange;
                }
            }
            else if (e.Delta < 0)
            {
                if (fCtrl)
                {
                    ykoeff /= kchange;
                }
                else
                {
                    koeffScale /= kchange;
                }
            }

            if(fCtrl)
            {
                DrawAxes();
            }

            DrawLines();
        }

        void DrawLines()
        {
            foreach (var el in lstLines)
            {
                ToCanvasKoord(el);
            }
        }

        private void canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DrawAxes();
            DrawLines();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawAxes();
        }
    }
}
