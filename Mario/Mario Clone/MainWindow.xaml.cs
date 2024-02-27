using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Mario_Clone;

namespace Mario_Clone
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Canvas rootCanvas = new Canvas();
        public static Game game;
        public static Thread t;

        public MainWindow()
        {
            InitializeComponent();
            root.Closing += new System.ComponentModel.CancelEventHandler(OnClose);
            Viewbox main = new Viewbox();
            main.StretchDirection = StretchDirection.Both;
            main.Stretch = Stretch.Fill;
            rootCanvas.Height = root.Height;
            rootCanvas.Width = root.Width;
            rootCanvas.Background = (Brush)new BrushConverter().ConvertFromString("#4B4B4B");
            main.Child = rootCanvas;
            root.Content = main;
            rootCanvas.Background = Brushes.DodgerBlue;
            game = new Game(rootCanvas, "Level 1.txt");
            game.Load();
                

            t = new Thread(game.Tick);
            t.SetApartmentState(ApartmentState.STA);
            t.Start();

        }

        private void Window_KeyDown(object sender, KeyEventArgs a)
        {
            switch (a.Key)
            {
                case Key.Up:
                    MainWindow.game.Up = true;
                    break;
                case Key.Down:
                    MainWindow.game.Down = true;
                    break;
                case Key.Right:
                    MainWindow.game.Right = true;
                    break;
                case Key.Left:
                    MainWindow.game.Left = true;
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs a)
        {
            switch (a.Key) { 
                case Key.Up:
                    game.Up = false;
                    break;
                case Key.Down:
                    game.Down = false;
                    break;
                case Key.Right:
                    game.Right = false;
                    break;
                case Key.Left:
                    game.Left = false;
                    break;
            }
        }

        private void OnClose(object sender, EventArgs a)
        {
            t.Abort();
            Application.Current.Shutdown();
        }



    }
}

        

