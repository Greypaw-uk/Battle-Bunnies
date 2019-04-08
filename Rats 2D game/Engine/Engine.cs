using Microsoft.Xna.Framework;

using static BattleBunnies.Global;
using static BattleBunnies.Players;

using static BattleBunnies.Weapons.RocketLauncher;
using static BattleBunnies.Weapons.Grenade;

namespace BattleBunnies

{
    public enum EquippedWeapon
    {
        NoWeapon,
        RocketLauncher,
        Grenade
    }
    
    static class Engine
    {
        public static EquippedWeapon equippedWeapon;

        //  Weapon Variables
        public static Vector2 projectilePosition;
        public static Vector2 projectileDirection;
        public static float projectileAngle;
        public static float projectileScaling = 0.1f;

        //  THROTTLE SHOTS
        public static bool canShoot;


        public static void FireWeapon()
        {
            if (canShoot)
            {
                canShoot = false;
                switch (equippedWeapon)
                {
                    case EquippedWeapon.NoWeapon:

                        break;

                    case EquippedWeapon.RocketLauncher:
                    {
                        rocketFlying = true;
                        FireRocket();
                    }
                        break;

                    case EquippedWeapon.Grenade:
                        {
                            grenadeThrown = true;
                            GrenadeOut();
                        }
                        break;
                }
            }
        }

        public static bool CheckOutOfScreen()
        {
            bool projectileOutOfScreen = projectilePosition.Y > screenHeight;
            projectileOutOfScreen |= projectilePosition.X < 0;
            projectileOutOfScreen |= projectilePosition.X > screenWidth;

            return projectileOutOfScreen;
        }

        public static void NextPlayer()
        {
            currentPlayer = currentPlayer + 1;
            currentPlayer = currentPlayer % numberOfPlayers;
            while (!players[currentPlayer].IsAlive)
            {
                currentPlayer = ++currentPlayer % numberOfPlayers;
            }
            players[currentPlayer].weaponFuse = 5.0f;
            players[currentPlayer].Angle = 0;
            players[currentPlayer].Power = 0;
            equippedWeapon = EquippedWeapon.NoWeapon;
            canShoot = false;
        }
    }
}
