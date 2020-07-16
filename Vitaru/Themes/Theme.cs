using System.Drawing;

namespace Vitaru.Themes
{
    public abstract class Theme
    {
        public abstract Color PrimaryColor { get; }

        public abstract Color SecondaryColor { get; }

        public abstract Color ComplementaryColor { get; }

        public abstract string Background { get; }
    }
}
