using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    public void RaiseLoadRequestEvent(GameSceneSO locationToGo,Vector3 posToGo,bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(locationToGo, posToGo, fadeScreen);
    }
}
