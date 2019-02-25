using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using PSTParse;
using static System.Console;

namespace PSTParseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            var pstPath = @"C:\localdocs\testPsts\trg.pst";
            var fileInfo = new FileInfo(pstPath);
            var pstSizeGigabytes = ((double)fileInfo.Length / 1000 / 1000 / 1000).ToString("0.000");
            using (var file = new PSTFile(pstPath))
            {
                Debug.Assert((double)fileInfo.Length / 1000 / 1000 / 1000 == file.SizeMB / 1000);
                //Console.WriteLine("Magic value: " + file.Header.DWMagic);
                //Console.WriteLine("Is Ansi? " + file.Header.IsANSI);

                var stack = new Stack<MailFolder>();
                stack.Push(file.TopOfPST);
                var totalCount = 0;
                //var maxSearchSize = 1500;
                var maxSearchSize = int.MaxValue;
                var totalEncryptedCount = 0;
                var totalUnsentCount = 0;
                var skippedFolders = new List<string>();
                while (stack.Count > 0)
                {
                    var curFolder = stack.Pop();

                    foreach (var child in curFolder.SubFolders)
                    {
                        stack.Push(child);
                    }
                    var count = curFolder.Count;
                    var line = $"{string.Join(" -> ", curFolder.Path)}({curFolder.ContainerClass}) ({count} messages)";
                    if (curFolder.Path.Count > 1 && curFolder.ContainerClass != "" && curFolder.ContainerClass != "IPF.Note")
                    {
                        skippedFolders.Add(line);
                        continue;
                    }
                    WriteLine(line);

                    var currentFolderCount = 0;
                    foreach (var message in curFolder.GetIpmNotes())
                    //foreach (var ipmItem in curFolder.GetIpmItems())
                    {
                        totalCount++;
                        currentFolderCount++;

                        //if (!(ipmItem is Message message))
                        //{
                        //    nonMessageTypes++;
                        //    continue;
                        //}
                        if (message.Unsent)
                        {
                            totalUnsentCount++;
                            continue;
                        }

                        //if (message.IsRMSEncrypted)
                        //{
                        //    totalEncryptedCount++;
                        //    //stack.Clear();
                        //    //break;
                        //}
                        Debug.Assert(message.IsRMSEncryptedHeaders == message.IsRMSEncrypted, "encryption mismatch, big problems");
                        if (message.IsRMSEncryptedHeaders)
                        {
                            totalEncryptedCount++;
                            //stack.Clear();
                            //break;
                        }
                        if (totalCount == maxSearchSize)
                        {
                            stack.Clear();
                            break;
                        }

                        //var recipients = message.Recipients;
                        //if (!message.HasAttachments) continue;
                        //foreach (var attachment in message.AttachmentHeaders)
                        //{

                        //}
                    }
                }
                sw.Stop();
                var elapsedSeconds = (double)sw.ElapsedMilliseconds / 1000;
                WriteLine("{0} messages total", totalCount);
                WriteLine("{0} encrypted messages total", totalEncryptedCount);
                WriteLine("{0} totalUnsentCount", totalUnsentCount);
                WriteLine("Parsed {0} ({1} GB) in {2:0.00} seconds", Path.GetFileName(pstPath), pstSizeGigabytes, elapsedSeconds);

                WriteLine("\r\nSkipped Folders:\r\n");
                foreach (var line in skippedFolders)
                {
                    WriteLine(line);
                }
                Read();
            }
        }
    }
}
