using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AgentStatsLogger : MonoBehaviour
{
    private AgentController agent;
    private int episodeCount = 0;
    private float episodeStartTime;
    private float totalReward = 0f;
    private string logFilePath;

    void Start()
    {
        if (agent == null)
        {
            agent = GetComponent<AgentController>();
        }

        episodeStartTime = Time.time;
        logFilePath = Application.persistentDataPath + "/AgentStatsLog.csv";

        if (!File.Exists(logFilePath))
        {
            File.WriteAllText(logFilePath, "Episode, Timestamp, Duration, TotalReward, TargetsCollected, RoomVisited, EpisodeEndReason");
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

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logLine = $"{episodeCount}, {timestamp}, {duration:F2}, {totalReward:F2}, {collected}, {agent.GetVisitedRoomCount()}, {reason}";
        File.AppendAllText(logFilePath, logLine + "\n");
        File.SetLastWriteTime(logFilePath, DateTime.Now);


        episodeCount++;
    }
}
