using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionIndex : int
{
    rShldrBend = 0,
    rForearmBend,
    rHand,
    rThumb2,
    rMid1,

    lShldrBend,
    lForearmBend,
    lHand,
    lThumb2,
    lMid1,

    lEar,
    lEye,
    rEar,
    rEye,
    Nose,

    rThighBend,
    rShin,
    rFoot,
    rToe,

    lThighBend,
    lShin,
    lFoot,
    lToe,

    abdomenUpper,

    //Calculated coordinates
    hip,
    head,
    neck,
    spine,

    Count,
    None,
}

public static partial class EnumExtend
{
    public static int Int(this PositionIndex i)
    {
        return (int)i;
    }
}

public class VNectModel : MonoBehaviour
{

    public class JointPoint
    {
        public PositionIndex Index;
        public Vector2 Pos2D = new Vector2();
        public float score2D;

        public Vector3 Pos3D = new Vector3();
        public Vector3 Now3D = new Vector3();
        public Vector3[] PrevPos3D = new Vector3[6];
        public float score3D;
        public bool Visibled;
        public bool UpperBody;

        // Bones
        public Transform Transform = null;
        public Quaternion InitRotation;
        public Quaternion Inverse;
        public Quaternion InverseRotation;

        public JointPoint Child = null;
        public JointPoint Parent = null;
        public float VecFlag = 1f;

        public static float Q = 0.001f;
        public static float R = 0.0015f;
        public Vector3 P = new Vector3();
        public Vector3 X = new Vector3();
        public Vector3 K = new Vector3();
    }

    public class Skeleton
    {
        public GameObject LineObject;
        public LineRenderer Line;

        public JointPoint start = null;
        public JointPoint end = null;
        public bool upperBody = false;
    }

    private List<Skeleton> Skeletons = new List<Skeleton>();
    public Material SkeletonMaterial;

    public float SkeletonX;
    public float SkeletonY;
    public float SkeletonZ;
    public float SkeletonScale;

    // Joint position and bone
    private JointPoint[] jointPoints;
    public JointPoint[] JointPoints { get { return jointPoints; } }

    private Vector3 initPosition; // Initial center position

    private Quaternion InitGazeRotation;
    private Quaternion gazeInverse;

    // UnityChan
    public GameObject ModelObject;
    public GameObject Nose;
    private Animator anim;

    private float movementScale = 0.01f;
    private float centerTall = 224 * 0.75f;
    private float tall = 224 * 0.75f;
    private float prevTall = 224 * 0.75f;
    public float ZScale = 0.8f;

    private bool UpperBodyMode = false;
    private float UpperBodyF = 1f;


