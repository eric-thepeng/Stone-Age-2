using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IResourceSetProvider
{
    public ResourceSet ProvideResourceSet(int index = 0);
}
