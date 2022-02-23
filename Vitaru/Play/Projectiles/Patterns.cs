// Copyright (c) 2018-2022 Shawn Bozek.
// Licensed under EULA https://docs.google.com/document/d/1xPyZLRqjLYcKMxXLHLmA5TxHV-xww7mHYVUuWLt2q9g/edit?usp=sharing

using System;
using System.Collections.Generic;
using System.Numerics;
using Prion.Nucleus.Utilities;
using Vitaru.Editor.KeyFrames;

namespace Vitaru.Play.Projectiles
{
    public static class Patterns
    {
        private const float max_dist = 800;

        public static List<Projectile> Wave(float speed, float diameter, float damage, Vector2 position,
            double startTime, int team, float complexity = 1, float angle = (float)Math.PI / 2)
        {
            List<Projectile> projectiles = new();

            int bulletCount = (int)(complexity * 5);
            float directionModifier = (float)Math.PI / 2 / (bulletCount - 1);
            float direction = angle - (float)Math.PI / 4;

            for (int i = 1; i <= bulletCount; i++)
            {
                projectiles.Add(new Bullet
                {
                    StartTime = startTime,
                    StartPosition = position,
                    Speed = speed,
                    Angle = direction,
                    Distance = max_dist,
                    CurveType = CurveType.Straight,
                    SpeedEasing = Easings.OutSine,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = i % 2 == 1 ? diameter : diameter * 1.5f,
                    },
                    Damage = i % 2 == 1 ? damage : damage * 0.8f,
                    Team = team,
                });
                direction += directionModifier;
            }

            return projectiles;
        }

        public static List<Projectile> Line(float startSpeed, float endSpeed, float diameter, float damage,
            Vector2 position, double startTime, int team, float complexity = 1, float angle = (float)Math.PI / 2)
        {
            List<Projectile> projectiles = new();

            int bulletCount = (int)(complexity * 3);
            float speedModifier = (endSpeed - startSpeed) / bulletCount;
            float speed = startSpeed;

            for (int i = 1; i <= bulletCount; i++)
            {
                projectiles.Add(new Bullet
                {
                    StartTime = startTime,
                    StartPosition = position,
                    Speed = speed,
                    Angle = angle,
                    Distance = max_dist,
                    CurveType = CurveType.Straight,
                    SpeedEasing = Easings.OutQuad,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = diameter,
                    },
                    Damage = damage,
                    Team = team,
                });
                speed += speedModifier;
            }

