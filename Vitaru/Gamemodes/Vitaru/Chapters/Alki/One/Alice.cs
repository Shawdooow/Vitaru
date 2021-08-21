// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.One
{
    public class Alice : Player
    {
        #region Fields

        public override string Name => "Alice";

        public override float HealthCapacity => 40;

        public override float EnergyCapacity => 24;

        public override float EnergyCost => 12;

        public override float EnergyDrainRate => 6;

        public override Color PrimaryColor => "#fc0330".HexToColor();

        public override Color SecondaryColor => "#363636".HexToColor();

        public override Color ComplementaryColor => "#7da1a8".HexToColor();

        public override string Ability => "Unbound";

        public override Role Role => Role.Specialized;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override bool Implemented => true;

        /// <summary>
        /// This here is me homunculus flesh puppet that me soul will transfer to, in the event of me death!
        /// -Eugene Krabs
        /// </summary>
        protected Alice Homunculus;

        protected Sprite TargetA;
        protected Sprite TargetB;

        #endregion

        public Alice(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Gamefield.OverlaysLayer.Add(TargetA = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
            {
                Position = new Vector2(GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize().X / -2 + 200,
                    GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize().Y / 2 - 100),
                Size = new Vector2(50),
                Alpha = 0.5f,
                Color = PrimaryColor
            });

            Gamefield.OverlaysLayer.Add(TargetB = new Sprite(Game.TextureStore.GetTexture("Gameplay\\glow.png"))
            {
                Position = new Vector2(GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize().X / -2 + 100,
                    GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize().Y / 2 - 200),
                Size = new Vector2(50),
                Alpha = 0.5f,
                Color = SecondaryColor
            });
        }

        protected override bool CheckSpellActivate(VitaruActions action)
        {
            if (action == VitaruActions.ModifierOne)
                return true;
            if (action == VitaruActions.ModifierTwo)
                return true;

            if (Health > HealthCapacity / 2)
                return base.CheckSpellActivate(action);
            return false;
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);

            switch (action)
            {
                case VitaruActions.ModifierOne when Homunculus != null:
                    TargetA.Position = InputManager.Mouse.Position;
                    Homunculus.TargetPositions[0] = TargetA.Position;
                    break;
                case VitaruActions.ModifierTwo when Homunculus != null:
                    TargetB.Position = InputManager.Mouse.Position;
                    Homunculus.TargetPositions[1] = TargetB.Position;
                    break;
                case VitaruActions.Spell:
                    TakeDamage(HealthCapacity / 2);

                    TargetA.Alpha = 1;
                    TargetB.Alpha = 1;

                    Drawable.Alpha = 0.5f;
                    Gamefield.Shade = Shades.Gray;
                    Gamefield.Intensity = 0.2f;

                    Gamefield.Add(Homunculus = new Alice(Gamefield)
                    {
                        Position = Position,
                        Health = HealthCapacity / 2,
                        AI = true,
                        TargetPositions = new List<Vector2>
                        {
                            TargetA.Position,
                            TargetB.Position
                        },
                        OnDie = () => SpellDeactivate(VitaruActions.Spell)
                    });
                    break;
            }
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);

            switch (action)
            {
                case VitaruActions.Spell:
                    TargetA.Alpha = 0.5f;
                    TargetB.Alpha = 0.5f;

                    Drawable.Alpha = 1f;
                    Gamefield.Shade = Shades.None;
                    Gamefield.Intensity = 1f;

                    if (Homunculus != null)
                    {
                        Heal(Homunculus.Health);
                        Gamefield.Remove(Homunculus);
                        Homunculus = null;
                    }
                    break;
            }
        }

        protected override void Die()
        {
            base.Die();
            if (SpellActive && !Homunculus.Dead)
            {
                Gamefield.Remove(this);
                //"Transfer our soul back to our body"
                Homunculus.AI = false;
            }

            if (Homunculus != null)
                Homunculus.OnDie = null;
        }
    }
}