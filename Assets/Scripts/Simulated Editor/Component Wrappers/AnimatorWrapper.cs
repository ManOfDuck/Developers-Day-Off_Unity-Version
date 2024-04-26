using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorWrapper : ComponentWrapper<Animator>
{
    protected override string DefaultVisualComponentName => "Animator";

    public override void SetDefaultValues()
    {
        // We good
    }
}
