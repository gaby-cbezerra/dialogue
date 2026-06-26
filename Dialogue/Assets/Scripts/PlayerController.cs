using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private GameInput gameInput;

    [SerializeField] private bool isPlayer1;

    [SerializeField] private Rigidbody enemyRb;

    [SerializeField] private float pushForce = 500f;

    [SerializeField] private float maxPushDistance = 5f;

    [SerializeField] private float forceCooldown = 5f;

    private float currentCooldown = 5f;

    private bool canPush = true;

    [Tooltip("Force multiplier applied when moving (higher = stronger acceleration)")] [SerializeField]
    private float speed = 300f;

    [Tooltip("Maximum horizontal speed (units/sec)")] [SerializeField]
    private float maxSpeed = 6f;

    [Tooltip("Torque strength applied to make the ball rotate (higher = faster spin)")] [SerializeField]
    private float torqueStrength = 10f;

    [Tooltip("Rigidbody to apply forces to. If left empty, GetComponent<Rigidbody>() will be used.")] [SerializeField]
    private Rigidbody rb;

    [Header("Coins")]

    [SerializeField]
    private int currentCoins = 0;


    [SerializeField]
    private float coinPushBonus = 50f;


    [SerializeField]
    private float coinWeightBonus = 0.5f;

    // Current movement input (-1..1 for X and Y)
    private Vector2 moveInput = Vector2.zero;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        gameInput = new GameInput();

        currentCooldown = forceCooldown;

        BallData selectedData;

        if (isPlayer1)
            selectedData = SelectedBalls.Player1Ball;
        else
            selectedData = SelectedBalls.Player2Ball;


        ApplyBallData(selectedData);
    }

    private void OnEnable()
    {
        gameInput.Gameplay.Enable();

        if (isPlayer1)
        {
            gameInput.Gameplay.MoveP1.performed += OnMovePerformed;
            gameInput.Gameplay.MoveP1.canceled += OnMoveCanceled;

            gameInput.Gameplay.InteractP1.performed += OnPush;
        }
        else
        {
            gameInput.Gameplay.MoveP2.performed += OnMovePerformed;
            gameInput.Gameplay.MoveP2.canceled += OnMoveCanceled;

            gameInput.Gameplay.InteractP2.performed += OnPush;
        }
    }


    private void OnDisable()
    {
        if (isPlayer1)
        {
            gameInput.Gameplay.MoveP1.performed -= OnMovePerformed;
            gameInput.Gameplay.MoveP1.canceled -= OnMoveCanceled;

            gameInput.Gameplay.InteractP1.performed -= OnPush;
        }
        else
        {
            gameInput.Gameplay.MoveP2.performed -= OnMovePerformed;
            gameInput.Gameplay.MoveP2.canceled -= OnMoveCanceled;

            gameInput.Gameplay.InteractP2.performed -= OnPush;
        }

        gameInput.Gameplay.Disable();
    }

    private void OnMovePerformed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    private void OnMoveCanceled(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
    }

    private void Update()
    {
        if (!canPush)
        {
            currentCooldown += Time.deltaTime;

            float progress = currentCooldown / forceCooldown;

            if (ForceBarUI.Instance != null)
            {
                if(isPlayer1)
                {
                    ForceBarUI.Instance.UpdatePlayer1Bar(progress);
                }
                else
                {
                    ForceBarUI.Instance.UpdatePlayer2Bar(progress);
                }
            }


            if (currentCooldown >= forceCooldown)
            {
                canPush = true;
                currentCooldown = forceCooldown;
            }
        }
    }

    private void FixedUpdate()
    {
        Debug.Log("Speed atual: " + speed);
        if (rb == null) return;

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        rb.AddForce(move * speed, ForceMode.Force);

        Vector3 torque =
            new Vector3(move.z, 0f, -move.x) *
            torqueStrength;

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


        // ganha mais resistência
        rb.mass += coinWeightBonus;


        // ganha mais força de ataque
        pushForce += coinPushBonus;


        // fica mais lento
        speed -= 10f;


        if(speed < 50f)
            speed = 50f;



        if(isPlayer1)
        {
            PlayerObserverManager.UpdatePlayer1Coins(currentCoins);
        }
        else
        {
            PlayerObserverManager.UpdatePlayer2Coins(currentCoins);
        }

        Debug.Log(
            "Moedas: " + currentCoins
        );
    }

    private void ApplyBallData(BallData data)
    {
        if (data == null)
        {
            Debug.Log("Sem BallData em " + gameObject.name);
            return;
        }


        Debug.Log(gameObject.name + " recebeu " + data.ballName);


        // velocidade escolhida
        speed = data.speed;


        // tamanho da bola
        transform.localScale = Vector3.one * data.size;


        // peso
        if (rb != null)
            rb.mass = data.weight;


        // cor/material
        MeshRenderer renderer = GetComponent<MeshRenderer>();

        if (renderer != null && data.material != null)
        {
            renderer.material = Instantiate(data.material);
        }
    }

    private void OnPush(InputAction.CallbackContext ctx)
    {
        if (!canPush)
            return;


        if (enemyRb == null)
            return;


        Vector3 direction = enemyRb.position - rb.position;


        float distance = direction.magnitude;


        direction.Normalize();


        float power =
            Mathf.Clamp01(1 - distance / maxPushDistance);


        enemyRb.AddForce(
            direction * pushForce * power,
            ForceMode.Impulse
        );


        canPush = false;

        currentCooldown = 0;


        if (ForceBarUI.Instance != null)
            if(isPlayer1)
            {
                ForceBarUI.Instance.UpdatePlayer1Bar(0);
            }
            else
            {
                ForceBarUI.Instance.UpdatePlayer2Bar(0);
            }
    }
    public bool IsPlayer1()
    {
        return isPlayer1;
    }
}