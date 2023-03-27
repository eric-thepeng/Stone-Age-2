using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using static UnityEditor.Progress;

public class ObjectStackList<T>
{
    public Dictionary<T, int> data;

    public ObjectStackList()
    {
        data = new Dictionary<T, int>();
    }
    
    public void AddAmount(T newItem, int amount = 0)
    {

        foreach(KeyValuePair<T,int> kvp in data)
        {
            if (kvp.Key.Equals(newItem))
            {
                data[kvp.Key] += amount;
                return;
            }
        }

        data.Add(newItem, 1);

    }

    /// <summary>
    /// Decrease amount of certain item.  
    /// </summary>
    /// <param name="removeItem"></param>
    public void RemoveAmount(T removeItem, int amount = 1)
    {
        foreach (KeyValuePair<T, int> kvp in data)
        {
            if (kvp.Key.Equals(removeItem))
            {
                data[kvp.Key] -= amount;
                if(kvp.Value < 0)
                {
                    data.Remove(kvp.Key);
                }
                return;
            }
        }
    }

    /// <summary>
    /// Return the amount of the input item. Return -1 if it does not exist.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public int GetAmount(T item)
    {
        foreach (KeyValuePair<T, int> kvp in data)
        {
            if (kvp.Key.Equals(item))
            {
                return kvp.Value;
            }
        }

        return -1;
    }

    public override string ToString()
    {
        string output = "";

        foreach (KeyValuePair<T, int> kvp in data)
        {
            output += "[ " + kvp.Key + " x " + kvp.Value + " ] ";
        }

        return output;
    }

}
