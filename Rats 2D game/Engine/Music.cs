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
                        musicPlaying = false;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(ukulele);
                        musicPlaying = true;
                    }
                    break;
                    case GameState.Playing:
                    {
                        musicPlaying = false;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(titleTheme);
                        musicPlaying = true;
                    }
                    break;
                }
            }
        }
    }
}
