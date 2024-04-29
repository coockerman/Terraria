using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Range(0, 1)]
    public float smoothTime;

    public Transform playerTransform;

    public Vector2 spawnPos {  get; }

    public void Spawn(Vector3 position)
    {
        GetComponent<Transform>().position = position;
    }

    public void FixedUpdate()
    {
        Vector3 pos = GetComponent<Transform>().position;

        pos.x = Mathf.Lerp(pos.x, playerTransform.position.x, smoothTime);
        pos.y = Mathf.Lerp(pos.y, playerTransform.position.y, smoothTime);

        GetComponent<Transform>().position = pos;
    }
}
