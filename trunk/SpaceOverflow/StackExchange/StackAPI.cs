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
        public StackAPI(string host)
            : this("api." + host, host, "1.0")
        { }

        public StackAPI(string apiHost, string host, string version)
        {
            this.APIHost = apiHost;
            this.Host = host;
            this.Version = version;
        }

        public Uri BaseURI
        {
            get
            {
                var builder = new UriBuilder();
                builder.Scheme = "http";
                builder.Host = this.Host;
                return builder.Uri;
            }
        }

        public Uri APIBaseURI
        {
            get
            {
                var builder = new UriBuilder();
                builder.Scheme = "http";
                builder.Host = this.APIHost;
                builder.Path = this.Version + "/";
                return builder.Uri;
            }
        }

        public string Host { get; set; }
        public string APIHost { get; set; }
        public string Version { get; set; }
        public static string Key { get; set; }

        private static StackAPI _stackOverflow;
        public static StackAPI StackOverflow
        {
            get
            {
                if (StackAPI._stackOverflow == null)
                    StackAPI._stackOverflow = new StackAPI("stackoverflow.com");

                return StackAPI._stackOverflow;
            }
        }

        private static StackAPI _serverFault;
        public static StackAPI ServerFault
        {
            get
            {
                if (StackAPI._serverFault == null)
                    StackAPI._serverFault = new StackAPI("serverfault.com");

                return StackAPI._serverFault;
            }
        }

        private static StackAPI _superUser;
        public static StackAPI SuperUser {
            get {
                if (StackAPI._superUser == null)
                    StackAPI._superUser = new StackAPI("superuser.com");

                return StackAPI._superUser;
            }
        }

        private static StackAPI _meta;
        public static StackAPI Meta {
            get {
                if (StackAPI._meta == null)
                    StackAPI._meta = new StackAPI("meta.stackoverflow.com");

                return StackAPI._meta;
            }
        }

        private static StackAPI _stackApps;
        public static StackAPI StackApps {
            get {
                if (StackAPI._stackApps == null)
                    StackAPI._stackApps = new StackAPI("stackapps.com");

                return StackAPI._stackApps;
            }
        }
    }
}
