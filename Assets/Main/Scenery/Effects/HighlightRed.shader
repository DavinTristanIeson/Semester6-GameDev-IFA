Shader "Custom/FinalPhase" {
  Properties {
    _MainTex("Texture", 2D) = "white" {}
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

      float4 frag(v2f_img input) : COLOR {
        // sample texture for color
        float4 base = tex2D(_MainTex, input.uv);
        float gray = (base.r * 0.3333 + base.g * 0.3333 + base.b * 0.3333) * 0.4;

        float maxDiff = max(base.r - base.g, base.r - base.b);
        // return float4(maxDiff, maxDiff, maxDiff, 1.0);

        int exceedRThreshold = maxDiff - 0.5;
        // return float4(exceedRThreshold, exceedRThreshold, exceedRThreshold, 1.0);

        int enable = round(maxDiff);
        return base * enable + float4(gray, gray, gray, base.a) * (1 - enable);
      }
      ENDCG
}}}