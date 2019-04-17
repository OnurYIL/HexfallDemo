
namespace HexFallDemo
{
    public enum SwipeDirection
    {
        left,
        right,
        up,
        down
    }
    public enum SwapDirection
    {
        clockwise = 1,
        antiClockwise = -1
    }
    public enum GameState
    {
        rotating,
        matching,
        selecting,
        ending,
        menu,
        idle,
        init,
        matchingComplete
    }
}