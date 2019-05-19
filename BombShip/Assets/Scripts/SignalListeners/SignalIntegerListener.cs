using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityIntegerEvent : UnityEvent<int> { }

public class SignalIntegerListener : MonoBehaviour {

    [Header("Signal integer listener informations")]
    public SignalInteger signal;
    public UnityIntegerEvent signalEvent;

    public void OnSignalRaised(int value) {
        signalEvent.Invoke(value);
    }

    private void OnEnable() {
        signal.RegisterListener(this);
    }

    private void OnDisable() {
        signal.DeRetisterListener(this);
    }

}