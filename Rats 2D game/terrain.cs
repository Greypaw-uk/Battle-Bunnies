using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rats_2D_game
{
    class Terrain
    {
        Variables v = new Variables();
        Graphics g = new Graphics();

        public int[] terrainContour;

        public Texture2D backgroundTexture;
        public Texture2D groundTexture;
        public Texture2D foregroundTexture;



        public void CreateForeground()
        {
            Color[,] groundColors = g.TextureTo2DArray(groundTexture);
            Color[] foregroundColors = new Color[v.screenWidth * v.screenHeight];

            for (int x = 0; x < v.screenWidth; x++)
            {
                for (int y = 0; y < v.screenHeight; y++)
                {
                    if (y > terrainContour[x])
                        foregroundColors[x + y * v.screenWidth] = groundColors[x % groundTexture.Width, y % groundTexture.Height];
                    else
                        foregroundColors[x + y * v.screenWidth] = Color.Transparent;
                }
            }

            foregroundTexture = new Texture2D(v.device, v.screenWidth, v.screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(foregroundColors);

            v.foregroundColourArray = g.TextureTo2DArray(foregroundTexture);
        }

        public void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, v.screenWidth, v.screenHeight);
            v.spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            v.spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        public void GenerateTerrainContour()
        {
            terrainContour = new int[v.screenWidth];

            double rand1 = v.Randomiser.NextDouble() + 1;
            double rand2 = v.Randomiser.NextDouble() + 2;
            double rand3 = v.Randomiser.NextDouble() + 3;

            float offset = v.screenHeight / 2;
            float peakheight = 100;
            float flatness = 70;

            for (int x = 0; x < v.screenWidth; x++)
            {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                terrainContour[x] = (int)height;
            }
        }

        public void FlattenTerrainBelowPlayers()
        {
            foreach (PlayerData player in v.players)
                if (player.IsAlive)
                    for (int x = 0; x < 40; x++)
                        terrainContour[(int)player.Position.X + x] = terrainContour[(int)player.Position.X];
        }

        public void AddCrater(Color[,] tex, Matrix mat)
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

                        if ((screenX) > 0 && (screenX < v.screenWidth))
                            if (terrainContour[screenX] < screenY)
                                terrainContour[screenX] = screenY;
                    }
                }
            }
        }
    }
}
