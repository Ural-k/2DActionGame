using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : CharacterControl
{
    private PlayerInput m_playerInput;
    private InputAction m_moveAction;
    private InputAction m_jumpAction;

    protected override void Awake()
    {
        base.Awake();
        m_playerInput = GetComponent<PlayerInput>();
        m_moveAction = m_playerInput.actions["Move"];
        m_jumpAction = m_playerInput.actions["Jump"];

    }

    private void OnEnable()
    {
        m_moveAction.performed += OnMove;
        m_moveAction.canceled += OnMoveCanceled;
        m_jumpAction.performed += OnJump;
    }

    private void OnDisable()
    {
        m_moveAction.performed -= OnMove;
        m_moveAction.canceled -= OnMoveCanceled;
        m_jumpAction.performed -= OnJump;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 moveInput = context.ReadValue<Vector2>();
        Move(moveInput);
    }

    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        m_rigidBody2D.linearVelocityX = 0;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
        {
            Jump();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsGrounded())
        {
            Debug.Log("地面に接地しています");
        }
    }

    public override void Move(Vector2 direction)
    {
        m_rigidBody2D.linearVelocityX = direction.x * m_moveSpeed;
    }

    public override void Jump()
    {
        m_rigidBody2D.AddForceY(m_jumpForce,ForceMode2D.Impulse);
    }

    public override void Attack()
    {
        // Implement player attack logic here
    }
}
