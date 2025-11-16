Shader "Custom/AffineMapping"
{
    SubShader
    {
        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_Texture);
            SAMPLER(sampler_Texture);

            CBUFFER_START(UnityPerMaterial)
            float4 _Texture_ST;
            float4 _Texture_TexelSize;
            CBUFFER_END

            struct VertexInput
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct VertexOutput
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
        ENDHLSL

        Pass
        {
            Cull Back
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            VertexOutput vert(VertexInput i)
            {
                VertexOutput o;

                o.positionCS = TransformObjectToHClip(i.positionOS);
                o.uv = TRANSFORM_TEX(i.uv, _Texture);

                return o;
            }

            float4 frag(VertexOutput i) : SV_Target
            {
                float4 col = SAMPLE_TEXTURE2D(_Texture, sampler_Texture, i.uv);
                return col;
            }
        
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}
