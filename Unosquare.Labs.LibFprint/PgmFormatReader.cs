namespace Unosquare.Labs.LibFprint
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Provides utility methods to read PGM files into Bitmap objects.
    /// </summary>
    public static class PgmFormatReader
    {

        /// <summary>
        /// The standard grayscale palette that is loaded in the static constructor.
        /// </summary>
        private static readonly ColorPalette GrayscalePalette = null;

        /// <summary>
        /// Initializes the static <see cref="PgmFormatReader"/> class.
        /// </summary>
        static PgmFormatReader()
        {
            
            // We initialize the palette to 256 colors (grayscale)
            // PGM files are always grayscale and we will always represent them in 
            // 256 shades of gray.
            using (var dummyBitmap = new Bitmap(1, 1, PixelFormat.Format8bppIndexed))
            {
                GrayscalePalette = dummyBitmap.Palette;
                for (int i = 0; i < 256; i++)
                {
                    GrayscalePalette.Entries[i] = Color.FromArgb(i, i, i);
                }
            }
        }

        /// <summary>
        /// Reads the specified PGM stream and returns a Bitmap.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        /// <exception cref="BadImageFormatException">PGM Magic Number not found.</exception>
        public static Bitmap Read(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII))
            {
                if ((reader.ReadChar() == 'P' && reader.ReadChar() == '5') == false)
                    throw new BadImageFormatException("PGM Magic Number not found.");

                // Read onew whitespace character
                reader.ReadChar();

                // Get basic PGM format properties
                var parseBuffer = new StringBuilder();
                var pgmWidth = ReadInteger(reader, parseBuffer);
                var pgmHeight = ReadInteger(reader, parseBuffer);
                var grayscaleLevels = ReadInteger(reader, parseBuffer);
                var isTwoByteLevel = (grayscaleLevels > 255);

                // Create the bitmap and assign the 8bpp Indexed palette
                var bmp = new Bitmap(pgmWidth, pgmHeight, PixelFormat.Format8bppIndexed);
                bmp.Palette = GrayscalePalette;

                // Create the target lockbits
                var bitmapData = bmp.LockBits(new Rectangle(0, 0, pgmWidth, pgmHeight), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                var rowLengthOffset = bitmapData.Stride - bitmapData.Width;
                var dataLength = bitmapData.Stride * bitmapData.Height;
                var pixelData = new byte[dataLength];

                var pixelDataIndex = 0;
                var currentPixelValue = (byte)0;

                for (int rowIndex = 0; rowIndex < pgmHeight; rowIndex++)
                {
                    for (int columnIndex = 0; columnIndex < pgmWidth; columnIndex++)
                    {
                        currentPixelValue = reader.ReadByte();
                        if (isTwoByteLevel)
                            currentPixelValue = (byte)(((double)((currentPixelValue << 8) + reader.ReadByte()) / grayscaleLevels) * 255.0);

                        pixelData[pixelDataIndex] = currentPixelValue;
                        pixelDataIndex++;
                    }

                    pixelDataIndex += rowLengthOffset;
                }

                // Use a fast copy mechanism to write the data to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(pixelData, 0, bitmapData.Scan0, dataLength);

                // Release the write lock on the bits.
                bmp.UnlockBits(bitmapData);

                return bmp;
            }
        }

        /// <summary>
        /// Reads the specified PGM buffer and returns a Bitmap.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        /// <returns></returns>
        public static Bitmap Read(byte[] buffer)
        {
            using (var ms = new MemoryStream(buffer))
            {
                ms.Position = 0;
                return Read(ms);
            }
        }

        /// <summary>
        /// Reads the specified PGM file and returns a Bitmap.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public static Bitmap Read(string path)
        {
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                return Read(fs);
            }
        }

        /// <summary>
        /// Reads an integer by reading from a Binary Reader.
        /// It uses a StringBuilder to hold and parse numbers.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="sb">The sb.</param>
        /// <returns></returns>
        private static int ReadInteger(BinaryReader reader, StringBuilder sb)
        {
            var c = '\0';
            sb.Length = 0;
            while (char.IsDigit(c = reader.ReadChar()))
            {
                sb.Append(c);
            }

            return int.Parse(sb.ToString().Trim());
        }
    }
}