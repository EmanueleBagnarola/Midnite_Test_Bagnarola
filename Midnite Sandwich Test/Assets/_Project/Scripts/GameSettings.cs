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
    [Header("How many rows and columns the grid must have. Grid start at (0,0)")]
    public int GridSize = 4;

    /// <summary>
    /// Indicates how much height each tile rises when stacked
    /// </summary>
    [Header("Indicates how much height each tile rises when stacked")]
    public float TilesHeight = 0.1f;
}
