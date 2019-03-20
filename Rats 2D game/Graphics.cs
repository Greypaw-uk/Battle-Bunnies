using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rats_2D_game
{
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

    class Graphics
    {
        private Variables v = new Variables();
        private Terrain t = new Terrain();

        public List<ParticleData> particleList = new List<ParticleData>();
        public List<Vector2> smokeList = new List<Vector2>();

        // PLAYERS
        public void DrawPlayers()
        {
            foreach (PlayerData player in v.players)
            {
                if (player.IsAlive)
                {
                    int xPos = (int)player.Position.X;
                    int yPos = (int)player.Position.Y;
                    Vector2 cannonOrigin = new Vector2(11, 50);

                    v.spriteBatch.Draw(v.cannonTexture, new Vector2(xPos + 20, yPos - 10), null, player.Colour, player.Angle,
                        cannonOrigin, v.playerScaling, SpriteEffects.None, 1);
                    v.spriteBatch.Draw(v.carriageTexture, player.Position, null, player.Colour, 0,
                        new Vector2(0, v.carriageTexture.Height), v.playerScaling, SpriteEffects.None, 0);
                }
            }
        }

        // UI
        public void DrawText()
        {
            PlayerData player = v.players[v.currentPlayer];
            int currentAngle = (int)MathHelper.ToDegrees(player.Angle);
            v.spriteBatch.DrawString(v.font, "Cannon angle: " + currentAngle, new Vector2(20, 20), player.Colour);
            v.spriteBatch.DrawString(v.font, "Cannon power: " + player.Power, new Vector2(20, 45), player.Colour);
        }

        // ROCKET
        public void DrawRocket()
        {
            if (v.rocketFlying)
                v.spriteBatch.Draw(v.rocketTexture, v.rocketPosition, null, v.players[v.currentPlayer].Colour, v.rocketAngle,
                    new Vector2(42, 240), 0.1f, SpriteEffects.None, 1);
        }

        public void UpdateRocket()
        {
            if (v.rocketFlying)
            {
                Vector2 gravity = new Vector2(0, 1);
                v.rocketDirection += gravity / 10.0f;
                v.rocketPosition += v.rocketDirection;
                v.rocketAngle = (float)Math.Atan2(v.rocketDirection.X, -v.rocketDirection.Y);

                for (int i = 0; i < 5; i++)
                {
                    Vector2 smokePos = v.rocketPosition;
                    smokePos.X += v.Randomiser.Next(10) - 5;
                    smokePos.Y += v.Randomiser.Next(10) - 5;
                    smokeList.Add(smokePos);
                }
            }
        }

        //PARTICLES
        public void DrawSmoke()
        {
            foreach (Vector2 smokePos in smokeList)
                v.spriteBatch.Draw(v.smokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f, SpriteEffects.None, 1);
        }

        public void DrawExplosion()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ParticleData particle = particleList[i];
                v.spriteBatch.Draw(v.explosionTexture, particle.Position, null, particle.ModColour, i,
                    new Vector2(256, 256), particle.Scaling, SpriteEffects.None, 1);
            }
        }

        public void UpdateParticles(GameTime gameTime)
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

        public void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge, GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
            float rotation = (float)v.Randomiser.Next(10);
            Matrix mat = Matrix.CreateTranslation(-v.explosionTexture.Width / 2, -v.explosionTexture.Height / 2, 0)
                         * Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(size / (float)v.explosionTexture.Width * 2.0f)
                         * Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
            t.AddCrater(v.explosionColourArray, mat);

            for (int i = 0; i < v.players.Length; i++)
                v.players[i].Position.Y = t.terrainContour[(int)v.players[i].Position.X];
            t.FlattenTerrainBelowPlayers();
            t.CreateForeground();
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

            float particleDistance = (float)v.Randomiser.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(v.Randomiser.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            particleList.Add(particle);
        }

        // TEXTURES
        public Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height];
            texture.GetData(colors1D);

            Color[,] colors2D = new Color[texture.Width, texture.Height];
            for (int x = 0; x < texture.Width; x++)
            for (int y = 0; y < texture.Height; y++)
                colors2D[x, y] = colors1D[x + y * texture.Width];

            return colors2D;
        }
    }
}
