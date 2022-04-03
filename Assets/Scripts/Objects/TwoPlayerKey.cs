using UnityEngine;

namespace Speed.Objects
{
    public class TwoPlayerKey : MonoBehaviour
    {
        private Player player1;
        private Player player2;

        public TwoPlayerKey(Player player1 = null, Player player2 = null)
        {
            if (player1 is null)
                return;

            if (player2 is null)
                return;

            this.player1 = player1;
            this.player2 = player2;
        }

        public void SetPlayers(Player p1, Player p2)
        {
            if (p1 is null) return;

            if (p2 is null) return;

            player1 = p1;
            player2 = p2;
        }

        public bool IsBothPlayersAccepted()
        {
            return player1.pressedDrawButton && player2.pressedDrawButton;
        }

        public void SetTrue(Player player)
        {
            player.pressedDrawButton = true;
        }

        public void ResetKeys()
        {
            player1.pressedDrawButton = false;
            player2.pressedDrawButton = false;
        }
    }
}
