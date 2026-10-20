using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class InputSystemHandle : MonoBehaviour
{
    private PlayerInput m_playerInput;  // PlayerInputコンポーネントへの参照

    private readonly List<InputBinding> m_bindings = new(); // 登録されたInputActionとコールバックのリスト

    /// <summary>
    /// 登録したイベントを保持するクラス
    /// </summary>
    private class InputBinding
    {
        public InputAction Action;

        public Action<InputAction.CallbackContext> Performed;
        public Action<InputAction.CallbackContext> Canceled;
    }

    private void Awake()
    {
        m_playerInput = GetComponent<PlayerInput>();

        RegisterActions();
    }

    private void OnDestroy()
    {
        UnregisterActions();
    }

    /// <summary>
    /// 同じGameObjectにアタッチされているMonoBehaviourのメソッドに、InputActionsAttributeが付与されているメソッドをInputActionに登録する
    /// </summary>
    private void RegisterActions()
    { 
        MonoBehaviour[] components =
            GetComponents<MonoBehaviour>();

        foreach (MonoBehaviour component in components)
        {
            RegisterComponent(component);
        }
    }

    /// <summary>
    /// InputActionsAttributeが付与されているメソッドをInputActionに登録する
    /// </summary>
    private void RegisterComponent(MonoBehaviour component)
    {
        MethodInfo[] methods =
            component.GetType().GetMethods(
                BindingFlags.Instance |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

        foreach (MethodInfo method in methods)
        {
            RegisterPerformed(component, method);
            RegisterCanceled(component, method);
        }
    }

    private void RegisterPerformed(
        MonoBehaviour component,
        MethodInfo method)
    {
        InputActionsAttribute attribute =
            method.GetCustomAttribute<InputActionsAttribute>();

        if (attribute == null)
        {
            return;
        }

        InputAction action =
            m_playerInput.actions.FindAction(
                attribute.ActionName);

        if (action == null)
        {
            Debug.LogError(
                $"Input Action '{attribute.ActionName}' が見つかりません。");

            return;
        }

        Action<InputAction.CallbackContext> callback =
            CreatePerformedCallback(component, method);

        if (callback == null)
        {
            return;
        }

        action.performed += callback;

        m_bindings.Add(new InputBinding
        {
            Action = action,
            Performed = callback
        });
    }

    private void RegisterCanceled(
        MonoBehaviour component,
        MethodInfo method)
    {
        InputActionCancelAttribute attribute =
            method.GetCustomAttribute<InputActionCancelAttribute>();

        if (attribute == null)
        {
            return;
        }

        InputAction action =
            m_playerInput.actions.FindAction(
                attribute.ActionName);

        if (action == null)
        {
            Debug.LogError(
                $"Input Action '{attribute.ActionName}' が見つかりません。");

            return;
        }

        Action<InputAction.CallbackContext> callback =
            CreateCanceledCallback(component, method);

        if (callback == null)
        {
            return;
        }

        action.canceled += callback;

        m_bindings.Add(new InputBinding
        {
            Action = action,
            Canceled = callback
        });
    }

    private Action<InputAction.CallbackContext>
        CreatePerformedCallback(
            MonoBehaviour component,
            MethodInfo method)
    {
        // Vector2を受け取るメソッド
        if (method.GetParameters().Length == 1 &&
            method.GetParameters()[0].ParameterType == typeof(Vector2))
        {
            Action<Vector2> methodDelegate =
                (Action<Vector2>)method.CreateDelegate(
                    typeof(Action<Vector2>),
                    component);

            return context =>
            {
                Vector2 value =
                    context.ReadValue<Vector2>();

                methodDelegate(value);
            };
        }

        // 引数なしのメソッド
        if (method.GetParameters().Length == 0)
        {
            Action methodDelegate =
                (Action)method.CreateDelegate(
                    typeof(Action),
                    component);

            return context =>
            {
                methodDelegate();
            };
        }

        Debug.LogError(
            $"{method.Name} の引数が対応していません。" +
            "Vector2 または引数なしにしてください。");

        return null;
    }

    private Action<InputAction.CallbackContext>
        CreateCanceledCallback(
            MonoBehaviour component,
            MethodInfo method)
    {
        // キャンセルは基本的に引数なし
        if (method.GetParameters().Length != 0)
        {
            Debug.LogError(
                $"{method.Name} はキャンセル用メソッドなので" +
                "引数なしにしてください。");

            return null;
        }

        Action methodDelegate =
            (Action)method.CreateDelegate(
                typeof(Action),
                component);

        return context =>
        {
            methodDelegate();
        };
    }

    /// <summary>
    /// 登録したイベントを解除する
    /// </summary>
    private void UnregisterActions()
    {
        foreach (InputBinding binding in m_bindings)
        {
            if (binding.Performed != null)
            {
                binding.Action.performed -= binding.Performed;
            }

            if (binding.Canceled != null)
            {
                binding.Action.canceled -= binding.Canceled;
            }
        }

        m_bindings.Clear();
    }
}