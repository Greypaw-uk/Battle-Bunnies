using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

using static BattleBunnies.Engine;
using static BattleBunnies.Players;
using static BattleBunnies.Graphics;
using static BattleBunnies.Global;

using static BattleBunnies.Weapons.RocketLauncher;
using static BattleBunnies.Weapons.Grenade;


namespace BattleBunnies
{
    public static class CollisionDetection
    {
        //      Takes two images and takes the X,Y location of each pixel in both images
        //      Checks whether pixels from image 1 intercept those of image 2
        //      If no collision, returns -1,-1
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

        public static Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Width];
                }
            }
            return colors2D;
        }

        public static Vector2 CheckTerrainCollision()
        {
            Matrix projectileMat = Matrix.CreateTranslation(-42, -240, 0)
                * Matrix.CreateRotationZ(projectileAngle)
                * Matrix.CreateScale(projectileScaling)
                * Matrix.CreateTranslation(projectilePosition.X, projectilePosition.Y, 0);

            Matrix terrainMat = Matrix.Identity;

            Vector2 terrainCollisionPoint = new Vector2();

            if (equippedWeapon.Equals(EquippedWeapon.RocketLauncher))
            {
                terrainCollisionPoint =
                    TexturesCollide(rocketColourArray, projectileMat, foregroundColourArray, terrainMat);
                //return terrainCollisionPoint;
            }
            else if (equippedWeapon.Equals(EquippedWeapon.Grenade))
            {
                terrainCollisionPoint =
                    TexturesCollide(grenadeColourArray, projectileMat, foregroundColourArray, terrainMat);
                //return terrainCollisionPoint;
            }
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
                        var terrain = new Vector2(1, Math.Abs(terrainContour[(int)terrainCollisionPoint.Y + 1]
                            - terrainContour[(int)terrainCollisionPoint.Y - 1]));
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
