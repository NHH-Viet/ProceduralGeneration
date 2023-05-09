Shader "Custom/Terrain" {
	Properties {
		testTexture("Texture",2D) = "white"{}
		testScale("Scale", Float) = 1
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		const static int maxLayerCount = 8;
		const static float eps= 1E-4; // debug chia 0

        int layerCount;
        float3 baseColor[maxLayerCount]; //màu
        float baseStartHeight[maxLayerCount];// chiều cao bắt đầu
		float baseBlends[maxLayerCount];// độ hòa màu
		float baseColorStr[maxLayerCount];
		float baseTextureScale[maxLayerCount];
		float baseTextureActive[maxLayerCount];

		float minHeight;
		float maxHeight;

		sampler2D testTexture;
		float testScale;

		UNITY_DECLARE_TEX2DARRAY(baseTexture);
		struct Input {
			float3 worldPos;
			float3 worldNormal;
		};

		float inverseLerp(float a, float b, float value) {
			return saturate((value-a)/(b-a));
		}

		float3 triplanar(float3 worldPos, float scale, float3 blendAxes,int textureIndex){
			float3 scaleWorlPos = worldPos / scale;
			float3 xProject = UNITY_SAMPLE_TEX2DARRAY(baseTexture, float3(scaleWorlPos.y, scaleWorlPos.z, textureIndex))* blendAxes.x;
			float3 yProject = UNITY_SAMPLE_TEX2DARRAY(baseTexture, float3(scaleWorlPos.x, scaleWorlPos.z, textureIndex))* blendAxes.y;
			float3 zProject = UNITY_SAMPLE_TEX2DARRAY(baseTexture, float3(scaleWorlPos.x, scaleWorlPos.y, textureIndex))* blendAxes.z;

			return xProject + yProject + zProject;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			float heightPercent = inverseLerp(minHeight,maxHeight, IN.worldPos.y);
			float3 blendAxes = abs(IN.worldNormal);
			blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z ; // đảm bảo giá trị cộng lại ko lớn hơn 1

			for (int i = 0; i < layerCount; i ++) {
				if(baseTextureActive[i]){
					//float drawStrength = saturate(sign(heightPercent - baseStartHeight[i]));
					//Drawstr bằng 0 nếu pixel hiện tại bằng một nữa baseblend mà nhỏ hơn starting height và bằng 1 nếu bằng một nữa baseblend mà to hơn starting height
					//thêm '- eps' để tránh chia bằng 0
					float drawStrength = inverseLerp(-baseBlends[i]/2 - eps, baseBlends[i]/2, heightPercent - baseStartHeight[i] );
					float3 baseColour = baseColor[i]*baseColorStr[i];
					float3 textureColor = triplanar(IN.worldPos,baseTextureScale[i], blendAxes, i)*(1-baseColorStr[i]);
					o.Albedo = o.Albedo * (1-drawStrength) + (baseColour+textureColor) * drawStrength;
				}
				else
					continue;
			}

		}


		ENDCG
	}
	FallBack "Diffuse"
}
