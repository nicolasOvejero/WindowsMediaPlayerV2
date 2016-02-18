using System;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using WpfApplication1.Model;
using System.Linq;
using System.Windows.Controls;
using System.Windows;

namespace WpfApplication1
{
    [XmlType(TypeName = "PlaysList")]
    [Serializable]
    public class Playlist
    {
        [XmlArray("ElementPlaylist")]
        public List<generalModel> pl { get; set; }
        [XmlAttribute]
        public int _position  { get; set; }
        [XmlIgnore]
        private static readonly List<string> SoundExtensions = new List<string> { ".MP3", ".WAV" };
        [XmlIgnore]
        private static readonly List<string> ImageExtensions = new List<string> { ".JPG" };
        [XmlIgnore]
        private static readonly List<string> VideoExtensions = new List<string> { ".MPEG", ".AVI", ".WMV" };
        [XmlIgnore]
        public int _playPos { get; set; }

        public Playlist()
        {
            pl = new List<generalModel>();
            _position = 0;
            _playPos = 0;
        }

        /*
        ** Fonction pour ajouter des elements 
        */
        public void Add(modelVideo video)
        {
            video.pos = _position;
            pl.Add(video);
            _position++;
        }

        /*
        ** Fonction pour ajouter des elements 
        */
        public void Add(modelPicture image)
        {
            image.pos = _position;
            pl.Add(image);
            _position++;
        }

        /*
        ** Fonction pour ajouter des elements 
        */
        public void Add(modelSongs music)
        {
            music.pos = _position;
            pl.Add(music);
            _position++;
        }

        /*
        ** Fonction pour ajouter des elements 
        */
        public void Add(String file)
        {
            String path = Path.GetExtension(file);
            try 
            {
                TagLib.File tagFile = TagLib.File.Create(file);
                FileInfo fileI = new FileInfo(file);
                if (SoundExtensions.Contains(path.ToUpperInvariant()))
                {
                    var tmpSong = new modelSongs(Path.GetFileNameWithoutExtension(file), file,
                                        fileI.Length, tagFile.Tag.FirstAlbumArtist,
                                        DateTime.UtcNow, new TimeSpan(tagFile.Properties.Duration.Ticks), tagFile.Tag.Album);
                    tmpSong.pos = _position;
                    pl.Add(tmpSong);
                }
                else if (ImageExtensions.Contains(path.ToUpperInvariant()))
                {
                    var tmpImage = new modelPicture(Path.GetFileNameWithoutExtension(file), file,
                                        file.Length, tagFile.Tag.FirstAlbumArtist, DateTime.UtcNow);
                    tmpImage.pos = _position;
                    pl.Add(tmpImage);
                }
                else if (VideoExtensions.Contains(path.ToUpperInvariant()))
                {
                    var tmpVideo = new modelVideo(Path.GetFileNameWithoutExtension(file), file,
                                        file.Length, tagFile.Tag.FirstAlbumArtist,
                                        DateTime.UtcNow, new TimeSpan(tagFile.Properties.Duration.Ticks));
                    tmpVideo.pos = _position;
                    pl.Add(tmpVideo);
                }
                _position++;
            }
            catch (TagLib.UnsupportedFormatException)
            {
                MessageBox.Show("Ce format n'est pas pris en compte");
            }
        }

        /*
        ** Fonction pour mettre les elements dans la listbox Playlist
        */
        public void drawItem(ListBox playlist)
        {
            IEnumerable<generalModel> query = pl.OrderBy(x => x.pos);
            playlist.Items.Clear();
            foreach (generalModel element in query)
            {
                playlist.Items.Add(element.Title);
            }
        }

        /*
        ** Fonction pour mettre les elements dans la listbox Playlist (overload)
        */
        private void drawItem(ListBox playlist, IEnumerable<generalModel> list)
        {
            IEnumerable<generalModel> query = list.OrderBy(x => x.pos);
            playlist.Items.Clear();
            foreach (generalModel element in query)
            {
                playlist.Items.Add(element.Title);
            }
        }

        /*
        ** Fonction pour aller plus loin
        */
        public void LetsGoNext(Func<generalModel, bool> paul)
        {
            _playPos++;
            if (_playPos > _position)
                _playPos = 0;
            var query = pl.FirstOrDefault(i => i.pos == _playPos);
            if (query == null)
                LetsGoNext(paul);
            else
                paul(query);
        }

        /*
        ** Fonction pour revenir en arriere
        */
        public void LetsGoPrev(Func<generalModel, bool> paul)
        {
            _playPos--;
            if (_playPos < 0)
                _playPos = _position;
            var query = pl.FirstOrDefault(i => i.pos == _playPos);
            if (query == null)
                LetsGoPrev(paul);
            else
                paul(query);
        }

        /*
        ** Fonction pour faire la recherche
        */
        public void Search(String text, ListBox playlist)
        {
            var query = from value in pl where value.Title.ToUpper().Contains(text.ToUpper()) select value;
            drawItem(playlist, query);
        }

        /*
        ** Fonction pour supprimer un element de la playlist
        */
        public void delete(String text, ListBox playlist)
        {
            var query = pl.First(pl => pl.Title == text);
            pl.Remove(query);
            drawItem(playlist);
        }
    }
}
