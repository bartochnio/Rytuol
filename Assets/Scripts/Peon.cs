using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Peon : MonoBehaviour, IMovable, IPeon
{
    public float MaxSpeed = 1.0f;

    public Payload Payload;

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
    Transform mTarget;

    enum State
    {
        eIdle,

        eMovingToPeonsArea,

        eCapturingSavage,
        eGatheringFruit,
        eTamingAnimal,

        eStoringSavage,
        eStoringFruit,
        eStoringAnimal,

        eRetrievingSavage,
        eRetrievingFruit,
        eRetrievingAnimal,

        eOfferingSavage,
        eOfferingFruit,
        eOfferingAnimal
    }
    State actionState = State.eIdle;

    void Awake()
    {
        velocity = Vector3.right;
        //MaxSpeed = 2.0f;

        mSteering = new SteeringBehaviors(this);
        //mSteering.SetFlag(Behavior.separation);
    }

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
        switch (actionState)
        {
            case State.eMovingToPeonsArea:
            case State.eGatheringFruit:
                OnMove();
                break;

            case State.eTamingAnimal:
            case State.eCapturingSavage:        
                OnPursue();
                break;

            case State.eStoringFruit:
            case State.eStoringAnimal:
            case State.eStoringSavage:
                OnMove();
                break;

            case State.eIdle:
                OnIdle();
                break;
        }

        //for (int i = 0; i < mPath.Count - 1; ++i)
        //{
        //    Debug.DrawLine(mPath[i], mPath[i + 1], Color.red);
        //}
    }

    void OnMove()
    {
        if(mSteering.IsPathFinished())
        {
            if (actionState == State.eMovingToPeonsArea)
            {
                actionState = State.eIdle;
                Village.GetGlobalInstance().RegisterPeon(this);
            }
            else if (actionState == State.eStoringFruit || actionState == State.eStoringSavage || actionState == State.eStoringAnimal)
            {
                Payload.HidePayload();

                switch (actionState)
                {
                    case State.eStoringFruit: Village.GetGlobalInstance().StoreFruit(transform.position); break;
                    case State.eStoringSavage: Village.GetGlobalInstance().StoreSavage(transform.position); break;
                    case State.eStoringAnimal: Village.GetGlobalInstance().StoreAnimal(transform.position); break;
                }

                MoveToPeonsArea();
            }
        }

        Locomotion();
    }

    void OnPursue()
    {
        MoveToPoint(mTarget.position);
        Locomotion();
    }

    void OnIdle()
    {

    }

    public void MoveToPeonsArea()
    {
        actionState = State.eMovingToPeonsArea;
        MoveToPoint(Village.GetGlobalInstance().PeonsArea.AnyLocation);
    }

    public void StoreForestItem(IForestItem item)
    {
        switch (item.ItemType)
        {
            case ForestItemEnum.eFruit:
                actionState = State.eStoringFruit;
                MoveToPoint(Village.GetGlobalInstance().FruitsArea.AnyLocation);
                Payload.ShowPayload(VillageItemEnum.eFruit);
                break;

            case ForestItemEnum.eAnimal:
                actionState = State.eStoringAnimal;
                MoveToPoint(Village.GetGlobalInstance().AnimalsArea.AnyLocation);
                GameObject.Destroy((item as Critter).gameObject);
                Payload.ShowPayload(VillageItemEnum.eAnimal);
                break;

            case ForestItemEnum.eSavage:
                actionState = State.eStoringSavage;
                MoveToPoint(Village.GetGlobalInstance().SavagesArea.AnyLocation);
                GameObject.Destroy((item as Critter).gameObject);
                Payload.ShowPayload(VillageItemEnum.eSavage);
                break;
        }

        mTarget = null;
    }

    public void SeekVillageItem(IVillageItem item)
    {

    }

    public void SeekForestItem(IForestItem item)
    {
        switch (item.ItemType)
        {
            case ForestItemEnum.eSavage:
                mTarget = ((Critter)item).transform;
                actionState = State.eCapturingSavage;
                break;

            case ForestItemEnum.eFruit:
                mTarget = ((ForestItem)item).transform;
                MoveToPoint(mTarget.position);
                actionState = State.eGatheringFruit;
                break;

            case ForestItemEnum.eAnimal:
                mTarget = ((Critter)item).transform;
                actionState = State.eTamingAnimal;
                break;
        }
    }

    public void MoveToPoint(Vector2 pos)
    {
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

        if (other.transform == mTarget)
        {
            var forestItem = mTarget.GetComponent<IForestItem>();
            if (forestItem != null)
            {
                this.StoreForestItem(forestItem);
                forestItem.Unselect();
            } 
            else
            {
                actionState = State.eIdle;
            }
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
