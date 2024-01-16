using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AIAutonomousAgent : AIAgent
{
    public AIPerception seekPerception = null;
    public AIPerception fleePerception = null;
    public AIPerception flockPerception = null;

    private void Update()
    {
        //Seek
        if (seekPerception != null) { 
            var gameObjects = seekPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                movement.ApplyForce(Seek(gameObjects[0]));
            }
        }   

        //Flee
        if (fleePerception != null)
        {
            var gameObjects = fleePerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                Vector3 force = Flee(gameObjects[0]);
                movement.ApplyForce(force);
            }
        }

        //Flock
        if (flockPerception != null)
        {
            var gameObjects = flockPerception.GetGameObjects();
            if (gameObjects.Length > 0)
            {
                movement.ApplyForce(Cohesion(gameObjects));
                movement.ApplyForce(Separation(gameObjects, 5));
                movement.ApplyForce(Alignment(gameObjects));
            }
        }

        transform.position = Utilities.Wrap(transform.position, new Vector3(-10, -10, -10), new Vector3(10, 10, 10));
    }

    private Vector3 Seek(GameObject target)
    {
        Vector3 direction = target.transform.position - transform.position;
        //Head to tail "seeking"
        return GetSteringForce(direction);
    }

    private Vector3 Flee(GameObject target)
    {
        Vector3 direction = transform.position - target.transform.position;
        //Head to tail "seeking"
        return GetSteringForce(direction);
    }

    private Vector3 Cohesion(GameObject[] neighbors)
    {
        Vector3 positions = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            positions += neighbor.transform.position;
        }
        Vector3 center = positions / neighbors.Length;
        Vector3 direction = center - transform.position;
        return GetSteringForce(direction);
    }

    private Vector3 Separation(GameObject[] neighbors, float radius)
    {
        Vector3 separation = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            // get direction vector away from neighbor
            Vector3 direction = (transform.position - neighbor.transform.position);
            // check if within separation radius
            if (direction.magnitude < radius)
            {
                // scale separation vector inversely proportional to the direction distance
                // closer the distance the stronger the separation
                separation += direction / direction.sqrMagnitude;
            }
        }

        Vector3 force = GetSteringForce(separation);

        return force;
    }

    private Vector3 Alignment(GameObject[] neighbors)
    {
        Vector3 velocity = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            velocity += neighbor.GetComponent<AIAgent>().movement.velocity;
        }
        Vector3 averageVelocity = velocity / neighbors.Length;
        Vector3 force = GetSteringForce(averageVelocity);
        return force;
    }

    private Vector3 GetSteringForce(Vector3 direction)
    {
        Vector3 desired = direction.normalized * movement.maxSpeed;
        Vector3 steering = desired - movement.velocity;
        Vector3 force = Vector3.ClampMagnitude(steering, movement.maxForce);

        return force;
    }
}
