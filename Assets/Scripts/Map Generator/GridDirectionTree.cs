using System.Collections.Generic;

class GridDirectionTree
{
    public int[] coordinate;
    public LinkedList<GridDirectionTree> children;

    public GridDirectionTree(int[] coordinate)
    {
        this.coordinate = new int[coordinate.Length];
        coordinate.CopyTo(this.coordinate, 0);
    }

    public void AddChildRoom(int[] coordinate)
    {
        children.AddFirst(new GridDirectionTree(coordinate));
    }
}