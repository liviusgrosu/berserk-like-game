using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GenerateDirectPath : MonoBehaviour
{
    // TODO: add custom editor range
    // https://docs.unity3d.com/ScriptReference/EditorGUILayout.MinMaxSlider.html
    public int MinRange;
    public int MaxRange;
    public int EdgeSize = 4;

    private int startDirection, endDirection, leftDirection, rightDirection;

    Grid grid;

    private int[,] gridOrientation = {
        //  Start   Left    Forward Right
        {   0,      1,      2,      3},
        {   1,      2,      3,      0},
        {   2,      3,      0,      1},
        {   3,      0,      1,      2}
    };

    private int[,] startSideRooms;
    private int[,] endSideRooms;

    int[] startTile;

    void Start()
    {
        // --- Init Grid ---
        grid = new Grid(MinRange, MaxRange);

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
                    FillEdgeTiles(i, 0, i, grid.EdgeSize - 1, i);
                }
                break;
            case 1:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, i, grid.EdgeSize - 1, i, 0);
                }
                break;
            case 2:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, grid.EdgeSize - 1, i, 0, i);
                }
                break;
            case 3:
                for (int i = 0; i < grid.EdgeSize; i++)
                {
                    FillEdgeTiles(i, i, 0, i, grid.EdgeSize - 1);
                }
                break;
            default:
                break;
        }

        // --- Calculate Path ---

        int randNum = Random.Range(0, grid.EdgeSize);

        startTile =  new int[] {startSideRooms[randNum, 0], startSideRooms[randNum, 1]};
    }

    void FillEdgeTiles(int idx, int s1, int s2, int e1, int e2)
    {
        startSideRooms[idx, 0] = s1;
        startSideRooms[idx, 1] = s2;

        endSideRooms[idx, 0] = e1;
        endSideRooms[idx, 1] = e2;
    }

    void OnDrawGizmos()
    {
        if (grid == null)
        {
            return;
        }
        for (int i = 0; i < grid.EdgeSize; i++)
        {
            for (int j = 0; j < grid.EdgeSize; j++)
            {
                if (CheckIfTileExists(startSideRooms, i, j))
                {
                    Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                    if (startTile[0] == i && startTile[1] == j)
                    {
                        Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
                    }
                }
                else if(CheckIfTileExists(endSideRooms, i, j))
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                }
                else
                {
                    Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                }

                // Check if tile exists in startSideRooms
                Gizmos.DrawCube(new Vector3(1.5f * i, 0, 1.5f * j), new Vector3(1, 1, 1));
            }
        }
    }

    bool CheckIfTileExists(int[,] side, int row, int col)
    {
        for(int i = 0; i < side.GetLength(0); i++)
        {
            if (side[i, 0] == row && side[i, 1] == col)
            {
                return true;
            }
        }
        return false;
    }
}
