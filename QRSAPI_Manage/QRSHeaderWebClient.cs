using System;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace QRSAPI_Manage
{
    public class QRSHeaderWebClient
    {
        private string serverUrl;
        private string vpHeaderName;
        private string vpHeaderVal;
        private readonly NameValueCollection _queryStringCollection;
        private readonly CookierAwareWebClient _QRSClient;

        public QRSHeaderWebClient(string QRSserverUrl, string QRSVpHeaderName, string QRSVpHeaderVal)
        {
            _QRSClient = new CookierAwareWebClient { Encoding = Encoding.UTF8 };
            //_QRSClient.UseDefaultCredentials = true;
          
            _queryStringCollection = new NameValueCollection { { "xrfkey", "ABCDEFG123456789" } };
            //ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            serverUrl = QRSserverUrl;
            vpHeaderName = QRSVpHeaderName;
            vpHeaderVal = QRSVpHeaderVal;

            //do a simple first GET to set cookies for subsequent actions (otherwise POST commands wont work)
            try
            {
                string resp = Get("/qrs/about");
            }
            catch (Exception ex)
            {
                throw new Exception("Couldnt connect to the server at " + QRSserverUrl + " , check that the Proxy and QRS are running.");
               
            }
        }

        public string Put(string endpoint, string content)
        {
            SetHeaders();
            NameValueCollection queryStringCollection = new NameValueCollection(_queryStringCollection);

            _QRSClient.QueryString = queryStringCollection;

            try
            {
                return _QRSClient.UploadString(serverUrl + endpoint, "Put", content);
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }

        public string Put(string endpoint, Dictionary<string, string> queries)
        {
            SetHeaders();
            NameValueCollection queryStringCollection = new NameValueCollection(_queryStringCollection);

            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                    queryStringCollection.Add(query.Key, query.Value);
            }

            _QRSClient.QueryString = queryStringCollection;

            try
            {
                return _QRSClient.UploadString(serverUrl + endpoint, "Put", "");
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }


        public byte[] PutFile(string endpoint, string filepath)
        {
            SetHeaders();
            _QRSClient.QueryString = _queryStringCollection;

            try
            {
                return _QRSClient.UploadFile(serverUrl + endpoint, "Put", filepath);
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }

        public string Post(string endpoint, string content)
        {
            return Post(endpoint, content, null);
        }

        public string Post(string endpoint, string content, Dictionary<string, string> queries)
        {
            SetHeaders();

            NameValueCollection queryStringCollection = new NameValueCollection(_queryStringCollection);

            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                    queryStringCollection.Add(query.Key, query.Value);
            }

            _QRSClient.QueryString = queryStringCollection;

            try
            {
                return _QRSClient.UploadString(serverUrl + endpoint, "Post", content);
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }

        public string PostFile(string endpoint, string filepath, Dictionary<string, string> queries)
        {
            SetHeaders();

            NameValueCollection queryStringCollection = new NameValueCollection(_queryStringCollection);

            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                    queryStringCollection.Add(query.Key, query.Value);
            }
            _QRSClient.QueryString = queryStringCollection;

            try
            {
                byte[] result = _QRSClient.UploadFile(serverUrl + endpoint, "Post", filepath);
                return Encoding.UTF8.GetString(result, 0, result.Length);
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }

        public string Delete(string endpoint)
        {
            SetHeaders();
            _QRSClient.QueryString = _queryStringCollection;

            try
            {
                return _QRSClient.UploadString(serverUrl + endpoint, "Delete", "");
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }

        public string Get(string url)
        {
            //SetHeaders();
            return Get(url, null);
        }



        public string Get(string endpoint, Dictionary<string, string> queries)
        {
            SetHeaders();
            NameValueCollection queryStringCollection = new NameValueCollection(_queryStringCollection);
            if (queries != null)
            {
                foreach (KeyValuePair<string, string> query in queries)
                    queryStringCollection.Add(query.Key, query.Value);
            }
            _QRSClient.QueryString = queryStringCollection;

            try
            {
                string response = _QRSClient.DownloadString(serverUrl + endpoint);
                return response;
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }

        public void GetFile(string endpoint, string filepath)
        {
            SetHeaders();

            NameValueCollection queryStringCollection = new NameValueCollection(_queryStringCollection);

            _QRSClient.QueryString = queryStringCollection;

            try
            {
                _QRSClient.DownloadFile(serverUrl + endpoint, filepath);
                return;
            }
            catch (WebException ex)
            {
                throw new Exception(ParseWebException(ex));
            }
        }



        private void SetHeaders()
        {
            _QRSClient.Headers.Clear();
            _QRSClient.Headers.Add("Accept-Charset", "utf-8");
            _QRSClient.Headers.Add("Accept", "application/json");
            _QRSClient.Headers.Add("Content-Type", "application/json");
            _QRSClient.Headers.Add("X-Qlik-xrfkey", "ABCDEFG123456789");
            _QRSClient.Headers.Add("X-QlikView-xrfkey", "ABCDEFG123456789");
            _QRSClient.Headers.Add(vpHeaderName, vpHeaderVal);

        }

        private static string ParseWebException(WebException exception)
        {
            if (exception.Status == WebExceptionStatus.ConnectFailure)
                return exception.Status + ": " + exception.Message;

            HttpWebResponse webResponse = (HttpWebResponse)exception.Response;
            Stream responseStream = webResponse.GetResponseStream();
            return webResponse.StatusDescription + (responseStream != null ? ": " + new StreamReader(responseStream).ReadToEnd() : string.Empty);
        }
    }

    public class CookierAwareWebClient : WebClient
    {
        public CookieContainer QRSCookieContainer = new CookieContainer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);
            request.CookieContainer = QRSCookieContainer;
            request.UserAgent = "Windows";
            return request;
        }
    }
}
