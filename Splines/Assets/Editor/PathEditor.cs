using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;

    void OnSceneGUI() {
        Input();
        Draw();
    }

    void Input()
    {
        Event guiEvent = Event.current;
        Vector2 mousepos = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition).origin;
        if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.shift)
        {
            Undo.RecordObject(creator, "Add segment");
            path.AddSegment(mousepos); 
        }
    }
    void Draw()
    {
        for (int i = 0; i < path.NumSegments; i++)
        {
            Handles.color = Color.black;
            Vector2[] points = path.GetPointsInSegment(i);
            Handles.DrawLine(points[1], points[0]);
            Handles.DrawLine(points[2], points[3]);
            Handles.DrawBezier(points[0], points[3], points[1], points[2], Color.green, null, 2);

        }
        
        Handles.color = Color.red;
        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector2 newpos = Handles.FreeMoveHandle(path[i], Quaternion.identity, .1f, Vector2.zero, Handles.CylinderHandleCap);
            if (path[i] != newpos)
            {
                Undo.RecordObject(creator, "Move poin");
                path.MovePoint(i, newpos); 
            }
        }
    }

    void OnEnable()
    {
        creator = (PathCreator)target;
        if (creator.path == null)
        {
            creator.CreatePath(); 
        }
        path = creator.path; 
    }
}
