using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllDecorationObjects", menuName = "ScriptableObjects/AllDecorationObjects", order = 2)]
public class AllDecorationObjectsSO : ScriptableObject
{
    public List<DecorationObjectSO> Decorations;
}
