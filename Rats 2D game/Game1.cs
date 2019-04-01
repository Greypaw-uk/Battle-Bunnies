using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
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
            var graphics = new GraphicsDeviceManager(this);
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
                    gameState = GameState.SplashScreen;
                    equippedWeapon = EquippedWeapon.NoWeapon;
            }

            PreferredBackBufferWidth = 800;
            PreferredBackBufferHeight = 600;
            IsFullScreen = false;
            //ApplyChanges();
            Window.Title = "Battle Bunnies";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = GraphicsDevice;

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            font = Content.Load<SpriteFont>("myFont");

            splashScreen = Content.Load<Texture2D>("splash");

            titleScreen = Content.Load<Texture2D>("titleScreen");
            startButton = Content.Load<Texture2D>("start");
            weaponMenu = Content.Load<Texture2D>("weaponMenu");

            backgroundTexture = Content.Load<Texture2D>("background");
            bunnyTexture = Content.Load<Texture2D>("body");
            rocketTexture = Content.Load<Texture2D>("rocket");
            smokeTexture = Content.Load<Texture2D>("smoke");
            groundTexture = Content.Load<Texture2D>("candySkulls");
            explosionTexture = Content.Load<Texture2D>("explosion");

            noWeaponTexture = Content.Load<Texture2D>("noWeapon");
            launcherTexture = Content.Load<Texture2D>("launcher");
            grenadeTexture = Content.Load<Texture2D>("holdingGrenade");

            launcherIcon = Content.Load<Texture2D>("launcherIcon");
            grenadeIcon = Content.Load<Texture2D>("grenadeIcon");

            GenerateTerrainContour();
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            CreateForeground();

            playerScaling = 40.0f / (float)launcherTexture.Width;

            rocketColourArray = TextureTo2DArray(rocketTexture);
            launcherColourArray = TextureTo2DArray(launcherTexture);
            bunnyColourArray = TextureTo2DArray(bunnyTexture);

            explosionColourArray = TextureTo2DArray(explosionTexture);

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
                float fuseBurn = (float)gameTime.ElapsedGameTime.TotalSeconds;
                players[currentPlayer].weaponFuse -= fuseBurn;
            }

            //  GAME TIMER
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer -= elapsed;
            if (timer < 0)
            {
                //Timer expired, execute action
                timer = TIMER;   //Reset Timer
            }

            //  PARTICLE GENERATION
            if (particleList.Count > 0)
            {
                UpdateParticles(gameTime);
            }

            //  GAME MUSIC
            if(gameState.Equals(GameState.SplashScreen) || (gameState.Equals(GameState.TitleScreen)))
            {
                MediaPlayer.Play(titleTheme);
            }
            else
            {
                MediaPlayer.Stop();
            }

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


        //      #################################################
        //      #                                               #
        //      #               GAME LOGIC                      #
        //      #                                               #
        //      #################################################


        public void FireWeapon()
        {
            if (canShoot)
            {
                canShoot = false;
                switch (equippedWeapon)
                {
                    case EquippedWeapon.NoWeapon:

                        break;

                    case EquippedWeapon.RocketLauncher:
                    {
                        rocketFlying = true;
                        launch.Play();

                        projectilePosition = players[currentPlayer].Position;
                        projectilePosition.X += 20;
                        projectilePosition.Y -= 10;
                        projectileAngle = players[currentPlayer].Angle;
                        Vector2 up = new Vector2(0, -1);
                        Matrix rotMatrix = Matrix.CreateRotationZ(projectileAngle);
                        projectileDirection = Vector2.Transform(up, rotMatrix);
                        projectileDirection *= players[currentPlayer].Power / 50.0f;
                    }
                    break;

                    case EquippedWeapon.Grenade:
                    {
                        grenadeThrown = true;

                        projectilePosition = players[currentPlayer].Position;
                        projectilePosition.X += 20;
                        projectilePosition.Y -= 10;
                        projectileAngle = players[currentPlayer].Angle;
                        Vector2 grenadeGrav = new Vector2(0, -1);
                        Matrix grenadeSpin = Matrix.CreateRotationZ(projectileAngle);
                        projectileDirection = Vector2.Transform(grenadeGrav, grenadeSpin);
                        projectileDirection *= players[currentPlayer].Power / 50.0f;
                    }
                    break;
                }
            }
        }

        public void UpdateRocket()
        {
            if (rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                projectileDirection += gravity / 10.0f;
                projectilePosition += projectileDirection;
                projectileAngle = (float)Math.Atan2(projectileDirection.X, -projectileDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = projectilePosition;
                    smokePos.X += randomiser.Next(10) - 5;
                    smokePos.Y += randomiser.Next(10) - 5;
                    smokeList.Add(smokePos);
                }
            }
        }

        public void UpdateGrenade(GameTime gameTime)
        {
            if (grenadeThrown)
            {
                if (players[currentPlayer].weaponFuse <=0)
                {
                    smokeList = new List<Vector2>();

                    AddExplosion(projectilePosition, 10, 80.0f, 2000.0f, gameTime);
                    hitTerrain.Play();

                    grenadeThrown = false;

                    NextPlayer();
                }
                Vector2 gravity = new Vector2(0, 1);
                projectileDirection += gravity / 10.0f;
                projectilePosition += projectileDirection;
                projectileAngle = (float)Math.Atan2(projectileDirection.X, -projectileDirection.Y);
            }
        }

        public bool CheckOutOfScreen()
        {
            bool projectileOutOfScreen = projectilePosition.Y > screenHeight;
            projectileOutOfScreen |= projectilePosition.X < 0;
            projectileOutOfScreen |= projectilePosition.X > screenWidth;

            return projectileOutOfScreen;
        }

        public void NextPlayer()
        {
            currentPlayer = currentPlayer + 1;
            currentPlayer = currentPlayer % numberOfPlayers;
            while (!players[currentPlayer].IsAlive)
            {
                currentPlayer = ++currentPlayer % numberOfPlayers;
            }
            players[currentPlayer].weaponFuse = 5.0f;
            players[currentPlayer].Angle = 0;
            players[currentPlayer].Power = 0;
            equippedWeapon = EquippedWeapon.NoWeapon;
            canShoot = false;
        }


        //      #################################################
        //      #                                               #
        //      #               COLLSION DETECTION              #
        //      #                                               #
        //      #################################################


        public Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
        {
            Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
            int width1 = tex1.GetLength(0);
            int height1 = tex1.GetLength(1);
            int width2 = tex2.GetLength(0);
            int height2 = tex2.GetLength(1);

            for (int x1 = 0; x1 < width1; x1++)
            {
                for (int y1 = 0; y1 < height1; y1++)
                {
                    Vector2 pos1 = new Vector2(x1, y1);
                    Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

                    int x2 = (int)pos2.X;
                    int y2 = (int)pos2.Y;
                    if ((x2 >= 0) && (x2 < width2))
                    {
                        if ((y2 >= 0) && (y2 < height2))
                        {
                            if (tex1[x1, y1].A > 0)
                            {
                                if (tex2[x2, y2].A > 0)
                                {
                                    Vector2 screenPos = Vector2.Transform(pos1, mat1);
                                    return screenPos;
                                }
                            }
                        }
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        public Vector2 CheckTerrainCollision()
        {
            Matrix projectileMat = Matrix.CreateTranslation(-42, -240, 0) 
                * Matrix.CreateRotationZ(projectileAngle) 
                * Matrix.CreateScale(projectileScaling)
                * Matrix.CreateTranslation(projectilePosition.X, projectilePosition.Y, 0);

            Matrix terrainMat = Matrix.Identity;
            Vector2 terrainCollisionPoint =
                TexturesCollide(rocketColourArray, projectileMat, foregroundColourArray, terrainMat);
            return terrainCollisionPoint;
        }

        public Vector2 CheckPlayersCollision()
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) 
                * Matrix.CreateRotationZ(projectileAngle) 
                * Matrix.CreateScale(projectileScaling) 
                * Matrix.CreateTranslation(projectilePosition.X, projectilePosition.Y, 0);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                PlayerData player = players[i];
                if (player.IsAlive)
                {
                    if (i != currentPlayer)
                    {
                        int xPos = (int)player.Position.X;
                        int yPos = (int)player.Position.Y;

                        Matrix launcherMat = Matrix.CreateTranslation(0, -launcherTexture.Height, 0) 
                            * Matrix.CreateScale(playerScaling) 
                            * Matrix.CreateTranslation(xPos, yPos, 0);

                        Vector2 launcherCollisionPoint = TexturesCollide(launcherColourArray, launcherMat, rocketColourArray, rocketMat);

                        if (launcherCollisionPoint.X > -1)
                        {
                            players[i].IsAlive = false;
                            return launcherCollisionPoint;
                        }

                        Matrix bunnyMat = Matrix.CreateTranslation(-11, -50, 0) 
                            * Matrix.CreateRotationZ(player.Angle) 
                            * Matrix.CreateScale(playerScaling) 
                            * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);

                        Vector2 bunnyCollisionPoint = TexturesCollide(bunnyColourArray, bunnyMat, rocketColourArray, rocketMat);
                        if (bunnyCollisionPoint.X > -1)
                        {
                            players[i].IsAlive = false;
                            return bunnyCollisionPoint;
                        }
                    }
                }
            }
            return new Vector2(-1, -1);
        }

        public void CheckCollisions(GameTime gameTime)
        {
            Vector2 terrainCollisionPoint = CheckTerrainCollision();
            Vector2 playerCollisionPoint = CheckPlayersCollision();
            bool projectileOutOfScreen = CheckOutOfScreen();

            // Check Projectile Collision with Player

            if (playerCollisionPoint.X > -1)
            {
                if(rocketFlying)
                {
                    rocketFlying = false;

                    smokeList = new List<Vector2>();
                    AddExplosion(playerCollisionPoint, 10, 80.0f, 2000.0f, gameTime);

                    hitTerrain.Play();
                    hitbunny.Play();
                    NextPlayer();
                }
                if (grenadeThrown)
                {
                    // TODO ensure grenade does not kill player on contact unless exploding
                }
            }


            // Check Projectile Collision with Terrain
            if (terrainCollisionPoint.X > -1)
            {
                if (rocketFlying)
                {
                    rocketFlying = false;

                    smokeList = new List<Vector2>();
                    AddExplosion(terrainCollisionPoint, 4, 30.0f, 1000.0f, gameTime);

                    hitTerrain.Play();
                    NextPlayer();
                }
                else if (grenadeThrown)
                {
                    //  Bounce that mofo 
                    if(timer <= 0)
                    {
                        var terrain = new Vector2(terrainContour[(int) terrainCollisionPoint.Y] - 1,
                            terrainContour[(int) terrainCollisionPoint.Y] + 1);
                        terrain.Normalize();
                        var reflection = Vector2.Reflect(projectileDirection, terrain);
                        projectileDirection = -reflection;
                        timer = 0.5f;
                    }
                }
            }


            // 'Removes' projectiles when they touch the bottom of the screen, or they go too far left/right
            if (projectileOutOfScreen)
            {
                rocketFlying = false;
                grenadeThrown = false;

                smokeList = new List<Vector2>();
                NextPlayer();
            }
        }
    }
}