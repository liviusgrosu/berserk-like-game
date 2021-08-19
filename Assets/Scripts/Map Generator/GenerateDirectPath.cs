using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateDirectPath : MonoBehaviour
{
    // TODO: add custom editor range
    // https://docs.unity3d.com/ScriptReference/EditorGUILayout.MinMaxSlider.html
    public int MinRange;
    public int MaxRange;
    public int EdgeSize = 4;
    private int[,] grid;

    private int startDirection, endDirection, leftDirection, rightDirection;

    private int[,] gridOrientation = {
        //  Start   Left    Forward Right
        {   0,      1,      2,      3},
        {   1,      2,      3,      0},
        {   2,      3,      0,      1},
        {   3,      0,      1,      2}
    };

    private int[,] startSideRooms;
    private int[,] endSideRooms;

    void Start()
    {
        Grid grid = new Grid(MinRange, MaxRange);

        startSideRooms = new int[grid.EdgeSize, 2];
        endSideRooms = new int[grid.EdgeSize, 2];

        startDirection = Random.Range(0, grid.EdgesCount);
        for(int i = 0; i < gridOrientation.GetLength(0); i++)
        {
            if (startDirection == gridOrientation[i, 0])
            {
                leftDirection =  gridOrientation[i, 1];
                endDirection =   gridOrientation[i, 2];
                rightDirection = gridOrientation[i, 3];
                break;
            }
        }

        switch(startDirection)
        {
            case 0:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    startSideRooms[i, 0] = 0;
                    startSideRooms[i, 1] = i;

                    endSideRooms[i, 0] = 3;
                    endSideRooms[i, 1] = i;
                }
                break;
            case 1:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    startSideRooms[i, 0] = i;
                    startSideRooms[i, 1] = 3;

                    endSideRooms[i, 0] = i;
                    endSideRooms[i, 1] = 0;
                }
                break;
            case 2:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    startSideRooms[i, 0] = 3;
                    startSideRooms[i, 1] = i;

                    endSideRooms[i, 0] = 0;
                    endSideRooms[i, 1] = i;
                }
                break;
            case 3:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    startSideRooms[i, 0] = i;
                    startSideRooms[i, 1] = 0;

                    endSideRooms[i, 0] = i;
                    endSideRooms[i, 1] = 3;
                }
                break;
            default:
                break;
        }
        int k = 0;
    }
}
