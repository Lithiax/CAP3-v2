using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D cursor;
    [SerializeField] Texture2D cursorClicked;
    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        Cursor.SetCursor(cursor, new Vector2(0, 0), CursorMode.ForceSoftware);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(cursorClicked, new Vector2(0, 0), CursorMode.ForceSoftware);
        }

        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(cursor, new Vector2(0, 0), CursorMode.ForceSoftware);
        }
    }

}
