using System;
using System.Reflection;
using Xamarin.Forms;

namespace Xamarin_DAW
{
    public class Xamarin_DAW : Application
    {
        static View error(string message)
        {
            Label label = new Label();
            label.Text = message;
            return label;
        }

        public static View tryLoadLibrary(string path)
        {
            // Use the file name to load the assembly into the current
            // application domain.
            Assembly a;
            try
            {
                a = Assembly.LoadFile(path);
            }
            catch (Exception unused)
            {
                Console.WriteLine(unused);
                return error("failed to load Assembly File (" + unused.GetType().Name + ")\nFile: " + path);
            }

            if (a == null)
            {
                return error("failed to load Assembly File (Null)\nFile: " + path);
            }

            if (a.ReflectionOnly)
            {
                return error("Assembly File was loaded into a reflection-only context\nFile: " + path);
            }

            // Get the type to use.
            Type myType;
            try
            {
                myType = a.GetType("Xamarin_DAW__Test_Plugin.Class1");
            }
            catch (Exception unused)
            {
                Console.WriteLine(unused);
                return error("failed to get type from Assembly File (" + unused.GetType().Name + ")\nFile: " + path);
            }

            if (myType == null)
            {
                return error("failed to get type from Assembly File (Null)\nFile: " + path);
            }

            // Get the method to call.
            MethodInfo myMethod;
            try
            {
                myMethod = myType.GetMethod("onCreateView");
            }
            catch (Exception unused)
            {
                Console.WriteLine(unused);
                return error("failed to get method `onCreateView` from type (" + unused.GetType().Name + ")\nFile: " + path);
            }
            if (myMethod == null)
            {
                return error("failed to get method `onCreateView` from type (Null)\nFile: " + path);
            }

            // Create an instance.
            object obj;
            try
            {
                obj = Activator.CreateInstance(myType);
            }
            catch (Exception unused)
            {
                Console.WriteLine(unused);
                return error("failed to create instance of type (" + unused.GetType().Name + ")\nFile: " + path);
            }
            if (obj == null)
            {
                return error("failed to create instance of type (Null)\nFile: " + path);
            }

            // Execute the method.
            View view;
            try
            {
                view = (View)myMethod.Invoke(obj, null);
            }
            catch (Exception unused)
            {
                Console.WriteLine(unused);
                return error("failed to invoke method `onCreateView` (" + unused.GetType().Name + ")\nFile: " + path);
            }
            if (view == null)
            {
                return error("failed to invoke method `onCreateView` (Null)\nFile: " + path);
            }

            return view;
        }

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
                //View v = tryLoadLibrary("/Users/smallville7123/Projects/threeplatforms/test_plugin/bin/Debug/netstandard2.1/test_plugin.dll");
                View v = tryLoadLibrary("/Users/smallville7123/Projects/Xamarin_DAW/Xamarin_DAW__Test_Plugin/bin/Debug/netstandard2.1/Xamarin_DAW__Test_Plugin.dll");

                content.Content = v;
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

        public Xamarin_DAW()
        {
            //InitializeComponent();

            StackLayout screenContent = new StackLayout();
            screenContent.Orientation = StackOrientation.Vertical;

            screenContent.Children.Add(createTestView());
            screenContent.Children.Add(createTestPluginView());

            ContentPage screen = new ContentPage();
            screen.Content = screenContent;

            MainPage = screen;
        }

        private void Add_Button_Clicked(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
