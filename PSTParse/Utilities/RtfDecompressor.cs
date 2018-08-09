

namespace PSTParse.Utilities
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;


    /// <summary>
    /// Copyright 2018 Dmitry Brant
    /// 
    /// Documentation for Microsoft Compressed RTF:
    /// https://msdn.microsoft.com/en-us/library/cc463890(v=exchg.80).aspx
    /// </summary>
    public class RtfDecompressor
    {
        private const int DictionaryLength = 0x1000;
        private byte[] InitialDictionary;

        public RtfDecompressor()
        {
            var builder = new StringBuilder();
            builder.Append(@"{\rtf1\ansi\mac\deff0\deftab720{\fonttbl;}{\f0\fnil \froman \fswiss \fmodern \fscript ")
                .Append(@"\fdecor MS Sans SerifSymbolArialTimes New RomanCourier{\colortbl\red0\green0\blue0")
                .Append("\r\n").Append(@"\par \pard\plain\f0\fs20\b\i\u\tab\tx");
            InitialDictionary = Encoding.ASCII.GetBytes(builder.ToString());
        }

        public string Decompress(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            return Decompress(bytes);
        }

        public string Decompress(byte[] bytes)
        {
            try
            {
                byte[] decompressed = Decompress(bytes, false);
                return Encoding.ASCII.GetString(decompressed);
            }
            catch (Exception) { }
            return "";
        }

        public byte[] Decompress(byte[] data, bool enforceCrc)
        {
            RtfHeader header = new RtfHeader(data);

            if (header.compressionType.Equals("MELA"))
            {
                // uncompressed!
                byte[] outBytes = new byte[data.Length];
                data.CopyTo(outBytes, 0);
                return outBytes;
            }
            else if (header.compressionType.Equals("LZFu"))
            {
                if (enforceCrc)
                {
                    var headerCrc = CRC(data, RtfHeader.Length);
                    if (headerCrc != header.crc)
                    {
                        throw new ArgumentException("Header CRC is incorrect.");
                    }
                }

                var dictionary = new byte[DictionaryLength];
                using (var destination = new MemoryStream(header.uncompressedSize))
                {
                    Array.Copy(InitialDictionary, 0, dictionary, 0, InitialDictionary.Length);
                    var dictionaryPtr = InitialDictionary.Length;
                    try
                    {
                        int bytePtr = RtfHeader.Length;
                        while (bytePtr < data.Length)
                        {
                            var control = new RtfControl(data[bytePtr]);
                            var offset = 1;

                            for (int j = 0; j < control.flags.Length; j++)
                            {
                                if (control.flags[j])
                                {
                                    destination.WriteByte(data[bytePtr + offset]);
                                    dictionary[dictionaryPtr] = data[bytePtr + offset];
                                    dictionaryPtr++;
                                    dictionaryPtr %= DictionaryLength;
                                }
                                else
                                {
                                    var word = (((int)(data[bytePtr + offset])) << 8) | data[bytePtr + offset + 1];
                                    var upper = (word & 0xFFF0) >> 4;
                                    var lower = (word & 0xF) + 2;

                                    if (upper == dictionaryPtr)
                                    {
                                        return destination.ToArray();
                                    }

                                    for (int k = 0; k < lower; k++)
                                    {
                                        var correctedOffset = (upper + k);
                                        correctedOffset %= DictionaryLength;
                                        if (destination.Position == header.uncompressedSize)
                                        {
                                            return destination.ToArray();
                                        }
                                        destination.WriteByte(dictionary[correctedOffset]);
                                        dictionary[dictionaryPtr] = dictionary[correctedOffset];
                                        dictionaryPtr++;
                                        dictionaryPtr %= DictionaryLength;
                                    }
                                    offset++;
                                }
                                offset++;
                            }
                            bytePtr += control.length;
                        }
                    }
                    catch (Exception) { }

                    // return partial result in case of error
                    return destination.ToArray();
                }
            }
            throw new ApplicationException("Unrecognized compression type.");
        }

        private struct RtfHeader
        {
            public const int Length = 0x10;
            public int compressedSize;
            public int uncompressedSize;
            public string compressionType;
            public uint crc;

            public RtfHeader(byte[] bytes)
            {
                compressedSize = BitConverter.ToInt32(bytes, 0);
                uncompressedSize = BitConverter.ToInt32(bytes, 4);
                compressionType = Encoding.ASCII.GetString(bytes, 8, 4);
                crc = BitConverter.ToUInt32(bytes, 12);
            }
        }

        private struct RtfControl
        {
            public BitArray flags;
            public int length;

            public RtfControl(byte b)
            {
                flags = new BitArray(8);
                int zeros = 0;
                for (int i = 0; i < flags.Length; i++)
                {
                    flags[i] = ((b & (0x1 << i)) == 0);
                    zeros += flags[i] ? 1 : 0;
                }
                length = ((8 - zeros) * 2) + zeros + 1;
            }
        }

        private static uint CRC(byte[] buffer, int offset)
        {
            uint crc = 0;
            for (int i = offset; i < buffer.Length; i++)
            {
                crc = CRC32.CrcTableOffset32[(crc ^ buffer[i]) & 0xFF] ^ (crc >> 8);
            }
            return crc;
        }
    }
}