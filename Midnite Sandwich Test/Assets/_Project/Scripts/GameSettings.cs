using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
public class GameSettings : ScriptableObject
{
    /// <summary>
    /// How many rows and columns the grid must have.
    /// The first coordinate it's always (0,0)
    /// </summary>
    public int GridSize = 4;
}
