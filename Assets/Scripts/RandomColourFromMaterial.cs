using UnityEngine;

public class RandomColourFromMaterial : MonoBehaviour
{
    [SerializeField] Material baseMaterial;

    [SerializeField] Vector2 hueRange;
    [SerializeField] Vector2 saturationRange;
    [SerializeField] Vector2 brightnessRange;

    Material generatedMaterial;

    public Material generateMaterial()
    {
        Material newMaterial = new Material(baseMaterial);

        newMaterial.color = Color.HSVToRGB(
            Random.Range(hueRange.x, hueRange.y) / 355f, 
            Random.Range(saturationRange.x, saturationRange.y) / 100f, 
            Random.Range(brightnessRange.x, brightnessRange.y) / 100f
        );

        generatedMaterial = newMaterial;

        return newMaterial;
    }

    public void applyMaterial(Material mat)
    {
        gameObject.GetComponent<Renderer>().material = mat;
    }

    public void applyMaterial()
    {
        applyMaterial(generatedMaterial);
    }

    public void generateApplyMaterial()
    {
        generateMaterial();
        applyMaterial();
    }


}
