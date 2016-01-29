using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MemoryRecord
{
  public MemoryRecord(Vector3 pos, float timeSeen)
  {
      lastKnownPosition = pos;
      timeLastSeen = timeSeen;
  }

  public Vector3 lastKnownPosition;
  public float timeLastSeen;
};

public class Perception : MonoBehaviour 
{
    public float fov = 45.0f;
    public GameObject owner;
    public float rememberDuration = 2.0f;

    private CircleCollider2D circleCollider;
    private Dictionary<GameObject,MemoryRecord> memory = new Dictionary<GameObject,MemoryRecord>();
    private List<GameObject> recordsToRemove = new List<GameObject>();

    void OnEnable()
    {
        circleCollider = GetComponent<CircleCollider2D>();
    }

	void Start () 
    {
	
	}
	
	void Update () 
    {
        Vector3 left = Quaternion.Euler(0.0f, 0.0f, fov/2.0f) * owner.transform.right;
        Vector3 right = Quaternion.Euler(0.0f, 0.0f, -(fov / 2.0f)) * owner.transform.right;

        float r = circleCollider.radius*2.0f;
        Debug.DrawRay(transform.position, left*r, Color.red);
        Debug.DrawRay(transform.position, right*r, Color.red);

        recordsToRemove.Clear();
        foreach(var record in memory)
        {
            float currentTime = Time.time;
            float timeSeen = record.Value.timeLastSeen;

            Debug.DrawLine(transform.position, record.Value.lastKnownPosition, Color.green);

            if (currentTime - timeSeen > rememberDuration)
            {
                recordsToRemove.Add(record.Key);
            }
        }

        foreach (var item in recordsToRemove)
            memory.Remove(item);
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("enter" + other.ToString());
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject == owner)
            return;

        Vector3 toTarget = other.transform.position - owner.transform.position;
        float angle = Vector3.Angle(owner.transform.right, toTarget);

        if (angle <= fov/2.0f)
        {

            if (memory.ContainsKey(other.gameObject))
            {
                memory[other.gameObject].timeLastSeen = Time.time;
                memory[other.gameObject].lastKnownPosition = other.gameObject.transform.position;
            }
            else
            {
                MemoryRecord record = new MemoryRecord(other.transform.position, Time.time);
                memory.Add(other.gameObject, record);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {

    }

    void OnCollisionEnter2D(Collision2D collisionInfo)
    {
        Debug.Log("collision");
    }
}
