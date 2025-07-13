using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using RingingBloom.Common;
using System.IO;

namespace RingingBloom.WWiseTypes
{
    public class Labels
    {
        public IDictionary<ulong, string> wemLabels = new Dictionary<ulong, string>();
        //HIRC will get a dictionary too once implemented
        public Labels(XmlReader xml)
        {
            while (xml.Read())
            {
                if(xml.Name == "Wem")
                {
                    uint key = Convert.ToUInt32(xml.GetAttribute("Key"));
                    string value = xml.GetAttribute("Value");
                    wemLabels.Add(key, value);
                }
            }
            xml.Close();
        }
        
        public Labels()
        {
            //just need to say default exists
        }

        public void Export(string name, List<Wem> wems, List<ulong> ids)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            settings.OmitXmlDeclaration = true;
            //check and see if labels already exist
            if (File.Exists(name))
            {
                Labels oldLabel = new Labels(XmlReader.Create(name));
                wemLabels = oldLabel.wemLabels;
            }
            //now grab any new keys from the lists
            for(int i = 0; i < ids.Count; i++)
            {
                foreach(Wem wem in wems)
                {
                    if(wem.Id == ids[i])
                    {
                        //now check and see if it already exists in our labels
                        if (wemLabels.ContainsKey(ids[i]))
                        {
                            wemLabels[ids[i]] = wem.Name;
                        }
                        else
                        {
                            //add new key-value pair
                            wemLabels.Add(new KeyValuePair<ulong, string>(ids[i], wem.Name));
                        }
                    }
                }
            }
            Directory.CreateDirectory(Path.GetDirectoryName(name));
            XmlWriter xml = XmlWriter.Create(name,settings);
            xml.WriteStartElement("RingingBloomLabel");//was gonna have it be the filename, but you can't put numbers in element names apparently
            xml.WriteAttributeString("LabelVersion", "1.0");//not used for version checking or anything, but will be useful to diagnose if an issue is caused by outdated version
            foreach(KeyValuePair<ulong, string> kvp in wemLabels)
            {
                xml.WriteStartElement("Wem");
                xml.WriteAttributeString("Key", kvp.Key.ToString());
                xml.WriteAttributeString("Value", kvp.Value);
                xml.WriteEndElement();
            }
            xml.WriteEndElement();
            xml.Flush();
            xml.Close();
        }
    }
}
