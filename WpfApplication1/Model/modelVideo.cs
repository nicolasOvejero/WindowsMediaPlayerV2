using System;
using System.Xml.Serialization;

namespace WpfApplication1.Model
{
    [XmlType(TypeName = "Video")]
    [Serializable]
    public class modelVideo : generalModel
    {
        private TimeSpan _length;

        [XmlElement("realLength")]
        public string realLength { get { return _length.ToString("hh':'mm':'ss"); } }
        [XmlElement("Length")]
        public TimeSpan Length { get { return _length; } set { _length = value; } }

        public modelVideo()
        { }

        public modelVideo(string title = "", string path = "", long size = 0,
            string author = "", DateTime added = default(DateTime), TimeSpan length = default(TimeSpan))
            : base(title, path, size, author, added)
        {
            if (length == default(TimeSpan))
                length = TimeSpan.Zero;
            else
            {
                _length = length;
            }
        }
    }
}
