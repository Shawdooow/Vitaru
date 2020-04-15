// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
using Prion.Game.Input.Handlers;
using Prion.Game.Input.Receivers;

namespace Vitaru.Characters.Players
{
    public class Player : Character, IHasInputKeys<VitaruActions>
    {
        public BindInputHandler<VitaruActions> InputHandler { get; set; }


        public Player(DrawablePlayer drawable) : base(drawable)
        {
            InputHandler = new VitaruInputManager();
        }

        public override void Update()
        {
            Drawable.Position = GetNewPlayerPosition(100f);
        }

        public bool Pressed(VitaruActions t)
        {
            return true;
        }

        public bool Released(VitaruActions t)
        {
            return true;
        }

        protected virtual Vector2 GetNewPlayerPosition(double playerSpeed)
        {
            Vector2 playerPosition = Drawable.Position;

            double yTranslationDistance = playerSpeed * (Clock.LastElapsedTime / 1000f);
            double xTranslationDistance = playerSpeed * (Clock.LastElapsedTime / 1000f);

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