using System.Collections.Generic;

class GridDirectionNode
{
    public int[] coordinate;
    //public LinkedList<GridDirectionNode> children;
    public GridDirectionNode child;

    public GridDirectionNode(int[] coordinate)
    {
        this.coordinate = new int[coordinate.Length];
        coordinate.CopyTo(this.coordinate, 0);
    }

    public void AddChildRoom(int[] coordinate)
    {
        //children.AddFirst(new GridDirectionNode(coordinate));
        child = new GridDirectionNode(coordinate);
    }
}