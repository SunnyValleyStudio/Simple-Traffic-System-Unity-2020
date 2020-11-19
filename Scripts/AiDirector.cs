using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleCity.AI
{
    public class AiDirector : MonoBehaviour
    {
        public PlacementManager placementManager;
        public GameObject[] pedestrianPrefabs;

        public GameObject carPrefab;

        AdjacencyGraph graph = new AdjacencyGraph();

        public void SpawnAllAagents()
        {
            foreach (var house in placementManager.GetAllHouses())
            {
                TrySpawningAnAgent(house, placementManager.GetRandomSpecialStrucutre());
            }
            foreach (var specialStructure in placementManager.GetAllSpecialStructures())
            {
                TrySpawningAnAgent(specialStructure, placementManager.GetRandomHouseStructure());
            }
        }

        private void TrySpawningAnAgent(StructureModel startStructure, StructureModel endStructure)
        {
            if(startStructure != null && endStructure != null)
            {
                var startPosition = ((INeedingRoad)startStructure).RoadPosition;
                var endPosition = ((INeedingRoad)endStructure).RoadPosition;

                var startMarkerPosition = placementManager.GetStructureAt(startPosition).GetPedestrianSpawnMarker(startStructure.transform.position);
                var endMarkerPosition = placementManager.GetStructureAt(endPosition).GetNearestMarkerTo(endStructure.transform.position);

                var agent = Instantiate(GetRandomPedestrian(), startMarkerPosition.Position, Quaternion.identity);
                var path = placementManager.GetPathBetween(startPosition, endPosition, true);
                if(path.Count > 0)
                {
                    path.Reverse();
                    List<Vector3> agentPath = GetPedestrianPath(path, startMarkerPosition.Position, endMarkerPosition);
                    var aiAgent = agent.GetComponent<AiAgent>();
                    aiAgent.Initialize(agentPath);
                }
            }
        }

        public void SpawnACar()
        {
            foreach (var house in placementManager.GetAllHouses())
            {
                TrySpawninACar(house, placementManager.GetRandomSpecialStrucutre());
            }
        }

        private void TrySpawninACar(StructureModel startStructure, StructureModel endStructure)
        {
            if (startStructure != null && endStructure != null)
            {
                var startRoadPosition = ((INeedingRoad)startStructure).RoadPosition;
                var endRoadPosition = ((INeedingRoad)endStructure).RoadPosition;

                var path = placementManager.GetPathBetween(startRoadPosition, endRoadPosition);
                path.Reverse();

                var car = Instantiate(carPrefab, startRoadPosition, Quaternion.identity);
                car.GetComponent<CarAI>().SetPath(path.ConvertAll(x => (Vector3)x));
            }
        }

        private List<Vector3> GetPedestrianPath(List<Vector3Int> path, Vector3 startPosition, Vector3 endPosition)
        {
            graph.ClearGraph();
            CreatAGraph(path);
            Debug.Log(graph);
            return AdjacencyGraph.AStarSearch(graph,startPosition,endPosition);
        }

        private void CreatAGraph(List<Vector3Int> path)
        {
            Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();

            for (int i = 0; i < path.Count; i++)
            {
                var currentPosition = path[i];
                var roadStructure = placementManager.GetStructureAt(currentPosition);
                var markersList = roadStructure.GetPedestrianMarkers();
                bool limitDistance = markersList.Count == 4;
                tempDictionary.Clear();
                foreach (var marker in markersList)
                {
                    graph.AddVertex(marker.Position);
                    foreach (var markerNeighbourPosition in marker.GetAdjacentPositions())
                    {
                        graph.AddEdge(marker.Position, markerNeighbourPosition);
                    }

                    if(marker.OpenForconnections && i+1 < path.Count)
                    {
                        var nextRoadStructure = placementManager.GetStructureAt(path[i + 1]);
                        if (limitDistance)
                        {
                            tempDictionary.Add(marker, nextRoadStructure.GetNearestMarkerTo(marker.Position));
                        }
                        else
                        {
                            graph.AddEdge(marker.Position, nextRoadStructure.GetNearestMarkerTo(marker.Position));
                        }
                    }
                }
                if(limitDistance && tempDictionary.Count == 4)
                {
                    var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).ToList();
                    for (int j = 0; j < 2; j++)
                    {
                        graph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                    }
                }
            }
        }

        private GameObject GetRandomPedestrian()
        {
            return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
        }

        private void Update()
        {
            foreach (var vertex in graph.GetVertices())
            {
                foreach (var vertexNeighbour in graph.GetConnectedVerticesTo(vertex))
                {
                    Debug.DrawLine(vertex.Position + Vector3.up, vertexNeighbour.Position + Vector3.up, Color.red);
                }
            }
        }

        public void NewMethod(int parameter)
        {
            Debug.Log("hello");
        }

        public void NewMethdTow(string name)
        {
            Debug.Log(name);
        }
    }
}

