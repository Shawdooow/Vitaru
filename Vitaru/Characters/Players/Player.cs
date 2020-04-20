// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Game.Audio;
using Prion.Game.Audio.OpenAL;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Input.Handlers;
using Prion.Game.Input.Receivers;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Projectiles;

namespace Vitaru.Characters.Players
{
    public class Player : Character, IHasInputKeys<VitaruActions>
    {
        public const int PLAYER_TEAM = 1;

        public override float HitboxDiameter => 6f;

        public BindInputHandler<VitaruActions> InputHandler { get; set; }

        private double shootTime;

        public virtual DrawablePlayer GenerateDrawable()
        {
            DrawablePlayer draw = new DrawablePlayer(this)
            {
                Position = new Vector2(0, 200),
            };
            Drawable = draw;
            return draw;
        }

        public Player(Gamefield gamefield) : base(gamefield)
        {
            Team = PLAYER_TEAM;
            InputHandler = new VitaruInputManager();
            InputHandler.Add(this);
        }

        public override void Update()
        {
            base.Update();

            if (InputHandler.Actions[VitaruActions.Shoot] && Clock.Current >= shootTime)
            {
                shoot();
                shootTime = Clock.Current + 250;
            }

            //TODO: fix this being needed?
            if (Drawable == null) return;

            Drawable.Position = GetNewPlayerPosition(0.1f);
        }

        private void shoot()
        {
            Bullet bullet = new Bullet
            {
                Team = Team,
                StartPosition = Drawable.Position,
                StartTime = Clock.Current,
                Damage = 20,
                Diameter = 20f,
                Distance = 800,
                Speed = 1,
            };

            Gamefield.Add(bullet);
        }
        
        public bool Pressed(VitaruActions t)
        {
            switch (t)
            {
                default:
                    return true;
                case VitaruActions.Slow:
                    Drawable.Hitbox.FadeTo(1f, 200);
                    return true;
                case VitaruActions.Spell:
                    return true;
            }
        }

        public bool Released(VitaruActions t)
        {
            switch (t)
            {
                default:
                    return true;

                case VitaruActions.Slow:
                    Drawable.Hitbox.ClearTransforms();
                    Drawable.Hitbox.FadeTo(0f, 200);
                    return true;
            }
        }

        protected virtual Vector2 GetNewPlayerPosition(double playerSpeed)
        {
            Vector2 playerPosition = Drawable.Position;

            double yTranslationDistance = playerSpeed * Clock.LastElapsedTime;
            double xTranslationDistance = playerSpeed * Clock.LastElapsedTime;

            if (InputHandler.Actions[VitaruActions.Slow])
            {
                xTranslationDistance /= 2d;
                yTranslationDistance /= 2d;
            }

            if (InputHandler.Actions[VitaruActions.Up])
                playerPosition.Y -= (float) yTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Left])
                playerPosition.X += (float) xTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Down])
                playerPosition.Y += (float) yTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Right])
                playerPosition.X -= (float) xTranslationDistance;

            //if (!VitaruPlayfield.BOUNDLESS)
            //{
            //    playerPosition = Vector2.ComponentMin(playerPosition, ChapterSet.PlayfieldBounds.Zw);
            //    playerPosition = Vector2.ComponentMax(playerPosition, ChapterSet.PlayfieldBounds.Xy);
            //}

            return playerPosition;
        }
    }
}