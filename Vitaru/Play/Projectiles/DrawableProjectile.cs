// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Nucleus.Utilities;
using Vitaru.Graphics.Projectiles.Bullets;

namespace Vitaru.Play.Projectiles
{
    public abstract class DrawableProjectile : DrawableGameEntity
    {
        public override string Name { get; set; } = nameof(DrawableProjectile);

        protected BulletLayer Layer;

        protected int Location;

        public override Vector2 Position
        {
            get => Layer.bPosition[Location];
            set => Layer.bPosition[Location] = value;
        }

        public override Vector2 Size
        {
            get => Layer.bSize[Location];
            set => Layer.bSize[Location] = value;
        }

        public override Vector2 Scale { get; set; }

        public override float Alpha
        {
            get => Layer.bGlowColor[Location].W;
            set => Layer.bGlowColor[Location] = Color.Vector(value);
        }

        public override Color Color
        {
            get => Layer.bGlowColor[Location].XYZUnsafeReadonly().Color();
            set => Layer.bGlowColor[Location] = value.Vector(Alpha);
        }

        protected DrawableProjectile(BulletLayer layer, int location)
        {
            Layer = layer;
            Location = location;
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            Layer = null;
        }
    }
}