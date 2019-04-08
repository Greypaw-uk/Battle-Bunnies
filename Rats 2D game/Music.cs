using Microsoft.Xna.Framework.Media;

using static BattleBunnies.Global;

namespace BattleBunnies
{
    public static class Music
    {
        public static bool musicPlaying;

        public static void PlayMusic()
        {
            if (musicPlaying)
            {
                switch (gameState)
                {
                    case GameState.TitleScreen:
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Play(ukulele);
                    }
                        break;
                    case GameState.Playing:
                    {
                        MediaPlayer.Stop();
                        MediaPlayer.Play(titleTheme);
                    }
                        break;
                }
            }
        }
    }
}
