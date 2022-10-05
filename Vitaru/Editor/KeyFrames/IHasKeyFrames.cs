// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System.Collections.Generic;

namespace Vitaru.Editor.KeyFrames
{
    public interface IHasKeyFrames
    {
        bool PreLoaded { get; set; }

        bool Started { get; set; }

        double StartTime { get; set; }

        double EndTime { get; set; }

        double Duration => EndTime - StartTime;

        virtual double TimePreLoad => 600;

        virtual double TimeUnLoad => TimePreLoad;

        List<KeyValuePair<int, List<KeyFrame>>> KeyFrames { get; set; }
    }
}