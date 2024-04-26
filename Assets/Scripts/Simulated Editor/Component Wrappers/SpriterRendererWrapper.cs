using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriterRendererWrapper : ComponentWrapper<SpriteRenderer>
{
    protected override string DefaultVisualComponentName => "SpriteRenderer";

    public override void SetDefaultValues()
    {
        // We good
    }
}
