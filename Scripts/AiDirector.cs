//	Created by: Sunny Valley Studio 
//	https://svstudio.itch.io

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SVS
{

    public class AiDirector : MonoBehaviour
    {
        public PlacementManager placementManager;
        //public RoadManager roadManager;
        public GameObject[] pedestrianPrefabs;

        public GameObject carPrefab;

        AdjacencyGraph graph = new AdjacencyGraph();
        List<Vector3> pathForPedestrian = new List<Vector3>();

        private void TrySpawningAnAgent(StructureModel startStructure, StructureModel endStructure)
        {
            
            if (startStructure != null)
            {
                var roadStartPosition = ((INeedingRoad)startStructure).RoadPosition;
                var spawnPoint = ((RoadStructure)placementManager.GetStructureAt(roadStartPosition)).GetPedestrianSpawnMarker(startStructure.transform.position).Position;
                var agent = Instantiate(GetRandomPedestrian(), spawnPoint, Quaternion.identity);
                agent.transform.position = spawnPoint;
                if (endStructure != null)
                {
                    var roadEndPosition = ((INeedingRoad)endStructure).RoadPosition;
                    var endPoint = ((RoadStructure)placementManager.GetStructureAt(roadEndPosition)).GetPedestrianSpawnMarker(endStructure.transform.position).Position;
                    var path = placementManager.GetPathBetween(roadStartPosition, roadEndPosition, true);
                    if (path.Count > 2)
                    {
                        path.Reverse();
                        List<Vector3> pedestrianPath = GetPedestrianPathFromRoadPath(path, spawnPoint, endPoint);
                        var aiAgent = agent.GetComponent<AIAgent>();
                        aiAgent.Initailize(pedestrianPath);
                        return;
                    }
                }
                Destroy(agent);
                Debug.LogError("Agent has no destination or it cannot reach it!!!");
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
            if(startStructure != null && endStructure != null)
            {
                var startRoadPosition = ((INeedingRoad)startStructure).RoadPosition;
                var endRoadPosition = ((INeedingRoad)endStructure).RoadPosition;

                var path = placementManager.GetPathBetween(startRoadPosition, endRoadPosition);
                path.Reverse();

                var car = Instantiate(carPrefab, startRoadPosition, Quaternion.identity);
                car.GetComponent<CarAI>().SetPath(path.ConvertAll(x => (Vector3)x));
            }
        }

        public void SpawnAllAgents()
        {
            foreach (var house in placementManager.GetAllHouses())
            {
                TrySpawningAnAgent(house, placementManager.GetRandomSpecialStrucutre());
            }
            foreach (var house in placementManager.GetAllSpecialStructures())
            {
                TrySpawningAnAgent(house, placementManager.GetRandomHouseStructure());
            }
        }

        private GameObject GetRandomPedestrian()
        {
            return pedestrianPrefabs[UnityEngine.Random.Range(0, pedestrianPrefabs.Length)];
        }

        public List<Vector3> GetPedestrianPathFromRoadPath(List<Vector3Int> path, Vector3 startStructurePosition, Vector3 endStructurePosition)
        {
            graph.ClearGraph();
            CreateAGraph(path);
            pathForPedestrian = AdjacencyGraph.AStarSearch(graph, startStructurePosition, endStructurePosition);
            return pathForPedestrian;
        }

        private void CreateAGraph(List<Vector3Int> path)
        {
            Dictionary<Marker, Vector3> tempDictionary = new Dictionary<Marker, Vector3>();
            for (int i = 0; i < path.Count; i++)
            {
                var currenPosition = path[i];
                var roadStructure = ((RoadStructure)placementManager.GetStructureAt(currenPosition));
                var markersList = roadStructure.GetPedestrianMarkers();
                bool limitDistance = markersList.Count == 4 ? true : false;
                tempDictionary.Clear();
                foreach (Marker marker in markersList)
                {
                    graph.AddVertex(marker.Position);
                    foreach (var markerNeighbourPosition in marker.GetAdjacentMarkers())
                    {
                        graph.AddEdge(marker.Position, markerNeighbourPosition);
                    }
                    if (marker.OpenForConnections && i + 1 < path.Count)
                    {
                        var nextRoadStructure = ((RoadStructure)placementManager.GetStructureAt(path[i + 1]));

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
                if (limitDistance && tempDictionary.Count == 4)
                {
                    var distanceSortedMarkers = tempDictionary.OrderBy(x => Vector3.Distance(x.Key.Position, x.Value)).Select(x => x).ToList();
                    for (int j = 0; j < 2; j++)
                    {
                        graph.AddEdge(distanceSortedMarkers[j].Key.Position, distanceSortedMarkers[j].Value);
                    }
                }
            }
        }

        private void DegubGraph()
        {
            foreach (var vertex in graph.GetVertices())
            {
                foreach (var vertexNeighbour in graph.GetConnectedVerticesTo(vertex.Position))
                {
                    Debug.DrawLine(vertex.Position + Vector3.up, vertexNeighbour.Position + Vector3.up, Color.red);
                }
            }
        }

        private void Update()
        {
            DegubGraph();

            if(Input.GetKeyDown(KeyCode.P)){
                var startStructure = placementManager.GetRandomHouseStructure();
                var endStructure = placementManager.GetRandomSpecialStrucutre();
                var roadStartPosition = ((INeedingRoad)startStructure).RoadPosition;
                var spawnPoint = ((RoadStructure)placementManager.GetStructureAt(roadStartPosition)).GetPedestrianSpawnMarker(startStructure.transform.position).Position;
                var agent = Instantiate(GetRandomPedestrian(), spawnPoint, Quaternion.identity);
                agent.transform.position = spawnPoint;
                if (endStructure != null)
                {
                    var roadEndPosition = ((INeedingRoad)endStructure).RoadPosition;
                    var path = placementManager.GetPathBetween(roadStartPosition, roadEndPosition, true);
                    if (path.Count > 0)
                    {
                        path.Reverse();
                        var aiAgent = agent.GetComponent<AIAgent>();
                        aiAgent.Initailize(new List<Vector3>(path.Select(x => (Vector3)x).ToList()));
                        return;
                    }
                }
            }
        }
    }
}