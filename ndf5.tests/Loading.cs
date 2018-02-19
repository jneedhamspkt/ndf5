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

            using (Stream fTestStream = fAssembly.GetManifestResourceStream(aFile))
            {
                using (Hdf5File fTest = Hdf5File.Open(fTestStream))
                {
                    Assert.That(() => fTest.SuperBlock.ToString(), Throws.Nothing);
                }
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

                    using (Hdf5File fTestFile = Hdf5File.Open(new FileInfo(fTestFileName)))
                    {
                        Assert.That(() => fTestFile.SuperBlock.ToString(), Throws.Nothing);
                    }
                }
            }
        }
    }
}
