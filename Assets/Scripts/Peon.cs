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
    public float maxSpeed { get { return MaxSpeed; } set { MaxSpeed = value; } }
    public Vector3 position { get { return transform.position; } set { transform.position = value; } }
    public float speed { get { return velocity.magnitude; } }
    public Vector3 heading { get { return (velocity.sqrMagnitude > 0.001f) ? velocity.normalized : Vector3.zero; } }
    public ArrayList GetNeighbours() { return mNeighbours; }


// private vars
    SteeringBehaviors mSteering;
    ArrayList mNeighbours = new ArrayList();
    List<Waypoint> mPath = new List<Waypoint>();
    Transform mTarget;
    int mQueueSlot = -1;
	ForestItemEnum payLoadForestItem;
	VillageItemEnum payLoadVillageItem;
	Temple targetTemple;
    Animator anim; 

    enum State
    {
        eIdle,
        eMovingToPeonsArea,

        eCapturingSavage,
        eGatheringFruit,
        eTamingAnimal,

        eStoringItem,

        eRetrievingItem,

        eQueueingItem,

		eOfferingItem,

        eGetSavageForConversion,
        eConvertSavage
    }
    State actionState = State.eIdle;


    void OnEnable()
    {
        Messenger.AddListener("speedBoost", SpeedBoost);
    }

    void OnDisable()
    {
        Messenger.RemoveListener("speedBoost", SpeedBoost);
    }

    float counter = 0.0f;
    float prevMaxSpeed;
    bool mSpeedBoost = false;
    void SpeedBoost()
    {
        prevMaxSpeed = maxSpeed;
        maxSpeed = 4.0f;
        mSpeedBoost = true;
    }

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
        if (anim == null)
            anim = GetComponent<Animator>();
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

			case State.eQueueingItem:
				OnQueueing ();
			break;

			case State.eOfferingItem:
				OnOffering();
			break;

            case State.eGetSavageForConversion:
                OnConversion();
                break;

            case State.eConvertSavage:
                OnChapel();
                break;

        }

        //for (int i = 0; i < mPath.Count - 1; ++i)
        //{
        //    Debug.DrawLine(mPath[i].pos, mPath[i + 1].pos, Color.red);
        //}

        if (mSpeedBoost)
        {
            counter += Time.deltaTime;
            if (counter >= 5.0f)
            {
                maxSpeed = prevMaxSpeed;
                mSpeedBoost = false;
                counter = 0.0f;
            }
        }
    }


