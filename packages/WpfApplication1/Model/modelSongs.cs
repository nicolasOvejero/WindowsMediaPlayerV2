using System;
using System.Xml.Serialization;

namespace WpfApplication1.Model
{
    [XmlType(TypeName = "Music")]
    [Serializable]
    public class modelSongs : generalModel
    {
        private TimeSpan _length;
        private string _album;

        [XmlElement("realLength")]
        public String RealLength { get { return _length.ToString("hh':'mm':'ss"); } set { _length = TimeSpan.Parse(value); } }
        [XmlElement("Album")]
        public string Album { get { return _album; } set { _album = value; } }
        [XmlIgnore]
        public TimeSpan Length { get { return _length; } set { _length = value; } }

        public modelSongs()
        { }

        public modelSongs(string title = "", string path = "", long size = 0, string author = "",
            DateTime added = default(DateTime), TimeSpan length = default(TimeSpan), string album = "")
            : base(title, path, size, author, added)
        {
            if (length == default(TimeSpan))
                length = TimeSpan.Zero;
            else
                _length = length;
            _album = album;
        }
    }
}
