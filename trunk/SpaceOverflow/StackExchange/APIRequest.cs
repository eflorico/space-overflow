using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace StackExchange
{
    public abstract class APIRequest<TResponse> : APIObject
    {
        public APIRequest(StackAPI api)
            : base(api)
        { }

        protected abstract string Route { get; }
        protected abstract Dictionary<string, string> Parameters { get; }

        protected Uri GetUri()
        {
            var parameters = this.Parameters;

            if (StackAPI.Key != null) parameters.Add("key", StackAPI.Key);
            
            var uri = this.API.APIBaseURI.ToString() + this.Route;
            uri += "?" + parameters.Aggregate("", (sum, item) => sum + Uri.EscapeDataString(item.Key) + "=" + Uri.EscapeDataString(item.Value) + "&");

            return new Uri(uri);
        }

        protected abstract TResponse ProcessResponse(JObject response);

        public TResponse GetResponse()
        {
            var stw = new Stopwatch();
            stw.Start();

            var request = (HttpWebRequest)WebRequest.Create(this.GetUri());
            request.CachePolicy = new System.Net.Cache.HttpRequestCachePolicy(System.Net.Cache.HttpRequestCacheLevel.NoCacheNoStore);
            //request.Timeout = 3000;

            var response = (HttpWebResponse)request.GetResponse();
            var responseStream = response.GetResponseStream();

            stw.Stop();
            Debug.Print("Got response in " + stw.Elapsed.ToString());
            stw.Reset();
            stw.Start();

            if (response.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);            

            var reader = new StreamReader(responseStream);
            var json = reader.ReadToEnd();
            reader.Close();
            response.Close();

            stw.Stop();
            Debug.Print("Parsed JSON in " + stw.Elapsed.ToString());
            stw.Reset();

            return this.ProcessResponse(JObject.Parse(json));
        }
    }
}
