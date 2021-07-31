// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using Vitaru.Editor.Editables;
using Vitaru.Play;
using Vitaru.Play.Characters.Enemies;

namespace Vitaru.Gamemodes.Vitaru.Edit
{
    public class EditableEnemy : EditableGenerator
    {
        public override IEditable GetEditable(Gamefield field) => new Enemy(field)
        {
            Color = Color.GreenYellow
        };
    }
}