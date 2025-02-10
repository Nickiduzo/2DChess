using System.Collections.Generic;

public class Rook : Figure
{
    public int amountOfMoves;

    List<Direction> directions = new List<Direction>()
    {
        Direction.Up(), Direction.Down(), Direction.Left(), Direction.Right(),
    };

    private void Start()
    {
        amountOfMoves = 0;

        King.CastleOportunity += HandlerRook;
    }

    private void HandlerRook(King king)
    {
        if(king.isWhite == isWhite && amountOfMoves == 0)
        {
            Iteractable(false);
        }
    }

    public override void MoveTo(Position newPosition)
    {
        base.MoveTo(newPosition);
        amountOfMoves++;
    }

    public override List<Position> GetAvaibleMoves()
    {
        List<Position> avaibleMoves = new List<Position>();

        foreach ( var direction in directions )
        {
            Position current = position;

            while (true)
            {
                current = new Position(current.row + direction.RowDelta, current.col + direction.ColDelta);

                if (!board.IsFreeCell(current) && !board.IsContainEnemy(current, isWhite)) break;

                if (board.IsFreeCell(current))
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

    private void OnDisable()
    {
        King.CastleOportunity -= HandlerRook;
    }
}
