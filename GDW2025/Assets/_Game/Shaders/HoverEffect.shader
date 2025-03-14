Shader "Custom/HoverEffect"
{
    Properties
    {
        _HoverColor ("Hover Farbe", Color) = (1,1,1,1)
        _PulseSpeed ("Puls Geschwindkeit", Float) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            // Additive Blending und kein Tiefenschreiben für Transparenz
            Blend SrcAlpha One
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _HoverColor;
            float _PulseSpeed;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                // Transformation in Clipspace
                OUT.position = TransformObjectToHClip(IN.vertex);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                
                // Berechne den minimalen Abstand zum Rand (0 .. 0.5)
                float minEdge = min(min(uv.x, 1.0 - uv.x), min(uv.y, 1.0 - uv.y));

                // Bestimme die Breite des sichtbaren Randbereichs (ca. 20 %)
                float borderWidth = 0.2;

                // Erzeuge einen weichen Übergang: Außen (Wert 1) bis innen (Wert 0)
                float borderFactor = smoothstep(borderWidth, 0.0, minEdge);

                // Pulsierender Effekt
                float pulse = 0.75 + 0.25 * sin(_Time.y * _PulseSpeed);

                // Kombiniere Rand- und Puls-Effekt zum finalen Alpha
                float alpha = borderFactor * pulse;

                // Wende die Hover-Farbe an und moduliere den Alpha-Wert
                float4 color = _HoverColor;
                color.a *= alpha;

                return color * 1.25f;
            }
            ENDHLSL
        }
    }
}
