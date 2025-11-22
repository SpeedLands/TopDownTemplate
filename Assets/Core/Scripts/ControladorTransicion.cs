using System.Collections;
using UnityEngine;

public class ControladorTransicion : MonoBehaviour
{
    [Header("Referencias")]
    public Animator animatorFondo; // Arrastra aquí tu objeto del Fondo
    public Animator animatorFade;  // Arrastra aquí tu PanelFade

    [Header("Tiempos")]
    public float tiempoOscuro = 1f; // Cuánto tarda en ponerse negro

    // Llama a esta función desde tus botones (On Click)
    // Ejemplo: CambiarFondo("MENU_VISTA")
    public void CambiarFondo(string nombreTriggerFondo)
    {
        StartCoroutine(SecuenciaTransicion(nombreTriggerFondo));
    }

    IEnumerator SecuenciaTransicion(string nuevoEstado)
    {
        // 1. Decirle al panel negro que se oscurezca (Fade Out)
        // Asegurate de tener un Trigger llamado "Oscurecer" en tu FadeController
        animatorFade.SetTrigger("Oscurecer"); 

        // 2. Esperar a que la pantalla esté totalmente negra
        yield return new WaitForSeconds(tiempoOscuro);

        // 3. AHORA cambiamos el fondo (Nadie lo ve porque está negro)
        // Aquí activas el Trigger o Bool de tu Animator de Fondo
        // Nota: Asegúrate de usar el nombre correcto de tus parámetros del fondo
        animatorFondo.Play(nuevoEstado); 
        // O si usas triggers: animatorFondo.SetTrigger(nuevoEstado);

        // 4. Esperar un mini momento para asegurar que cargó (opcional)
        yield return new WaitForSeconds(0.2f);

        // 5. Decirle al panel negro que se aclare (Fade In)
        animatorFade.SetTrigger("Aclarar");
    }
}