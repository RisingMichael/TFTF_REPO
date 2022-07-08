using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject pointsOfInterest;

    private const float speedFactor = 0.2f;

    private Vector3 target;

    private void Awake()
    {
        target = ChooseTarget();
    }

    private void FixedUpdate()
    {
        float step = speedFactor * Time.fixedDeltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target, step);

        if (Vector3.Distance(transform.position, target) < 0.001f) target = ChooseTarget();
    }

    private Vector3 ChooseTarget()
    {
        int numberOfPoints = pointsOfInterest.transform.childCount;
        int targetIndex = Random.Range(0, numberOfPoints - 1);
        return pointsOfInterest.transform.GetChild(targetIndex).position;
    }

}
