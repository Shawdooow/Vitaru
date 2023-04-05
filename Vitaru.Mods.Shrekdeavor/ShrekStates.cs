// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing


using Prion.Nucleus.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Vitaru.Mods.Shrekdeavor
{
    public struct ShrekState
    {
        public ulong InputState { get; init; }
        public ulong OutputState { get; init; }
        public string InputText { get; init; }
        public string OutputText { get; init; }


        internal static ulong State;


        public static List<ShrekState> GetStates()
        {
            List<ShrekState> list = new List<ShrekState>();

            foreach (StartStates s in Enum.GetValues(typeof(StartStates)))
                list.Add(GetState(s));

            foreach (LeftStates s in Enum.GetValues(typeof(LeftStates)))
                list.Add(GetState(s));

            foreach (DownStates s in Enum.GetValues(typeof(DownStates)))
                list.Add(GetState(s));

            foreach (DownBackStates s in Enum.GetValues(typeof(DownBackStates)))
                list.Add(GetState(s));

            foreach (DownBackGoldenTicketStates s in Enum.GetValues(typeof(DownBackGoldenTicketStates)))
                list.Add(GetState(s));

            return list;
        }

        public static ShrekState GetState<E>(E e)
            where E : Enum
        {
            InputTextAttribute[] t = e.GetAttributes<E, InputTextAttribute>();

            return new ShrekState
            {
                InputState = e.GetAttributes<E, InputStateAttribute>()[0].InputState,
                OutputState = Convert.ToUInt64(e),
                InputText = t.Length > 0 ? t[0].InputText : e.ToString(),
                OutputText = e.GetDescription(),
            };
        }
    }

    public class InputStateAttribute : Attribute
    {
        public InputStateAttribute(ulong state)
        {
            InputStateValue = state;
        }

        public virtual ulong InputState => InputStateValue;

        protected ulong InputStateValue { get; set; }
    }

    public class InputTextAttribute : Attribute
    {
        public InputTextAttribute(string input)
        {
            InputTextValue = input;
        }

        public virtual string InputText => InputTextValue;

        protected string InputTextValue { get; set; }
    }

    public enum StartStates
    {
        [Description("Welcome to the Great Shrekdeavor REDICED Edition!\n\n" +
                "You are walking down a path, which direction do you travel? (left, right)"), 
            InputState(0)]
        Shrekdeavor = 1,



        [Description("You walk down the left path. " +
                "After walking for some time you come upon a large white castle making quite a ruckus. " +
                "Inside you find lord fuckwad is hosting a WWE tournament with many contestants. " +
                "You recognize Shrek, an old friend and approach him. " +
                "Shrek greets you and informs you he is next up and must choose an opponent. " +
                "He asks for your advice.\n\n" +
                "(Master Chief, Jack Black, Albert Wesker, Imposter, John Wick, " +
                "Lady Dimitrescu, Walter White, Thanos, Megamind, Minions, Grinch, " +
                "The Mask, Mr Incredible, Myers, Putin, Kim Jong Un, Xi Jinping, Elmo, Gru, Pinhead)"), 
            InputState((ulong)Shrekdeavor)]
        Left = 1000,

        [Description("You walk down the right path.\n\n" +
                ""), 
            InputState((ulong)Shrekdeavor)]
        Right = 2000,



        [Description("Not really caring to make a decision you forage straight forward into the forest.\n\n" +
                ""), 
            InputState((ulong)Shrekdeavor)]
        Forward = 3000,
        [Description("You begin traveling backwards but fucked up, you are traveling backwards in time.\n\n" +
                ""), 
            InputState((ulong)Shrekdeavor)]
        Back = 4000,
        [Description("Despite usually being bound to earth by gravity you manage to start floating straight up.\n\n" +
                ""), 
            InputState((ulong)Shrekdeavor)]
        Up = 5000,
        [Description("You begin burrowing into the ground. " +
                "A few miles down you turn around to find shrek is behind you and is helping you dig. " +
                "You continue until you hit the mantle. " +
                "Shrek advises you turn back, what shall you do? (back, cointinue)\n\n" +
                ""), 
            InputState((ulong)Shrekdeavor)]
        Down = 6000,



        [Description("You drop dead instantly. " +
            "You awake to the sweet, stinging scent of onions a few hours later and find shrek has revived you. (kill, dream)\n\n"), 
            InputState((ulong)Shrekdeavor)]
        Die = 7000,
    }

    public enum LeftStates
    {
        [Description("Shrek enters the arena with WWE legend Master Chief, he looks tough. " +
            "Shrek is a bit nervous about this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left), InputText("Master Chief")]
        MasterChief = StartStates.Left + 10,

        [Description("Shrek enters the arena with the king of hell himself, he looks tough. " +
            "Shrek is a bit nervous about this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left), InputText("Jack Black")]
        JackBlack = StartStates.Left + 20,

        [Description("Shrek enters the arena with some freak in a leather jacket, he looks like a joke. " +
            "Shrek is confident about this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left), InputText("Albert Wesker")]
        AlbertWesker = StartStates.Left + 30,

        [Description("Shrek enters the arena with a short space man wielding a chef's knife, he looks like a joke. " +
            "Shrek is confident about this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left)]
        Imposter = StartStates.Left + 40,

        [Description(""),
            InputState((ulong)StartStates.Left), InputText("John Wick")]
        JohnWick = StartStates.Left + 50,

        [Description(""),
            InputState((ulong)StartStates.Left), InputText("Lady Dimitrescu")]
        LadyDimitrescu = StartStates.Left + 60,

        [Description(""),
            InputState((ulong)StartStates.Left), InputText("Walter White")]
        WalterWhite = StartStates.Left + 70,

        [Description("Shrek enters the arena with taller and more in-shape purple variant of himself, he looks pissed. " +
            "Shrek is confused about this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left)]
        Thanos = StartStates.Left + 80,

        [Description("Shrek enters the arena with a blue dude who has terminal 7 brain cancer, he looks retarded. " +
            "Shrek is conflicted about this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left)]
        Megamind = StartStates.Left + 90,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Minions = StartStates.Left + 100,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Grinch = StartStates.Left + 110,

        [Description("Shrek enters the arena with Jim Carrey's The Mask, he looks menacing. " +
            "Shrek is afraid of this one. " +
            "What strategy should he use?\n\n" +
            "(Aggressive, Defensive, Retarded)"),
            InputState((ulong)StartStates.Left)]
        Mask = StartStates.Left + 120,

        [Description(""),
            InputState((ulong)StartStates.Left), InputText("Mr. Incredible")]
        MrIncredible = StartStates.Left + 130,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Myers = StartStates.Left + 140,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Putin = StartStates.Left + 150,

        [Description(""),
            InputState((ulong)StartStates.Left), InputText("Kim Jong Un")]
        KimJongUn = StartStates.Left + 160,

        [Description(""),
            InputState((ulong)StartStates.Left), InputText("Xi Jinping")]
        XiJinping = StartStates.Left + 170,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Elmo = StartStates.Left + 180,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Gru = StartStates.Left + 190,

        [Description(""),
            InputState((ulong)StartStates.Left)]
        Pinhead = StartStates.Left + 200,
    }

    public enum DownStates
    {
        [Description("You turn back and go to the surface, " +
            "only to find that you are no longer on earth but on a giant onion in space with many layers. " +
            "Lord fuckwad sent space pirates to space assasinate you, how do you retaliate? (golden ticket, golden onion, railgun, microwave gun)\n\n" +
            ""),
            InputState((ulong)StartStates.Down)]
        Back = StartStates.Down + 100,
        [Description("You tell shrek you must continue, he understands and follows you down. " +
            "You continue through the mantle until you reach an opening. " +
            "You have found Hell. " +
            "Where do you head next? (lava, center)\n\n" +
            ""),
            InputState((ulong)StartStates.Down)]
        Continue = StartStates.Down + 200,
    }

    public enum DownBackStates
    {
        [Description("You pull a golden ticket out of seemingly nowhere and flail it around. " +
            "The polar express appears from behind a asteroid and runs over all the space pirates. It then stops to pick you up, then heads toward the north pole.\n\n" +
            "" +
            "Once you reach the north pole you realize that all the elfs have been enslaved by Adolf Hitler, " +
            "and the nazis are going to board the train and search for more elfs. " +
            "You and shrek hop off just outside of town and meet up with the rebels trying to kill Hitler, who are being lead by Santa. " +
            "His weapons storehouse is unlike anything you've ever seen; tanks, planes, nukes. " +
            "You name it he had it. " +
            "His spys inform you all that now is a good time to strike, however Santa wants to perform some ritual before you go. " +
            "Whats the plan? (ritual, nuke, sneaky, siege)\n\n" +
            ""),
            InputState((ulong)DownStates.Back), InputText("Golden Ticket")]
        GoldenTicket = DownStates.Back + 10,
    }

    public enum DownBackGoldenTicketStates
    {
        [Description("You perform the ritual by drawing a pentagram on the ground with santa and lighting five candles." +
            "Jack Black himself ascends from the depths of hell while absolutely shreading a guitar solo.\n\n" +
            ""),
            InputState((ulong)DownBackStates.GoldenTicket)]
        Ritual = DownBackStates.GoldenTicket + 1,
    }
}
