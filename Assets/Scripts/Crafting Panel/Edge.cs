using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge : MonoBehaviour
{
    public enum facing {Up, Down, Left, Right}
    public facing myFacing;

    public enum Shape {TrianglePos, TriangleNeg, Square, Circle, Smooth, Wave}
    public Shape shape = Shape.Smooth;

    public Vector2 attachedCoordination;

    public Tetris myTetris;
    public Tetris oppositeTetris = null;

    public List<Edge> touchingEdges = new List<Edge>();

    void Start()
    {
        GetComponentInParent<Tetris>().allEdges.Add(this);
        myTetris = GetComponentInParent<Tetris>();
    }

    public void RefreshState() //refresh if it connects to another Edge, and if so, revursively refresh all edges.
    {
        if (touchingEdges.Count == 0) return;
        if (oppositeTetris != null) return;
    
        oppositeTetris = touchingEdges[0].getTetris();
        touchingEdges[0].RefreshState();
    }

    public void ResetState() //clear out the connection information
    {
        if(oppositeTetris != null)
        {
            foreach (Edge e in oppositeTetris.allEdges)
            {
                if (e.oppositeTetris == myTetris)
                {
                    e.oppositeTetris = null;
                }
            }

        }
        oppositeTetris = null;
    }

    private void OnTriggerEnter2D(Collider2D collision) //To log if there is a connected edge
    {
        if (collision.GetComponent<Edge>() != null && checkFacingMatch(myFacing, collision.GetComponent<Edge>().myFacing) && checkShapeMatch(shape, collision.GetComponent<Edge>().shape) )
        {
            touchingEdges.Add(collision.GetComponent<Edge>());
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.GetComponent<Edge>() != null)
        {
            touchingEdges.Remove(collision.GetComponent<Edge>());
        }
    }
    
    public Tetris getTetris() { return myTetris; }
    public bool isConnected() { return oppositeTetris != null; }
    public Tetris getOppositeTetris() { return oppositeTetris; }
    public Edge getOppositeEdge() { return touchingEdges[0]; }
    public Vector3 getOppositeEdgeDistance() {return touchingEdges[0].transform.position - transform.position;}
    public Vector2 getAttachedCoord() { return attachedCoordination; }

    //Get the direction of connection
    public Vector2 getAttachToDirection()
    {
        if (myFacing == facing.Left) return new Vector2(-1,0);
        else if (myFacing == facing.Right) return new Vector2(1, 0);
        else if (myFacing == facing.Down) return new Vector2(0, 1);
        else if (myFacing == facing.Up) return new Vector2(0, -1);

        print("error at getAttachToDirection()");
        return new Vector2(0, 0);
    }

    public bool checkShapeMatch(Shape one, Shape two)
    {
        if(one == Shape.TriangleNeg)
        {
            if (two == Shape.TrianglePos) return true;
        }
        else if(one == Shape.TrianglePos)
        {
            if (two == Shape.TriangleNeg) return true;
        }
        else if(one == two) return true;

        return false;
    }

    //Check if the two edges are facing each other so that they can be connected
    public bool checkFacingMatch(facing one, facing two)
    {
        if (one == facing.Up && two == facing.Down) return true;
        if (one == facing.Down && two == facing.Up) return true;
        if (one == facing.Left && two == facing.Right) return true;
        if (one == facing.Right && two == facing.Left) return true;
        return false;
    }
    
}
