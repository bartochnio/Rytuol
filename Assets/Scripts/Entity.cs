using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Entity : MonoBehaviour, IMovable
{
    //IMovable
    public Vector3 velocity { get; set; }
    public float maxSpeed { get; set; }
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public float speed { get { return velocity.magnitude; } }
    public Vector3 heading { get { return (velocity.sqrMagnitude > 0.001f) ? velocity.normalized : Vector3.zero; } }
    public ArrayList GetNeighbours() { return mNeighbours; }

    //Custom Inspector stuff
    [HideInInspector]
    public bool Loop = false;

    [HideInInspector]
    public List<Vector3> Path = new List<Vector3>(); 

    //Private
    SteeringBehaviors mSteering;
    ArrayList mNeighbours = new ArrayList();

    void Start()
    {
        velocity = Vector3.right;
        maxSpeed = 2.0f;

        mSteering = new SteeringBehaviors(this);
        mSteering.SetPath(Path, Loop);
        
        //INTERNAL
        mSteering.SetFlag(Behavior.separation);
        mSteering.SetFlag(Behavior.alignment);
        mSteering.SetFlag(Behavior.cohesion);
        mSteering.SetFlag(Behavior.followPath);
        mSteering.SetFlag(Behavior.wander2d);
    }
	
	void Update ()
    {
        Vector3 steering = mSteering.Compute();

        velocity += steering*Time.deltaTime*5.0f;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (steering.sqrMagnitude < float.Epsilon)
            velocity *= 0.85f;

        //movement
        transform.position += velocity*Time.deltaTime;

        //heading
        if (velocity.sqrMagnitude > 0.01f)
            transform.right = velocity.normalized;

        //INTERNAL
        //Debug.DrawRay(transform.position, velocity/maxSpeed, Color.green);

        //foreach(object o in mNeighbours)
        //{
        //    IMovable m = (IMovable)o;
        //    Debug.DrawLine(transform.position, m.position, Color.red);
        //}
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == gameObject)
        {
            return;
        }

        IMovable movable = other.GetComponent<IMovable>();
        if (movable != null)
            mNeighbours.Add(movable);
    }

    void OnTriggerExit2D(Collider2D other)
    {

        IMovable movable = other.GetComponent<IMovable>();
        if (movable != null)
            mNeighbours.Remove(movable);
    }
}

