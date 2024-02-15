using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//https://stackoverflow.com/questions/49845970/how-do-i-set-a-material-to-an-gameobject-in-unity-c-sharp-script
public class HighlightTest : MonoBehaviour
{
    [SerializeField] Material highlightMaterial;
    [SerializeField] Renderer renderer;

    // Start is called before the first frame update
    void Start()
    {
        renderer.material = highlightMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
