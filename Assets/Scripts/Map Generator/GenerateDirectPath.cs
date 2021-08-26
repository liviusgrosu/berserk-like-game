﻿using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [Tooltip("Occurance of optional rooms")]
    [Range(0, 1)]
    public float OptionalRoomCoverage = 0.5f;

    public int MinDepthRange;
    public int MaxDepthRange;

    public GameObject PlatformModel, ConnectorModel, WallModel;

    public float DebugScaleFactor = 1f;

    private int startDirection, endDirection, leftDirection, rightDirection;

    Grid grid;
    GridRoom gridDirectionParentNode, currentDirectionNode;

    private int[,] startSideRooms;
    private int[,] endSideRooms;

    private int[] startRoom, endRoom;
    private int[] currentRoom;

    void Awake()
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

        // Get the rooms that exist on the start and end edges
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

        // Pick a random start room
        int randNum = UnityEngine.Random.Range(0, grid.EdgeSize);
        startRoom = new int[] {startSideRooms[randNum, 0], startSideRooms[randNum, 1]};
        currentRoom = new int[2];
        startRoom.CopyTo(currentRoom, 0);

        gridDirectionParentNode = new GridRoom(currentRoom, null);
        currentDirectionNode = gridDirectionParentNode;

        // Create the main path
        TraversePath(startDirection);
        // Store the available paths around each room in the main path
        CalculateAvailableDirectionsForPath();
        // Calculate optional paths
        TraverseOptionalPath();
        // Recalculate available directions and connecting rooms
        RecalculatePathInformation(gridDirectionParentNode);
        RenderPath();
    }

    void TraversePath(int previousDirection)
    {
        grid.GridData[currentRoom[0], currentRoom[1]] = 1;

        if (CheckIfRoomOnEdge(endSideRooms, currentRoom[0], currentRoom[1]))
        {
            // Stop when a tile on the end side is traversed
            endRoom = new int[] { currentRoom[0], currentRoom[1] };
            grid.GridData[currentRoom[0], currentRoom[1]] = 3;
            return;
        }

        if (startRoom.SequenceEqual(currentRoom))
        {
            grid.GridData[currentRoom[0], currentRoom[1]] = 2;
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

    void CalculateAvailableDirectionsForPath()
    {
        currentDirectionNode = gridDirectionParentNode.children[0];
        while(currentDirectionNode.children.Count != 0)
        {
            CalculateRoomsAvaibleDirections(currentDirectionNode);
            currentDirectionNode = currentDirectionNode.children[0];
        }
    }

    void TraverseOptionalPath()
    {
        List<GridRoom> rooms = new List<GridRoom>();

        // Add main path the list
        currentDirectionNode = gridDirectionParentNode.children[0];
        while (currentDirectionNode.children.Count != 0)
        {
            rooms.Add(currentDirectionNode);
            currentDirectionNode = currentDirectionNode.children[0];
        }

        // Reset pointer to parent node
        currentDirectionNode = gridDirectionParentNode.children[0];
        
        int currentRoomCount = (int)(rooms.Count * (OptionalRoomCoverage) - 1);
        for (int roomIdx = 0; roomIdx < currentRoomCount; roomIdx++)
        {
            // Get a random room
            int randomRoomIdx = UnityEngine.Random.Range(1, rooms.Count - 2);
            GridRoom currentRoom = rooms[randomRoomIdx];
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

                    // Update surrounding rooms to remove its available directions
                    UpdateSurroundingAvailableDirections(coordinate);

                    currentRoom.AddChildRoom(coordinate);
                    // Traverse that newly created child
                    currentRoom = currentRoom.children[currentRoom.children.Count - 1];
                    rooms.Add(currentRoom);

                    // Calculate the rooms available directions
                    CalculateRoomsAvaibleDirections(currentRoom);

                    if (currentRoom.availableDirections.Count == 0)
                    {
                        // No available rooms left then stop traversing path
                        break;
                    }

                    // Find a new random direction
                    randomDirection = currentRoom.availableDirections[UnityEngine.Random.Range(0, currentRoom.availableDirections.Count)];
                    // Add that direction and remove it from available directions of the room
                    currentRoom.availableDirections.RemoveAll(item => item == randomDirection);
                }
            }
        }
    }
    void RecalculatePathInformation(GridRoom room)
    {
        // Get coordinates of adjacent rooms
        int[] northCoordinate = new int[] {room.coordinate[0] - 1, room.coordinate[1] };
        int[] eastCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] + 1 };
        int[] southCoordinate = new int[] {room.coordinate[0] + 1, room.coordinate[1] };
        int[] westCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] - 1 };

        List<int> availableDirections = new List<int> {0, 1, 2, 3};

        // Check any adjacent rooms that are free
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

        // Update the available directions in the current room
        room.availableDirections = availableDirections;

        // Check connecting rooms
        List<int> adjacentRoomDirections = new List<int>();

        foreach(GridRoom child in room.children)
        {
            if (child.coordinate.SequenceEqual(northCoordinate))
            {
                adjacentRoomDirections.Add(0);
            }
            else if (child.coordinate.SequenceEqual(eastCoordinate))
            {
                adjacentRoomDirections.Add(1);
            }
            else if (child.coordinate.SequenceEqual(southCoordinate))
            {
                adjacentRoomDirections.Add(2);
            }
            else if (child.coordinate.SequenceEqual(westCoordinate))
            {
                adjacentRoomDirections.Add(3);
            }
            // Update the connecting room list
            room.adjacentRoomDirections = adjacentRoomDirections;
            
            // Check the children too
            RecalculatePathInformation(child);
        }
    }
    void RenderPath()
    {
        if (grid == null || gridDirectionParentNode.children.Count == 0)
        {
            return;
        }

        currentDirectionNode = gridDirectionParentNode;
        TraverseAndDrawRoom(currentDirectionNode);
    }

    void CalculateRoomsAvaibleDirections(GridRoom room)
    {
        room.availableDirections = new List<int> {0, 1, 2, 3};
        // Get coordinates around the path
        int[] northCoordinate = new int[] {room.coordinate[0] - 1, room.coordinate[1] };
        int[] eastCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] + 1 };
        int[] southCoordinate = new int[] {room.coordinate[0] + 1, room.coordinate[1] };
        int[] westCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] - 1 };

        // Check north
        if (CheckIfCoordinateExist(gridDirectionParentNode, northCoordinate) || CheckRoomOutOfBound(northCoordinate))
        {
            room.availableDirections[0] = -1;
        }

        // Check east
        if (CheckIfCoordinateExist(gridDirectionParentNode, eastCoordinate) || CheckRoomOutOfBound(eastCoordinate))
        {
            room.availableDirections[1] = -1;
        }

        // Check south
        if (CheckIfCoordinateExist(gridDirectionParentNode, southCoordinate) || CheckRoomOutOfBound(southCoordinate))
        {
            room.availableDirections[2] = -1;
        }

        // Check west
        if (CheckIfCoordinateExist(gridDirectionParentNode, westCoordinate) || CheckRoomOutOfBound(westCoordinate))
        {
            room.availableDirections[3] = -1;
        }
        // Update the available directions list
        room.availableDirections.RemoveAll(item => item == -1);
    }

    bool CheckIfCoordinateExist(GridRoom room, int[] coordinate)
    {
        GridRoom targetRoom = new GridRoom(new int[] {-1, -1});

        CheckIfCoordinateExistLoop(room, targetRoom, coordinate);

        return targetRoom.coordinate.SequenceEqual(coordinate);
    }

    void CheckIfCoordinateExistLoop(GridRoom room, GridRoom targetRoom, int[] coordinate)
    {
        if (coordinate[0] == room.coordinate[0] && coordinate[1] == room.coordinate[1])
        {
            coordinate.CopyTo(targetRoom.coordinate, 0);
            return;
        }

        foreach(GridRoom child in room.children)
        {
            CheckIfCoordinateExistLoop(child, targetRoom, coordinate);
        }
    }

    GridRoom GetRoom(GridRoom currentRoom, int[] targetCoordinate)
    {
        if(currentRoom.coordinate.SequenceEqual(targetCoordinate))
        {
            return currentRoom;
        }

        foreach(GridRoom childRoom in currentRoom.children)
        {
            GridRoom result = GetRoom(childRoom, targetCoordinate);
            if (result != null)
            {
                return result;
            }
            
        }

        return null;
    }

    bool CheckRoomOutOfBound(int[] coordinate)
    {
        // Check if the room is out of bound
        return coordinate[0] < 0 || coordinate[1] >= grid.EdgeSize || coordinate[0] >= grid.EdgeSize || coordinate[1] < 0;
    }

    void FillEdgeTiles(int idx, int startRoomY, int startRoomX, int endRoomY, int endRoomX)
    {
        // Store the room coordinates on the start to a list
        startSideRooms[idx, 0] = startRoomY;
        startSideRooms[idx, 1] = startRoomX;

        // Store the room coordinates on the end to a list
        endSideRooms[idx, 0] = endRoomY;
        endSideRooms[idx, 1] = endRoomX;
    }

    void OnDrawGizmos()
    {
        if (grid == null || gridDirectionParentNode.children.Count == 0)
        {
            return;
        }

        for (int i = 0; i < grid.EdgeSize; i++)
        {
            for (int j = 0; j < grid.EdgeSize; j++)
            {
                if (startRoom.SequenceEqual(new int[]{i, j}))
                {
                    // Check if room is the start
                    Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                    Gizmos.DrawCube(new Vector3(DebugScaleFactor * 1.5f * i, 0, DebugScaleFactor * 1.5f * j), new Vector3(1, 1, 1));
                }

                if (endRoom.SequenceEqual(new int[]{i, j}))
                {
                    // Check if room is the start
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(new Vector3(DebugScaleFactor * 1.5f * i, 0, DebugScaleFactor * 1.5f * j), new Vector3(1, 1, 1));
                }
            }
        }
    }

    void TraverseAndDrawRoom(GridRoom room)
    {
        // Render the platform
        GameObject platformInstaObj = Instantiate(PlatformModel, new Vector3(DebugScaleFactor * 1.5f * room.coordinate[0], -0.5f, DebugScaleFactor * 1.5f * room.coordinate[1]), PlatformModel.transform.rotation);
        platformInstaObj.transform.localScale = Vector3.Scale(platformInstaObj.transform.localScale, new Vector3(DebugScaleFactor, DebugScaleFactor, DebugScaleFactor));
        platformInstaObj.name = $"Platform - [{room.coordinate[0]},{room.coordinate[1]}]";

        // Render the connector
        foreach(int direction in room.adjacentRoomDirections)
        {
            GameObject platformConnectorInstaObj = Instantiate(ConnectorModel, Vector3.zero, ConnectorModel.transform.rotation);
            platformConnectorInstaObj.transform.localScale = Vector3.Scale(platformConnectorInstaObj.transform.localScale, new Vector3(DebugScaleFactor, DebugScaleFactor, DebugScaleFactor));
            Vector3 platformPosition = platformInstaObj.transform.position;
            Vector3 newPos = Vector3.zero;
            string baseName = $"Connector - [{room.coordinate[0]},{room.coordinate[1]}]";
            switch (direction)
            {
                // North
                case 0:
                    newPos = new Vector3(platformPosition.x - (DebugScaleFactor * 0.75f), -0.5f, platformPosition.z);
                    platformConnectorInstaObj.name = $"{baseName} N";
                    break;
                // East
                case 1:
                    newPos = new Vector3(platformPosition.x, -0.5f, platformPosition.z + (DebugScaleFactor * 0.75f));
                    platformConnectorInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    platformConnectorInstaObj.name = $"{baseName} E";
                    break;
                // South
                case 2:
                    newPos = new Vector3(platformPosition.x + (DebugScaleFactor * 0.75f), -0.5f, platformPosition.z);
                    platformConnectorInstaObj.name = $"{baseName} S";
                    break;
                // West
                case 3:
                    newPos = new Vector3(platformPosition.x, -0.5f, platformPosition.z - (DebugScaleFactor * 0.75f));
                    platformConnectorInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    platformConnectorInstaObj.name = $"{baseName} W";
                    break;
            }
            platformConnectorInstaObj.transform.position = newPos;
        }

        // Render the walls
        List<int> wallDirections = new List<int> {0, 1, 2, 3};
        wallDirections = wallDirections.Except(room.adjacentRoomDirections).ToList();

        // Remove the parent room from the list
        if (room.parent != null)
        {
            wallDirections.Remove(GetParentRoomDirection(room));
        }

        foreach(int direction in wallDirections)
        {
            GameObject wallInstaObj = Instantiate(WallModel, Vector3.zero, ConnectorModel.transform.rotation);
            wallInstaObj.transform.localScale = Vector3.Scale(wallInstaObj.transform.localScale, new Vector3(DebugScaleFactor, DebugScaleFactor, DebugScaleFactor));
            Vector3 wallPosition = platformInstaObj.transform.position;
            Vector3 newPos = Vector3.zero;
            string baseName = $"Wall - [{room.coordinate[0]},{room.coordinate[1]}]";
            switch (direction)
            {
                // North
                case 0:
                    newPos = new Vector3(wallPosition.x - (DebugScaleFactor * 0.4f), -0.5f, wallPosition.z);
                    wallInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    wallInstaObj.name = $"{baseName} N";
                    break;
                // East
                case 1:
                    newPos = new Vector3(wallPosition.x, -0.5f, wallPosition.z + (DebugScaleFactor * 0.4f));
                    wallInstaObj.name = $"{baseName} E";
                    break;
                // South
                case 2:
                    newPos = new Vector3(wallPosition.x + (DebugScaleFactor * 0.4f), -0.5f, wallPosition.z);
                    wallInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    wallInstaObj.name = $"{baseName} S";
                    break;
                // West
                case 3:
                    newPos = new Vector3(wallPosition.x, -0.5f, wallPosition.z - (DebugScaleFactor * 0.4f));
                    wallInstaObj.name = $"{baseName} W";
                    break;
            }
            wallInstaObj.transform.position = newPos;
        }

        foreach(GridRoom child in room.children)
        {
            // Traverse the children rooms
            TraverseAndDrawRoom(child);
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

    int GetParentRoomDirection(GridRoom room)
    {
        int[] northCoordinate = new int[] {room.coordinate[0] - 1, room.coordinate[1] };
        int[] eastCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] + 1 };
        int[] southCoordinate = new int[] {room.coordinate[0] + 1, room.coordinate[1] };
        int[] westCoordinate =  new int[] {room.coordinate[0], room.coordinate[1] - 1 };

        if (room.parent.coordinate.SequenceEqual(northCoordinate))
        {
            return 0;
        }
        if (room.parent.coordinate.SequenceEqual(eastCoordinate))
        {
            return 1;
        }
        if (room.parent.coordinate.SequenceEqual(southCoordinate))
        {
            return 2;
        }
        if (room.parent.coordinate.SequenceEqual(westCoordinate))
        {
            return 3;
        }
        return -1;
    }

    void UpdateSurroundingAvailableDirections(int[] currentCoordinate)
    {
        int[] northCoordinate = new int[] {currentCoordinate[0] - 1, currentCoordinate[1] };
        int[] eastCoordinate =  new int[] {currentCoordinate[0], currentCoordinate[1] + 1 };
        int[] southCoordinate = new int[] {currentCoordinate[0] + 1, currentCoordinate[1] };
        int[] westCoordinate =  new int[] {currentCoordinate[0], currentCoordinate[1] - 1 };

        GridRoom northDirectionRoom = GetRoom(gridDirectionParentNode, northCoordinate);
        GridRoom eastDirectionRoom = GetRoom(gridDirectionParentNode, eastCoordinate);
        GridRoom southDirectionRoom = GetRoom(gridDirectionParentNode, southCoordinate);
        GridRoom westDirectionRoom = GetRoom(gridDirectionParentNode, westCoordinate);

        if (northDirectionRoom != null)
        {
            northDirectionRoom.availableDirections.Remove(2);
        }

        if (eastDirectionRoom != null)
        {
            eastDirectionRoom.availableDirections.Remove(3);
        }

        if (southDirectionRoom != null)
        {
            southDirectionRoom.availableDirections.Remove(0);
        }

        if (westDirectionRoom != null)
        {
            westDirectionRoom.availableDirections.Remove(1);
        }
    }

    public Vector3 GetStartRoomPosition()
    {
        return new Vector3(DebugScaleFactor * startRoom[0] * 1.5f, 0f, DebugScaleFactor * startRoom[1] * 1.5f);
    }
}
