# AmongCode

Alfred is an intelligent reinforcement learning agent designed for a social deduction game inspired by *Among Us*. This modular Unity ML-Agents project is structured for scalability, experimentation, and deep behavior modeling.

---

## Project Goals

- Train Alfred to explore rooms and collect targets.
- Add progressive capabilities like memory, deception detection, and voting logic.
- Simulate social deduction behavior through trust modeling and suspicion accumulation.
- Evaluate RL training performance with logs, stats, and modular UI.

---

## Folder Structure

```
Scripts/
├── AgentCore/         # Core agent logic and reward system
├── MemorySystem/      # Vision-based memory tracking
├── TargetSystem/      # Target (task) spawn and collection logic
├── RoomSystem/        # Room entry detection and exploration logic
├── EventSystem/       # Visual and sound feedback for key events
├── SocialSystem/      # Suspicion, voting, and trust behavior modules
├── Logging/           # CSV logs, debugging tools
├── UI/                # In-game stat HUD and voting interfaces
├── Training/          # Training configs and curriculum setups
├── NavigationSystem/  # Pathfinding and obstacle logic
```

---

## Requirements

- Unity 2021.3+ with ML-Agents installed
- ML-Agents Python package (`mlagents`) if training
- `.onnx` trained model (optional, for inference)
- CSV viewer (for analyzing logs)

---

## Setup Instructions

1. Clone or download the project.
2. Open in Unity and install ML-Agents from Package Manager if needed.
3. Set up your `Behavior Parameters`, `Decision Requester`, and `RewardSystem`.
4. Train using:
   ```bash
   mlagents-learn config.yaml --run-id=alfred_run_1
   ```
5. Observe Alfred's behavior through the in-game HUD or exported CSV logs.

---

## Training Logs

Training data and evaluation logs can be found in:
```
Scripts/Logging/AgentStatsLogger.cs
Training/metrics.md
```

---

## Contributions

---
---

## License

MIT License

---
