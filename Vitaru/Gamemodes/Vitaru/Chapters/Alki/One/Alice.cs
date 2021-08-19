// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Drawing;
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

        public override bool Implemented => false;

        /// <summary>
        /// This here is me homunculus flesh puppet that me soul will transfer to, in the event of me death!
        /// -Eugene Krabs
        /// </summary>
        protected Alice Homunculus;

        #endregion

        public Alice(Gamefield gamefield) : base(gamefield)
        {
        }

        protected override bool CheckSpellActivate(VitaruActions action)
        {
            if (Health > HealthCapacity / 2)
                return base.CheckSpellActivate(action);
            return false;
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);

            TakeDamage(HealthCapacity / 2);
            Gamefield.Shade = Shades.Gray;

            Gamefield.Add(Homunculus = new Alice(Gamefield)
            {
                Health = HealthCapacity / 2,
                AI = true
            });
        }

        protected override void SpellDeactivate(VitaruActions action)
        {
            base.SpellDeactivate(action);

            Gamefield.Shade = Shades.None;

            Heal(Homunculus.Health);
            Gamefield.Remove(Homunculus);
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (Homunculus.Dead)
                SpellDeactivate(VitaruActions.Spell);
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
        }
    }
}