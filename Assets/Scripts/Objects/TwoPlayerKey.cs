using UnityEngine;
public class TwoPlayerKey : MonoBehaviour
{
    private Player _p1;
    private Player _p2;

    public TwoPlayerKey(Player p1 = null, Player p2 = null)
    {
        if (p1 is null)
            return;

        if (p2 is null)
            return;

        _p1 = p1;
        _p2 = p2;
    }

    public TwoPlayerKey SetPlayers(Player p1 , Player p2 )
    {
        if (p1 is null)
            return null;

        if (p2 is null)
            return null;

        _p1 = p1;
        _p2 = p2;
        return this;
    }

    public bool IsBothPlayersAccepted()
    {
        if (_p1.DrawMiddle && _p2.DrawMiddle)
            return true;
        return false;
    }

    public void SetTrue(Player player)
    {
        player.DrawMiddle = true;
    }

    public void ResetKeys()
    {
        _p1.DrawMiddle = false;
        _p2.DrawMiddle = false;
    }

}
