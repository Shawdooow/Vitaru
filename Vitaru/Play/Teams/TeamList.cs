using System.Collections.Generic;
using Prion.Golgi.Utilities;
using Prion.Nucleus.Utilities.Interfaces;

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
