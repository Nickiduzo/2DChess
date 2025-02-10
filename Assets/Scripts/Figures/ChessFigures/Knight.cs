using System.Collections.Generic;

public class Knight : Figure
{
    readonly List<Direction> directions = new List<Direction>()
    {
        Direction.KnightMove1(), Direction.KnightMove2(), Direction.KnightMove3(), Direction.KnightMove4(),
        Direction.KnightMove5(), Direction.KnightMove6(), Direction.KnightMove7(), Direction.KnightMove8()
    };
    public override List<Position> GetAvaibleMoves()
    {
        List<Position> avaibleMoves = new List<Position>();

        foreach (var direction in directions)
        {
            Position current = new Position(position.row + direction.RowDelta, position.col + direction.ColDelta);

            if (board.IsFreeCell(current))
            {
                avaibleMoves.Add(current);
            }
            else if (board.IsContainEnemy(current, isWhite))
            {
                avaibleMoves.Add(current);
            }
        }

        King ownKing = board.GetKing(isWhite);
        if (ownKing.IsCheck(ownKing.position))
        {
            List<Figure> figures = ownKing.GetAttackFigures();

            if (figures.Count == 1)
            {
                return board.GetCaptureOrBlockPositions(avaibleMoves, isWhite, figures[0]);
            }
            else
            {
                return new List<Position>();
            }
        }

        return GetSafeMoves(avaibleMoves);
    }
}
