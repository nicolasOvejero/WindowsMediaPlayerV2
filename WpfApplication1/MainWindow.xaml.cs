using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WpfApplication1.Model;
using WpfApplication1.ViewModel;

namespace WpfApplication1
{
    public partial class MainWindow : Window
    {
        private bool fullscreen = false;
        private TimeSpan _pos;
        private DispatcherTimer _time = new DispatcherTimer();
        private DispatcherTimer DoubleClick = new DispatcherTimer();
        private bool isDragging = false;
        private static Timer aTimer;
        private bool inTimeout = false;
        private ListingFile LFile = new ListingFile();
        Playlist PlayList = new Playlist();
        [DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();
        private xmlWork xml = new xmlWork();

        public MainWindow()
        {
            try
            {
                InitializeComponent();
            }
            catch (System.Windows.Markup.XamlParseException e)
            {
                Console.WriteLine(e);
            }
            DoubleClick.Interval = TimeSpan.FromMilliseconds(GetDoubleClickTime());
            DoubleClick.Tick += (s, e) => DoubleClick.Stop();
            _time.Interval = TimeSpan.FromMilliseconds(1000);
            _time.Tick += new EventHandler(ticktock);
        }

        void ticktock(object sender, EventArgs e)
        {
            if (!isDragging)
            {
                seekBar.Value = mediaElement1.Position.TotalSeconds;
                timeact.Content = string.Format("{0:D2}:{1:D2}:{2:D2}", mediaElement1.Position.Hours, mediaElement1.Position.Minutes, mediaElement1.Position.Seconds);
            }
            if (_pos.Equals(mediaElement1.Position))
            {
                if (PlayList._position != 1)
                    PlayList.LetsGoNext(this.readFile);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /* 
        ** Button rewind 
        */
        private void rewind(object sender, RoutedEventArgs e)
        {
            mediaElement1.SpeedRatio -= 0.5;
        }

        private void forward(object sender, RoutedEventArgs e)
        {
            mediaElement1.SpeedRatio += 0.5;
        }

        /*
        ** Button close windows
        */
        private void myclose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /*
        ** Bouton play / pause
        */
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(buttonPlay.Content) == "Play")
            {
                mediaElement1.LoadedBehavior = MediaState.Play;
                buttonPlay.Content = "Pause";
                Image image = (Image)buttonPlay.Template.FindName("buttonPlayPauseImage", buttonPlay);
                image.BeginInit();
                image.Source = new BitmapImage(new Uri("images/Pause.png", UriKind.RelativeOrAbsolute));
                image.EndInit();
            }
            else if (Convert.ToString(buttonPlay.Content) == "Pause")
            {
                mediaElement1.LoadedBehavior = MediaState.Pause;
                buttonPlay.Content = "Play";
                Image image = (Image)buttonPlay.Template.FindName("buttonPlayPauseImage", buttonPlay);
                image.BeginInit();
                image.Source = new BitmapImage(new Uri("images/Play.png", UriKind.RelativeOrAbsolute));
                image.EndInit();
            }
        }

        /* 
        ** Bouton Stop 
        */
        private void button3_Click(object sender, RoutedEventArgs e)
        {
            mediaElement1.LoadedBehavior = MediaState.Stop;
            buttonPlay.Content = "Play";
            Image image = (Image)buttonPlay.Template.FindName("buttonPlayPauseImage", buttonPlay);
            image.BeginInit();
            image.Source = new BitmapImage(new Uri("images/Play.png", UriKind.RelativeOrAbsolute));
            image.EndInit();
        }

        /*
        ** Fonction pour recup le time de la vidéo
        */
        void mediaElement1_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (mediaElement1.NaturalDuration.HasTimeSpan)
            {
                _pos = mediaElement1.NaturalDuration.TimeSpan;
                seekBar.Maximum = _pos.TotalSeconds;
                seekBar.LargeChange = Math.Min(10, _pos.Seconds / 10);
                timemax.Content = string.Format("{0:D2}:{1:D2}:{2:D2}", _pos.Hours, _pos.Minutes, _pos.Seconds);
            }
            _time.Start();
        }


