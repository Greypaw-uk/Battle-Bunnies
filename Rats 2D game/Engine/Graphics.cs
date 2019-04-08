using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using static BattleBunnies.Engine;
using static BattleBunnies.Global;
using static BattleBunnies.Keymapping;
using static BattleBunnies.Music;
using static BattleBunnies.Players;
using static BattleBunnies.Terrain;

namespace BattleBunnies
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

    public static class Graphics
    {

        public static SpriteBatch spriteBatch;

        //      #################################################
        //      #                                               #
        //      #               UI AND STUFF                    #
        //      #                                               #
        //      #################################################


        public static void DrawSplashScreen()
        {
            Rectangle splashScreenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(splashScreen, splashScreenRectangle, Color.White);
        }

        public static void DrawTitleScreen()
        {
            musicPlaying = true;

            var _startX = screenWidth / 2;
            var _startY = screenHeight / 2;

            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(titleScreen, screenRectangle, Color.White);

            Rectangle startRectangle = new Rectangle((int) _startX - 100, (int) _startY - 50, 200, 100);
            spriteBatch.Draw(startButton, startRectangle, Color.White);

            // Clicking Start button
            if (mouseState.X > startRectangle.X
                & mouseState.X < startRectangle.X + startRectangle.Width
                & mouseState.Y > startRectangle.Y
                & mouseState.Y < startRectangle.Y + startRectangle.Height)
            {
                if (mouseState.LeftButton.Equals(ButtonState.Pressed) 
                    & lastMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    gameState = GameState.Playing;
                }
            }
        }

        public static void DrawWeaponMenu()
        {
            Rectangle weaponMenuRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(weaponMenu, weaponMenuRectangle, myTransparentColor);

            // Rocket Launcher

            Rectangle rocketRectangle = new Rectangle(30, 10, 100, 100);
            spriteBatch.Draw(launcherIcon, rocketRectangle, Color.White);

            if (mouseState.X > rocketRectangle.X
                & mouseState.X < rocketRectangle.X + launcherIcon.Width
                & mouseState.Y > rocketRectangle.Y
                & mouseState.Y < rocketRectangle.Y + launcherIcon.Height)
            {
                if (mouseState.LeftButton.Equals(ButtonState.Pressed) 
                    & lastMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    equippedWeapon = EquippedWeapon.RocketLauncher;
                    gameState = GameState.Playing;
                    timer = 1;
                }
            }

            // Grenade
            Rectangle grenadeRectangle = new Rectangle(180, 10, 100, 100);
            spriteBatch.Draw(grenadeIcon, grenadeRectangle, Color.White);

            if (mouseState.X > grenadeRectangle.X
                & mouseState.X < grenadeRectangle.X + grenadeIcon.Width
                & mouseState.Y > grenadeRectangle.Y
                & mouseState.Y < grenadeRectangle.Y + grenadeIcon.Height)
            {
                if (mouseState.LeftButton.Equals(ButtonState.Pressed) 
                    & lastMouseState.LeftButton.Equals(ButtonState.Released))
                {
                    equippedWeapon = EquippedWeapon.Grenade;
                    gameState = GameState.Playing;
                    timer = 1;
                }
            }
        }

        public static void DrawText()
        {
            PlayerData player = players[currentPlayer];
            int currentAngle = (int) MathHelper.ToDegrees(player.Angle);
            spriteBatch.DrawString(font, "Shot angle: " + currentAngle, new Vector2(20, 20), player.Colour);
            spriteBatch.DrawString(font, "Shot power: " + player.Power, new Vector2(20, 45), player.Colour);
            spriteBatch.DrawString(font, "Fuse Timer: " + player.weaponFuse, new Vector2(20, 60), player.Colour);
        }


        //      #################################################
        //      #                                               #
        //      #               GRAPHICS                        #
        //      #                                               #
        //      #################################################


        public static void DrawScenery()
        {
            Rectangle screenRectangle = new Rectangle(0, 0, screenWidth, screenHeight);
            spriteBatch.Draw(backgroundTexture, screenRectangle, Color.White);
            spriteBatch.Draw(foregroundTexture, screenRectangle, Color.White);
        }

        public static void DrawPlayers()
        {
            foreach (PlayerData player in players)
            {
                if (player.IsAlive)
                {
                    int xPos = (int) player.Position.X;
                    int yPos = (int) player.Position.Y;
                    Vector2 bunnyOrigin = new Vector2(22, 22);

                    // Draw some weapons
                    switch (equippedWeapon)
                    {
                        case EquippedWeapon.NoWeapon:
                            spriteBatch.Draw(noWeaponTexture, new Vector2(xPos + 20, yPos - 20), null,
                                player.Colour, player.Angle, bunnyOrigin,
                                playerScaling, SpriteEffects.None, 1);
                            break;
                        case EquippedWeapon.Grenade:
                            spriteBatch.Draw(grenadeTexture, new Vector2(xPos + 20, yPos - 20), null, player.Colour,
                                player.Angle, bunnyOrigin,
                                playerScaling, SpriteEffects.None, 1);
                            break;
                        case EquippedWeapon.RocketLauncher:
                            spriteBatch.Draw(launcherTexture, new Vector2(xPos + 20, yPos - 20), null,
                                player.Colour, player.Angle, bunnyOrigin,
                                playerScaling, SpriteEffects.None, 1);
                            break;
                    }
                    //  Draw some bunnies
                    spriteBatch.Draw(bunnyTexture, player.Position, null, player.Colour, 0,
                        new Vector2(0, bunnyTexture.Height),
                        playerScaling, SpriteEffects.None, 0);
                }
            }
        }


        public static void DrawSmoke()
        {
            foreach (Vector2 smokePos in smokeList)
            {
                spriteBatch.Draw(smokeTexture, smokePos, null, Color.White, 0, new Vector2(40, 35), 0.2f,
                    SpriteEffects.None, 1);
            }
        }

        public static void DrawExplosion()
        {
            for (int i = 0; i < particleList.Count; i++)
            {
                ParticleData particle = particleList[i];
                spriteBatch.Draw(explosionTexture, particle.Position, null, particle.ModColour, i,
                    new Vector2(256, 256), particle.Scaling,
                    SpriteEffects.None, 1);
            }
        }



        public static void UpdateParticles(GameTime gameTime)
        {
            float now = (float) gameTime.TotalGameTime.TotalMilliseconds;
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
                    particle.Position = 0.5f * particle.Accelaration * relAge * relAge +
                                        particle.Direction * relAge + particle.OrginalPosition;

                    float invAge = 1.0f - relAge;
                    particle.ModColour = new Color(new Vector4(invAge, invAge, invAge, invAge));

                    Vector2 positionFromCenter = particle.Position - particle.OrginalPosition;
                    float distance = positionFromCenter.Length();
                    particle.Scaling = (50.0f + distance) / 200.0f;

                    particleList[i] = particle;
                }
            }
        }

        public static void AddExplosion(Vector2 explosionPos, int numberOfParticles, float size, float maxAge,
            GameTime gameTime)
        {
            for (int i = 0; i < numberOfParticles; i++)
            {
                AddExplosionParticle(explosionPos, size, maxAge, gameTime);
            }

            float rotation = (float) randomiser.Next(10);
            Matrix mat = Matrix.CreateTranslation(-explosionTexture.Width / 2, -explosionTexture.Height / 2, 0) *
                         Matrix.CreateRotationZ(rotation)
                         * Matrix.CreateScale(size / (float) explosionTexture.Width * 2.0f) *
                         Matrix.CreateTranslation(explosionPos.X, explosionPos.Y, 0);
            AddCrater(explosionColourArray, mat);

            for (int i = 0; i < players.Length; i++)
            {
                players[i].Position.Y = terrainContour[(int) players[i].Position.X];
            }

            FlattenTerrainBelowPlayers();
            CreateForeground();
        }

        public static void AddExplosionParticle(Vector2 explosionPos, float explosionSize, float maxAge,
            GameTime gameTime)
        {
            ParticleData particle = new ParticleData();

            particle.OrginalPosition = explosionPos;
            particle.Position = particle.OrginalPosition;

            particle.BirthTime = (float) gameTime.TotalGameTime.TotalMilliseconds;
            particle.MaxAge = maxAge;
            particle.Scaling = 0.25f;
            particle.ModColour = Color.White;

            float particleDistance = (float) randomiser.NextDouble() * explosionSize;
            Vector2 displacement = new Vector2(particleDistance, 0);
            float angle = MathHelper.ToRadians(randomiser.Next(360));
            displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(angle));

            particle.Direction = displacement * 2.0f;
            particle.Accelaration = -particle.Direction;

            particleList.Add(particle);
        }
    }
}
