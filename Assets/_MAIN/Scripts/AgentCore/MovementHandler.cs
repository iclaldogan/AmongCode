using UnityEngine;
using Unity.MLAgents.Actuators;

[RequireComponent(typeof(Rigidbody))]
public class MovementHandler : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Move(ActionBuffers actions)
    {
        float rotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];

        rb.MovePosition(transform.position + transform.forward * moveForward * moveSpeed * Time.deltaTime);
        transform.Rotate(0f, rotate * moveSpeed, 0f, Space.Self);

        Debug.Log($"➡️ MoveForward: {moveForward}, Rotate: {rotate}");
    }

    public void ManualMove(ActionSegment<float> actions)
    {
        actions[0] = Input.GetAxisRaw("Horizontal");
        actions[1] = Input.GetAxisRaw("Vertical");
    }

    public void ResetPhysics()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
