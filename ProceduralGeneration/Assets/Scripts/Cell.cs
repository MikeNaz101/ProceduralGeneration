using UnityEngine;

public class Cell : MonoBehaviour
{
    public bool isAlive;
    private SpriteRenderer spriteRenderer;
    
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    public void UpdateColor()
    {
        spriteRenderer.color = isAlive ? Color.white : Color.red;
    }
}
