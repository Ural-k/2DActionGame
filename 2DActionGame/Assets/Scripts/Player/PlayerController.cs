using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterControl
{
    [SerializeField] private float m_moveSpeed = 5f;
    [SerializeField] private float m_jumpForce = 10f;

    private Rigidbody2D m_rigidbody2D;
    private PlayerInput m_playerInput;
    private InputAction m_moveAction;


    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_playerInput = GetComponent<PlayerInput>();
        m_moveAction = m_playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        m_moveAction.performed += OnMove;
        m_moveAction.canceled += OnMoveCanceled;
    }

    private void OnDisable()
    {
        m_moveAction.performed -= OnMove;
        m_moveAction.canceled -= OnMoveCanceled;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        Move(moveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        m_rigidbody2D.linearVelocityX = 0;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public override void Move(Vector2 direction)
    {
        m_rigidbody2D.linearVelocityX = direction.x * m_moveSpeed;
    }

    public override void Jump()
    {
        // Implement player jump logic here
    }

    public override void Attack()
    {
        // Implement player attack logic here
    }

}
