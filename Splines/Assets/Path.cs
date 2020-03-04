﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class Path
{
    [SerializeField, HideInInspector]
    List<Vector2> points;
    [SerializeField, HideInInspector]
    bool isClosed;
    [SerializeField, HideInInspector]
    bool autoSetControlPoints;

    public Path(Vector2 centre)
    {
        points = new List<Vector2>
        {
                centre + Vector2.left,
                centre + (Vector2.left + Vector2.up) * .5f,
                centre + (Vector2.right + Vector2.down) * .5f,
                centre + Vector2.right
        };
    }

    public Vector2 this[int i]
    {
        get{ return points[i]; }
    }

    public bool AutoSetControlPoints
    {
        get { return autoSetControlPoints;  }
        set
        {
            if (autoSetControlPoints != value)
            {
                autoSetControlPoints = value;
                if (autoSetControlPoints)
                    AutoSetAllControlPoints();
            }
        }
    }
    public int NumPoints { get { return points.Count; } }

    public int NumSegments
    { get { return points.Count / 3; } }

    public void AddSegment(Vector2 anchorPos)
    {
        points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
        points.Add(((points[points.Count - 1]) + anchorPos) * .5f);
        points.Add(anchorPos);
        if (autoSetControlPoints)
            AutoSetAllAffectedControlPoints(points.Count - 1);
    }

    public Vector2[] GetPointsInSegment(int i)
    {
        return new Vector2[] { points[i * 3], points[i * 3 + 1], points[i * 3 + 2], points[LoopIndex(i * 3 + 3)] };
    }

    public void MovePoint(int i, Vector2 pos)
    {
        Vector2 deltaMove = pos - points[i]; //nuova posizione - la precedente
        if(i%3 == 0 || !autoSetControlPoints)
        { 
            points[i] = pos;

            if (autoSetControlPoints)
                AutoSetAllAffectedControlPoints(i);
            else
            {
                if (i % 3 == 0)
                {//Se è un anchor points, lo sposto con il punto
                    if (i + 1 < points.Count || isClosed) //anchor point 3 + 1 sarà il control point -> 4
                        points[LoopIndex(i + 1)] += deltaMove;

                    if (i - 1 >= 0 || isClosed)  //anchor point 3-1 sarà il control point -> 2
                        points[LoopIndex(i - 1)] += deltaMove;
                }
                else //se non è un anchor
                {
                    bool nextPointIsAnchor = (i + 1) % 3 == 0;
                    int correspondingControllIndex = (nextPointIsAnchor) ? i + 2 : i - 2;
                    int anchorIndex = (nextPointIsAnchor) ? i + 1 : i - 1;
                    if (correspondingControllIndex >= 0 && correspondingControllIndex < points.Count || isClosed)
                    {
                        float dst = (points[LoopIndex(anchorIndex)] - points[LoopIndex(correspondingControllIndex)]).magnitude;
                        Vector2 dir = (points[LoopIndex(anchorIndex)] - pos).normalized;
                        points[LoopIndex(correspondingControllIndex)] = points[LoopIndex(anchorIndex)] + dir * dst;
                    }
                }
            }
        }
    }

    public void ToggleClosed()
    {
        isClosed = !isClosed;
        if (isClosed)
        {
            points.Add(points[points.Count - 1] * 2 - points[points.Count - 2]);
            points.Add(points[0] * 2 - points[1]);
            if (autoSetControlPoints)
            {
                AutoSetAnchorControlPoints(0);
                AutoSetAnchorControlPoints(points.Count - 3);
            }
        }
        else
        {
            points.RemoveRange(points.Count - 2, 2);
            if (autoSetControlPoints)
                AutoSetStartAndEndControls();
            
        }
    }

    void AutoSetAllAffectedControlPoints(int updatedAnchorIndex)
    {
        for (int i = updatedAnchorIndex -3; i < updatedAnchorIndex + 3; i += 3)
        {
            if (i >= 0 && i < points.Count || isClosed) {
                AutoSetAnchorControlPoints(LoopIndex(i));
            }
        }
        AutoSetStartAndEndControls();
    }

    void AutoSetAllControlPoints()
    {
        for (int i = 0; i < points.Count; i += 3)
            AutoSetAnchorControlPoints(i);
        
        AutoSetStartAndEndControls();
    }

    void AutoSetAnchorControlPoints(int anchorIndex)
    {
        Vector2 anchorPos = points[anchorIndex];
        Vector2 dir = Vector2.zero;
        float[] neighdist = new float[2];
        if(anchorIndex - 3 >= 0 || isClosed)
        {
            Vector2 offset = points[LoopIndex(anchorIndex - 3)] - anchorPos;
            dir += offset.normalized;
            neighdist[0] = offset.magnitude;
        }

        if (anchorIndex + 3 >= 0 || isClosed)
        {
            Vector2 offset = points[LoopIndex(anchorIndex + 3)] - anchorPos;
            dir -= offset.normalized;
            neighdist[1] = -offset.magnitude;
        }

        dir.Normalize(); 

        for (int i = 0; i < 2; i++)
        {
            int controlIndex = anchorIndex + i * 2 - 1;
            if (controlIndex >= 0 && controlIndex < points.Count || isClosed)
            {
                points[LoopIndex(controlIndex)] = anchorPos + dir * neighdist[i] * .5f; 

            }
        }
    }

    void AutoSetStartAndEndControls()
    {
        if (!isClosed)
        {
            points[1] = (points[0] + points[2]) * .5f;
            points[points.Count - 2] = (points[points.Count - 1] + points[points.Count - 3]) * .5f;
        }
    }
    int LoopIndex(int i)
    {
        return (i + points.Count) % points.Count;
    }
}

