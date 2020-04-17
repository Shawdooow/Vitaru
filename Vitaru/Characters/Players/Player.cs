// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Numerics;
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

        public BindInputHandler<VitaruActions> InputHandler { get; set; }

        private int shootBullet;

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
            while (shootBullet > 0)
            {
                Bullet bullet = new Bullet
                {
                    StartPosition = Drawable.Position,
                    StartTime = Clock.Current,
                    Distance = 400,
                    Speed = 100,
                };

                Gamefield.Add(bullet);

                shootBullet--;
            }

            if (Drawable == null) return;

            Drawable.Position = GetNewPlayerPosition(100f);

            //TODO: HitDetection
            //for (int i = 0; i < playfield.Children.Count; i++)
            //{
            //    
            //    //Difference
            //    Vector2 pos = 
            //
            //    //Oddly this seems to help but not always?!?!?!?!?!?!?!?!?!!?!?!?!?!?!!??!?!?!?!?!?!?!?!?!?!?!?!!?!?!?!?!?!?!?!?!!?!?!?!?!?!?!!?!?!?!?!?!?!!?!?
            //    pos += new Vector2(hitbox1.Width / 4 + hitbox2.Width / 4);
            //
            //    double distance = Math.Sqrt(Math.Pow(pos.X, 2) + Math.Pow(pos.Y, 2));
            //    double edgeDistance = distance - (hitbox1.Width / 2 + hitbox2.Width / 2);
            //
            //    if (edgeDistance <= 0)
            //        return true;
            //}
        }

        public bool Pressed(VitaruActions t)
        {
            switch (t)
            {
                default:
                    return true;

                case VitaruActions.Shoot:
                    shootBullet++;
                    return true;
            }
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