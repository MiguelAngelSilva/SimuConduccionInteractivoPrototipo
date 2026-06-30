using UnityEngine;
using UnityEngine.InputSystem; // Importante: habilita el nuevo sistema

public class ControladorAuto : MonoBehaviour
{
    public float velocidadMovimiento = 20f;
    public float velocidadGiro = 45f;

    // Variables internas para guardar el movimiento
    private Vector2 entradaMovimiento;

    // Nueva propiedad pública para que el Gestor de Emergencias pueda leer la velocidad
    public float VelocidadActual { get; private set; }

    // Esta función lee el teclado (flechas/WASD) y Joysticks automáticamente
    public void OnMove(InputValue valor)
    {
        entradaMovimiento = valor.Get<Vector2>();
    }

    void Update()
    {
        // Movimiento hacia adelante/atrás (Eje Y del stick o flechas arriba/abajo)
        float movimientoAdelante = entradaMovimiento.y * velocidadMovimiento * Time.deltaTime;

        // Giro hacia los lados (Eje X del stick o flechas izquierda/derecha)
        float movimientoGiro = entradaMovimiento.x * velocidadGiro * Time.deltaTime;

        // Guardamos la velocidad calculada para este cuadro
        VelocidadActual = movimientoAdelante;

        // Aplica el movimiento al auto
        transform.Translate(0, 0, movimientoAdelante);
        transform.Rotate(0, movimientoGiro, 0);
    }
}
