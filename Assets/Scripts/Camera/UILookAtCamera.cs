using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class UILookAtCamera : MonoBehaviour
{
    [SerializeField] Transform target;
    private void OnEnable()
    {
        StartCoroutine(LookAtTarget());
    }

    private IEnumerator LookAtTarget()
    {
        while (this.gameObject.activeInHierarchy)
        {
            Vector3 dir = target.position - transform.position;

            transform.rotation = Quaternion.LookRotation(dir);
            yield return null;
        }
    }
}
