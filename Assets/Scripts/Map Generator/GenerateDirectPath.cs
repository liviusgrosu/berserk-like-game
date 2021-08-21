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

    public int MinDepthRange;
    public int MaxDepthRange;

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
        // TODO: Calculate optional paths
        TraverseOptionalPath();
        int k = 0;
    }

    void CalculateAvailableDirectionsForPath()
    {
        currentDirectionNode = gridDirectionParentNode.children[0];
        while(currentDirectionNode.children.Count != 0)
        {
            // TODO: Loop through all the children
            // If you dont then its some rooms will have outdated available directions when the optional rooms are added
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
            currentDirectionNode = currentDirectionNode.children[0];
        }
    }

    void TraverseOptionalPath()
    {
        List<GridDirectionNode> rooms = new List<GridDirectionNode>();

        // Add main path the list
        currentDirectionNode = gridDirectionParentNode.children[0];
        while (currentDirectionNode.children.Count != 0)
        {
            rooms.Add(currentDirectionNode);
            currentDirectionNode = currentDirectionNode.children[0];
        }

        // Reset pointer to parent node
        currentDirectionNode = gridDirectionParentNode.children[0];

        int currentRoomCount = rooms.Count / 2;
        for (int roomIdx = 0; roomIdx < currentRoomCount; roomIdx++)
        {
            // Get a random room
            int randomRoomIdx = UnityEngine.Random.Range(1, rooms.Count - 1);
            GridDirectionNode currentRoom = rooms[randomRoomIdx];
            // Remove the main path room from the list
            rooms.RemoveAt(randomRoomIdx);
            List<int> availableDirections = currentRoom.availableDirections;
            if (availableDirections.Count != 0)
            {
                // Choose a random available direction
                int randomDirection = availableDirections[UnityEngine.Random.Range(0, availableDirections.Count)];
                // Add that direction and remove it from available directions of the room
                availableDirections.RemoveAll(item => item == randomDirection);

                int randomDepth = UnityEngine.Random.Range(MinDepthRange, MaxDepthRange);

                // Traverse depth
                for (int depthIdx = 0; depthIdx < randomDepth; depthIdx++)
                {
                    // Get coordinate of new room
                    int[] coordinate = new int[2];
                    currentRoom.coordinate.CopyTo(coordinate, 0);

                    switch (randomDirection)
                    {
                        case 0:
                            coordinate[0] -= 1;
                            break;
                        case 1:
                            coordinate[1] += 1;
                            break;
                        case 2:
                            coordinate[0] += 1;
                            break;
                        case 3:
                            coordinate[1] -= 1;
                            break;
                        default:
                            break;
                    }

                    grid.GridData[coordinate[0], coordinate[1]] = 1;

                    currentRoom.AddChildRoom(coordinate);
                    // Traverse that newly created child
                    currentRoom = currentRoom.children[currentRoom.children.Count - 1];
                    rooms.Add(currentRoom);
                    // TODO: maybe instantiate the list in the class as this
                    currentRoom.availableDirections = new List<int> {0, 1, 2, 3};
                    availableDirections = currentRoom.availableDirections;

                    // Get coordinates around the path
                    int[] northCoordinate = new int[] {currentRoom.coordinate[0] - 1, currentRoom.coordinate[1] };
                    int[] eastCoordinate =  new int[] {currentRoom.coordinate[0], currentRoom.coordinate[1] + 1 };
                    int[] southCoordinate = new int[] {currentRoom.coordinate[0] + 1, currentRoom.coordinate[1] };
                    int[] westCoordinate =  new int[] {currentRoom.coordinate[0], currentRoom.coordinate[1] - 1 };

                    // Remove rooms that are adjacent
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

                    if (availableDirections.Count == 0)
                    {
                        // No available rooms left then stop traversing path
                        break;
                    }

                    // Get a random room
                    randomDirection = availableDirections[UnityEngine.Random.Range(0, availableDirections.Count)];
                    // Add that direction and remove it from available directions of the room
                    availableDirections.RemoveAll(item => item == randomDirection);
                }
            }
        }
    }

    bool CheckIfCoordinateExist(GridDirectionNode room, int[] coordinate)
    {
        if(room.children.Count == 0)
        {
            return false;
        }
        if (coordinate[0] == room.coordinate[0] && coordinate[1] == room.coordinate[1])
        {
            return true;
        }

        foreach(GridDirectionNode child in room.children)
        {
            return CheckIfCoordinateExist(child, coordinate);
        }
        return false;
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

        if (pathCandidates.Count == 0)
        {
            // If theres no more availble paths then the path is finished
            return;
        }

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
        currentDirectionNode = currentDirectionNode.children[0];
        TraversePath(newDirection);
    }

    void OnDrawGizmos()
    {
        if (grid == null || gridDirectionParentNode.children.Count == 0)
        {
            return;
        }

        gizmosPreviousDirectionNode = gridDirectionParentNode;
        gizmosCurrentDirectionNode = gridDirectionParentNode.children[0];
 
        Gizmos.color = Color.cyan;

        DrawDirectionLines(gizmosPreviousDirectionNode);


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

    void DrawDirectionLines(GridDirectionNode currentNode)
    {
        foreach(GridDirectionNode nextRoom in currentNode.children)
        {
            Vector3 currentRoomPos = new Vector3(currentNode.coordinate[0] * 1.5f, 0f, currentNode.coordinate[1] * 1.5f);
            Vector3 nextRoomPos = new Vector3(nextRoom.coordinate[0] * 1.5f, 0f, nextRoom.coordinate[1] * 1.5f);

            Gizmos.DrawLine(currentRoomPos, nextRoomPos);
            DrawDirectionLines(nextRoom);
        }
        return;
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
