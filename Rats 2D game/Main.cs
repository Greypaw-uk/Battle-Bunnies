using BattleBunnies.Weapons;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using static BattleBunnies.CollisionDetection;
using static BattleBunnies.Engine;
using static BattleBunnies.Global;
using static BattleBunnies.Graphics;
using static BattleBunnies.Keymapping;
using static BattleBunnies.Players;
using static BattleBunnies.Music;
using static BattleBunnies.Terrain;

using static BattleBunnies.Weapons.RocketLauncher;
using static BattleBunnies.Weapons.Grenade;

// TODO Add reset button for testing
// TODO Add method for winning a game based on remaining bunnies

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


            //  SCREEN SETUP
            PreferredBackBufferWidth = 800;
            PreferredBackBufferHeight = 600;
            IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Battle Bunnies";

            currentTexture = powTexture;

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
            powTexture = Content.Load<Texture2D>("pow");

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
            grenadeColourArray = TextureTo2DArray(grenadeTexture);

            BunnyColourArray = TextureTo2DArray(launcherTexture);
            bunnyColourArray = TextureTo2DArray(bunnyTexture);

            explosionColourArray = TextureTo2DArray(explosionTexture);
            powColourArray = TextureTo2DArray(powTexture);

            // Sounds
            titleTheme = Content.Load<Song>("titleTheme");
            ukulele = Content.Load<Song>("ukulele");

            hitbunny = Content.Load<SoundEffect>("rabbitDeath");
            hitTerrain = Content.Load<SoundEffect>("hitterrain");
            launch = Content.Load<SoundEffect>("launch");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // WEAPON DAMAGE
            switch (equippedWeapon)
            {
                case EquippedWeapon.NoWeapon:
                {
                    weaponDamage = 0;
                    break;
                }
                case EquippedWeapon.RocketLauncher:
                {
                    weaponDamage = 50.0f;
                    break;
                }
                case EquippedWeapon.Grenade:
                {
                    weaponDamage = 45.0f;
                    break;
                }
            }

            // BUNNY UPDATES
            for (int i = 0; i < numberOfPlayers; i++)
            {
                if (players[i].IsAlive && players[i].Health <= 0)
                {
                    players[i].IsAlive = false;
                    hitbunny.Play();
                    // TODO add some form of effect for a dead bunny - thinking a wreath appearing
                    
                }
            }

            //  MOUSE CONTROLS
            ProcessMouse();

            // Store last state for comparrison
            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            // KEYBOARD CONTROLS
            ProcessKeyboard();

            // Store last state for comparrison
            lastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            //  WEAPON LOGIC

            if (RocketLauncher.rocketFlying)
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

            //  MUSIC
            PlayMusic();
           

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
                        
                        DisplayPlayerHealth();
                    spriteBatch.End();

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
                        DrawExplosion(currentTexture);
                        DrawSmoke(); // TODO Move back to basic draw?
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