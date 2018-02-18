using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;

using NUnit.Framework;

namespace ndf5.tests
{
    [TestFixture]
    public class Loading
    {

        public static IEnumerable<string> AllExampleFiles
        {
            get{
                Assembly
                    fAssembly = Assembly.GetExecutingAssembly();
                string[]
                    fResources = fAssembly.GetManifestResourceNames();
                return fResources.Where(a => a.EndsWith(".h5", StringComparison.InvariantCulture));
            }
        }

        [Test, TestCaseSource(nameof(AllExampleFiles))]
        public void Test_basic_loading(string aFile)
        {
            Assembly
                fAssembly = Assembly.GetExecutingAssembly();

            using(Stream fTestStream = fAssembly.GetManifestResourceStream(aFile))
            {
                Hdf5File.Open(fTestStream).Dispose();
            }
        }
    }
}
