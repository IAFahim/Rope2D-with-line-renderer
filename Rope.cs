using System.Collections.Generic;
using UnityEngine;

public class Rope2D : MonoBehaviour
{
    public Transform player;

    public LineRenderer rope;
    public LayerMask collMask;

    [field: SerializeReference] public List<Vector2> RopePositions { get; set; } = new();

    private void Awake() => AddPosToRope(transform.position);

    private void Update()
    {
        UpdateRopePositions();
        LastSegmentGoToPlayerPos();

        DetectCollisionEnter();
        if (RopePositions.Count > 2) DetectCollisionExits();
    }

    private void DetectCollisionEnter()
    {
        var position = player.position;
        RaycastHit2D hit = Physics2D.Raycast(position,
            (Vector2)rope.GetPosition(RopePositions.Count - 2) - (Vector2)position,
            Vector2.Distance(position, rope.GetPosition(RopePositions.Count - 2)), collMask);
        if (hit.collider != null)
        {
            RopePositions.RemoveAt(RopePositions.Count - 1);
            AddPosToRope(hit.point);
        }
    }

    private void DetectCollisionExits()
    {
        var position = player.position;
        RaycastHit2D hit = Physics2D.Raycast(position,
            (Vector2)rope.GetPosition(RopePositions.Count - 3) - (Vector2)position,
            Vector2.Distance(position, rope.GetPosition(RopePositions.Count - 3)), collMask);
        if (hit.collider == null)
        {
            RopePositions.RemoveAt(RopePositions.Count - 2);
        }
    }

    private void AddPosToRope(Vector2 _pos)
    {
        RopePositions.Add(_pos);
        RopePositions.Add(player.position); //Always the last pos must be the player
    }

    private void UpdateRopePositions()
    {
        rope.positionCount = RopePositions.Count;
        var v3Array = new Vector3[RopePositions.Count];
        var v2S = RopePositions.ToArray();
        for (var i = 0; i < v2S.Length; i++)
        {
            v3Array[i] = v2S[i];
        }

        rope.SetPositions(v3Array);
    }

    private void LastSegmentGoToPlayerPos() => rope.SetPosition(rope.positionCount - 1, player.position);
}
