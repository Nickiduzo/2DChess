using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private BoardData data;
    [SerializeField] private Transform board;

    public Cell[,] cells;

    public List<Figure> whiteFigures;
    public List<Figure> blackFigures;

    public List<Position> lightWays;

    public bool isWhiteMove;
    public bool isPawnLast;

    public Stack<Figure> stack;

    public Figure selectedFigure = null;
    private void Awake()
    {
        cells = new Cell[8,8];
        InitializeBoard();

        whiteFigures = new List<Figure>();
        blackFigures = new List<Figure>();
        
        lightWays = new List<Position>();

        stack = new Stack<Figure>();

        InitializeFigures();

        Figure.OnMouseClick += HandleLights;
        Cell.OnMouseClick += HandleFigure;

        isWhiteMove = true;
        SwitchFigures();
    }

    private void Update()
    {
        if(isPawnLast)
        {
            SwitchBlackFigures(false);
            SwitchWhiteFigures(false);
        }
    }

    private void InitializeBoard()
    {
        int id = 0;
        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int col = 0; col < cells.GetLength(1); col++)
            {
                if(row % 2 == 0)
                {
                    if(col % 2 == 0)
                    {
                        cells[row, col] = CreateBlackCell(id, row, col);
                    }
                    else
                    {
                        cells[row, col] = CreateWhiteCell(id, row, col);
                    }
                }
                else
                {
                    if (col % 2 == 0)
                    {
                        cells[row, col] = CreateWhiteCell(id, row, col);
                    }
                    else
                    {
                        cells[row, col] = CreateBlackCell(id, row, col);
                    }
                }

                id++;
            }
        }
    }

    private Cell CreateWhiteCell(int id, int row, int col)
    {
        var emptyCell = Instantiate(data.GetCell(), board.position, Quaternion.identity, board.transform);
        Cell cell = emptyCell.GetComponent<Cell>();
        if(cell != null)
        {
            cell.Initialize(id, data.GetWhiteColor(), row, col);
        }
        return cell;
    }

    private Cell CreateBlackCell(int id, int row, int col)
    {
        var emptyCell = Instantiate(data.GetCell(), board.position, Quaternion.identity, board.transform);
        Cell cell = emptyCell.GetComponent<Cell>();
        if(cell != null)
        {
            cell.Initialize(id, data.GetBlackColor(), row, col);
        }
        return cell;
    }
    private void InitializeFigures()
    {
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 8; col++)
            {
                if(row == 0)
                {
                    if(col == 0 || col == 7)
                    {
                        CreateFigure<Rook>(data.GetWhiteRook(), true, row, col);
                    }

                    if(col == 1 || col == 6)
                    {
                        CreateFigure<Knight>(data.GetWhiteKnight(), true, row, col);
                    }

                    if(col == 2 || col == 5)
                    {
                        //CreateBishop(true, row, col);
                        CreateFigure<Bishop>(data.GetWhiteBishop(), true, row, col);
                    }

                    if(col == 3)
                    {
                        CreateFigure<Queen>(data.GetWhiteQueen(), true, row, col);
                    }

                    if(col == 4)
                    {
                        CreateFigure<King>(data.GetWhiteKing(), true, row, col);
                    }
                }
                else
                {
                    CreateFigure<Pawn>(data.GetWhitePawn(), true, row, col);
                }
            }
        }

        for (int row = 7; row > 5; row--)
        {
            for (int col = 0; col < 8; col++)
            {
                if(row == 7)
                {
                    if(col == 0 || col == 7)
                    {
                        CreateFigure<Rook>(data.GetBlackRook(), false, row, col);
                    }

                    if(col == 1 || col == 6)
                    {
                        CreateFigure<Knight>(data.GetBlackKnight(), false, row, col);
                    }

                    if(col == 2 || col == 5)
                    {
                        CreateFigure<Bishop>(data.GetBlackBishop(), false, row, col);
                    }

                    if(col == 3)
                    {
                        CreateFigure<Queen>(data.GetBlackQueen(), false, row, col);
                    }

                    if(col == 4)
                    {
                        CreateFigure<King>(data.GetBlackKing(), false, row, col);
                    }
                }
                else
                {
                    CreateFigure<Pawn>(data.GetBlackPawn(), false, row, col);
                }
            }
        }
    }

    public void CreateFigure<T>(GameObject prefab, bool isWhite, int row, int col) where T : Figure
    {
        var temp = Instantiate(prefab, cells[row, col].transform.position, Quaternion.identity, cells[row, col].transform);
        
        T figure = temp.GetComponent<T>();
        figure.Initialize(new Position(row, col), isWhite,this);

        if(isWhite)
        {
            whiteFigures.Add(figure);
        }
        else
        {
            blackFigures.Add(figure);
        }
    }


    private void HandleLights(List<Position> positions, Figure figure)
    {
        DeLightWays();

        if (selectedFigure == figure)
        {
            selectedFigure = null;
            return;
        }

        selectedFigure = figure;
        LightWays(positions, figure);
    }

    private void HandleFigure(Position position)
    {
        if (selectedFigure == null) return;

        if (lightWays.Contains(position))
        { 
            selectedFigure.MoveTo(position);
            isWhiteMove = !isWhiteMove;
            SwitchFigures();
        }

        selectedFigure = null;
        DeLightWays();
    }

    public void AddMoveToStack(Figure figure)
    {
        stack.Push(figure);
    }

    public Figure GetFigureAt(Position position)
    {
        if (!IsInBounds(position.row, position.col)) return null;
        int figureRow = position.row;
        int figureCol = position.col;

        return cells[figureRow, figureCol].GetComponentInChildren<Figure>();
    }

    public Figure GetLastFigure()
    {
        return stack.Peek();
    }

    public void LightWays(List<Position> positions, Figure figure)
    {
        foreach (Position position in positions)
        {
            int row = position.row;
            int col = position.col;

            if (cells[row,col].ContainFigure())
            {
                if(IsContainEnemy(new Position(row, col), figure.isWhite))
                {
                    cells[row, col].SetColor(data.GetEnemyColor());
                }
                else
                {
                    cells[row, col].SetColor(data.GetFreeColor());
                }
            }
            else
            {
                cells[row, col].SetColor(data.GetFreeColor());
            }

            lightWays.Add(position);
        }
    }

    public void DeLightWays()
    {
        foreach (Position position in lightWays)
        {
            int row = position.row;
            int col = position.col;

            cells[row, col].SetDefault();
        }
        lightWays.Clear();
    }

    public void HighLightKing(Position position)
    {
        cells[position.row, position.col].SetColor(data.GetCheckColor());
    }

    public bool IsFreeCell(Position position)
    {
        int row = position.row;
        int col = position.col;

        return IsInBounds(row, col) && !cells[row, col].ContainFigure();
    }

    public bool IsContainEnemy(Position position, bool isWhite)
    {
        if (IsInBounds(position.row, position.col))
        {
            if (cells[position.row, position.col].ContainFigure())
            {
                Figure enemy = cells[position.row, position.col].GetComponentInChildren<Figure>();
                if (enemy != null && enemy.isWhite != isWhite)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool IsInBounds(int row, int col)
    {
        return row >= 0 && row <= 7 && col >= 0 && col <= 7;
    }

    public bool IsInsideBoard(Position position)
    {
        return position.row >= 0 && position.row <= 7 && position.col >= 0 && position.col <= 7;
    }

    public void SwitchFigures()
    {
        if (isWhiteMove)
        {
            SwitchWhiteFigures(true);
            SwitchBlackFigures(false);
        }
        else
        {
            SwitchWhiteFigures(false);
            SwitchBlackFigures(true);
        }
    }

    public List<Position> GetCaptureOrBlockPositions(List<Position> avaible, bool isWhite, Figure enemy)
    {
        List<Position> result = new();

        if (avaible.Contains(enemy.position))
        {
            result.Add(enemy.position);
        }

        King mainKing = GetKing(isWhite);

        switch (enemy.type)
        {
            case FigureType.Bishop:
                Bishop bishop = (Bishop)enemy;
                foreach (Position position in mainKing.GetBishopPositions(bishop))
                {
                    if(avaible.Contains(position))
                    {
                        result.Add(position);
                    }
                }
                break;
            case FigureType.Rook:
                Rook rook = (Rook)enemy;
                foreach (Position position in mainKing.GetRookPositions(rook))
                {
                    if(avaible.Contains(position))
                    {
                        result.Add(position);
                    }
                }
                break;
            case FigureType.Queen:
                Queen queen = (Queen)enemy;
                foreach (Position position in mainKing.GetQueenPositions(queen))
                {
                    if(avaible.Contains(position))
                    {
                        result.Add(position);
                    }
                }
                break;
            default: print("Wtf");
                break;
        }
        return result;
    }



    public void SwitchWhiteFigures(bool flag)
    {
        foreach (var item in whiteFigures)
            item.Iteractable(flag);
    }

    public void SwitchBlackFigures(bool flag)
    {
        foreach(var item in blackFigures) 
            item.Iteractable(flag);
    }

    public void DestroyFigure(Figure figure)
    {
        if(figure.isWhite)
        {
            foreach (var item in whiteFigures)
            {
                if(item.position.Equals(figure.position))
                {
                    whiteFigures.Remove(item);
                    Destroy(figure.gameObject);
                    return;
                }
            }
        }
        else
        {
            foreach(var item in blackFigures)
            {
                if(item.position.Equals(figure.position))
                {
                    blackFigures.Remove(item);
                    Destroy(figure.gameObject);
                    return;
                }
            }
        }
    }

    public Color CheckColor() => data.GetCheckColor();

    public King GetKing(bool isWhite)
    {
        foreach (Figure figure in isWhite ? whiteFigures : blackFigures)
        {
            if (figure is King king) return king;
        }
        return null;
    }

    private void OnDisable()
    {
        Figure.OnMouseClick -= HandleLights;
        Cell.OnMouseClick -= HandleFigure;
    }
}
