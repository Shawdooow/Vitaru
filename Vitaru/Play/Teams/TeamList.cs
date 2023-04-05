﻿// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Golgi.Utilities;
using Prion.Nucleus.Utilities.Interfaces;
using System.Collections.Generic;

namespace Vitaru.Play.Teams
{
    public class TeamList : IHasName, IHasTeam
    {
        public string Name { get; set; } = "Team";

        public int Team
        {
            get => team;
            set
            {
                team = value;
                for (int i = 0; i < Members.Count; i++)
                    Members[i].Team = value;
            }
        }

        private int team;

        public IReadOnlyList<IHasTeam> Members
        {
            get => ProtectedMembers.AsReadOnly();
            set
            {
                ProtectedMembers = new List<IHasTeam>();

                for (int i = 0; i < value.Count; i++)
                    Add(value[i]);
            }
        }

        protected List<IHasTeam> ProtectedMembers = new();

        public void Add(IHasTeam m)
        {
            m.Team = Team;
            ProtectedMembers.Add(m);
        }

        public void Remove(IHasTeam m)
        {
            ProtectedMembers.Remove(m);
        }
    }
}
