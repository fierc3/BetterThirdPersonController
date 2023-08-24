using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootIk : MonoBehaviour
{
    private Animator animator;

    Vector3 rightFootPosition;
    Vector3 leftFootPosition;

    [SerializeField]
    float offset_y = 1;
    [SerializeField]
    float offset_x = 1;

    [SerializeField]
    float footOffset = 1;

    LayerMask ignoreLayers;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        ignoreLayers = ~(1 << 6);

        if (!animator.isHuman)
        {
            this.enabled = false;
            Debug.LogWarning("FootIK: Animator is not humanoid");
        }

    }


    private void OnAnimatorIK(int layerIndex)
    {
        // foot ik logic
        float rightWeight = animator.GetFloat("rightFoot");
        float leftWeight = animator.GetFloat("leftFoot");

        if (DetectFootPosition(animator.GetBoneTransform(HumanBodyBones.RightFoot), ref rightFootPosition))
        {

            animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightWeight);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootPosition);
        }

        if (DetectFootPosition(animator.GetBoneTransform(HumanBodyBones.LeftFoot), ref leftFootPosition))
        {

            animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftWeight);
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootPosition);
        }

    }

    bool DetectFootPosition(Transform originTransfom, ref Vector3 position)
    {
        var origin = originTransfom.position;
        origin.y += offset_y;
        var destination = originTransfom.position;
        destination.y = transform.position.y;

        RaycastHit hit;
        Debug.DrawLine(origin, destination);
        if (Physics.Linecast(origin, destination, out hit, ignoreLayers))
        {
            position = hit.point;
            position.y += footOffset;
            return true;
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
