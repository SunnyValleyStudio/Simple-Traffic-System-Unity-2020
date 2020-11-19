using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleCity.AI
{
    public class RoadHelper : MonoBehaviour
    {
        [SerializeField]
        protected List<Marker> pedestrianMarkers;
        [SerializeField]
        protected List<Marker> carMarkers;
        [SerializeField]
        protected bool isCorner;
        [SerializeField]
        protected bool hasCrosswalks;

        float approximateThresholdCorner = 0.3f;

        public virtual Marker GetpositioForPedestrianToSpwan(Vector3 structurePosition)
        {
            return GetClosestMarkeTo(structurePosition, pedestrianMarkers);
        }

        private Marker GetClosestMarkeTo(Vector3 structurePosition, List<Marker> pedestrianMarkers, bool isCorner = false)
        {
            if (isCorner)
            {
                foreach (var marker in pedestrianMarkers)
                {
                    var direction = marker.Position - structurePosition;
                    direction.Normalize();
                    if(Mathf.Abs(direction.x) < approximateThresholdCorner || Mathf.Abs(direction.z) < approximateThresholdCorner)
                    {
                        return marker;
                    }
                }
                return null;
            }
            else
            {
                Marker closestMarker = null;
                float distance = float.MaxValue;
                foreach (var marker in pedestrianMarkers)
                {
                    var markerDistance = Vector3.Distance(structurePosition, marker.Position);
                    if(distance > markerDistance)
                    {
                        distance = markerDistance;
                        closestMarker = marker;
                    }
                }
                return closestMarker;
            }
        }

        public Vector3 GetClosestPedestrainPosition(Vector3 currentPosition)
        {
            return GetClosestMarkeTo(currentPosition, pedestrianMarkers, isCorner).Position;
        }

        public Vector3 GetClosestCarMarkerPosition(Vector3 currentPosition)
        {
            return GetClosestMarkeTo(currentPosition, carMarkers, isCorner).Position;
        }


        public List<Marker> GetAllPedestrianMarkers()
        {
            return pedestrianMarkers;
        }

        public List<Marker> GetAllCarMarkers()
        {
            return carMarkers;
        }
    }
}

