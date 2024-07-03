using UnityEngine;

public class PlayerStateBasedCameraEffects : MonoBehaviour {
  public Material DefaultMaterial;
  public Material FinalPhaseMaterial;
  private Material material;
  private float inversion = 0.0f;
  private float lastHealth = 1.0f;

  void OnRenderImage(RenderTexture source, RenderTexture destination) {
    if (material == null){
      material = DefaultMaterial;
    }
    Graphics.Blit(source, destination, material);
  }


  void Update(){
    if (inversion > 0.0f){
      inversion = Mathf.Max(0.0f, inversion - 0.04f);
      material.SetFloat("Inversion", inversion);
    }
  }

  void OnEnable(){
    material = DefaultMaterial;
    material.SetFloat("Inversion", 0);
    material.SetFloat("Vignette Softness", 0);
    material = DefaultMaterial;
  }

  public void TriggerHealth(float healthPercentage){
    if (lastHealth == healthPercentage){
      return;
    }
    float VignetteSoftness = 1.2f * (1f - healthPercentage);
    if (lastHealth > healthPercentage){
      inversion = 1.0f;
    }
    lastHealth = healthPercentage;
    DefaultMaterial.SetFloat("VignetteSoftness", VignetteSoftness);
  }

  public void StartFinalPhaseSequence(){
    material = FinalPhaseMaterial;
  }
}