using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Layers._3D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus.Debug;

namespace Vitaru.Play
{
    public class PlayLayers
    {
        public readonly GamefieldBorder Border;

        public readonly Layer2D<IDrawable2D> Layer2Ds;
        public readonly Layer3D<IDrawable3D> Layer3Ds;

        public readonly Layer2D<IDrawable2D> OverlayLayer;
        public readonly Layer2D<IDrawable2D> CharacterLayer;
        public readonly Layer2D<IDrawable2D> ProjectileLayer;

        public readonly float MaxBarSize;

        public readonly Box HealthBar;
        public readonly Box HealthChange;
        public readonly Text2D MaxHealthText;
        public readonly Text2D CurrentHealthText;

        public readonly Box EnergyBar;
        public readonly Box EnergyChange;
        public readonly Text2D MaxEnergyText;
        public readonly Text2D CurrentEnergyText;

        public Shades Shade;
        public float Intensity;

        public PlayLayers()
        {
            Debugger.Assert(Game.DrawThreaded);
        }
    }
}
