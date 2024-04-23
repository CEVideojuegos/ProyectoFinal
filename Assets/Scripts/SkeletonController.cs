using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController : MonoBehaviour
{
    [SerializeField] int SkeletonHP;
    [SerializeField] int SkeletonDMG;
    [SerializeField] int SkeletonSpeed;

    private bool isWalking;
    private bool isBlocking;
    private bool isHurt;
    private bool isIdle;
    private bool isDead;
}
