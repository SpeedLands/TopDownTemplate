using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    [Tooltip("El Prefab de la imagen del corazón que se usará para crear la barra de vida.")]
    public Image heartPrefab;

    [Header("Sprites de los Estados del Corazón")]
    public Sprite fullHeartSprite;
    public Sprite threeQuarterHeartSprite;
    public Sprite halfHeartSprite;
    public Sprite oneQuarterHeartSprite;
    public Sprite emptyHeartSprite;
    private List<Image> hearts = new List<Image>();

    public void SetMaxHearts(int maxHearts)
    {
        // Limpia los corazones antiguos antes de crear nuevos
        foreach (Image heart in hearts)
        {
            Destroy(heart.gameObject);
        }
        hearts.Clear();

        // Crea un nuevo objeto de corazón por cada corazón máximo
        for (int i = 0; i < maxHearts; i++)
        {
            Image newHeart = Instantiate(heartPrefab, transform);
            hearts.Add(newHeart);
        }
    }

    public void UpdateHearts(int currentHealthInQuarters)
    {
        // Recorre cada contenedor de corazón en la UI
        for (int i = 0; i < hearts.Count; i++)
        {
            // Cada corazón completo representa 4 unidades de vida (cuartos).
            // Calculamos cuántos "cuartos" de vida le corresponden a este corazón específico.
            // Por ejemplo, para el primer corazón (i=0), el valor es la vida actual.
            // Para el segundo (i=1), es la vida que queda después de llenar el primero, y así sucesivamente.
            int heartQuarterValue = currentHealthInQuarters - (i * 4);

            // Aseguramos que el valor no sea negativo.
            heartQuarterValue = Mathf.Clamp(heartQuarterValue, 0, 4);
            
            // Asignamos el sprite y el color según el valor calculado
            switch (heartQuarterValue)
            {
                case 4:
                    hearts[i].sprite = fullHeartSprite;
                    break;
                case 3:
                    hearts[i].sprite = threeQuarterHeartSprite;
                    break;
                case 2:
                    hearts[i].sprite = halfHeartSprite;
                    break;
                case 1:
                    hearts[i].sprite = oneQuarterHeartSprite;
                    break;
                case 0:
                    hearts[i].sprite = emptyHeartSprite;
                    break;
            }
        }
    }
}