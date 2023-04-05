// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Golgi.Audio.Tracks;
using Prion.Golgi.Utilities;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Input;
using Prion.Nucleus.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Vitaru.Gamemodes.Vitaru.Chapters.Abilities;
using Vitaru.Input;
using Vitaru.Play;
using Vitaru.Play.Characters.Players;
using Vitaru.Play.Projectiles;
using Vitaru.Play.Teams;

namespace Vitaru.Gamemodes.Vitaru.Chapters.Alki.Two
{
    public class Claire : Player
    {
        #region Fields


        public const float BUFFS_DIVISOR = 5;

        public override string Name => "Claire";

        public override float HealthCapacity => 50;

        public override float EnergyCapacity => 24;

        public override float EnergyCost => 12;

        public override Color PrimaryColor => "#d9c630".HexToColor();

        public override Color SecondaryColor => "#c9120f".HexToColor();

        public override Color ComplementaryColor => "#c8c8c8".HexToColor();

        public override string Seal => "Gameplay\\seal alki 2.png";

        public override float SealRotationSpeed => 0.5f;

        public override string Ability => "Snap Shot";

        public override string[] AbilityStats => new[]
        {
            $"Buffs Divisor: {BUFFS_DIVISOR}",
        };

        public override Role Role => Role.Support;

        public override Difficulty Difficulty => Difficulty.Hard;

        public override ImplementationState ImplementationState => ImplementationState.MostlyComplete;

        public override string OriginMedia => "Alki.Two";

        public override string Description => "Has always tried her hardest to keep things working.";

        protected Camera Camera;

        protected Sprite Screenshot;

        private float Buffs;


        #endregion


        public Claire(PlayManager manager) : base(manager) { }

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            PlayManager.Layers.OverlayLayer.Add(Camera = new Camera(this, PlayManager.Layers.OverlayLayer)
            {
                OnScreenshot = screenshot => Screenshot = screenshot,
            });
        }

        public override void Update()
        {
            base.Update();

            if (Screenshot != null)
            {
                Buffs = 0;
                foreach (TeamList team in PlayManager.Teams)
                {
                    if (team.Team == Team) continue;

                    IReadOnlyList<IHasTeam> members = team.Members;
                    for (int i = 0; i < members.Count; i++)
                    {
                        Projectile projectile = (Projectile)members[i];
                        if (projectile == null) continue;

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
                                    Buffs += (1 / BUFFS_DIVISOR);
                                    PlayManager.ProjectileManager.Remove(projectile);
                                    //projectile.Collision();
                                    break;
                                }
                                else
                                    continue;
                        }
                    }
                }

                Buffs = MathF.Round(Buffs, 2);
                Screenshot.ClearTransforms();
                Screenshot.FadeTo(0, TrackManager.CurrentTrack.Metadata.GetBeatLength() * 16, Easings.InCirc)
                    .OnComplete(() => Buffs = 0);
                Screenshot = null;
            }

            Camera.Position = InputManager.Mouse.Position;
        }

        protected override void PatternWave(int count = 3) =>
            base.PatternWave(count + 2 * (int)MathF.Round(Buffs, MidpointRounding.ToZero));

        protected override void SpellActivate(VitaruActions action)
        {
            base.SpellActivate(action);

            if (action == VitaruActions.Spell)
                Camera.QueueScreenshot();
        }

        protected override void SpellUpdate()
        {
            base.SpellUpdate();
            if (DrawablePlayer != null) DrawablePlayer.Seal.LeftValue.Text = $"{Buffs}x";
        }
    }
}