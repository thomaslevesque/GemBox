using System.Drawing.Imaging;

namespace GemBox.Drawing
{
    /// <summary>
    /// Provides common color transform matrices
    /// </summary>
    public static class ColorTransforms
    {
        static ColorTransforms()
        {
            GrayScale = new ColorMatrix(
                new[]
                {
                    new float[] {.3f, .3f, .3f, 0, 0},
                    new float[] {.59f, .59f, .59f, 0, 0},
                    new float[] {.11f, .11f, .11f, 0, 0},
                    new float[] {0f, 0f, 0f, 1f, 0f},
                    new float[] {0, 0, 0, 0, 1}
                });

            Invert = new ColorMatrix(
                new[]
                {
                    new float[] {-1, 0, 0, 0, 0},
                    new float[] {0, -1, 0, 0, 0},
                    new float[] {0, 0, -1, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {1, 1, 1, 0, 1},
                });

            Sepia = new ColorMatrix(
                new[]
                {
                    new float[] {0.393f, 0.349f, 0.272f, 0, 0},
                    new float[] {0.769f, 0.686f, 0.534f, 0, 0},
                    new float[] {0.189f, 0.168f, 0.131f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {0, 0, 0, 0, 1}
                });

            Polaroid = new ColorMatrix(
                new[]
                {
                    new float[] {1.438f, -.062f, -.062f, 0, 0},
                    new float[] {-.122f, 1.378f, -.122f, 0, 0},
                    new float[] {-.016f, -.016f, 1.483f, 0, 0},
                    new float[] {0, 0, 0, 1, 0},
                    new float[] {-.03f, .05f, -.02f, 0, 1}
                });
        }

        /// <summary>
        /// Grayscale transform
        /// </summary>
        public static ColorMatrix GrayScale { get; }

        /// <summary>
        /// Invert transform
        /// </summary>
        public static ColorMatrix Invert { get; }

        /// <summary>
        /// Sepia transform
        /// </summary>
        public static ColorMatrix Sepia { get; }

        /// <summary>
        /// Polaroid transform
        /// </summary>
        public static ColorMatrix Polaroid { get; }
    }
}
