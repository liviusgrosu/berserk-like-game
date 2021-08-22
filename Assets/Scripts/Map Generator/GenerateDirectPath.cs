using System;
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

    public int MinDepthRange;
    public int MaxDepthRange;

    public GameObject PlatformModel, ConnectorModel, WallModel;

    private int startDirection, endDirection, leftDirection, rightDirection;

    Grid grid;
    GridDirectionNode gridDirectionParentNode, currentDirectionNode;
    GridDirectionNode gizmosCurrentDirectionNode, gizmosPreviousDirectionNode;

    private int[,] startSideRooms;
    private int[,] endSideRooms;

    private int[] startRoom, endRoom;
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
        startRoom = new int[] {startSideRooms[randNum, 0], startSideRooms[randNum, 1]};
        currentRoom = new int[2];
        startRoom.CopyTo(currentRoom, 0);

        gridDirectionParentNode = new GridDirectionNode(currentRoom, null);
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

    void CalculateAvailableDirectionsForPath()
    {
        currentDirectionNode = gridDirectionParentNode.children[0];
        while(currentDirectionNode.children.Count != 0)
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
            currentDirectionNode = currentDirectionNode.children[0];
        }
    }

    void RecalculatePathInformation(GridDirectionNode room)
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

        foreach(GridDirectionNode child in room.children)
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

                    // Remove all unavailable rooms
                    availableDirections.RemoveAll(item => item == -1);

                    if (availableDirections.Count == 0)
                    {
                        // No available rooms left then stop traversing path
                        break;
                    }

                    // Find a new random direction
                    randomDirection = availableDirections[UnityEngine.Random.Range(0, availableDirections.Count)];
                    // Add that direction and remove it from available directions of the room
                    availableDirections.RemoveAll(item => item == randomDirection);
                }
            }
        }
    }

    bool CheckIfCoordinateExist(GridDirectionNode room, int[] coordinate)
    {
        GridDirectionNode targetRoom = new GridDirectionNode(new int[] {-1, -1});

        CheckIfCoordinateExistLoop(room, targetRoom, coordinate);

        return targetRoom.coordinate.SequenceEqual(coordinate);
    }

    void CheckIfCoordinateExistLoop(GridDirectionNode room, GridDirectionNode targetRoom, int[] coordinate)
    {
        if (coordinate[0] == room.coordinate[0] && coordinate[1] == room.coordinate[1])
        {
            coordinate.CopyTo(targetRoom.coordinate, 0);
            return;
        }

        foreach(GridDirectionNode child in room.children)
        {
            CheckIfCoordinateExistLoop(child, targetRoom, coordinate);
        }
    }

    bool CheckRoomOutOfBound(int[] coordinate)
    {
        // Check if the room is out of bound
        return coordinate[0] < 0 || coordinate[1] >= grid.EdgeSize || coordinate[0] >= grid.EdgeSize || coordinate[1] < 0;
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
            endRoom = new int[] { currentRoom[0], currentRoom[1] };
            grid.GridData[currentRoom[0], currentRoom[1]] = 3;

            // TODO: add the final room to the tree here...
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

    void OnDrawGizmos()
    {
        if (grid == null || gridDirectionParentNode.children.Count == 0)
        {
            return;
        }

        // Debug: Draw the path lines
        // gizmosPreviousDirectionNode = gridDirectionParentNode;
        // Gizmos.color = Color.cyan;
        // DrawDirectionLines(gizmosPreviousDirectionNode);


        for (int i = 0; i < grid.EdgeSize; i++)
        {
            for (int j = 0; j < grid.EdgeSize; j++)
            {
                if (startRoom.SequenceEqual(new int[]{i, j}))
                {
                    // Check if room is the start
                    Gizmos.color = new Color(0f, 1f, 0f, 0.5f);
                    Gizmos.DrawCube(new Vector3(1.5f * i, 0, 1.5f * j), new Vector3(1, 1, 1));
                }

                if (endRoom.SequenceEqual(new int[]{i, j}))
                {
                    // Check if room is the start
                    Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
                    Gizmos.DrawCube(new Vector3(1.5f * i, 0, 1.5f * j), new Vector3(1, 1, 1));
                }
            }
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

    void TraverseAndDrawRoom(GridDirectionNode room)
    {
        // Render the platform
        GameObject platformInstaObj = Instantiate(PlatformModel, new Vector3(1.5f * room.coordinate[0], -0.5f, 1.5f * room.coordinate[1]), PlatformModel.transform.rotation);
        platformInstaObj.name = $"Platform - [{room.coordinate[0]},{room.coordinate[1]}]";

        // Render the connector
        foreach(int direction in room.adjacentRoomDirections)
        {
            GameObject platformConnectorInstaObj = Instantiate(ConnectorModel, Vector3.zero, ConnectorModel.transform.rotation);
            Vector3 platformPosition = platformInstaObj.transform.position;
            Vector3 newPos = Vector3.zero;
            switch (direction)
            {
                // North
                case 0:
                    newPos = new Vector3(platformPosition.x - 0.75f, -0.5f, platformPosition.z);
                    platformConnectorInstaObj.name = $"Connector - [{room.coordinate[0]},{room.coordinate[1]}] N";
                    break;
                // East
                case 1:
                    newPos = new Vector3(platformPosition.x, -0.5f, platformPosition.z + 0.75f);
                    platformConnectorInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    platformConnectorInstaObj.name = $"Connector - [{room.coordinate[0]},{room.coordinate[1]}] E";
                    break;
                // South
                case 2:
                    newPos = new Vector3(platformPosition.x + 0.75f, -0.5f, platformPosition.z);
                    platformConnectorInstaObj.name = $"Connector - [{room.coordinate[0]},{room.coordinate[1]}] S";
                    break;
                // West
                case 3:
                    newPos = new Vector3(platformPosition.x, -0.5f, platformPosition.z - 0.75f);
                    platformConnectorInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    platformConnectorInstaObj.name = $"Connector - [{room.coordinate[0]},{room.coordinate[1]}] W";
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
            Vector3 wallPosition = platformInstaObj.transform.position;
            Vector3 newPos = Vector3.zero;
            switch (direction)
            {
                // North
                case 0:
                    newPos = new Vector3(wallPosition.x - 0.4f, -0.5f, wallPosition.z);
                    wallInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    wallInstaObj.name = $"Wall - [{room.coordinate[0]},{room.coordinate[1]}] N";
                    break;
                // East
                case 1:
                    newPos = new Vector3(wallPosition.x, -0.5f, wallPosition.z + 0.4f);
                    
                    wallInstaObj.name = $"Wall - [{room.coordinate[0]},{room.coordinate[1]}] E";
                    break;
                // South
                case 2:
                    newPos = new Vector3(wallPosition.x + 0.4f, -0.5f, wallPosition.z);
                    wallInstaObj.transform.Rotate(0f, 90f, 0f, Space.World);
                    wallInstaObj.name = $"Wall - [{room.coordinate[0]},{room.coordinate[1]}] S";
                    break;
                // West
                case 3:
                    newPos = new Vector3(wallPosition.x, -0.5f, wallPosition.z - 0.4f);
                    wallInstaObj.name = $"Wall - [{room.coordinate[0]},{room.coordinate[1]}] W";
                    break;
            }
            wallInstaObj.transform.position = newPos;
        }

        foreach(GridDirectionNode child in room.children)
        {
            TraverseAndDrawRoom(child);
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

    int GetParentRoomDirection(GridDirectionNode room)
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
}
