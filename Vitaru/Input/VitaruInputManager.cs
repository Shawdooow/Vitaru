// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using OpenTK.Input;
using Prion.Game.Input.Handlers;

namespace Vitaru.Input
{
    public class VitaruInputManager : ConfigedInputHandler<VitaruActions>
    {
        public override string Filename => "vitaruinput";

        protected override void OnInitialiseDefaults()
        {
            InputSettings.SetValue(VitaruActions.Up, Key.W);
            InputSettings.SetValue(VitaruActions.Down, Key.S);
            InputSettings.SetValue(VitaruActions.Left, Key.A);
            InputSettings.SetValue(VitaruActions.Right, Key.D);
            InputSettings.SetValue(VitaruActions.Sneak, Key.ShiftLeft);
            InputSettings.SetValue(VitaruActions.Shoot, MouseButton.Left);
            InputSettings.SetValue(VitaruActions.Spell, MouseButton.Right);
            InputSettings.SetValue(VitaruActions.Increase, Key.Q);
            InputSettings.SetValue(VitaruActions.Decrease, Key.E);
            InputSettings.SetValue(VitaruActions.Pull, Key.F);
        }
    }

    public enum VitaruActions
    {
        Up,
        Down,
        Left,
        Right,
        Sneak,
        Shoot,
        Spell,

        Increase,
        Decrease,

        Pull,
    }
}