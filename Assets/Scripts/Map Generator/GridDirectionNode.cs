using System.Collections.Generic;

class GridDirectionNode
{
    public int[] coordinate;
    public GridDirectionNode parent;
    public List<GridDirectionNode> children;
    public List<int> availableDirections;
    public List<int> adjacentRoomDirections;

    public GridDirectionNode(int[] coordinate, GridDirectionNode parent)
    {
        this.parent = parent;
        Init(coordinate);
    }

    public GridDirectionNode(int[] coordinate)
    {
        Init(coordinate);
    }

    void Init(int[] coordinate)
    {
        this.coordinate = new int[coordinate.Length];
        coordinate.CopyTo(this.coordinate, 0);

        children = new List<GridDirectionNode>();
        adjacentRoomDirections = new List<int>();
        availableDirections = new List<int>();
    }

    public void AddChildRoom(int[] coordinate)
    {
        children.Add(new GridDirectionNode(coordinate, this));
    }
}