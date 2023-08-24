using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeClimb : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private float offsetFromWall = 0.3f;

    [SerializeField]
    private float climbSpeed = 1f;
    [SerializeField]
    private float rotateSpeed = 0.2f;

    private bool isClimbing = false;
    private bool isLerping = false;
    private bool isInPosition = false;
    private Vector3 startPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private Quaternion startRot = Quaternion.identity;
    private Quaternion endRot = Quaternion.identity;

    [SerializeField]
    private float positionOffset = 0.5f;


    private float speedMultiplier = 0.2f;

    Transform helper;
    float t = 0;



    private void Start()
    {
        helper = new GameObject().transform;
        helper.name = "helper";

        CheckForClimb();
    }

    public void CheckForClimb()
    {
        var origin = transform.position;
        origin.y += 1.4f;
        Vector3 dir = transform.forward;
        RaycastHit hit;

        Debug.DrawLine(origin, dir * 5);
        if (Physics.Raycast(origin,dir, out hit, 5))
        {
            helper.position = PosWithOffset(origin, hit.point);
            InitForClimb(hit);
        }
    }

    void InitForClimb(RaycastHit hit)
    {
        this.isClimbing = true;
        helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
        startPos = transform.position;
        targetPos = hit.point + (hit.normal * offsetFromWall);
        t = 0;
        isInPosition = false;
        animator.CrossFade("climb_idle", 2);
    }

    private void Update()
    {
        if (!isInPosition)
        {
            GetInPosition();
            return;
        }

        if (!isLerping)
        {
            float hor = Input.GetAxis("Horizontal");
            float vert = Input.GetAxis("Vertical");
            float m = Mathf.Abs(hor) + Mathf.Abs(vert);

            var h = helper.right * hor;
            var v = helper.up * vert;
            var moveDir = (h + v).normalized;

            if (moveDir == Vector3.zero) return;
            
            var canMove = CanMove(moveDir);

            if (!canMove) return;

            t = 0;
            isLerping = true;
            startPos = transform.position;
            //var tp = helper.position - transform.position;
            targetPos = helper.position;

        }
        else
        {
            t += Time.deltaTime;

            if(t > 1)
            {
                t = 1;
                isLerping = false;
            }

            var climbPosiiton = Vector3.Lerp(startPos, targetPos, t);
            transform.position  = climbPosiiton;
            transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, Time.deltaTime * rotateSpeed);
        }
    }

    bool CanMove(Vector3 targetMoveDir)
    {
        Debug.Log(targetMoveDir);

        var origin = transform.position;
        float distance = positionOffset;
        var dir = targetMoveDir;
        Debug.DrawRay(origin, dir * distance, Color.red);
        RaycastHit hit;

        if(Physics.Raycast(origin, dir, out hit, distance))
        {
            return false;
        }

        origin += targetMoveDir * distance;
        dir = helper.forward;
        float distanceForward = 0.5f;

        Debug.DrawRay (origin, dir * distanceForward, Color.blue);

        if(Physics.Raycast(origin, dir, out hit, distance))
        {
            helper.position = PosWithOffset(origin, hit.point);
            helper.rotation = Quaternion.LookRotation(-hit.normal);
            return true;
        }

        origin += dir * distanceForward;
        dir = -Vector3.up;

        Debug.DrawRay(origin, dir * distanceForward, Color.yellow);  

        if(Physics.Raycast(origin, dir, out hit, distanceForward))
        {
            float angle = Vector3.Angle(helper.up, hit.normal);
            if(angle < 40)
            {
                helper.position = PosWithOffset(origin, hit.point);
                helper.rotation = Quaternion.LookRotation(-hit.normal);
                return true;
            }
        }

        return false;
    }

    void GetInPosition()
    {
        t += Time.deltaTime;

        if(t > 1)
        {
            t = 1;
            isInPosition = true;
        }

        var tp = Vector3.Lerp(startPos, targetPos, t);
        transform.rotation = Quaternion.Slerp(transform.rotation, helper.rotation, Time.deltaTime * rotateSpeed);
        transform.position = tp;
    }

    Vector3 PosWithOffset(Vector3 origin, Vector3 target)
    {
        var direction = origin - target;
        direction.Normalize();
        var offset = direction * offsetFromWall;
        return target + offset;
    }

}
