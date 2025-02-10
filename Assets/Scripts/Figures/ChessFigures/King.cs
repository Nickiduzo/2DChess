using System;
using System.Collections.Generic;

public class King : Figure
{
    public static event Action<King> CastleOportunity;
    public static event Action<bool> OnCheckMate;
    public static event Action OnStalemate;

    public int amountOfMoves;

    private Position longCastling;
    private Position shortCastling;

    private readonly List<Direction> directions = new()
    {
        Direction.UpLeft(), Direction.UpRight(), Direction.DownLeft(), Direction.DownRight(),
        Direction.Up(), Direction.Down(), Direction.Right(), Direction.Left()
    };
    
    private readonly List<Direction> knightMoves = new()
    {
        Direction.KnightMove1(), Direction.KnightMove2(), Direction.KnightMove3(), Direction.KnightMove4(),
        Direction.KnightMove5(), Direction.KnightMove6(), Direction.KnightMove7(), Direction.KnightMove8()
    };
    
    private readonly List<Direction> straightDirections = new()
    {
        Direction.Up(), Direction.Down(), Direction.Left(), Direction.Right()
    };
    
    private readonly List<Direction> diagonalDirections = new()
    {
        Direction.UpLeft(), Direction.UpRight(), Direction.DownLeft(), Direction.DownRight()
    };

    private Position checkPosition;

    private void Start()
    {
        amountOfMoves = 0;
    }

    private void Update()
    {
        if(IsCheckMate())
        {
            board.HighLightKing(position);
            OnCheckMate?.Invoke(isWhite);
        }

        if(IsStalemate())
        {
            OnStalemate?.Invoke();
        }
    }

    public override void OnMouseDown()
    {
        base.OnMouseDown();
        if(amountOfMoves == 0)
        {
            CastleOportunity?.Invoke(this);
        }
    }

    public override void MoveTo(Position newPosition)
    {
        Figure temp = board.cells[newPosition.row, newPosition.col].GetComponentInChildren<Figure>();
        
        if(temp != null)
        {
            if (TryCaptureFigure(temp, newPosition)) return;
            if (TryCastling(temp, ref newPosition)) return;
        }
    
        amountOfMoves++;
        UpdatePosition(newPosition);
    }

    private bool TryCaptureFigure(Figure target, Position newPosition)
    {
        if (target.isWhite != isWhite)
        {
            board.DestroyFigure(target);
            UpdatePosition(newPosition);
        }
        return false;
    }

    private bool TryCastling(Figure rook, ref Position kingNewPosition)
    {
        if(rook is Rook && rook.isWhite == isWhite)
        {
            if(kingNewPosition.Equals(longCastling))
            {
                kingNewPosition = new Position(kingNewPosition.row, 2);
                rook.MoveTo(new Position(kingNewPosition.row, 3));
            }
            else if (kingNewPosition.Equals(shortCastling))
            {
                kingNewPosition = new Position(kingNewPosition.row, 6);
                rook.MoveTo(new Position(kingNewPosition.row, 5));
            }
            else
            {
                return false;
            }

            UpdatePosition(kingNewPosition);
            return true;
        }
        return false;
    }

    public override List<Position> GetAvaibleMoves()
    {
        List<Position> avaibleMoves = new List<Position>();
        HashSet<Direction> blocked = GetBlockedDirections(position);

        foreach (var direction in directions)
        {
            Position current = new Position(position.row + direction.RowDelta, position.col + direction.ColDelta);

            bool canCapture = board.IsContainEnemy(current, isWhite) && !IsCheck(current);

            if(blocked.Contains(direction) && !canCapture)
            {
                continue;
            }

            if (board.IsFreeCell(current) && !IsCheck(current))
            {
                avaibleMoves.Add(current);
            }
            else if (canCapture)
            {
                avaibleMoves.Add(current);
            }
        }

        if(!IsCheck(position))
        {
            if (IsLongCastling())
            {
                longCastling = new Position(position.row, 0);
                avaibleMoves.Add(longCastling);
            }
            else
            {
                longCastling = null;
            }

            if (IsShortCastling())
            {
                shortCastling = new Position(position.row, 7);
                avaibleMoves.Add(shortCastling);
            }
            else
            {
                shortCastling = null;
            }
        }

        return avaibleMoves;
    }

