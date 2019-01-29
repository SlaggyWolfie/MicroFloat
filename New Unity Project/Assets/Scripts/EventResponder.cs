using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Slavi;

public class EventResponder : MonoBehaviour
{
    // Start is called before the first frame update
    public float onLossDelay = 4;

    private void Start()
    {
        EventQueue.Instance.AddListener<LoseLevelEvent>(LoseLevel);
        EventQueue.Instance.AddListener<WinLevelEvent>(WinLevel);
    }

    private void LoseLevel(LoseLevelEvent e)
    {
        StartCoroutine(CoroutineHelpers.Wait(onLossDelay,
            () => { SceneManager.LoadScene(SceneManager.GetActiveScene().name); }));
    }

    private void WinLevel(WinLevelEvent e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //GameplayManager.Instance.RulesInstance.CompleteLevel();
        GameplayManager.Instance.Init(GameplayManager.Instance.RulesInstance.GetFirstIncompleteLevel());
    }

    private void OnDisable()
    {
        EventQueue.Instance.RemoveListener<LoseLevelEvent>(LoseLevel);
        EventQueue.Instance.RemoveListener<WinLevelEvent>(WinLevel);
    }

    //[RuntimeInitializeOnLoadMethod]
    //private static void StaticStart()
    //{
    //    EventQueue.Instance.AddListener<LoseLevelEvent>(StaticLoseLevel);
    //    EventQueue.Instance.AddListener<WinLevelEvent>(StaticWinLevel);
    //}

    //private static void StaticLoseLevel(LoseLevelEvent e)
    //{
    //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //}

    //private static void StaticWinLevel(WinLevelEvent e)
    //{
    //    StaticLoseLevel(null);
    //    GameplayManager.Instance.RulesInstance.CompleteLevel();
    //    GameplayManager.Instance.Init(GameplayManager.Instance.RulesInstance.GetFirstIncompleteLevel());
    //}
}
