using UnityEngine.Events;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    public UnityEvent OnTileEntered;
    public TileType tileType = TileType.Normal;

    public enum TileType
    {
        Normal,
        Checkpoint,
        Event,
        Minigame
    }
    private void Awake()
    {
        // Initialize the event if it's null
        if (OnTileEntered == null)
        {
            OnTileEntered = new UnityEvent();
        }
    }

    public void Init(bool isOffset)
    {
        _highlight.SetActive(false);
    }

    public void SetColor(Color color)
    {
        _renderer.color = color;
    }

    private void OnMouseEnter()
    {
        _highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }
}