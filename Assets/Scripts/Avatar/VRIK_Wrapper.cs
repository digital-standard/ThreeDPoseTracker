using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIK_Wrapper : VRIK
{
    public void UpdateVRIK(Transform transform)
    {
        References.AutoDetectReferences(transform, out references);
        solver.SetToReferences(references);

        base.UpdateSolver();
    }

    public void UpdateVRIK()
    {
        base.UpdateSolver();
    }

    public void SetIKPositionWeight(float weight)
    {
        this.solver.SetIKPositionWeight(weight);

    }

    public void SetLegPositionWeight(float weight)
    {
        this.solver.rightLeg.positionWeight = weight;
        this.solver.leftLeg.positionWeight = weight;
    }

    public void SetHeightOffset(float offset)
    {
        var ik = GetComponent<GrounderVRIK>();
        ik.solver.heightOffset = offset;
    }
}
