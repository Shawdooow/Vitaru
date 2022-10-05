using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Layers._3D;

namespace Vitaru.Play
{
    public class PlayLayers
    {
        public readonly GamefieldBorder Border;

        public readonly Layer2D<IDrawable2D> Layer2Ds;
        public readonly Layer3D<IDrawable3D> Layer3Ds;
    }
}
