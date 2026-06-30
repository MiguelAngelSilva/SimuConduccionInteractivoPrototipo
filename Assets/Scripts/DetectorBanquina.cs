using UnityEngine;

public class DetectorBanquina : MonoBehaviour
{
    // Variable pública que cambiará de estado según la posición del coche
    public bool autoEstaEnBanquina = false;

    // Se ejecuta automáticamente cuando el auto entra en la zona invisible
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.name == "Auto")
        {
            autoEstaEnBanquina = true;
        }
    }

    // Se ejecuta si el auto vuelve a salir a la pista central
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other.name == "Auto")
        {
            autoEstaEnBanquina = false;
        }
    }
}
