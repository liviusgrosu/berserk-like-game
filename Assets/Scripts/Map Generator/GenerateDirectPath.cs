using System;
using System.Linq;
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
    int[] currentTile;

    int currentTripCounter = 0;
    int tripCounter = 40;

    void Start()
    {
        // --- Init Grid ---
        grid = new Grid(MinRange, MaxRange);

        startSideRooms = new int[grid.EdgeSize, 2];
        endSideRooms = new int[grid.EdgeSize, 2];

        startDirection = UnityEngine.Random.Range(0, grid.EdgesCount);
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

        int randNum = UnityEngine.Random.Range(0, grid.EdgeSize);

        startTile = new int[] {startSideRooms[randNum, 0], startSideRooms[randNum, 1]};
        currentTile = startTile.ToArray();
        TraversePath(startDirection);

        currentTripCounter = 0;
    }

    void FillEdgeTiles(int idx, int s1, int s2, int e1, int e2)
    {
        startSideRooms[idx, 0] = s1;
        startSideRooms[idx, 1] = s2;

        endSideRooms[idx, 0] = e1;
        endSideRooms[idx, 1] = e2;
    }

    void TraversePath(int previousDirection)
    {
        currentTripCounter++;
        if (currentTripCounter >= tripCounter)
        {
            return;
        }

        grid.GridData[currentTile[0], currentTile[1]] = 1;

        if (CheckIfTileExists(endSideRooms, currentTile[0], currentTile[1]))
        {
            return;
        }

        // Check available paths
        int[] availablePaths = { leftDirection, endDirection, rightDirection };

        // Remove possibility of going back
        if (previousDirection == leftDirection)
        {
            availablePaths[2] = -1;
        }
        else if (previousDirection == rightDirection)
        {
            availablePaths[0] = -1;
        }

        // Left direction
        switch (leftDirection)
        {
            case 0:
                // if its not in the grid then remove 
                if (currentTile[0] - 1 < 0)
                {
                    availablePaths[0] = -1;
                }

                break;
            case 1:
                // if its not in the grid then remove 
                if (currentTile[1] + 1 >= grid.EdgeSize)
                {
                    availablePaths[0] = -1;
                }
                break;
            case 2:
                // if its not in the grid then remove 
                if (currentTile[0] + 1 >= grid.EdgeSize)
                {
                    availablePaths[0] = -1;
                }
                break;
            case 3:
                // if its not in the grid then remove 
                if (currentTile[1] - 1 < 0)
                {
                    availablePaths[0] = -1;
                }
                break;
            default:
                break;
        }

        // End direction
        switch (endDirection)
        {
            case 0:
                // if its not in the grid then remove 
                if (currentTile[0] - 1 < 0)
                {
                    availablePaths[1] = -1;
                }
                break;
            case 1:
                // if its not in the grid then remove 
                if (currentTile[1] + 1 >= grid.EdgeSize)
                {
                    availablePaths[1] = -1;
                }
                break;
            case 2:
                // if its not in the grid then remove 
                if (currentTile[0] + 1 >= grid.EdgeSize)
                {
                    availablePaths[1] = -1;
                }
                break;
            case 3:
                // if its not in the grid then remove 
                if (currentTile[1] - 1 < 0)
                {
                    availablePaths[1] = -1;
                }
                break;
            default:
                break;
        }

        // Right direction
        switch (rightDirection)
        {
            case 0:
                // if its not in the grid then remove 
                if (currentTile[0] - 1 < 0)
                {
                    availablePaths[2] = -1;
                }

                break;
            case 1:
                // if its not in the grid then remove 
                if (currentTile[1] + 1 >= grid.EdgeSize)
                {
                    availablePaths[2] = -1;
                }
                break;
            case 2:
                // if its not in the grid then remove 
                if (currentTile[0] + 1 >= grid.EdgeSize)
                {
                    availablePaths[2] = -1;
                }
                break;
            case 3:
                // if its not in the grid then remove 
                if (currentTile[1] - 1 < 0)
                {
                    availablePaths[2] = -1;
                }
                break;
            default:
                break;
        }
        
        // Remove unavailable paths
        int newDirection = -1;
        List<int> pathCandidates = availablePaths.ToList();
        pathCandidates.RemoveAll(item => item == -1);

        if (!pathCandidates.Any())
        {
            // If theres no more availble paths then the path is finished
            return;
        }

        // Get random index of possible path candidates
        int rand = UnityEngine.Random.Range(0, pathCandidates.Count);
        int directionIdx = Array.IndexOf(availablePaths, pathCandidates[rand]);
        newDirection = availablePaths[directionIdx];
        
        switch (newDirection)
        {
            case 0:
                currentTile[0] -= 1;
                break;
            case 1:
                currentTile[1] += 1;
                break;
            case 2:
                currentTile[0] += 1;
                break;
            case 3:
                currentTile[1] -= 1;
                break;
            default:
                break;
        }

        TraversePath(newDirection);
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

                if (grid.GridData[i, j] == 1)
                {
                    Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
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
