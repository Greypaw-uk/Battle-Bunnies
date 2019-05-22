using System.Security.Permissions;
using Microsoft.Xna.Framework;
using static BattleBunnies.Global;

namespace BattleBunnies
{
    public struct PlayerData
    {
        public Vector2 Position;
        public bool IsAlive;
        public Color Colour;
        public float Angle;
        public float Power;
        public float WeaponFuse;
        public float Health;
    }

    public static class Players
    {
        //  Player Variables 
        public static PlayerData[] players;

        public static int numberOfPlayers = 4;
        public static float playerScaling;
        public static int currentPlayer = 0;

        //      #################################################
        //      #                                               #
        //      #               PLAYER LOGIC                    #
        //      #                                               #
        //      #################################################


        public static void SetUpPlayers()
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

            players = new PlayerData[numberOfPlayers];
            for (int i = 0; i < numberOfPlayers; i++)
            {
                players[i].WeaponFuse = 5.0f;
                players[i].IsAlive = true;
                players[i].Colour = playerColors[i];
                players[i].Angle = MathHelper.ToRadians(90);
                players[i].Power = 0;
                players[i].Position = new Vector2();
                players[i].Position.X = screenWidth / (numberOfPlayers + 1) * (i + 1);
                players[i].Position.Y = terrainContour[(int) players[i].Position.X];
                players[i].Health = 100;
            }
        }

        public static void PlayerDeath()
        {
            var _players = numberOfPlayers;
            var _winningTeam = "";

            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (!players[i].IsAlive)
                {
                    _players--;
                    if (_players == 1)
                    {
                        // TODO Make this work!
                    }
                }
                else
                {
                    _winningTeam = players[i].Colour.ToString();
                }
            }
        }
    }
}
