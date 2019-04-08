using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static BattleBunnies.Players;
using static BattleBunnies.Graphics;
using static BattleBunnies.Engine;
using static BattleBunnies.Global;

namespace BattleBunnies.Weapons
{
    class Grenade
    {
        public static bool grenadeThrown;

        public static void GrenadeOut()
        {
            projectilePosition = players[currentPlayer].Position;
            projectilePosition.X += 20;
            projectilePosition.Y -= 10;
            projectileAngle = players[currentPlayer].Angle;
            Vector2 grenadeGrav = new Vector2(0, -1);
            Matrix grenadeSpin = Matrix.CreateRotationZ(projectileAngle);
            projectileDirection = Vector2.Transform(grenadeGrav, grenadeSpin);
            projectileDirection *= players[currentPlayer].Power / 50.0f;
        }


        public static void UpdateGrenade(GameTime gameTime)
        {
            if (grenadeThrown)
            {
                if (players[currentPlayer].weaponFuse <= 0)
                {
                    smokeList = new List<Vector2>();

                    AddExplosion(projectilePosition, 10, 80.0f, 2000.0f, gameTime);
                    hitTerrain.Play();

                    grenadeThrown = false;

                    NextPlayer();
                }
                Vector2 gravity = new Vector2(0, 1);
                projectileDirection += gravity / 10.0f;
                projectilePosition += projectileDirection;
                projectileAngle = (float)Math.Atan2(projectileDirection.X, -projectileDirection.Y);
            }
        }


        public static void DrawGrenade()
        {
            if (grenadeThrown)
            {
                spriteBatch.Draw(grenadeIcon, projectilePosition, null, players[currentPlayer].Colour,
                    projectileAngle,
                    new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
            }
        }
    }
}
