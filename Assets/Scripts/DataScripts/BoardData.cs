using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BoardData", menuName = "BoardData/Board")]
public class BoardData : ScriptableObject
{
    [SerializeField] private GameObject cell;

    [Header("White cell")]
    [SerializeField][Range(0,255)] private float whiteRedPropetry;
    [SerializeField][Range(0,255)] private float whiteGreenPropetry;
    [SerializeField][Range(0,255)] private float whiteBluePropetry;
    [SerializeField][Range(0,1f)] private float whiteAlphaProperty;

    [Header("Black cell")]
    [SerializeField][Range(0,255)] private float blackRedPropetry;
    [SerializeField][Range(0,255)] private float blackGreenPropetry;
    [SerializeField][Range(0,255)] private float blackBluePropetry;
    [SerializeField][Range(0,1f)] private float blackAlphaProperty;

    [Header("Free cell")]
    [SerializeField][Range(0, 255)] private float freeRedProperty;
    [SerializeField][Range(0, 255)] private float freeGreenProperty;
    [SerializeField][Range(0, 255)] private float freeBlueProperty;
    [SerializeField][Range(0,1f)] private float freeAlphaProperty;

    [Header("Enemy cell")]
    [SerializeField][Range(0,255)] private float enemyRedProperty;
    [SerializeField][Range(0,255)] private float enemyGreenProperty;
    [SerializeField][Range(0,255)] private float enemyBlueProperty;
    [SerializeField][Range(0,1f)] private float enemyAlphaProperty;

    [Header("Check")]
    [SerializeField][Range(0,255)] private float checkRedProperty;
    [SerializeField][Range(0,255)] private float checkGreenProperty;
    [SerializeField][Range(0,255)] private float checkBlueProperty;
    [SerializeField][Range(0,1f)] private float checkAlphaProperty;

    [Header("White figures")]
    [SerializeField] private GameObject whitePawn;
    [SerializeField] private GameObject whiteKnight;
    [SerializeField] private GameObject whiteBishop;
    [SerializeField] private GameObject whiteRook;
    [SerializeField] private GameObject whiteQueen;
    [SerializeField] private GameObject whiteKing;

    [Header("Black figures")]
    [SerializeField] private GameObject blackPawn;
    [SerializeField] private GameObject blackKnight;
    [SerializeField] private GameObject blackBishop;
    [SerializeField] private GameObject blackRook;
    [SerializeField] private GameObject blackQueen;
    [SerializeField] private GameObject blackKing;

    public PawnPick pawnPick;

    public GameObject GetCell()
    {
        return cell;
    }

    public Color GetWhiteColor()
    {
        return new Color(whiteRedPropetry, whiteGreenPropetry, whiteBluePropetry, whiteAlphaProperty);
    }

    public Color GetBlackColor()
    {
        return new Color(blackRedPropetry, blackGreenPropetry, blackBluePropetry, blackAlphaProperty);
    }

    public Color GetCheckColor()
    {
        return new Color(checkRedProperty, checkGreenProperty, checkBlueProperty, checkAlphaProperty);
    }

    public Color GetEnemyColor()
    {
        return new Color(enemyRedProperty, enemyGreenProperty, enemyBlueProperty, enemyAlphaProperty);
    }

    public Color GetFreeColor()
    {
        return new Color(freeRedProperty, freeGreenProperty, freeBlueProperty, freeAlphaProperty);
    }

    public GameObject GetWhitePawn()
    {
        return whitePawn;
    }

    public GameObject GetBlackPawn()
    {
        return blackPawn;
    }

    public GameObject GetWhiteKnight()
    {
        return whiteKnight;
    }

    public GameObject GetBlackKnight()
    {
        return blackKnight;
    }

    public GameObject GetWhiteBishop()
    {
        return whiteBishop;
    }

    public GameObject GetBlackBishop()
    {
        return blackBishop;
    }

    public GameObject GetWhiteRook()
    {
        return whiteRook;
    }

    public GameObject GetBlackRook()
    {
        return blackRook;
    }

    public GameObject GetWhiteQueen()
    {
        return whiteQueen;
    }

    public GameObject GetBlackQueen()
    {
        return blackQueen;
    }

    public GameObject GetWhiteKing()
    {
        return whiteKing;
    }

    public GameObject GetBlackKing()
    {
        return blackKing;
    }
}

[System.Serializable]
public struct PawnPick
{
    [Header("Black sprites")]
    public Sprite queenSpriteBlack;
    public Sprite rookSpriteBlack;
    public Sprite bishopSpriteBlack;
    public Sprite knightSpriteBlack;

    [Header("White sprites")]
    public Sprite queenSpriteWhite;
    public Sprite rookSpriteWhite;
    public Sprite bishopSpriteWhite;
    public Sprite knightSpriteWhite;
}