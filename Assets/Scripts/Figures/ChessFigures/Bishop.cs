using System.Collections.Generic;

public class Bishop : Figure
{
    readonly List<Direction> directions = new List<Direction>()
    {
        Direction.UpLeft(), Direction.UpRight(), Direction.DownLeft(), Direction.DownRight()
    };

    public override List<Position> GetAvaibleMoves()
    {
        List<Position> avaibleMoves = new List<Position>();

        foreach ( var direction in directions )
        {
            Position current = position;

            while (true)
            {
                current = new Position(current.row + direction.RowDelta, current.col + direction.ColDelta);

                if(!board.IsFreeCell(current) && !board.IsContainEnemy(current, isWhite)) break;

                if(board.IsFreeCell(current))
                {
                    avaibleMoves.Add(current);
                }
                else
                {
                    if (board.IsContainEnemy(current, isWhite)) avaibleMoves.Add(current);
                    break;
                }
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
