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

    public struct ParticleData
    {
        public float BirthTime;
        public float MaxAge;
        public Vector2 OrginalPosition;
        public Vector2 Accelaration;
        public Vector2 Direction;
        public Vector2 Position;
        public float Scaling;
        public Color ModColour;
    }

    public class Variables
    {
        // Screen Settings
        public int ScreenWidth;
        public int ScreenHeight;

        // Player Settings
        public PlayerData[] Players;

        public float PlayerScaling;
        public int CurrentPlayer { get; set; }
        public int NumberOfPlayers { get; set; }

        // Rocket Settings
        public bool RocketFlying { get; set; }

        public Vector2 RocketPosition;
        public Vector2 RocketDirection;
        public float RocketAngle { get; set; }
        public float RocketScaling { get; set; }

        // Colours
        public Color[,] ExplosionColourArray;

        public Color[,] RocketColourArray;
        public Color[,] ForegroundColourArray;
        public Color[,] CarriageColourArray;
        public Color[,] CannonColourArray;

        // Textures & Graphics
        public GraphicsDevice Device;

        public SpriteBatch SpriteBatch;
        public SpriteFont Font;

        public Texture2D CarriageTexture;
        public Texture2D CannonTexture;
        public Texture2D RocketTexture;
        public Texture2D SmokeTexture;
        public Texture2D ExplosionTexture;

        public Texture2D BackgroundTexture;
        public Texture2D GroundTexture;
        public Texture2D ForegroundTexture;

        // Sounds 
        public SoundEffect HitCannon;

        public SoundEffect HitTerrain;
        public SoundEffect Launch;

        // Particles
        public List<ParticleData> ParticleList;
        public List<Vector2> SmokeList;

        // Terrain
        public int[] TerrainContour;


        
    }


}
