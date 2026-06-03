using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameInput gameInput;

    [Tooltip("Force multiplier applied when moving (higher = stronger acceleration)")]
    [SerializeField] private float speed = 300f;

    [Tooltip("Maximum horizontal speed (units/sec)")]
    [SerializeField] private float maxSpeed = 6f;

    [Tooltip("Torque strength applied to make the ball rotate (higher = faster spin)")]
    [SerializeField] private float torqueStrength = 10f;

    [Tooltip("Rigidbody to apply forces to. If left empty, GetComponent<Rigidbody>() will be used.")]
    [SerializeField] private Rigidbody rb;

    [Header("Coins")]
    [SerializeField] private int currentCoins = 0;

    // Current movement input (-1..1 for X and Y)
    private Vector2 moveInput = Vector2.zero;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        gameInput = new GameInput();
    }

    private void OnEnable()
    {
        gameInput.Gameplay.Enable();

        gameInput.Gameplay.Move.performed += OnMovePerformed;
        gameInput.Gameplay.Move.canceled += OnMovePerformed;
    }

    private void OnDisable()
    {
        gameInput.Gameplay.Move.performed -= OnMovePerformed;
        gameInput.Gameplay.Move.canceled -= OnMovePerformed;

        gameInput.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        rb.AddForce(move * speed * Time.fixedDeltaTime, ForceMode.Force);

        Vector3 torque =
            new Vector3(move.z, 0f, -move.x) *
            torqueStrength *
            Time.fixedDeltaTime;

        rb.AddTorque(torque, ForceMode.Force);

        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);

        float horizontalSpeed = horizontal.magnitude;

        if (horizontalSpeed > maxSpeed)
        {
            Vector3 clamped = horizontal.normalized * maxSpeed;

            rb.linearVelocity =
                new Vector3(
                    clamped.x,
                    vel.y,
                    clamped.z
                );
        }
    }

    public void SetMove(Vector2 input)
    {
        moveInput = input;
    }

    public void AddCoin(int amount)
    {
        currentCoins += amount;

        PlayerObserverManager.UpdateCoins(currentCoins);

        Debug.Log("Moedas: " + currentCoins);
    }
}