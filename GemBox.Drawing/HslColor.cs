namespace GemBox.Drawing
{
    /// <summary>
    /// Represents a color in the HSL (hue, saturation, lightness) color space
    /// </summary>
    public struct HslColor
    {
        /// <summary>
        /// Initialize a new instance of HslColor
        /// </summary>
        /// <param name="h">Hue, value between 0° and 360°</param>
        /// <param name="s">Saturation, value between 0 and 1</param>
        /// <param name="l">Lightness, value between 0 and 1</param>
        public HslColor(double h, double s, double l)
        {
            H = h;
            S = s;
            L = l;
        }

        /// <summary>
        /// Gets the hue of this color
        /// </summary>
        public double H { get; }

        /// <summary>
        /// Gets the saturation of this color
        /// </summary>
        public double S { get; }

        /// <summary>
        /// Gets the lightness of this color
        /// </summary>
        public double L { get; }

        /// <summary>
        /// Convers this HslColor structure to a human-readable string
        /// </summary>
        /// <returns>A string that consists of the HSL component values.</returns>
        public override string ToString()
        {
            return $"HsvColor [H={H}, S={S}, L={L}]";
        }
    }
}