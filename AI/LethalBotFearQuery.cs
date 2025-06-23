using GameNetcodeStuff;
using LethalBots.Enums;
using UnityEngine;
using LethalBots.Managers;

namespace LethalBots.AI
{
    /// <summary>
    /// Class for the <c>LethalBotFearQuery</c> defines the fear query for use in <see cref="LethalBotThreat>"/> and <seealso cref="LethalBotManager.GetFearRangeForEnemy"/>
    /// </summary>
    public class LethalBotFearQuery
    {
        public LethalBotFearQuery(EnemyAI? bot, EnemyAI enemyAI, PlayerControllerB? playerToCheck, EnumFearQueryType queryType) 
            : this(bot, enemyAI, queryType)
        {
            PlayerToCheck = playerToCheck;
        }
        public LethalBotFearQuery(EnemyAI? bot, EnemyAI enemyAI, EnumFearQueryType queryType)
        {
            Bot = bot;
            EnemyAI = enemyAI;
            QueryType = queryType;
        }
        public EnemyAI? Bot { get; private set; }
        public EnemyAI EnemyAI { get; private set; }
        public PlayerControllerB? PlayerToCheck { get; private set; }
        public EnumFearQueryType QueryType { get; private set; }
    }
}
