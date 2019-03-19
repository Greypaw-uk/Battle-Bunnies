using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rats_2D_game
{
    class Methods
    {
        Variables v = new Variables();

        public Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            for (int y = 0; y < texture.Height; y++)
                colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }

        public void SetUpPlayers()
        {
            Color[] playerColors = new Color[10];
            playerColors[0] = Color.Red;
            playerColors[1] = Color.Green;
            playerColors[2] = Color.Blue;
            playerColors[3] = Color.Purple;
            playerColors[4] = Color.Orange;
            playerColors[5] = Color.Indigo;
            playerColors[6] = Color.Yellow;
            playerColors[7] = Color.SaddleBrown;
            playerColors[8] = Color.Tomato;
            playerColors[9] = Color.Turquoise;

            v.Players = new PlayerData[v.NumberOfPlayers];
            for (int i = 0; i < v.NumberOfPlayers; i++)
            {
                v.Players[i].IsAlive = true;
                v.Players[i].Colour = playerColors[i];
                v.Players[i].Angle = MathHelper.ToRadians(90);
                v.Players[i].Power = 100;
                v.Players[i].Position = new Vector2(0, 0);

                v.Players[i].Position.X = v.ScreenWidth / (v.NumberOfPlayers + 1) * (i + 1);
                v.Players[i].Position.Y = v.TerrainContour[(int)v.Players[i].Position.X];
            }
        }

        public void NextPlayer()
        {
            v.CurrentPlayer = v.CurrentPlayer + 1;
            v.CurrentPlayer = v.CurrentPlayer % v.NumberOfPlayers;
            while (!v.Players[v.CurrentPlayer].IsAlive)
                v.CurrentPlayer = ++v.CurrentPlayer % v.NumberOfPlayers;
        }
    }
}
