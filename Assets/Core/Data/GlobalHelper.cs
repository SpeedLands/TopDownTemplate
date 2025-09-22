using System;
using UnityEngine;

public class GlobalHelper
{
    public static String GenerateUniqueID(GameObject obj)
    {
        return $"{obj.scene.name}_{obj.transform.position.x}_{obj.transform.position.y}";
    }
}
