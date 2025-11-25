using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Stack
    {
        public ItemDefinition def;
        public int count;
    }

    public List<Stack> items = new List<Stack>();

    public UnityEvent pickupEvent = new();

    public void Add(ItemDefinition def, int amt)
    {
        if (!def) return;

        if (def.stackable)
        {
            var found = items.Find(s => s.def == def);
            if (found != null)
            {
                found.count = Mathf.Min(found.count + amt, def.maxStack);
            }
            else
            {
                items.Add(new Stack { def = def, count = Mathf.Min(amt, def.maxStack) });
            }
        }
        else
        {
            // add one stack per item for non-stackables
            for (int i = 0; i < amt; i++)
                items.Add(new Stack { def = def, count = 1 });
        }
        
        pickupEvent.Invoke();

        // TODO: raise UI event, play sfx, etc.
        Debug.Log($"Picked up {amt}x {def.displayName}");
    }
}
