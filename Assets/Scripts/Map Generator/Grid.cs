using System;

public class Grid
{
    public Grid(int minRange, int maxRange)
    {
        Random rand = new Random();

        EdgesCount = 4;
        EdgeSize = rand.Next(minRange, maxRange);
        GridData = new int[EdgeSize, EdgeSize];  
    }

    public int[,] GridData;

    // Temp: will get this from template data
    public int EdgesCount;
    public int EdgeSize;
}
