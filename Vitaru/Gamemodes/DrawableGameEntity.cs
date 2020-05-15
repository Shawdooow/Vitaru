using System;
using Prion.Game.Graphics.Drawables;
using Prion.Game.Graphics.Layers;
using Vitaru.Gamemodes.Projectiles;

namespace Vitaru.Gamemodes
{
    public class DrawableGameEntity : Layer2D<IDrawable2D>
    {
        public override string Name { get; set; } = nameof(DrawableGameEntity);

        public event Action OnDelete;

        /// <summary>
        ///     Tells this <see cref="DrawableGameEntity" /> to remove itself from our Parent and Dispose
        /// </summary>
        public virtual void Delete() => OnDelete?.Invoke();
    }
}
