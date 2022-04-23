using Microsoft.Win32;
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

namespace _28_MusicPlayer
{
    /// <summary>
    /// Interaction logic for MusicPlayer.xaml
    /// </summary>
    public partial class MusicPlayer : Window
    {
        MediaPlayer mediaPlayer = new MediaPlayer();
        string fileName;

        public MusicPlayer()
        {
            InitializeComponent();
        }

        // open audio file
        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileSelector = new OpenFileDialog
            {
                Multiselect = false,
                DefaultExt = ".mp3"
            };

            bool? fileSelected = fileSelector.ShowDialog();
            if(fileSelected == true)
            {
                fileName = fileSelector.FileName;
                FileNameTextBox.Text = fileSelector.SafeFileName;
                mediaPlayer.Open(new Uri(fileName));
            }
        }

        // play
        private void PlayAudio(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
        }

        // pause
        private void PauseAudio(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
        }

        // stop
        private void StopAudio(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }
    }
}
