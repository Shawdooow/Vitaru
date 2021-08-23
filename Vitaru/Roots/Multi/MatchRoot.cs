using Prion.Nucleus.Entitys;
using Prion.Nucleus.Groups.Packs;
using Vitaru.Server.Match;

namespace Vitaru.Roots.Multi
{
    public class MatchRoot : MultiRoot
    {
        public MatchRoot(MatchInfo match, Pack<Updatable> networking) : base(networking)
        {
        }
    }
}
