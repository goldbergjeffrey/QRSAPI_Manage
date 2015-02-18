using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Qlik.Engine;
using Qlik.Sense.Client;
using Qlik.Sense.Client.Visualizations;
using System.Diagnostics;



namespace QRSAPI_Manage
{
    class QlikSdkDoStuff
    {
        ILocation senseSource = null;
        public List<AppInfo> appList = new List<AppInfo>();
        public List<string> imgList = new List<string>();
        public IAppIdentifier selectedApp;
        public IApp application;
        public string connectResult;
        public bool SenseDesktopConnect()
        {
            try
            {
                //check to see if Qlik Sense Desktop is running
                if (IsProcessOpen("QlikSense"))
                {
                    connectResult = "Qlik Sense Desktop is running" + System.Environment.NewLine;
                    senseSource = Qlik.Engine.Location.Local;
                    connectResult += "Connected to local Qlik Sense Desktop";
                    return true;
                }
                else
                {
                    connectResult = "Please launch Qlik Sense desktop.";
                    return false;
                }
            }
            catch(TypeLoadException ex)
            {
                connectResult = ex.Message;
                return false;
            }
        }

        public string getAppList()
        {
            string result="";
            appList.Clear();
            try
            {
                result = "AppList : {" + System.Environment.NewLine;
                foreach (IAppIdentifier appId in senseSource.GetAppIdentifiers())
                {
                    AppInfo app = new AppInfo();
                    app.id = appId.AppId;
                    app.name = appId.AppName;
                    app.meta = appId.Meta;
                    appList.Add(app);
                    result += app.id + " : " + app.name + ", " + System.Environment.NewLine;
                }
                result += "}";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string getApp(string strSelectedApp)
        {
            string result = "";
            try
            {
               selectedApp = senseSource.AppWithName(strSelectedApp);
                application = senseSource.App(selectedApp);
                result = "App: " + selectedApp.AppName + " loaded.";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public void getImageList()
        {
           // string result = "";
            imgList.Clear();
            getThumbnailImage();
            getAppImages();

            //return result;
        }

        private void getThumbnailImage()
        {
            NxAppLayout appLayout = application.GetAppLayout();
            try
            {
                Object thumb = appLayout.GetMember("thumbnail");
                if (! imgList.Contains(makeFilePath(thumb.ToString())))
                {
                    imgList.Add(makeFilePath(thumb.ToString()));
                }
               
            }
            catch (Exception e)
            {
                //placeholder
            }
        }

        private void getAppImages()
        {
            //get the sheets in the app
            IEnumerable<ISheet> sheets = application.GetSheets();
            foreach (ISheet sheet1 in sheets)
            {
                foreach (ITextImage child in sheet1.Children.OfType<ITextImage>())
                {
                    if (child.Markdown.Contains("image"))
                    {
                        int firstParen = child.Markdown.IndexOf("(");
                        int rightParen = child.Markdown.IndexOf(")") - 1;
                        string imagePath = child.Markdown.Substring(firstParen + 1, rightParen - firstParen);
                        imagePath = imagePath.Replace(@"\", "");
                        if(! imgList.Contains(makeFilePath(imagePath)))
                        {
                            imgList.Add(makeFilePath(imagePath));
                        }
                       
                    }
                    if (child.Background.Url != "")
                    {
                        if (!imgList.Contains(makeFilePath(child.Background.Url)))
                        {
                            imgList.Add(makeFilePath(child.Background.Url));
                        }
                    }
                }
            }
        }

        private string makeFilePath(string strUrlPath)
        {
            string tempString = strUrlPath;
            tempString = tempString.Replace("/", @"\");
            tempString = strDefaultContentFolder() + tempString;
            return tempString;
        }

        private string strDefaultContentFolder()
        {
            using (IHub hub = senseSource.Hub())
            {
                string appFolder = hub.GetDefaultAppFolder();
                string[] folderSplit = appFolder.Split('\\');
                string contentFolder ="";
                for (int i = 0; i < folderSplit.Length - 1; i++)
                {
                    contentFolder += folderSplit[i] + @"\";
                }
                return contentFolder.TrimEnd('\\');
                
            }
        }
    #region helperClasses
        public class AppInfo
        {
            public string id {get; set;}
            public string name { get; set; }
            public NxMeta meta { get; set; }
        }

        public bool IsProcessOpen(string name)
        {
            Process[] runningProcesses = Process.GetProcessesByName(name);
            if (runningProcesses.Length > 0)
            {
                return true;
            }
            return false;
        }


    #endregion

    }
}