        /* 
        ** Fonction de volume
        */
        private void ChangeMediaVolume(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaElement1.Volume = (double)volumeSlider.Value / 100;
            if (Volume != null)
            {
                Volume.Text = ((int)volumeSlider.Value).ToString();
            }
        }

        /* 
        ** Fonction pour la bar de temps
        */
        private void seekBar_DragStarted(object sender, DragStartedEventArgs e)
        {
            isDragging = true;
        }

        private void seekBar_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            isDragging = false;
            mediaElement1.Position = TimeSpan.FromSeconds(seekBar.Value);
        }

        /*
        ** Fonction pour le plein ecran
        */
        private void MediaPlayer_MouseLeftUp(object sender, MouseButtonEventArgs e)
        {
            if (!DoubleClick.IsEnabled)
                DoubleClick.Start();
            else
            {
                if (!fullscreen)
                {
                    WindowState = WindowState.Maximized;
                    Border.Margin = new Thickness(0, 0, 0, 0);
                    style.Visibility = Visibility.Collapsed;
                    playList.Visibility = Visibility.Collapsed;
                    menu.Visibility = Visibility.Collapsed;
                    pl.Visibility = Visibility.Collapsed;
                    PanelBot.Visibility = Visibility.Collapsed;
                    tbSearch.Visibility = Visibility.Collapsed;
                    imsh.Visibility = Visibility.Collapsed;
                    pl.IsExpanded = false;
                    mediaElement1.Margin = new Thickness(0, 0, 0, 0);
                }
                else
                {
                    WindowState = WindowState.Normal;
                    style.Visibility = Visibility.Visible;
                    playList.Visibility = Visibility.Visible;
                    menu.Visibility = Visibility.Visible;
                    pl.Visibility = Visibility.Visible;
                    PanelBot.Visibility = Visibility.Visible;
                    mediaElement1.Margin = new Thickness(10, 8, 24, 72);
                }
                fullscreen = !fullscreen;
            }
        }

