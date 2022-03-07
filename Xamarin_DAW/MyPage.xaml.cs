using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin_DAW
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyPage : ContentPage
    {
        public MyPage()
        {
            InitializeComponent();
            //FileView.WidthRequest = 140;
            if (Plugin.Is_Android)
            {
                //FileView.FontSize = 40;
            }
            else
            {
                //FileView.FontSize = 20;
            }
        }
    }
}
