using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgoundLooper : MonoBehaviour
{
    public float scrollspeed=0.1f;
    private Vector2 offset = Vector2.zero;
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<Renderer>().material;
        offset = material.GetTextureOffset("_MainTex");
    }

    // Update is called once per frame
    void Update()
    {
        offset += new Vector2(scrollspeed * Time.deltaTime, 0);
        material.SetTextureOffset("_MainTex",offset);
    }
}
