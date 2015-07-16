using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace GemBox.Drawing
{
    /// <summary>
    /// Provides direct and fast access to the pixels of an image.
    /// To get an instance of this class, use the <see cref="BitmapExtensions.LockPixels(Bitmap, Rectangle, ImageLockMode)"/> method.
    /// </summary>
    /// <remarks>
    /// This class offers features similar to those of the <see cref="System.Drawing.Imaging.BitmapData"/> class, with the following differences:
    /// <list type="bullet">
    /// <item>PixelData implements IDisposable; a call to Dispose (explicit or via a <c>using</c> block) frees the locked pixels</item>
    /// <item>PixelData provides an indexer to easily get or set the color of a pixel</item>
    /// <item>PixelData only supports one pixel format : <see cref="System.Drawing.Imaging.PixelFormat.Format32bppArgb"/></item>
    /// </list>
    /// </remarks>
    public sealed class PixelData : IDisposable
    {
        private readonly Bitmap _bitmap;
        private readonly BitmapData _data;
        private readonly int _pixelSize;

        internal PixelData(Bitmap bitmap, Rectangle rect, ImageLockMode flags)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));
            _bitmap = bitmap;
            _data = bitmap.LockBits(rect, flags, PixelFormat.Format32bppArgb);
            _pixelSize = _data.Stride / _data.Width;
        }

        /// <summary>
        /// Frees the locked pixels
        /// </summary>
        public void Dispose()
        {
            _bitmap.UnlockBits(_data);
        }

        /// <summary>
        /// Gets or sets the address of the first pixel data in the bitmap. This can also be thought of as the first scan line in the bitmap.
        /// </summary>
        public IntPtr Scan0 => _data.Scan0;

        /// <summary>
        /// Gets or sets the stride width (also called scan width) of the Bitmap object.
        /// </summary>
        public int Stride => _data.Stride;

        /// <summary>
        /// Gets or sets the pixel width of the Bitmap object. This can also be thought of as the number of pixels in one scan line.
        /// </summary>
        public int Width => _data.Width;

        /// <summary>
        /// Gets or sets the pixel height of the Bitmap object. Also sometimes referred to as the number of scan lines.
        /// </summary>
        public int Height => _data.Height;

        /// <summary>
        /// Gets or sets the format of the pixel information in the Bitmap object that returned this BitmapData object.
        /// </summary>
        public PixelFormat PixelFormat => _data.PixelFormat;

        /// <summary>
        /// Gets or sets the color of the pixel at the specified coordinates
        /// </summary>
        /// <param name="x">Horizontal position of the pixel</param>
        /// <param name="y">Vertical position of the pixel</param>
        /// <returns>The color of the pixel</returns>
        public unsafe Color this[int x, int y]
        {
            get
            {
                byte* ptr = GetPixelPointer(x, y);
                return Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
            }
            set
            {
                byte* ptr = GetPixelPointer(x, y);
                ptr[0] = value.B;
                ptr[1] = value.G;
                ptr[2] = value.R;
                ptr[3] = value.A;
            }
        }

        private unsafe byte* GetPixelPointer(int x, int y)
        {
            if (x < 0 || x > Width - 1)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y > Height - 1)
                throw new ArgumentOutOfRangeException(nameof(y));
            int offset = y * _data.Stride + x * _pixelSize;
            return ((byte*)_data.Scan0) + offset;
        }
    }
}