    private bool IsLongCastling()
    {
        Rook rook = board.cells[position.row, 0].GetComponentInChildren<Rook>();

        return rook != null && rook.isWhite == isWhite && rook.amountOfMoves == 0 && amountOfMoves == 0 && IsFreeLong(rook.position);
    }

    private bool IsShortCastling()
    {
        Rook rook = board.cells[position.row, 7].GetComponentInChildren<Rook>();

        return rook != null && rook.isWhite == isWhite && rook.amountOfMoves == 0 && amountOfMoves == 0 && IsFreeShort(rook.position);
    }

    private bool IsFreeLong(Position rookPosition)
    {
        int rowCastling = position.row;
        for (int col = position.col - 1; col > rookPosition.col; col--)
        {
            if (!board.IsFreeCell(new Position(rowCastling, col)) || IsCheck(new Position(rowCastling, col)))
            {
                return false;
            }
        }
        return true;
    }

    private bool IsFreeShort(Position rookPosition)
    {
        int rowCaslting = position.row;
        for(int col = position.col + 1; col < rookPosition.col; col++)
        {
            if(!board.IsFreeCell(new Position(rowCaslting, col)) || IsCheck(new Position(rowCaslting, col)))
            {
                return false;
            }
        }
        return true;
    }

    #region Check

    private bool IsStalemate()
    {
        if(IsCheck(position))
        {
            return false;
        }

        List<Figure> figures = isWhite ? board.whiteFigures : board.blackFigures;

        foreach (Figure figure in figures)
        {
            if(figure.GetAvaibleMoves().Count > 0)
            {
                return false;
            }
        }

        return true;
    }

    public bool IsCheck(Position pos)
    {
        if (IsUnderPawnAttack(pos)) return true;

        if (IsUnderLineAttack(pos)) return true;

        if(IsUnderDiagonalAttack(pos)) return true;

        if(IsUnderKnightAttack(pos)) return true;

        if(IsUnderKingAttack(pos)) return true;

        return false;
    }

