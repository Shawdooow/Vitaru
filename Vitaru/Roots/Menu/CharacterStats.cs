using System.Numerics;
using System.Windows.Forms.VisualStyles;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Vitaru.Gamemodes;
using Vitaru.Gamemodes.Characters.Players;

namespace Vitaru.Roots.Menu
{
    public class CharacterStats : ListLayer<IDrawable2D>
    {
        private readonly Text2D name;
        private readonly Text2D health;
        private readonly Text2D energy;
        private readonly Text2D ability;
        private readonly Text2D role;
        private readonly Text2D difficulty;
        private readonly Text2D background;

        public CharacterStats()
        {
            Position = new Vector2(240, -20);
            Size = new Vector2(100, 400);
            Spacing = 2f;

            Children = new[]
            {
                name = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Name"
                },
                health = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Health"
                },
                energy = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Energy"
                },
                ability = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Ability"
                },
                role = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Role"
                },
                difficulty = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Difficulty"
                },
                //background = new Text2D
                //{
                //    ParentOrigin = Mounts.TopLeft,
                //    Origin = Mounts.TopLeft,
                //    FontScale = 0.3f,
                //    Text = "Background"
                //}
            };

            GamemodeStore.SelectedGamemode.OnSelectedCharacterChange += value => change(GamemodeStore.GetPlayer(value));
            change(GamemodeStore.GetPlayer(GamemodeStore.SelectedGamemode.SelectedCharacter));
        }

        private void change(Player player)
        {
            name.Text = $"Name: {player.Name}";
            health.Text = $"Health: {player.HealthCapacity}";
            energy.Text = $"Energy: {player.EnergyCapacity}";
            ability.Text = $"Ability: {player.Ability}";
            role.Text = $"Role: {player.Role}";
            difficulty.Text = $"Difficulty: {player.Difficulty}";
            //background.Text = $"Background: {player.Difficulty}";
        }
    }
}
