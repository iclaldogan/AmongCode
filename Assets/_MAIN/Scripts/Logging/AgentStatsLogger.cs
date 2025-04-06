using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class AgentStatsLogger : MonoBehaviour
{
    private AgentController agent;
    private int episodeCount = 0;
    private float episodeStartTime;
    private float totalReward = 0f;
    private string logFilePath;

    void Start()
    {
        if(agent == null)
        {
            agent = GetComponent<AgentController>();
        }

        episodeStartTime = Time.time;
        logFilePath = Application.persistentDataPath + "/AgentStatsLog.csv";

        if (!File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, "Episode, Duration, TotalReward, TargetsCollected, RoomVisited, EpisodeEndReason\n");
        }
    }

    public void StartEpisode()
    {
        episodeStartTime = Time.time;
        totalReward = 0f;
    }

    public void AddReward(float reward)
    {
        totalReward += reward;
    }

    public void EndEpisode(string reason)
    {
        float duration = Time.time - episodeStartTime;

        int collected = agent.TargetCount - agent.SpawnedTargetCount;
        bool success = (collected == agent.TargetCount && agent.GetVisitedRoomCount() == agent.TotalRoomCount);


        string logLine = $"{episodeCount}, {duration:F2}, {totalReward:F2}, {collected}, {agent.GetVisitedRoomCount()}, {reason}";
        File.AppendAllText(logFilePath, logLine + "\n");

        episodeCount++;
    }
}
