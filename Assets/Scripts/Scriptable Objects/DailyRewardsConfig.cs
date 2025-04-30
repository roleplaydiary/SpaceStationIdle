using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DailyRewardsConfig", menuName = "Game/Daily Rewards Config")]
public class DailyRewardsConfig : ScriptableObject
{
    [System.Serializable]
    public struct Reward
    {
        public enum RewardType
        {
            Credits,
            Resource
        }

        public RewardType type;
        public int creditAmount;
        public Resources resourceReward;
    }

    public List<Reward> rewards;
}