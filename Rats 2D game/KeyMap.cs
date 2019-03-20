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
                v.players[v.currentPlayer].Angle -= 0.01f;
            if (keybState.IsKeyDown(Keys.Right))
                v.players[v.currentPlayer].Angle += 0.01f;

            if (v.players[v.currentPlayer].Angle > MathHelper.PiOver2)
                v.players[v.currentPlayer].Angle = -MathHelper.PiOver2;
            if (v.players[v.currentPlayer].Angle < -MathHelper.PiOver2)
                v.players[v.currentPlayer].Angle = MathHelper.PiOver2;

            if (keybState.IsKeyDown(Keys.Down))
                v.players[v.currentPlayer].Power -= 1;
            if (keybState.IsKeyDown(Keys.Up))
                v.players[v.currentPlayer].Power += 1;
            if (keybState.IsKeyDown(Keys.PageDown))
                v.players[v.currentPlayer].Power -= 20;
            if (keybState.IsKeyDown(Keys.PageUp))
                v.players[v.currentPlayer].Power += 20;

            if (v.players[v.currentPlayer].Power > 1000)
                v.players[v.currentPlayer].Power = 1000;
            if (v.players[v.currentPlayer].Power < 0)
                v.players[v.currentPlayer].Power = 0;

            if (keybState.IsKeyDown(Keys.Enter) || keybState.IsKeyDown(Keys.Space))
            {
                v.rocketFlying = true;
                v.launch.Play();

                v.rocketPosition = v.players[v.currentPlayer].Position;
                v.rocketPosition.X += 20;
                v.rocketPosition.Y -= 10;
                v.rocketAngle = v.players[v.currentPlayer].Angle;
                Vector2 up = new Vector2(0, -1);
                Matrix rotMatrix = Matrix.CreateRotationZ(v.rocketAngle);
                v.rocketDirection = Vector2.Transform(up, rotMatrix);
                v.rocketDirection *= v.players[v.currentPlayer].Power / 50.0f;
            }
        }
    }
}