// private functions

	void OnIdle()
	{
        anim.SetTrigger("IDLE");
	}

	void OnPursue()
	{
		MoveToPoint(mTarget.position);
		Locomotion();
	}
		
    void OnConversion()
    {
        Locomotion();
    }

    void OnChapel()
    {
        if (mSteering.IsPathFinished())
        {
            Payload.HidePayload();
            MoveToPeonsArea();

            Chapel.GetInstance().Indoctrinate();
        }

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

	void OnQueueing()
    {
        if (mSteering.IsPathFinished())
        {
			actionState = State.eIdle;
            SacrificeQueue.GetInstance().ClaimSlot(this, mQueueSlot);
        }

        Locomotion();
    }

	void OnOffering()
	{
		if (mSteering.IsPathFinished())
		{
			FinishSacrifice ();
		}

		Locomotion();
	}

	void FinishSacrifice() {
        Debug.Log(" Finishing Sacrifice " + ItemToSacrifice);
        if (ItemToSacrifice == VillageItemEnum.eSelfSacrifice)
        {
            targetTemple.ReciveSacrifice(VillageItemEnum.eSavage);
            actionState = State.eIdle;
            Kill();
        }
        else
        {
            Payload.HidePayload();
            MoveToPeonsArea();
            targetTemple.ReciveSacrifice(ItemToSacrifice);
        }
		mQueueSlot = -1;
	}
    

    void MoveToPoint(Vector2 pos)
    {
        mSteering.SetFlag(Behavior.followPath);
        mSteering.SetTarget(pos);
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

        if (heading.magnitude > 0.01f)
        {
            if (heading.x > 0 && heading.y > 0)
                anim.SetTrigger("UR");
            else if (heading.x < 0 && heading.y > 0)
                anim.SetTrigger("UL");
            else if (heading.x < 0 && heading.y < 0)
                anim.SetTrigger("DL");
            else if (heading.x > 0 && heading.y < 0)
                anim.SetTrigger("DR");
        }
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

				var tree = forestItem as ForestItem;
				if (tree != null) {
					tree.ApplePicked ();
				}
            } 
            else
            {
                var villageItem = mTarget.GetComponent<IVillageItem>();
                villageItem.Unselect();

                if (actionState == State.eGetSavageForConversion)
                {
                    this.RetrieveSavage(villageItem);
                }
                else
                {         
                    this.RetrieveVillageItem(villageItem);   
                }
            }
        }

		if (actionState == State.eOfferingItem)
		{
			Temple collidingTemple = other.GetComponent<Temple> ();

			if (collidingTemple != null && targetTemple == collidingTemple)
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
            item.ApplePicked();
			break;

		case ForestItemEnum.eAnimal:
			MoveToPoint (Village.GetGlobalInstance ().AnimalsArea.AnyLocation);
			Payload.ShowPayload (VillageItemEnum.eAnimal);

			Forest.GetGlobalInstance ().OnAnimalTamed ();
			GameObject.Destroy ((item as Critter).gameObject);
			break;

		case ForestItemEnum.eSavage:
			MoveToPoint(Village.GetGlobalInstance().SavagesArea.AnyLocation);
			Payload.ShowPayload(VillageItemEnum.eSavage);

			Forest.GetGlobalInstance ().OnSavageCaptured ();
			GameObject.Destroy((item as Critter).gameObject);
			break;
		}

		mTarget = null;
	}

	public void RetrieveVillageItem(IVillageItem item)
	{
        IStorageArea a = Village.GetGlobalInstance().AnimalsArea;
        switch ( item.ItemType)
        {
            case VillageItemEnum.eFruit:
                a = Village.GetGlobalInstance().FruitsArea;
                break;
            case VillageItemEnum.eSavage:
                a = Village.GetGlobalInstance().SavagesArea;
                break;
        }

        VillageItem sI = item as VillageItem;
        if ( a.retriveList.Contains( sI ))
        {
            a.retriveList.Remove(sI);
        }

		payLoadVillageItem = item.ItemType;
		actionState = State.eQueueingItem;
		Payload.ShowPayload(item.ItemType);

		MoveToPoint(SacrificeQueue.GetInstance().GetSlotPos(mQueueSlot));
        GameObject.Destroy((item as VillageItem).gameObject);
        mTarget = null;
	}

    public void RetrieveSavage(IVillageItem item)
    {
        Payload.ShowPayload(VillageItemEnum.eSavage);
        GameObject.Destroy((item as VillageItem).gameObject);

        actionState = State.eConvertSavage;
        MoveToPoint(Chapel.GetInstance().GetEntrace());
    }

    public void SeekVillageItem(IVillageItem item, int queueSlot)
	{
		mTarget = ((VillageItem)item).transform;
		mQueueSlot = queueSlot;

		actionState = State.eRetrievingItem;

		MoveToPoint(mTarget.position);
	}

    public void SeekSavageToConvert(IVillageItem item)
    {
        mTarget = ((VillageItem)item).transform;
        actionState = State.eGetSavageForConversion;
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

	public void Sacrifice (Temple temple, Vector3 templeLocation)
	{
		targetTemple = temple;
		actionState = State.eOfferingItem;
		MoveToPoint(templeLocation);
	}

    public void SelfSacrifice( int queueSlot)
    {
        actionState = State.eQueueingItem;
        payLoadVillageItem = VillageItemEnum.eSelfSacrifice;
        Payload.ShowPayload(VillageItemEnum.eSelfSacrifice);
        mQueueSlot = queueSlot;
		MoveToPoint(SacrificeQueue.GetInstance().GetSlotPos(mQueueSlot));
    }
	public VillageItemEnum ItemToSacrifice {
		get { return payLoadVillageItem; }
	}

	public bool IsSafeToKill()
    {
	    return actionState == State.eIdle && mQueueSlot == -1;	
	}

	public void Kill() {
		if (mTarget != null) {
			var fi = mTarget.GetComponent<IForestItem> ();
			if (fi != null) {
				fi.Unselect ();
			}
			else {
				var vi = mTarget.GetComponent<IVillageItem> ();
				vi.Unselect ();
			}
		}

		if (actionState == State.eIdle) {
			Village.GetGlobalInstance ().UnregisterPeon (this); // unregister itself if waiting in peons area
		}
        GameObject.Find("ThunderStorm").transform.position = this.gameObject.transform.position;
        GameObject.Find("ThunderStorm").GetComponent<Animator>().SetTrigger("Thunder");
        GameObject.Find("Background").GetComponent<Animator>().SetTrigger("Thunder");
        GameObject.Destroy (gameObject);
	}
}
