using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonController 
{
    public enum STATE
    {

    };

    public enum EVENT
    {

        ENTER,
        UPDATE,
        EXIT
    };


    [SerializeField] int SkeletonHP;
    [SerializeField] int SkeletonDMG;
    [SerializeField] int SkeletonSpeed;

    private Animator _animator;

    private bool isWalking;
    private bool isBlocking;
    private bool isHurt;
    private bool isIdle;
    private bool isDead;

}
