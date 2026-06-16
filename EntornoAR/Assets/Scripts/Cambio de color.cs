using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cambiodecolor : MonoBehaviour
{
    [SerializeField] private Material Rojo;
    [SerializeField] private Material Azul;
    [SerializeField] private Material Verde;
    [SerializeField] private Material Amarillo;
    
    [SerializeField] private LineRenderer Trazo;

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("CuboRojo"))
        {
            Trazo.material = Rojo;
        }
        else if (other.gameObject.CompareTag("CuboAzul"))
        {
            Trazo.material = Azul;
        }
        else if (other.gameObject.CompareTag("CuboVerde"))
        {
            Trazo.material = Verde;
        }
        else if (other.gameObject.CompareTag("CuboAmarillo"))
        {
            Trazo.material = Amarillo;
        }
    }
}
