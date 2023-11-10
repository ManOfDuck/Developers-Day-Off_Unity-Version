using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Root : MonoBehaviour
{
    private Vector2 mousePos;
    public float manaUsed;
    private GameManager gameManager;

    public void Start()
    {
        gameManager = GameManager.Instance;
        this.gameObject.tag = "rootable";
        manaUsed = 0;
    }

    public void Update()
    {
        float input = Mouse.current.rightButton.ReadValue();
        float key = Keyboard.current.tKey.ReadValue();

        if (key == 1)
        {
            destroy();
        }

        mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Collider2D[] overlapArray = Physics2D.OverlapCircleAll(mousePos, 0.1f);

        foreach (Collider2D collider in overlapArray)
        {
            if (collider == this.gameObject.GetComponent<PolygonCollider2D>() && input == 1)
            {
                destroy();

            }
        }
    }

    public void destroy()
    {
        Destroy(this.gameObject);
        gameManager.mana += manaUsed;
    }
}
