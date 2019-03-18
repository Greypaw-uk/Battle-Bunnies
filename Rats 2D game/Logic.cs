using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Rats_2D_game
{


    public class Logic : Game
    {
        Terrain t = new Terrain();
        Variables v = new Variables();
        CollisionDetection c = new CollisionDetection();
        Graphics g = new Graphics();
        KeyMap k = new KeyMap();

        public Logic()
        {
            v.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            v.graphics.PreferredBackBufferWidth = 500;
            v.graphics.PreferredBackBufferHeight = 500;
            v.graphics.IsFullScreen = false;
            v.graphics.ApplyChanges();
            Window.Title = "Rat's 2D Battle Game";

            v.currentPlayer = 0;
            v.NumberOfPlayers = 4;

            v.rocketFlying = false;
            float rocketScaling = 0.1f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            v.spriteBatch = new SpriteBatch(GraphicsDevice);
            v.device = v.graphics.GraphicsDevice;

            v.font = Content.Load<SpriteFont>("myFont");

            t.backgroundTexture = Content.Load<Texture2D>("background");
            v.carriageTexture = Content.Load<Texture2D>("carriage");
            v.cannonTexture = Content.Load<Texture2D>("cannon");
            v.rocketTexture = Content.Load<Texture2D>("rocket");
            v.smokeTexture = Content.Load<Texture2D>("smoke");
            t.groundTexture = Content.Load<Texture2D>("foreground");
            v.explosionTexture = Content.Load<Texture2D>("explosion");

            v.screenWidth = v.device.PresentationParameters.BackBufferWidth;
            v.screenHeight = v.device.PresentationParameters.BackBufferHeight;
            v.playerScaling = 40.0f / (float)v.carriageTexture.Width;

            t.GenerateTerrainContour();
            SetUpPlayers();
            t.FlattenTerrainBelowPlayers();
            t.CreateForeground();

            v.rocketColourArray = g.TextureTo2DArray(v.rocketTexture);
            v.carriageColourArray = g.TextureTo2DArray(v.carriageTexture);
            v.cannonColourArray = g.TextureTo2DArray(v.cannonTexture);

            v.explosionColourArray = g.TextureTo2DArray(v.explosionTexture);

            v.hitCannon = Content.Load<SoundEffect>("hitcannon");
            v.hitTerrain = Content.Load<SoundEffect>("hitterrain");
            v.launch = Content.Load<SoundEffect>("launch");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            k.ProcessKeyboard();

            if (v.rocketFlying)
            {
                g.UpdateRocket();
                c.CheckCollisions(gameTime);
            }

            if (g.particleList.Count > 0)
                g.UpdateParticles(gameTime);

            base.Update(gameTime);
        }

        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            v.spriteBatch.Begin();
            t.DrawScenery();
            g.DrawPlayers();
            g.DrawText();
            g.DrawRocket();
            g.DrawSmoke();
            v.spriteBatch.End();

            v.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            g.DrawExplosion();
            v.spriteBatch.End();

            base.Draw(gameTime);
        }

        public void NextPlayer()
        {
            v.currentPlayer = v.currentPlayer + 1;
            v.currentPlayer = v.currentPlayer % v.NumberOfPlayers;
            while (!v.players[v.currentPlayer].IsAlive)
                v.currentPlayer = ++v.currentPlayer % v.NumberOfPlayers;
        }

        

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

            v.players = new PlayerData[v.NumberOfPlayers];
            for (int i = 0; i < v.NumberOfPlayers; i++)
            {
                v.players[i].IsAlive = true;
                v.players[i].Colour = playerColors[i];
                v.players[i].Angle = MathHelper.ToRadians(90);
                v.players[i].Power = 100;
                v.players[i].Position = new Vector2();
                v.players[i].Position.X = v.screenWidth / (v.NumberOfPlayers + 1) * (i + 1);
                v.players[i].Position.Y = t.terrainContour[(int)v.players[i].Position.X];
            }
        }
    }
}