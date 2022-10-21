using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestVSSelector.Models;

namespace VSSwitcher.Services
{
    public class SolutionParser
    {
        private const string vsVersionSearchPattern = "# Visual Studio";
        private const string slnVersionSearchPattern = "Microsoft Visual Studio Solution File, Format Version";

        public string SolutionPath { get; }
        public VSVersion VisualStudioVersion { get; }
        public int SolutionVersion { get; }

        public SolutionParser(string solutionPath)
        {
            SolutionPath = solutionPath;

            var lines = File.ReadAllLines(SolutionPath).ToList();

            var lineVSVersion = lines.Find(s => s.Contains(vsVersionSearchPattern));
            var lineSLNVersion = lines.Find(s => s.Contains(slnVersionSearchPattern));

            var strVSVersion = lineVSVersion.Replace(vsVersionSearchPattern, "").Trim();
            if (strVSVersion.Contains("Version"))
                strVSVersion = strVSVersion.Replace("Version", "").Trim();

            var strSLNVersion = lineSLNVersion.Replace(slnVersionSearchPattern, "").Trim();

            SolutionVersion = (int)Math.Truncate(double.Parse(strSLNVersion));

            int vsVersion = int.Parse(strVSVersion);
            switch (SolutionVersion)
            {
                case 10:
                    VisualStudioVersion = VSVersion.VS2008;
                    break;
                case 11:
                    if (vsVersion == 2010)
                        VisualStudioVersion = VSVersion.VS2010;
                    else
                        VisualStudioVersion = VSVersion.VS2012;
                    break;
                case 12:
                    switch (vsVersion)
                    {
                        case 2013:
                            VisualStudioVersion = VSVersion.VS2013;
                            break;
                        case 14:
                            VisualStudioVersion = VSVersion.VS2015;
                            break;
                        case 15:
                            VisualStudioVersion = VSVersion.VS2017;
                            break;
                        case 16:
                            VisualStudioVersion = VSVersion.VS2019;
                            break;
                        case 17:
                            VisualStudioVersion = VSVersion.VS2022;
                            break;
                    }
                    break;
            }

        }
    }
}
