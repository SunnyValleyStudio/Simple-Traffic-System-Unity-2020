using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartRoad : MonoBehaviour
{
    Queue<CarAI> trafficQueue = new Queue<CarAI>();
    public CarAI currentCar;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            var car = other.GetComponent<CarAI>();
            if(car != null && car != currentCar && car.IsThisLastPathIndex() == false)
            {
                trafficQueue.Enqueue(car);
                car.Stop = true;
            }
        }
    }

    private void Update()
    {
        if(currentCar == null)
        {
            if(trafficQueue.Count > 0)
            {
                currentCar = trafficQueue.Dequeue();
                currentCar.Stop = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Car"))
        {
            var car = other.GetComponent<CarAI>();
            if(car != null)
            {
                RemoveCar(car);
            }
        }
    }

    private void RemoveCar(CarAI car)
    {
        if(car == currentCar)
        {
            currentCar = null;
        }
    }
}
