using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using MsgKit;
using MsgKit.Enums;
using PSTParse;
using PSTParse.MessageLayer;

namespace PSTParseApp
{
    class Program
    {
        //static void Main(string[] args)
        //{
        //    try
        //    {
        //        var sender = new Sender(address, null, addressType: MsgKit.Enums.AddressType.Unknown, senderIsCreator: true);
        //        var subject = "Hello Neverland subject";
        //        //var repping = new Representing("tinkerbell@neverland.com", "Tinkerbell", AddressType.Ex);
        //        using (var email = new Email(sender, subject))
        //        {
        //            //email.AddNamedProperty("test1", "test2", Guid.NewGuid());
        //            email.BodyRtf = "something rtf?";
        //            //email.Recipients.AddCc("crocodile@neverland.com", "The evil ticking crocodile");
        //            //email.Subject = "This is the subject";
        //            email.BodyText = "Hello Neverland text";
        //            email.BodyHtml = "<html><head></head><body><b>Hello Neverland html</b></body></html>";
        //            email.Importance = MessageImportance.IMPORTANCE_NORMAL;
        //            email.IconIndex = MessageIconIndex.ReadMail;
        //            email.SentOn = new DateTime(2015, 02, 05);
        //            var fileStream = new FileStream(@"C:\Users\u403598\Desktop\temp\tempAreYouSure\testout.png", FileMode.Open);
        //            email.Attachments.Add(fileStream, "testout1.png");
        //            //email.Attachments.Add(fileStream, "testout2.png", isInline: true, contentId: Guid.NewGuid().ToString());
        //            email.InternetMessageId = "123@zxyz";
        //            email.Save(@"C:\Users\u403598\Desktop\temp\tempAreYouSure\email.msg");

        //            //using (var msg = new MsgReader.Outlook.Storage.Message(@"C:\Users\u403598\Desktop\temp\tempAreYouSure\email.msg"))
        //            //{
        //            //    var from = msg.Sender;
        //            //    var sentOn = msg.SentOn;
        //            //    var recipientsTo = msg.GetEmailRecipients(Storage.Recipient.RecipientType.To, false, false);
        //            //    var recipientsCc = msg.GetEmailRecipients(Storage.Recipient.RecipientType.Cc, false, false);
        //            //    var subjectX = msg.Subject;
        //            //    var htmlBody = msg.BodyHtml;
        //            //    // etc...
        //            //}

        //            // Show the E-mail
        //            //Process.Start(@"c:\email.msg");
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        static void Main(string[] args)
        {
            var sw = new Stopwatch();
            sw.Start();
            //var pstPath = @"C:\temp\pstCreation\sharp_test.pst";
            //var pstPath = @"C:\temp\pstCreation\Leann.pst";
            var pstPath = @"C:\temp\pstCreation\sharp_test_single.pst";

            var logPath = @"C:\temp\pstCreation\outTest.log";
            var pstSize = new FileInfo(pstPath).Length * 1.0 / 1024 / 1024;
            using (var file = new PSTFile(pstPath))
            {
                Console.WriteLine("Magic value: " + file.Header.DWMagic);
                Console.WriteLine("Is Ansi? " + file.Header.IsANSI);


                var stack = new Stack<MailFolder>();
                stack.Push(file.TopOfPST);
                var totalCount = 0;
                if (File.Exists(logPath))
                    File.Delete(logPath);
                using (var writer = new StreamWriter(logPath))
                {
                    while (stack.Count > 0)
                    {
                        var curFolder = stack.Pop();

                        foreach (var child in curFolder.SubFolders)
                            stack.Push(child);
                        var count = curFolder.ContentsTC.RowIndexBTH.Properties.Count;
                        totalCount += count;
                        Console.WriteLine(String.Join(" -> ", curFolder.Path) + " ({0} messages)", count);

                        var counter = 0;
                        foreach (var ipmItem in curFolder)
                        {
                            //counter++;
                            //if (counter == 1) continue;

                            if (ipmItem is PSTParse.MessageLayer.Message)
                            {
                                var message = ipmItem as PSTParse.MessageLayer.Message;
                                if (string.IsNullOrEmpty(message.SenderAddress)) continue;


                                //foreach (var attachment in message.Attachments)
                                //{
                                //    File.WriteAllBytes($@"C:\Users\u403598\Desktop\temp\tempPLEASES\{Guid.NewGuid().ToString().Substring(0, 5)}-{attachment.AttachmentLongFileName}", attachment.Data);
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
                        }
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

        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}
