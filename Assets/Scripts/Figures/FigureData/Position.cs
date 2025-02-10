[System.Serializable]
public class Position
{
    public int row;

    public int col;
    public Position(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    public void SetPosition(int row, int col)
    {
        if(ItIsWithinBounds(row, col))
        {
            this.row = row;
            this.col = col;
        }
    }

    public bool ItIsWithinBounds(int row, int col)
    {
        return row >= 0 && row <= 7 && col >= 0 && col <= 7;
    }

    public override bool Equals(object obj)
    {
        if(obj is Position pos)
        {
            return row == pos.row && col == pos.col;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return row.GetHashCode() ^ col.GetHashCode();
    }
}
