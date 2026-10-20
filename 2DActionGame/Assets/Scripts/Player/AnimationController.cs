using System.Collections.Generic;
using UnityEngine;

public abstract class AnimationController : MonoBehaviour
{
    [SerializeField]
    protected Animator m_animator;  // Animatorコンポーネントへの参照

    protected HashSet<string> m_animationCoolDowns = new();  // アニメーションを管理するHashSet

    protected virtual void Awake()
    {
        m_animator = GetComponent<Animator>();
        foreach(AnimationClip clip in m_animator.runtimeAnimatorController.animationClips)
        {
            m_animationCoolDowns.Add(clip.name);
        }
    }

}
