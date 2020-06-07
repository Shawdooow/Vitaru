// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Sprites;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Vitaru.Edit
{
    public class EditableBullet : EditableProjectile
    {
        public override IEditable GetEditable(Gamefield field) => new Bullet
        {
            Color = Color.GreenYellow
        };

        public override IDrawable2D GetOverlay(DrawableGameEntity draw) =>
            new Sprite(Game.TextureStore.GetTexture("Edit\\enemyOutline.png"))
            {
                Color = Color.Yellow
            };
    }
}