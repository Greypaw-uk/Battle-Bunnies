using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

using static BattleBunnies.Global;
using static BattleBunnies.Players;
using static BattleBunnies.Engine;
using static BattleBunnies.Music;

using static BattleBunnies.Weapons.RocketLauncher;
using static BattleBunnies.Weapons.Grenade;

namespace BattleBunnies
{
    static class Keymapping
    {

        public static MouseState lastMouseState;
        public static MouseState mouseState;

        public static KeyboardState lastKeyboardState;
        public static KeyboardState keyboardState;

        public static void ProcessMouse()
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
                    // MOUSE AIMING
                    Vector2 dPos = players[currentPlayer].Position - mousePointer;
                    players[currentPlayer].Angle = -(float)Math.Atan2(dPos.X, dPos.Y);

                    // WEAPON MENU ON RIGHT CLICK
                    if (mouseState.RightButton.Equals(ButtonState.Released) &&
                        lastMouseState.RightButton.Equals(ButtonState.Pressed))
                    {
                        gameState = GameState.WeaponMenu;
                    }

                    // SHOOTING
                    if (mouseState.LeftButton.Equals(ButtonState.Pressed)
                        && equippedWeapon != EquippedWeapon.NoWeapon)
                    {
                        players[currentPlayer].Power += 5;
                        if (players[currentPlayer].Power > 500)
                        {
                            players[currentPlayer].Power = 500;
                        }
                        canShoot = true;
                    }

                    if (mouseState.LeftButton.Equals(ButtonState.Released)
                        && lastMouseState.LeftButton.Equals(ButtonState.Pressed)
                        && timer <= 0)
                    {
                        FireWeapon();
                    }
                    break;

                case GameState.WeaponMenu:
                    {
                        if (mouseState.RightButton.Equals(ButtonState.Released)
                                && lastMouseState.RightButton.Equals(ButtonState.Pressed))
                        {
                            gameState = GameState.Playing;
                        }
                        break;
                    }
            }
        }

        public static void ProcessKeyboard()
        {
            switch (gameState)
            {
                case GameState.Playing:

                    if (keyboardState.IsKeyDown(Keys.C))
                    {
                        gameState = GameState.WeaponMenu;
                        canShoot = false;
                    }

                    if (keyboardState.IsKeyDown(Keys.F)
                        && lastKeyboardState.IsKeyUp(Keys.F)
                        && !grenadeThrown)
                    {
                        if (players[currentPlayer].weaponFuse >= 5.0f)
                        {
                            players[currentPlayer].weaponFuse = 1.0f;
                        }
                        else
                        {
                            players[currentPlayer].weaponFuse++;
                        }
                    }

                    if (keyboardState.IsKeyDown(Keys.LeftControl) 
                        && keyboardState.IsKeyDown(Keys.M)
                        && lastKeyboardState.IsKeyUp(Keys.LeftControl)
                        || lastKeyboardState.IsKeyUp(Keys.M))
                    {
                        if (!musicPlaying)
                        {
                            musicPlaying = true;
                        }
                        else
                        {
                            musicPlaying = false;
                        }
                    }
                    break;

                case GameState.WeaponMenu:
                    if (keyboardState.IsKeyDown(Keys.Escape))
                    {
                        equippedWeapon = EquippedWeapon.NoWeapon;
                        gameState = GameState.Playing;
                    }
                    break;
            }
        }
    }
}
