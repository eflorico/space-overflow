using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using StackExchange;

namespace SpaceOverflow
{
    public static class Config
    {
        static Config() {
            if (!File.Exists(Path)) {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(Path)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(Path));

                Document = new XDocument(
                    new XElement("config",
                        new XElement("sitecache")
                    )
                );
                SiteCache = new List<StackAPI>();
                Document.Save(Path);
            }
            else {
                Document = XDocument.Load(Path);
                SiteCache = (from i in Document.Root.Element("sitecache").Elements()
                             select new StackAPI(i.Attribute("name").Value,
                                 new Uri(i.Attribute("endpoint").Value),
                                 new Uri(i.Attribute("site_uri").Value),
                                 APIState.Normal)).ToList();
            }
        }

        public static XDocument Document { get; private set; }

        public static void Save() {
            Document.Root.Element("sitecache").Descendants().Remove();

            foreach (var site in SiteCache) {
                Document.Root.Element("sitecache").Add(new XElement("site", 
                    new XAttribute("name", site.Name), 
                    new XAttribute("endpoint", site.Endpoint), 
                    new XAttribute("site_uri", site.SiteUri)));   
            }

            Document.Save(Path);
        }

        public static List<StackAPI> SiteCache { get; private set; }

        public static string Path {
            get {
                return System.IO.Path.Combine(System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "SpaceOverflow"),
                    "config.xml");
            }
        }
    }
}
