using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Intersections
{
    class Intersection : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Car")
                other.GetComponent<CarEngine>().Intersection = true;
        }
        private void OnTriggerStay(Collider other)
        {
            if (other.gameObject.tag == "Car")
                other.GetComponent<CarEngine>().Intersection = true;
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.tag == "Car")
                other.GetComponent<CarEngine>().Intersection = false;
        }
    }
}
