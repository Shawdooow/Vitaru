// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Linq;
using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Roots.Menu
{
    public class CharacterStats : ListLayer<IDrawable2D>
    {
        private readonly Text2D name;
        private readonly Text2D health;
        private readonly Text2D energy;
        private readonly Text2D energyCost;
        private readonly Text2D energyDrain;
        private readonly Text2D ability;
        private readonly Text2D role;
        private readonly Text2D difficulty;
        private readonly Text2D notes;
        private readonly Text2D origin;
        private readonly Text2D description;

        public CharacterStats()
        {
            Position = new Vector2(380, -20);
            Size = new Vector2(100, 400);
            Spacing = 2f;

            Children = new[]
            {
                name = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Name",
                },
                health = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Health",
                },
                energy = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Energy",
                },
                energyCost = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Energy Cost",
                },
                energyDrain = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Energy Drain Rate",
                },
                ability = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Ability",
                },
                role = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Role",
                },
                difficulty = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Difficulty",
                },


                notes = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Background",
                    FixedWidth = 240,
                },
                origin = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Origin",
                    FixedWidth = 240,
                },
                description = new Text2D
                {
                    ParentOrigin = Mounts.TopLeft,
                    Origin = Mounts.TopLeft,
                    FontScale = 0.3f,
                    Text = "Background",
                    FixedWidth = 240,
                },
            };

            GamemodeStore.SelectedGamemode.OnSelectedCharacterChange += value => change(GamemodeStore.GetPlayer(value));
            change(GamemodeStore.GetPlayer(GamemodeStore.SelectedGamemode.SelectedCharacter));
        }

        private void change(Player player)
        {
            name.Text = $"Name: {player.Name}";
            health.Text = $"Health: {player.HealthCapacity}HP";
            energy.Text = $"Energy: {player.EnergyCapacity}SP";
            energyCost.Text = $"Energy Cost: {player.EnergyCost}SP";
            energyDrain.Text = $"Energy Drain Rate: {player.EnergyDrainRate}SP/s";
            ability.Text = $"Ability: {player.Ability}";
            //AbilityStats get put here down below
            role.Text = $"Role: {player.Role.GetDescription()}";
            difficulty.Text = $"Difficulty: {player.Difficulty.GetDescription()}";
            //implemented gets put here down below
            notes.Text = $"Notes: {player.Notes}";
            origin.Text = $"Origin: {player.OriginMedia}";
            description.Text = $"{player.Description}";

            //Don't remove the first 6
            while (ProtectedChildren.Count > 6)
                Remove(ProtectedChildren.Last(), false);

            if (player.AbilityStats != null)
                foreach (string stat in player.AbilityStats)
                    Add(new Text2D
                    {
                        ParentOrigin = Mounts.TopLeft,
                        Origin = Mounts.TopLeft,
                        FontScale = 0.3f,
                        Text = stat,
                    });

            Add(role);
            Add(difficulty);

            if (player.Notes != string.Empty) Add(notes);
            if (player.OriginMedia != string.Empty) Add(origin);

#if !PUBLIC
            if (player.Description != string.Empty) Add(description);
#endif
        }
    }
}