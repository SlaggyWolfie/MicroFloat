//#define WE_NEED_ACCESS_TO_COMPLETED_LEVELS
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu]
public class RulesSettings : ScriptableObject
{
#if WE_NEED_ACCESS_TO_COMPLETED_LEVELS
    private List<LevelSettings> _completedLevels = new List<LevelSettings>();
#endif

    [SerializeField, ReorderableList] private List<LevelSettings> _levelOrder = new List<LevelSettings>();

    public int IncompleteLevelCount { get { return _levelOrder.Count; } }
    public LevelSettings GetIncompleteLevel(int index) { return _levelOrder[index]; }
    public LevelSettings GetFirstIncompleteLevel() {
        //Debug.Log(IncompleteLevelCount + " What " + _levelOrder[0]);
        return _levelOrder.Count <= 0 ? null : GetIncompleteLevel(0); }

    /// <summary>
    /// Completes the first level in the list.
    /// </summary>
    /// <returns>True, if there are any levels to complete; False, if there are none left.</returns>
    public bool CompleteLevel()
    {
        if (_levelOrder.Count == 0) return false;
#if WE_NEED_ACCESS_TO_COMPLETED_LEVELS
        _completedLevels.Add(_levelOrder[0]);
#endif
        _levelOrder.RemoveAt(0);
        return true;
    }

    /// <summary>
    /// Completes the supplied level, if not already completed.
    /// </summary>
    /// <param name="level">The level to complete.</param>
    /// <returns>True, if the level is completed successfully; False, if the level has already been completed.</returns>
    public bool CompleteLevel(LevelSettings level)
    {
        if (level == null) return false;
#if WE_NEED_ACCESS_TO_COMPLETED_LEVELS
        if (!_completedLevels.Contains(level)) _completedLevels.Add(level);
#endif
        //Debug.Log("Completed level " + (_levelOrder.IndexOf(level) + 1) + " " + level.ToString());
        return _levelOrder.Remove(level);
    }
}
