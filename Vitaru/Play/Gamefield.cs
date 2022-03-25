// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Nucleus.Debug;
using Prion.Nucleus.Groups.Packs;
using Vitaru.Editor.IO;
using Vitaru.Gamemodes;
using Vitaru.Levels;
using Vitaru.Play.Characters;
using Vitaru.Play.Characters.Players;

namespace Vitaru.Play
{
    public class Gamefield : Pack<IPack>
    {
        public static double Current { get; private set; } = double.MinValue;

        public virtual Shades Shade { get; set; }

        public virtual float Intensity { get; set; } = 1;

        protected readonly FormatConverter FormatConverter;

        public readonly Pack<Character> CharacterPack = new()
        {
            Name = "Character Pack",
        };

        public readonly Layer2D<IDrawable2D> CharacterLayer = new()
        {
            Name = "Character Layer",
            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize(),
        };

        public Player ActivePlayer { get; protected set; }

        public readonly GamefieldBorder Border;

        public Gamefield()
        {
            Current = double.MinValue;

            Vector2 size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize();

            Add(CharacterPack);

            FormatConverter = GamemodeStore.SelectedGamemode.Gamemode.GetFormatConverter();
            FormatConverter.Gamefield = this;

            try
            {
                //if (LevelStore.CurrentLevel.EnemyData != null)
                    //UnloadedEnemies.AddRange(FormatConverter.StringToEnemies(LevelStore.CurrentLevel.EnemyData));
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error converting level data to Enemies, purging bad data...", LogType.IO);
                //UnloadedEnemies.Clear();
                LevelStore.CurrentLevel.EnemyData = string.Empty;
            }
        }

        public class GamefieldBorder : Layer2D<Box>
        {
            public GamefieldBorder(Vector2 size)
            {
                Size = size;

                const int w = 2;
                Children = new[]
                {
                    new Box
                    {
                        Height = w,
                        Width = size.X + w,
                        Y = -size.Y / 2,
                    },
                    new Box
                    {
                        Height = w,
                        Width = size.X + w,
                        Y = size.Y / 2,
                    },
                    new Box
                    {
                        Width = w,
                        Height = size.Y + w,
                        X = -size.X / 2,
                    },
                    new Box
                    {
                        Width = w,
                        Height = size.Y + w,
                        X = size.X / 2,
                    },
                };
            }
        }
    }
}