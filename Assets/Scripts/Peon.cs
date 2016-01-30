using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Peon : MonoBehaviour, IMovable
{
    public float MaxSpeed = 1.0f;

    //IMovable
    public Vector3 velocity { get; set; }
    public float maxSpeed { get { return MaxSpeed; } }
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public float speed { get { return velocity.magnitude; } }
    public Vector3 heading { get { return (velocity.sqrMagnitude > 0.001f) ? velocity.normalized : Vector3.zero; } }
    public ArrayList GetNeighbours() { return mNeighbours; }

    SteeringBehaviors mSteering;
    ArrayList mNeighbours = new ArrayList();
    List<Vector2> mPath = new List<Vector2>();

    enum STATE
    {
        IDLE,
        MOVE
    };

    STATE mState;

    // Use this for initialization
    void Start ()
    {
        velocity = Vector3.right;
        //MaxSpeed = 2.0f;

        mSteering = new SteeringBehaviors(this);
        mSteering.SetFlag(Behavior.separation);
    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (mState)
        {
            case STATE.MOVE:
                OnMove();
                break;

            case STATE.IDLE:
                OnIdle();
                break;
        }

        Locomotion();
    }

    void OnMove()
    {

    }

    void OnIdle()
    {

    }

    public void MoveToPoint(Vector2 pos)
    {
        mState = STATE.MOVE;
        mSteering.SetFlag(Behavior.followPath);
        mPath = AStar.GetInstance().FindPath(transform.position, pos);
        mSteering.SetPath(mPath);
    }

    void Locomotion()
    {
        Vector3 steering = mSteering.Compute();
        velocity += steering * Time.deltaTime * 5.0f;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (steering.sqrMagnitude < float.Epsilon)
            velocity *= 0.85f;

        //movement
        transform.position += velocity * Time.deltaTime;
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
