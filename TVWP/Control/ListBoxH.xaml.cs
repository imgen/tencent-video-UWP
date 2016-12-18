using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace TVWP.Control
{
    public sealed partial class ListBoxH : UserControl
    {
        public ListBoxH()
        {
            this.InitializeComponent();
        }
        public ListBox GetListBox { get { return lb; }}
    }
}
