using System;
using System.Diagnostics;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;


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
            InitializeComponent();
#if phone
            ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
#endif
            Class.Main.Initial(main);
            //Debug.WriteLine( Uri.UnescapeDataString("http://v.qq.com/x/search/?ses=qid%3DdONGcy3g-hAMiTqN4ONQ4avmAGKrGb0IrTOPIn6mvZmj2c_9TilhDA%26last_query%3D%E7%BE%8E%E5%9B%BD%E9%98%9F%E9%95%BF3%26tabid_list%3D0%7C3%7C7%26tabname_list%3D%E5%85%A8%E9%83%A8%7C%E7%BB%BC%E8%89%BA%7C%E5%85%B6%E4%BB%96&q=%E7%BE%8E%E5%9B%BD%E9%98%9F%E9%95%BF3&stag=4&filter=sort%3D2%26pubfilter%3D0%26duration%3D1%26tabid%3D0"));
        }
    }
}
