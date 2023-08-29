using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimbAnimHook : MonoBehaviour
{

    Animator anim;
    IKSnapshot ikBase;
    IKSnapshot current = new IKSnapshot();
    IKSnapshot next = new IKSnapshot();

    public float weight_rh;
    public float weight_lh;
    public float weight_rf;
    public float weight_lf;

    [SerializeField]
    private float wallOffset = 0.1f;

    Vector3 rh, lh, rf, lf;
    Transform helper;


    public void Init(FreeClimb freeClimb, Transform helper)
    {
        anim = freeClimb.animator;
        ikBase = freeClimb.baseIkSnapshot;
        this.helper = helper;
    }

    public void CreatePositions(Vector3 origin)
    {
        var ik = CreateSnaphot(origin);
        CopySnaphot(ref current, ik);

        UpdateIkPosition(AvatarIKGoal.LeftFoot, current.lf);
        UpdateIkPosition(AvatarIKGoal.RightFoot, current.rf);
        UpdateIkPosition(AvatarIKGoal.LeftHand, current.lh);
        UpdateIkPosition(AvatarIKGoal.RightHand, current.rh);

        UpdateIkWeight(AvatarIKGoal.LeftFoot, 1);
        UpdateIkWeight(AvatarIKGoal.RightFoot, 1);
        UpdateIkWeight(AvatarIKGoal.LeftHand, 1);
        UpdateIkWeight(AvatarIKGoal.RightHand, 1);
    }

    public IKSnapshot CreateSnaphot(Vector3 origin)
    {
        var r = new IKSnapshot();
        r.lh = GetPositionPercise(LocalToWorld(ikBase.lh));
        r.rh = GetPositionPercise(LocalToWorld(ikBase.rh));
        r.lf = GetPositionPercise(LocalToWorld(ikBase.lf));
        r.rf = GetPositionPercise(LocalToWorld(ikBase.rf));
        return r;
    }

    Vector3 GetPositionPercise(Vector3 origin)
    {
        Vector3 r = origin;
        var targetOrigin = origin;
        Vector3 dir = helper.forward;
        targetOrigin += -(dir * 0.2f);

        if(Physics.Raycast(origin, dir, out RaycastHit hit, 1.5f))
        {

            r = hit.point + (hit.normal * wallOffset);
        }

        return r;
    }

    Vector3 LocalToWorld(Vector3 local)
    {
        var world = helper.position;
        world += helper.right * local.x;
        world += helper.forward * local.z;
        world += helper.up * local.y;
        return world;
    }

    public void CopySnaphot(ref IKSnapshot to, IKSnapshot from)
    {
        to.rh = from.rh;
        to.lh = from.lh;
        to.lf = from.lf;
        to.rf = from.rf;
    }

    public void UpdateIkPosition(AvatarIKGoal goal, Vector3 pos)
    {
        switch (goal)
        {
            case AvatarIKGoal.LeftFoot:
                lf = pos;
                break;
            case AvatarIKGoal.RightFoot:
                rf = pos;
                break;
            case AvatarIKGoal.LeftHand:
                lh = pos;
                break;
            case AvatarIKGoal.RightHand:
                rh = pos;
                break;
        }
    }

    public void UpdateIkWeight(AvatarIKGoal goal, float weight)
    {
        switch (goal)
        {
            case AvatarIKGoal.LeftFoot:
                weight_lf = weight;
                break;
            case AvatarIKGoal.RightFoot:
                weight_rf = weight;
                break;
            case AvatarIKGoal.LeftHand:
                weight_lh = weight;
                break;
            case AvatarIKGoal.RightHand:
                weight_rh = weight;
                break;
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        this.SetIkPos(AvatarIKGoal.LeftFoot, lf, weight_lf);
        this.SetIkPos(AvatarIKGoal.RightFoot, rf, weight_rf);
        this.SetIkPos(AvatarIKGoal.LeftHand, lh, weight_lh);
        this.SetIkPos(AvatarIKGoal.RightHand, rh, weight_rh);
        
    }

    void SetIkPos(AvatarIKGoal goal, Vector3 targetPos, float weight)
    {
        anim.SetIKPositionWeight(goal, weight);
        anim.SetIKPosition(goal, targetPos);

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
