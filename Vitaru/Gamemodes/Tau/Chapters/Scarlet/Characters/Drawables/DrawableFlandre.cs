// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Scarlet.Characters.Drawables
{
    public class DrawableFlandre : DrawableTouhosuPlayer
    {
        private List<DrawableFlandre> puppets = new List<DrawableFlandre>();

        private DrawableFlandre masterFlandre;

        public DrawableFlandre(VitaruPlayfield playfield)
            : base(playfield, new Flandre())
        {
        }

        /// <summary>
        ///     This ctor should only be used by "Four of a Kind"
        /// </summary>
        /// <param name="flandre"></param>
        public DrawableFlandre(DrawableFlandre flandre)
            : base(flandre.VitaruPlayfield, new Flandre())
        {
            masterFlandre = flandre;
            ControlType = ControlType.Puppet;
        }

        /// <summary>
        ///     Called when our master dies, they will become us!
        /// </summary>
        /// <param name="flandre"></param>
        public void TakeOver(DrawableFlandre flandre)
        {
            flandre.puppets.Remove(this);
            puppets = flandre.puppets;
            Actions = flandre.Actions;
            SpellActive = flandre.SpellActive;
            ControlType = flandre.ControlType;
            VitaruPlayfield.Player = this;

            foreach (DrawableFlandre puppet in puppets)
                puppet.masterFlandre = this;
        }

        protected override void SpellActivate(VitaruAction action)
        {
            base.SpellActivate(action);

            float quadWidth = CurrentPlayfield.Size.X / 4;
            float quadXPos;
            int quad;

            if (X <= quadWidth)
            {
                quad = 1;
                quadXPos = Position.X;
            }
            else if (X > quadWidth && X <= quadWidth * 2)
            {
                quad = 2;
                quadXPos = Position.X - quadWidth;
            }
            else if (X > quadWidth * 2 && X <= quadWidth * 3)
            {
                quad = 3;
                quadXPos = Position.X - quadWidth * 2;
            }
            else
            {
                quad = 4;
                quadXPos = Position.X - quadWidth * 3;
            }

            int j = 4;

            for (int i = 1; i < j; i++)
            {
                if (i == quad)
                {
                    j++;
                    continue;
                }

                DrawableFlandre flan;
                CurrentPlayfield.Add(flan = new DrawableFlandre(this)
                    {Position = new Vector2(quadXPos + i * quadWidth, Position.Y), Team = Team});
                puppets.Add(flan);
            }
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();

            if (SpellActive)
                Drain(Clock.ElapsedFrameTime / 1000 * TouhosuPlayer.EnergyDrainRate);
        }

        protected override void SpellDeactivate(VitaruAction action)
        {
            base.SpellDeactivate(action);

            restart:
            foreach (DrawableFlandre flan in puppets)
            {
                flan.Hurt(99999);
                goto restart;
            }

            puppets = new List<DrawableFlandre>();
        }

        protected override void Death()
        {
            if (puppets.Count > 0)
            {
                //TODO: Animation for this
                Dead = true;
                puppets.First().TakeOver(this);
                Expire();
            }
            else if (ControlType == ControlType.Puppet)
            {
                masterFlandre.puppets.Remove(this);
                Dead = true;
                Expire();
            }
        }
    }
}