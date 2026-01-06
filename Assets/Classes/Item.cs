using UnityEngine;

public class Item
{
    public int id;
    public string name;
    public Sprite image;
    public Item(int id, string name, Sprite image)
    {
        this.id = id;
        this.name = name;
        this.image = image;
    }
}
