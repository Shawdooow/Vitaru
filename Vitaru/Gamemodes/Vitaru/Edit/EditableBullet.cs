// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Projectiles;

namespace Vitaru.Gamemodes.Vitaru.Edit
{
    public class EditableBullet : EditableProjectile
    {
        public override IEditable GetEditable() => new Bullet
        {
            Color = Color.GreenYellow
        };
    }
}