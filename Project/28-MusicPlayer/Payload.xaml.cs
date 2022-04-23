using System;
using System.Windows;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System.Drawing;
using System.Windows.Shapes;
using System.Windows.Markup;
using Imaged = System.Drawing.Image;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using TagLib;
using System.Collections.Generic;
using System.Linq;
using WMPLib;
using System.Windows.Input;

namespace _28_MusicPlayer
{
    public partial class Payload : Window
    {
        bool isPlayingAudio = false;
        List<string> songPaths = new List<string>();
        WindowsMediaPlayer Player;
        bool isOnShufflePlay = false;
        List<string> shufflePlayListArray;

        public Payload()
        {
            InitializeComponent();
            Player = new WindowsMediaPlayer();
            Player.PlayStateChange +=  new _WMPOCXEvents_PlayStateChangeEventHandler(MediaPlayerStateChange);

        }

        // close button
        private void CloseApp(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        // open audio file
        private void AddSongs(object sender, RoutedEventArgs e)
        {
            // open file selector
            OpenFileDialog fileSelector = new OpenFileDialog { Multiselect = true, DefaultExt = ".mp3" };

            // if file selected put it in the media player and update info
            bool? fileSelected = fileSelector.ShowDialog();
            if (fileSelected == true)
            {
                //UpdateMediaPlayerInfo(fileSelector.FileName);
                Playlist.Items.Clear();
                songPaths = fileSelector.FileNames.ToList();
                foreach (var filename in fileSelector.FileNames)
                {
                    addToPlaylist(filename);
                }
            }
        }

        // updates cover image and song title when we select a song
        void UpdateMediaPlayerInfo(string path)
        {
            // open audio file in the media player
            Player.URL = path;
            // play music
            PlayAudio();
            // open audio file and setup tags (info)
            TagLib.File f = new TagLib.Mpeg.AudioFile(path);
            // Artist - Title
            AudioTitle.Text = f.Tag.Performers[0] + " - " + f.Tag.Title;
            // Song Cover
            SongCover.ImageSource = ToWpfImage(f.Tag.Pictures[0]);
        }

        // converts System.Drawing.Image to a BitmapImage which can be added to Controls.Image.Source = BirmapImage;
        public static BitmapImage ToWpfImage(IPicture pic)
        {
            MemoryStream mama = new MemoryStream(pic.Data.Data);
            Imaged img = Imaged.FromStream(mama);

            MemoryStream ms = new MemoryStream();  // no using here! BitmapImage will dispose the stream after loading
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            BitmapImage ix = new BitmapImage();
            ix.BeginInit();
            ix.CacheOption = BitmapCacheOption.OnLoad;
            ix.StreamSource = ms;
            ix.EndInit();
            mama.Close();
            return ix;
        }

        // play/pause button
        private void PlayPauseAudioButton(object sender, RoutedEventArgs e)
        {
            if (isPlayingAudio) PauseAudio();
            else PlayAudio();
        }

        // when selected a song
        private void SongSelected(object sender, SelectionChangedEventArgs e)
        {
            if (Playlist.SelectedIndex>=0)
            {
                UpdateMediaPlayerInfo(songPaths[Playlist.SelectedIndex]);
            }
        }

        // play audio
        void PlayAudio()
        {
            if (Player.URL == string.Empty) return;
            Player.controls.play();
            PackIcon playPauseIcon = (PackIcon)PlayPauseButton.Content;
            isPlayingAudio = true;
            playPauseIcon.Kind = PackIconKind.Pause;
            PlayPauseButton.Content = playPauseIcon;
        }

        // pause audio
        void PauseAudio()
        {
            Player.controls.pause();
            PackIcon playPauseIcon = (PackIcon)PlayPauseButton.Content;
            isPlayingAudio = false;
            playPauseIcon.Kind = PackIconKind.Play;
            PlayPauseButton.Content = playPauseIcon;
        }

        // stop the audio
        void StopAudio()
        {
            Player.controls.stop();
            PackIcon playPauseIcon = (PackIcon)PlayPauseButton.Content;
            isPlayingAudio = false;
            playPauseIcon.Kind = PackIconKind.Play;
            PlayPauseButton.Content = playPauseIcon;
        }

        // adds item to playlist
        void addToPlaylist(string path) => Playlist.Items.Add(CreateListViewItem(path));

        // listviewItem creator
        ListViewItem CreateListViewItem(string path = null)
        {
            // open audio file and setup tags (info)
            TagLib.File f = new TagLib.Mpeg.AudioFile(path);
            // order in playlist
            string songOrderumber = (Playlist.Items.Count + 1).ToString();
            TextBlock orderNumber = new TextBlock { Text = "#"+songOrderumber, Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center };
            // song cover
            BitmapImage bitmapImage = ToWpfImage(f.Tag.Pictures[0]);
            ImageBrush itemSongCover = new ImageBrush { ImageSource = bitmapImage };
            Ellipse songCoverEllipse = new Ellipse { Width = 30, Height = 30, Fill = itemSongCover };
            // artist and title
            string itemTitleString = f.Tag.Performers[0] + " - " + f.Tag.Title;
            TextBlock itemTitle = new TextBlock
            {
                Text = itemTitleString, Margin=new Thickness(10,0, 10,0), VerticalAlignment=VerticalAlignment.Center, Width=100,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            // song length
            string songLengthString = f.Properties.Duration.ToString();
            songLengthString = RegexForSongLength(songLengthString);
            TextBlock songLength = new TextBlock { Text = songLengthString, VerticalAlignment=VerticalAlignment.Center, Margin = new Thickness(20, 0, 0, 0) };

            // stack panel
            StackPanel stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(orderNumber);
            stackPanel.Children.Add(songCoverEllipse);
            stackPanel.Children.Add(itemTitle);
            stackPanel.Children.Add(songLength);

            // listviewItem
            ListViewItem item = new ListViewItem { Content = stackPanel };
            return item;
        }

        // song length shortener
        string RegexForSongLength(string songLengthString)
        {
            songLengthString = songLengthString.Remove(8, songLengthString.Length - 8);
            if(songLengthString[0] == '0' && songLengthString[1] == '0')
            {
                songLengthString = songLengthString.Remove(0, 3);
                if(songLengthString[0] == '0') songLengthString = songLengthString.Remove(0, 1);
            }

            return songLengthString;
        }

        // goes to next song
        private void NextSongButton(object sender, RoutedEventArgs e)
        {
            if (Player.URL == string.Empty) return;
            int nextSongIndex = Playlist.SelectedIndex + 1;
            if (nextSongIndex > songPaths.Count - 1) nextSongIndex = 0;
            UpdateMediaPlayerInfo(songPaths[nextSongIndex]);
            Playlist.SelectedIndex = nextSongIndex;
        }

        // goes to previous song
        private void PreviousSongButton(object sender, RoutedEventArgs e)
        {
            if (Player.URL == string.Empty) return;
            int nextSongIndex = Playlist.SelectedIndex - 1;
            if (nextSongIndex < 0) nextSongIndex = songPaths.Count - 1;
            UpdateMediaPlayerInfo(songPaths[nextSongIndex]);
            Playlist.SelectedIndex = nextSongIndex;
        }

        // repeat buton
        private void RepeatButton(object sender, RoutedEventArgs e)
        {
            if (Player.URL == string.Empty) return;
            StopAudio();
            PlayAudio();
        }

        // shuffle play button
        private void ShufflePlayButton(object sender, RoutedEventArgs e)
        {
            ShufflePlay();
        }

        // shuffle play
        void ShufflePlay()
        {
            if (Player.URL == string.Empty) return;
            if (isOnShufflePlay)
            {
                Random rand = new Random();
                int randomSongIndex = rand.Next(0, shufflePlayListArray.Count);
                UpdateMediaPlayerInfo(shufflePlayListArray[randomSongIndex]);
                shufflePlayListArray.RemoveAt(randomSongIndex);
                if(shufflePlayListArray.Count < 1)
                {
                    isOnShufflePlay = false;
                }
            }
            else
            {
                shufflePlayListArray = songPaths.ToList();
                Random rand = new Random();
                int randomSongIndex = rand.Next(0, shufflePlayListArray.Count);
                UpdateMediaPlayerInfo(shufflePlayListArray[randomSongIndex]);
                shufflePlayListArray.RemoveAt(randomSongIndex);
                isOnShufflePlay = true;
            }
        }

        // media player state change
        private void MediaPlayerStateChange(int NewState)
        {
            if ((WMPPlayState)NewState == WMPPlayState.wmppsStopped && isOnShufflePlay)
            {
                ShufflePlay();
            }
        }

        // click and drag window
        private void WindowDrag(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) DragMove();
        }
    }
}