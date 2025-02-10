using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Figure : MonoBehaviour
{
    public static event Action<List<Position>, Figure> OnMouseClick;

    public bool isWhite;
    public Position position;
    public FigureType type;

    protected Board board;

    private Image figureImage;
    public void Initialize(Position startPosition, bool isWhite, Board board)
    {
        this.position = startPosition;
        this.isWhite = isWhite;
        this.board = board;
        
        figureImage = GetComponent<Image>();
    }

    public virtual void OnMouseDown()
    {
        OnMouseClick?.Invoke(GetAvaibleMoves(), this);
    }

    public virtual void MoveTo(Position newPosition)
    {
        if (board.cells[newPosition.row, newPosition.col].ContainFigure())
        {
            Figure temp = board.cells[newPosition.row, newPosition.col].GetComponentInChildren<Figure>();
            if(temp != null && temp.isWhite != isWhite)
            {
                board.DestroyFigure(temp);
            }
        }
        UpdatePosition(newPosition);
    }

    public void UpdatePosition(Position newPosition)
    {
        position.SetPosition(newPosition.row, newPosition.col);
        transform.SetPositionAndRotation(board.cells[position.row, position.col].transform.position, Quaternion.identity);
        transform.SetParent(board.cells[position.row, position.col].transform);

        board.AddMoveToStack(this);
    }

    public void Iteractable(bool switcher) => figureImage.raycastTarget = switcher;

    public virtual List<Position> GetAvaibleMoves()
    {
        return new List<Position>();
    }

    public virtual List<Position> GetSafeMoves(List<Position> rawMoves)
    {
        Position pinDirection;

        if (!IsPinned(out pinDirection))
        {
            return rawMoves;
        }

        List<Position> safeMoves = new List<Position>();
        foreach (Position move in rawMoves)
        {
            int dRow = move.row - position.row;
            int dCol = move.col - position.col;
            if (dRow * pinDirection.col == dCol * pinDirection.row)
            {
                safeMoves.Add(move);
            }
        }

        return safeMoves;
    }

    protected bool IsPinned(out Position pinDirection)
    {
        pinDirection = new Position(0, 0);
        King friendlyKing = board.GetKing(isWhite);
        if (friendlyKing == null)
            return false;

        int deltaRow = this.position.row - friendlyKing.position.row;
        int deltaCol = this.position.col - friendlyKing.position.col;

        if (deltaRow != 0 && deltaCol != 0 && Math.Abs(deltaRow) != Math.Abs(deltaCol))
        {
            return false;
        }

        int normRow = deltaRow == 0 ? 0 : Math.Sign(deltaRow);
        int normCol = deltaCol == 0 ? 0 : Math.Sign(deltaCol);
        pinDirection = new Position(normRow, normCol);

        Position pos = new Position(friendlyKing.position.row + pinDirection.row,
                                    friendlyKing.position.col + pinDirection.col);
        bool foundSelf = false;
        while (board.IsInsideBoard(pos))
        {
            Figure f = board.GetFigureAt(pos);
            if (f != null)
            {
                if (!foundSelf)
                {
                    if (f != this)
                        return false;
                    else
                        foundSelf = true;
                }
                else
                {
                    if (f.isWhite != isWhite)
                    {
                        if ((pinDirection.row == 0 || pinDirection.col == 0) &&
                            (f.type == FigureType.Rook || f.type == FigureType.Queen))
                        {
                            return true;
                        }
                        if ((pinDirection.row != 0 && pinDirection.col != 0) &&
                            (f.type == FigureType.Bishop || f.type == FigureType.Queen))
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            pos = new Position(pos.row + pinDirection.row, pos.col + pinDirection.col);
        }
        return false;
    }
}
