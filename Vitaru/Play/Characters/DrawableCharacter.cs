// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;

namespace Vitaru.Play.Characters
{
    public abstract class DrawableCharacter : DrawableGameEntity
    {
        public override string Name { get; set; } = nameof(DrawableCharacter);

        public override Vector2 Position { get; set; }

        public override Vector2 Size
        {
            get => CharacterSprite.Size;
            set => CharacterSprite.Size = value;
        }

        public virtual float Diameter
        {
            get => Hitbox.Width;
            set
            {
                Vector2 size = new(value);
                Hitbox.Size = size;
                HitboxOutline.Size = size;
            }
        }

        public override Vector2 Scale
        {
            get => CharacterSprite.Scale;
            set => CharacterSprite.Scale = value;
        }

        public override float Alpha
        {
            get => CharacterSprite.Alpha;
            set => CharacterSprite.Alpha = value;
        }

        public virtual float HitboxAlpha
        {
            get => Hitbox.Alpha;
            set
            {
                Hitbox.Alpha = value;
                HitboxOutline.Alpha = value;
            }
        }

        public virtual Texture Texture
        {
            get => CharacterSprite.Texture;
            set => CharacterSprite.Texture = value;
        }

        public override Color Color
        {
            get => CharacterSprite.Color;
            set => CharacterSprite.Color = value;
        }

        public virtual Color SecondaryColor
        {
            get => HitboxOutline.Color;
            set => HitboxOutline.Color = value;
        }

        public virtual Color ComplementaryColor { get; set; }

        public virtual Color HitboxColor
        {
            get => Hitbox.Color;
            set => Hitbox.Color = value;
        }

        protected Sprite CharacterSprite;
        protected Circle HitboxOutline;
        protected Circle Hitbox;

        protected Layer2D<IDrawable2D> CharacterLayer;
        protected Layer2D<IDrawable2D> ParentLayer;

        protected DrawableCharacter(Layer2D<IDrawable2D> layer)
        {
            ParentLayer = layer;
            ParentLayer.Add(CharacterLayer = new Layer2D<IDrawable2D>());

            CharacterLayer.Add(CharacterSprite = new Sprite());
            CharacterLayer.Add(HitboxOutline = new Circle
            {
                Alpha = 0,
                Scale = new Vector2(1.75f)
            });
            CharacterLayer.Add(Hitbox = new Circle
            {
                Alpha = 0,
            });
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);

            ParentLayer.Remove(CharacterLayer);
            ParentLayer = null;
        }
    }
}