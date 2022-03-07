using System;
using Xamarin.Forms;

namespace Xamarin_DAW
{
    public partial class Xamarin_DAW
    {
        class TestUI
        {

            StackLayout createTestView()
            {
                StackLayout buttons = new StackLayout();
                buttons.Orientation = StackOrientation.Vertical;

                Button add_Button = new Button()
                {
                    Text = "add view"
                };

                Button remove_Button = new Button()
                {
                    Text = "remove view"
                };

                buttons.Children.Add(add_Button);
                buttons.Children.Add(remove_Button);

                Frame content = new Frame();

                add_Button.Clicked += (sender, eventArgs) =>
                {
                    content.Content = new Button()
                    {
                        Text = "I AM AN ADDED BUTTON"
                    };
                };

                remove_Button.Clicked += (sender, eventArgs) =>
                {
                    content.Content = null;
                };

                StackLayout screenContent = new StackLayout();
                screenContent.Orientation = StackOrientation.Vertical;
                screenContent.Children.Add(content);
                screenContent.Children.Add(buttons);
                return screenContent;
            }

            StackLayout createTestPluginView()
            {
                StackLayout buttons = new StackLayout();
                buttons.Orientation = StackOrientation.Vertical;

                Button add_Button = new Button()
                {
                    Text = "add plugin view"
                };

                Button remove_Button = new Button()
                {
                    Text = "remove plugin view"
                };

                buttons.Children.Add(add_Button);
                buttons.Children.Add(remove_Button);

                Frame content = new Frame();

                add_Button.Clicked += (sender, eventArgs) =>
                {
                    string PLUGIN_PATH;
                    if (Plugin.Is_Android)
                    {
                        PLUGIN_PATH = "/data/local/tmp/Xamarin_DAW__Test_Plugin.dll";
                    }
                    else
                    {
                        PLUGIN_PATH = "/Users/smallville7123/Projects/Xamarin_DAW__Test_Plugin/Xamarin_DAW__Test_Plugin/bin/Debug/netstandard2.1/Xamarin_DAW__Test_Plugin.dll";
                    }
                    View loaded = pluginManager.LoadFile(PLUGIN_PATH);
                    if (loaded != null)
                    {
                        content.Content = loaded;
                        return;
                    }
                    pluginManager.updatePluginList();

                    if (pluginManager.hasPlugins())
                    {
                        if (pluginManager.hasLoadedPlugins())
                        {
                            content.Content = pluginManager.getLoadedPluginByIndex(0).onCreateView();
                        }
                        else
                        {
                            content.Content = pluginManager.instantiatePluginByIndex(0).onCreateView();
                        }
                    }
                    else
                    {
                        content.Content = PluginManager.VIEW__NO_PLUGINS_FOUND;
                    }
                };

                remove_Button.Clicked += (sender, eventArgs) =>
                {
                    content.Content = null;
                };

                StackLayout screenContent = new StackLayout();
                screenContent.Orientation = StackOrientation.Vertical;
                FlexLayout screenContent_ = new();
                screenContent_.Direction = FlexDirection.Column;
                screenContent_.AlignItems = FlexAlignItems.Center;
                screenContent_.JustifyContent = FlexJustify.SpaceEvenly;
                screenContent_.Children.Add(content);
                screenContent_.Children.Add(buttons);
                screenContent.Children.Add(screenContent_);
                return screenContent;
            }

            internal static View createUI()
            {
                TestUI ui = new();
                FlexLayout screenContent = new();
                screenContent.Direction = FlexDirection.Column;
                screenContent.AlignItems = FlexAlignItems.Center;
                screenContent.JustifyContent = FlexJustify.SpaceEvenly;

                screenContent.Children.Add(ui.createTestView());
                screenContent.Children.Add(ui.createTestPluginView());
                return screenContent;
            }
        }

        public void hasStoragePermission(bool v)
        {
            pluginManager.hasStoragePermission(v);
        }
    }
}
