using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PSTParse;

namespace PSTParseApp
{
    class Program
    {
        static void Main(string[] args)
        {

            //var pstPath = @"C:\test\FranCoen_Export_0001.pst";
            var pstPath = @"C:\test\dtmtcm@gmail.com.pst";
            var file = new PSTFile(pstPath);
            Console.WriteLine("Magic value: " + file.Header.DWMagic);
            Console.WriteLine("Is Ansi? " + file.Header.IsANSI);

            //file.Header.NodeBT.Root.GetOffset(1);
            Console.Read();
        }
    }
}
