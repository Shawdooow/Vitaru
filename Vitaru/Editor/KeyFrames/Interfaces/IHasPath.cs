using System.Numerics;

namespace Vitaru.Editor.KeyFrames.Interfaces
{
    public interface IHasPath : IHasPosition
    {
        float Distance { get; set; }

        CurveType CurveType { get; set; }

        float CurveAmount { get; set; }
    }

    public enum CurveType
    {
        Straight,
        Target,

        Bezier,
    }
}
