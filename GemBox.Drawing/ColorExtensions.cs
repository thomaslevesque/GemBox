using System;
using System.Drawing;

namespace GemBox.Drawing
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Convertit une couleur en niveau de gris
        /// </summary>
        /// <param name="color">Couleur à convertir</param>
        /// <returns>Couleur équivalente en niveau de gris</returns>
        public static Color ToGrayScale(this Color color)
        {
            byte gray = (byte)Math.Round(color.R * .3f + color.G * .59f + color.B * .11f);
            return Color.FromArgb(color.A, gray, gray, gray);
        }

        /// <summary>
        /// Convertit une couleur RGB en couleur HSL
        /// </summary>
        /// <param name="color">Couleur RGB à convertir</param>
        /// <returns>Couleur équivalente dans l'espace colorimétrique HSL</returns>
        public static HslColor ToHsl(this Color color)
        {
            double hue = color.GetHue();
            double saturation = color.GetSaturation();
            double lightness = color.GetBrightness();

            return new HslColor(hue, saturation, lightness);
        }

        /// <summary>
        /// Convertit une couleur HSL en couleur RGB
        /// </summary>
        /// <param name="hslColor">Couleur HSL à convertir</param>
        /// <returns>Couleur équivalente dans l'espace colorimétrique RGB</returns>
        public static Color ToRgb(this HslColor hslColor)
        {
            double hue = hslColor.H;
            double saturation = hslColor.S;
            double lightness = hslColor.L;

            byte r, g, b;
            if (saturation.Equals(0.0))
            {
                r = (byte)Math.Round(lightness * 255d);
                g = (byte)Math.Round(lightness * 255d);
                b = (byte)Math.Round(lightness * 255d);
            }
            else
            {
                double t1, t2;
                double th = hue / 360.0d;

                if (lightness < 0.5d)
                {
                    t2 = lightness * (1d + saturation);
                }
                else
                {
                    t2 = (lightness + saturation) - (lightness * saturation);
                }
                t1 = 2d * lightness - t2;

                var tr = th + (1.0d / 3.0d);
                var tg = th;
                var tb = th - (1.0d / 3.0d);

                tr = ColorCalc(tr, t1, t2);
                tg = ColorCalc(tg, t1, t2);
                tb = ColorCalc(tb, t1, t2);
                r = (byte)Math.Round(tr * 255d);
                g = (byte)Math.Round(tg * 255d);
                b = (byte)Math.Round(tb * 255d);
            }
            return Color.FromArgb(r, g, b);
        }

        private static double ColorCalc(double c, double t1, double t2)
        {

            if (c < 0) c += 1d;
            if (c > 1) c -= 1d;
            if (6.0d * c < 1.0d) return t1 + (t2 - t1) * 6.0d * c;
            if (2.0d * c < 1.0d) return t2;
            if (3.0d * c < 2.0d) return t1 + (t2 - t1) * (2.0d / 3.0d - c) * 6.0d;
            return t1;
        }

        /// <summary>
        /// Convertit une couleur RGB en couleur HSV
        /// </summary>
        /// <param name="color">Couleur RGB à convertir</param>
        /// <returns>Couleur équivalente dans l'espace colorimétrique HSV</returns>
        public static HsvColor ToHsv(this Color color)
        {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            double hue = color.GetHue();
            double saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            double value = max / 255d;

            return new HsvColor(hue, saturation, value);
        }

        /// <summary>
        /// Convertit une couleur HSV en couleur RGB
        /// </summary>
        /// <param name="hsvColor">Couleur HSL à convertir</param>
        /// <returns>Couleur équivalente dans l'espace colorimétrique RGB</returns>
        public static Color ToRgb(this HsvColor hsvColor)
        {
            double hue = hsvColor.H;
            double saturation = hsvColor.S;
            double value = hsvColor.V;

            int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            value = value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - saturation));
            int q = Convert.ToInt32(value * (1 - f * saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

            if (hi == 0)
                return Color.FromArgb(255, v, t, p);
            if (hi == 1)
                return Color.FromArgb(255, q, v, p);
            if (hi == 2)
                return Color.FromArgb(255, p, v, t);
            if (hi == 3)
                return Color.FromArgb(255, p, q, v);
            if (hi == 4)
                return Color.FromArgb(255, t, p, v);
            return Color.FromArgb(255, v, p, q);
        }
    }
}
