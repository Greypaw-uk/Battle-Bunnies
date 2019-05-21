using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static BattleBunnies.Players;
using static BattleBunnies.Graphics;
using static BattleBunnies.Engine;
using static BattleBunnies.Global;

namespace BattleBunnies.Weapons
{
    public static class RocketLauncher
    {
        public static bool rocketFlying;

        public static void FireRocket()
        {
            currentTexture = powTexture;

            rocketFlying = true;
            launch.Play();

            projectilePosition = players[currentPlayer].Position;
            projectilePosition.X += 20;
            projectilePosition.Y -= 10;
            projectileAngle = players[currentPlayer].Angle;
            Vector2 up = new Vector2(0, -1);
            Matrix rotMatrix = Matrix.CreateRotationZ(projectileAngle);
            projectileDirection = Vector2.Transform(up, rotMatrix);
            projectileDirection *= players[currentPlayer].Power / 50.0f;
        }


        public static void UpdateRocket()
        {
            if (rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                projectileDirection += gravity / 10.0f;
                projectilePosition += projectileDirection;
                projectileAngle = (float) Math.Atan2(projectileDirection.X, -projectileDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = projectilePosition;
                    smokePos.X += randomiser.Next(10) - 5;
                    smokePos.Y += randomiser.Next(10) - 5;
                    smokeList.Add(smokePos);
                }
            }
        }


        public static void DrawRocket()
        {
            if (RocketLauncher.rocketFlying)
            {
                spriteBatch.Draw(rocketTexture, projectilePosition, null, players[currentPlayer].Colour,
                    projectileAngle,
                    new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
            }
        }
    }
}
