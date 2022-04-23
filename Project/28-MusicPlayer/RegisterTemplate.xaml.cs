using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace _28_MusicPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        // click and drag on tool bar to move
        private void ToolBarDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }

        // close button
        private void CloseButton(object sender, RoutedEventArgs e) => Close();

        // minimise button
        private void Minimise(object sender, RoutedEventArgs e)
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Minimized;
        }

        // when window is maximised
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            if (WindowStyle != WindowStyle.None)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback)delegate (object unused)
                {
                    WindowStyle = WindowStyle.None;
                    return null;
                }
                , null);
            }
        } 
    }
}
