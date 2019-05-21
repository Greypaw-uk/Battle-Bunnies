using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using static BattleBunnies.CollisionDetection;
using static BattleBunnies.Global;
using static BattleBunnies.Players;

namespace BattleBunnies
{
    public static class Terrain
    {
        public static void CreateForeground()
        {
            Color[,] groundColors = TextureTo2DArray(groundTexture);
            Color[] foregroundColors = new Color[screenWidth * screenHeight];

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    if (y > terrainContour[x])
                    {
                        foregroundColors[x + y * screenWidth] = groundColors[x % groundTexture.Width, y % groundTexture.Height];
                    }
                    else
                    {
                        foregroundColors[x + y * screenWidth] = Color.Transparent;
                    }
                }
            }

            foregroundTexture = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(foregroundColors);

            foregroundColourArray = TextureTo2DArray(foregroundTexture);
        }


        public static void GenerateTerrainContour()
        //  Generates 3 waves with a random offset, which will form the foreground
        {
            terrainContour = new int[screenWidth];

            double rand1 = randomiser.NextDouble() + 1;
            double rand2 = randomiser.NextDouble() + 2;
            double rand3 = randomiser.NextDouble() + 3;

            float offset = screenHeight / 2;
            float peakheight = 80;
            float flatness = 70;

            for (int x = 0; x < screenWidth; x++)
            {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                terrainContour[x] = (int)height;
            }
        }

        public static void FlattenTerrainBelowPlayers()
        {
            foreach (PlayerData player in players)
            {
                if (player.IsAlive)
                {
                    for (int x = 0; x < 40; x++)
                    {
                        terrainContour[(int)player.Position.X + x] = terrainContour[(int)player.Position.X];
                    }
                }
            }
        }

        public static void AddCrater(Color[,] tex, Matrix mat)
        {
            int width = tex.GetLength(0);
            int height = tex.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (tex[x, y].R > 10)
                    {
                        Vector2 imagePos = new Vector2(x, y);
                        Vector2 screenPos = Vector2.Transform(imagePos, mat);

                        int screenX = (int)screenPos.X;
                        int screenY = (int)screenPos.Y;

                        if ((screenX) > 0 && (screenX < screenWidth))
                        {
                            if (terrainContour[screenX] < screenY)
                            {
                                terrainContour[screenX] = screenY;
                            }
                        }
                    }
                }
            }
        }
    }
}
