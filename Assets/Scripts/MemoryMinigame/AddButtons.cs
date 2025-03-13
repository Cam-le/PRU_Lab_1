using UnityEngine;
using UnityEngine.UI;

public class AddButtons : MonoBehaviour
{
    [SerializeField]
    private Transform puzzleField;

    [SerializeField]
    private GameObject button;

    [SerializeField]
    private GridLayoutGroup gridLayout;

    private void Awake()
    {
        // Nếu gridLayout chưa được gán từ Inspector, tự động tìm trong puzzleField
        if (gridLayout == null)
        {
            gridLayout = puzzleField.GetComponent<GridLayoutGroup>();
        }

        //for (int i = 0; i < 16; i++)
        //{
        //    GameObject _button = Instantiate(button);
        //    _button.name = "" + i;
        //    _button.transform.SetParent(puzzleField, false);
        //}
    }

    public void GenerateButtons(int gridSize)
    {
        // Xóa các button cũ nếu có
        foreach (Transform child in puzzleField)
        {
            Destroy(child.gameObject);
        }

        // Tự động chỉnh số cột dựa vào độ khó
        if (gridSize == 9)
            gridLayout.constraintCount = 3; // 3x3 (Chế độ dễ)
        else if (gridSize == 12)
            gridLayout.constraintCount = 4; // 3x4 (Bình thường)
        else if (gridSize == 16)
            gridLayout.constraintCount = 4; // 4x4 (Khó)

        for (int i = 0; i < gridSize; i++)
        {
            GameObject _button = Instantiate(button);
            _button.name = "" + i;
            _button.transform.SetParent(puzzleField, false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
