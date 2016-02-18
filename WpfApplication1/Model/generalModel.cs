using System;
using System.Xml.Serialization;

namespace WpfApplication1.Model
{
    [XmlType(TypeName = "Model")]
    [XmlInclude(typeof(modelSongs))]
    [XmlInclude(typeof(modelPicture))]
    [XmlInclude(typeof(modelVideo))]
    [Serializable]
    public class generalModel
    {
        private string _title;
        private string _path;
        private long _size;
        private DateTime _added;
        private string _author;

        [XmlIgnore]
        public int pos { get; set; }
        [XmlElement("Title")]
        public String Title { get { return _title; } set { _title = value; } }
        [XmlElement("Path")]
        public String Path { get { return _path; } set { _path = value; } }
        [XmlElement("Size")]
        public long Size { get { return _size; } set { _size = value; } }
        [XmlElement("Add")]
        public DateTime Add { get { return _added; } set { _added = value; } }
        [XmlElement("Author")]
        public String Author { get { return _author; } set { _author = value; } }

        public generalModel()
        {
            _title = "";
            _path = "";
            _size = 0;
            _added = DateTime.MinValue;
            _author = "";
        }

        public generalModel(string title = "", string path = "", long size = 0,
                string author = "", DateTime added = default(DateTime))
        {
            _title = title;
            _path = path;
            _size = size;
            if (added == default(DateTime))
                _added = DateTime.UtcNow;
            else
                _added = added;
            _author = author;
        }
    }
}