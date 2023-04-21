using UnityEngine;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Sprite _cursorSprite;
    [SerializeField] private Vector3 _cursorOffset; 
    [SerializeField] private float _scale; 

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        SetCursorPosition();
    }

    private void SetCursorPosition()
    {
        Vector2 mousePosition = Input.mousePosition;

        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        worldPosition.z = 0f;

        transform.position = worldPosition + _cursorOffset;
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 
                _cursorSprite.texture.width* _scale, _cursorSprite.texture.height* _scale), 
            _cursorSprite.texture);
    }
}