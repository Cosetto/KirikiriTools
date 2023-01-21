﻿using System.IO;

namespace Arc.Ddsi.Xp3Pack
{
    internal class Xp3ArchiveWriter
    {
        private static readonly byte[] HeaderMagic = { 0x58, 0x50, 0x33, 0x0D, 0x0A, 0x20, 0x0A, 0x1A, 0x8B, 0x67, 0x01 };

 		// https://github.com/crskycode/KrkrzPack/blob/bbf8e08d99227c57497e905ca2aa6bb5124fc647/KrkrzPack/Program.cs#L74
		// $$$ Warning: Extracting this archive may infringe on author's rights. 警告 : このアーカイブを展開することにより、あなたは著作者の権利を侵害するおそれがあります。
        private static readonly byte[] ProtectionWarningPng = {
            0x89,0x50,0x4e,0x47,0x0a,0x1a,0x0a,0x00,0x00,0x00,0x0d,0x49,0x48,0x44,0x52,0x00,
            0x00,0x00,0x01,0x00,0x00,0x00,0x01,0x08,0x02,0x00,0x00,0x00,0x90,0x77,0x53,0xde,
            0x00,0x00,0x00,0xa5,0x74,0x45,0x58,0x74,0x57,0x61,0x72,0x6e,0x69,0x6e,0x67,0x00,
            0x57,0x61,0x72,0x6e,0x69,0x6e,0x67,0x3a,0x20,0x45,0x78,0x74,0x72,0x61,0x63,0x74,
            0x69,0x6e,0x67,0x20,0x74,0x68,0x69,0x73,0x20,0x61,0x72,0x63,0x68,0x69,0x76,0x65,
            0x20,0x6d,0x61,0x79,0x20,0x69,0x6e,0x66,0x72,0x69,0x6e,0x67,0x65,0x20,0x6f,0x6e,
            0x20,0x61,0x75,0x74,0x68,0x6f,0x72,0x27,0x73,0x20,0x72,0x69,0x67,0x68,0x74,0x73,
            0x2e,0x20,0x8c,0x78,0x8d,0x90,0x20,0x3a,0x20,0x82,0xb1,0x82,0xcc,0x83,0x41,0x81,
            0x5b,0x83,0x4a,0x83,0x43,0x83,0x75,0x82,0xf0,0x93,0x57,0x8a,0x4a,0x82,0xb7,0x82,
            0xe9,0x82,0xb1,0x82,0xc6,0x82,0xc9,0x82,0xe6,0x82,0xe8,0x81,0x41,0x82,0xa0,0x82,
            0xc8,0x82,0xbd,0x82,0xcd,0x92,0x98,0x8d,0xec,0x8e,0xd2,0x82,0xcc,0x8c,0xa0,0x97,
            0x98,0x82,0xf0,0x90,0x4e,0x8a,0x51,0x82,0xb7,0x82,0xe9,0x82,0xa8,0x82,0xbb,0x82,
            0xea,0x82,0xaa,0x82,0xa0,0x82,0xe8,0x82,0xdc,0x82,0xb7,0x81,0x42,0x4b,0x49,0x44,
            0x27,0x00,0x00,0x00,0x0c,0x49,0x44,0x41,0x54,0x78,0x9c,0x63,0xf8,0xff,0xff,0x3f,
            0x00,0x05,0xfe,0x02,0xfe,0x0d,0xef,0x46,0xb8,0x00,0x00,0x00,0x00,0x49,0x45,0x4e,
            0x44,0xae,0x42,0x60,0x82,0x89,0x50,0x4e,0x47,0x0a,0x1a,0x0a,0x00,0x00,0x00,0x0d,
            0x49,0x48,0x44,0x52,0x00,0x00,0x01,0xef,0x00,0x00,0x00,0x13,0x01,0x03,0x00,0x00,
            0x00,0x83,0x60,0x17,0x58,0x00,0x00,0x00,0x06,0x50,0x4c,0x54,0x45,0x00,0x00,0x00,
            0xff,0xff,0xff,0xa5,0xd9,0x9f,0xdd,0x00,0x00,0x02,0x4f,0x49,0x44,0x41,0x54,0x78,
            0xda,0xd5,0xd3,0x31,0x6b,0xdb,0x40,0x14,0x07,0x70,0x1d,0x0a,0x55,0xb3,0x44,0x6d,
            0xb2,0xc4,0x20,0xac,0x14,0x9b,0x78,0x15,0xf1,0x12,0x83,0xf1,0x0d,0x1d,0x4a,0x21,
            0x44,0x1f,0xa2,0xa1,0x5a,0xd3,0x78,0x49,0xc0,0x44,0x86,0x14,0xb2,0x04,0xec,0xc4,
            0x53,0x40,0xf8,0xbe,0x8a,0x4c,0x42,0x6a,0x83,0xf0,0x7d,0x05,0xb9,0x15,0xba,0x55,
            0xe8,0x2d,0x3e,0x10,0x7a,0x3d,0x25,0xf9,0x06,0x19,0x4a,0x6f,0x38,0x74,0x9c,0x7e,
            0xf7,0x7f,0x8f,0xe3,0x34,0xc4,0x37,0x8c,0x52,0x7b,0x8b,0xfe,0xe7,0x3c,0xe3,0x8b,
            0xd7,0xef,0x02,0x45,0x06,0x99,0xae,0x99,0x02,0x11,0x10,0x39,0xa2,0x2c,0x7d,0x2e,
            0x68,0x3b,0xf7,0x53,0x1f,0x27,0x65,0x17,0xd6,0xba,0x44,0x51,0xed,0x31,0x79,0xcd,
            0xd4,0xff,0xbc,0xd4,0x62,0xa2,0x78,0x3c,0xb0,0x48,0xb5,0xcc,0x00,0xc0,0xe1,0x82,
            0xc4,0x7d,0x89,0xbc,0xf1,0xc2,0x0f,0xb5,0x33,0x3f,0xbd,0x34,0xc7,0x4e,0x02,0x12,
            0xd4,0xd9,0x04,0x0a,0xe3,0x56,0xf1,0xdb,0x67,0x9e,0x6d,0x0e,0x6d,0xc4,0xb5,0x2b,
            0x15,0x5f,0x92,0xe1,0xa9,0xae,0xbd,0x27,0x80,0x00,0x06,0xdf,0xc4,0x70,0xd7,0x20,
            0x73,0xb3,0x9d,0xea,0x5a,0xb1,0xbf,0x51,0x24,0xc3,0x33,0xbd,0x33,0x27,0x10,0xd2,
            0xe5,0xc6,0xfa,0x88,0xfc,0x0c,0x1d,0x5d,0xf1,0x46,0x47,0x15,0x9e,0x7b,0xf7,0x18,
            0x9b,0x4c,0x8a,0xdc,0x55,0xe9,0xaa,0xc0,0x1d,0x9c,0xd5,0x54,0x7a,0x9f,0x73,0x1a,
            0x25,0x2c,0x91,0xed,0xe1,0x87,0xa6,0x00,0x45,0x85,0x00,0xc6,0x6e,0x56,0x26,0xbb,
            0x79,0x4e,0x2f,0xbf,0xaa,0x3a,0x15,0x9f,0xb0,0x82,0xdb,0x72,0x55,0xf5,0x6e,0xcc,
            0x70,0xdd,0x47,0xde,0xc1,0xac,0x77,0x11,0xba,0x5d,0x32,0x9b,0x6a,0xb5,0xf6,0x66,
            0x37,0x59,0xc5,0x44,0xa2,0x31,0x03,0x4e,0x03,0xd9,0xd2,0x83,0xb0,0x6e,0x56,0xbd,
            0x6b,0x26,0x62,0xea,0x4d,0xc2,0x6e,0x64,0xcb,0xc7,0x03,0x97,0x2e,0xbd,0x00,0x25,
            0x54,0x3c,0xb6,0x04,0xe3,0xf4,0x41,0x5c,0x09,0x68,0x6f,0x9f,0x9f,0x3c,0x3a,0x52,
            0xdb,0xf2,0x82,0xec,0x50,0xb7,0xe4,0x3e,0x09,0x8a,0x82,0xbf,0x5e,0x9c,0x48,0xbd,
            0x6b,0x2c,0x16,0x0c,0xe6,0x19,0xd9,0x3b,0xf6,0x7a,0x1e,0xbc,0xf0,0x23,0xc5,0x0f,
            0x35,0x8f,0x0d,0xfb,0x07,0xda,0x6e,0x73,0x9e,0xeb,0x58,0x7a,0xbd,0x6f,0x8c,0x5a,
            0xb0,0xbf,0xf1,0xc2,0xd7,0x46,0x48,0x91,0xa7,0x1e,0x43,0xc9,0x19,0x64,0xf9,0x8e,
            0xc3,0x8d,0x27,0x57,0x40,0xcf,0xec,0xe0,0x3c,0xba,0x9e,0x44,0xbd,0x8e,0x98,0x6e,
            0xf5,0x0f,0xb6,0x4f,0x93,0x0c,0x5a,0x21,0x35,0x9e,0x3e,0x4f,0x2f,0x2d,0x68,0xde,
            0x04,0x71,0x69,0xd6,0x55,0x3a,0xa7,0x9c,0x27,0x82,0x56,0x5c,0x24,0xf9,0x97,0x3d,
            0x57,0xdc,0xb9,0x22,0x1f,0x3c,0x48,0x16,0x2d,0x1a,0xad,0xc5,0x2e,0x11,0x57,0xe1,
            0x59,0x7f,0x6c,0x15,0x09,0x8c,0x38,0x15,0x77,0x15,0x6f,0x77,0xa3,0x22,0xa2,0xcb,
            0x63,0x95,0xce,0x55,0xba,0xa3,0x26,0xe0,0x8c,0xab,0x2e,0x1c,0xce,0xc7,0xbc,0x95,
            0x0f,0x16,0xc0,0x17,0xf7,0x9f,0x5a,0x2b,0xb3,0x23,0xd8,0xf2,0xdc,0xbb,0x1b,0x14,
            0x02,0x5a,0x2a,0x6a,0xfc,0x30,0xf5,0x83,0x66,0xf7,0x5e,0x46,0x6c,0x7a,0xac,0x49,
            0xa1,0x8a,0x8f,0x2f,0xea,0x3e,0xa6,0x36,0xb3,0xb3,0xe6,0xc7,0xe6,0xc8,0x9e,0xc5,
            0xa7,0xb5,0x77,0xf6,0x2f,0xf1,0x9b,0x8e,0xb2,0x13,0x9f,0x08,0x16,0x0e,0x46,0x63,
            0x6b,0x9d,0x39,0x3f,0x42,0x6a,0xcf,0x12,0x6a,0x4c,0xbf,0x5f,0x36,0xfe,0xac,0x4a,
            0x5a,0x57,0xe9,0xff,0xf1,0x8b,0x7b,0x1b,0xff,0x0b,0x28,0x8d,0x8d,0xf8,0xb3,0xe9,
            0xa1,0xdf,0x00,0x00,0x00,0x00,0x49,0x45,0x4e,0x44,0xae,0x42,0x60,0x82
        };
        
