using System;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 1f;
    private int currentIndex = 0;
    private List<Vector2Int> waypoints;
    private bool initialize = false;

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isMapReady)
        {
            waypoints = GenerateMap2.pathPosition;

            if (waypoints != null && waypoints.Count != 0)
            {
                if (!initialize)
                {
                    initialize = true;
                    for (int i = 0; i < waypoints.Count; i++)
                    {
                        if (i == 0)
                        {
                            Vector3 nextPosition = new Vector3(waypoints[i+1].x, transform.position.y, waypoints[i+1].y);
                            transform.position = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].y);
                            Vector3 direction = (nextPosition - transform.position).normalized;

                            if (direction != Vector3.zero)
                            {
                                Quaternion targetRotation = Quaternion.LookRotation(direction);
                                transform.rotation = targetRotation;
                            }
                            return;
                        }
                    }
                }

                Vector3 targetPos = new Vector3(waypoints[currentIndex].x, transform.position.y, waypoints[currentIndex].y);
                transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                Vector3 directionNext = (targetPos - transform.position).normalized;

                if (directionNext != Vector3.zero)
                {
                    Quaternion targetRotationNext = Quaternion.LookRotation(directionNext);
                    transform.rotation = targetRotationNext;
                }

                if (Vector3.Distance(transform.position, targetPos) < 0.05f)
                {
                    currentIndex++;
                    if (currentIndex >= waypoints.Count)
                    {
                        currentIndex = waypoints.Count - 1;

                    }
                }
            }
            else
            {
                Debug.LogWarning("Brak danych œcie¿ki!");
                return;
            }
        }
    }
}
