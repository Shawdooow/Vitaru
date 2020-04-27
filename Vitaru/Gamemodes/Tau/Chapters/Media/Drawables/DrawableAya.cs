// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using Vitaru.Gamemodes.Tau.Chapters.Abilities;

#endregion

namespace Vitaru.Gamemodes.Tau.Chapters.Media.Drawables
{
    public class DrawableAya : DrawableTouhosuPlayer
    {
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly Camera camera;

        public DrawableAya(VitaruPlayfield playfield)
            : base(playfield, new Aya())
        {
            VitaruPlayfield.Gamefield.Add(camera = new Camera());
        }

        protected override void SpellActivate(VitaruAction action)
        {
            base.SpellActivate(action);

            ScreenSnap snap = new ScreenSnap(camera.CameraBox);
            VitaruPlayfield.VitaruInputManager.Add(snap);

            foreach (Drawable draw in CurrentPlayfield)
            {
                DrawableBullet bullet = draw as DrawableBullet;
                if (bullet?.Hitbox != null && Hitbox.HitDetect(camera.Hitbox, bullet.Hitbox))
                {
                    bullet.HitObject.Damage = 0;
                    bullet.ForceJudgement = true;
                    bullet.Masking = true;
                    bullet.Alpha = 0;
                }
            }
        }

        protected override void Update()
        {
            base.Update();
            camera.Position = Cursor.Position;
        }
    }
}