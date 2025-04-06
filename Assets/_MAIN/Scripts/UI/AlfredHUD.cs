using UnityEngine;
using TMPro;

public class AlfredHUD : MonoBehaviour
{
    public TextMeshProUGUI statsText;
    public AgentController agent;
   

    void Start()
    {
        if (agent == null || statsText == null)
        {
            Debug.LogWarning("AlfredHUD: Agent or statsText is not assigned!");
            return;
        }

        // Initially show statsText
        statsText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (agent == null || statsText == null) return;

        // Hide stats if feedback text is being shown (optional)
        if (agent.feedbackText != null && agent.feedbackText.gameObject.activeSelf)
        {
            statsText.gameObject.SetActive(false);
            return;
        }
        else
        {
            statsText.gameObject.SetActive(true);
        }

        // Update stats
        float timeLeft = Mathf.Max(0f, agent.GetTimeLeft());
        int currentEpisode = agent.GetEpisodeCount();
        float reward = agent.GetCumulativeReward(); // built-in
        float totalReward = agent.GetLifetimeReward();
        int visitedRooms = agent.GetVisitedRoomCount();
        string roomName = agent.GetCurrentRoomName();
        float timeInRoom = agent.GetCurrentRoomStayTime();
        int successCount = agent.GetSuccessCount();

        statsText.text = $" |__| Alfred Stats\n" +
                         $"Episode: {currentEpisode}\n" +
                         $"Time Left: {timeLeft:F1}s\n" +
                         $"Reward (This Ep): {reward:F2}\n" +
                         $"Total Reward : {totalReward:F2}\n" +
                         $"Rooms Visited : {visitedRooms}/{agent.TotalRoomCount}\n" +
                         $"Current Room: {roomName}\n" +
                         $"Time in Room: {timeInRoom:F1}s\n" +
                         $"Tasks Done: {successCount}";

    }
}
