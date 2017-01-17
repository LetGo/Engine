Shader "bmt/postRender/occlusion"  
{  
    Properties  
    {  
        _MainTex("Base (RGB)", 2D) = "white" {}  
        _DepthMap("DepthMap (RGB)", 2D) = "white" {}  
        _OcclusionMap("OcclusionMap (RGB)", 2D) = "white" {}  
        _Intensity("Intensity", Float) = 0.0  
        _Tiling("Tiling", Vector) = (1.0, 1.0, 0.0, 0.0)  
		_Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }  
  
    SubShader  
    {  
        Pass  
        {  
			//Blend SrcAlpha OneMinusSrcAlpha
            ZTest Always  
            ZWrite Off  
            Cull Off  
            Fog{ Mode Off }  
  
            CGPROGRAM  
            #include "UnityCG.cginc"  
            #pragma vertex vert  
            #pragma fragment frag  
            #pragma fragmentoption ARB_precision_hint_fastest  
  
            sampler2D _MainTex;  
            sampler2D _DepthMap;  
            sampler2D _OcclusionMap;  
            sampler2D _CameraDepthNormalsTexture;  
			sampler2D _CameraDepthTexture;  
            fixed4 _MainTex_TexelSize;  
            fixed _Intensity;  
            fixed _Power;  
            fixed4 _Tiling;  
			uniform half4 _Color;
			
            struct a2v  
            {  
                fixed4 vertex : POSITION;  
				float3 normal : NORMAL; 
                fixed2 texcoord : TEXCOORD0;  
				
            };  
  
            struct v2f  
            {  
                fixed4 vertex : SV_POSITION;  
                fixed2 uv : TEXCOORD0;  
				float2 cap : TEXCOORD1;
				float4 proj0   	 : TEXCOORD2;
            };  
  
            v2f vert(a2v v)  
            {  
                v2f o;  
                o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);  
                o.uv = v.texcoord.xy;  
				
				//o.proj0 = ComputeScreenPos(o.vertex);
				//COMPUTE_EYEDEPTH(o.proj0.z);
			
							
				//half2 capCoord;
                //capCoord.x = dot(UNITY_MATRIX_IT_MV[0].xyz,v.normal);
                //capCoord.y = dot(UNITY_MATRIX_IT_MV[1].xyz,v.normal);
                //o.cap = capCoord * 0.5 + 0.5;
				
                return o;  
            }  
  
            fixed4 frag(v2f i) : SV_Target  
            {  
                fixed4 c = tex2D(_MainTex, i.uv);  
  
                fixed4 depthMap = tex2D(_DepthMap, i.uv);  
                fixed depth = DecodeFloatRG(depthMap.zw);  
	            fixed3 normal = DecodeViewNormalStereo(depthMap);  
	      				 
                fixed4 cameraDepthMap = tex2D(_CameraDepthNormalsTexture, i.uv);  
                fixed cameraDepth = DecodeFloatRG(cameraDepthMap.zw);  
          

			
                fixed4 o = c;  
                if (depth > 0 && cameraDepth < depth)  
                {  
                    fixed2 uv = i.uv * _Tiling.xy + _Tiling.zw;  
                    fixed3 color = tex2D(_OcclusionMap, uv);  
                    fixed nf = saturate(dot(normal, fixed3(0, 0, 1)));  
                    nf = pow(nf, _Intensity);  
                    o.rgb = lerp(color, c.rgb, nf); 
					o.rgb *=_Color*1.1;
					
                }  
	
	
                return o;  
            }  
  
            ENDCG  
        }  
    }  
  
    Fallback off  
}  