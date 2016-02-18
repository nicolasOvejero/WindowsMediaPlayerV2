using System;
using System.Xml.Serialization;

namespace WpfApplication1.Model
{
    [XmlType(TypeName = "Image")]
    [Serializable]
    public class modelPicture : generalModel
    {
            public modelPicture() { }
            public modelPicture(string title = "", string path = "", long size = 0, string author = "", DateTime added = default(DateTime))
                : base(title, path, size, author, added)
            { }
    }
}
