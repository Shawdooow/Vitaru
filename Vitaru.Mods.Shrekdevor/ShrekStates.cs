// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing


namespace Vitaru.Mods.Shrekdevor
{
    public struct ShrekState
    {
        public ulong InputState { get; init; }
        public ulong OutputState { get; init; }
        public string InputText { get; init; }
        public string OutputText { get; init; }


        internal static ulong State;


        private const ulong start = 1;

        private const ulong left = 1000;
        private const ulong right = 2000;

        private const ulong forward = 3000;
        private const ulong back = 4000;
        private const ulong up = 5000;
        private const ulong down = 6000;

        private const ulong die = 7000;


        public static readonly ShrekState[] States = new[]
        {
            new ShrekState
            {
                InputState = 0,
                OutputState = start,
                InputText = "Shrekdevor",
                OutputText = "Welcome to the Great Shrekdevor REDICED Edition!\n\n" +
                "You are walking down a path, which direction do you travel? (left, right)",
            },




            new ShrekState
            {
                InputState = start,
                OutputState = left,
                InputText = nameof(left),
                OutputText = "You walk down the left path.\n\n" +
                "",
            },


            new ShrekState
            {
                InputState = start,
                OutputState = right,
                InputText = nameof(right),
                OutputText = "You travel down the right path.\n\n" +
                "",
            },


            new ShrekState
            {
                InputState = start,
                OutputState = forward,
                InputText = nameof(forward),
                OutputText = "Not really caring to make a decision you forage straight forward into the forest.\n\n" +
                "",
            },


            new ShrekState
            {
                InputState = start,
                OutputState = back,
                InputText = nameof(back),
                OutputText = "You begin traveling backwards but fucked up, you are traveling backwards in time.\n\n" +
                "",
            },


            new ShrekState
            {
                InputState = start,
                OutputState = up,
                InputText = nameof(up),
                OutputText = "Despite usually being bound to earth by gravity you manage to start floating straight up.\n\n" +
                "",
            },


            new ShrekState
            {
                InputState = start,
                OutputState = down,
                InputText = nameof(down),
                OutputText = "You begin burrowing into the ground. " +
                            "A few miles down you turn around to find shrek is behind you and is helping you dig. " +
                            "You continue until you hit the mantle. " +
                            "Shrek advises you turn back, what shall you do? (back, cointinue)\n\n" +
                            "",
            },
            new ShrekState
            {
                InputState = down,
                OutputState = down + 100,
                InputText = "back",
                OutputText = "You turn back and go to the surface, " +
                            "only to find that you are no longer on earth but on a giant onion in space with many layers. " +
                            "Lord fuckwad sent space pirates to space assasinate you, how do you retaliate? (golden ticket, golden onion, railgun, microwave gun)\n\n" +
                            "",
            },
            new ShrekState
            {
                InputState = down + 100,
                OutputState = down + 110,
                InputText = "golden ticket",
                OutputText = "You pull a golden ticket out of seemingly nowhere and flail it around. " +
                            "The polar express appears from behind a asteroid and runs over all the space pirates. It then stops to pick you up, then heads toward the north pole.\n\n" +
                            "" +
                            "Once you reach the north pole you realize that all the elfs have been enslaved by Adolf Hitler, " +
                            "and the nazis are going to board the train and search for more elfs. " +
                            "You and shrek hop off just outside of town and meet up with the rebels trying to kill Hitler, who are being lead by Santa. " +
                            "His weapons storehouse is unlike anything you've ever seen; tanks, planes, nukes. " +
                            "You name it he had it. " +
                            "His spys inform you all that now is a good time to strike, however Santa wants to perform some ritual before you go. " +
                            "Whats the plan? (ritual, nuke, sneaky, siege)\n\n" +
                            "",
            },
            new ShrekState
            {
                InputState = down + 110,
                OutputState = down + 111,
                InputText = "ritual",
                OutputText = "You perform the ritual by drawing a pentagram on the ground with santa and lighting five candles." +
                            "Jack Black himself ascends from the depths of hell while absolutely shreading a guitar solo.\n\n" +
                            "",
            },

            new ShrekState
            {
                InputState = down,
                OutputState = down + 200,
                InputText = "continue",
                OutputText = "You tell shrek you must continue, he understands and follows you down. " +
                "You continue through the mantle until you reach an opening. " +
                "You have found Hell. " +
                "Where do you head next? (lava, center)\n\n" +
                "",
            },
        };
    }
}
