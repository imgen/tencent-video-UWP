using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.BackgroundTransfer;
using Windows.Networking.Sockets;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TVWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            Task.Run(() => { Class.WebClass.Initial(); });
            InitializeComponent();
#if phone
            //StatusBar.GetForCurrentView().BackgroundColor = Colors.Black;
            StatusBar s = StatusBar.GetForCurrentView();
            s.BackgroundOpacity = 1;
            s.ForegroundColor = Colors.White;
#endif
#if DEBUG
            Application.Current.UnhandledException += (o, e) => {
                Debug.WriteLine(e.Exception.StackTrace);
            };
#endif
            Application.Current.Suspending += (o, e) => {
                Class.Setting.SaveDispose();
                Class.DownLoad.SaveMission(null);
            };
            Class.Setting.LoadDispose();
            Class.Component.Initail();
            Class.Main.Initial(main);
            //Application.Current.EnteredBackground += (o, e) =>
            //{
            //    Debug.WriteLine("true");
            //}
            //ContentDialog cd = new ContentDialog();
            //await cd.ShowAsync();
            // MessageDialog

            //RegisterLiveTileTask();
            //string value = "\u65b0A\u7586\u548c\u7530\u5e02\u957f\u963f\u8fea\u529b-\u52aa\u5c14\u4e70\u4e70\u63d0\u6d89\u4e25\u91cd\u8fdd\u7eaa\u88ab\u67e5";
            //Debug.WriteLine(Uri.UnescapeDataString(value));//
        }
        bool reg=false;
        private  void RegisterLiveTileTask()
        {
            //foreach (var task in BackgroundTaskRegistration.AllTasks)
            //{
            //    if (task.Value.Name == "BackTask")
            //    {
            //        //task.Value.Unregister(true);
            //        reg = true;
            //        break;
            //    }
            //}
            //if (!reg)
            //{
            //    var builder = new BackgroundTaskBuilder();
            //    builder.Name = "BackTask";
            //    builder.TaskEntryPoint = typeof(BackTask.DownLoad).FullName;
            //    var tri = new TimeTrigger(15, false);
            //    builder.SetTrigger(tri);
            //    BackgroundTaskRegistration task = builder.Register();
            //    task.Progress += (o, e) => { };
            //    task.Completed += (o, e) => { };
            //}
        }
    }
}
