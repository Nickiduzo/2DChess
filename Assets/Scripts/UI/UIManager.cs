using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private BoardData boardData;

    [SerializeField] private Board board;

    [SerializeField] private Button reload;

    [SerializeField] private GameObject gameOver;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [SerializeField] private GameObject pawnPick;

    [SerializeField] private Button pawnQueenPick;
    [SerializeField] private Button pawnRookPick;
    [SerializeField] private Button pawnKnightPick;
    [SerializeField] private Button pawnBishopPick;

    [SerializeField] private Image queenImage;
    [SerializeField] private Image rookImage;
    [SerializeField] private Image bishopImage;
    [SerializeField] private Image knightImage;

    private Pawn currentPawn;
    private void Start()
    {
        reload.onClick.AddListener(() => ReloadScene());

        pawnQueenPick.onClick.AddListener(() => PickQueen());
        pawnRookPick.onClick.AddListener(() => PickRook());
        pawnKnightPick.onClick.AddListener(() => PickKnight());
        pawnBishopPick.onClick.AddListener(() => PickBishop());

        Pawn.OnGetLastPosition += SwitchPickEnable;
        King.OnCheckMate += SwitchWindow;
        King.OnStalemate += SwitchStaleWindow;
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

    private void SwitchStaleWindow()
    {
        if(!gameOver.activeSelf)
        {
            gameOver.SetActive(true);
            board.SwitchBlackFigures(false);
            board.SwitchWhiteFigures(false);
            gameOverText.text = "It's a draw!";
        }
    }

    private void SwitchWindow(bool isWhite)
    {
        if(!gameOver.activeSelf)
        {
            gameOver.SetActive(true);
            board.SwitchBlackFigures(false);
            board.SwitchWhiteFigures(false);
            gameOverText.text = isWhite ? "Black wins" : "White wins";
        }
    }

    private void SwitchPickEnable(Pawn pawn)
    {
        currentPawn = pawn;
        board.isPawnLast = true;
        pawnPick.SetActive(true);
        if (pawn.isWhite)
        {
            ShowWhitePeak();
            pawnPick.transform.position = pawn.transform.position + new Vector3(10, 10, 10);
        }
        else
        {
            ShowBlackPeak();
            pawnPick.transform.position = pawn.transform.position + new Vector3(10, -10, 10);
        }
    }

    private void ShowWhitePeak()
    {
        queenImage.sprite = boardData.pawnPick.queenSpriteWhite;
        rookImage.sprite = boardData.pawnPick.rookSpriteWhite;
        knightImage.sprite = boardData.pawnPick.knightSpriteWhite;
        bishopImage.sprite = boardData.pawnPick.bishopSpriteWhite;
    }

    private void ShowBlackPeak()
    {
        queenImage.sprite = boardData.pawnPick.queenSpriteBlack;
        rookImage.sprite = boardData.pawnPick.rookSpriteBlack;
        knightImage.sprite = boardData.pawnPick.knightSpriteBlack;
        bishopImage.sprite = boardData.pawnPick.bishopSpriteBlack;
    }

    private void PickQueen()
    {
        print("Queen picked");
        pawnPick.SetActive(false);

        GameObject queenPrefab = currentPawn.isWhite ? boardData.GetWhiteQueen() : boardData.GetBlackQueen();
        bool isWhite = currentPawn.isWhite;
        int row = currentPawn.position.row;
        int col = currentPawn.position.col;

        board.CreateFigure<Queen>(queenPrefab, isWhite, row, col);

        board.DestroyFigure(currentPawn);

        board.isPawnLast = false;
        board.SwitchFigures();
    }

    private void PickRook()
    {
        print("Rook picked");
        pawnPick.SetActive(false);

        GameObject rookPrefab = currentPawn.isWhite ? boardData.GetWhiteRook() : boardData.GetBlackRook();
        bool isWhite = currentPawn.isWhite;
        int row = currentPawn.position.row;
        int col = currentPawn.position.col;

        board.CreateFigure<Rook>(rookPrefab, isWhite, row, col);

        board.DestroyFigure(currentPawn);

        board.isPawnLast = false;
        board.SwitchFigures();
    }

    private void PickBishop()
    {
        print("Bishop picked");
        pawnPick.SetActive(false);

        GameObject bishopPrefab = currentPawn.isWhite ? boardData.GetWhiteBishop() : boardData.GetBlackBishop();
        bool isWhite = currentPawn.isWhite;
        int row = currentPawn.position.row;
        int col = currentPawn.position.col;

        board.CreateFigure<Bishop>(bishopPrefab, isWhite, row, col);

        board.DestroyFigure(currentPawn);
        
        board.isPawnLast = false;
        board.SwitchFigures();
    }

    private void PickKnight()
    {
        print("Knight picked");
        pawnPick.SetActive(false);

        GameObject knightPrefab = currentPawn.isWhite ? boardData.GetWhiteKnight() : boardData.GetBlackKnight();
        bool isWhite = currentPawn.isWhite;
        int row = currentPawn.position.row;
        int col = currentPawn.position.col;

        board.CreateFigure<Knight>(knightPrefab, isWhite, row, col);

        board.DestroyFigure(currentPawn);
        
        board.isPawnLast = false;
        board.SwitchFigures();
    }

    private void OnDisable()
    {
        reload.onClick.RemoveAllListeners();

        pawnQueenPick.onClick.RemoveAllListeners();
        pawnRookPick.onClick.RemoveAllListeners();
        pawnBishopPick.onClick.RemoveAllListeners();
        pawnKnightPick.onClick.RemoveAllListeners();

        Pawn.OnGetLastPosition -= SwitchPickEnable;
        King.OnCheckMate -= SwitchWindow;
        King.OnStalemate -= SwitchStaleWindow;
    }
}
