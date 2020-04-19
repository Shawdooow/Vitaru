﻿// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;

namespace Vitaru.Server.Server
{
    [Serializable]
    public class Setting<T> : Setting
    {
        public T Value;
    }

    [Serializable]
    public class Setting
    {
        public string Name;

        public Sync Sync;
    }

    public enum Sync
    {
        None,
        Client,
        All,
    }
}