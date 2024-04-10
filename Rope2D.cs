using System.Collections.Generic;
using UnityEngine;

public class Rope2D : MonoBehaviour
{
    public Transform player;

    public LineRenderer rope;
    public LayerMask collMask;

    [field:SerializeReference] public List<Vector2> RopePositions { get; set; } = new();
    private Vector3[] _ropePositionsV3Cache;
    
    private void Awake()
    {
        AddPosToRope(transform.position);
        _ropePositionsV3Cache = new Vector3[RopePositions.Count];
    }

    private void Update()
    {
        UpdateRopePositions();
        LastSegmentGoToPlayerPos();

        DetectCollisionEnter();
        if (RopePositions.Count > 2) DetectCollisionExits();
    }

    private void DetectCollisionEnter()
    {
        RaycastHit2D hit = Physics2D.Linecast(player.position, rope.GetPosition(RopePositions.Count - 2), collMask);
        if (hit)
        {
            var hitPoint = hit.point;
            if (hitPoint == RopePositions[^2]) return;
            RopePositions.RemoveAt(RopePositions.Count - 1);
            AddPosToRope(hit.point);    
        }
    }

    private void DetectCollisionExits()
    {
        RaycastHit2D hit = Physics2D.Linecast(player.position, rope.GetPosition(RopePositions.Count - 3), collMask);
        if (!hit)
        {
            RopePositions.RemoveAt(RopePositions.Count - 2);
        }
    }

    private void AddPosToRope(Vector2 pos)
    {
        RopePositions.Add(pos);
        RopePositions.Add(player.position); //Always the last pos must be the player
    }

    

    private void UpdateRopePositions()
    {
        if (_ropePositionsV3Cache.Length != RopePositions.Count)
            _ropePositionsV3Cache = new Vector3[RopePositions.Count];

        for (int i = 0; i < RopePositions.Count; i++)
            _ropePositionsV3Cache[i] = RopePositions[i];

        rope.positionCount = RopePositions.Count;
        rope.SetPositions(_ropePositionsV3Cache);
    }

    private Vector3[] ToVector3Array(Vector2[] v2Array)
    {
        Vector3[] v3 = new Vector3[v2Array.Length];
        for (int i = 0; i < v2Array.Length; i++)
        {
            v3[i] = v2Array[i];
        }

        return v3;
    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, player.position);
}