    public JointPoint[] Init(int inputImageSize)
    {
        movementScale = 0.01f * 224f / inputImageSize;
        centerTall = inputImageSize * 0.75f;
        tall = inputImageSize * 0.75f;
        prevTall = inputImageSize * 0.75f;

        jointPoints = new JointPoint[PositionIndex.Count.Int()];
        for (var i = 0; i < PositionIndex.Count.Int(); i++)
        {
            jointPoints[i] = new JointPoint();
            jointPoints[i].Index = (PositionIndex)i;
            jointPoints[i].UpperBody = false;
        }

        anim = ModelObject.GetComponent<Animator>();
        jointPoints[PositionIndex.hip.Int()].Transform = transform;

        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
        jointPoints[PositionIndex.rForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
        jointPoints[PositionIndex.rHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightHand);
        jointPoints[PositionIndex.rThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightThumbIntermediate);
        jointPoints[PositionIndex.rMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightMiddleProximal);
        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
        jointPoints[PositionIndex.lForearmBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
        jointPoints[PositionIndex.lHand.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftHand);
        jointPoints[PositionIndex.lThumb2.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftThumbIntermediate);
        jointPoints[PositionIndex.lMid1.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftMiddleProximal);
        // Face
        jointPoints[PositionIndex.lEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.lEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftEye);
        jointPoints[PositionIndex.rEar.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.rEye.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightEye);
        jointPoints[PositionIndex.Nose.Int()].Transform = Nose.transform;

        // Right Leg
        jointPoints[PositionIndex.rThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
        jointPoints[PositionIndex.rShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
        jointPoints[PositionIndex.rFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightFoot);
        jointPoints[PositionIndex.rToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.RightToes);

        // Left Leg
        jointPoints[PositionIndex.lThighBend.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
        jointPoints[PositionIndex.lShin.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
        jointPoints[PositionIndex.lFoot.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
        jointPoints[PositionIndex.lToe.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.LeftToes);

        // etc
        jointPoints[PositionIndex.abdomenUpper.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);
        jointPoints[PositionIndex.head.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Head);
        jointPoints[PositionIndex.hip.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Hips);
        jointPoints[PositionIndex.neck.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Neck);
        jointPoints[PositionIndex.spine.Int()].Transform = anim.GetBoneTransform(HumanBodyBones.Spine);

        // UpperBody Settings
        jointPoints[PositionIndex.hip.Int()].UpperBody = true;
        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].UpperBody = true;
        jointPoints[PositionIndex.rForearmBend.Int()].UpperBody = true;
        jointPoints[PositionIndex.rHand.Int()].UpperBody = true;
        jointPoints[PositionIndex.rThumb2.Int()].UpperBody = true;
        jointPoints[PositionIndex.rMid1.Int()].UpperBody = true;
        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].UpperBody = true;
        jointPoints[PositionIndex.lForearmBend.Int()].UpperBody = true;
        jointPoints[PositionIndex.lHand.Int()].UpperBody = true;
        jointPoints[PositionIndex.lThumb2.Int()].UpperBody = true;
        jointPoints[PositionIndex.lMid1.Int()].UpperBody = true;
        // Face
        jointPoints[PositionIndex.lEar.Int()].UpperBody = true;
        jointPoints[PositionIndex.lEye.Int()].UpperBody = true;
        jointPoints[PositionIndex.rEar.Int()].UpperBody = true;
        jointPoints[PositionIndex.rEye.Int()].UpperBody = true;
        jointPoints[PositionIndex.Nose.Int()].UpperBody = true;
        // etc
        jointPoints[PositionIndex.spine.Int()].UpperBody = true;
        jointPoints[PositionIndex.neck.Int()].UpperBody = true;

        // Parent and Child Settings
        // Right Arm
        jointPoints[PositionIndex.rShldrBend.Int()].Child = jointPoints[PositionIndex.rForearmBend.Int()];
        jointPoints[PositionIndex.rForearmBend.Int()].Child = jointPoints[PositionIndex.rHand.Int()];
        jointPoints[PositionIndex.rForearmBend.Int()].Parent = jointPoints[PositionIndex.rShldrBend.Int()];
        //jointPoints[PositionIndex.rHand.Int()].Parent = jointPoints[PositionIndex.rForearmBend.Int()];

        // Left Arm
        jointPoints[PositionIndex.lShldrBend.Int()].Child = jointPoints[PositionIndex.lForearmBend.Int()];
        jointPoints[PositionIndex.lForearmBend.Int()].Child = jointPoints[PositionIndex.lHand.Int()];
        jointPoints[PositionIndex.lForearmBend.Int()].Parent = jointPoints[PositionIndex.lShldrBend.Int()];
        //jointPoints[PositionIndex.lHand.Int()].Parent = jointPoints[PositionIndex.lForearmBend.Int()];

        // Fase

        // Right Leg
        jointPoints[PositionIndex.rThighBend.Int()].Child = jointPoints[PositionIndex.rShin.Int()];
        jointPoints[PositionIndex.rShin.Int()].Child = jointPoints[PositionIndex.rFoot.Int()];
        jointPoints[PositionIndex.rFoot.Int()].Child = jointPoints[PositionIndex.rToe.Int()];
        jointPoints[PositionIndex.rFoot.Int()].Parent = jointPoints[PositionIndex.rShin.Int()];
         
        // Left Leg
        jointPoints[PositionIndex.lThighBend.Int()].Child = jointPoints[PositionIndex.lShin.Int()];
        jointPoints[PositionIndex.lShin.Int()].Child = jointPoints[PositionIndex.lFoot.Int()];
        jointPoints[PositionIndex.lFoot.Int()].Child = jointPoints[PositionIndex.lToe.Int()];
        jointPoints[PositionIndex.lFoot.Int()].Parent = jointPoints[PositionIndex.lShin.Int()];

        // etc
        jointPoints[PositionIndex.spine.Int()].Child = jointPoints[PositionIndex.neck.Int()];
        jointPoints[PositionIndex.neck.Int()].Child = jointPoints[PositionIndex.head.Int()];
        //jointPoints[PositionIndex.head.Int()].Child = jointPoints[PositionIndex.Nose.Int()];
        //jointPoints[PositionIndex.hip.Int()].Child = jointPoints[PositionIndex.spine.Int()];

        // Line Child Settings
        // Right Arm
        AddSkeleton(PositionIndex.rShldrBend, PositionIndex.rForearmBend, true);
        AddSkeleton(PositionIndex.rForearmBend, PositionIndex.rHand, true);
        AddSkeleton(PositionIndex.rHand, PositionIndex.rThumb2, true);
        AddSkeleton(PositionIndex.rHand, PositionIndex.rMid1, true);

        // Left Arm
        AddSkeleton(PositionIndex.lShldrBend, PositionIndex.lForearmBend, true);
        AddSkeleton(PositionIndex.lForearmBend, PositionIndex.lHand, true);
        AddSkeleton(PositionIndex.lHand, PositionIndex.lThumb2, true);
        AddSkeleton(PositionIndex.lHand, PositionIndex.lMid1, true);

        // Face
        //AddSkeleton(PositionIndex.lEar, PositionIndex.lEye);
        //AddSkeleton(PositionIndex.lEye, PositionIndex.Nose);
        //AddSkeleton(PositionIndex.rEar, PositionIndex.rEye);
        //AddSkeleton(PositionIndex.rEye, PositionIndex.Nose);
        AddSkeleton(PositionIndex.lEar, PositionIndex.Nose, true);
        AddSkeleton(PositionIndex.rEar, PositionIndex.Nose, true);

        // Right Leg
        AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShin, false);
        AddSkeleton(PositionIndex.rShin, PositionIndex.rFoot, false);
        AddSkeleton(PositionIndex.rFoot, PositionIndex.rToe, false);

        // Left Leg
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShin, false);
        AddSkeleton(PositionIndex.lShin, PositionIndex.lFoot, false);
        AddSkeleton(PositionIndex.lFoot, PositionIndex.lToe, false);

        // etc
        AddSkeleton(PositionIndex.spine, PositionIndex.neck, true);
        AddSkeleton(PositionIndex.neck, PositionIndex.head, true);
        AddSkeleton(PositionIndex.head, PositionIndex.Nose, true);
        AddSkeleton(PositionIndex.neck, PositionIndex.rShldrBend, true);
        AddSkeleton(PositionIndex.neck, PositionIndex.lShldrBend, true);
        AddSkeleton(PositionIndex.rThighBend, PositionIndex.rShldrBend, true);
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.lShldrBend, true);
        AddSkeleton(PositionIndex.rShldrBend, PositionIndex.abdomenUpper, true);
        AddSkeleton(PositionIndex.lShldrBend, PositionIndex.abdomenUpper, true);
        AddSkeleton(PositionIndex.rThighBend, PositionIndex.abdomenUpper, true);
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.abdomenUpper, true);
        AddSkeleton(PositionIndex.lThighBend, PositionIndex.rThighBend, true);

        // Set Inverse
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Transform.position, jointPoints[PositionIndex.lThighBend.Int()].Transform.position, jointPoints[PositionIndex.rThighBend.Int()].Transform.position);
        foreach (var jointPoint in jointPoints)
        {
            if (jointPoint.Transform != null)
            {
                jointPoint.InitRotation = jointPoint.Transform.rotation;
            }

            if (jointPoint.Child != null)
            {
                jointPoint.Inverse = GetInverse(jointPoint, jointPoint.Child, forward);
                jointPoint.InverseRotation = jointPoint.Inverse * jointPoint.InitRotation;
            }
        }
        var hip = jointPoints[PositionIndex.hip.Int()];
        initPosition = transform.position;
        //initPosition = jointPoints[PositionIndex.hip.Int()].Transform.position;
        hip.Inverse = Quaternion.Inverse(Quaternion.LookRotation(forward));
        hip.InverseRotation = hip.Inverse * hip.InitRotation;

        // For Head Rotation
        var head = jointPoints[PositionIndex.head.Int()];
        head.InitRotation = jointPoints[PositionIndex.head.Int()].Transform.rotation;
        var gaze = jointPoints[PositionIndex.Nose.Int()].Transform.position - jointPoints[PositionIndex.head.Int()].Transform.position;
        head.Inverse = Quaternion.Inverse(Quaternion.LookRotation(gaze));
        head.InverseRotation = head.Inverse * head.InitRotation;

        var lHand = jointPoints[PositionIndex.lHand.Int()];
        var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
        lHand.InitRotation = lHand.Transform.rotation;
        lHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Transform.position - jointPoints[PositionIndex.lMid1.Int()].Transform.position, lf));
        lHand.InverseRotation = lHand.Inverse * lHand.InitRotation;

        var rHand = jointPoints[PositionIndex.rHand.Int()];
        var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
        rHand.InitRotation = jointPoints[PositionIndex.rHand.Int()].Transform.rotation;
        rHand.Inverse = Quaternion.Inverse(Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Transform.position - jointPoints[PositionIndex.rMid1.Int()].Transform.position, rf));
        rHand.InverseRotation = rHand.Inverse * rHand.InitRotation;

        jointPoints[PositionIndex.hip.Int()].score3D = 1f;
        jointPoints[PositionIndex.neck.Int()].score3D = 1f;
        jointPoints[PositionIndex.Nose.Int()].score3D = 1f;
        jointPoints[PositionIndex.head.Int()].score3D = 1f;
        jointPoints[PositionIndex.spine.Int()].score3D = 1f;

        return JointPoints;
    }

    public void SetNose(float x, float y, float z)
    {
        if (this.Nose == null)
        {
            this.Nose = new GameObject(this.name + "_Nose");
        }
        var ani = ModelObject.GetComponent<Animator>();
        var t = ani.GetBoneTransform(HumanBodyBones.Head);
        this.Nose.transform.position = new Vector3(t.position.x + x, t.position.y + y, t.position.z + z);

    }

    public void SetSettings(AvatarSetting setting)
    {
        this.name = setting.AvatarName;

        ResetPosition(setting.PosX, setting.PosY, setting.PosZ);
        SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
        SetScale(setting.Scale);
        SetZScale(setting.DepthScale);

        SetSkeleton(setting.SkeletonVisible == 1);

        SkeletonX = setting.SkeletonPosX;
        SkeletonY = setting.SkeletonPosY;
        SkeletonZ = setting.SkeletonPosZ;
        SkeletonScale = setting.SkeletonScale;
    }

    public void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void SetZScale(float zScale)
    {
        ZScale = zScale;
    }

    public void SetSkeleton(bool flag)
    {
        foreach (var sk in Skeletons)
        {
            sk.LineObject.SetActive(flag);
        }
    }

    public void ResetPosition(float x, float y, float z)
    {
        transform.position = new Vector3(x, y, z);
        initPosition = transform.position;
    }

    public Vector3 GetHeadPosition()
    {
        return anim.GetBoneTransform(HumanBodyBones.Head).position;
    }

    public void Show()
    {
    }

    public void Hide()
    {
        SetSkeleton(false);
    }

    public void SetUpperBodyMode(bool upper)
    {
        UpperBodyMode = upper;
        UpperBodyF = upper ? 0f : 1f;
    }

    private float tallHeadNeck;
    private float tallNeckSpine;
    private float tallSpineCrotch;
    private float tallThigh;
    private float tallShin;
    public float EstimatedScore;
    private float VisibleThreshold = 0.05f;

    public void PoseUpdate()
    {
        // 身長からｚの移動量を算出
        tallHeadNeck = Vector3.Distance(jointPoints[PositionIndex.head.Int()].Pos3D, jointPoints[PositionIndex.neck.Int()].Pos3D);
        tallNeckSpine = Vector3.Distance(jointPoints[PositionIndex.neck.Int()].Pos3D, jointPoints[PositionIndex.spine.Int()].Pos3D);

        jointPoints[PositionIndex.lToe.Int()].Visibled = jointPoints[PositionIndex.lToe.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.rToe.Int()].Visibled = jointPoints[PositionIndex.rToe.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.lFoot.Int()].Visibled = jointPoints[PositionIndex.lFoot.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.rFoot.Int()].Visibled = jointPoints[PositionIndex.rFoot.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.lShin.Int()].Visibled = jointPoints[PositionIndex.lShin.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.rShin.Int()].Visibled = jointPoints[PositionIndex.rShin.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.lThighBend.Int()].Visibled = jointPoints[PositionIndex.lThighBend.Int()].score3D < VisibleThreshold ? false : true;
        jointPoints[PositionIndex.rThighBend.Int()].Visibled = jointPoints[PositionIndex.rThighBend.Int()].score3D < VisibleThreshold ? false : true;

        var leftShin = 0f;
        var rightShin = 0f;
        var shinCnt = 0;
        if (jointPoints[PositionIndex.lShin.Int()].Visibled && jointPoints[PositionIndex.lFoot.Int()].Visibled)
        {
            leftShin = Vector3.Distance(jointPoints[PositionIndex.lShin.Int()].Pos3D, jointPoints[PositionIndex.lFoot.Int()].Pos3D);
            shinCnt++;
        }
        if (jointPoints[PositionIndex.rShin.Int()].Visibled && jointPoints[PositionIndex.rFoot.Int()].Visibled)
        {
            rightShin = Vector3.Distance(jointPoints[PositionIndex.rShin.Int()].Pos3D, jointPoints[PositionIndex.rFoot.Int()].Pos3D);
            shinCnt++;
        }
        if (shinCnt != 0)
        {
            tallShin = (rightShin + leftShin) / shinCnt;
        }

        var rightThigh = 0f;
        var leftThigh = 0f;
        var thighCnt = 0;
        if (jointPoints[PositionIndex.rThighBend.Int()].Visibled && jointPoints[PositionIndex.rShin.Int()].Visibled)
        {
            rightThigh = Vector3.Distance(jointPoints[PositionIndex.rThighBend.Int()].Pos3D, jointPoints[PositionIndex.rShin.Int()].Pos3D);
            thighCnt++;
        }
        if (jointPoints[PositionIndex.lThighBend.Int()].Visibled && jointPoints[PositionIndex.lShin.Int()].Visibled)
        {
            leftThigh = Vector3.Distance(jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.lShin.Int()].Pos3D);
            thighCnt++;
        }
        if (thighCnt != 0)
        {
            tallThigh = (rightThigh + leftThigh) / 2f;
        }

        var crotch = (jointPoints[PositionIndex.rThighBend.Int()].Pos3D + jointPoints[PositionIndex.lThighBend.Int()].Pos3D) / 2f;
        tallSpineCrotch = Vector3.Distance(jointPoints[PositionIndex.spine.Int()].Pos3D, crotch);

        if(tallThigh <= 0.01f && tallShin <= 0.01f)
        {
            tallThigh = tallNeckSpine;
            tallShin = tallNeckSpine;
        }
        else if (tallShin <= 0.01f)
        {
            tallShin = tallThigh;
        }
        else if (tallThigh <= 0.01f)
        {
            tallThigh = tallShin;
        }

        var t = tallHeadNeck + tallNeckSpine + tallSpineCrotch + (tallThigh + tallShin) * UpperBodyF;

        tall = t * 0.7f + prevTall * 0.3f;
        prevTall = tall;

        var dz = (centerTall - tall) / centerTall * ZScale;

        var score = 0f;
        var scoreCnt = 0;
        for (var i = 0; i < 24; i++)
        {
            if (!jointPoints[i].Visibled)
            {
                continue;
            }

            if (jointPoints[i].Child != null)
            {
                score += jointPoints[i].score3D;
                scoreCnt++;
            }
        }

        if (scoreCnt > 0)
        {
            EstimatedScore = score / scoreCnt;
        }
        else
        {
            EstimatedScore = 0f;
        }

        if(EstimatedScore < 0.03f)
        {
            return;
        }
        // センターの移動と回転
        var forward = TriangleNormal(jointPoints[PositionIndex.hip.Int()].Pos3D, jointPoints[PositionIndex.lThighBend.Int()].Pos3D, jointPoints[PositionIndex.rThighBend.Int()].Pos3D);
        transform.position = jointPoints[PositionIndex.hip.Int()].Pos3D * movementScale + new Vector3(initPosition.x, initPosition.y, initPosition.z + dz);
        jointPoints[PositionIndex.hip.Int()].Transform.rotation = Quaternion.LookRotation(forward) * jointPoints[PositionIndex.hip.Int()].InverseRotation;

        // 各ボーンの回転
        foreach (var jointPoint in jointPoints)
        {
            if (this.UpperBodyMode && !jointPoint.UpperBody)
            {
                continue;
            }

            if (!jointPoint.Visibled)
            {
                continue;
            }

            if (jointPoint.Parent != null)
            {
                var fv = jointPoint.Parent.Pos3D - jointPoint.Pos3D;
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, jointPoint.VecFlag * fv) * jointPoint.InverseRotation;
            }
            else if (jointPoint.Child != null)
            {
                if (!jointPoint.Child.Visibled)
                {
                    continue;
                }
                jointPoint.Transform.rotation = Quaternion.LookRotation(jointPoint.Pos3D - jointPoint.Child.Pos3D, forward) * jointPoint.InverseRotation;
            }
        }

        // Head Rotation
        var gaze = jointPoints[PositionIndex.Nose.Int()].Pos3D - jointPoints[PositionIndex.head.Int()].Pos3D;
        var f = TriangleNormal(jointPoints[PositionIndex.Nose.Int()].Pos3D, jointPoints[PositionIndex.rEar.Int()].Pos3D, jointPoints[PositionIndex.lEar.Int()].Pos3D);
        var head = jointPoints[PositionIndex.head.Int()];
        head.Transform.rotation = Quaternion.LookRotation(gaze, f) * head.InverseRotation;

        // Wrist rotation (Test code)
        var lHand = jointPoints[PositionIndex.lHand.Int()];
        if (lHand.Visibled)
        {
            var lf = TriangleNormal(lHand.Pos3D, jointPoints[PositionIndex.lMid1.Int()].Pos3D, jointPoints[PositionIndex.lThumb2.Int()].Pos3D);
            lHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.lThumb2.Int()].Pos3D - jointPoints[PositionIndex.lMid1.Int()].Pos3D, lf) * lHand.InverseRotation;
        }
        var rHand = jointPoints[PositionIndex.rHand.Int()];
        if (rHand.Visibled)
        {
            var rf = TriangleNormal(rHand.Pos3D, jointPoints[PositionIndex.rThumb2.Int()].Pos3D, jointPoints[PositionIndex.rMid1.Int()].Pos3D);
            rHand.Transform.rotation = Quaternion.LookRotation(jointPoints[PositionIndex.rThumb2.Int()].Pos3D - jointPoints[PositionIndex.rMid1.Int()].Pos3D, rf) * rHand.InverseRotation;
        }

        foreach (var sk in Skeletons)
        {
            if (this.UpperBodyMode && !sk.upperBody)
            {
                continue;
            }

            var s = sk.start;
            var e = sk.end;

            sk.Line.SetPosition(0, new Vector3(s.Pos3D.x * SkeletonScale + SkeletonX, s.Pos3D.y * SkeletonScale + SkeletonY, s.Pos3D.z * SkeletonScale + SkeletonZ));
            sk.Line.SetPosition(1, new Vector3(e.Pos3D.x * SkeletonScale + SkeletonX, e.Pos3D.y * SkeletonScale + SkeletonY, e.Pos3D.z * SkeletonScale + SkeletonZ));
        }
    }

    Vector3 TriangleNormal(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 d1 = a - b;
        Vector3 d2 = a - c;

        Vector3 dd = Vector3.Cross(d1, d2);
        dd.Normalize();

        return dd;
    }

    private Quaternion GetInverse(JointPoint p1, JointPoint p2, Vector3 forward)
    {
        return Quaternion.Inverse(Quaternion.LookRotation(p1.Transform.position - p2.Transform.position, forward));
    }

    private void AddSkeleton(PositionIndex s, PositionIndex e, bool upperBody)
    {
        var sk = new Skeleton()
        {
            LineObject = new GameObject(this.name + "_Skeleton" +  (Skeletons.Count + 1).ToString("00")),
            start = jointPoints[s.Int()],
            end = jointPoints[e.Int()],
            upperBody = upperBody,
        };

        sk.Line = sk.LineObject.AddComponent<LineRenderer>();
        sk.Line.startWidth = 0.04f;
        sk.Line.endWidth = 0.01f;
        //頂点の数を決める
        sk.Line.positionCount = 2;
        sk.Line.material = SkeletonMaterial;

        Skeletons.Add(sk);
    }

    public static bool IsPoseUpdate = false;

    private void Update()
    {
        if (jointPoints != null)
        {
            /**/
            if (IsPoseUpdate)
            {
                PoseUpdate();
            }
            else
            {
                /*
                foreach (var jp in jointPoints)
                {
                    KalmanUpdate(jp);
                }
                PoseUpdate();
                foreach (var jp in jointPoints)
                {
                    jp.Now3D = jp.Pos3D;
                }
                */
            }
            IsPoseUpdate = false;
            /**/
        }
    }

    void KalmanUpdate(VNectModel.JointPoint measurement)
    {
        measurementUpdate(measurement);
        measurement.Pos3D.x = measurement.X.x + (measurement.Now3D.x - measurement.X.x) * measurement.K.x;
        measurement.Pos3D.y = measurement.X.y + (measurement.Now3D.y - measurement.X.y) * measurement.K.y;
        measurement.Pos3D.z = measurement.X.z + (measurement.Now3D.z - measurement.X.z) * measurement.K.z;
        measurement.X = measurement.Pos3D;
    }

    void measurementUpdate(VNectModel.JointPoint measurement)
    {
        measurement.K.x = (measurement.P.x + VNectModel.JointPoint.Q) / (measurement.P.x + VNectModel.JointPoint.Q + VNectModel.JointPoint.R);
        measurement.K.y = (measurement.P.y + VNectModel.JointPoint.Q) / (measurement.P.y + VNectModel.JointPoint.Q + VNectModel.JointPoint.R);
        measurement.K.z = (measurement.P.z + VNectModel.JointPoint.Q) / (measurement.P.z + VNectModel.JointPoint.Q + VNectModel.JointPoint.R);
        measurement.P.x = VNectModel.JointPoint.R * (measurement.P.x + VNectModel.JointPoint.Q) / (VNectModel.JointPoint.R + measurement.P.x + VNectModel.JointPoint.Q);
        measurement.P.y = VNectModel.JointPoint.R * (measurement.P.y + VNectModel.JointPoint.Q) / (VNectModel.JointPoint.R + measurement.P.y + VNectModel.JointPoint.Q);
        measurement.P.z = VNectModel.JointPoint.R * (measurement.P.z + VNectModel.JointPoint.Q) / (VNectModel.JointPoint.R + measurement.P.z + VNectModel.JointPoint.Q);
    }
}
