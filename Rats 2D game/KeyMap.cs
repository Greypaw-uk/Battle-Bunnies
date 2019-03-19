using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Rats_2D_game
{
    class KeyMap
    {
        Variables v = new Variables();

        public void ProcessKeyboard()
        {
            KeyboardState keybState = Keyboard.GetState();
            if (keybState.IsKeyDown(Keys.Left))
                v.Players[v.CurrentPlayer].Angle -= 0.01f;
            if (keybState.IsKeyDown(Keys.Right))
                v.Players[v.CurrentPlayer].Angle += 0.01f;
            
            if (v.Players[v.CurrentPlayer].Angle > MathHelper.PiOver2)
                v.Players[v.CurrentPlayer].Angle = -MathHelper.PiOver2;


            if (v.Players[v.CurrentPlayer].Angle < -MathHelper.PiOver2)
                v.Players[v.CurrentPlayer].Angle = MathHelper.PiOver2;
            

            if (keybState.IsKeyDown(Keys.Down))
                v.Players[v.CurrentPlayer].Power -= 1;
            if (keybState.IsKeyDown(Keys.Up))
                v.Players[v.CurrentPlayer].Power += 1;
            if (keybState.IsKeyDown(Keys.PageDown))
                v.Players[v.CurrentPlayer].Power -= 20;
            if (keybState.IsKeyDown(Keys.PageUp))
                v.Players[v.CurrentPlayer].Power += 20;

            if (v.Players[v.CurrentPlayer].Power > 1000)
                v.Players[v.CurrentPlayer].Power = 1000;
            if (v.Players[v.CurrentPlayer].Power < 0)
                v.Players[v.CurrentPlayer].Power = 0;

            if (keybState.IsKeyDown(Keys.Enter) || keybState.IsKeyDown(Keys.Space))
            {
                v.RocketFlying = true;
                v.Launch.Play();

                v.RocketPosition = v.Players[v.CurrentPlayer].Position;
                v.RocketPosition.X += 20;
                v.RocketPosition.Y -= 10;
                v.RocketAngle = v.Players[v.CurrentPlayer].Angle;
                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(v.RocketAngle);
                v.RocketDirection = Vector2.Transform(up, rotMatrix);
                v.RocketDirection *= v.Players[v.CurrentPlayer].Power / 50.0f;
            }
        }
    }
}
