// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Vitaru.Editor.IO;
using Vitaru.Gamemodes.Characters.Enemies;

namespace Vitaru.Gamemodes.Vitaru.Edit
{
    public class EditableEnemy : Editable
    {
        public override IEditable GetEditable() => new Enemy(null);
    }
}