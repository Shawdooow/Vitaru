// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Runtime;

namespace Vitaru.Roots
{
    public class PlayRoot : MenuRoot
    {
        public override string Name => nameof(PlayRoot);

        protected override bool UseLevelBackground => true;

        public override void LoadingComplete()
        {
            base.LoadingComplete();
            GCSettings.LatencyMode = GCLatencyMode.SustainedLowLatency;
        }

        protected override void Dispose(bool finalize)
        {
            base.Dispose(finalize);
            GCSettings.LatencyMode = GCLatencyMode.Interactive;
        }
    }
}