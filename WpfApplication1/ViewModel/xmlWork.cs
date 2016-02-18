using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;
using WpfApplication1.Model;
using WpfApplication1.View;

namespace WpfApplication1.ViewModel
{
    class xmlWork
    {
        public void createXML(Playlist PlayList)
        {
            PopUp pop = new PopUp();
            if (pop.ShowDialog() == true)
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(Playlist));
                    String path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\myWMPv2\\";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    using (StreamWriter wr = new StreamWriter(path + pop.ResponseText))
                    {
                        xs.Serialize(wr, PlayList);
                    }
                }
                catch (InvalidOperationException)
                {
                    MessageBox.Show("La création a échouée");
                }
            }
        }

        public Playlist createFromXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Playlist));
            Playlist FirstPlayList = new Playlist();

            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.AddExtension = true;
                ofd.Filter = "All Files (*.xml) | *.xml";
                ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\myWMPv2\\";
                bool? result = ofd.ShowDialog();
                if (result ?? true)
                {
                    StreamReader reader = new StreamReader(ofd.FileName);
                    FirstPlayList._position = 0;
                    FirstPlayList = serializer.Deserialize(reader) as Playlist;
                    int i = 0;
                    foreach (generalModel lp in FirstPlayList.pl)
                    {
                        lp.pos = i;
                        FirstPlayList._position = i;
                        i++;
                    }
                    reader.Close();
                }
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Le fichier XML est corrompu, merci de ne pas jouer avec !");
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Le fichier XML est corrompu, merci de ne pas jouer avec !");
            }
            return FirstPlayList;
        }
    }
}
