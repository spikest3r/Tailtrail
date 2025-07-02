Shader "Custom/ReplaceColor" {
    Properties {
        _MainTex ("Sprite", 2D) = "white" {}
        _TargetColor ("Target Color", Color) = (1,1,1,1)
        _ReplaceColor ("Replace Color", Color) = (1,0,0,1)
        _Tolerance ("Tolerance", Range(0,1)) = 0.1
    }
    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _TargetColor;
            float4 _ReplaceColor;
            float _Tolerance;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                float diff = distance(col.rgb, _TargetColor.rgb);
                if (diff < _Tolerance) {
                    col.rgb = _ReplaceColor.rgb;
                }
                return col;
            }
            ENDCG
        }
    }
}
