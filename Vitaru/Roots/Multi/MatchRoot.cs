﻿// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using Vitaru.Server.Match;

namespace Vitaru.Roots.Multi
{
    public class MatchRoot : MultiRoot
    {
        public MatchRoot(MatchInfo match, Pack<Updatable> networking) : base(networking) { }
    }
}