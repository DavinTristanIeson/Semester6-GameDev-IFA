using UnityEngine;

public class PlayerStateBasedCameraEffects : MonoBehaviour {
  public Material material;
  private float inversion = 0.0f;
  private float lastHealth = 1.0f;
  private float finalPhaseVignetteRadius = 1.0f;
  private float finalPhaseVignetteSoftness = 0.0f;
  void OnRenderImage(RenderTexture source, RenderTexture destination) {
    Graphics.Blit(source, destination, material);
  }

  void Update(){
    if (inversion > 0.0f){
      inversion = Mathf.Max(0.0f, inversion - 0.04f);
      material.SetFloat("Inversion", inversion);
    }
    if (finalPhaseVignetteRadius < 1.0f){
      finalPhaseVignetteRadius = Mathf.Min(1.0f, finalPhaseVignetteRadius + 0.0015f);
      material.SetFloat("VignetteRadius", finalPhaseVignetteRadius);
    }
    if (finalPhaseVignetteSoftness > 0.0f){
      finalPhaseVignetteSoftness = Mathf.Max(0.0f, finalPhaseVignetteSoftness - 0.0015f);
      material.SetFloat("VignetteSoftness", finalPhaseVignetteSoftness);
    }
  }

  void OnEnable(){
    material.SetFloat("Inversion", 0);
    material.SetFloat("VignetteSoftness", 0);
    material.SetFloat("VignetteRadius", 1);
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
    material.SetFloat("VignetteSoftness", VignetteSoftness);
  }

  public void StartFinalPhaseSequence(){
    finalPhaseVignetteRadius = 0.0f;
    finalPhaseVignetteSoftness = 1.0f;
  }
}