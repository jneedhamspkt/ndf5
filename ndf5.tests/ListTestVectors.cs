﻿using System;
using System.Linq;
using System.IO;
using System.Reflection;
namespace ndf5.tests
{
    public class RunTests
    {
        public static int Main(string[] args)
        {
            foreach(string fFile in Loading.AllExampleFiles)
            {
                Assembly
                fAssembly = Assembly.GetExecutingAssembly();

                try
                {
                    using (Stream fTestStream = fAssembly.GetManifestResourceStream(fFile))
                    {
                        using (Hdf5File fTest = Hdf5File.Open(fTestStream))
                        {
                            ndf5.Metadata.ISuperBlock
                                fSuperBlock = fTest.SuperBlock;

                            string fRootGroupAddr = fSuperBlock.RootGroupAddress.HasValue 
                                ? $"0x{fSuperBlock.RootGroupAddress:X16}"
                                : "null";

                            Console.WriteLine(
                                $"{fFile.Replace("ndf5.tests.TestData.", "").PadRight(40)}| " +
                                $"SBVersion:{fSuperBlock.SuperBlockVersion:X} O:{fSuperBlock.SizeOfOffsets} " +
                                $"L:{fSuperBlock.SizeOfLengths} GiK:{fSuperBlock.GroupInternalNodeK} " +
                                $"GlK: {fSuperBlock.GroupLeafNodeK} IsiK:{fSuperBlock.IndexedStorageInternalNodeK} " +
                                $"Root Obj Addr: { fRootGroupAddr}");
                        }
                    }
                }
                catch(Exception fException)
                {
                    Console.WriteLine(
                        $"{fFile.Replace("ndf5.tests.TestData.", "").PadRight(40)}| {fException.Message}");
                }
            }
            return 0;
        }
    }
}
