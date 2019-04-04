using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using static BattleBunnies.Global;
using static BattleBunnies.Graphics;
using static BattleBunnies.Players;

namespace BattleBunnies

{
    public enum EquippedWeapon
    {
        NoWeapon,
        RocketLauncher,
        Grenade
    }
    
    static class Engine
    {
        public static EquippedWeapon equippedWeapon;

        //  Weapon Variables
        public static bool rocketFlying = false;

        public static bool grenadeThrown = false;

        public static Vector2 projectilePosition;
        public static Vector2 projectileDirection;
        public static float projectileAngle;
        public static float projectileScaling = 0.1f;

        //  THROTTLE SHOTS
        public static bool canShoot;


        //      #################################################
        //      #                                               #
        //      #               GAME LOGIC                      #
        //      #                                               #
        //      #################################################


        public static void FireWeapon()
        {
            if (canShoot)
            {
                canShoot = false;
                switch (equippedWeapon)
                {
                    case EquippedWeapon.NoWeapon:

                        break;

                    case EquippedWeapon.RocketLauncher:
                        {
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
                        break;

                    case EquippedWeapon.Grenade:
                        {
                            grenadeThrown = true;

                            projectilePosition = players[currentPlayer].Position;
                            projectilePosition.X += 20;
                            projectilePosition.Y -= 10;
                            projectileAngle = players[currentPlayer].Angle;
                            Vector2 grenadeGrav = new Vector2(0, -1);
                            Matrix grenadeSpin = Matrix.CreateRotationZ(projectileAngle);
                            projectileDirection = Vector2.Transform(grenadeGrav, grenadeSpin);
                            projectileDirection *= players[currentPlayer].Power / 50.0f;
                        }
                        break;
                }
            }
        }

        public static void UpdateRocket()
        {
            if (rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                projectileDirection += gravity / 10.0f;
                projectilePosition += projectileDirection;
                projectileAngle = (float)Math.Atan2(projectileDirection.X, -projectileDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = projectilePosition;
                    smokePos.X += randomiser.Next(10) - 5;
                    smokePos.Y += randomiser.Next(10) - 5;
                    smokeList.Add(smokePos);
                }
            }
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

        public static bool CheckOutOfScreen()
        {
            bool projectileOutOfScreen = projectilePosition.Y > screenHeight;
            projectileOutOfScreen |= projectilePosition.X < 0;
            projectileOutOfScreen |= projectilePosition.X > screenWidth;

            return projectileOutOfScreen;
        }

        public static void NextPlayer()
        {
            currentPlayer = currentPlayer + 1;
            currentPlayer = currentPlayer % numberOfPlayers;
            while (!players[currentPlayer].IsAlive)
            {
                currentPlayer = ++currentPlayer % numberOfPlayers;
            }
            players[currentPlayer].weaponFuse = 5.0f;
            players[currentPlayer].Angle = 0;
            players[currentPlayer].Power = 0;
            equippedWeapon = EquippedWeapon.NoWeapon;
            canShoot = false;
        }


        //      #################################################
        //      #                                               #
        //      #               COLLSION DETECTION              #
        //      #                                               #
        //      #################################################


        public static Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        public static Vector2 CheckTerrainCollision()
        {
            Matrix projectileMat = Matrix.CreateTranslation(-42, -240, 0)
                * Matrix.CreateRotationZ(projectileAngle)
                * Matrix.CreateScale(projectileScaling)
                * Matrix.CreateTranslation(projectilePosition.X, projectilePosition.Y, 0);

            Matrix terrainMat = Matrix.Identity;
            Vector2 terrainCollisionPoint =
                TexturesCollide(rocketColourArray, projectileMat, foregroundColourArray, terrainMat);
            return terrainCollisionPoint;
        }

        public static Vector2 CheckPlayersCollision()
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0)
                * Matrix.CreateRotationZ(projectileAngle)
                * Matrix.CreateScale(projectileScaling)
                * Matrix.CreateTranslation(projectilePosition.X, projectilePosition.Y, 0);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                PlayerData player = players[i];
                if (player.IsAlive)
                {
                    if (i != currentPlayer)
                    {
                        int xPos = (int)player.Position.X;
                        int yPos = (int)player.Position.Y;

                        Matrix launcherMat = Matrix.CreateTranslation(0, -launcherTexture.Height, 0)
                            * Matrix.CreateScale(playerScaling)
                            * Matrix.CreateTranslation(xPos, yPos, 0);

                        Vector2 launcherCollisionPoint = TexturesCollide(launcherColourArray, launcherMat, rocketColourArray, rocketMat);

                        if (launcherCollisionPoint.X > -1)
                        {
                            players[i].IsAlive = false;
                            return launcherCollisionPoint;
                        }

                        Matrix bunnyMat = Matrix.CreateTranslation(-11, -50, 0)
                            * Matrix.CreateRotationZ(player.Angle)
                            * Matrix.CreateScale(playerScaling)
                            * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);

                        Vector2 bunnyCollisionPoint = TexturesCollide(bunnyColourArray, bunnyMat, rocketColourArray, rocketMat);
                        if (bunnyCollisionPoint.X > -1)
                        {
                            players[i].IsAlive = false;
                            return bunnyCollisionPoint;
                        }
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        public static void CheckCollisions(GameTime gameTime)
        {
            Vector2 terrainCollisionPoint = CheckTerrainCollision();
            Vector2 playerCollisionPoint = CheckPlayersCollision();
            bool projectileOutOfScreen = CheckOutOfScreen();

            // Check Projectile Collision with Player

            if (playerCollisionPoint.X > -1)
            {
                if (rocketFlying)
                {
                    rocketFlying = false;

                    smokeList = new List<Vector2>();
                    AddExplosion(playerCollisionPoint, 10, 80.0f, 2000.0f, gameTime);

                    hitTerrain.Play();
                    hitbunny.Play();
                    NextPlayer();
                }
                if (grenadeThrown)
                {
                    // TODO ensure grenade does not kill player on contact unless exploding
                }
            }


            // Check Projectile Collision with Terrain
            if (terrainCollisionPoint.X > -1)
            {
                if (rocketFlying)
                {
                    rocketFlying = false;

                    smokeList = new List<Vector2>();
                    AddExplosion(terrainCollisionPoint, 4, 30.0f, 1000.0f, gameTime);

                    hitTerrain.Play();
                    NextPlayer();
                }
                else if (grenadeThrown)
                {
                    //  Bounce that mofo 
                    if (timer <= 0)
                    {
                        var terrain = new Vector2(terrainContour[(int)terrainCollisionPoint.Y] - 1,
                            terrainContour[(int)terrainCollisionPoint.Y] + 1);
                        terrain.Normalize();
                        var reflection = Vector2.Reflect(projectileDirection, terrain);
                        projectileDirection = -reflection;
                        timer = 0.5f;
                    }
                }
            }


            // 'Removes' projectiles when they touch the bottom of the screen, or they go too far left/right
            if (projectileOutOfScreen)
            {
                rocketFlying = false;
                grenadeThrown = false;

                smokeList = new List<Vector2>();
                NextPlayer();
            }
        }
    }
}
