using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SignalInteger : ScriptableObject {

    [Header("Signal informations")]
    public List<SignalIntegerListener> listeners = new List<SignalIntegerListener>();

    public void Raise(int value) {
        for (int i = listeners.Count - 1; i >= 0; i--) {
            listeners[i].OnSignalRaised(value);
        }
    }

    public void RegisterListener(SignalIntegerListener listener) {
        listeners.Add(listener);
    }

    public void DeRetisterListener(SignalIntegerListener listener) {
        listeners.Remove(listener);
    }

}