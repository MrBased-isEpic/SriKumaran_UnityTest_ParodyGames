using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
    private int noOfPoints;
    private int pointsEarned;

    private void Start()
    {
        PointCube[] cubes = GetComponentsInChildren<PointCube>();
        noOfPoints = cubes.Length;

        pointsEarned = 0;
    }
    
    public void AddPoint()
    {
        pointsEarned++;

        if (pointsEarned == noOfPoints)
        {
            Debug.Log("Won the game");
            //win
        }
    }
}
