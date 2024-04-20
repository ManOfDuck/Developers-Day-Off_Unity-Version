using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2DWrapper : ComponentWrapper<Rigidbody2D>
{
    protected override string DefaultVisualComponentName => "Rigidbody2D";

    public override void SetDefaultValues()
    {
        WrappedComponent.isKinematic = true;
    }
}