using Microsoft.VisualBasic.Devices;
using Prion.Golgi.Audio.Tracks;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Mitochondria.Graphics.Layers._3D;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Text;
using Prion.Nucleus.Debug;
using System.Drawing;
using System.Numerics;
using Vitaru.Gamemodes;

namespace Vitaru.Play
{
    public class PlayLayers
    {
        public readonly GamefieldBorder Border;

        public readonly Layer3D<IDrawable3D> Layer3Ds;
        public readonly Layer2D<IDrawable2D> Layer2Ds;

        public readonly Layer2D<IDrawable2D> CharacterLayer;
        public readonly Layer2D<IDrawable2D> ProjectileLayer;
        public readonly Layer2D<IDrawable2D> OverlayLayer;

        public readonly float MaxBarSize;
        public readonly Vector2 Size;

        public readonly Box HealthBar;
        public readonly Box HealthChange;
        public readonly Text2D MaxHealthText;
        public readonly Text2D CurrentHealthText;

        public readonly Box EnergyBar;
        public readonly Box EnergyChange;
        public readonly Text2D MaxEnergyText;
        public readonly Text2D CurrentEnergyText;

        public Shades Shade;
        public float Intensity;

        public PlayLayers()
        {
            Debugger.Assert(Game.DrawThreaded);

            Size = GamemodeStore.SelectedGamemode.Gamemode.GetGamefieldSize();
            MaxBarSize = Size.Y - 32;

            Border = new GamefieldBorder(Size);

            Layer3Ds = new Layer3D<IDrawable3D>
            {
                Clock = TrackManager.CurrentTrack.DrawClock,
            };
            Layer2Ds = new Layer2D<IDrawable2D>
            {
                Clock = TrackManager.CurrentTrack.DrawClock,
                Size = Size
            };

            OverlayLayer = new Layer2D<IDrawable2D>
            {
                Size = Size
            };
            CharacterLayer = new Layer2D<IDrawable2D>
            {
                Size = Size
            };
            ProjectileLayer = new Layer2D<IDrawable2D>
            {
                Size = Size
            };

            Layer2Ds.Add(CharacterLayer);
            Layer2Ds.Add(ProjectileLayer);
            Layer2Ds.Add(OverlayLayer);

            OverlayLayer.Children = new IDrawable2D[]
            {
                new Box
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomLeft,

                    Position = new Vector2(32, -16),
                    Size = new Vector2(8, MaxBarSize),

                    Color = Color.Black,
                    Alpha = 0.5f,
                },
                HealthChange = new Box
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomLeft,

                    Position = new Vector2(32, -16),
                    Size = new Vector2(8, MaxBarSize),

                    Color = Color.Red,
                },
                HealthBar = new Box
                {
                    ParentOrigin = Mounts.BottomRight,
                    Origin = Mounts.BottomLeft,

                    Position = new Vector2(32, -16),
                    Size = new Vector2(8, MaxBarSize),
                },
                MaxHealthText = new Text2D
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,

                    Position = new Vector2(32, -MaxBarSize / 2 - 32),
                    FontScale = 0.48f,
                },
                CurrentHealthText = new Text2D(false)
                {
                    ParentOrigin = Mounts.CenterRight,
                    Origin = Mounts.CenterLeft,

                    X = 64,
                    FontScale = 0.32f,
                },


                new Box
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomRight,

                    Position = new Vector2(-32, -16),
                    Size = new Vector2(8, MaxBarSize),

                    Color = Color.Black,
                    Alpha = 0.5f,
                },
                EnergyChange = new Box
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomRight,

                    Position = new Vector2(-32, -16),
                    Size = new Vector2(8, MaxBarSize),

                    Color = Color.Red,
                },
                EnergyBar = new Box
                {
                    ParentOrigin = Mounts.BottomLeft,
                    Origin = Mounts.BottomRight,

                    Position = new Vector2(-32, -16),
                    Size = new Vector2(8, MaxBarSize),
                },
                MaxEnergyText = new Text2D
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,

                    Position = new Vector2(-32, -MaxBarSize / 2 - 32),
                    FontScale = 0.48f,
                },
                CurrentEnergyText = new Text2D(false)
                {
                    ParentOrigin = Mounts.CenterLeft,
                    Origin = Mounts.CenterRight,

                    X = -64,
                    FontScale = 0.32f,
                },
            };
        }
    }
}
