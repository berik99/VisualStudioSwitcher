using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Windows;
using System.Windows.Controls;
using TestVSSelector.Models;
using VSSwitcher.Models;
using VSSwitcher.Services;

namespace VSSwitcher
{
    public partial class SwitcherWindow : Window
    {
        public ObservableCollection<VisualStudio> VSList { get; set; }

        private string CurrFile { get; set; }
        private SolutionParser Parser { get; set; }

        public SwitcherWindow()
        {
            InitializeComponent();
            AppName.Text = Title;
            Application.Configuration.VSList = Application.Configuration.VSList.OrderByDescending(vs => vs.VSVersion).ToList();
            VSList = new ObservableCollection<VisualStudio>();
            foreach (var vs in Application.Configuration.VSList)
                VSList.Add(vs);
            ListShow.ItemsSource = VSList;

            CurrFile = Application.CurrFile;
            if (!string.IsNullOrEmpty(CurrFile))
            {
                Parser = new SolutionParser(CurrFile);
                var usablevs = GetNearestVS(Parser.VisualStudioVersion, VSList.ToList());
                ListShow.SelectedItem = usablevs;
                SolutionName.Text = Path.GetFileName(CurrFile);
                SolutionName.Visibility = Visibility.Visible;
                SolutionPicker.Visibility = Visibility.Hidden;
            }
            else
            {
                SolutionName.Visibility = Visibility.Hidden;
                SolutionPicker.Visibility = Visibility.Visible;
            }
        }

        private VisualStudio GetNearestVS(VSVersion targetVersion, List<VisualStudio> list)
        {
            var equal = list.Find(vs => vs.VSVersion == targetVersion);
            if (equal != null)
                return equal;

            var rightClosest = list.Where(vs => vs.VSVersion >= targetVersion).OrderBy(vs => vs.VSVersion).First();
            if (rightClosest != null)
                return rightClosest;

            var leftClosest = list.First(vs => vs.VSVersion <= targetVersion);
            return leftClosest;
        }

        private void ListShow_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Parser != null)
            {
                var selected = e.AddedItems[0] as VisualStudio;
                LauncherButtonText.Text = selected.Description;
                if (Parser.VisualStudioVersion > selected.VSVersion)
                    MessageText.Text = $"WARNING: opening solution for {Parser.VisualStudioVersion} with {selected.VSVersion} may cause incompatibilities";
                else
                    MessageText.Text = string.Empty;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settings = new SettingsWindow();
            settings.ShowDialog();
        }

        private void LauncherButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedVs = ListShow.SelectedItem as VisualStudio;
            if (RegisterCheck.IsChecked ?? false)
            {
                bool saveConfig = !string.IsNullOrEmpty(CurrFile);
                var existingSln = Application.Configuration.Solutions.Find(s => s.FilePath == CurrFile);
                if (existingSln != null)
                {
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("This solution is already registered. Overwrite it?", "Solution already registered", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        Application.Configuration.Solutions.Remove(existingSln);
                    }
                    else
                    {
                        saveConfig = false;
                    }
                }

                if (saveConfig)
                {
                    Solution sln = new Solution
                    {
                        FilePath = CurrFile,
                        UseAdminPermission = UseAdminCheck.IsChecked ?? false,
                        VSID = selectedVs.VSID
                    };
                    Application.Configuration.Solutions.Add(sln);
                }
            }
            Application.RunVisualStudio(selectedVs.Path, CurrFile, UseAdminCheck.IsChecked ?? false);
            Close();
        }

        private void SolutionPicker_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Visual Studio Solution (*.sln)|*.sln"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                CurrFile = openFileDialog.FileName;

                var slnInList = Application.Configuration.Solutions.Find(sln => sln.FilePath == CurrFile);
                if (slnInList != null)
                {
                    var defaultVs = VSList.First(vs => vs.VSID == slnInList.VSID);
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show($"This solution is already registered with {slnInList.VSID}. Open with configuration or continue with dialog?", "Solution already registered", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        Application.RunVisualStudio(defaultVs.Path, CurrFile, slnInList.UseAdminPermission);
                        Close();
                        return;
                    }


                }

                Parser = new SolutionParser(CurrFile);
                var usablevs = GetNearestVS(Parser.VisualStudioVersion, VSList.ToList());
                ListShow.SelectedItem = usablevs;
                SolutionName.Text = Path.GetFileName(CurrFile);
                SolutionName.Visibility = Visibility.Visible;
                SolutionPicker.Visibility = Visibility.Hidden;
            }
        }
    }
}

//App.Configuration.VSList = new List<VisualStudio>
//{
//    new VisualStudio
//    {
//        VSID = "VS2022",
//        Description = "Visual Studio 2022",
//        VSVersion = VSVersion.VS2022,
//        Path = @"I:\Visual Studio\VS2022\Common7\IDE\devenv.exe"
//    },
//    new VisualStudio
//    {
//        VSID = "VS2019",
//        Description = "Visual Studio 2019",
//        VSVersion = VSVersion.VS2019,
//        Path = @"I:\Visual Studio\VS2019\Common7\IDE\devenv.exe"
//    },
//    new VisualStudio
//    {
//        VSID = "VS2012",
//        Description = "Visual Studio 2012",
//        VSVersion = VSVersion.VS2012,
//        Path = @"I:\Visual Studio\VS2012\Common7\IDE\devenv.exe"
//    },
//    new VisualStudio
//    {
//        VSID = "VS2008",
//        Description = "Visual Studio 2008",
//        VSVersion = VSVersion.VS2008,
//        Path = @"I:\Visual Studio\VS2008\Common7\IDE\devenv.exe"
//    },
//};