        public static void Write(string folderPath, string xp3FilePath)
        {
            if (!folderPath.EndsWith("\\") && !folderPath.EndsWith("/"))
                folderPath += "\\";

            using (Stream xp3Stream = File.Open(xp3FilePath, FileMode.Create, FileAccess.Write))
            using (BinaryWriter xp3Writer = new BinaryWriter(xp3Stream))
            {
                xp3Writer.Write(HeaderMagic);
				xp3Writer.Write(0L);        // Index offset
				// https://github.com/morkt/GARbro/blob/ea096c52ef71065d0d46bd14f601a99b63b99873/ArcFormats/KiriKiri/ArcXP3.cs#L435-L441
				// This is for xp3 archive V2
                // xp3Writer.Write((long)0x17);
				// xp3Writer.Write((int)1);
				// xp3Writer.Write((byte)0x80);
				// xp3Writer.Write((long)0);
				xp3Writer.Write(ProtectionWarningPng);

                Xp3IndexBuilder index = new Xp3IndexBuilder();

                foreach (string filePath in Directory.EnumerateFiles(folderPath, "*", SearchOption.AllDirectories))
                {
                    string extension = Path.GetExtension(filePath);
                    bool compressed = extension != ".mpg";

                    long offset = xp3Stream.Position;
                    long originalSize;
                    long compressedSize;
                    AppendFile(xp3Stream, filePath, compressed, out originalSize, out compressedSize);

                    string relativeFilePath = filePath.Substring(folderPath.Length);
                    index.Add(relativeFilePath, offset, originalSize, compressedSize, compressed);
                }

                long indexOffset = xp3Stream.Length;
                xp3Writer.Write(index.Build());

                xp3Stream.Seek(HeaderMagic.Length, SeekOrigin.Begin);
                xp3Writer.Write(indexOffset);
            }
        }

        private static void AppendFile(Stream xp3Stream, string filePath, bool compressed, out long originalSize, out long compressedSize)
        {
            using (Stream fileStream = File.OpenRead(filePath))
            {
                originalSize = fileStream.Length;

                long startPos = xp3Stream.Position;
                if (compressed)
                {
                    using (ZlibStream compressionStream = new ZlibStream(xp3Stream))
                    {
                        fileStream.CopyTo(compressionStream);
                    }
                }
                else
                {
                    fileStream.CopyTo(xp3Stream);
                }

                compressedSize = xp3Stream.Position - startPos;
            }
        }
    }
}
