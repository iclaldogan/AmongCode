using UnityEngine;

[RequireComponent(typeof(AgentController))]
public class RewardSystem : MonoBehaviour
{
    private AgentController agent;

    [Header("Reward Values")]
    public float reward_TargetCollected = 7f;
    public float reward_AllTargets = 12f;
    public float reward_FirstRoomVisit = 8f;
    public float reward_AllRoomsVisited = 14f;
    public float reward_MissionComplete = 20f;

    [Header("Penalty Values")]
    public float penalty_Timeout = -15f;
    public float penalty_WallHit = -7f;

    private void Awake()
    {
        agent = GetComponent<AgentController>();
    }

    public void ResetRewards()
    {
        // Optional: clear counters if needed
    }

    public void CollectTarget(GameObject target)
    {
        Destroy(target);
        agent.targetManager.RemoveTargetFromList(target);

        ApplyReward(reward_TargetCollected);
        agent.eventVisualizer?.FlashColor(Color.green);

        if (agent.targetManager.AllTargetsCollected())
        {
            ApplyReward(reward_AllTargets);
            agent.eventVisualizer?.FlashColor(Color.cyan);
        }
    }

    public void FirstTimeRoomVisit(string roomName)
    {
        ApplyReward(reward_FirstRoomVisit);
        agent.eventVisualizer?.FlashColor(Color.yellow);
    }

    public void AllRoomsVisited()
    {
        ApplyReward(reward_AllRoomsVisited);
    }

    public void MissionComplete()
    {
        ApplyReward(reward_MissionComplete);
    }

    public void HitWall()
    {
        ApplyReward(penalty_WallHit);
    }

    public void ApplyTimeoutPenalty()
    {
        ApplyReward(penalty_Timeout);
    }

    public void ApplyReward(float value)
    {
        agent.AddRewardToAgent(value); // unified method in AgentController
    }
}
