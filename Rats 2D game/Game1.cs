using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

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

    public class Game1 : Game
    {
//  Screen/Device set up
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GraphicsDevice device;

        int screenWidth;
        int screenHeight;

//  GAME TEXTURES
        Texture2D backgroundTexture;
        Texture2D foregroundTexture;
        Texture2D launcherTexture;
        Texture2D bunnyTexture;
        Texture2D rocketTexture;
        Texture2D smokeTexture;
        Texture2D groundTexture;
        Texture2D explosionTexture;

        private Texture2D launcherIcon;

//  GUI 
        private Texture2D splashScreen;
        private Texture2D titleScreen;
        private Texture2D startButton;

        SpriteFont font;

//  SOUND EFFECTS
        private SoundEffect hitbunny;
        private SoundEffect hitTerrain;
        private SoundEffect launch;

        private Song titleTheme;

//  Player Variables 
        PlayerData[] players;
        int numberOfPlayers = 4;
        float playerScaling;
        int currentPlayer = 0;
        public bool canShoot = false;

//  Weapons
        enum EquippedWeapon
        {
            NoWeapon,
            RocketLauncher
        }
        private EquippedWeapon equippedWeapon = EquippedWeapon.NoWeapon;

//  Rocket Variables
        bool rocketFlying = false;
        Vector2 rocketPosition;
        Vector2 rocketDirection;
        float rocketAngle;
        float rocketScaling = 0.1f;

//  Colour Arrays
        Color[,] rocketColourArray;
        Color[,] foregroundColourArray;
        Color[,] launcherColourArray;
        Color[,] bunnyColourArray;
        Color[,] explosionColourArray;

//  Misc
        List<Vector2> smokeList = new List<Vector2>(); Random randomiser = new Random();
        int[] terrainContour;
        List<ParticleData> particleList = new List<ParticleData>();

        enum GameState
        {
            SplashScreen,
            TitleScreen,
            Playing,
            Paused,
            WeaponMenu
        }
        GameState gameState = GameState.SplashScreen;

// Controls
        private MouseState lastMouseState;
        private MouseState mouseState;


// ===========================================


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

            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Battle Bunnies";

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;

            screenWidth = device.PresentationParameters.BackBufferWidth;
            screenHeight = device.PresentationParameters.BackBufferHeight;

            font = Content.Load<SpriteFont>("myFont");

            splashScreen = Content.Load<Texture2D>("splash");

            titleScreen = Content.Load<Texture2D>("title");
            startButton = Content.Load<Texture2D>("start");

            backgroundTexture = Content.Load<Texture2D>("background");
            bunnyTexture = Content.Load<Texture2D>("launcher");
            launcherTexture = Content.Load<Texture2D>("body");
            rocketTexture = Content.Load<Texture2D>("rocket");
            smokeTexture = Content.Load<Texture2D>("smoke");
            groundTexture = Content.Load<Texture2D>("foreground");
            explosionTexture = Content.Load<Texture2D>("explosion");

            launcherIcon = Content.Load<Texture2D>("launcherIcon");

            GenerateTerrainContour();
            SetUpPlayers();
            FlattenTerrainBelowPlayers();
            CreateForeground();

            playerScaling = 40.0f / (float) launcherTexture.Width;

            rocketColourArray = TextureTo2DArray(rocketTexture);
            launcherColourArray = TextureTo2DArray(launcherTexture);
            bunnyColourArray = TextureTo2DArray(bunnyTexture);

            explosionColourArray = TextureTo2DArray(explosionTexture);

            hitbunny = Content.Load<SoundEffect>("hitbunny");
            hitTerrain = Content.Load<SoundEffect>("hitterrain");
            launch = Content.Load<SoundEffect>("launch");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            ProcessMouse();
            ProcessKeyboard();

            lastMouseState = mouseState;
            mouseState = Mouse.GetState();

            if (rocketFlying)
            {
                UpdateRocket();
                CheckCollisions(gameTime);
            }

            if (particleList.Count > 0)
            {
                UpdateParticles(gameTime);
            }

            if(gameState.Equals(GameState.SplashScreen) || (gameState.Equals(GameState.TitleScreen)))
            {
                MediaPlayer.Play(titleTheme);
            }
            else
            {
                MediaPlayer.Stop();
            }

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
        //      #               PLAYER LOGIC                    #
        //      #                                               #
        //      #################################################


        private void SetUpPlayers()
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
                players[i].IsAlive = true;
                players[i].Colour = playerColors[i];
                players[i].Angle = MathHelper.ToRadians(90);
                players[i].Power = 100;
                players[i].Position = new Vector2();
                players[i].Position.X = screenWidth / (numberOfPlayers + 1) * (i + 1);
                players[i].Position.Y = terrainContour[(int)players[i].Position.X];
            }
        }


        //      #################################################
        //      #                                               #
        //      #               TERRAIN LOGIC                   #
        //      #                                               #
        //      #################################################


        private void CreateForeground()
        {
            Color[,] groundColors = TextureTo2DArray(groundTexture);
            Color[] foregroundColors = new Color[screenWidth * screenHeight];

            for (int x = 0; x < screenWidth; x++)
            {
                for (int y = 0; y < screenHeight; y++)
                {
                    if (y > terrainContour[x])
                    {
                        foregroundColors[x + y * screenWidth] = groundColors[x % groundTexture.Width, y % groundTexture.Height];
                    }
                    else
                    {
                        foregroundColors[x + y * screenWidth] = Color.Transparent;
                    }
                }
            }

            foregroundTexture = new Texture2D(device, screenWidth, screenHeight, false, SurfaceFormat.Color);
            foregroundTexture.SetData(foregroundColors);

            foregroundColourArray = TextureTo2DArray(foregroundTexture);
        }


        private void GenerateTerrainContour()
        {
            terrainContour = new int[screenWidth];

            double rand1 = randomiser.NextDouble() + 1;
            double rand2 = randomiser.NextDouble() + 2;
            double rand3 = randomiser.NextDouble() + 3;

            float offset = screenHeight / 2;
            float peakheight = 80;
            float flatness = 70;

            for (int x = 0; x < screenWidth; x++)
            {
                double height = peakheight / rand1 * Math.Sin((float)x / flatness * rand1 + rand1);
                height += peakheight / rand2 * Math.Sin((float)x / flatness * rand2 + rand2);
                height += peakheight / rand3 * Math.Sin((float)x / flatness * rand3 + rand3);
                height += offset;
                terrainContour[x] = (int)height;
            }
        }

        private void FlattenTerrainBelowPlayers()
        {
            foreach (PlayerData player in players)
            {
                if (player.IsAlive)
                {
                    for (int x = 0; x < 40; x++)
                    {
                        terrainContour[(int)player.Position.X + x] = terrainContour[(int)player.Position.X];
                    }
                }
            }
        }

        private void AddCrater(Color[,] tex, Matrix mat)
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

                        if ((screenX) > 0 && (screenX < screenWidth))
                        {
                            if (terrainContour[screenX] < screenY)
                            {
                                terrainContour[screenX] = screenY;
                            }
                        }
                    }
                }
            }
        }


        //      #################################################
        //      #                                               #
        //      #               UI AND STUFF                    #
        //      #                                               #
        //      #################################################


        private void DrawSplashScreen()
        {
            Rectangle splashScreenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(splashScreen, splashScreenRectangle, Color.White);
        }

        private void DrawTitleScreen()
        {
            var _startX = screenWidth * 0.2f;
            var _startY = screenHeight * 0.5f;

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(titleScreen, screenRectangle, Color.White);

            Rectangle startRectangle = new Rectangle((int)_startX, (int)_startY, 200, 100);
            spriteBatch.Draw(startButton, startRectangle, Color.White);

            // Clicking Start button
            if (mouseState.X > startRectangle.X
                && mouseState.X < startRectangle.X + startRectangle.Width
                && mouseState.Y > startRectangle.Y
                && mouseState.Y < startRectangle.Y + startRectangle.Height)
            {
                if (mouseState.LeftButton.Equals(ButtonState.Pressed) && lastMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    gameState = GameState.Playing;
                    canShoot = false;
                }  
            }
        }

        private void DrawWeaponMenu()
        {
            Rectangle weaponMenuRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(titleScreen, weaponMenuRectangle, Color.White);

        // Rocket Launcher

            Rectangle rocketRectangle = new Rectangle(0, 0, 100, 100);
            spriteBatch.Draw(launcherIcon, rocketRectangle, Color.White);

            if (mouseState.X > rocketRectangle.X
                && mouseState.X < rocketRectangle.X + launcherIcon.Width
                && mouseState.Y > rocketRectangle.Y
                && mouseState.Y < rocketRectangle.Y + launcherIcon.Height)
            {
                if (mouseState.LeftButton.Equals(ButtonState.Pressed) && lastMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    equippedWeapon = EquippedWeapon.RocketLauncher;
                    gameState = GameState.Playing;
                }
            }
        }


        //      #################################################
        //      #                                               #
        //      #               GRAPHICS                        #
        //      #                                               #
        //      #################################################


        private void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        private void DrawPlayers()
        {
            foreach (PlayerData player in players)
            {
                if (player.IsAlive)
                {
                    int xPos = (int)player.Position.X;
                    int yPos = (int)player.Position.Y;
                    Vector2 bunnyOrigin = new Vector2(22, 22);

                    spriteBatch.Draw(bunnyTexture, new Vector2(xPos + 20, yPos - 20), null, player.Colour, player.Angle, bunnyOrigin,
                        playerScaling, SpriteEffects.None, 1);
                    spriteBatch.Draw(launcherTexture, player.Position, null, player.Colour, 0, new Vector2(0, launcherTexture.Height), 
                        playerScaling, SpriteEffects.None, 0);
                }
            }
        }

        private void DrawText()
        {
            PlayerData player = players[currentPlayer];
            int currentAngle = (int)MathHelper.ToDegrees(player.Angle);
            spriteBatch.DrawString(font, "Shot angle: " + currentAngle, new Vector2(20, 20), player.Colour);
            spriteBatch.DrawString(font, "Shot power: " + player.Power, new Vector2(20, 45), player.Colour);
        }

        private void DrawRocket()
        {
            if (rocketFlying)
            {
                spriteBatch.Draw(rocketTexture, rocketPosition, null, players[currentPlayer].Colour, rocketAngle, 
                    new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
            }
        }

        private void DrawSmoke()
        {
            foreach (Vector2 smokePos in smokeList)
            {
                spriteBatch.Draw(smokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
            }
        }

        private void DrawExplosion()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ParticleData particle = particleList[i];
                spriteBatch.Draw(explosionTexture, particle.Position, null, particle.ModColour, i, new Vector2(256, 256), particle.Scaling, 
                    SpriteEffects.None, 1);
            }
        }

        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + y * texture.Width];
                }
            }

            return colors2D;
        }

        private void UpdateParticles(GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            for (int i = particleList.Count - 1; i >= 0; i--)
            {
                ParticleData particle = particleList[i];
                float timeAlive = now - particle.BirthTime;

                if (timeAlive > particle.MaxAge)
                {
                    particleList.RemoveAt(i);
                }
                else
                {
                    float relAge = timeAlive / particle.MaxAge;
                    particle.Position = 0.5f * particle.Accelaration * relAge * relAge + particle.Direction * relAge + particle.OrginalPosition;

                    float invAge = 1.0f - relAge;
                    particle.ModColour = new Color(new Vector4(invAge, invAge, invAge, invAge));

                    Vector2 positionFromCenter = particle.Position - particle.OrginalPosition;
                    float distance = positionFromCenter.Length();
                    particle.Scaling = (50.0f + distance) / 200.0f;

                    particleList[i] = particle;
                }
            }
        }

        private void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
            {
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
            }

            float rotation = (float)randomiser.Next(10);
            Matrix mat = Matrix.CreateTranslation(-explosionTexture.Width / 2, -explosionTexture.Height / 2, 0) * Matrix.CreateRotationZ(rotation) 
                * Matrix.CreateScale(size / (float)explosionTexture.Width * 2.0f) * Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
            AddCrater(explosionColourArray, mat);

            for (int i = 0; i < players.Length; i++)
            {
                players[i].Position.Y = terrainContour[(int)players[i].Position.X];
            }

            FlattenTerrainBelowPlayers();
            CreateForeground();
        }

        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
        {
            ParticleData particle = new ParticleData();

            particle.OrginalPosition = explosionPos;
            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColour = Color.White;

            float particleDistance = (float)randomiser.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(randomiser.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            particleList.Add(particle);
        }


        //      #################################################
        //      #                                               #
        //      #               GAME LOGIC                      #
        //      #                                               #
        //      #################################################


        private void UpdateRocket()
        {
            if (rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                rocketDirection += gravity / 10.0f;
                rocketPosition += rocketDirection;
                rocketAngle = (float)Math.Atan2(rocketDirection.X, -rocketDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = rocketPosition;
                    smokePos.X += randomiser.Next(10) - 5;
                    smokePos.Y += randomiser.Next(10) - 5;
                    smokeList.Add(smokePos);
                }
            }
        }

        private bool CheckOutOfScreen()
        {
            bool rocketOutOfScreen = rocketPosition.Y > screenHeight;
            rocketOutOfScreen |= rocketPosition.X < 0;
            rocketOutOfScreen |= rocketPosition.X > screenWidth;

            return rocketOutOfScreen;
        }

        private void NextPlayer()
        {
            currentPlayer = currentPlayer + 1;
            currentPlayer = currentPlayer % numberOfPlayers;
            while (!players[currentPlayer].IsAlive)
            {
                currentPlayer = ++currentPlayer % numberOfPlayers;
            }
            players[currentPlayer].Angle = 0;
            players[currentPlayer].Power = 0;
        }


        //      #################################################
        //      #                                               #
        //      #               COLLSION DETECTION              #
        //      #                                               #
        //      #################################################


        private Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
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

        private Vector2 CheckTerrainCollision()
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(rocketAngle) * Matrix.CreateScale(rocketScaling) 
                * Matrix.CreateTranslation(rocketPosition.X, rocketPosition.Y, 0);
            Matrix terrainMat = Matrix.Identity;
            Vector2 terrainCollisionPoint = TexturesCollide(rocketColourArray, rocketMat, foregroundColourArray, terrainMat);
            return terrainCollisionPoint;
        }

        private Vector2 CheckPlayersCollision()
        {
            Matrix rocketMat = Matrix.CreateTranslation(-42, -240, 0) * Matrix.CreateRotationZ(rocketAngle) * Matrix.CreateScale(rocketScaling) 
                * Matrix.CreateTranslation(rocketPosition.X, rocketPosition.Y, 0);
            for (int i = 0; i < numberOfPlayers; i++)
            {
                PlayerData player = players[i];
                if (player.IsAlive)
                {
                    if (i != currentPlayer)
                    {
                        int xPos = (int)player.Position.X;
                        int yPos = (int)player.Position.Y;

                        Matrix launcherMat = Matrix.CreateTranslation(0, -launcherTexture.Height, 0) * Matrix.CreateScale(playerScaling) 
                            * Matrix.CreateTranslation(xPos, yPos, 0);
                        Vector2 launcherCollisionPoint = TexturesCollide(launcherColourArray, launcherMat, rocketColourArray, rocketMat);

                        if (launcherCollisionPoint.X > -1)
                        {
                            players[i].IsAlive = false;
                            return launcherCollisionPoint;
                        }

                        Matrix bunnyMat = Matrix.CreateTranslation(-11, -50, 0) * Matrix.CreateRotationZ(player.Angle) 
                            * Matrix.CreateScale(playerScaling) * Matrix.CreateTranslation(xPos + 20, yPos - 10, 0);
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

        private void CheckCollisions(GameTime gameTime)
        {
            Vector2 terrainCollisionPoint = CheckTerrainCollision();
            Vector2 playerCollisionPoint = CheckPlayersCollision();
            bool rocketOutOfScreen = CheckOutOfScreen();

            if (playerCollisionPoint.X > -1)
            {
                rocketFlying = false;

                smokeList = new List<Vector2>(); AddExplosion(playerCollisionPoint, 10, 80.0f, 2000.0f, gameTime);

                hitbunny.Play();
                NextPlayer();
            }

            if (terrainCollisionPoint.X > -1)
            {
                rocketFlying = false;

                smokeList = new List<Vector2>(); AddExplosion(terrainCollisionPoint, 4, 30.0f, 1000.0f, gameTime);

                hitTerrain.Play();
                NextPlayer();
            }

            if (rocketOutOfScreen)
            {
                rocketFlying = false;

                smokeList = new List<Vector2>();
                NextPlayer();
            }
        }


        //      #################################################
        //      #                                               #
        //      #               KEYMAPPING STUFF                #
        //      #                                               #
        //      #################################################


        private void ProcessMouse()
        {
            Vector2 mousePointer = new Vector2(mouseState.X, mouseState.Y);

            switch (gameState)
            {
                case GameState.SplashScreen:
                    if (mouseState.LeftButton.Equals(ButtonState.Pressed) && (lastMouseState.LeftButton.Equals(ButtonState.Pressed)))
                    {
                        gameState = GameState.TitleScreen;
                    }

                break;

                case GameState.Playing:
                    Vector2 dPos = players[currentPlayer].Position - mousePointer;

                    players[currentPlayer].Angle = -(float)Math.Atan2(dPos.X, dPos.Y);

                    if (mouseState.LeftButton.Equals(ButtonState.Pressed))
                    {
                        canShoot = true;
                        players[currentPlayer].Power += 5;
                    }

                    else if (mouseState.LeftButton.Equals(ButtonState.Released))
                    {
                        if (canShoot)
                            switch (equippedWeapon)
                            {
                                case EquippedWeapon.NoWeapon:

                                break;

                                case EquippedWeapon.RocketLauncher:
                                    rocketFlying = true;
                                    canShoot = false;
                                    launch.Play();

                                    rocketPosition = players[currentPlayer].Position;
                                    rocketPosition.X += 20;
                                    rocketPosition.Y -= 10;
                                    rocketAngle = players[currentPlayer].Angle;
                                    Vector2 up = new Vector2(0, -1);
                                    Matrix rotMatrix = Matrix.CreateRotationZ(rocketAngle);
                                    rocketDirection = Vector2.Transform(up, rotMatrix);
                                    rocketDirection *= players[currentPlayer].Power / 50.0f;
                                break;
                            }
                    }

                break;
            }
        }

        private void ProcessKeyboard()
        {
            KeyboardState keybState = Keyboard.GetState();

            switch(gameState)
            {
                case GameState.Playing:

                    if (keybState.IsKeyDown(Keys.C))
                    {
                        gameState = GameState.WeaponMenu;
                        canShoot = false;
                    }

                    if (keybState.IsKeyDown(Keys.Left))
                    {
                        players[currentPlayer].Angle -= 0.01f;
                    }

                    if (keybState.IsKeyDown(Keys.Right))
                    {
                        players[currentPlayer].Angle += 0.01f;
                    }

                    if (players[currentPlayer].Angle > MathHelper.PiOver2)
                    {
                        players[currentPlayer].Angle = -MathHelper.PiOver2;
                    }

                    if (players[currentPlayer].Angle < -MathHelper.PiOver2)
                    {
                        players[currentPlayer].Angle = MathHelper.PiOver2;
                    }

                    if (keybState.IsKeyDown(Keys.Down))
                    {
                        players[currentPlayer].Power -= 1;
                    }

                    if (keybState.IsKeyDown(Keys.Up))
                    {
                        players[currentPlayer].Power += 1;
                    }

                    if (keybState.IsKeyDown(Keys.PageDown))
                    {
                        players[currentPlayer].Power -= 20;
                    }

                    if (keybState.IsKeyDown(Keys.PageUp))
                    {
                        players[currentPlayer].Power += 20;
                    }

                    if (players[currentPlayer].Power > 1000)
                    {
                        players[currentPlayer].Power = 1000;
                    }

                    if (players[currentPlayer].Power < 0)
                    {
                        players[currentPlayer].Power = 0;
                    }

                    if (keybState.IsKeyDown(Keys.Space))
                    {
                        rocketFlying = true;
                        launch.Play();

                        rocketPosition = players[currentPlayer].Position;
                        rocketPosition.X += 20;
                        rocketPosition.Y -= 10;
                        rocketAngle = players[currentPlayer].Angle;
                        Vector2 up = new Vector2(0, -1);
                        Matrix rotMatrix = Matrix.CreateRotationZ(rocketAngle);
                        rocketDirection = Vector2.Transform(up, rotMatrix);
                        rocketDirection *= players[currentPlayer].Power / 50.0f;
                    }
                break;

                case GameState.WeaponMenu:
                    if (keybState.IsKeyDown(Keys.Escape))
                    {
                        gameState = GameState.Playing;
                    }
                break;
            }
        }
    }
}