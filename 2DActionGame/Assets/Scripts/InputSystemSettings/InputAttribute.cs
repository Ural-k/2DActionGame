using System;

/// <summary>
/// InputSystemのアクションに対応するメソッドに付与する属性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class InputActionsAttribute : Attribute
{
    public string ActionName { get; }

    public InputActionsAttribute(string actionName)
    {
        ActionName = actionName;
    }
}

/// <summary>
/// InputActionがキャンセルされたときに呼び出されるメソッドに付与する属性
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class InputActionCancelAttribute : Attribute
{
    public string ActionName { get; }
    public InputActionCancelAttribute(string actionName)
    {
        ActionName = actionName;
    }
}
