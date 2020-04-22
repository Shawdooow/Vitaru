// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Drawing;
using System.Numerics;
using Prion.Application.Utilities;
using Prion.Game.Graphics.Transforms;
using Prion.Game.Input.Events;
using Prion.Game.Input.Handlers;
using Prion.Game.Input.Receivers;
using Vitaru.Gamemodes.Projectiles;
using Vitaru.Input;
using Vitaru.Play;

namespace Vitaru.Gamemodes.Characters.Players
{
    public class Player : Character, IHasInputKeys<VitaruActions>, IHasInputMousePosition
    {
        public const int PLAYER_TEAM = 1;

        public override float HitboxDiameter => 6f;

        public override Color PrimaryColor => Color.Navy;

        public override Color SecondaryColor => "#92a0dd".HexToColor();

        public override Color ComplementaryColor => "#d6d6d6".HexToColor();

        public BindInputHandler<VitaruActions> InputHandler { get; set; }

        private Vector2 cursor = Vector2.Zero;

        private double shootTime;
        private const double shoot_speed = 250;

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
                PatternWave();

            //TODO: fix this being needed?
            if (Drawable == null) return;

            Drawable.Position = GetNewPlayerPosition(0.1f);
        }

        protected virtual void PatternWave()
        {
            shootTime = Clock.Current + shoot_speed;

            const int numberbullets = 3;
            float directionModifier = -0.2f;

            float cursorAngle = 0;

            if (InputHandler.Actions[VitaruActions.Sneak])
            {
                cursorAngle = ((float) Math.Atan2(cursor.Y - Drawable.Position.Y, cursor.X - Drawable.Position.X))
                    .ToDegrees() + 90;
                directionModifier = -0.1f;
            }

            for (int i = 1; i <= numberbullets; i++)
            {
                float size;
                float damage;
                Color color;

                if (i % 2 == 0)
                {
                    size = 28;
                    damage = 24;
                    color = PrimaryColor;
                }
                else
                {
                    size = 20;
                    damage = 18;
                    color = SecondaryColor;
                }

                //-90 = up
                BulletAddRad(1, (cursorAngle - 90).ToRadians() + directionModifier, color, size, damage);

                if (InputHandler.Actions[VitaruActions.Sneak])
                    directionModifier += 0.1f;
                else
                    directionModifier += 0.2f;
            }
        }

        protected virtual void BulletAddRad(float speed, float angle, Color color, float size, float damage)
        {
            Bullet bullet = new Bullet
            {
                Team = Team,
                StartPosition = Drawable.Position,
                StartTime = Clock.Current,
                Distance = 600,

                Speed = speed,
                Angle = angle,
                Color = color,
                Diameter = size,
                Damage = damage,
            };

            Gamefield.Add(bullet);
        }

        public bool Pressed(VitaruActions t)
        {
            switch (t)
            {
                default:
                    return true;
                case VitaruActions.Sneak:
                    Drawable.Hitbox.FadeTo(1f, 200);
                    return true;
                case VitaruActions.Shoot:
                    PatternWave();
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

                case VitaruActions.Sneak:
                    Drawable.Hitbox.ClearTransforms();
                    Drawable.Hitbox.FadeTo(0f, 200);
                    return true;
            }
        }

        public void OnMouseMove(MousePositionEvent e) => cursor = e.Position;

        protected virtual Vector2 GetNewPlayerPosition(double playerSpeed)
        {
            Vector2 playerPosition = Drawable.Position;

            double yTranslationDistance = playerSpeed * Clock.LastElapsedTime;
            double xTranslationDistance = playerSpeed * Clock.LastElapsedTime;

            if (InputHandler.Actions[VitaruActions.Sneak])
            {
                xTranslationDistance /= 2d;
                yTranslationDistance /= 2d;
            }

            if (InputHandler.Actions[VitaruActions.Up])
                playerPosition.Y -= (float) yTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Down])
                playerPosition.Y += (float) yTranslationDistance;

            if (InputHandler.Actions[VitaruActions.Left])
                playerPosition.X -= (float) xTranslationDistance;
            if (InputHandler.Actions[VitaruActions.Right])
                playerPosition.X += (float) xTranslationDistance;

            //if (!VitaruPlayfield.BOUNDLESS)
            //{
            //    playerPosition = Vector2.ComponentMin(playerPosition, ChapterSet.PlayfieldBounds.Zw);
            //    playerPosition = Vector2.ComponentMax(playerPosition, ChapterSet.PlayfieldBounds.Xy);
            //}

            return playerPosition;
        }
    }
}