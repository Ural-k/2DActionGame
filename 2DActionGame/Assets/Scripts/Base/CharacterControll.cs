using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class CharacterControl : MonoBehaviour
{

    [SerializeField] 
    protected float m_moveSpeed;  // 移動速度   
    [SerializeField] 
    protected float m_jumpForce;  // ジャンプ力
    [SerializeField] 
    protected Rigidbody2D m_rigidBody2D;  // Rigidbody2Dコンポーネントへの参照
    [SerializeField] 
    protected BoxCollider2D m_boxCollider;  // BoxCollider2Dコンポーネントへの参照
    [SerializeField]
    protected float groundCheckDistance_ = 0.2f;  // 接地判定の距離
    [SerializeField]
    protected LayerMask groundLayer_;  // 接地判定に使用するレイヤーマスク

    private readonly HashSet<string> m_coolDowns = new();  // クールダウン中のアクションを管理するHashSet

    private float LEFTOFFSET = 0.5f;  // 左側のオフセット
    private float CENTEROFFSET = 1.5f;  // 中央のオフセット
    private float RIGHTOFFSET = 2.5f;  // 右側のオフセット

    protected virtual void Awake()
    {
        m_rigidBody2D = GetComponent<Rigidbody2D>();
        m_boxCollider = GetComponent<BoxCollider2D>();
    }
    /// <summary>
    /// 移動
    /// </summary>
    public abstract void Move(Vector2 direction);

    /// <summary>
    /// ジャンプ
    /// </summary>
    public abstract void Jump();

    /// <summary>
    /// 攻撃
    /// </summary>
    public abstract void Attack();

    protected bool IsInCoolDown(string coolDownName,float seconds)
    {
       if(m_coolDowns.Contains(coolDownName))
        {
            return true; // クールダウン中
        }
        else
        {
            m_coolDowns.Add(coolDownName); // クールダウンを開始
            StartCoroutine(CoolDownCoroutine(coolDownName, seconds));
            return false; // クールダウンではない
        }
    }

    private IEnumerator CoolDownCoroutine(string coolDownName, float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_coolDowns.Remove(coolDownName); // クールダウンを終了
    }

    /// <summary>
    /// 接地判定
    /// </summary>
    private bool CheckGround(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            point,
            Vector2.down,
            groundCheckDistance_,
            groundLayer_
        );

        return hit.collider != null;
    }


    /// <summary>
    /// 三点チェック
    /// </summary>
    protected bool IsGrounded()
    {
        Bounds bounds = m_boxCollider.bounds;

        // Colliderの左右
        float left = bounds.min.x;
        float right = bounds.max.x;

        // Colliderの下
        float bottom = bounds.min.y;

        // 横幅を3等分
        float sectionWidth = bounds.size.x / 3.0f;

        // それぞれの区画の中央
        Vector2 leftPoint = new Vector2(
            left + sectionWidth * LEFTOFFSET,
            bottom
        );

        Vector2 centerPoint = new Vector2(
            left + sectionWidth * CENTEROFFSET,
            bottom
        );

        Vector2 rightPoint = new Vector2(
            left + sectionWidth * RIGHTOFFSET,
            bottom
        );

        // 3本のRay
        bool leftHit = CheckGround(leftPoint);
        bool centerHit = CheckGround(centerPoint);
        bool rightHit = CheckGround(rightPoint);

        return leftHit || centerHit || rightHit;
    }


    /// <summary>
    /// デバッグ用の足元のRayを描画
    /// </summary>
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.yellow;
        if (m_boxCollider == null)
            m_boxCollider = GetComponent<BoxCollider2D>();

        Bounds bounds = m_boxCollider.bounds;

        float left = bounds.min.x;
        float bottom = bounds.min.y;
        float sectionWidth = bounds.size.x / 3.0f;

        Vector2 leftPoint = new Vector2(
            left + sectionWidth * 0.5f,
            bottom
        );

        Vector2 centerPoint = new Vector2(
            left + sectionWidth * 1.5f,
            bottom
        );

        Vector2 rightPoint = new Vector2(
            left + sectionWidth * 2.5f,
            bottom
        );

        Gizmos.DrawLine(
            leftPoint,
            leftPoint + Vector2.down * groundCheckDistance_
        );

        Gizmos.DrawLine(
            centerPoint,
            centerPoint + Vector2.down * groundCheckDistance_
        );

        Gizmos.DrawLine(
            rightPoint,
            rightPoint + Vector2.down * groundCheckDistance_
        );
    }
}
