using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rats_2D_game
{
    class Terrain
    {
        Variables v = new Variables();
        Methods m = new Methods();

        private Random Randomiser = new Random();

        public void CreateForeground()
        {
            Color[,] groundColors = m.TextureTo2DArray(v.GroundTexture);
            Color[] foregroundColors = new Color[v.ScreenWidth * v.ScreenHeight];

            for (int x = 0; x < v.ScreenWidth; x++)
            {
                for (int y = 0; y < v.ScreenHeight; y++)
                {
                    if (y > v.TerrainContour[x])
                        foregroundColors[x + y * v.ScreenWidth] = groundColors[x % v.GroundTexture.Width, y % v.GroundTexture.Height];
                    else
                        foregroundColors[x + y * v.ScreenWidth] = Color.Transparent;
                }
            }

            v.ForegroundTexture = new Texture2D(v.Device, v.ScreenWidth, v.ScreenHeight, false, SurfaceFormat.Color);
            v.ForegroundTexture.SetData(foregroundColors);

            v.ForegroundColourArray = m.TextureTo2DArray(v.ForegroundTexture);
        }

        public void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, v.ScreenWidth, v.ScreenHeight);
            v.SpriteBatch.Draw(v.BackgroundTexture, screenRectangle, Color.White);
            v.SpriteBatch.Draw(v.ForegroundTexture, screenRectangle, Color.White);
        }

        public void GenerateTerrainContour()
        {
            v.TerrainContour = new int[v.ScreenWidth];

            double rand1 = Randomiser.NextDouble() + 1;
            double rand2 = Randomiser.NextDouble() + 2;
            double rand3 = Randomiser.NextDouble() + 3;

            float offset = v.ScreenHeight / 2;
            float peakheight = 100;
            float flatness = 70;

            for (int x = 0; x < v.ScreenWidth; x++)
            {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                v.TerrainContour[x] = (int)height;
            }
        }

        public void FlattenTerrainBelowPlayers()
        {
            foreach (PlayerData player in v.Players)
                if (player.IsAlive)
                    for (int x = 0; x < 40; x++)
                        v.TerrainContour[(int)player.Position.X + x] = v.TerrainContour[(int)player.Position.X];
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

                        if ((screenX) > 0 && (screenX < v.ScreenWidth))
                            if (v.TerrainContour[screenX] < screenY)
                                v.TerrainContour[screenX] = screenY;
                    }
                }
            }
        }
    }
}
