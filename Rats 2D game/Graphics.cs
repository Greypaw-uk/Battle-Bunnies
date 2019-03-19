using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Rats_2D_game
{
    class Graphics
    {
        Variables v = new Variables();
        Terrain t = new Terrain();

        private Random _randomiser = new Random();

        public Graphics()
        {
            t = new Terrain();
            v = new Variables();
        }

        // PLAYERS
        public void DrawPlayers()
        {
            foreach (PlayerData player in v.Players)
            {
                if (player.IsAlive)
                {
                    int xPos = (int)player.Position.X;
                    int yPos = (int)player.Position.Y;
                    Vector2 cannonOrigin = new Vector2(11, 50);

                    v.SpriteBatch.Draw(v.CannonTexture, new Vector2(xPos + 20, yPos - 10), null, player.Colour, player.Angle,
                        cannonOrigin, v.PlayerScaling, SpriteEffects.None, 1);
                    v.SpriteBatch.Draw(v.CarriageTexture, player.Position, null, player.Colour, 0,
                        new Vector2(0, v.CarriageTexture.Height), v.PlayerScaling, SpriteEffects.None, 0);
                }
            }
        }

        // UI
        public void DrawText()
        {
            PlayerData player = v.Players[v.CurrentPlayer];
            int currentAngle = (int)MathHelper.ToDegrees(player.Angle);
            v.SpriteBatch.DrawString(v.Font, "Cannon angle: " + currentAngle, new Vector2(20, 20), player.Colour);
            v.SpriteBatch.DrawString(v.Font, "Cannon power: " + player.Power, new Vector2(20, 45), player.Colour);
        }

        // ROCKET
        public void DrawRocket()
        {
            if (v.RocketFlying)
                v.SpriteBatch.Draw(v.RocketTexture, v.RocketPosition, null, v.Players[v.CurrentPlayer].Colour, v.RocketAngle,
                    new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
        }

        public void UpdateRocket()
        {
            if (v.RocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                v.RocketDirection += gravity / 10.0f;
                v.RocketPosition += v.RocketDirection;
                v.RocketAngle = (float)Math.Atan2(v.RocketDirection.X, -v.RocketDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = v.RocketPosition;
                    smokePos.X += _randomiser.Next(10) - 5;
                    smokePos.Y += _randomiser.Next(10) - 5;
                    v.SmokeList.Add(smokePos);
                }
            }
        }

        //PARTICLES
        public void DrawSmoke()
        {
            foreach (Vector2 smokePos in v.SmokeList)
                v.SpriteBatch.Draw(v.SmokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
        }

        public void DrawExplosion()
        {
            for (int i = 0; i < v.ParticleList.Count; i++)
            {
                ParticleData particle = v.ParticleList[i];
                v.SpriteBatch.Draw(v.ExplosionTexture, particle.Position, null, particle.ModColour, i,
                    new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
            }
        }

        public void UpdateParticles(GameTime gameTime)
        {
            float now = (float)gameTime.TotalGameTime.TotalMilliseconds;
            for (int i = v.ParticleList.Count - 1; i >= 0; i--)
            {
                ParticleData particle = v.ParticleList[i];
                float timeAlive = now - particle.BirthTime;

                if (timeAlive > particle.MaxAge)
                {
                    v.ParticleList.RemoveAt(i);
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

                    v.ParticleList[i] = particle;
                }
            }
        }

        public void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
            float rotation = (float)_randomiser.Next(10);
            Matrix mat = Matrix.CreateTranslation(-v.ExplosionTexture.Width / 2, -v.ExplosionTexture.Height / 2, 0)
                         * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(size / (float)v.ExplosionTexture.Width * 2.0f)
                         * Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
            t.AddCrater(v.ExplosionColourArray, mat);

            for (int i = 0; i < v.Players.Length; i++)
                v.Players[i].Position.Y = v.TerrainContour[(int)v.Players[i].Position.X];
            t.FlattenTerrainBelowPlayers();
            t.CreateForeground();
        }

        private void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge, GameTime gameTime)
        {
            ParticleData particle = new ParticleData
            {
                OrginalPosition = explosionPos
            };
            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float)gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColour = Color.White;

            float particleDistance = (float)_randomiser.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(_randomiser.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            v.ParticleList.Add(particle);
        }
    }
}