            return projectiles;
        }

        public static List<Projectile> Triangle(float speed, float diameter, float damage, Vector2 position,
            double startTime, int team, float complexity = 1, float angle = (float)Math.PI / 2)
        {
            List<Projectile> projectiles = new();

            int bulletCount = (int)(complexity * 3);

            if (bulletCount % 3 != 0)
                bulletCount++;
            if (bulletCount % 3 != 0)
                bulletCount++;

            float directionModifier = (float)Math.PI / 4 / (bulletCount - 1);
            float direction = angle;

            for (int i = 1; i <= bulletCount; i++)
            {
                projectiles.Add(new Bullet
                {
                    StartTime = startTime,
                    StartPosition = position,
                    Speed = speed,
                    Angle = direction,
                    Distance = max_dist,
                    CurveType = CurveType.Straight,
                    SpeedEasing = Easings.OutQuad,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = diameter,
                    },
                    Damage = damage,
                    Team = team,
                });
                direction += directionModifier;

                if (i == 1)
                {
                    speed *= 0.9f;
                    direction -= directionModifier + directionModifier / 2;
                }

                if (i == 3)
                {
                    speed *= 0.92f;
                    direction -= directionModifier * 3 + directionModifier / 2;
                }
            }

            return projectiles;
        }

        public static List<Projectile> Wedge(float speed, float diameter, float damage, Vector2 position,
            double startTime, int team, float complexity = 1, float angle = (float)Math.PI / 2)
        {
            List<Projectile> projectiles = new();

            int bulletCount = (int)(complexity * 7);

            if (bulletCount % 2 == 0)
                bulletCount++;

            float directionModifier = (float)Math.PI / 2 / (bulletCount - 1);
            float direction = angle - (float)Math.PI / 4;

            float speedModifier = (speed * 1.5f - speed * 0.75f) / bulletCount;

            for (int i = 1; i <= bulletCount; i++)
            {
                projectiles.Add(new Bullet
                {
                    StartTime = startTime,
                    StartPosition = position,
                    Speed = speed,
                    Angle = i % 2 == 0 ? angle - direction : angle + direction,
                    Distance = max_dist,
                    CurveType = CurveType.Straight,
                    SpeedEasing = Easings.OutSine,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = diameter,
                    },
                    Damage = damage,
                    Team = team,
                });

                if (i % 2 == 0)
                {
                    speed -= speedModifier;
                    direction -= directionModifier;
                }
            }

            return projectiles;
        }

        public static List<Projectile> Circle(float speed, float diameter, float damage, Vector2 position,
            double startTime, int team, float complexity = 1)
        {
            List<Projectile> projectiles = new();

            int bulletCount = (int)(complexity * 12);
            float directionModifier = (float)Math.PI * 2 / bulletCount;
            float direction = (float)Math.PI / 2;

            for (int i = 1; i <= bulletCount; i++)
            {
                projectiles.Add(new Bullet
                {
                    StartTime = startTime,
                    StartPosition = position,
                    Speed = speed,
                    Angle = direction,
                    Distance = max_dist,
                    CurveType = CurveType.Straight,
                    SpeedEasing = Easings.OutCubic,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = i % 2 == 1 ? diameter : diameter * 1.5f,
                    },
                    Damage = i % 2 == 1 ? damage : damage * 0.8f,
                    Team = team,
                });
                direction += directionModifier;
            }

            return projectiles;
        }

        public static List<Projectile> Swipe(float speed, float diameter, float damage, double startTime, int team,
            float complexity = 1)
        {
            List<Projectile> projectiles = new();

            const float dist = 800;
            int bulletCount = (int)(complexity * 4);
            float directionModifier = (float)Math.PI / bulletCount;
            float direction = (float)Math.PI / 8f;

            for (int i = 1; i <= bulletCount; i++)
            {
                Vector2 offset = new((float)Math.Cos(direction) * (-dist / 2),
                    (float)Math.Sin(direction) * (-dist / 2));

                projectiles.Add(new Bullet
                {
                    ObeyBoundries = false,
                    TargetPlayer = true,
                    Shape = Shape.Triangle,
                    StartTime = startTime,
                    StartPosition = offset,
                    Speed = speed,
                    Angle = direction,
                    Distance = dist,
                    CurveType = CurveType.Target,
                    SpeedEasing = Easings.InOutQuint,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = diameter,
                    },
                    Damage = damage,
                    Team = team,
                });
                direction += directionModifier;
            }

            return projectiles;
        }

        public static List<Projectile> Cross(float speed, float diameter, float damage, double startTime, int team,
            float complexity = 1)
        {
            List<Projectile> projectiles = new();

            const float dist = max_dist;
            int bulletCount = (int)(complexity * 4);
            float directionModifier = (float)Math.PI * 2 / bulletCount;
            float direction = (float)Math.PI / 4;

            for (int i = 1; i <= bulletCount; i++)
            {
                Vector2 offset = new((float)Math.Cos(direction) * (-dist / 2),
                    (float)Math.Sin(direction) * (-dist / 2));

                projectiles.Add(new Bullet
                {
                    ObeyBoundries = false,
                    TargetPlayer = true,
                    Shape = Shape.Triangle,
                    StartTime = startTime,
                    StartPosition = offset,
                    Speed = speed,
                    Angle = direction,
                    Distance = dist,
                    CurveType = CurveType.Target,
                    SpeedEasing = Easings.InOutQuint,
                    CircularHitbox = new CircularHitbox
                    {
                        Diameter = diameter,
                    },
                    Damage = damage,
                    Team = team,
                });
                direction += directionModifier;
            }

            return projectiles;
        }

        public static List<Projectile> Spiral(float speed, float diameter, float damage, Vector2 position,
            double startTime, double duration, int team, double beatLength = 500, float complexity = 1, int arms = 8)
        {
            List<Projectile> projectiles = new();

            float direction = 0;

            for (double j = startTime; j <= startTime + duration; j += beatLength / 16)
            {
                for (int i = 1; i <= arms; i++)
                {
                    projectiles.Add(new Bullet
                    {
                        StartTime = j,
                        StartPosition = position,
                        Speed = speed,
                        Angle = direction,
                        CircularHitbox = new CircularHitbox
                        {
                            Diameter = diameter,
                        },
                        Damage = damage,
                        Distance = max_dist,
                        SpeedEasing = Easings.OutQuad,
                        CurveType = CurveType.Bezier,
                        CurveAmount = -240,
                        Team = team,
                    });

                    direction += MathF.PI / (arms / 2f);
                }

                direction += MathF.PI / 0.94f;
            }

            return projectiles;
        }
    }
}

