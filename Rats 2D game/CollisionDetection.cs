using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Rats_2D_game
{
    class CollisionDetection
    {
        Variables v = new Variables();
        Graphics g = new Graphics();
        Methods m = new Methods();

        public void CheckCollisions(GameTime gameTime)
        {
            Vector2 terrainCollisionPoint = CheckTerrainCollision();
            Vector2 playerCollisionPoint = CheckPlayersCollision();
            bool rocketOutOfScreen = CheckOutOfScreen();

            if (playerCollisionPoint.X > -1)
            {
                v.RocketFlying = false;

                v.SmokeList = new List<Vector2>(); g.AddExplosion(playerCollisionPoint, 10, 80.0f, 2000.0f, gameTime);

                v.HitCannon.Play();
                m.NextPlayer();
            }

            if (terrainCollisionPoint.X > -1)
            {
                v.RocketFlying = false;

                v.SmokeList = new List<Vector2>(); g.AddExplosion(terrainCollisionPoint, 4, 30.0f, 1000.0f, gameTime);

                v.HitTerrain.Play();
                m.NextPlayer();
            }

            if (rocketOutOfScreen)
            {
                v.RocketFlying = false;

                v.SmokeList = new List<Vector2>();
                m.NextPlayer();
            }
        }

        private bool CheckOutOfScreen()
        {
            bool rocketOutOfScreen = v.RocketPosition.Y > v.ScreenHeight;
            rocketOutOfScreen |= v.RocketPosition.X < 0;
            rocketOutOfScreen |= v.RocketPosition.X > v.ScreenWidth;

            return rocketOutOfScreen;
        }

        private Vector2 CheckTerrainCollision()
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(v.RocketAngle) 
                * Matrix.CreateScale(v.RocketScaling) * Matrix.CreateTranslation(v.RocketPosition.X, v.RocketPosition.Y, 0);
            Matrix terrainMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = TexturesCollide(v.RocketColourArray, rocketMat, v.ForegroundColourArray, terrainMat);
            return terrainCollisionPoint;
        }

        private Vector2 CheckPlayersCollision()
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(v.RocketAngle) 
                * Matrix.CreateScale(v.RocketScaling) * Matrix.CreateTranslation(v.RocketPosition.X, v.RocketPosition.Y, 0);
            for (int i = 0; i < v.NumberOfPlayers; i++)
            {
                PlayerData player = v.Players[i];
                if (player.IsAlive)
                {
                    if (i != v.CurrentPlayer)
                    {
                        int xPos = (int)player.Position.X;
                        int yPos = (int)player.Position.Y;

                        Matrix carriageMat = Matrix.CreateTranslation(0, -v.CarriageTexture.Height, 0)
                            * Matrix.CreateScale(v.PlayerScaling) * Matrix.CreateTranslation(xPos, yPos, 0);
                        Vector2 carriageCollisionPoint = TexturesCollide(v.CarriageColourArray, carriageMat, v.RocketColourArray,
                            rocketMat);

                        if (carriageCollisionPoint.X > -1)
                        {
                            v.Players[i].IsAlive = false;
                            return carriageCollisionPoint;
                        }

                        Matrix cannonMat = Matrix.CreateTranslation(-11, -50, 0) * Matrix.CreateRotationZ(player.Angle)
                            * Matrix.CreateScale(v.PlayerScaling) * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);
                        Vector2 cannonCollisionPoint = TexturesCollide(v.CannonColourArray, cannonMat, v.RocketColourArray, rocketMat);
                        if (cannonCollisionPoint.X > -1)
                        {
                            v.Players[i].IsAlive = false;
                            return cannonCollisionPoint;
                        }
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        private Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1To2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1To2);

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

    }
}
