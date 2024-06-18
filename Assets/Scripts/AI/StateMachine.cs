using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
[System.Serializable]
public class StateMachine<T>
{
    public Dictionary<Type, State<T>> States { get; private set; } = new Dictionary<Type, State<T>>();

    public State<T> CurrentState { get; private set; }
    public State<T> PreviousState { get; private set; }
    public StateMachine()
    {

    }
    public StateMachine(Dictionary<Type, State<T>> states)
    {
        States = states;
    }
    public void AddState(Type type, State<T> state)
    {
        if (States.ContainsKey(type))
        {
            Debug.Log("State of type " + type + " already in dictionary");
            return;
        }
        States.Add(type, state);
    }

    public void ChangeState(Type type)
    {
        if (!States.ContainsKey(type))
        {
            Debug.LogError("Can't fine state of type " + type);
            return;
        }
        if (CurrentState != null)
        {
            PreviousState = CurrentState;
            CurrentState.OnExit();
        }
        CurrentState = States[type];
        CurrentState.OnEnter();
    }
    public void StopStateMachine(bool exitCurrentState = true)
    {
        if (exitCurrentState)
        {
            if (CurrentState != null)
            {
                PreviousState = CurrentState;
                CurrentState.OnExit();
                CurrentState = null;
            }
        }
    }
    public void Tick()
    {
        if (CurrentState != null)
        {
            CurrentState.OnUpdate();
        }
    }
    public string GetCurrentDebugInfo()
    {
        StringBuilder debugInfo = new StringBuilder();
        if (CurrentState != null)
        {
            debugInfo.AppendLine($"Current State: {CurrentState.GetType().Name}");
            debugInfo.AppendLine($"Owner Type: {typeof(T).Name}");

            FieldInfo[] fields = CurrentState.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (FieldInfo field in fields)
            {
                debugInfo.AppendLine($"{field.Name}: {field.GetValue(CurrentState)}");
            }
        }
        else
        {
            debugInfo.AppendLine("No state");
        }

        return debugInfo.ToString();
    }
}
[System.Serializable]
public abstract class State<T>
{
    public StateMachine<T> StateMachine { get; set; }
    public T Owner { get; set; }

    public State(StateMachine<T> stateMachine, T owner)
    {
        StateMachine = stateMachine;
        Owner = owner;
    }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnUpdate() { }
}
