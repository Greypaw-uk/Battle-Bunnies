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
        Methods m = new Methods();

        public GraphicsDeviceManager Graphics;

        public Logic()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Graphics.PreferredBackBufferWidth = 500;
            Graphics.PreferredBackBufferHeight = 500;
            Graphics.IsFullScreen = false;
            Graphics.ApplyChanges();
            Window.Title = "Rat's 2D Battle Game";

            v.CurrentPlayer = 0;
            v.NumberOfPlayers = 4;

            v.RocketFlying = false;
            v.RocketScaling = 0.1f;
        }

        protected override void LoadContent()
        {
            v.SpriteBatch = new SpriteBatch(GraphicsDevice);
            v.Device = Graphics.GraphicsDevice;

            v.Font = Content.Load<SpriteFont>("myFont");

            v.BackgroundTexture = Content.Load<Texture2D>("background");
            v.CarriageTexture = Content.Load<Texture2D>("carriage");
            v.CannonTexture = Content.Load<Texture2D>("cannon");
            v.RocketTexture = Content.Load<Texture2D>("rocket");
            v.SmokeTexture = Content.Load<Texture2D>("smoke");
            v.GroundTexture = Content.Load<Texture2D>("foreground");
            v.ExplosionTexture = Content.Load<Texture2D>("explosion");

            v.ScreenWidth = v.Device.PresentationParameters.BackBufferWidth;
            v.ScreenHeight = v.Device.PresentationParameters.BackBufferHeight;
            v.PlayerScaling = 40.0f / (float)v.CarriageTexture.Width;

            t.GenerateTerrainContour();
            m.SetUpPlayers();
            t.FlattenTerrainBelowPlayers();
            t.CreateForeground();

            v.RocketColourArray = m.TextureTo2DArray(v.RocketTexture);
            v.CarriageColourArray = m.TextureTo2DArray(v.CarriageTexture);
            v.CannonColourArray = m.TextureTo2DArray(v.CannonTexture);

            v.ExplosionColourArray = m.TextureTo2DArray(v.ExplosionTexture);

            v.HitCannon = Content.Load<SoundEffect>("hitcannon");
            v.HitTerrain = Content.Load<SoundEffect>("hitterrain");
            v.Launch = Content.Load<SoundEffect>("launch");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Exit();

            k.ProcessKeyboard();

            if (v.RocketFlying)
            {
                g.UpdateRocket();
                c.CheckCollisions(gameTime);
            }

            if (v.ParticleList.Count > 0)
                g.UpdateParticles(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            v.SpriteBatch.Begin();
            t.DrawScenery();
            g.DrawPlayers();
            g.DrawText();
            g.DrawRocket();
            g.DrawSmoke();
            v.SpriteBatch.End();

            v.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            g.DrawExplosion();
            v.SpriteBatch.End();

            base.Draw(gameTime);
        }

        

         
    }
}