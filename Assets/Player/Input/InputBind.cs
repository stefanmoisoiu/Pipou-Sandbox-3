using System;
using UnityEngine.Events;

[Serializable]
public struct InputBind
{
    public InputManager.InputType type;
    public InputManager.InputAdvancement advancement;
    public UnityEvent inputCallback;

    public void Bind() => InputManager.Bind(CallCallback,type,advancement);

    private void CallCallback() => inputCallback?.Invoke();
}