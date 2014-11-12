using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;


namespace QRSAPI_Manage
{
    class QRSDoStuff
    {
        QRSHeaderWebClient qrsClient;
        public List<stream> existingStreams = new List<stream>();
        public List<app> unpublishedApps = new List<app>();
        public List<app> allApps = new List<app>();
        public task createdTask = new task();

        public Boolean qrsConnected = false;

        public string connectQrs(string serverUrl, string vpHeaderName, string vpHeaderVal)
        {
            string result = "";
            qrsClient = new QRSHeaderWebClient(serverUrl, vpHeaderName, vpHeaderVal);
            try
            {
                result = qrsClient.Get("/qrs/about");
                qrsConnected = true;
                return result;
            }
            catch (Exception ex)
            {
                qrsConnected = false;
                result = ex.InnerException.ToString();
            }
            return result;
        }

        public string uploadApp(string fileNameWithPath)
        {
            
            string[] fileName = fileNameWithPath.Split('\\');
            string[] appName = fileName[fileName.Length - 1].Split('.');

            Dictionary<string, string> _params = new Dictionary<string,string>();
            _params.Add("name",appName[0]);

            string result;

            try
            {
                string postFileResult = qrsClient.PostFile("/qrs/app/upload", fileNameWithPath, _params);
                app appResult = JsonConvert.DeserializeObject<app>(postFileResult);
                result = JsonConvert.SerializeObject(appResult, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public string addContentLibrary(string libName, string libType)
        {
            Dictionary<string, string> _params = new Dictionary<string, string>();

            _params.Add("name", libName);
            _params.Add("type", libType);
            string result;

            try
            {
                string postFileResult = qrsClient.Post("/qrs/content", libName, _params);
                ContentLibrary contentLibResult = JsonConvert.DeserializeObject<ContentLibrary>(postFileResult);
                result = JsonConvert.SerializeObject(contentLibResult, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }

        public string getListOfStreams()
        {
            string result ="";
            try
            {
                string getRequestResult = qrsClient.Get("/qrs/stream");
                Newtonsoft.Json.Linq.JArray foo = Newtonsoft.Json.Linq.JArray.Parse(getRequestResult);

                for (int i = 0; i < foo.Count; i++)
                {
                    Newtonsoft.Json.Linq.JToken record = foo[i];
                    stream item = new stream();
                    item.ID = record["id"].ToString();
                    item.name = record["name"].ToString();
                    existingStreams.Add(item);
                    result += record.ToString() + System.Environment.NewLine;
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string getUnpublishedApps()
        {
            string result = "";
            Dictionary<string, string> filter = new Dictionary<string, string>();
            filter.Add("filter", "published eq false");
            try
            {
                string getRequestResult = qrsClient.Get("/qrs/app", filter);
                JArray foo = JArray.Parse(getRequestResult);
                for (int i = 0; i < foo.Count; i++)
                {
                    JToken record = foo[i];
                    app item = new app();
                    item.name = record["name"].ToString();
                    item.ID = record["id"].ToString();
                    unpublishedApps.Add(item);
                    result += record.ToString() + System.Environment.NewLine;
                }

            }
            catch (Exception ex)
            {
                result= ex.Message;
            }
            return result;
        }

        public string getAllApps()
        {
            string result = "";
            try
            {
                string getRequestResult = qrsClient.Get("/qrs/app");
                JArray foo = JArray.Parse(getRequestResult);
                foreach (JToken record in foo)
                {
                    app item = new app();
                    item.name = record["name"].ToString();
                    item.ID = record["id"].ToString();
                    allApps.Add(item);
                    result += record.ToString() + System.Environment.NewLine;
                }

            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public app getApp(string appID)
        {
            app resultApp = new app();
            string result = qrsClient.Get("/qrs/app/" + appID);
            resultApp = JsonConvert.DeserializeObject<app>(result);
            return resultApp;
        }

        public string publishApp(string appID, string streamID)
        {
            string result = "";
            Dictionary<string, string> _param = new Dictionary<string, string>();
            _param.Add("stream", streamID);
            try
            {
                string getRequestResult = qrsClient.Put("/qrs/app/" + appID + "/publish", _param);
                result = getRequestResult;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string createTask(string taskName, app app)
        {
            string result = "";
            task newtask = new task();
            newtask.app = app;
            newtask.enabled = true;
            newtask.name = "Refresh Data: " + taskName;
            newtask.taskSessionTimeout = 1440;
            newtask.maxRetries = 1;
            try
            {
                string taskresult = qrsClient.Post("/qrs/reloadtask",
                        JsonConvert.SerializeObject(newtask, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                result = taskresult;
                createdTask = JsonConvert.DeserializeObject<task>(taskresult);
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result; 
        }

        


        public string createTrigger(string taskName, app app, string refreshInterval, string schemaFilterDescription, 
            string incrementDescription, int incrementOption)
        {
            string result = "";

            task newtask = new task();
            newtask.app = app;
            newtask.enabled = true;
            newtask.name = "Refresh Data: " + taskName;
            newtask.taskSessionTimeout = 1440;
            newtask.maxRetries = 1;

            List<schemaEvent> schemaEvents = new List<schemaEvent>();
            List<string> schemaFilter = new List<string>();
            schemaFilter.Add(schemaFilterDescription);

            schemaEvent sEvent = new schemaEvent();
            sEvent.name = refreshInterval + " Refresh";
            sEvent.enabled = true;
            sEvent.eventType = 0;
            sEvent.reloadTask = newtask;
            sEvent.schemaFilterDescription = schemaFilter.ToArray();
            sEvent.incrementDescription = incrementDescription;
            sEvent.incrementOption = incrementOption;
            sEvent.modifiedDate = String.Format("{0:s}", DateTime.Now);
            schemaEvents.Add(sEvent);

            reloadTaskBundle taskBundle = new reloadTaskBundle();
            taskBundle.task = newtask;
            taskBundle.schemaEvents = schemaEvents.ToArray();

            string taskToAdd = JsonConvert.SerializeObject(taskBundle, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore});

            try
            {
                string taskResult = qrsClient.Post("/qrs/reloadTask/create", taskToAdd);
                result = taskResult;
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        #region Helper Functions


        #endregion

        #region JSONClasses

        public class stream
        {
            public string ID { get; set; }
            public string name { get; set; }
        }

        public class LoginLicenceType
        {
            public string ID { get; set; }
            public string name { get; set; }
            public string assignedTokens { get; set; }
        }

        public class dataconnection
        {
            public string ID { get; set; }
            public string name { get; set; }
            public string connectionstring { get; set; }
            public string type { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

        public class systemrule
        {
            public string name { get; set; }
            public string rule { get; set; }
            public string resourceFilter { get; set; }
            public string actions { get; set; }
            public string comment { get; set; }
            public string category { get; set; }
        }
        public class tag
        {
            public string ID { get; set; }
            public string name { get; set; }
        }

        public class CustomProperty
        {
            public string ID { get; set; }
            public string name { get; set; }
            public string valueType { get; set; }
            public string[] objecttypes { get; set; }
            public string[] choiceValues { get; set; }
        }

        public class user
        {
            public string Id { get; set; }
            public string userId { get; set; }
            public string userDirectory { get; set; }
            public string name { get; set; }
            public List<userattribute> attributes { get; set; }
            public List<string> roles { get; set; }
            public List<CustomPropertyApplied> customProperties { get; set; }
        }

        public class CustomPropertyApplied
        {
            public CustomProperty definition { get; set; }
            public string value { get; set; }
        }

        public class userattribute
        {
            public string attributeType { get; set; }
            public string attributeValue { get; set; }
            public string externalId { get; set; }
        }
        public class app
        {
            public string ID { get; set; }
            public string name { get; set; }
            public List<tag> tags { get; set; }
            public List<CustomProperty> customproperties { get; set; }
            public string modifiedDate { get; set; }
        }

        public class appobject
        {
            public string ID { get; set; }
            public string name { get; set; }
            public string objectType { get; set; }
            public string approved { get; set; }
            public app app { get; set; }
            public user owner { get; set; }
        }

        public class task
        {
            public string ID { get; set; }
            public string name { get; set; }
            public int taskType { get; set; }
            public bool enabled { get; set; }
            public app app { get; set; }
            public int taskSessionTimeout {get; set;}
            public int maxRetries {get; set;}
        }

        public class schemaEvent
        {
            public string ID {get; set;}
            public string name {get; set;}
            public bool enabled {get; set;}
            public int eventType {get; set;}
            public task reloadTask {get; set;}
            public string[] schemaFilterDescription {get; set;}
            public string incrementDescription {get; set;}
            public int incrementOption {get; set;}
            public string modifiedDate { get; set; }
        }

        public class reloadTaskBundle
        {
            public task task {get; set;}
            public schemaEvent[] schemaEvents {get; set;}
            public compositeEvent[] compositeEvents { get; set; }
        }

        public class compositeEvent
        {
            public string ID { get; set; }
            public string name { get; set; }
            public bool enabled { get; set; }
            public int eventType { get; set; }
            public timeConstraint timeConstraint { get; set; }
            public task reloadTask { get; set; }
            public compositeRule[] compositeRules { get; set; }
        }

        public class compositeRule
        {
            public string ruleState { get; set; }
            public task reloadTask { get; set; }
        }

        public class timeConstraint
        {
            public int days { get; set; }
            public int hours { get; set; }
            public int minutes { get; set; }
        }

        public class endpoint
        {
            public string method { get; set; }
            public string fullpath { get; set; }
            public string path { get; set; }
            public string options { get; set; }
            public string primaryobject { get; set; }
            public string jsontemplate { get; set; }
        }

        public class JSONObject
        {
            public string JSON { get; set; }
            public string fullpath { get; set; }
        }

        public class Proxy
        {
            public string ID { get; set; }
            public string name { get; set; }
            public string modifiedDate { get; set; }
            public ProxySettings Settings { get; set; }
        }

        public class ProxySettings
        {
            public List<VirtualProxy> virtualProxyConfigurations { get; set; }
        }

        public class VirtualProxy
        {
            public string prefix { get; set; }
            public string authenticationModuleRedirectUri { get; set; }
            public string sessionCookieHeaderName { get; set; }
            public bool defaultVirtualProxy { get; set; }
        }

        public class ContentLibrary
        {
            public string ID { get; set; }
            public string name { get; set; }
            public string type { get; set; }
        }

        #endregion
    }
}

