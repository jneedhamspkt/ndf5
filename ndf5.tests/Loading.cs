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

        [Test, TestCaseSource(nameof(AllExampleFiles))]
        public void Test_FileInfo_Loading(string aFile)
        {
            Assembly
                fAssembly = Assembly.GetExecutingAssembly();

            using (Stream fSource = fAssembly.GetManifestResourceStream(aFile))
            {
                //Write the File to a Temporary File and use FileInfo Arguments
                string
                fTestFileName = $"{Guid.NewGuid()}.h5";
                using(FileStream fTest = new FileStream(
                    fTestFileName,
                    FileMode.CreateNew,
                    FileAccess.ReadWrite, 
                    FileShare.ReadWrite,
                    64, 
                    FileOptions.DeleteOnClose ))
                {
                    int 
                        fLength = (int)fSource.Length;
                    byte[]
                        fFileBytes = new byte[fLength];
                    fSource.Read(fFileBytes, 0, fLength);
                    fTest.Write(fFileBytes, 0, fLength);
                    fTest.Flush();

                    Hdf5File.Open(new FileInfo(fTestFileName)).Dispose();
                }
            }
        }
    }
}
