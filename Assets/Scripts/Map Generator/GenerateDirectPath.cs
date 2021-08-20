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

    private int startDirection, endDirection, leftDirection, rightDirection;

    Grid grid;
    GridDirectionNode gridDirectionParentNode, currentDirectionNode;
    GridDirectionNode gizmosCurrentDirectionNode, previousCurrentDirectionNode;

    private int[,] gridOrientation = {
        // 0 - North
        // 1 - East
        // 2 - South
        // 3 - West
        //  Back    Left    Forward Right
        {   0,      1,      2,      3},
        {   1,      2,      3,      0},
        {   2,      3,      0,      1},
        {   3,      0,      1,      2}
    };

    private int[,] startSideRooms;
    private int[,] endSideRooms;

    private int[] startTile;
    private int[] currentRoom;

    void Start()
    {
        // --- Init Grid ---
        grid = new Grid(MinRange, MaxRange);

        startSideRooms = new int[grid.EdgeSize, 2];
        endSideRooms = new int[grid.EdgeSize, 2];
        // Select a random direction as the start
        startDirection = UnityEngine.Random.Range(0, grid.EdgesCount);

        // Get the surronding available directions
        leftDirection = (startDirection + 1) % grid.EdgesCount;
        endDirection = (leftDirection + 1) % grid.EdgesCount;
        rightDirection = (endDirection + 1) % grid.EdgesCount;

        // Get the rooms that exist in the start and end edges
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
        currentRoom = new int[2];
        startTile.CopyTo(currentRoom, 0);

        gridDirectionParentNode = new GridDirectionNode(currentRoom);
        currentDirectionNode = gridDirectionParentNode;

        TraversePath(startDirection);
        int k = 0;
    }

    void FillEdgeTiles(int idx, int s1, int s2, int e1, int e2)
    {
        // Store the room coordinates on the start to a list
        startSideRooms[idx, 0] = s1;
        startSideRooms[idx, 1] = s2;

        // Store the room coordinates on the end to a list
        endSideRooms[idx, 0] = e1;
        endSideRooms[idx, 1] = e2;
    }

    void TraversePath(int previousDirection)
    {
        grid.GridData[currentRoom[0], currentRoom[1]] = 1;

        if (CheckIfRoomOnEdge(endSideRooms, currentRoom[0], currentRoom[1]))
        {
            // Stop when a tile on the end side is traversed
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
        PruneAvailableDirections(0, leftDirection, availablePaths);

        // End direction
        PruneAvailableDirections(1, endDirection, availablePaths);

        // Right direction
        PruneAvailableDirections(2, rightDirection, availablePaths);
        
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
        
        // Traverse the next room
        switch (newDirection)
        {
            case 0:
                currentRoom[0] -= 1;
                break;
            case 1:
                currentRoom[1] += 1;
                break;
            case 2:
                currentRoom[0] += 1;
                break;
            case 3:
                currentRoom[1] -= 1;
                break;
            default:
                break;
        }

        currentDirectionNode.AddChildRoom(currentRoom);
        currentDirectionNode = currentDirectionNode.child;
        TraversePath(newDirection);
    }

    void OnDrawGizmos()
    {
        if (grid == null)
        {
            return;
        }

        previousCurrentDirectionNode = gridDirectionParentNode;
        gizmosCurrentDirectionNode = gridDirectionParentNode.child;

        while(true)
        {
            Gizmos.color = Color.cyan;

            Vector3 startPos = new Vector3(previousCurrentDirectionNode.coordinate[0] * 1.5f, 0f, previousCurrentDirectionNode.coordinate[1] * 1.5f);
            Vector3 endPos = new Vector3(gizmosCurrentDirectionNode.coordinate[0] * 1.5f, 0f, gizmosCurrentDirectionNode.coordinate[1] * 1.5f);

            Gizmos.DrawLine(startPos, endPos);

            previousCurrentDirectionNode = gizmosCurrentDirectionNode;
            if (gizmosCurrentDirectionNode.child == null)
            {
                break;
            }
            // Go to the next node
            gizmosCurrentDirectionNode = gizmosCurrentDirectionNode.child;
        }

        for (int i = 0; i < grid.EdgeSize; i++)
        {
            for (int j = 0; j < grid.EdgeSize; j++)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

                if (CheckIfRoomOnEdge(startSideRooms, i, j))
                {
                    // Check if room exists in the start side 
                    Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                }
                else if(CheckIfRoomOnEdge(endSideRooms, i, j))
                {
                    // Check if room exists in the end side
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                }

                if (grid.GridData[i, j] == 1)
                {
                    // Check if room is part of path
                    Gizmos.color = new Color(0f, 0f, 1f, 0.5f);
                }

                if (startTile[0] == i && startTile[1] == j)
                {
                    // Check if room is the start
                    Gizmos.color = new Color(1f, 1f, 0f, 0.5f);
                }

                // Draw the cube
                Gizmos.DrawCube(new Vector3(1.5f * i, 0, 1.5f * j), new Vector3(1, 1, 1));
            }
        }
    }

    bool CheckIfRoomOnEdge(int[,] side, int row, int col)
    {
        // Check if the tiles lie within the edge list
        for(int i = 0; i < side.GetLength(0); i++)
        {
            if (side[i, 0] == row && side[i, 1] == col)
            {
                return true;
            }
        }
        return false;
    }

    void PruneAvailableDirections(int directionIdx, int direction, int[] availablePaths)
    {
        switch (direction)
        {
            case 0:
                if (currentRoom[0] - 1 < 0)
                {
                    // if its not in the grid then remove it from the available paths
                    availablePaths[directionIdx] = -1;
                }
                break;
            case 1:
                
                if (currentRoom[1] + 1 >= grid.EdgeSize)
                {
                    availablePaths[directionIdx] = -1;
                }
                break;
            case 2:
                if (currentRoom[0] + 1 >= grid.EdgeSize)
                {
                    availablePaths[directionIdx] = -1;
                }
                break;
            case 3:
                if (currentRoom[1] - 1 < 0)
                {
                    availablePaths[directionIdx] = -1;
                }
                break;
            default:
                break;
        }
    }
}
