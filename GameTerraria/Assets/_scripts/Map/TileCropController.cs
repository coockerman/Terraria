using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileCropController : MonoBehaviour
{
    public ItemClass item;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<Inventory>().AddItem(item))
                Destroy(this.gameObject);
        }
    }
}
