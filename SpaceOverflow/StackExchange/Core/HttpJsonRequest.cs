using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json.Linq;
using System.Net.Cache;
using System.IO.Compression;
using System.IO;
using System.Diagnostics;

namespace StackExchange
{
    public class HttpJsonRequest : IAsyncOperationProvider<JObject>
    {
        public HttpJsonRequest(Uri uri) {
            this.Uri = uri;
        }

        public Uri Uri { get; set; }
        protected HttpWebRequest PendingRequest;

        protected HttpWebRequest BuildRequest() {
            var request = (HttpWebRequest)WebRequest.Create(this.Uri);
            request.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
            request.Timeout = 10000;
            return request;
        }

        protected JObject ReceiveResponse(HttpWebResponse response) {
            var responseStream = response.GetResponseStream();

            if (response.ContentEncoding.ToLower().Contains("gzip"))
                responseStream = new GZipStream(responseStream, CompressionMode.Decompress);
            else if (response.ContentEncoding.ToLower().Contains("deflate"))
                responseStream = new DeflateStream(responseStream, CompressionMode.Decompress);

            var reader = new StreamReader(responseStream);
            var json = reader.ReadToEnd();
            reader.Close();
            response.Close();

            return JObject.Parse(json);
        }

        #region IAsyncOperationProvider<JObject> Member

        public void Begin(Action<JObject> success, Action<Exception> error) {
            this.Abort();

            Debug.Print("-- Request to " + this.Uri + " started");

            try {
                this.PendingRequest = this.BuildRequest();
                this.PendingRequest.BeginGetResponse(new AsyncCallback(result => {
                    try {
                        var httpResponse = (HttpWebResponse)this.PendingRequest.EndGetResponse(result);
                        Debug.Print("-- Got response for " + this.Uri);
                        var response = this.ReceiveResponse(httpResponse);
                        success(response);
                        Debug.Print("-- Response from " + this.Uri + " received");
                    }
                    catch (Exception ex) {
                        error(ex);
                    }
                    finally {
                        this.PendingRequest = null;
                    }
                }), null);
            }
            catch (Exception ex) {
                error(ex);
                this.PendingRequest = null;
            }
        }

        public bool IsRunning {
            get { return this.PendingRequest != null; }
        }

        public void Abort() {
            if (this.PendingRequest != null) {
                this.PendingRequest.Abort();
                Debug.Print("-- Aborted request to " + this.Uri);
            }
        }

        #endregion
    }
}
