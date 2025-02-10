using System;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public static event Action<Position> OnMouseClick;
    [SerializeField] private Image image;
    public int Id { get; set; }

    private Color defaultColor;

    private Position position;
    public bool ContainFigure()
    {
        return GetComponentInChildren<Figure>() != null;
    }

    public void OnMouseDown()
    {
        OnMouseClick?.Invoke(position);
    }

    public void Initialize(int id, Color defaultColor, int row, int col)
    {
        Id = id;
        this.defaultColor = defaultColor;

        position = new Position(row, col);

        SetColor(defaultColor);
    }
    public void SetColor(Color color) => image.color = color;

    public void SetDefault() => image.color = defaultColor;
}
