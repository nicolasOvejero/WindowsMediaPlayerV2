using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfApplication1.Model;

namespace WpfApplication1
{
    class ListingFile
    {
        public TreeView listFolder { get; set; }
        public ListView block { get; set; }
        private object dummyNode = null;
        private List<modelSongs> songs = new List<modelSongs>();
        private List<modelPicture> image = new List<modelPicture>();
        private List<modelVideo> video = new List<modelVideo>();
        private static readonly List<string> SoundExtensions = new List<string> { ".MP3", ".WAV" };
        private static readonly List<string> ImageExtensions = new List<string> { ".JPG" };
        private static readonly List<string> VideoExtensions = new List<string> { ".MPEG", ".AVI", ".WMV" };
        public int type { set;  get; }
        public bool titleOrder { get; set; }
        public bool dureationOrder { get; set; }
        public bool authorOrder { get; set; }
        public bool OrderAlbum { get; set; }

        /*
        ** Tri la liste des musiques par order de titre ascendant
        */
        public void orderMusicByTitle()
        {
            titleOrder = false;
            IEnumerable<modelSongs> query = songs.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des musiques par order de titre descendant
        */
        public void orderDescMusicByTitle()
        {
            titleOrder = true;
            IEnumerable<modelSongs> query = songs.OrderByDescending(x => x.Title, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
         ** Tri la liste des musique par order de durée ascendant
         */
        public void orderMusicByDuration()
        {
            dureationOrder = false;
            IEnumerable<modelSongs> query = songs.OrderBy(x => x.RealLength, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des musiques par order de durée descendant
        */
        public void orderDescMusicByDuration()
        {
            dureationOrder = true;
            IEnumerable<modelSongs> query = songs.OrderByDescending(x => x.RealLength, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des musiques par order de author ascendant
        */
        public void orderMusicByAuthor()
        {
            authorOrder = false;
            IEnumerable<modelSongs> query = songs.OrderBy(x => x.Author, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des musiques par order de author descendant
        */
        public void orderDescMusicByAuthor()
        {
            authorOrder = true;
            IEnumerable<modelSongs> query = songs.OrderByDescending(x => x.Author, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des musiques par order de Album ascendant
        */
        public void orderMusicByAlbum()
        {
            OrderAlbum = false;
            IEnumerable<modelSongs> query = songs.OrderBy(x => x.Album, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des musiques par order de Album descendant
        */
        public void orderDescMusicByAlbum()
        {
            OrderAlbum = true;
            IEnumerable<modelSongs> query = songs.OrderByDescending(x => x.Album, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelSongs song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
         ** Tri la liste des images par order de titre ascendant
         */
        public void orderImageByTitle()
        {
            titleOrder = false;
            IEnumerable<modelPicture> query = image.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelPicture song in query)
            {
                block.Items.Add(song);
            }
        }

        /*
        ** Tri la liste des images par order de titre descendant
        */
        public void orderDescImageByTitle()
        {
            titleOrder = true;
            IEnumerable<modelPicture> query = image.OrderByDescending(x => x.Title, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelPicture image in query)
            {
                block.Items.Add(image);
            }
        }

        /*
        ** Tri la liste des videos par order de Title ascendant
        */
        public void orderVideoByTitle()
        {
            titleOrder = false;
            IEnumerable<modelVideo> query = video.OrderBy(x => x.Title, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelVideo video in query)
            {
                block.Items.Add(video);
            }
        }

        /*
        ** Tri la liste des videos par order de Title descendant
        */
        public void orderDescVideoByTitle()
        {
            titleOrder = true;
            IEnumerable<modelVideo> query = video.OrderByDescending(x => x.Title, StringComparer.InvariantCultureIgnoreCase);
            block.Items.Clear();
            foreach (modelVideo video in query)
            {
                block.Items.Add(video);
            }
        }

        /*
        ** Fonction qui permet de lister les directory
        */
        public void ListDirectory(Environment.SpecialFolder directory)
        {
            listFolder.Items.Clear();
            TreeViewItem item = new TreeViewItem();
            item.Header = directory;
            item.Tag = Environment.GetFolderPath(directory);
            item.FontWeight = FontWeights.Normal;
            item.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0xD8, 0xD8));
            item.Items.Add(dummyNode);
            item.Expanded += new RoutedEventHandler(folder_Expanded);
            item.Selected += treeItem_Selected;
            listFolder.Items.Add(item);
        }

        /*
        ** Fonction qui permet de lister le subItem
        */
        private void folder_Expanded(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;

            if (item.Items.Count == 1 && item.Items[0] == dummyNode)
            {
                item.Items.Clear();
                try
                {
                    foreach (string s in Directory.GetDirectories(item.Tag.ToString()))
                    {
                        TreeViewItem subitem = new TreeViewItem();
                        subitem.Header = s.Substring(s.LastIndexOf("\\") + 1);
                        subitem.Tag = s;
                        subitem.FontWeight = FontWeights.Normal;
                        subitem.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xD8, 0xD8, 0xD8));
                        subitem.Items.Add(dummyNode);
                        subitem.Expanded += new RoutedEventHandler(folder_Expanded);
                        subitem.Selected += treeItem_Selected;
                        item.Items.Add(subitem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("error", ex.ToString());
                }
            }
        }

        /*
        ** Fonction pour afficher les musiques
        */
        private void getSoundInfo(String[] fileEntries)
        {
            foreach (String fileName in fileEntries)
            {
                String path = Path.GetExtension(fileName);
                if (SoundExtensions.Contains(path.ToUpperInvariant()))
                {
                    TagLib.File tagFile = TagLib.File.Create(fileName);
                    FileInfo file = new FileInfo(fileName);
                    var tmpSong = new modelSongs(Path.GetFileNameWithoutExtension(fileName), fileName,
                                        file.Length, tagFile.Tag.FirstAlbumArtist,
                                        DateTime.UtcNow, new TimeSpan(tagFile.Properties.Duration.Ticks), tagFile.Tag.Album);
                    songs.Add(tmpSong);
                }
                this.orderMusicByTitle();
            }
        }

        /*
        ** Fonction pour afficher les images
        */
        private void getImageInfo(String[] fileEntries)
        {
            foreach (String fileName in fileEntries)
            {
                String path = Path.GetExtension(fileName);
                if (ImageExtensions.Contains(path.ToUpperInvariant()))
                {
                    TagLib.File tagFile = TagLib.File.Create(fileName);
                    FileInfo file = new FileInfo(fileName);
                    var tmpImage = new modelPicture(Path.GetFileNameWithoutExtension(fileName), fileName,
                                        file.Length, tagFile.Tag.FirstAlbumArtist, DateTime.UtcNow);
                    image.Add(tmpImage);
                }
                this.orderImageByTitle();
            }
        }

        /*
        ** Fonction pour afficher les videos
        */
        private void getVideoInfo(String[] fileEntries)
        {
            foreach (String fileName in fileEntries)
            {
                String path = Path.GetExtension(fileName);
                if (VideoExtensions.Contains(path.ToUpperInvariant()))
                {
                    TagLib.File tagFile = TagLib.File.Create(fileName);
                    FileInfo file = new FileInfo(fileName);
                    var tmpVideo = new modelVideo(Path.GetFileNameWithoutExtension(fileName), fileName,
                                        file.Length, tagFile.Tag.FirstAlbumArtist,
                                        DateTime.UtcNow, new TimeSpan(tagFile.Properties.Duration.Ticks));
                    video.Add(tmpVideo);
                }
                this.orderVideoByTitle();
            }
        }

        /*
        ** Fonction pour choisir les bonnes informations pour l'affichage
        */
        private void treeItem_Selected(object sender, RoutedEventArgs e)
        {
            TreeViewItem item = sender as TreeViewItem;
            TreeViewItem TreeItem = e.OriginalSource as TreeViewItem;

            if (TreeItem.Tag == item.Tag)
            {
                String[] fileEntries = Directory.GetFiles(item.Tag.ToString());
                block.Items.Clear();
                switch (type)
                {
                    case 0:
                        getSoundInfo(fileEntries);
                        break;
                    case 1:
                        getImageInfo(fileEntries);
                        break;
                    case 2:
                        getVideoInfo(fileEntries);
                        break;
                }
            }
        }

        /*
        ** Fonction pour ajouter des musiques dans la playlist
        */
        public void putSongsInPlayList(ListView source, ListBox target, Playlist listPL)
        {
            foreach (modelSongs item in source.SelectedItems)
            {
                listPL.Add(item);
            }
        }

        /*
        ** Fonction pour ajouter des images dans la playlist
        */
        public void putImagesInPlayList(ListView source, ListBox target, Playlist listPL)
        {
            foreach (modelPicture item in source.SelectedItems)
            {
                listPL.Add(item);
            }
        }

        /*
        ** Fonction pour ajouter des videos dans la playlist
        */
        public void putVideosInPlayList(ListView source, ListBox target, Playlist listPL)
        {
            foreach (modelVideo item in source.SelectedItems)
            {
                listPL.Add(item);
            }
        }
    }
}