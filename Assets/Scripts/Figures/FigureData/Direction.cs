public class Direction
{
    public int RowDelta { get; set; }
    public int ColDelta { get; set; }

    public Direction(int rowDelta, int colDelta)
    {
        RowDelta = rowDelta;
        ColDelta = colDelta;
    }

    public override bool Equals(object obj)
    {
        if (obj is Direction other)
        {
            return RowDelta == other.RowDelta && ColDelta == other.ColDelta;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return RowDelta * 10 + ColDelta;
    }

    public static Direction Up() => new Direction(1, 0);
    public static Direction Down() => new Direction(-1, 0);
    public static Direction Left() => new Direction(0, -1);
    public static Direction Right() => new Direction(0, 1);
    public static Direction UpLeft() => new Direction(1, -1);
    public static Direction UpRight() => new Direction(1, 1);
    public static Direction DownLeft() => new Direction(-1, -1);
    public static Direction DownRight() => new Direction(-1, 1);

    public static Direction KnightMove1() => new Direction(2, 1);
    public static Direction KnightMove2() => new Direction(2, -1);
    public static Direction KnightMove3() => new Direction(-2, 1);
    public static Direction KnightMove4() => new Direction(-2, -1);
    public static Direction KnightMove5() => new Direction(1, 2);
    public static Direction KnightMove6() => new Direction(-1, 2);
    public static Direction KnightMove7() => new Direction(1, -2);
    public static Direction KnightMove8() => new Direction(-1, -2);
}
