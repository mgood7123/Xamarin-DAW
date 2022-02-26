using Xamarin.Forms;

namespace Xamarin_DAW__Test_Plugin
{
    public class Class1 : Xamarin_DAW.Plugin
    {
        public override View onCreateView()
        {
            return new Button
            {
                Text = "I AM A PLUGIN BUTTON!!!"
            };
        }
    }
}
