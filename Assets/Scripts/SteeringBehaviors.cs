using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IMovable
{
    Vector3 velocity { get; set; }
    float maxSpeed { get; }
    Vector3 position { get; set; }
    float speed { get; }
    Vector3 heading { get; }

    ArrayList GetNeighbours();
}

//path wrapper for useful functions
public class WaypointPath
{
    public List<Waypoint> mWaypoints;
    
    private int mCurWaypointIdx = 0;
    public bool mLooped;

    public void Reset()
    {
        if (mWaypoints != null)
        {
            mWaypoints.Clear();
            mCurWaypointIdx = 0;
        }
    }

    public Vector3 GetCurrent()
    {
        return (Vector3)mWaypoints[mCurWaypointIdx].pos;
    }

    public void SetNext()
    {
        if (mLooped)
            mCurWaypointIdx = (mCurWaypointIdx + 1) % mWaypoints.Count;
        else if (mCurWaypointIdx < mWaypoints.Count - 1)
            ++mCurWaypointIdx;
    }

    public bool HasFinished()
    {
        return !mLooped && (mCurWaypointIdx == mWaypoints.Count - 1);
    }

    public int GetSize()
    {
        return mWaypoints == null ? 0 : mWaypoints.Count;
    }

    public void Smooth(Vector2 start, Vector2 target)
    {
        int idx = mCurWaypointIdx;

        if (mWaypoints.Count <= 3)
            return;

        Vector2 next = target;
        for (int i = (mWaypoints.Count-2); i >= idx; --i)
        {
            Vector3 vertA = mWaypoints[i].e1;
            Vector3 vertB = mWaypoints[i].e2;

            Vector3 deltaBA = vertB - vertA;
            Vector3 deltaNS = (Vector3)next - (Vector3)start;
            Vector3 P = new Vector3(-deltaBA.z, deltaBA.y, deltaBA.x);

            float h = (Vector2.Dot(vertA - (Vector3)start, P)) / Vector2.Dot(deltaNS, P);
            Vector3 p;
            if (h < 0.0f) p = vertA;
            else if (h > 1.0f) p = vertB;
            else p = (Vector3)start + deltaNS * h;

            Vector3 res = Utils.projectPointToSegment(p, start, next);
            Vector3 pathPoint = Utils.projectPointToSegment(res, vertA, vertB);
            mWaypoints[i].pos = pathPoint;
            next = pathPoint;
        }
    }
}

public enum Behavior
{
    seek =          0x00000001,
    flee =          0x00000002,
    arrive =        0x00000004,
    pursuit =       0x00000008,
    evade =         0x00000010,
    wander2d =      0x00000020,
    followPath =    0x00000040,
    separation =    0x00000080,
    alignment =     0x00000100,
    cohesion =      0x00000200
};

class SteeringBehaviors
{
    private Vector3 mWanderTarget = Vector3.zero;
    private WaypointPath mWaypointPath = new WaypointPath();
    private IMovable mOwner;
    private int mSteeringFlags = 0;
    private Vector2 mTarget = Vector2.zero;

    public SteeringBehaviors(IMovable owner)
    {
        mOwner = owner;
    }


    //helper functions

    public void SetFlag(Behavior f)
    {
        mSteeringFlags |= (int)f;
    }

    public bool IsPathFinished()
    {
        return mWaypointPath.HasFinished() && Vector2.Distance(mWaypointPath.GetCurrent(),mOwner.position) < 0.1f;
    }

    public void SetPath(List<Waypoint> path, bool loop = false)
    {
        mWaypointPath.Reset();
        mWaypointPath.mWaypoints = path;
        mWaypointPath.mLooped = loop;
    }

    public void SetTarget(Vector2 target)
    {
        mTarget = target;
    }

    private bool AppendForce(ref Vector3 total, Vector3 toAdd)
    {
        float len = toAdd.magnitude;
        float lenLeft = mOwner.maxSpeed - total.magnitude;

        if (lenLeft <= 0.0f)
            return false;

        if (len <= lenLeft)
        {
            total += toAdd;
        }
        else
        {
            toAdd = toAdd.normalized * lenLeft;
            total += toAdd;
        }

        return true;
    }

    private bool IsOn(Behavior b)
    {
        return (mSteeringFlags & (int)b) != 0;
    }

    public Vector3 Compute()
    {
        Vector3 total = Vector3.zero;

        Vector3 force = Vector3.zero;

        if (IsOn(Behavior.followPath))
        {
            force = FollowPath();
            if (!AppendForce(ref total, force))
                return total;
        }

        if (IsOn(Behavior.separation))
        {
            force = Separation();
            if (!AppendForce(ref total, force)) 
                return total;
        }

        if (IsOn(Behavior.alignment))
        {
            force = Alignment();
            if (!AppendForce(ref total, force))
                return total;
        }

        if (IsOn(Behavior.cohesion))
        {
            force = Cohesion();
            if (!AppendForce(ref total, force))
                return total;
        }

        if (IsOn(Behavior.wander2d))
        {
            force = Wander2D();
            if (!AppendForce(ref total, force))
                return total;
        }

        return total;
    }

