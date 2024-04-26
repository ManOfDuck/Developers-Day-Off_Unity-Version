using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollider2DWrapper : ComponentWrapper<BoxCollider2D>
{
    protected override string DefaultVisualComponentName => "BoxCollider2D";

    public override void SetDefaultValues()
    {
        // We good
    }
}

