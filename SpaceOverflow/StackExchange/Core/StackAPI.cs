using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;

namespace StackExchange
{
    public class StackAPI
    {
        public StackAPI(string name, Uri endpoint, Uri siteUri, APIState state)
        {
            this.Name = name;
            this.Endpoint = endpoint;
            this.SiteUri = siteUri;
            this.State = state;
        }

        public string Name { get; private set; }
        public Uri Endpoint { get; private set; }
        public Uri SiteUri { get; private set; }
        public APIState State { get; private set; }

        public Uri BaseUri {
            get {
                return new Uri(this.Endpoint, StackAPI.Version + "/");
            }
        }

        public static string Version { get { return "1.0"; } }
        public static string Key { get; set; }
    }

    public enum APIState
    {
        Normal,
        ClosedBeta,
        OpenBeta,
        LinkedMeta
    }
}
