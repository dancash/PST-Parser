using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using PSTParse;
using PSTParse.Message_Layer;

namespace PSTParseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            //var pstPath = @"C:\test\FranCoen_Export_0001.pst";
            //var pstPath = @"C:\test\dtmtcm@gmail.com.pst";
            //var pstPath = @"C:\test\Outlook Data File.pst";
            var pstPath = @"C:\test\StevenFisher.pst";
            var pstSize = new FileInfo(pstPath).Length*1.0/1024/1024;
            using (var file = new PSTFile(pstPath))
            {
                Console.WriteLine("Magic value: " + file.Header.DWMagic);
                Console.WriteLine("Is Ansi? " + file.Header.IsANSI);


                var stack = new Stack<MailFolder>();
                stack.Push(file.TopOfPST);
                var totalCount = 0;
                while (stack.Count > 0)
                {
                    var curFolder = stack.Pop();

                    foreach (var child in curFolder.SubFolders)
                        stack.Push(child);
                    var count = curFolder.ContentsTC.RowIndexBTH.Properties.Count;
                    totalCount += count;
                    Console.WriteLine(String.Join(" -> ", curFolder.Path) + " ({0} messages)", count);

                    foreach (var message in curFolder)
                    {

                    }
                }
                sw.Stop();
                Console.WriteLine("{0} messages total", totalCount);
                Console.WriteLine("Parsed {0} ({2:0.00} MB) in {1} milliseconds", Path.GetFileName(pstPath),
                                  sw.ElapsedMilliseconds, pstSize);
                //file.Header.NodeBT.Root.GetOffset(1);
                Console.Read();
            }
        }
    }
}
