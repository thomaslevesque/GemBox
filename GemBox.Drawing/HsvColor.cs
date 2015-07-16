namespace GemBox.Drawing
{
    /// <summary>
    /// Represents a color in the HSV (hue, saturation, value) color space
    /// </summary>
    public struct HsvColor
    {
        /// <summary>
        /// Initialize a new instance of HsvColor
        /// </summary>
        /// <param name="h">Hue, value between 0° and 360°</param>
        /// <param name="s">Saturation, value between 0 and 1</param>
        /// <param name="v">Value, value between 0 and 1</param>
        public HsvColor(double h, double s, double v)
        {
            H = h;
            S = s;
            V = v;
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
        /// Gets the value of this color
        /// </summary>
        public double V { get; }

        /// <summary>
        /// Convers this HsvColor structure to a human-readable string
        /// </summary>
        /// <returns>A string that consists of the HSV component values.</returns>
        public override string ToString()
        {
            return $"HsvColor [H={H}, S={S}, V={V}]";
        }
    }
}
