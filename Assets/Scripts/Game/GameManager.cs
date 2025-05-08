using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static event Action<SoundType, Vector3> OnPlayerMadeSound;

    public static void SendPlayerMadeSound(SoundType type, Vector3 playerPosition)
    {
        if(OnPlayerMadeSound != null) OnPlayerMadeSound.Invoke(type, playerPosition);
    }
}
