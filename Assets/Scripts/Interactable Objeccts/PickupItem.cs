using UnityEngine;

public class PickupItem : InteractableObject
{
    public int id;
    public string itemName;
    public float itemWidth;
    void Start()
    {
        type = InteractableType.pickup;
        itemWidth = GetItemWidth();
    }
    public override void Interact(GameObject character, Player playerScript)
    {
        playerScript.PickUpItem(gameObject);
    }

    /// <summary>
    /// Measures width of the object based on COLLIDER
    ///
    /// WARNING: works only for BoxCollider (do we need more?)
    ///         center of the item should separate the width in half
    ///         transform.size also does not change the size
    /// </summary>
    /// <returns></returns>
    private float GetItemWidth()
    {
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        itemWidth = collider.size.x;
        return itemWidth;
    }
}
