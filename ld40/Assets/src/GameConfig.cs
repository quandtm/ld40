using UnityEngine;

[CreateAssetMenu(menuName="Game Config", fileName="Difficulty")]
public class GameConfig : ScriptableObject
{
    public int NumGhosts;
    public float TimeLimit;
    public float SecondsBetweenHints;
    public float LastChanceHintTime;
    public bool EnableHints;
    public int MinBuyersForWin;
}