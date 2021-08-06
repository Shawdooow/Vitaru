﻿// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using Vitaru.Gamemodes.Vitaru.Chapters.Abilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.Projectiles;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two
{
    public class Claire : Player
    {
        #region Fields

        public override string Name => "Claire";

        public override float HealthCapacity => 40;

        public override float EnergyCapacity => 32;

        public override float EnergyCost => 16;

        public override Color PrimaryColor => "#d9c630".HexToColor();

        public override Color SecondaryColor => "#c9120f".HexToColor();

        public override Color ComplementaryColor => "#757670".HexToColor();

        public override string Ability => "Snap Shot";

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.Hard;

        protected Camera Camera;

        protected Sprite Screenshot;

        #endregion

        public Claire(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            Gamefield.OverlaysLayer.Add(Camera = new Camera(this, Gamefield.OverlaysLayer)
            {
                OnScreenshot = screenshot => Screenshot = screenshot
            });
        }

        public override void Update()
        {
            base.Update();

            if (Screenshot != null)
            {
                foreach (Gamefield.ProjectilePack pack in Gamefield.ProjectilePacks)
                {
                    if (pack.Team == Team) continue;

                    IReadOnlyList<Projectile> projectiles = pack.Children;
                    for (int i = 0; i < projectiles.Count; i++)
                    {
                        Projectile projectile = projectiles[i];

                        //Hack to disable bullets we shouldn't interact with
                        if (!projectile.Active)
                            continue;

                        switch (projectile)
                        {
                            default:
                                continue;
                            case Bullet bullet:
                                Vector2 border = Camera.Hitbox.Size / 2;

                                if (bullet.CircularHitbox.Position.X >= Camera.Hitbox.Position.X - border.X && 
                                    bullet.CircularHitbox.Position.X <= Camera.Hitbox.Position.X + border.X &&
                                    bullet.CircularHitbox.Position.Y >= Camera.Hitbox.Position.Y - border.Y &&
                                    bullet.CircularHitbox.Position.Y <= Camera.Hitbox.Position.Y + border.Y)
                                {
                                    Gamefield.Remove(projectile);
                                    projectile.Collision();
                                    break;
                                }
                                else
                                    continue;
                        }
                    }
                }

                Screenshot = null;
            }

            Camera.Position = InputManager.Mouse.Position;
        }

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);

            if (action == VitaruActions.Spell)
                Camera.QueueScreenshot();
        }
    }
}