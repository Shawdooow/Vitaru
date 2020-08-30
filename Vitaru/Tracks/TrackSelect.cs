using System.Drawing;
using System.Drawing.Text;
using System.Numerics;
using Prion.Mitochondria;
using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers;
using Prion.Mitochondria.Graphics.Sprites;
using Prion.Mitochondria.Graphics.Transforms;
using Prion.Mitochondria.Graphics.UI;
using Vitaru.Levels;
using Vitaru.Server.Track;

namespace Vitaru.Tracks
{
    public class TrackSelect : HoverableLayer<IDrawable2D>
    {
        public TrackSelect()
        {
            Alpha = 0.75f;

            ParentOrigin = Mounts.CenterLeft;
            Origin = Mounts.CenterLeft;

            X = 8;
            Size = new Vector2(160, 400);
        }

        public override void LoadingComplete()
        {
            base.LoadingComplete();

            ListLayer<Button> list;

            Children = new IDrawable2D[]
            {
                new Box
                {
                    Name = "Background",
                    Color = Color.Black,
                    Alpha = 0.8f,
                    ParentSizing = Axes.Both,
                },
                list = new ListLayer<Button>
                {
                    //ParentSizing = Axes.Both,
                    Size = new Vector2(160, 400)
                }, 
            };
            foreach (LevelPack p in LevelStore.LoadedLevels)
            {
                list.Add(new Button
                {
                    ParentOrigin = Mounts.TopCenter,
                    Origin = Mounts.TopCenter,

                    //ParentSizing = Axes.Horizontal,
                    Width = 160,
                    Height = 20,

                    Text = p.Title,

                    SpriteText =
                    {                   
                        ParentOrigin = Mounts.TopLeft,
                        Origin = Mounts.TopLeft,

                        TextScale = 0.25f
                    },

                    OnClick = () =>
                    {
                        if (!TrackManager.Switching)
                            Game.ScheduleLoad(() =>
                            {
                                LevelStore.SetLevel(p);
                                TrackManager.SetTrack(p.Levels[0].LevelTrack);
                            });
                    }

                });
            }
        }

        public override void OnHovered()
        {
            base.OnHovered();
            this.FadeTo(1f, 200f);
        }

        public override void OnHoverLost()
        {
            base.OnHoverLost();
            this.FadeTo(0.75f, 200f);
        }
    }
}
