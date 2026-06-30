using UnityEngine;
using TMPro;
using UnityEngine.InputSystem; // Necesario para detectar los botones del joystick

public class GestorEmergencias : MonoBehaviour
{
    private enum PasoJuego { Conduciendo, AlertaEmitida, EsperandoFrenoBanquina, AbriendoApp, Finalizado }
    private PasoJuego pasoActual = PasoJuego.Conduciendo;

    [Header("Configuración de Tiempos")]
    public float tiempoParaEmergencia = 20f;
    private float cronometroInicial = 0f;
    private float tiempoTotalSecuencia = 0f;

    [Header("Cálculo de Puntuación")]
    public int puntajeMaximo = 10000;

    [Header("Referencias de Objetos (Físicos)")]
    public TextMeshProUGUI textoPantalla;
    public ControladorAuto scriptAuto;
    public DetectorBanquina scriptBanquina;

    [Header("Referencias de Interfaz (App Seguros)")]
    public GameObject panelAppSeguros;

    private string[] situacionesEmergencia = {
        "¡ALERTA! Neumático pinchado.",
        "¡EMERGENCIA! Falla mecánica en el motor.",
        "¡CUIDADO! Niebla densa adelante.",
        "¡ALERTA! Choque en cadena adelante.",
        "¡EMERGENCIA! Humo en el tablero eléctrico.",
        "¡ALERTA! Objeto peligroso en la calzada."
    };
    private string emergenciaSeleccionada;

    void Update()
    {
        if (pasoActual == PasoJuego.Finalizado) return;

        switch (pasoActual)
        {
            case PasoJuego.Conduciendo:
                cronometroInicial += Time.deltaTime;
                if (cronometroInicial >= tiempoParaEmergencia)
                {
                    DispararEmergencia();
                }
                break;

            case PasoJuego.EsperandoFrenoBanquina:
                tiempoTotalSecuencia += Time.deltaTime;

                bool autoDetenido = Mathf.Abs(scriptAuto.VelocidadActual) <= 0.01f;
                bool enLugarSeguro = scriptBanquina.autoEstaEnBanquina;

                // DIAGNÓSTICO CORRECTO: Se muestra de forma segura usando el nuevo sistema
                if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                {
                    Debug.Log($"[DIAGNÓSTICO] ¿Auto Frenado?: {autoDetenido} (Velocidad: {scriptAuto.VelocidadActual}) | ¿Está en Banquina?: {enLugarSeguro}");
                }

                if (autoDetenido && enLugarSeguro)
                {
                    pasoActual = PasoJuego.AbriendoApp;
                    ActivarAplicacionSimulada();
                }
                else if (autoDetenido && !enLugarSeguro)
                {
                    textoPantalla.text = $"{emergenciaSeleccionada}\n\n⚠️ ¡Peligro! Muévete hacia la BANQUINA derecha para estar a salvo.";
                    textoPantalla.color = Color.yellow;
                }
                else
                {
                    textoPantalla.text = $"{emergenciaSeleccionada}\n\n🛑 Estaciónate por completo en la BANQUINA.";
                    textoPantalla.color = Color.red;
                }
                break;

            case PasoJuego.AbriendoApp:
                tiempoTotalSecuencia += Time.deltaTime;

                // NUEVO: Escucha si el jugador presiona la barra espaciadora del teclado
                // o cualquier botón principal (botón sur/X/A) de un joystick conectado.
                if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame ||
                    Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame)
                {
                    AccionPresionarBotonEmergencia();
                }
                break;
        }
    }

    void DispararEmergencia()
    {
        pasoActual = PasoJuego.EsperandoFrenoBanquina;
        int indiceAleatorio = Random.Range(0, situacionesEmergencia.Length);
        emergenciaSeleccionada = situacionesEmergencia[indiceAleatorio];
    }

    void ActivarAplicacionSimulada()
    {
        textoPantalla.text = "";
        panelAppSeguros.SetActive(true);
    }

    public void AccionPresionarBotonEmergencia()
    {
        if (pasoActual != PasoJuego.AbriendoApp) return;

        pasoActual = PasoJuego.Finalizado;

        int penalizacion = Mathf.RoundToInt(tiempoTotalSecuencia * 250);
        int puntajeFinal = puntajeMaximo - penalizacion;
        if (puntajeFinal < 0) puntajeFinal = 0;

        textoPantalla.text = $"¡ASISTENCIA EN CAMINO!\n\n" +
                             $"Tiempo total de reacción: {tiempoTotalSecuencia.ToString("F2")}s\n" +
                             $"PUNTAJE CORPORATIVO: {puntajeFinal} pts";
        textoPantalla.color = Color.green;

        panelAppSeguros.SetActive(false);
    }
}
