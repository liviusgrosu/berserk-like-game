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

    // The chances of the path going towards the end side

    [Tooltip("The chance of traversing towards the end")]
    [Range(1, 100)]
    public int EndDirectionFactor = 50;

    private int startDirection, endDirection, leftDirection, rightDirection;

    Grid grid;
    GridDirectionNode gridDirectionParentNode, currentDirectionNode;
    GridDirectionNode gizmosCurrentDirectionNode, gizmosPreviousDirectionNode;

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

        // Create the main path
        TraversePath(startDirection);
        // Store the available paths around each room in the main path
        CalculateAvailableDirectionsForPath();
        int k = 0;
    }

    void CalculateAvailableDirectionsForPath()
    {
        currentDirectionNode = gridDirectionParentNode.child;
        while(currentDirectionNode.child != null)
        {
            List<int> availableDirections = new List<int> {0, 1, 2, 3};

            // Get coordinates around the path
            int[] northCoordinate = new int[] {currentDirectionNode.coordinate[0] - 1, currentDirectionNode.coordinate[1] };
            int[] eastCoordinate =  new int[] {currentDirectionNode.coordinate[0], currentDirectionNode.coordinate[1] + 1 };
            int[] southCoordinate = new int[] {currentDirectionNode.coordinate[0] + 1, currentDirectionNode.coordinate[1] };
            int[] westCoordinate =  new int[] {currentDirectionNode.coordinate[0], currentDirectionNode.coordinate[1] - 1 };

            if (CheckIfCoordinateExist(gridDirectionParentNode, northCoordinate) || CheckRoomOutOfBound(northCoordinate))
            {
                availableDirections[0] = -1;
            }

            if (CheckIfCoordinateExist(gridDirectionParentNode, eastCoordinate) || CheckRoomOutOfBound(eastCoordinate))
            {
                availableDirections[1] = -1;
            }

            if (CheckIfCoordinateExist(gridDirectionParentNode, southCoordinate) || CheckRoomOutOfBound(southCoordinate))
            {
                availableDirections[2] = -1;
            }

            if (CheckIfCoordinateExist(gridDirectionParentNode, westCoordinate) || CheckRoomOutOfBound(westCoordinate))
            {
                availableDirections[3] = -1;
            }

            availableDirections.RemoveAll(item => item == -1);

            currentDirectionNode.availableDirections = availableDirections;
            currentDirectionNode = currentDirectionNode.child;
        }
    }

    bool CheckIfCoordinateExist(GridDirectionNode room, int[] coordinate)
    {
        if(room.child == null)
        {
            return false;
        }
        if (coordinate[0] == room.coordinate[0] && coordinate[1] == room.coordinate[1])
        {
            return true;
        }
        return CheckIfCoordinateExist(room.child, coordinate);
    }

    bool CheckRoomOutOfBound(int[] coordinate)
    {
        // Check if the room is out of bound
        return coordinate[0] - 1 < 0 || coordinate[1] + 1 > grid.EdgeSize || coordinate[0] + 1 > grid.EdgeSize || coordinate[1] - 1 < 0;
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

        // currentDirectionNode.availableDirections = new List<int>(pathCandidates);

        if (pathCandidates.Count >= 2 && pathCandidates.Contains(endDirection))
        {
            if(UnityEngine.Random.Range(0, 100) >= EndDirectionFactor)
            {
                // Make end direction harder to traverse
                pathCandidates.Remove(endDirection);
            }
        }

        // Get random index of possible path candidates
        int rand = UnityEngine.Random.Range(0, pathCandidates.Count);
        int directionIdx = Array.IndexOf(availablePaths, pathCandidates[rand]);
        newDirection = availablePaths[directionIdx];
        
        // Current available direction becomes whats left
        // currentDirectionNode.availableDirections.RemoveAll(item => item == newDirection);

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

    void TraverseOptionalPath()
    {

    }

    void OnDrawGizmos()
    {
        if (grid == null)
        {
            return;
        }

        gizmosPreviousDirectionNode = gridDirectionParentNode;
        gizmosCurrentDirectionNode = gridDirectionParentNode.child;

        while(true)
        {
            Gizmos.color = Color.cyan;

            Vector3 startPos = new Vector3(gizmosPreviousDirectionNode.coordinate[0] * 1.5f, 0f, gizmosPreviousDirectionNode.coordinate[1] * 1.5f);
            Vector3 endPos = new Vector3(gizmosCurrentDirectionNode.coordinate[0] * 1.5f, 0f, gizmosCurrentDirectionNode.coordinate[1] * 1.5f);

            // Draw the path between current and previous node
            Gizmos.DrawLine(startPos, endPos);

            if (gizmosCurrentDirectionNode.availableDirections != null)
            {
                foreach(int direction in gizmosCurrentDirectionNode.availableDirections)
                {
                    Gizmos.color = Color.yellow;

                    int[] nextCoordinate = new int[gizmosCurrentDirectionNode.coordinate.Length];
                    gizmosCurrentDirectionNode.coordinate.CopyTo(nextCoordinate, 0);

                    switch(direction)
                    {
                        case 0:
                            nextCoordinate[0]--;
                            break;
                        case 1:
                            nextCoordinate[1]++;
                            break;
                        case 2:
                            nextCoordinate[0]++;
                            break;
                        case 3:
                            nextCoordinate[1]--;
                            break;
                        default:
                            break;
                    }
                    Vector3 nextAvailableNode = new Vector3(nextCoordinate[0] * 1.5f, 0f, nextCoordinate[1] * 1.5f);
                    Gizmos.DrawLine(endPos, nextAvailableNode);
                }
            }

            gizmosPreviousDirectionNode = gizmosCurrentDirectionNode;
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
