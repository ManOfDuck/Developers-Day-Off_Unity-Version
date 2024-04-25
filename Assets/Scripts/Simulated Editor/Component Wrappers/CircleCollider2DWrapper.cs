using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCollider2DWrapper : ComponentWrapper<CircleCollider2D>
{
    protected override string DefaultVisualComponentName => "CircleCollider2D";

    public override void SetDefaultValues()
    {
        //We good
    }
}
