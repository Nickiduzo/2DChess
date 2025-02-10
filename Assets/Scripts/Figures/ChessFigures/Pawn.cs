using System;
using System.Collections.Generic;

public class Pawn : Figure
{
    private bool isDoubleMove;
    private int prevRow;
    private Position enPassaunt;

    public static event Action<Pawn> OnGetLastPosition;
    public override void MoveTo(Position newPosition)
    {
        if (board.cells[newPosition.row, newPosition.col].ContainFigure())
        {
            Figure temp = board.cells[newPosition.row, newPosition.col].GetComponentInChildren<Figure>();
            if (temp != null && temp.isWhite != isWhite)
            {
                board.DestroyFigure(temp);
            }
        }

        UpdatePosition(newPosition);

        if(newPosition.Equals(enPassaunt))
        {
            int enPasDirRow = isWhite ? -1 : 1;
            Figure temp = board.cells[position.row + enPasDirRow, position.col].GetComponentInChildren<Figure>();
            if (temp != null && temp.isWhite != isWhite)
            {
                board.DestroyFigure(temp);
            }
        }

        isDoubleMove = IsDoubleMove();

        if(IsLastPosition())
        {
            OnGetLastPosition?.Invoke(this);
        }
    }

    public override List<Position> GetAvaibleMoves()
    {
        List<Position> avaibleMoves = new List<Position>();

        prevRow = position.row;

        int direction = isWhite ? 1 : -1;

        Position forward = new Position(position.row + direction, position.col);

        if(board.IsFreeCell(forward))
        {
            avaibleMoves.Add(forward);
            if (isWhite && position.row == 1 || !isWhite && position.row == 6)
            {
                forward = new Position(forward.row + direction, position.col);
                if(board.IsFreeCell(forward))
                {
                    avaibleMoves.Add(forward);
                }
            }
        }

        Position attackLeft = new Position(position.row + direction, position.col - 1);
        Position attackRight = new Position(position.row + direction,position.col + 1);

        if(board.IsContainEnemy(attackLeft, isWhite))
        {
            avaibleMoves.Add(attackLeft);
        }

        if(board.IsContainEnemy(attackRight, isWhite))
        {
            avaibleMoves.Add(attackRight);
        }

        if(IsEnPassaunt())
        {
            print("Is en passaunt");
            Pawn enemy = board.GetLastFigure().GetComponent<Pawn>();

            int enDirRow = enemy.isWhite ? -1 : 1;

            enPassaunt = new Position(position.row + enDirRow, enemy.position.col);
            avaibleMoves.Add(enPassaunt);
        }
        else
        {
            enPassaunt = null;
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

    private bool IsEnPassaunt()
    {
        return IsPassauntCoordinate() && IsEnemyPawn();
    }

    private bool IsPassauntCoordinate()
    {
        return (isWhite && position.row == 4) || (!isWhite && position.row == 3);
    }

    private bool IsEnemyPawn()
    {
        if(board.GetLastFigure() is Pawn pawn)
        {
            return pawn.isDoubleMove && pawn.isWhite != isWhite;
        }
        return false;
    }

    private bool IsDoubleMove()
    {
        return prevRow == position.row + 2 || prevRow == position.row - 2;
    }

    private bool IsLastPosition()
    {
        return position.row == 0 || position.row == 7;
    }
}
