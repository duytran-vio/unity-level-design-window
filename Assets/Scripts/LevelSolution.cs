using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New level solution", menuName ="Level Solution")]
public class LevelSolution : ScriptableObject
{
    // public struct Point{
    //     public int x;
    //     public int y;
    // }

    public int nMinPath;
    // public Point[] minPath;
    // public Point test;

    public int[] x;
    public int[] y;
}
