﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public enum DIR
    {
        LEFT,RIGHT,FORWARD,BACK
    }

    public class CubeRotation
    {
        // arrays with the rotations and position in each legal direction
        static Quaternion[] rollRotationTableLeft;
        static Quaternion[] rollRotationTableRight;
        static Quaternion[] rollRotationTableForward;
        static Quaternion[] rollRotationTableBack;
        static Vector3[] rollPositionTableLeft;
        static Vector3[] rollPositionTableRight;
        static Vector3[] rollPositionTableForward;
        static Vector3[] rollPositionTableBack;
        int index = 0;
        Vector3 startPosition;
        Transform transform;

        static bool initiated = false;
        int steps;

        float tickTime;

        DIR direction;

        float accumulatedTime = 0f;

        public bool rolling {get; private set;}

        public CubeRotation(Transform transform, Vector3 direction, float rotationSpeed)
        {
            this.rolling = false;
            this.steps = 100;
            this.startPosition = transform.position;
            this.transform = transform;
            this.SetDirection(direction);
            this.SetRotationSpeed(rotationSpeed);

            if(!initiated)
            {
                this.CalculateRollRotations(out rollRotationTableLeft, out rollPositionTableLeft, this.steps, Vector3.left);
                this.CalculateRollRotations(out rollRotationTableRight, out rollPositionTableRight, this.steps, Vector3.right);
                this.CalculateRollRotations(out rollRotationTableForward, out rollPositionTableForward, this.steps, Vector3.forward);
                this.CalculateRollRotations(out rollRotationTableBack, out rollPositionTableBack, this.steps, Vector3.back);
                initiated = true;
            }
        }

        public void StartRolling()
        {
            this.rolling = true;
        }

        public void StopRolling()
        {
            this.rolling = false;
        }

        public bool SetRotationSpeed(float rotationSpeed)
        {
            if(this.rolling)
                return false;

            this.tickTime = (90f/rotationSpeed) * (1f/this.steps);

            return true;
        }

        public bool SetDirection(DIR direction)
        {
            if(this.rolling)
                return false;

            this.direction = direction;

            return true;
        }

        public bool SetDirection(Vector3 direction)
        {
            if(direction == Vector3.left)
            {
                return this.SetDirection(DIR.LEFT);
            }
            else if(direction == Vector3.right)
            {
                return this.SetDirection(DIR.RIGHT);
            }
            else if(direction == Vector3.forward)
            {
                return this.SetDirection(DIR.FORWARD);
            }
            else if(direction == Vector3.back)
            {
                return this.SetDirection(DIR.BACK);
            }
            else
            {
                Debug.LogError($"Direction {direction} is not legal. It has to be {Vector3.left},{Vector3.right},{Vector3.forward} or {Vector3.back}!");
                return false;
            }
        }

        public void Roll(float deltaTime)
        {
            // if not rolling, dont roll
            if(!this.rolling)
                return;
            
            // count up
            this.accumulatedTime += deltaTime;
            if(this.accumulatedTime <= this.tickTime)
            {
                return;
            }
            // reset time
            this.accumulatedTime = 0f;

            Quaternion q = Quaternion.identity;
            Vector3 v = Vector3.zero;

            switch(this.direction)
            {
                case DIR.LEFT:
                    v = rollPositionTableLeft[this.index];
                    q = rollRotationTableLeft[this.index];
                    break;
                case DIR.RIGHT:
                    v = rollPositionTableRight[this.index];
                    q = rollRotationTableRight[this.index];
                    break;
                case DIR.FORWARD:
                    v = rollPositionTableForward[this.index];
                    q = rollRotationTableForward[this.index];
                    break;
                case DIR.BACK:
                    v = rollPositionTableBack[this.index];
                    q = rollRotationTableBack[this.index];
                    break;
            }

            this.transform.position = v + this.startPosition;
            this.transform.rotation = q;    // rotation gets resettet on the first rotation step

            this.index++;
            if(this.index >= rollRotationTableLeft.Length)
            {
                this.startPosition = this.transform.position;
                this.index = 0;
                this.StopRolling();
            }
        }

        void CalculateRollRotations(out Quaternion[] rotations, out Vector3[] positions, int steps, Vector3 direction)
        {
            // assign out parameter
            rotations = new Quaternion[steps];
            positions = new Vector3[steps];

            if(direction != Vector3.left && direction != Vector3.right && direction != Vector3.forward && direction != Vector3.back)
            {
                Debug.LogError($"Direction {direction} is not legal. It has to be {Vector3.left},{Vector3.right},{Vector3.forward} or {Vector3.back}!");
                return;
            }
            // create dummy objects
            GameObject dummy = new GameObject();
            GameObject corner = new GameObject();
            corner.transform.parent = dummy.transform;

            // calculate step size
            float angleStep = 90f / (float)steps;

            // set corner and axis
            corner.transform.position = dummy.transform.position + direction*0.5f + Vector3.down*0.5f; // a corner to rotate around
            Vector3 axis = Vector3.Cross(direction, Vector3.down); // the axes is the crossproduct of the y and the direction axes

            // iterate through the steps and calculate rotation and position
            for(int i=0;i<steps-1;i++)
            {
                // rotate dummy
                dummy.transform.RotateAround(corner.transform.position, axis, angleStep);

                // save rotation and position after rotation step
                rotations[i] = dummy.transform.rotation;
                positions[i] = dummy.transform.position;
            }

            // round away the floating-point errors
            dummy.transform.position = new Vector3(
                Mathf.Round(dummy.transform.position.x),
                Mathf.Round(dummy.transform.position.y),
                Mathf.Round(dummy.transform.position.z));

            dummy.transform.rotation = Utils.MathHelpers.SnapRotation(dummy.transform.rotation);

            // the last step is snapped into position and rotation to avoid floating point errors
            rotations[steps-1] = dummy.transform.rotation;
            positions[steps-1] = dummy.transform.position;

            GameObject.Destroy(dummy);
            GameObject.Destroy(corner);
        }
    }
}