using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using TestVSSelector.Models;
using VSSwitcher.Models;
using Path = System.IO.Path;

namespace VSSwitcher
{
    public partial class App : Application
    {
        private const string confFileName = "configuration.json";
        private const string appName = "Visual Studio Switcher";
        private const string confFolder = "Configuration";
        private readonly string confPath;

        public static string CurrFile { get; private set; }
        public static Configuration Configuration { get; private set; }

        public App()
        {
            var confDirPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName, confFolder);
            Directory.CreateDirectory(confDirPath);
            confPath = Path.Combine(confDirPath, confFileName);
            if (!File.Exists(confPath))
                File.Create(confPath);
            try
            {
                Configuration = JsonDocument.Parse(File.ReadAllText(confPath)).Deserialize<Configuration>();
            }
            catch (JsonException jex)
            {
                Configuration = new Configuration();
            }
            catch (Exception ex)
            {
                Configuration = new Configuration();
            }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            if (e.Args.Length == 1)
            {
                CurrFile = e.Args[0];
                if (Configuration.Solutions.Find(s => s.FilePath == CurrFile) != null)
                {
                    var sln = Configuration.Solutions.Find(s => s.FilePath == CurrFile);
                    var vsPath = Configuration.VSList.Find(v => v.VSID == sln.VSID).Path;
                    RunVisualStudio(vsPath, CurrFile, sln.UseAdminPermission);
                }
                else
                {
                    SwitcherWindow mainView = new SwitcherWindow();
                    mainView.Show();
                }
            }
            else
            {
                Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Configuration.VSList = Configuration.VSList.OrderByDescending(vs => vs.VSVersion).ToList();
            var json = JsonSerializer.Serialize(Configuration);
            File.WriteAllText(confPath, json);
        }

        public static void RunVisualStudio(string vsToRun, string fileToOpen, bool useAdminPerms = false)
        {
            try
            {
                fileToOpen = $"\"{fileToOpen}\"";
                vsToRun = $"\"{vsToRun}\"";

                Process.Start(new ProcessStartInfo
                {
                    FileName = vsToRun,
                    WindowStyle = ProcessWindowStyle.Normal,
                    Arguments = fileToOpen,
                    Verb = useAdminPerms ? "runas" : ""
                });
            }
            catch (Exception ex) { }
        }
    }
}