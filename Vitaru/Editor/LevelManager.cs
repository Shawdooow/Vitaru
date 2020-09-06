﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using Vitaru.Editor.Editables;
using Vitaru.Editor.Editables.Properties;
using Vitaru.Server.Track;

namespace Vitaru.Editor
{
    public class LevelManager
    {
        public readonly Level Level;

        public EditableGenerator SelectedGenerator { get; private set; }

        public event Action<EditableGenerator> GeneratorSet;

        public IEditable SelectedEditable { get; private set; }

        public event Action<IEditable> EditableSet;

        public EditableProperty[] Properties { get; private set; } = new EditableProperty[0];

        public event Action<EditableProperty[]> PropertiesSet;

        public LevelManager(Level level)
        {
            Level = level;
        }

        public void SetGenerator(EditableGenerator generator)
        {
            SelectedGenerator = generator;
            GeneratorSet?.Invoke(generator);
        }

        public void SetEditable(IEditable editable)
        {
            SelectedEditable = editable;
            EditableSet?.Invoke(editable);
        }

        public void SetProperties(EditableProperty[] properties)
        {
            Properties = properties;
            PropertiesSet?.Invoke(properties);
        }
    }
}