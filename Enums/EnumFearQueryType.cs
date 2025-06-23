using LethalBots.AI;
using LethalBots.Managers;

namespace LethalBots.Enums
{
    /// <summary>
    /// Enum for the <c>EnumFearQueryType</c> defines the fear query for use in <see cref="LethalBotThreat"/> and <seealso cref="LethalBotManager.GetFearRangeForEnemy"/>
    /// </summary>
    public enum EnumFearQueryType
    {
        BotPanic,
        PlayerTeleport,
        PathfindingAvoid
    }
}
