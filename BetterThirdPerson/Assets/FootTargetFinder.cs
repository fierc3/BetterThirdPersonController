using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootTargetFinder : MonoBehaviour
{
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private float raycastDistance = 1.5f;

    [SerializeField]
    private GameObject target;
    [SerializeField]
    private int raycastCount = 8;

    Vector3 nearestPoint = Vector3.zero;
    float nearestDistance = float.MaxValue;

    private void Update()
    {
        nearestPoint = Vector3.zero;
        nearestDistance = float.MaxValue;
        // For each foot, perform a raycast to find the ground.
        RaycastAngledFoot(transform);
        target.transform.position = nearestPoint;
    }

    private void RaycastAngledFoot(Transform foot)
    {
        RaycastHit hit;
        Vector3 raycastStart = foot.position;
        //Vector3 raycastEnd = raycastStart + direction * raycastDistance;


        var angleDistancePerCast = 190f / raycastCount;

        for (int i = 0; i < raycastCount; i++)
        {
            // Calculate the current raycast direction based on the angle.
            float angle = i * angleDistancePerCast;
            Vector3 raycastDirection = Quaternion.Euler(0f, angle, 0f) * (Vector3.forward + Vector3.down * 2 );


            if (Physics.Raycast(raycastStart, transform.TransformDirection(raycastDirection), out hit, raycastDistance, groundLayer))
            {
                // Draw a line to visualize the raycast.
                Debug.DrawLine(raycastStart, hit.point, Color.green);

                // Adjust the foot position to match the hit point.
                //foot.position = hit.point;

                float distance = Vector3.Distance(raycastStart, hit.point);

                // Update the nearest point if the new hit point is closer.
                if (distance < nearestDistance)
                {
                    nearestPoint = hit.point;
                    nearestDistance = distance;
                }
            }
            else
            {
                // If no ground was detected, draw the raycast line as red.
                /*
                 * end point: (ray.direction.normalized*line_length) + ray.origin

start point: ray.origin
                 */
                Debug.DrawLine(raycastStart, raycastDirection.normalized * raycastDistance + raycastStart, Color.red);
                //Debug.DrawRay(raycastStart, raycastDirection, Color.red);
            }
        }

    }

    private void RaycastDown(Transform foot)
    {
        RaycastHit hit;
        Vector3 raycastStart = foot.position;
        //Vector3 raycastEnd = raycastStart + direction * raycastDistance;

        var nearestPoint = Vector3.zero;
        var nearestDistance = float.MaxValue;
        var angleDistancePerCast = 180f / raycastCount;

        for (int i = 0; i < raycastCount; i++)
        {
            // Calculate the current raycast direction based on the angle.
            float angle = i * angleDistancePerCast;
            Vector3 raycastDirection = Quaternion.Euler(0f, angle, 0f) * (Vector3.forward + Vector3.down);


            if (Physics.Raycast(raycastStart, raycastDirection, out hit, raycastDistance, groundLayer))
            {
                // Draw a line to visualize the raycast.
                Debug.DrawLine(raycastStart, hit.point, Color.green);

                // Adjust the foot position to match the hit point.
                //foot.position = hit.point;
            }
            else
            {
                // If no ground was detected, draw the raycast line as red.
                /*
                 * end point: (ray.direction.normalized*line_length) + ray.origin

start point: ray.origin
                 */
                Debug.DrawLine(raycastStart, raycastDirection.normalized * raycastDistance + raycastStart, Color.red);
                //Debug.DrawRay(raycastStart, raycastDirection, Color.red);
            }
        }

    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.nearestPoint, 0.01f);
    }
}
