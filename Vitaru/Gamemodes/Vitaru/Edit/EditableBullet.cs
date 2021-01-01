// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Vitaru.Editor.Editables;
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
    }
}