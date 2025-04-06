using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using TMPro;

[RequireComponent(typeof(MovementHandler))]
[RequireComponent(typeof(AgentStatsLogger))]
public class AgentController : Agent
{
    [Header("UI Feedback")]
    public TextMeshProUGUI feedbackText;

    [Header("Settings")]
    [SerializeField] private int timeForEpisode = 60;

    // Modules
    public MovementHandler movement;
    public AgentStatsLogger logger;
    public RewardSystem rewardSystem;
    public RoomTracker roomTracker;
    public TargetManager targetManager;
    public EventVisualizer eventVisualizer;
    public MemoryModule memory;

    // State
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private float timeLimit;
    private int episodeCount = 0;
    private int successCount = 0;
    private float lifetimeReward = 0f;

    // Accessors (used by HUD)
    public int GetEpisodeCount() => episodeCount;
    public float GetTimeLeft() => timeLimit - Time.time;
    public float GetLifetimeReward() => lifetimeReward;
    public int GetSuccessCount() => successCount;
    public string GetCurrentRoomName() => roomTracker?.GetCurrentRoomName();
    public int GetVisitedRoomCount() => roomTracker?.GetVisitedRoomCount() ?? 0;
    public float GetCurrentRoomStayTime() => roomTracker?.GetCurrentRoomStayTime() ?? 0f;
    public int TargetCount => targetManager?.TargetCount ?? 0;
    public int SpawnedTargetCount => targetManager?.SpawnedTargets.Count ?? 0;
    public int TotalRoomCount => roomTracker?.TotalRoomCount ?? 0;

    protected override void Awake()
    {
        base.Awake(); // Important for ML-Agents

        // Fetch all modular components
        movement = GetComponent<MovementHandler>();
        logger = GetComponent<AgentStatsLogger>();
        rewardSystem = GetComponent<RewardSystem>();
        roomTracker = GetComponent<RoomTracker>();
        //targetManager = GetComponent<TargetManager>();
        eventVisualizer = GetComponent<EventVisualizer>();
        memory = GetComponent<MemoryModule>();
    }

    void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        if (feedbackText != null)
            feedbackText.gameObject.SetActive(false);
    }

    public override void OnEpisodeBegin()
    {
        episodeCount++;
        //successCount = 0;
        //lifetimeReward = 0f;
        timeLimit = Time.time + timeForEpisode;

        transform.position = initialPosition;
        transform.rotation = initialRotation;
        movement.ResetPhysics();

        logger.StartEpisode();
        targetManager?.SpawnTargets();
        roomTracker?.ResetTracker();
        memory?.ClearMemory();
        rewardSystem?.ResetRewards();
        eventVisualizer?.HideFeedback();

        Debug.Log("New episode started.");
    }

    void Update()
    {
        if (Time.time >= timeLimit)
        {
            rewardSystem.ApplyTimeoutPenalty();
            eventVisualizer?.FlashColor(Color.blue);
            logger.EndEpisode("Timeout");
            EndEpisode();
        }

        roomTracker?.CheckRoomOverstay();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        memory?.CollectVisionObservations(sensor, transform);
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        movement.Move(actions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxisRaw("Horizontal"); // Rotate
        continuousActionsOut[1] = Input.GetAxisRaw("Vertical");   // Forward
    }



    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("target"))
        {
            rewardSystem.CollectTarget(other.gameObject);
            CheckMissionCompletion();
        }
        else if (other.CompareTag("wall"))
        {
            rewardSystem.HitWall();
            eventVisualizer?.FlashColor(Color.red);
            logger.EndEpisode("WallHit");
            EndEpisode();
        }
        else if (other.CompareTag("room") || other.CompareTag("corridor"))
        {
            roomTracker?.HandleRoomEntry(other);
            memory?.RecordRoomEntry(other);
            CheckMissionCompletion();
        }
    }

    private void CheckMissionCompletion()
    {
        if (roomTracker.AllRoomsVisited() && targetManager.AllTargetsCollected())
        {
            rewardSystem.MissionComplete();
            eventVisualizer?.ShowMissionComplete();
            logger.EndEpisode("MissionComplete");
            successCount++;
            EndEpisode();
        }
    }

    public void AddRewardToAgent(float reward)
    {
        AddReward(reward);
        lifetimeReward += reward;
        logger.AddReward(reward);
    }
}
