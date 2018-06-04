using UnityEngine;
using Pathfinding;
using System;

namespace BehaviorDesigner.Runtime.Tasks.Movement.AstarPathfindingProject.AIPath
{
    // Abstract class for any task that uses a group of AIPathAgents
    public abstract class AIPathGroupMovement : GroupMovement
    {
        [Tooltip("All of the agents")]
        public SharedGameObject[] agents = null;
        [Tooltip("The speed of the agents")]
        public SharedFloat speed = 3;
        [Tooltip("Turn speed of the agents")]
        public SharedFloat turningSpeed = 5;

        protected AIPathAgent[] aiPathAgents;
        protected Transform[] transforms;

        public override void OnStart()
        {
            aiPathAgents = new AIPathAgent[agents.Length];
            transforms = new Transform[agents.Length];

            // Set the speed and turning speed of all of the agents
            for (int i = 0; i < agents.Length; ++i) {
                aiPathAgents[i] = agents[i].Value.GetComponent<AIPathAgent>();
                transforms[i] = agents[i].Value.transform;

                if (aiPathAgents[i].target == null) {
                    var go = new GameObject();
                    aiPathAgents[i].target = go.transform;
                    aiPathAgents[i].target.name = agents[i].Value.name + " Target";
                }

                aiPathAgents[i].speed = speed.Value;
                aiPathAgents[i].turningSpeed = turningSpeed.Value;
                aiPathAgents[i].enabled = true;
            }
        }

        protected override bool SetDestination(int index, Vector3 target)
        {
            aiPathAgents[index].target.position = target;
            return true;
        }

        protected override Vector3 Velocity(int index)
        {
            return aiPathAgents[index].Velocity();
        }

        public override void OnEnd()
        {
            // Disable all of the agents when the task is done
            for (int i = 0; i < agents.Length; ++i) {
                aiPathAgents[i].enabled = false;
            }
        }

        // Reset the public variables
        public override void OnReset()
        {
            agents = null;
            speed = 3;
            turningSpeed = 5;
        }
    }
}
