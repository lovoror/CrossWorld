using UnityEngine;
using Pathfinding;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject.AIPath
{
    // Abstract class for any task that uses an AIPathAgent
    public abstract class AIPathMovementAgent : Movement
    {
        [Tooltip("The speed of the agent")]
        public SharedFloat speed = 3;
        [Tooltip("Turn speed of the agent")]
        public SharedFloat turningSpeed = 5;

        // A cache of the AIPath
        private AIPathAgent aiPathAgent;

        public override void OnAwake()
        {
            // cache for quick lookup
            aiPathAgent = gameObject.GetComponent<AIPathAgent>();

            if (aiPathAgent.target == null) {
                var target = new GameObject();
                target.name = Owner.name + " target";
                aiPathAgent.target = target.transform;
                aiPathAgent.canMove = false;
            }
        }

        public override void OnStart()
        {
            // set the speed and turning speed, then enable the agent
            aiPathAgent.speed = speed.Value;
            aiPathAgent.turningSpeed = turningSpeed.Value;
            aiPathAgent.RemovePath();
            aiPathAgent.SearchPath();
        }

        protected override bool SetDestination(Vector3 target)
        {
            if (aiPathAgent.target.position != target) {
                aiPathAgent.target.position = target;
            }
			aiPathAgent.canMove = true;  // zpf modify
            return true;
        }

        protected override Vector3 Velocity()
        {
            return aiPathAgent.Velocity();
        }

        protected override void UpdateRotation(bool update)
        {
            // Intentionally left blank
        }

        protected override bool HasPath()
        {
            return aiPathAgent.PathCalculated();
        }

        protected override void Stop()
        {
            aiPathAgent.RemovePath();
        }

        protected override bool HasArrived()
        {
            var arrived = HasPath() && aiPathAgent.TargetReached;
            if (arrived) {
                aiPathAgent.RemovePath();
            }
            return arrived;
        }

        public override void OnEnd()
        {
            // Disable the AIPath
            aiPathAgent.RemovePath();
            aiPathAgent.canMove = false;
        }

        // Reset the public variables
        public override void OnReset()
        {
            speed = 3;
            turningSpeed = 5;
        }
    }
}
