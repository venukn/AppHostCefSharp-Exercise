using System;
using System.Windows.Controls;
using RedGate.AppHost.Server;

namespace AppHostCefSharp
{
    public partial class BrowserWindow
    {
        private readonly BrowserServiceLocator locator;

        public BrowserWindow()
            : this("chrome://version", null, null)
        { }

        public BrowserWindow(string url, IPersistGeometry geometry)
            : this(url, geometry, null)
        { }

        public BrowserWindow(string url, IPersistGeometry geometry, string appDataPath)
        {
            InitializeComponent();
            try
            {
                geometry?.Restore(this);

                var safeAppHostChildHandle = new ChildProcessFactory()
                    .Create("AppHostCefSharp.WebBrowser.dll");

                locator = new BrowserServiceLocator(url, appDataPath);
                               
                var mainContent = safeAppHostChildHandle.CreateElement(locator);                
                //Content = safeAppHostChildHandle.CreateElement(locator);                
                stPanel.Content = mainContent;
              
                Closing += (sender, args) =>
                {
                    locator.Send("Close");
                    geometry?.Persist(this);
                };
            }
            catch (Exception ex)
            {
                stPanel.Content = new TextBlock { Text = ex.ToString() };
            }
        }

        public void Send(string msg)
        {
            locator.Send(msg);
        }
    }
}
