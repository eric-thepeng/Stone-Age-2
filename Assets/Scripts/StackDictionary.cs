using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackDictionary<T>
{
    Dictionary<T, int> dataBase;
    public StackDictionary()
    {
        dataBase = new Dictionary<T, int>();
    }
    public void Add(T key)
    {
        if (dataBase.ContainsKey(key)){dataBase[key] += 1; return;}
        dataBase.Add(key, 1);
    }
    public void Remove(T key)
    {
        if (!dataBase.ContainsKey(key)) { return; }
        if(dataBase[key] == 1) { dataBase.Remove(key); return; }
        dataBase[key] -= 1;
    }

}
