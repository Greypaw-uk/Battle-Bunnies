using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using static BattleBunnies.Engine;
using static BattleBunnies.Global;
using static BattleBunnies.Graphics;
using static BattleBunnies.Keymapping;
using static BattleBunnies.Players;

namespace BattleBunnies
{
    public class Game1 : Game
    {
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            switch (gameState)
            {
                case GameState.SplashScreen:
                {
                    IsMouseVisible = true;
                    break;
                }
                case GameState.TitleScreen:
                {
                    IsMouseVisible = true;
                    break;
                }
            }

            gameState = GameState.SplashScreen;
            equippedWeapon = EquippedWeapon.NoWeapon;

            PreferredBackBufferWidth = 800;
            PreferredBackBufferHeight = 600;
            IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Battle Bunnies";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Screen setup
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = GraphicsDevice;

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            font = Content.Load<SpriteFont>("myFont");

            splashScreen = Content.Load<Texture2D>("splash");

            titleScreen = Content.Load<Texture2D>("titleScreen");
            startButton = Content.Load<Texture2D>("start");
            weaponMenu = Content.Load<Texture2D>("weaponMenu");

            // Textures
            backgroundTexture = Content.Load<Texture2D>("background");
            bunnyTexture = Content.Load<Texture2D>("body");
            rocketTexture = Content.Load<Texture2D>("rocket");
            smokeTexture = Content.Load<Texture2D>("smoke");
            groundTexture = Content.Load<Texture2D>("candySkulls");
            explosionTexture = Content.Load<Texture2D>("explosion");

            noWeaponTexture = Content.Load<Texture2D>("noWeapon");
            launcherTexture = Content.Load<Texture2D>("launcher");
            grenadeTexture = Content.Load<Texture2D>("holdingGrenade");

            // Icons
            launcherIcon = Content.Load<Texture2D>("launcherIcon");
            grenadeIcon = Content.Load<Texture2D>("grenadeIcon");

            // Logic set up
            GenerateTerrainContour();
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            CreateForeground();

            playerScaling = 40.0f / launcherTexture.Width;

            rocketColourArray = TextureTo2DArray(rocketTexture);
            launcherColourArray = TextureTo2DArray(launcherTexture);
            bunnyColourArray = TextureTo2DArray(bunnyTexture);

            explosionColourArray = TextureTo2DArray(explosionTexture);

            // Sounds
            //titleTheme = Content.Load<Song>("titleTheme");
            hitbunny = Content.Load<SoundEffect>("rabbitDeath");
            hitTerrain = Content.Load<SoundEffect>("hitterrain");
            launch = Content.Load<SoundEffect>("launch");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            //  MOUSE CONTROLS
            ProcessMouse();
            ProcessKeyboard();

            // Store last state for comparrison
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            // KEYBOARD CONTROLS
            // Store last state for comparrison
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            //  WEAPON LOGIC

            if (rocketFlying)
            {
                UpdateRocket();
                CheckCollisions(gameTime);
            }

            if (grenadeThrown)
            {
                UpdateGrenade(gameTime);
                CheckCollisions(gameTime);

                //  FUSE TIMER
                var fuseBurn = (float) gameTime.ElapsedGameTime.TotalSeconds;
                players[currentPlayer].weaponFuse -= fuseBurn;
            }

            //  GAME TIMER
            var elapsed = (float) gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;
            if (timer < 0)
                timer = TIMER; //Reset Timer

            //  PARTICLE GENERATION
            if (particleList.Count > 0)
                UpdateParticles(gameTime);

            //  GAME MUSIC
            if (gameState.Equals(GameState.SplashScreen) || gameState.Equals(GameState.TitleScreen))
                MediaPlayer.Play(titleTheme);
            else
                MediaPlayer.Stop();

            //  FINISHED UPDATE CYCLE
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (gameState)
            {
                case GameState.SplashScreen:
                {
                    spriteBatch.Begin();
                    DrawSplashScreen();
                    spriteBatch.End();
                    break;
                }

                case GameState.TitleScreen:
                {
                    spriteBatch.Begin();
                    DrawTitleScreen();
                    spriteBatch.End();
                    break;
                }

                case GameState.Playing:
                {
                    spriteBatch.Begin();
                    DrawScenery();
                    DrawPlayers();
                    DrawText();
                    DrawRocket();
                    DrawGrenade();
                    DrawSmoke();
                    spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                    DrawExplosion();
                    spriteBatch.End();
                    break;
                }

                case GameState.WeaponMenu:
                {
                    spriteBatch.Begin();
                    DrawWeaponMenu();
                    spriteBatch.End();
                    break;
                }
            }

            base.Draw(gameTime);
        }
    }
}