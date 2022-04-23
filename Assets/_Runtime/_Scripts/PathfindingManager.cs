using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
//using UnityEditor.Animations;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    List<GridGraphNode> followPath =  new List<GridGraphNode>();
    public CharacterBehaviour follower;
    private int i = 1;
    private bool startRunning = false;
    public float moveToDist = 0.05f;
    public Animator anim;

    private void Start()
    {
        anim.SetBool("walking", false);
    }


    public void WalkPath(List<GridGraphNode> p)
    {
        followPath = p;
        follower.transform.position =
            new Vector3(followPath[0].transform.position.x, 0f, followPath[0].transform.position.z);
        startRunning = true;
        follower.GetComponent<CharacterBehaviour>().MoveTo = followPath[1].transform;
        followPath.RemoveAt(0);
    }

    private void Update()
    {
        //handles ensuring the character is always walking towards the correct node
        if(startRunning && followPath.Count != 0)
        {
            anim.SetBool("walking", true);
            //Debug.Log(followPath.Count);
            if (Vector3.Distance(follower.transform.position, followPath[0].transform.position)< moveToDist)
            {
                followPath.RemoveAt(0);
            }
            else
            {
                follower.GetComponent<CharacterBehaviour>().MoveTo = followPath[0].transform;
            }
        }
    }

}
