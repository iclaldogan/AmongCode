using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RewardSystem))]
public class RoomTracker : MonoBehaviour
{
    [Header("Room Settings")]
    public int totalRoomCount = 10;
    public List<Renderer> roomVisuals = new List<Renderer>();

    [Header("Overstay Settings")]
    public float roomOverstayThreshold = 30f;
    public float roomOverstayPenalty = -10f;
    public float corridorOverstayThreshold = 10f;
    public float corridorOverstayPenalty = -5f;

    private AgentController agent;
    private RewardSystem rewardSystem;

    private string currentRoom = "";
    private string currentTag = "";
    private Dictionary<string, int> visitCounts = new Dictionary<string, int>();
    private Dictionary<string, float> entryTimes = new Dictionary<string, float>();
    private Dictionary<string, float> roomDurations = new Dictionary<string, float>();
    private bool allRoomsVisited = false;

    public int TotalRoomCount => totalRoomCount;

    private void Awake()
    {
        agent = GetComponent<AgentController>();
        rewardSystem = GetComponent<RewardSystem>();
    }

    public void ResetTracker()
    {
        visitCounts.Clear();
        entryTimes.Clear();
        roomDurations.Clear();
        currentRoom = "";
        currentTag = "";
        allRoomsVisited = false;
    }

    public void HandleRoomEntry(Collider area)
    {
        string areaName = area.gameObject.name;
        string areaTag = area.tag;

        if (currentRoom == areaName) return;

        if (!string.IsNullOrEmpty(currentRoom) && entryTimes.ContainsKey(currentRoom))
        {
            float timeSpent = Time.time - entryTimes[currentRoom];
            if (!roomDurations.ContainsKey(currentRoom))
                roomDurations[currentRoom] = 0f;
            roomDurations[currentRoom] += timeSpent;
        }

        currentRoom = areaName;
        currentTag = areaTag;
        entryTimes[areaName] = Time.time;

        if (!visitCounts.ContainsKey(areaName))
        {
            visitCounts[areaName] = 1;

            if (areaTag == "room")
            {
                rewardSystem.FirstTimeRoomVisit(areaName);
                HighlightRoom(areaName, Color.yellow);
            }
            else if (areaTag == "corridor")
            {
                rewardSystem.ApplyReward(4f);
                agent.eventVisualizer?.FlashColor(Color.cyan);
            }

            Debug.Log($"Entered new {areaTag}: {areaName}");
        }
        else
        {
            visitCounts[areaName]++;
            int visitCount = visitCounts[areaName];
            float penalty = -2f * (visitCount - 3);
            rewardSystem.ApplyReward(penalty);

            Debug.Log($"Revisited {areaName} {visitCount}x → Penalty: {penalty}");

            if (visitCount > 7)
            {
                agent.logger.EndEpisode("RoomVisitLimitExceeded");
                agent.EndEpisode();
            }
        }

        if (!allRoomsVisited && GetVisitedRoomCount() >= totalRoomCount)
        {
            allRoomsVisited = true;
            rewardSystem.AllRoomsVisited();
            agent.logger.EndEpisode("ExploredAllRooms");
            Debug.Log("✅ All rooms visited!");
        }
    }

    public void CheckRoomOverstay()
    {
        if (string.IsNullOrEmpty(currentRoom) || !entryTimes.ContainsKey(currentRoom))
            return;

        float stayTime = Time.time - entryTimes[currentRoom];
        float threshold = currentTag == "corridor" ? corridorOverstayThreshold : roomOverstayThreshold;
        float penalty = currentTag == "corridor" ? corridorOverstayPenalty : roomOverstayPenalty;
        Color color = currentTag == "corridor" ? Color.cyan : Color.grey;

        if (stayTime > threshold)
        {
            rewardSystem.ApplyReward(penalty);
            agent.eventVisualizer?.FlashColor(color);
            Debug.Log($"🚨 Overstay in {currentRoom} → {penalty}");
            entryTimes[currentRoom] = Time.time;
        }
    }

    private void HighlightRoom(string name, Color color)
    {
        foreach (Renderer rend in roomVisuals)
        {
            if (rend.gameObject.name == name)
            {
                rend.material.color = color;
                return;
            }
        }
    }

    public string GetCurrentRoomName() => currentRoom;
    public float GetCurrentRoomStayTime()
    {
        if (!string.IsNullOrEmpty(currentRoom) && entryTimes.ContainsKey(currentRoom))
        {
            return Time.time - entryTimes[currentRoom];
        }
        return 0f;
    }

    public int GetVisitedRoomCount() => visitCounts.Keys.Count;
    public bool AllRoomsVisited() => allRoomsVisited;
    public Dictionary<string, float> GetRoomDurations() => roomDurations;
}
