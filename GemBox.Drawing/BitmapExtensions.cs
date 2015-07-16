using System.Drawing;
using System.Drawing.Imaging;

namespace GemBox.Drawing
{
    /// <summary>
    /// Provides extension methods for manipulating Bitmap images
    /// </summary>
    public static class BitmapExtensions
    {
        /// <summary>
        /// Returns a cropped version of the original image, keeping only the specified area.
        /// </summary>
        /// <param name="bitmap">The image to crop</param>
        /// <param name="cropRectangle">The area to keep</param>
        /// <returns>The cropped image</returns>
        public static Bitmap Crop(this Bitmap bitmap, Rectangle cropRectangle)
        {
            return bitmap.Clone(cropRectangle, bitmap.PixelFormat);
        }

        /// <summary>
        /// Returns a cropped version of the original image, keeping only the specified area.
        /// </summary>
        /// <param name="bitmap">The image to crop</param>
        /// <param name="cropRectangle">The area to keep</param>
        /// <returns>The cropped image</returns>
        public static Bitmap Crop(this Bitmap bitmap, RectangleF cropRectangle)
        {
            return bitmap.Clone(cropRectangle, bitmap.PixelFormat);
        }

        /// <summary>
        /// Returns a cropped version of the original image, keeping only the specified area.
        /// </summary>
        /// <param name="bitmap">The image to crop</param>
        /// <param name="x">The horizontal position of the area to keep</param>
        /// <param name="y">The vertical position of the area to keep</param>
        /// <param name="width">The width of the area to keep</param>
        /// <param name="height">The height of the area to keep</param>
        /// <returns>The cropped image</returns>
        public static Bitmap Crop(this Bitmap bitmap, int x, int y, int width, int height)
        {
            Rectangle cropRectangle = new Rectangle(x, y, width, height);
            return bitmap.Crop(cropRectangle);
        }

        /// <summary>
        /// Returns a cropped version of the original image, keeping only the specified area.
        /// </summary>
        /// <param name="bitmap">The image to crop</param>
        /// <param name="x">The horizontal position of the area to keep</param>
        /// <param name="y">The vertical position of the area to keep</param>
        /// <param name="width">The width of the area to keep</param>
        /// <param name="height">The height of the area to keep</param>
        /// <returns>The cropped image</returns>
        public static Bitmap Crop(this Bitmap bitmap, float x, float y, float width, float height)
        {
            RectangleF cropRectangle = new RectangleF(x, y, width, height);
            return bitmap.Crop(cropRectangle);
        }
        
        /// <summary>
        /// Removes transparency from an image by placing it on a background of the specified color.
        /// </summary>
        /// <param name="image">The original image</param>
        /// <param name="background">The background color to use</param>
        /// <returns>A new image that is the result of removing the transparency from the original image</returns>
        public static Image RemoveTransparency(this Image image, Color background)
        {
            var newImage = new Bitmap(image.Width, image.Height);
            using (var g = Graphics.FromImage(newImage))
            {
                g.Clear(background);
                g.DrawImage(image, Point.Empty);
            }
            return newImage;
        }

        /// <summary>
        /// Applies a color transform matrix to the image
        /// </summary>
        /// <param name="image">The original image</param>
        /// <param name="colorMatrix">The color transform matrix</param>
        /// <returns>A new image that is the result of applying the color transform.</returns>
        public static Image Apply(this Image image, ColorMatrix colorMatrix)
        {
            Bitmap newBitmap = new Bitmap(image.Width, image.Height);
            using (Graphics g = Graphics.FromImage(newBitmap))
            {
                ImageAttributes attributes = new ImageAttributes();
                attributes.SetColorMatrix(colorMatrix);
                g.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 0, 0, image.Width, image.Height,
                            GraphicsUnit.Pixel, attributes);
            }
            return newBitmap;
        }

        /// <summary>
        /// Locks a Bitmap image into system memory and provides a <see cref="PixelData">PixelData</see> object
        /// that provides access to the image's pixel data.
        /// </summary>
        /// <param name="bitmap">The Bitmap image to lock</param>
        /// <param name="flags">Access level (read/write) for the locked zone</param>
        /// <returns>A PixelData object to access the image's pixel data.</returns>
        public static PixelData LockPixels(this Bitmap bitmap, ImageLockMode flags)
        {
            return LockPixels(bitmap, new Rectangle(Point.Empty, bitmap.Size), flags);
        }

        /// <summary>
        /// Locks a Bitmap image into system memory and provides a <see cref="PixelData">PixelData</see> object
        /// that provides access to the image's pixel data.
        /// </summary>
        /// <param name="bitmap">The Bitmap image to lock</param>
        /// <param name="rect">Image area to lock</param>
        /// <param name="flags">Access level (read/write) for the locked zone</param>
        /// <returns>A PixelData object to access the image's pixel data.</returns>
        public static PixelData LockPixels(this Bitmap bitmap, Rectangle rect, ImageLockMode flags)
        {
            return new PixelData(bitmap, rect, flags);
        }
    }
}
