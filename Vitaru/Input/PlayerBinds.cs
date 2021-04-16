// Copyright (c) 2018-2021 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Input;
using Prion.Mitochondria.Input.Binds;

namespace Vitaru.Input
{
    public class PlayerBinds : BindManager<VitaruActions>
    {
        public override string Filename => "vitaruinput";

        protected override void OnInitialiseDefaults()
        {
            BindSettings.SetValue(VitaruActions.Up, Keys.W);
            BindSettings.SetValue(VitaruActions.Down, Keys.S);
            BindSettings.SetValue(VitaruActions.Left, Keys.A);
            BindSettings.SetValue(VitaruActions.Right, Keys.D);
            BindSettings.SetValue(VitaruActions.Sneak, Keys.LeftShift);
            BindSettings.SetValue(VitaruActions.Jump, Keys.Space);
            BindSettings.SetValue(VitaruActions.Shoot, MouseButtons.Left);
            BindSettings.SetValue(VitaruActions.Spell, MouseButtons.Right);
            BindSettings.SetValue(VitaruActions.ModifierOne, Keys.E);
            BindSettings.SetValue(VitaruActions.ModifierTwo, Keys.Q);
            BindSettings.SetValue(VitaruActions.Pull, Keys.F);
        }
    }

    public enum VitaruActions
    {
        Up,
        Down,
        Left,
        Right,
        Sneak,
        Jump,
        Shoot,
        Spell,

        ModifierOne,
        ModifierTwo,

        Pull
    }
}