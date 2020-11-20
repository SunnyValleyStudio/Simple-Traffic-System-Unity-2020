using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleCity.AI
{
    public class RoadHelperStraight : RoadHelper
    {
        [SerializeField]
        private Marker leftLaneMarker90, rightLaneMarker90;

        public override Marker GetPositioForCarToSpawn(Vector3 nextPathPosition)
        {
            int angle = (int)transform.rotation.eulerAngles.y;
            var direction = nextPathPosition - transform.position;
            return GetCorrectMarker(angle, direction);
        }


        public override Marker GetPositioForCarToEnd(Vector3 previousPathPosition)
        {
            int angle = (int)transform.rotation.eulerAngles.y;
            var direction = transform.position - previousPathPosition;
            return GetCorrectMarker(angle, direction);
        }


        private Marker GetCorrectMarker(int angle, Vector3 directionVector)
        {
            var direction = GetDirection(directionVector);
            if (angle == 0)
            {
                if (direction == Directon.left)
                {
                    return rightLaneMarker90;
                }
                else
                {
                    return leftLaneMarker90;
                }
            } else if (angle == 90)
            {
                if (direction == Directon.up)
                {
                    return rightLaneMarker90;
                }
                else
                {
                    return leftLaneMarker90;
                }
            }else if(angle == 270)
            {
                if (direction == Directon.left)
                {
                    return leftLaneMarker90;
                }
                else
                {
                    return rightLaneMarker90;
                }
            }
            else
            {
                if (direction == Directon.up)
                {
                    return leftLaneMarker90;
                }
                else
                {
                    return rightLaneMarker90;
                }
            }
        }

        public enum Directon
        {
            up,
            down,
            left,
            right
        }

        public Directon GetDirection(Vector3 direction)
        {
            if(Mathf.Abs(direction.z) > .5f)
            {
                if(direction.z > 0.5f)
                {
                    return Directon.up;
                }
                else
                {
                    return Directon.down;
                }
            }
            else
            {
                if(direction.x > 0.5f)
                {
                    return Directon.right;
                }
                else
                {
                    return Directon.left;
                }
            }
        }
    }
}

