using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Rats_2D_game
{
    public struct PlayerData
    {
        public Vector2 Position;
        public bool IsAlive;
        public Color Colour;
        public float Angle;
        public float Power;
    }

    class Variables
    {
        public Random Randomiser { get; set; }

        // Screen Settings
        public int screenWidth;
        public int screenHeight;

        // Player Settings
        public PlayerData[] players;
        public float playerScaling;
        public int currentPlayer { get; set; }
        public int NumberOfPlayers { get; set; }
        

        // Rocket Settings
        public bool rocketFlying { get; set; }

        public Vector2 rocketPosition;
        public Vector2 rocketDirection;
        public float rocketAngle { get; set; }
        public float rocketScaling { get; set; }

        // Particles

        

        // Colours
        public Color[,] explosionColourArray;
        public Color[,] rocketColourArray;
        public Color[,] foregroundColourArray;
        public Color[,] carriageColourArray;
        public Color[,] cannonColourArray;

        // Textures & Graphics
        public GraphicsDeviceManager graphics;
        public GraphicsDevice device;

        public SpriteBatch spriteBatch;
        public SpriteFont font;

        public Texture2D carriageTexture;
        public Texture2D cannonTexture;
        public Texture2D rocketTexture;
        public Texture2D smokeTexture;
        public Texture2D explosionTexture;

        // Sounds 
        public SoundEffect hitCannon;
        public SoundEffect hitTerrain;
        public SoundEffect launch;    
    }
}
