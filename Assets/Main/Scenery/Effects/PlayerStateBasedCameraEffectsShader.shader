Shader "Custom/PostProcess" {
  Properties {
    _MainTex("Texture", 2D) = "white" {}
    VignetteRadius("Vignette Radius", Range(0.0, 1.0)) = 1.0
    VignetteSoftness("Vignette Softness", Range(0.0, 1.0)) = 0.0
  }

  SubShader {
    Pass {
      CGPROGRAM
      #pragma vertex vert_img
      #pragma fragment frag
      #include "UnityCG.cginc" // required for v2f_img

      // REFERENCE: https://lindenreidblog.com/2018/02/05/camera-shaders-unity/

      // Properties
      sampler2D _MainTex;
      float VignetteRadius;
      float VignetteSoftness;
      float Inversion;

      float4 frag(v2f_img input) : COLOR {
        // sample texture for color
        float4 base = tex2D(_MainTex, input.uv);
        float distFromCenter = distance(input.uv.xy, float2(0.5, 0.5));
        float vignette = smoothstep(VignetteRadius, VignetteRadius - VignetteSoftness, distFromCenter);
        base = saturate(base * vignette);
        return max(base - (Inversion / 2), (Inversion / 2) - base);
      }
      ENDCG
}}}