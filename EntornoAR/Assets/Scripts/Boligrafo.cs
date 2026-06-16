using System.Collections.ObjectModel;
using System.Globalization;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static Unity.Burst.Intrinsics.X86;

public class Boligrafo : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    [SerializeField] private InputActionReference Pintar;
    [SerializeField] private InputActionReference CambiarColor;
    [SerializeField] private InputActionReference LimpiarLienzo;
    [SerializeField] private InputActionReference EnviarPuntos;

    // 1. NUEVO: Referencia al LineRenderer para dibujar la l nea
    [SerializeField] private LineRenderer LineaTrayectoria;
    private LineRenderer lineaActual;

    [SerializeField] private GameObject PuntaPincel;
    [SerializeField] private GameObject Punto;
    [SerializeField] private Comunicacion com;

    public int escala = 10;

    private ObservableCollection<GameObject> Puntos = new ObservableCollection<GameObject>();

    private System.Diagnostics.Stopwatch Delay = new System.Diagnostics.Stopwatch();
    void Start()
    {
        Pintar.action.started += Crear_Punto;
        Pintar.action.canceled += Crear_Punto;
        Pintar.action.Enable();

        EnviarPuntos.action.started += Mandar_Puntos;
        EnviarPuntos.action.Enable();

        LimpiarLienzo.action.started += Limpiar_Lienzo;
        LimpiarLienzo.action.Enable();

        //CambiarColor.action.started += Mandar_Puntos;
        //CambiarColor.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Pintar.action.ReadValue<bool>());
        if (Delay.Elapsed.TotalSeconds > 0.5)
        {
            //Crear_Punto();
            Delay.Restart();
        }
    }

    private void OnDisable()
    {
        Pintar.action.started -= Crear_Punto;
        Pintar.action.canceled -= Crear_Punto;
        Pintar.action.Disable();

        EnviarPuntos.action.started -= Mandar_Puntos;
        EnviarPuntos.action.Disable();

        LimpiarLienzo.action.started -= Limpiar_Lienzo;
        LimpiarLienzo.action.Disable();

        //CambiarColor.action.started -= Mandar_Puntos;
        //CambiarColor.action.Disable();
    }

    private void ActivarTimer(InputAction.CallbackContext context)
    {
        Delay.Start();

    }

    private void DesactivarTimer(InputAction.CallbackContext context)
    {
        print("Fin");
        Delay.Restart();
        Delay.Stop();
        

    }

    private void Crear_Punto(InputAction.CallbackContext context)
    {
        Vector3 Posicion = PuntaPincel.transform.position;
        GameObject newObj = GameObject.Instantiate(Punto);
        newObj.SetActive(true);
        newObj.transform.position = Posicion;

        newObj.transform.SetParent(GameObject.Find("Lienzo").transform, true);

        Vector3 posicionLocal = newObj.transform.localPosition;
        Vector3 posicionFinal = new Vector3(0f, 0f, 0f);
        if (posicionLocal.z > 0 &&  posicionLocal.z < 0.15)
        {
            posicionFinal.z = 0.05f;
            if(posicionLocal.x < -0.5 || posicionLocal.x > 0.5)
            {
                print("Fuera de Rango");
                GameObject.Destroy(newObj);
                return;
            }
            if (posicionLocal.y < -0.5 || posicionLocal.y > 0.5)
            {
                GameObject.Destroy(newObj);
                return;
            }

            posicionFinal.x = posicionLocal.x;
            posicionFinal.y = posicionLocal.y;

            newObj.transform.localPosition = posicionFinal;

            Puntos.Add(newObj);
            Debug.Log(Punto.transform.localPosition[1]);
            Dibujar(newObj.transform.position);
        }
        else
        {
            GameObject.Destroy(newObj);
        }
    }

    private void Dibujar(Vector3 posicionPunto)
    {
        if (LineaTrayectoria == null) return;

        // Si es un trazo nuevo, creamos un objeto nuevo
        if (lineaActual == null)
        {
            // "Instantiate" crea una copia permanente en la escena que no se borrar  sola
            GameObject nuevaLineaObj = Instantiate(LineaTrayectoria.gameObject, Vector3.zero, Quaternion.identity);

            // Lo organizamos dentro de la carpeta "Lienzo" si existe
            GameObject padreDibujo = GameObject.Find("Lienzo");
            if (padreDibujo != null) nuevaLineaObj.transform.SetParent(padreDibujo.transform);

            lineaActual = nuevaLineaObj.GetComponent<LineRenderer>();
            lineaActual.positionCount = 0; // Empezamos de cero v rtices
        }

        // A adimos el punto a la l nea actual
        lineaActual.positionCount++;
        lineaActual.SetPosition(lineaActual.positionCount - 1, posicionPunto);
    }
    public void Mandar_Puntos(InputAction.CallbackContext context)
    {
        string mensaje = "Dibujar";

        foreach (GameObject punto in Puntos)
        {
            mensaje += "/";
            mensaje += (punto.transform.localPosition[0] / -escala).ToString(CultureInfo.InvariantCulture) + " ";
            mensaje += (punto.transform.localPosition[1] / escala).ToString(CultureInfo.InvariantCulture) + " ";
            mensaje += (punto.transform.localPosition[2] * (-0.1)).ToString(CultureInfo.InvariantCulture);
   
        }

        com.SEND_RS(mensaje);
        Debug.Log(mensaje);
    }

    public void Limpiar_Lienzo(InputAction.CallbackContext context)
    {
        Debug.Log("Borrar");

        for (int i = Puntos.Count - 1; i >= 0; i--)
        {
            GameObject punto = Puntos[i];
            Puntos.Remove(punto);
            GameObject.Destroy(punto);
        }

       
        Destroy(lineaActual.gameObject);
           

    }
}
