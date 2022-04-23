using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class CharacterBehaviour : MonoBehaviour
{
    //initializing variables
    public Transform MoveTo;
    public float speed = 1;
    private float dx;
    private float dz;
        
    private float normVelX = 0, normVelZ = 0;
    private float kinSeekVelX = 0, kinSeekVelZ = 0;
    public LayerMask lm;
    public float rsat = 0.5f;
    public float t2t = 0.55f;
    double maxAccel = 12.25;
    private bool dodge = false;
    public bool seekFlee = true;
    public float ypos = 1;
    
    void Update()
    {
        //calculates distance to find if wrapping is best
        dx = (MoveTo.position.x - transform.position.x);
        dz = (MoveTo.position.z - transform.position.z);

        Debug.DrawRay(transform.position, new Vector3(dx, 0, dz), Color.green);
        
        //depending on whether you are seeking or fleeing the apprpriate action is needed  
        if(seekFlee)
        {
            if(Mathf.Sqrt((kinSeekVelX * kinSeekVelX) + (kinSeekVelZ * kinSeekVelZ)) < 0.5f  || dodge)
            {
                //Debug.Log("here3");
                //Debug.Log(dx + "    "  + dz + "     " + Mathf.Sqrt((dx * dx) + (dz * dz)));
               
                if (Mathf.Sqrt((dx * dx) + (dz * dz)) < 1)
                {
                    //Debug.Log("here");
                    kinArrive();
                    dodge = true;
                   
                    
                }
                else
                {
                    //Debug.Log("here2");
                    kinArriveII();
                    dodge = false;                                  
                }
            }
            else
            {
                coneRotate();
            }
        }
        else
        {
            dx = -dx;
            dz = -dz;
            if (Mathf.Sqrt((dx * dx) + (dz * dz)) < 1)
            {
                KinFlee();
                dodge = true;
            }
            else
            {
                KinFleeII();
                dodge = false;
            }
        }
    }

    //kinematic arrive
    void kinArrive()
    {
        float fx = dodge? dx : transform.forward.x;
        float fz = dodge? dz : transform.forward.z;
        if (Mathf.Sqrt( ((dx * dx) + (dz * dz))) > rsat)
        {
            normVelX =  fx/ Mathf.Sqrt(((fx * fx) + (fz * fz)));
            normVelZ = fz / Mathf.Sqrt(((fx * fx) + (fz * fz)));
            kinSeekVelX = speed * normVelX;
            kinSeekVelZ = speed * normVelZ; 
                    
        }
        else
        {
            normVelX = fx / Mathf.Sqrt( ((fx * fx) + (fz * fz)));
            normVelZ = fz / Mathf.Sqrt( ((fx * fx) + (fz * fz)));
            float tempMaxVel = Mathf.Min(speed, (Mathf.Sqrt(((fx * fx) + (fz * fz))) / t2t));
            kinSeekVelX = tempMaxVel * normVelX;
            kinSeekVelZ = tempMaxVel * normVelZ;
        }
        
        float moveToPosx = transform.position.x + (kinSeekVelX * Time.deltaTime);
        float moveToPosz = transform.position.z + (kinSeekVelZ * Time.deltaTime);
        transform.position = new Vector3(moveToPosx, ypos, moveToPosz);
    }
    
    //kinematic arrive with rotating first
    void kinArriveII()
    {
        
        Debug.DrawRay(transform.position, transform.forward*10);
        float ang = Vector3.Angle(transform.forward, new Vector3(dx, 0, dz));
        if(ang > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dx, 0, dz), Vector3.up), 40*Time.deltaTime);
        }
        else
        {
            kinArrive();
        }
        
        
    }
    
    //kinematic arrive with cone availablility
    void coneArrive()
    {
        float fx = transform.forward.x;
        float fz = transform.forward.z;
        if (Mathf.Sqrt(((dx * dx) + (dz * dz))) > rsat)
        {
            normVelX =  fx/ Mathf.Sqrt(((fx * fx) + (fz * fz)));
            normVelZ = fz / Mathf.Sqrt(((fx * fx) + (fz * fz)));
            kinSeekVelX = speed * normVelX;
            kinSeekVelZ = speed * normVelZ; 
                    
        }
        else
        {
            normVelX = fx / Mathf.Sqrt(((fx * fx) + (fz * fz)));
            normVelZ = fz / Mathf.Sqrt(((fx * fx) + (fz * fz)));
            float tempMaxVel = Mathf.Min(speed, (Mathf.Sqrt(((fx * fx) + (fz * fz))) / t2t));
            kinSeekVelX = tempMaxVel * normVelX;
            kinSeekVelZ = tempMaxVel * normVelZ;
        }
        
        float moveToPosx = transform.position.x + (kinSeekVelX * Time.deltaTime);
        float moveToPosz = transform.position.z + (kinSeekVelZ * Time.deltaTime);
        transform.position = new Vector3(moveToPosx, ypos, moveToPosz);
    }
    
    //kinematic arrive but with rotating to a fist initial distance
    void coneRotate()
    {
        float ang = Vector3.Angle(transform.forward, new Vector3(dx, 0, dz));
        if(ang > 50f * Mathf.Sqrt(((normVelX * normVelX) + (normVelZ * normVelZ))))
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dx, 0, dz), Vector3.up), 5*Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dx, 0, dz), Vector3.up), 5*Time.deltaTime);
            coneArrive();
        }
    }
    
    //kinematic fee when close by
    void KinFlee()
    {
        
        if (Mathf.Sqrt((float) ((dx * dx) + (dz * dz))) > rsat)
        {
            normVelX = dx / Mathf.Sqrt((float) ((dx * dx) + (dz * dz)));
            normVelZ = dz / Mathf.Sqrt((float) ((dx * dx) + (dz * dz)));
            kinSeekVelX = speed * normVelX;
            kinSeekVelZ = speed * normVelZ; 
                    
        }
        else
        {
            normVelX = dx / Mathf.Sqrt((float) ((dx * dx) + (dz * dz)));
            normVelZ = dz / Mathf.Sqrt((float) ((dx * dx) + (dz * dz)));
            float tempMaxVel = Mathf.Min((float) speed, (float)(Mathf.Sqrt((float) ((dx * dx) + (dz * dz))) / t2t));
            kinSeekVelX = tempMaxVel * normVelX;
            kinSeekVelZ = tempMaxVel * normVelZ;
        }
        
        float moveToPosx = transform.position.x + (kinSeekVelX * Time.deltaTime);
        float moveToPosz = transform.position.z + (kinSeekVelZ * Time.deltaTime);
        transform.position = new Vector3(moveToPosx, ypos, moveToPosz);
        //Debug.Log(pInitX + ", " + pInitY);
    }
    
    //kinematic fee while rotating
    void KinFleeII()
    {
        Debug.DrawRay(transform.position, transform.forward*30);
        float ang = Vector3.Angle(transform.forward, new Vector3(dx, 0, dz));
        if(ang > 0.1f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(new Vector3(dx, 0, dz)), 40*Time.deltaTime);
        }
        else
        {
            KinFlee();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == lm)
            dodge = false;
    }
}
