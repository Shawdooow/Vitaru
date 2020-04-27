// Copyright (c) 2018-2020 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

#region usings

using System.Collections.Generic;

#endregion

namespace Vitaru.Gamemodes.Vitaru.HitObjects
{
    public class VitaruCluster : Cluster
    {
        public override double TimePreempt => Preemt;
        public override double TimeUnPreempt => 1200;

        protected double Preemt = 600;

        protected override List<Projectile> ProcessProjectiles(List<Projectile> projectiles)
        {
            foreach (Projectile p in projectiles)
                if (p.StartTime < StartTime - Preemt)
                    Preemt = StartTime - p.StartTime;

            return base.ProcessProjectiles(projectiles);
        }

        protected override List<Projectile> GetConvertCluster(Vector2 pos, int id)
        {
            if (!VitaruSettings.Patterns) return base.GetConvertCluster(pos, id);

            switch (id)
            {
                default:
                    return new List<Projectile>();
                case 1:
                    return Patterns.Swipe(ClusterSpeed * Velocity * 2, ClusterDiameter, ClusterDamage, StartTime, Team,
                        ClusterDensity);
                case 7:
                    return Patterns.Cross(ClusterSpeed * Velocity * 2, ClusterDiameter, ClusterDamage, StartTime, Team,
                        ClusterDensity);
            }
        }

        protected override int GetConvertPatternID(SampleInfo info)
        {
            if (!VitaruSettings.Patterns) return base.GetConvertPatternID(info);

            if (IsSpinner) return 6;

            switch (info.Bank)
            {
                default:
                    Logger.Log($"Bad SampleInfo: {info.Bank} - {info.Name}", LoggingTarget.Database, LogLevel.Error);
                    return 1;
                case "normal" when info.Name == "hitnormal":
                    return 1;
                case "normal" when info.Name == "hitwhistle":
                    return 2;
                case "normal" when info.Name == "hitfinish":
                    return 7;
                case "normal" when info.Name == "hitclap":
                    return 5;
                case "drum" when info.Name == "hitnormal":
                    return 1;
                case "drum" when info.Name == "hitwhistle":
                    return 2;
                case "drum" when info.Name == "hitfinish":
                    return 3;
                case "drum" when info.Name == "hitclap":
                    return 4;
                case "soft" when info.Name == "hitnormal":
                    return 1;
                case "soft" when info.Name == "hitwhistle":
                    return 2;
                case "soft" when info.Name == "hitfinish":
                    return 7;
                case "soft" when info.Name == "hitclap":
                    return 5;
            }
        }
    }
}