    //basic behaviors

    public Vector3 Seek(Vector3 target)
    {
        Vector3 toTarget = (target - mOwner.position).normalized * mOwner.maxSpeed;

        //INTERNAL
        //Debug.DrawLine(mOwner.position, target, Color.red);

        return toTarget - mOwner.velocity;
    }

    public Vector3 Flee(Vector3 target, float panicDist)
    {
        if (Vector3.SqrMagnitude(target - mOwner.position) > panicDist*panicDist)
        {
            return Vector3.zero;
        }

        return -Seek(target);
    }

    public Vector3 Arrive(Vector3 target, float deceleration)
    {
        Vector3 toTarget = (target - mOwner.position);
        float dist = toTarget.magnitude;

        Debug.Assert(deceleration > 0.0f);

        if (dist > 0.0f)
        {
            float speed = dist / deceleration;
            speed = Mathf.Min(speed, mOwner.maxSpeed);

            Vector3 desiredVel = toTarget * speed / dist;
            return desiredVel - mOwner.velocity;
        }

        return Vector3.zero;
    }

    public Vector3 Pursuit(IMovable evader)
    {
        Vector3 toEvader = evader.position - mOwner.position;
        float relativeHeading = Vector3.Dot(mOwner.velocity.normalized, evader.velocity.normalized);

        if (Vector3.Dot(toEvader,mOwner.velocity.normalized) > 0.0f && (relativeHeading < -0.95f) )
        {
            return Seek(evader.position);
        }

        float predictedLen = toEvader.magnitude / (mOwner.maxSpeed + evader.speed);
        return Seek(evader.position + evader.velocity * predictedLen);
    }

    public Vector3 Evade(IMovable pursuer, float panicDist)
    {
        Vector3 toPursuer = pursuer.position - mOwner.position;

        float predictedLen = toPursuer.magnitude / (mOwner.maxSpeed + pursuer.speed);
        return Flee(pursuer.position + pursuer.velocity * predictedLen, panicDist);
    }
    
    public Vector3 Wander2D() //only in XY plane
    {
        float wanderJitter = 1.0f;
        float wanderRadius = 2.0f;
        float wanderDist = 8.0f;

        mWanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, Random.Range(-1.0f, 1.0f) * wanderJitter, 0.0f);

        mWanderTarget.Normalize();
        mWanderTarget *= wanderRadius;

        Vector3 target = mWanderTarget + mOwner.velocity.normalized*wanderDist;        
        return target - mOwner.position;
    }

    public Vector3 FollowPath()
    {
        float waypointDist = 0.5f;
        float deceleration = 0.5f;

        if (mWaypointPath.GetSize() == 0)
            return Vector3.zero;

        //do smoothing here
        mWaypointPath.Smooth(mOwner.position, mTarget);

        if (Vector3.SqrMagnitude(mOwner.position - mWaypointPath.GetCurrent()) < waypointDist * waypointDist)
        {
            mWaypointPath.SetNext();
        }

        if (!mWaypointPath.HasFinished())
            return Seek(mWaypointPath.GetCurrent());
        else
            return Arrive(mWaypointPath.GetCurrent(), deceleration);
    }

    //group behaviors
    
    public Vector3 Separation()
    {
        Vector3 steeringForce = Vector3.zero;

        foreach (object o in mOwner.GetNeighbours())
        {
            IMovable m = (IMovable)o;
            Vector3 toAgent = mOwner.position - m.position;

            steeringForce += toAgent.normalized / toAgent.magnitude;
        }

        return steeringForce;
    }

    public Vector3 Alignment()
    {
        Vector3 averageHeading = Vector3.zero;

        foreach (object o in mOwner.GetNeighbours())
        {
            IMovable m = (IMovable)o;
            averageHeading += m.heading;
        }

        int neighbourCount = mOwner.GetNeighbours().Count;
        if (neighbourCount > 0)
        {
            averageHeading /= neighbourCount;
            averageHeading -= mOwner.heading;
        }

        return averageHeading;
    }

    public Vector3 Cohesion()
    {
        Vector3 steeringForce = Vector3.zero;

        Vector3 centerOfMass = Vector3.zero;
        foreach (object o in mOwner.GetNeighbours())
        {
            IMovable m = (IMovable)o;
            centerOfMass += m.position;
        }

        int neighbourCount = mOwner.GetNeighbours().Count;
        if (neighbourCount > 0)
        {
            centerOfMass /= neighbourCount;
            steeringForce = Seek(centerOfMass);
        }

        return steeringForce;
    }
}
