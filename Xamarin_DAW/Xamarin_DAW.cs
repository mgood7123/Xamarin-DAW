using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SkiaSharp;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Xamarin_DAW
{
    public partial class Xamarin_DAW : Application
    {

        public void setDensity(double density)
        {
            Plugin.XAMARIN_DAW_INTERNAL_USE_ONLY_SCREEN_DENSITY_SETTER = density;
        }

        class PluginManager
        {
            IEnumerable<Type> plugins;
            List<Plugin> loadedPlugins;

            public static readonly View VIEW__NO_PLUGINS_FOUND = error("no plugins found", false);

            class Error<R>
            {
                public bool error;
                public R result;
                public View errorView;
                public Error(R r)
                {
                    error = false;
                    result = r;
                }
                public Error(View v)
                {
                    error = true;
                    errorView = v;
                }
            }

            static View error(string msg, bool write_to_console = true)
            {
                if (write_to_console) Console.WriteLine(msg);
                Label label = new Label();
                label.Text = msg;
                return label;
            }

            Error<R> tryAction<R>(Func<R> action, string path, string message)
            {
                R result;
                try
                {
                    result = action();
                }
                catch (Exception exception)
                {
                    string msg = message + " (" + exception.GetType().Name + ")\n\n" +
                        "File: " + path;

                    Console.WriteLine(msg + "\n\n" +
                        "Exception: " + exception);
                    Label label = new Label();
                    label.Text = msg;
                    return new(label);
                }
                if (result == null)
                {
                    string msg = message + " ( null return value )\n\n" +
                        "File: " + path;

                    return new(error(msg));
                }
                return new(result);
            }

            static void PrintArray<T>(Func<T[]> action, string name, Func<T, string> toString)
            {
                try
                {
                    T[] array = action();
                    if (array != null && array.Length > 0)
                    {
                        Console.WriteLine("    " + name + ":");
                        array.ForEach((value) =>
                        {
                            Console.WriteLine("        " + toString(value));
                        });
                    }
                    else
                    {
                        Console.WriteLine("    " + name + ": None");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("    " + name + ": None (" + exception.GetType().Name + ")");
                }
            }

            static void PrintAssemblyInfo(Assembly assembly)
            {
                Console.WriteLine("Assembly name: " + assembly.GetName());
                PrintArray(() => assembly.GetFiles(), "Assembly files", value => value.Name);
                PrintArray(() => assembly.GetReferencedAssemblies(), "Assembly references", value => value.Name);
                PrintArray(() => assembly.GetModules(), "Assembly modules", value => value.Name);
                PrintArray(() => assembly.GetLoadedModules(), "Assembly modules (Loaded)", value => value.Name);
                PrintArray(() => assembly.GetTypes(), "Assembly types", value => value.Name);
                PrintArray(() => assembly.GetExportedTypes(), "Assembly types (Exported)", value => value.Name);
            }

            Error<Assembly> LoadReferencedAssemblies(string path, int depth = 0)
            {
                string space1 = "";
                string space2 = "    ";
                string space3 = "        ";
                for (int i = 0; i < depth * 3; i++)
                {
                    space1 += "    ";
                    space2 += "    ";
                    space3 += "    ";
                }

                Assembly assembly;

                try
                {
                    assembly = Assembly.LoadFile(path);
                }
                catch (Exception exception)
                {
                    return new(error(space1 + "failed to load declared referenced Assembly File (referenced from " + path + ")"));
                }
                if (assembly == null)
                {
                    return new(error(space1 + "failed to load declared referenced Assembly File (referenced from " + path + ")"));
                }

                if (assembly.ReflectionOnly)
                {
                    return new(error(space1 + "Assembly File was loaded into a reflection-only context\n\nFile: " + assembly.Location));
                }
                Console.WriteLine(space1 + "Assembly name: " + assembly.GetName().Name);
                Console.WriteLine(space2 + "Assembly location:");
                Console.WriteLine(space3 + assembly.Location);
                try
                {
                    var array1 = assembly.GetFiles();
                    if (array1 != null && array1.Length > 0)
                    {
                        Console.WriteLine(space2 + "Assembly files:");
                        array1.ForEach((value) =>
                        {
                            Console.WriteLine(space3 + value.Name);
                        });
                    }
                    else
                    {
                        Console.WriteLine(space2 + "Assembly files: None");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine(space2 + "Assembly files: None (" + exception.GetType().Name + ")");
                }
                Console.Write(space2 + "Assembly references:");

                string dir = Path.GetDirectoryName(assembly.Location);
                var array = assembly.GetReferencedAssemblies();
                if (array != null && array.Length > 0)
                {
                    Console.Write("\n");
                    foreach (var value in array) {
                        Console.Write(space3 + value.Name);
                        var loaded_assemblies = AppDomain.CurrentDomain.GetAssemblies();
                        if (loaded_assemblies != null && loaded_assemblies.Length > 0)
                        {
                            bool shouldSkip = false;
                            foreach (var assembly1 in loaded_assemblies)
                            {
                                if (AssemblyName.ReferenceMatchesDefinition(assembly1.GetName(), value))
                                {
                                    shouldSkip = true;
                                    break;
                                }
                            }
                            if (shouldSkip)
                            {
                                Console.WriteLine(" (loaded)");
                                continue;
                            }
                            else
                            {
                                Console.WriteLine(" (loading)");
                            }
                        }
                        else
                        {
                            Console.WriteLine(" (loading)");
                        }

                        string path1 = dir + '/' + value.Name + ".dll";

                        var e = LoadReferencedAssemblies(path1, depth + 1);
                        if (e.error) return e;
                    };
                }
                else
                {
                    Console.WriteLine(" None");
                }
                return new(assembly);
            }

            internal void hasStoragePermission(bool v)
            {
            }

            public void updatePluginList()
            {
                var type = typeof(Plugin);
                plugins = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.IsClass && !p.IsInterface && !p.IsAbstract && type.IsAssignableFrom(p));
            }

            public void printPluginList()
            {
                if (hasPlugins())
                {
                    Console.WriteLine("Plugins:");

                    foreach (Type plugin in plugins)
                    {
                        Console.WriteLine("    " + plugin.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Plugins: None");
                }
            }

            public void printLoadedPluginList()
            {
                if (hasLoadedPlugins())
                {
                    Console.WriteLine("Loaded Plugins:");

                    foreach (Plugin plugin in loadedPlugins)
                    {
                        Console.WriteLine("    " + plugin.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("Loaded Plugins: None");
                }
            }

            public View LoadFile(string path)
            {
                var e = LoadReferencedAssemblies(path);
                if (e.error) return e.errorView;
                return null;
            }

            public bool hasPlugins()
            {
                return plugins != null && plugins.Count() > 0;
            }

            public bool hasLoadedPlugins()
            {
                return loadedPlugins != null && loadedPlugins.Count > 0;
            }

            public Type getPluginTypeByIndex(int index)
            {
                if (plugins == null) return null;
                try
                {
                    return plugins.ElementAt(index);
                }
                catch (Exception unused)
                {
                    return null;
                }
            }

            public Plugin getLoadedPluginByIndex(int index)
            {
                if (loadedPlugins == null) return null;
                try
                {
                    return loadedPlugins.ElementAt(index);
                }
                catch (Exception unused)
                {
                    return null;
                }
            }

            public Plugin instantiatePlugin(Type plugin)
            {
                if (plugin == null)
                {
                    return null;
                }

                Plugin plugin_ = (Plugin)Activator.CreateInstance(plugin);
                if (loadedPlugins == null) loadedPlugins = new();
                loadedPlugins.Add(plugin_);
                return plugin_;
            }

            public Plugin instantiatePluginByIndex(int index)
            {
                return instantiatePlugin(getPluginTypeByIndex(index));
            }
        }

        static PluginManager pluginManager = new();

        Skia_UI_Kit.Application application;

        class MyApp : Skia_UI_Kit.Application
        {
            public override void OnCreate()
            {

                var box_a = new Skia_UI_Kit.Box()
                {
                    mLayoutParams = new()
                    {
                        width = 40,
                        height = 40
                    },
                    paint = new SKPaint() { Color = new SKColor(50, 50, 50) }
                };

                //Skia_UI_Kit.LinearLayout linearLayout = new();

                //linearLayout.setOrientation(Skia_UI_Kit.LinearLayout.VERTICAL);

                //linearLayout.addView();
                //linearLayout.addView(new Skia_UI_Kit.Box()
                //{
                //    mLayoutParams = new()
                //    {
                //        width = 40,
                //        height = 40
                //    },
                //    paint = new SKPaint() { Color = new SKColor(50, 50, 178) }
                //});
                SetContentView(box_a);
            }
        }

        protected override void OnStart()
        {
            application = new MyApp();

            // Handle when your app starts
            AppDomain test = AppDomain.CreateDomain("Test");
            //Console.WriteLine("created test domain: " + test);
            AppDomain.Unload(test);
            //Console.WriteLine("unloaded test domain");

            //ContentPage screen = new ContentPage();
            //screen.Content = TestUI.createUI();
            //MainPage = screen;

            //MainPage = new MyPage();
            MainPage = new MyPage()
            {
                Content = new Skia_UI_Kit.SkiaViewHost
                {
                    Application = application
                }
            };
            application.OnCreate();
        }

        protected override void OnSleep()
        {
            application.OnPause();
        }

        protected override void OnResume()
        {
            application.OnResume();
        }
    }
}
