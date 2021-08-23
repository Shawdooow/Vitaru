using Prion.Mitochondria;

namespace Vitaru.Play.Characters.Bosses
{
    public class DrawableBoss : DrawableCharacter
    {
        public DrawableBoss(Character character) : base(character, Game.TextureStore.GetTexture("Gameplay\\boss.png"))
        {
        }
    }
}
