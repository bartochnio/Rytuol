﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Peon : MonoBehaviour, IMovable, IPeon
{
// public vars
    public float MaxSpeed = 1.0f;

    public Payload Payload;


//IMovable
    public Vector3 velocity { get; set; }
    public float maxSpeed { get { return MaxSpeed; } }
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public float speed { get { return velocity.magnitude; } }
    public Vector3 heading { get { return (velocity.sqrMagnitude > 0.001f) ? velocity.normalized : Vector3.zero; } }
    public ArrayList GetNeighbours() { return mNeighbours; }


// private vars
    SteeringBehaviors mSteering;
    ArrayList mNeighbours = new ArrayList();
    List<Vector2> mPath = new List<Vector2>();
    Transform mTarget;
    int mQueueSlot = -1;
	ForestItemEnum payLoadForestItem;
	VillageItemEnum payLoadVillageItem;
	Temple.ID targetTemple;


    enum State
    {
        eIdle,
        eMovingToPeonsArea,

        eCapturingSavage,
        eGatheringFruit,
        eTamingAnimal,

        eStoringItem,

        eRetrievingItem,

        eOfferingItem,

		eSacrificingItem
    }
    State actionState = State.eIdle;


// MonoBehaviour
//
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
			case State.eIdle:
				OnIdle();
			break;

			case State.eMovingToPeonsArea:
			case State.eGatheringFruit:
				OnMove();
			break;

			case State.eTamingAnimal:
            case State.eCapturingSavage:        
                OnPursue();
            break;

			case State.eStoringItem:
				OnStoring();
			break;

			case State.eRetrievingItem:
	            OnRetrieving();
	        break;

			case State.eOfferingItem:
				OnOffering ();
			break;

			case State.eSacrificingItem:
				OnSacrificing();
			break;
		}

        //for (int i = 0; i < mPath.Count - 1; ++i)
        //{
        //    Debug.DrawLine(mPath[i], mPath[i + 1], Color.red);
        //}
    }


// private functions
	void OnIdle()
	{
	}

	void OnPursue()
	{
		MoveToPoint(mTarget.position);
		Locomotion();
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
        }

        Locomotion();
    }

	void OnStoring()
	{
		if (mSteering.IsPathFinished())
		{
			Payload.HidePayload();

			Village.GetGlobalInstance().StoreItem(transform.position, payLoadForestItem);

			MoveToPeonsArea();
		}

		Locomotion();
	}

    void OnRetrieving()
    {
        if (mSteering.IsPathFinished())
        {
            Payload.HidePayload();
        }

        Locomotion();
    }

    void OnOffering()
    {
        if (mSteering.IsPathFinished())
        {
			actionState = State.eIdle;
            SacrificeQueue.GetInstance().ClaimSlot(this, mQueueSlot);
        }

        Locomotion();
    }

	void OnSacrificing()
	{
		if (mSteering.IsPathFinished())
		{
			FinishSacrifice ();
		}

		Locomotion();
	}

	void FinishSacrifice() {
		Payload.HidePayload();
		MoveToPeonsArea();
	}
    

    void MoveToPoint(Vector2 pos)
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
                var villageItem = mTarget.GetComponent<IVillageItem>();
                this.RetrieveVillageItem(villageItem);
                villageItem.Unselect();
            }
        }

		if (actionState == State.eSacrificingItem)
		{
			Temple collidingTemple = other.GetComponent<Temple> ();

			if (collidingTemple != null && targetTemple == collidingTemple.templeId)
			{
				FinishSacrifice ();
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


// IPeon
//
	public void MoveToPeonsArea()
	{
		actionState = State.eMovingToPeonsArea;
		MoveToPoint(Village.GetGlobalInstance().PeonsArea.AnyLocation);
	}

	public void StoreForestItem(IForestItem item)
	{
		payLoadForestItem = item.ItemType;
		actionState = State.eStoringItem;

		switch (item.ItemType)
		{
		case ForestItemEnum.eFruit:
			MoveToPoint(Village.GetGlobalInstance().FruitsArea.AnyLocation);
			Payload.ShowPayload(VillageItemEnum.eFruit);
			break;

		case ForestItemEnum.eAnimal:
			MoveToPoint(Village.GetGlobalInstance().AnimalsArea.AnyLocation);
			Payload.ShowPayload(VillageItemEnum.eAnimal);
			GameObject.Destroy((item as Critter).gameObject);
			break;

		case ForestItemEnum.eSavage:
			MoveToPoint(Village.GetGlobalInstance().SavagesArea.AnyLocation);
			Payload.ShowPayload(VillageItemEnum.eSavage);
			GameObject.Destroy((item as Critter).gameObject);
			break;
		}

		mTarget = null;
	}

	public void RetrieveVillageItem(IVillageItem item)
	{
		payLoadVillageItem = item.ItemType;
		actionState = State.eOfferingItem;
		Payload.ShowPayload(item.ItemType);

		MoveToPoint(SacrificeQueue.GetInstance().GetSlotPos(mQueueSlot));
		GameObject.Destroy((item as VillageItem).gameObject);
		mTarget = null;
	}

	public void SeekVillageItem(IVillageItem item, int queueSlot)
	{
		mTarget = ((VillageItem)item).transform;
		mQueueSlot = queueSlot;

		actionState = State.eRetrievingItem;

		MoveToPoint(mTarget.position);
	}

	public void SeekForestItem(IForestItem item)
	{
		switch (item.ItemType)
		{
		case ForestItemEnum.eFruit:
			mTarget = ((ForestItem)item).transform;
			MoveToPoint(mTarget.position);
			actionState = State.eGatheringFruit;
			break;

		case ForestItemEnum.eSavage:
			mTarget = ((Critter)item).transform;
			actionState = State.eCapturingSavage;
			break;

		case ForestItemEnum.eAnimal:
			mTarget = ((Critter)item).transform;
			actionState = State.eTamingAnimal;
			break;
		}
	}

	public void Sacrifice (Temple.ID templeId, Vector3 templeLocation)
	{
		targetTemple = templeId;
		actionState = State.eSacrificingItem;
		MoveToPoint(templeLocation);
	}

	public VillageItemEnum ItemToSacrifice {
		get { return payLoadVillageItem; }
	}
}
