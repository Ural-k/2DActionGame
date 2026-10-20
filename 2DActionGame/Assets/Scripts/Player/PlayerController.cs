using System.Collections;
using UnityEngine;

public enum PlayerMoveState
{
    Moving,
    Evading,
    Recovering
}

public class PlayerController : CharacterControl
{
    [Header("Evade")]
    [SerializeField] private float m_evadeSpeed = 18f;
    [SerializeField] private float m_evadeDuration = 0.18f;
    [SerializeField] private float m_recoveringTime = 0.15f;
    [SerializeField] private float m_evadeCoolDown = 0.5f;
    [SerializeField, Range(0f, 1f)] private float m_evadeEndSpeedRate = 0.6f;

    private Vector2 m_playerDirection = Vector2.right;
    private Vector2 m_currentMoveInput = Vector2.zero;
    private PlayerMoveState m_playerMoveState = PlayerMoveState.Moving;

    protected override void Awake()
    {
        base.Awake();
    }

    [InputActions("Move")]
    public override void Move(Vector2 direction)
    {
        // 回避・硬直中でも、現在押されている入力は記録する
        m_currentMoveInput = direction;

        if (m_playerMoveState != PlayerMoveState.Moving)
        {
            return;
        }

        ApplyMove(direction);
    }

    private void ApplyMove(Vector2 direction)
    {
        if (direction.x != 0f)
        {
            m_playerDirection =
                direction.x > 0f ? Vector2.right : Vector2.left;
        }

        m_rigidBody2D.linearVelocityX = direction.x * m_moveSpeed;
    }

    [InputActionCancel("Move")]
    public void MoveCancel()
    {
        // 回避・硬直中にキーを離した場合も、入力状態は更新する
        m_currentMoveInput = Vector2.zero;

        if (m_playerMoveState != PlayerMoveState.Moving)
        {
            return;
        }

        m_rigidBody2D.linearVelocityX = 0f;
    }

    [InputActions("Jump")]
    public override void Jump()
    {
        if (m_playerMoveState != PlayerMoveState.Moving)
        {
            return;
        }

        if (IsGrounded())
        {
            m_rigidBody2D.AddForceY(m_jumpForce, ForceMode2D.Impulse);
        }
    }

    [InputActions("Evade")]
    private void Evade()
    {
        if (m_playerMoveState != PlayerMoveState.Moving)
        {
            return;
        }
        if(IsInCoolDown("Evade", m_evadeCoolDown))
        {
            Debug.Log("Evade is in cooldown.");
            return;
        }

        StartCoroutine(EvadingCoroutine());
    }

    public override void Attack()
    {
        // 攻撃実装用
    }

    private IEnumerator EvadingCoroutine()
    {
        m_playerMoveState = PlayerMoveState.Evading;

        // 入力中なら入力方向、入力なしなら向きと逆へバックステップ
        float evadeDirection =
            m_currentMoveInput.x != 0f
                ? Mathf.Sign(m_currentMoveInput.x)
                : -m_playerDirection.x;

        // 落下を止める
        float originalGravityScale = m_rigidBody2D.gravityScale;
        m_rigidBody2D.gravityScale = 0f;

        float elapsedTime = 0f;

        while (elapsedTime < m_evadeDuration)
        {
            // 出始めを速く、終わり際だけ少し減速させる
            float normalizedTime = elapsedTime / m_evadeDuration;
            float speed = Mathf.Lerp(
                m_evadeSpeed,
                m_evadeSpeed * m_evadeEndSpeedRate,
                normalizedTime
            );

            // 横方向に固定
            m_rigidBody2D.linearVelocity =
                new Vector2(evadeDirection * speed, 0f);

            elapsedTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        // 回避終了
        m_rigidBody2D.gravityScale = originalGravityScale;
        m_rigidBody2D.linearVelocity =
            new Vector2(0f, m_rigidBody2D.linearVelocity.y);

        m_playerMoveState = PlayerMoveState.Recovering;

        yield return new WaitForSeconds(m_recoveringTime);

        m_playerMoveState = PlayerMoveState.Moving;

        // キーを押し続けていれば、押し直さずに通常移動へ復帰
        ApplyMove(m_currentMoveInput);
    }
}