using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerNameSoItDoesNotGetFucked : MonoBehaviour
{
    public static GameManagerNameSoItDoesNotGetFucked Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}