        /*
        ** Fonction pour Autorisé le DragAndDrop
        */
        private void playList_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Move;
        }


        /*
        ** Fonction pour DragAndDrop dans la playlist
        ** Ajout dans la playlist du/des fichier(s) en question
        */
        public void playList_DragDrop(object sender, DragEventArgs e)
        {
            String[] files = (String[])e.Data.GetData(DataFormats.FileDrop, false);

            try
            {
                foreach (string file in files)
                {
                    PlayList.Add(file);
                    PlayList.drawItem(playList);
                }
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Ceci n'est pas un fichier");
            }
        }

        private bool readFile(generalModel value)
        {
            PlayList._playPos = value.pos;
            mediaElement1.MediaOpened += new RoutedEventHandler(mediaElement1_MediaOpened);
            mediaElement1.Source = new Uri(value.Path);
            mediaElement1.LoadedBehavior = MediaState.Play;
            WindowsTitle.Text = value.Title;
            buttonPlay.Content = "Pause";
            Image image = (Image)buttonPlay.Template.FindName("buttonPlayPauseImage", buttonPlay);
            image.BeginInit();
            image.Source = new BitmapImage(new Uri("images/Pause.png", UriKind.RelativeOrAbsolute));
            image.EndInit();
            return (true);
        }

        /*
        ** Fonction pour géré les double click
        ** Récupere le fichier double cliqué et le cherche dans la list
        */
        private void DoubleClickHandler(object sender, MouseEventArgs e)
        {
            try
            {
                var query = PlayList.pl.First(pl => pl.Title == playList.SelectedItem.ToString());
                this.readFile(query);
                this.GoHomeClick(sender, e);
            }
            catch (InvalidOperationException)
            { }
        }


        [DllImport("kernel32.dll")]
        static extern uint GetCompressedFileSizeW([In, MarshalAs(UnmanagedType.LPWStr)] string lpFileName,
           [Out, MarshalAs(UnmanagedType.U4)] out uint lpFileSizeHigh);

        /*
        ** Fonction qui gère le click sur le menu Export PLaylist
        */
        private void ExportPlayList(object sender, RoutedEventArgs e)
        {
            xml.createXML(PlayList);
        }

        /*
        ** Fonction qui gère le click sur le menu Import PLaylist
        */
        private void ImportPlayList(object sender, RoutedEventArgs e)
        {
            PlayList = xml.createFromXML();
            PlayList.drawItem(playList);
        }

        /*
        ** Function pour ouvrrir un fichier et le lire
        ** Cette fonction ajoute aussi dans la playlist le media
        */
        private void OpenFilesClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.AddExtension = true;
            ofd.DefaultExt = "*.*";
            ofd.Filter = "All Files (*.*) | *.*.mpeg; *.avi; *.wmv; *.jpg; *.mpeg; *.avi; *.wmv |" +
                "All Supported Audio (*.mp3, *.wma) |*.mp3; *.wma | " +
                "All Supported Image (*.jpg)|*.jpg | " +
                "All Supported Video (*.mpeg, *.avi, *.wmv) | *.mpeg; *.avi; *.wmv";
            ofd.ShowDialog();
            mediaElement1.MediaOpened += new RoutedEventHandler(mediaElement1_MediaOpened);
            try
            {
                mediaElement1.Source = new Uri(ofd.FileName);
                mediaElement1.LoadedBehavior = MediaState.Pause;
                WindowsTitle.Text = ofd.FileName;
                PlayList.Add(ofd.FileName);
                playList.Items.Add(Path.GetFileName(ofd.FileName));
            }
            catch (UriFormatException)
            {
                Console.WriteLine("Format not supported");
            }
        }

        /*
        ** Fonction pour detecter les mouvements de la souris en plein écran
        */
        private void mediaElement1_MouseMove(object sender, MouseEventArgs e)
        {
            if (fullscreen && !inTimeout)
            {
                PanelBot.Visibility = Visibility.Visible;
                SetTimer();
            }
        }

        /*
        ** Fonction pour set le Timeout à 5sec
        */
        private void SetTimer()
        {
            inTimeout = true;
            aTimer = new Timer(5000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = false;
            aTimer.Enabled = true;
        }

        /*
        ** Fonction qui cache le panelBot après les 5 seconde
        */
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (fullscreen)
                    PanelBot.Visibility = Visibility.Collapsed;
                else
                    PanelBot.Visibility = Visibility.Visible;
            }));
            inTimeout = false;
        }

        /*
        ** Cache tout est afficher la view pour le listing des musiques
        */
        private void MusicDirectory_Click(object sender, RoutedEventArgs e)
        {
            LFile.listFolder = foldersMusicItem;
            LFile.block = FileListingSong;
            LFile.type = 0;
            LFile.ListDirectory(Environment.SpecialFolder.MyMusic);
            foldersMusicItem.Visibility = Visibility.Visible;
            FileListingSong.Visibility = Visibility.Visible;
            foldersImageItem.Visibility = Visibility.Collapsed;
            FileListingImage.Visibility = Visibility.Collapsed;
            foldersVideoItem.Visibility = Visibility.Collapsed;
            FileListingVideo.Visibility = Visibility.Collapsed;
            mediaElement1.Visibility = Visibility.Collapsed;
            PanelBot.Visibility = Visibility.Collapsed;
        }

        /*
        ** Cache tout est afficher la view pour le listing des images
        */
        private void ImageDirectory_Click(object sender, RoutedEventArgs e)
        {
            LFile.listFolder = foldersImageItem;
            LFile.block = FileListingImage;
            LFile.type = 1;
            LFile.ListDirectory(Environment.SpecialFolder.MyPictures);
            foldersImageItem.Visibility = Visibility.Visible;
            FileListingImage.Visibility = Visibility.Visible;
            foldersMusicItem.Visibility = Visibility.Collapsed;
            FileListingSong.Visibility = Visibility.Collapsed;
            foldersVideoItem.Visibility = Visibility.Collapsed;
            FileListingVideo.Visibility = Visibility.Collapsed;
            mediaElement1.Visibility = Visibility.Collapsed;
            PanelBot.Visibility = Visibility.Collapsed;
        }

        /*
        ** Cache tout est afficher la view pour le listing des videos
        */
        private void VideoDirectory_Click(object sender, RoutedEventArgs e)
        {
            LFile.listFolder = foldersVideoItem;
            LFile.block = FileListingVideo;
            LFile.type = 2;
            LFile.ListDirectory(Environment.SpecialFolder.MyVideos);
            foldersVideoItem.Visibility = Visibility.Visible;
            FileListingVideo.Visibility = Visibility.Visible;
            foldersMusicItem.Visibility = Visibility.Collapsed;
            FileListingSong.Visibility = Visibility.Collapsed;
            foldersImageItem.Visibility = Visibility.Collapsed;
            FileListingImage.Visibility = Visibility.Collapsed;
            mediaElement1.Visibility = Visibility.Collapsed;
            PanelBot.Visibility = Visibility.Collapsed;
        }

        /*
        ** Cache tout est afficher le player
        */
        private void GoHomeClick(object sender, RoutedEventArgs e)
        {
            foldersVideoItem.Visibility = Visibility.Collapsed;
            foldersImageItem.Visibility = Visibility.Collapsed;
            foldersMusicItem.Visibility = Visibility.Collapsed;
            FileListingVideo.Visibility = Visibility.Collapsed;
            FileListingImage.Visibility = Visibility.Collapsed;
            FileListingSong.Visibility = Visibility.Collapsed;
            mediaElement1.Visibility = Visibility.Visible;
            PanelBot.Visibility = Visibility.Visible;
        }

        /*
        ** Fonction qui check si la touche entrer est pressed sur la liste des muisques
        */
        private void FileListingMusics_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LFile.putSongsInPlayList(FileListingSong, playList, PlayList);
                PlayList.drawItem(playList);
            }
        }

        /*
        ** Fonction qui check si la touche entrer est pressed sur la liste des images
        */
        private void FileListingImages_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LFile.putImagesInPlayList(FileListingImage, playList, PlayList);
                PlayList.drawItem(playList);
            }
        }
        /*
        ** Fonction qui check si la touche entrer est pressed sur la liste des videos
        */
        private void FileListingVideos_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                LFile.putVideosInPlayList(FileListingVideo, playList, PlayList);
                PlayList.drawItem(playList);
            }
        }

        private void MusicHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            switch ((string)headerClicked.Column.Header)
            {
                case "Title":
                    if (!LFile.titleOrder)
                        LFile.orderDescMusicByTitle();
                    else
                        LFile.orderMusicByTitle();
                    break;
                case "Duration":
                    if (!LFile.dureationOrder)
                        LFile.orderDescMusicByDuration();
                    else
                        LFile.orderMusicByDuration();
                    break;
                case "Author":
                    if (!LFile.authorOrder)
                        LFile.orderDescMusicByAuthor();
                    else
                        LFile.orderMusicByAuthor();
                    break;
                case "Album":
                    if (!LFile.OrderAlbum)
                        LFile.orderDescMusicByAlbum();
                    else
                        LFile.orderMusicByAlbum();
                    break;
            }
        }

        private void ImageHeaderClick(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader headerClicked = e.OriginalSource as GridViewColumnHeader;
            switch ((string)headerClicked.Column.Header)
            {
                case "Title":
                    if (!LFile.titleOrder)
                        LFile.orderDescImageByTitle();
                    else
                        LFile.orderImageByTitle();
                    break;
            }
        }

        private void next_click(object sender, RoutedEventArgs e)
        {
            PlayList.LetsGoNext(this.readFile);
        }

        private void prev_click(object sender, RoutedEventArgs e)
        {
            PlayList.LetsGoPrev(this.readFile);
        }

        private void SearchPlayList(object sender, TextChangedEventArgs e)
        {
            PlayList.Search(tbSearch.Text, playList);
        }

        private void playList_keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                try
                {
                    PlayList.delete(((ListBox)sender).SelectedItem.ToString(), playList);
                }
                catch (NullReferenceException)
                {
                }
            }
        }

        private void media_keyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (fullscreen)
                    MediaPlayer_MouseLeftUp(null, null);
            }
        }
        
        private void pl_expanded(object sender, RoutedEventArgs e)
        {
            imsh.Visibility = Visibility.Visible;
            tbSearch.Visibility = Visibility.Visible;
        }

        private void pl_collapsed(object sender, RoutedEventArgs e)
        {
            tbSearch.Visibility = Visibility.Collapsed;
            imsh.Visibility = Visibility.Collapsed;
        }
    }
}