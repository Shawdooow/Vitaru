namespace Vitaru.Play.Characters.Bosses
{
    public class Boss : Character
    {
        public override DrawableGameEntity GenerateDrawable() => new DrawableBoss(this);

        public Boss(Gamefield gamefield) : base(gamefield)
        {
        }
    }
}