/*
── █───▄▀█▀▀█▀▄▄───▐█──────▄▀█▀▀█▀▄▄
──█───▀─▐▌──▐▌─▀▀──▐█─────▀─▐▌──▐▌─█▀
─▐▌──────▀▄▄▀──────▐█▄▄──────▀▄▄▀──▐▌
─█────────────────────▀█────────────█
▐█─────────────────────█▌───────────█
▐█─────────────────────█▌───────────█
─█───────────────█▄───▄█────────────█
─▐▌───────────────▀███▀────────────▐▌
──█──────────▀▄───────────▄▀───────█
───█───────────▀▄▄▄▄▄▄▄▄▄▀────────█

             .  .
             |\_|\
             | a_a\
             | | "]
         ____| '-\___
        /.----.___.-'\
       //        _    \
      //   .-. (~v~) /|
     |'|  /\:  .--  / \
    // |-/  \_/____/\/~|
   |/  \ |  []_|_|_] \ |
   | \  | \ |___   _\ ]_}
   | |  '-' /   '.'  |
   | |     /    /|:  |
   | |     |   / |:  /\
   | |     /  /  |  /  \
   | |    |  /  /  |    \
   \ |    |/\/  |/|/\    \
    \|\ |\|  |  | / /\/\__\
     \ \| | /   | |__
         / |   |____)
          |_/

    ,------.
    `-____-'        ,-----------.
     ,i--i.         |           |
    / @  @ \       /  Woo Hoo!  |
   | -.__.- | ___-'             J
    \.    ,/ """"""""""""""""""'
    ,\""""/.
  ,'  `--'  `.
 (_,i'    `i._)
    |   o  |
    |  ,.  |
    | |  | |
    `-'  `-'

           .--'''''''''--.
        .'      .---.      '.
       /    .-----------.    \
      /        .-----.        \
      |       .-.   .-.       |
      |      /   \ /   \      |
       \    | .-. | .-. |    /
        '-._| | | | | | |_.-'
            | '-' | '-' |
             \___/ \___/
          _.-'  /   \  `-._
        .' _.--|     |--._ '.
        ' _...-|     |-..._ '
               |     |
               '.___.'
                 | |
                _| |_
               /\( )/\
              /  ` '  \
             | |     | |
             '-'     '-'
             | |     | |
             | |     | |
             | |-----| |
          .`/  |     | |/`.
          |    |     |    |
          '._.'| .-. |'._.'
                \ | /
                | | |
                | | |
                | | |
               /| | |\
             .'_| | |_`.
             `. | | | .'
          .    /  |  \    .
         /o`.-'  / \  `-.`o\
        /o  o\ .'   `. /o  o\
        `.___.'       `.___.'

                   __   __
                 .'  '.'  `.
              _.-|  o | o  |-._
            .~   `.__.'.__.'^  ~.
          .~     ^  /   \  ^     ~.
          \-._^   ^|     |    ^_.-/
          `\  `-._  \___/ ^_.-' /'
            `\_   `--...--'   /'
               `-.._______..-'      /\  /\
                  __/   \__         | |/ /_
                .'^   ^    `.      .'   `__\
              .'    ^     ^  `.__.'^ .\ \
             .' ^ .    ^   .    ^  .'  \/
            /    /        ^ \'.__.'
           |  ^ /|   ^      |
            \   \|^      ^  |  
             `\^ |        ^ |
               `~|    ^     |
                 |  ^     ^ |
                 \^         /
                  `.    ^ .'
             jgs   : ^    ; 
           .-~~~~~~   |  ^ ~~~~~~-.
          /   ^     ^ |    ^       \
          \^     ^   / \  ^     ^  /
           `~~~~~~~~'   `~~~~~~~~~'
*/