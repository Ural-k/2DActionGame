using UnityEngine;
public abstract class CharacterControl : MonoBehaviour
{
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
}
