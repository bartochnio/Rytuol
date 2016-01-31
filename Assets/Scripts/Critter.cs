using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Critter : MonoBehaviour, IMovable, IForestItem
{
    public float MaxSpeed;
    public ForestItemEnum itemType;
    public AudioSource sfx;
    public AudioClip click;
    GameObject smoke;
    // private vars
    bool bSelected;

    //IMovable
    public Vector3 velocity { get; set; }
    public float maxSpeed { get { return MaxSpeed; }}
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public float speed { get { return velocity.magnitude; } }
    public Vector3 heading { get { return (velocity.sqrMagnitude > 0.001f) ? velocity.normalized : Vector3.zero; } }
    public ArrayList GetNeighbours() { return mNeighbours; }

    SteeringBehaviors mSteering;
    ArrayList mNeighbours = new ArrayList();
    List<Waypoint> mPath = new List<Waypoint>();

    //IForestItem

    // MonoBehaviour
    //
  

    void OnMouseDown()
    {
        Select();
    }

    public void Select()
    {
        if (bSelected)
            return;
        bSelected = true;

        SetColor(Color.red);
        sfx.PlayOneShot(sfx.clip);
        Village.GetGlobalInstance().OrderCaptureItem(this);
    }

    // IForestItem
    public ForestItemEnum ItemType
    {
        get { return itemType; }
    }

    public Vector3 Location
    {
        get
        {
            Vector3 pos = transform.position;
            return new Vector3(pos.x, 0.0f, pos.z);
        }
    }

    public void Unselect()
    {
        SetColor(Color.white);
        bSelected = false;
        transform.rotation = Quaternion.identity;
    }

    // Use this for initialization
    void Start ()
    {
        velocity = Vector3.right;
        //MaxSpeed = 2.0f;

        mSteering = new SteeringBehaviors(this);
        Vector2 target = Forest.GetGlobalInstance().AnyLocation;//NavMesh2D.GetInstance().GetRandomPos();
        mSteering.SetTarget(target);

        mPath = AStar.GetInstance().FindPath(transform.position, target);
        mSteering.SetPath(mPath, false);
        if (sfx == null)
            sfx = GameObject.Find("Audio/AudioSFX").GetComponent<AudioSource>();
        smoke = Resources.Load("smoke") as GameObject;
        //INTERNAL
        //mSteering.SetFlag(Behavior.separation);
        //mSteering.SetFlag(Behavior.alignment);
        //mSteering.SetFlag(Behavior.cohesion);
        mSteering.SetFlag(Behavior.followPath);
        //mSteering.SetFlag(Behavior.wander2d);
    }
	
	// Update is called once per frame
	void Update ()
    {
        Vector3 steering = mSteering.Compute();

        velocity += steering * Time.deltaTime * 5.0f;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if (steering.sqrMagnitude < float.Epsilon)
            velocity *= 0.85f;

        //movement
        transform.position += velocity * Time.deltaTime;

        //heading
        //if (velocity.sqrMagnitude > 0.01f)
        //    transform.right = velocity.normalized;

        if (mSteering.IsPathFinished())
        {
            Vector2 target = Forest.GetGlobalInstance().AnyLocation;
            mSteering.SetTarget(target);
            mPath = AStar.GetInstance().FindPath(transform.position, target);
            mSteering.SetPath(mPath, false);
        }

        //for(int i = 0; i < mPath.Count-1; ++i)
        //{
        //    Debug.DrawLine(mPath[i], mPath[i + 1], Color.red);
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

    void SetColor(Color c)
    {
        SpriteRenderer render = GetComponent<SpriteRenderer>();
        render.color = c;
    }

	public bool IsSafeToKill {
		get {
			return !bSelected;
		}
	}

	public void Kill() {
        GameObject.Instantiate(smoke);
		GameObject.Destroy (gameObject);
	}
}