    public bool IsCheckMate()
    {
        if (!IsCheck(position))
        {
            return false;
        }

        if (GetAvaibleMoves().Count > 0) return false;

        var attackFigures = GetAttackFigures();

        if(attackFigures.Count == 1)
        {
            if (CanSideCapture(attackFigures[0]))
            {
                return false;
            }

            if (CanSideBlock(attackFigures[0]))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsUnderPawnAttack(Position pos)
    {
        int direction = isWhite ? 1 : -1; 

        Position leftAttack = new Position(pos.row + direction, pos.col - 1);
        Position rightAttack = new Position(pos.row + direction, pos.col + 1);

        return IsEnemyPawn(leftAttack) || IsEnemyPawn(rightAttack);
    }

    private bool IsEnemyPawn(Position pos)
    {
        Figure piece = board.GetFigureAt(pos);
        return piece is Pawn && piece.isWhite != isWhite;
    }

    private bool IsUnderLineAttack(Position pos)
    {
        foreach (Direction dir in straightDirections)
        {
            Position checkPos = pos;

            while (true)
            {
                checkPos = new Position(checkPos.row + dir.RowDelta, checkPos.col + dir.ColDelta);

                if (!board.IsInsideBoard(checkPos))
                    break;
                
                Figure piece = board.GetFigureAt(checkPos);

                if (piece != null)
                {
                    if (piece.isWhite != isWhite && (piece is Rook || piece is Queen))
                    {
                        return true;
                    }
                    break;
                }
            }
        }

        return false;
    }

    private bool IsUnderDiagonalAttack(Position pos)
    {
        foreach (Direction dir in diagonalDirections)
        {
            Position checkPos = pos;

            while (true)
            {
                checkPos = new Position(checkPos.row + dir.RowDelta, checkPos.col + dir.ColDelta);

                if (!board.IsInsideBoard(checkPos))
                    break;

                Figure piece = board.GetFigureAt(checkPos);

                if (piece != null)
                {
                    if (piece.isWhite != isWhite && (piece is Bishop || piece is Queen))
                    {
                        return true;
                    }
                    break;
                }
            }
        }

        return false;
    }

    private bool IsUnderKnightAttack(Position pos)
    {
        foreach (Direction move in knightMoves)
        {
            Position checkPos = new Position(pos.row + move.RowDelta, pos.col + move.ColDelta);
            Figure piece = board.GetFigureAt(checkPos);

            if (piece is Knight && piece.isWhite != isWhite)
            {
                return true; 
            }
        }
        return false;
    }

    private bool IsUnderKingAttack(Position pos)
    {
        foreach (Direction direction in directions)
        {
            Position checkPos = new Position(pos.row + direction.RowDelta, pos.col + direction.ColDelta);
            Figure piece = board.GetFigureAt(checkPos);

            if(piece != null && piece is King && piece.isWhite != isWhite)
            {
                return true;
            }
        }
        return false;
    }

    private HashSet<Direction> GetBlockedDirections(Position pos)
    {
        HashSet<Direction> blocked = new HashSet<Direction>();

        List<Direction> horizontals = new List<Direction>()
        {
             Direction.Left(), Direction.Right(), Direction.Down(), Direction.Up()
        };

        foreach (Direction direction in horizontals)
        {
            Position checkPos = pos;

            if(!blocked.Contains(direction))
            {
                while (true)
                {
                    checkPos = new Position(checkPos.row + direction.RowDelta, checkPos.col + direction.ColDelta);

                    if (!board.IsInsideBoard(checkPos))
                        break;

                    Figure piece = board.GetFigureAt(checkPos);

                    if(piece != null)
                    {
                        if(piece.isWhite != isWhite && (piece is Rook || piece is Queen))
                        {
                            Direction opposite = new Direction(-direction.RowDelta, -direction.ColDelta);
                            blocked.Add(direction);
                            blocked.Add(opposite);
                        }
                    }
                }
            }
        }

        List<Direction> verticals = new List<Direction>()
        {
            Direction.UpLeft(), Direction.DownRight(), Direction.UpRight(), Direction.DownLeft()
        };

        foreach (Direction direction in verticals)
        {
            Position checkPos = pos;

            if(!blocked.Contains(direction))
            {
                while (true)
                {
                    checkPos = new Position(checkPos.row + direction.RowDelta, checkPos.col + direction.ColDelta);

                    if (!board.IsInsideBoard(checkPos))
                        break;

                    Figure piece = board.GetFigureAt(checkPos);

                    if(piece != null)
                    {
                        if (piece.isWhite != isWhite && (piece is Bishop || piece is Queen))
                        {
                            Direction opposite = new Direction(-direction.RowDelta, -direction.ColDelta);
                            blocked.Add(direction);
                            blocked.Add(opposite);
                        }
                    }
                }
            }
        }

        return blocked;
    }

    #endregion

    public List<Figure> GetAttackFigures()
    {
        List<Figure> attackFigures = new List<Figure>();

        Position current = position;

        if(IsUnderPawnAttack(position))
        {
            int direction = isWhite ? 1 : -1;

            Position leftAttack = new Position(current.row + direction, current.col - 1);
            Position rightAttack = new Position(current.row + direction, current.col + 1);

            if(IsEnemyPawn(leftAttack))
            {
                var pawn = board.GetFigureAt(leftAttack);
                attackFigures.Add(pawn);
            }

            if (IsEnemyPawn(rightAttack))
            {
                var pawn = board.GetFigureAt(rightAttack);
                attackFigures.Add(pawn);
            }
        }

        if (IsUnderKnightAttack(position))
        {
            foreach (Direction move in knightMoves)
            {
                Position checkPos = new Position(current.row + move.RowDelta, current.col + move.ColDelta);
                Figure piece = board.GetFigureAt(checkPos);

                if(piece is Knight && piece.isWhite != isWhite)
                {
                    attackFigures.Add(piece);
                }
            }
        }

        if(IsUnderLineAttack(position))
        {
            foreach (Direction dir in straightDirections)
            {
                Position checkPos = position;

                while (true)
                {
                    checkPos = new Position(checkPos.row + dir.RowDelta, checkPos.col + dir.ColDelta);

                    if (!board.IsInsideBoard(checkPos))
                        break;

                    Figure piece = board.GetFigureAt(checkPos);

                    if (piece != null)
                    {
                        if (piece.isWhite != isWhite && (piece is Rook || piece is Queen))
                        {
                            attackFigures.Add(piece);
                        }
                        break;
                    }
                }
            }
        }

        if(IsUnderDiagonalAttack(position))
        {
            foreach (Direction dir in diagonalDirections)
            {
                Position checkPos = position;

                while (true)
                {
                    checkPos = new Position(checkPos.row + dir.RowDelta, checkPos.col + dir.ColDelta);

                    if (!board.IsInsideBoard(checkPos))
                        break;

                    Figure piece = board.GetFigureAt(checkPos);

                    if (piece != null)
                    {
                        if (piece.isWhite != isWhite && (piece is Bishop || piece is Queen))
                        {
                            attackFigures.Add(piece);
                        }
                        break;
                    }
                }
            }
        }

        return attackFigures;
    }

    private bool CanSideCapture(Figure enemy)
    {
        Position enemyPosition = enemy.position;

        foreach (Figure figure in isWhite ? board.whiteFigures : board.blackFigures)
        {
            if(figure != null && (figure is not King))
            {
                if (figure.GetAvaibleMoves().Contains(enemyPosition))
                    return true;
            }
        }

        return false;
    }

    private bool CanSideBlock(Figure enemy)
    {
        switch (enemy.type)
        {
            case FigureType.Bishop:
                if(IsContainBlockPosition(GetBishopPositions((Bishop)enemy)))
                {
                    return true;
                }
                break;
            case FigureType.Rook:
                if(IsContainBlockPosition(GetRookPositions((Rook)enemy)))
                {
                    return true;
                }
                break;
            case FigureType.Queen:
                if(IsContainBlockPosition(GetQueenPositions((Queen)enemy)))
                {
                    return true;
                }
                break;
            default:
                break;
        }
        return false;
    }

    private bool IsContainBlockPosition(List<Position> freePositions)
    {
        List<Figure> ownFigures = isWhite ? board.whiteFigures: board.blackFigures;

        foreach (Figure figure in ownFigures)
        {
            if((figure is not King))
            {
                foreach (Position position in freePositions)
                {
                    if(figure.GetAvaibleMoves().Contains(position))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
    public List<Position> GetQueenPositions(Queen queen)
    {
        List<Direction> directions = new List<Direction>()
        {
            Direction.UpLeft(), Direction.UpRight(), Direction.DownLeft(), Direction.DownRight(),
            Direction.Up(), Direction.Down(), Direction.Right(), Direction.Left()
        };

        foreach (var direction in directions)
        {
            Position current = queen.position;
            List<Position> tempPositions = new List<Position>();

            while(true)
            {
                current = new Position(current.row + direction.RowDelta, current.col + direction.ColDelta);
                
                if(!board.IsInsideBoard(current))
                {
                    break;
                }

                Figure piece = board.GetFigureAt(current);

                if(piece != null)
                {
                    if(piece.isWhite != queen.isWhite && (piece is King))
                    {
                        return tempPositions;
                    }
                    else
                    {
                        break;
                    }
                }

                tempPositions.Add(current);
            }
        }

        return new List<Position>();
    }

    public List<Position> GetRookPositions(Rook rook)
    {
        foreach (Direction dir in straightDirections)
        {
            Position checkPos = rook.position;
            List<Position> tempPositions = new List<Position>();

            while (true)
            {
                checkPos = new Position(checkPos.row + dir.RowDelta, checkPos.col + dir.ColDelta);

                if (!board.IsInsideBoard(checkPos))
                {
                    break;
                }

                Figure piece = board.GetFigureAt(checkPos);

                if (piece != null)
                {
                    if (piece.isWhite != rook.isWhite && (piece is King))
                    {
                        return tempPositions;
                    }
                    else
                    {
                        break;
                    }
                }

                tempPositions.Add(checkPos);
            }
        }
        return new List<Position>();
    }

    public List<Position> GetBishopPositions(Bishop bishop)
    {
        foreach (Direction dir in diagonalDirections)
        {
            Position checkPos = bishop.position;
            List<Position> tempPositions = new List<Position>();

            while (true)
            {
                checkPos = new Position(checkPos.row + dir.RowDelta, checkPos.col + dir.ColDelta);

                if (!board.IsInsideBoard(checkPos))
                {
                    break;
                }

                Figure piece = board.GetFigureAt(checkPos);

                if (piece != null)
                {
                    if (piece.isWhite != bishop.isWhite && (piece is King))
                    {
                        return tempPositions;
                    }
                    break;
                }

                tempPositions.Add(checkPos);
            }
        }
        return new List<Position>();
    }
}
