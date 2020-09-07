using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRIK_Wrapper : VRIK
{

    public void UpdateVRIK(Transform transform)
    {
        //ik.references.root = AvatarList[avatars.value].Avatar.gameObject.transform;
        References.AutoDetectReferences(transform, out references);
        solver.SetToReferences(references);

        base.UpdateSolver();
    }
}
