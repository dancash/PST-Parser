using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using PSTParse;
using PSTParse.MessageLayer;
using static System.Console;

namespace PSTParseApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            var pstPath = @"C:\temp\testPsts\Leann.pst";
            //var pstPath = @"C:\temp\testPsts\sharp_2_attachments_test.pst";
            //var pstPath = @"C:\temp\testPsts\trg.pst";
            var fileInfo = new FileInfo(pstPath);
            var pstSizeGigabytes = ((double)fileInfo.Length / 1000 / 1000 / 1000).ToString("0.000");
            var logPath = @"C:\temp\testPsts\outTest.log";
            var folderBlacklist = new HashSet<string>();
            //var folderBlacklist = new HashSet<string>
            //{
            //    "RSS Feeds",
            //    "Recoverable Items",
            //    "Outbox",
            //    "Calendar",
            //    "Junk E-mail",
            //    "Drafts",
            //    "Conversation History",
            //    "Contacts",
            //    "Deleted Items",
            //    "Sync Issues",
            //    "Notes",
            //    "ExternalContacts",
            //    "Files",
            //    "Conversation Action Settings",
            //    "Journal",
            //    "Suggested Contacts",
            //    "Tasks",
            //    "Quick Step Settings",
            //    "Yammer Root",
            //    "News Feed"
            //};

            sw.Start();
            using (var file = new PSTFile(pstPath))
            {
                //Console.WriteLine("Magic value: " + file.Header.DWMagic);
                //Console.WriteLine("Is Ansi? " + file.Header.IsANSI);

                var stack = new Stack<MailFolder>();
                stack.Push(file.TopOfPST);
                var totalCount = 0;
                var totalRMSEncryptedCount = 0;
                var skippedFolders = new List<string>();
                if (File.Exists(logPath))
                    File.Delete(logPath);
                using (var writer = new StreamWriter(logPath))
                {
                    while (stack.Count > 0)
                    {
                        var curFolder = stack.Pop();

                        foreach (var child in curFolder.SubFolders)
                        {
                            if (folderBlacklist.Contains(child.Path[1])) continue;
                            stack.Push(child);
                        }
                        var count = curFolder.Count;
                        var line = $"{String.Join(" -> ", curFolder.Path)}({curFolder.ContainerClass}) ({count} messages)";
                        if (curFolder.Path.Count > 1 && curFolder.ContainerClass != "IPF.Note")
                        {
                            skippedFolders.Add(line);
                            continue;
                            //WriteLine("skipping...");
                        }
                        WriteLine(line);

                        var currentFolderCount = 0;
                        foreach (var ipmItem in curFolder)
                        {
                            totalCount++;
                            currentFolderCount++;
                            if (ipmItem is Message message)
                            {
                                //if (message.MessageClass.ToLowerInvariant() == "IPM.Post".ToLowerInvariant())
                                //{

                                //}
                                if (message.MessageClass != "IPM.Note") continue;
                                //if (!message.IsIPMNote) continue;
                                //if (!message.IsIPMNote)
                                //{
                                //    throw new Exception("err ipm");
                                //}
                                if (string.IsNullOrEmpty(message.SenderAddress)) continue;
                                if (message.IsRMSEncrypted)
                                {
                                    totalRMSEncryptedCount++;
                                }

                                //var recipients = message.Recipients;
                                //if (!message.HasAttachments) continue;

                                //foreach (var attachment in message.AttachmentsLazy)
                                //{
                                //    //if (!message.IsRMSEncrypted && attachment.AttachmentLongFileName.ToLower().EndsWith("rpmsg"))
                                //    //{
                                //    //    throw new Exception("False negative, message should be labeled as encrypted");
                                //    //}
                                //    //if (message.IsRMSEncrypted && !attachment.AttachmentLongFileName.ToLower().EndsWith("rpmsg"))
                                //    //{
                                //    //    throw new Exception("False positive, message should not be labeled as encrypted");
                                //    //}
                                //    if (attachment.AttachmentLongFileName.ToLower().EndsWith("rpmsg"))
                                //    {
                                //        throw new Exception("False negative, message should be labeled as encrypted");
                                //    }
                                //    if (!attachment.AttachmentLongFileName.ToLower().EndsWith("rpmsg"))
                                //    {
                                //        throw new Exception("False positive, message should not be labeled as encrypted");
                                //    }
                                //}
                                //if (message.AttachmentsLazy.Count > 1)
                                //{
                                //}

                                //Console.WriteLine(message.Subject);
                                //Console.WriteLine(message.Imporance);
                                //Console.WriteLine("Sender Name: " + message.SenderName);
                                //if (message.From.Count > 0)
                                //    Console.WriteLine("From: {0}",
                                //                      String.Join("; ", message.From.Select(r => r.EmailAddress)));
                                //if (message.To.Count > 0)
                                //    Console.WriteLine("To: {0}",
                                //                      String.Join("; ", message.To.Select(r => r.EmailAddress)));
                                //if (message.CC.Count > 0)
                                //    Console.WriteLine("CC: {0}",
                                //                      String.Join("; ", message.CC.Select(r => r.EmailAddress)));
                                //if (message.BCC.Count > 0)
                                //    Console.WriteLine("BCC: {0}",
                                //                      String.Join("; ", message.BCC.Select(r => r.EmailAddress)));


                                //writer.WriteLine(ByteArrayToString(BitConverter.GetBytes(message.NID)));
                            }
                            else
                            {
                                throw new NotSupportedException();
                            }
                        }
                    }
                }
                sw.Stop();
                var elapsedSeconds = sw.ElapsedMilliseconds / 1000;
                WriteLine("{0} messages total", totalCount);
                WriteLine("{0} encrypted messages total", totalRMSEncryptedCount);
                WriteLine("Parsed {0} ({1} GB) in {2} seconds", Path.GetFileName(pstPath), pstSizeGigabytes, elapsedSeconds);

                WriteLine("\r\nSkipped Folders:\r\n");
                foreach (var line in skippedFolders)
                {
                    WriteLine(line);
                }
                //file.Header.NodeBT.Root.GetOffset(1);
                Read();
            }
        }

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
