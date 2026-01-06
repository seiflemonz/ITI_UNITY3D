using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Character
{
    public string name;
    public List<Item> inventory;

    public Character(string name)
    {
        this.name = name;
        this.inventory = new List<Item>();
    }
}
