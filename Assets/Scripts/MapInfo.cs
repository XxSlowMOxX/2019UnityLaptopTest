using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    public Vector3 mapSize;
    public SpriteRenderer mapImage;
    void Start()
    {
        mapSize = mapImage.bounds.size;
    }
}
