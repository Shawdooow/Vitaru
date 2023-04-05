// Copyright (c) 2018-2023 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using Prion.Mitochondria.Graphics.Drawables;
using Prion.Mitochondria.Graphics.Layers._2D;
using Prion.Nucleus;
using Prion.Nucleus.Utilities;
using Vitaru.Wiki.Content;

namespace Vitaru.Wiki.Included
{
    public class VitaruWiki : WikiPanel
    {
        public override string Name => "Vitaru";

        public override WikiSection[] GetSections() => new WikiSection[]
        {
            new About(),
            new Modes(),
            //new Credits(),
        };

        private class About : WikiSection
        {
            public override string Name => nameof(About);

            public override InputLayer<IDrawable2D> GetSection() => new WikiListLayer
            {
                Children = new IDrawable2D[]
                {
                    new Header("What is Vitaru?"),
                    new Description("Vitaru is a rhythm-based bullet hell."),
                    new Header("So what do I do?"),
                    new Description("Avoid the bullets flying at you. Although this is usually easier said than done."),
                },
            };
        }

        private class Modes : WikiSection
        {
            public override string Name => nameof(Modes);

            public override InputLayer<IDrawable2D> GetSection() => new WikiListLayer
            {
                Children = new IDrawable2D[]
                {
                    new Description(
                        "In order to change the feature sets Vitaru (and Prion) use you must pass in the launch argument [Features=Standard] without the []s. " +
                        "Swap the word \"Standard\" for any of the below options to enable other feature sets."),
                    new Header(Features.Safe.ToString()),
                    new Description(Features.Safe.GetDescription()),
                    new Header(Features.Standard.ToString()),
                    new Description(Features.Standard.GetDescription()),
                    new Header(Features.Upcoming.ToString()),
                    new Description(Features.Upcoming.GetDescription()),
                    new Header(Features.Experimental.ToString()),
                    new Description(Features.Experimental.GetDescription()),
                    new Header(Features.Radioactive.ToString()),
                    new Description(Features.Radioactive.GetDescription()),
                },
            };
        }

        private class Credits : WikiSection
        {
            public override string Name => nameof(Credits);

            public override InputLayer<IDrawable2D> GetSection() => new WikiListLayer
            {
                Children = new IDrawable2D[]
                {
                    new Description(""),
                },
            };
        }
    }
}