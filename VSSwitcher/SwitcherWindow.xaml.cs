using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        private readonly SolutionParser parser;

        public SwitcherWindow()
        {
            InitializeComponent();
            App.Configuration.VSList = App.Configuration.VSList.OrderByDescending(vs => vs.VSVersion).ToList();
            VSList = new ObservableCollection<VisualStudio>();
            foreach (var vs in App.Configuration.VSList)
                VSList.Add(vs);

            ListShow.ItemsSource = VSList;

            parser = new SolutionParser(App.CurrFile);

            var usablevs = GetNearestVS(parser.VisualStudioVersion, VSList.ToList());
            ListShow.SelectedItem = usablevs;

            //if (parser.VisualStudioVersion < usablevs.VSVersion)
            //    MessageText.Text = $"This solution was built with {parser.VisualStudioVersion}, ";
            //else if (parser.VisualStudioVersion > usablevs.VSVersion)
            //    MessageText.Text = $"This solution was built with {parser.VisualStudioVersion} ";
            //else
            //    MessageText.Text = string.Empty;
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
            var selected = e.AddedItems[0] as VisualStudio;
            LauncherButtonText.Text = selected.Description;
            if (parser.VisualStudioVersion > selected.VSVersion)
                MessageText.Text = $"WARNING: opening solution for {parser.VisualStudioVersion} with {selected.VSVersion} may cause incompatibilities";
            else
                MessageText.Text = string.Empty;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => Close();

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LauncherButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedVs = ListShow.SelectedItem as VisualStudio;
            if (RegisterCheck.IsChecked ?? false)
            {
                Solution sln = new Solution
                {
                    FilePath = App.CurrFile,
                    UseAdminPermission = UseAdminCheck.IsChecked ?? false,
                    VSID = selectedVs.VSID
                };
                App.Configuration.Solutions.Add(sln);
            }
            App.RunVisualStudio(selectedVs.Path, App.CurrFile, UseAdminCheck.IsChecked ?? false);
            Close();
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