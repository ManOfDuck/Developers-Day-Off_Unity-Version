using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxCollider2DWrapper : ComponentWrapper<BoxCollider2D>
{
    protected override string DefaultVisualComponentName => "BoxCollider2D";

    public override void SetDefaultValues()
    {

    }

    public override SimulatedComponent Copy(ComponentHolder destination)
    {
        BoxCollider2DWrapper copy = base.Copy(destination) as BoxCollider2DWrapper;
        if (destination is SimulatedObject o)
        {
            Collider2D trigger = o.ClickTrigger;
            copy.WrappedComponent.size = new Vector3(trigger.bounds.size.x / destination.transform.localScale.x, trigger.bounds.size.y / destination.transform.localScale.y, trigger.bounds.size.z / destination.transform.localScale.z);
            copy.WrappedComponent.offset = trigger.offset;
        }
        return copy;
    }

}

