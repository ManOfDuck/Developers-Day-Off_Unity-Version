using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2DWrapper : ComponentWrapper<Rigidbody2D>
{
    protected override string DefaultVisualComponentName => "Rigidbody2D";
    public RigidbodyType2D BodyType { get => WrappedComponent.bodyType; set => WrappedComponent.bodyType = value; }
    public float Mass { get => WrappedComponent.mass; set => WrappedComponent.mass = value; }

    public override void SetDefaultValues()
    {
        BodyType = RigidbodyType2D.Kinematic;
    }
}