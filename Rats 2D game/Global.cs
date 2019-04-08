using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace BattleBunnies
{
    public static class Global
    {
        public enum GameState
        {
            SplashScreen,
            TitleScreen,
            Playing,
            Paused,
            WeaponMenu
        }

        public static GameState gameState;


        //  SCREEN SETUP
        public static GraphicsDeviceManager graphics;
        public static GraphicsDevice device;

        public static int screenWidth;
        public static int screenHeight;

        public static int PreferredBackBufferWidth;
        public static int PreferredBackBufferHeight;
        public static bool IsFullScreen;


        //  GUI 
        public static Texture2D splashScreen;

        public static Texture2D titleScreen;
        public static Texture2D startButton;
        public static Texture2D weaponMenu;

        public static Color myTransparentColor = new Color(0, 0, 0, 127);

        public static SpriteFont font;


        //  GAME TEXTURES
        public static Texture2D backgroundTexture;
        public static Texture2D foregroundTexture;

        public static Texture2D bunnyTexture;
        public static Texture2D rocketTexture;
        public static Texture2D smokeTexture;
        public static Texture2D groundTexture;
        public static Texture2D explosionTexture;

        public static Texture2D noWeaponTexture;
        public static Texture2D launcherTexture;
        public static Texture2D grenadeTexture;

        public static Texture2D launcherIcon;
        public static Texture2D grenadeIcon;


        //  SOUND EFFECTS
        public static SoundEffect hitbunny;

        public static SoundEffect hitTerrain;
        public static SoundEffect launch;

        public static Song titleTheme;
        public static Song ukulele;


        //  Colour Arrays
        public static Color[,] rocketColourArray;

        public static Color[,] foregroundColourArray;
        public static Color[,] launcherColourArray;
        public static Color[,] bunnyColourArray;
        public static Color[,] explosionColourArray;
        public static Color[,] grenadeColourArray;


        //  Misc
        public static List<Vector2> smokeList = new List<Vector2>();

        public static Random randomiser = new Random();
        public static int[] terrainContour;
        public static List<ParticleData> particleList = new List<ParticleData>();


        //  GAME TIMER
        public static float timer = 0;
        public static float TIMER = 0;
    }
}