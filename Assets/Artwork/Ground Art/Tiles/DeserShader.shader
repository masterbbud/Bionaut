Shader "Hidden/DesertShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ParticleSize("Particle Size", Range(1.0, 10.0)) = 2.0
        _ParticleSlowness("Particle Slowing Factor", Range(1.0, 5000.0)) = 1000 // Original speed
        _TimeScale("Time Scale", Range(0.1, 1.0)) = 0.7 // For fade and other effects
        _ColorA("Sand Color A", Color) = (0.95, 0.75, 0.45, 1)
        _ColorB("Sand Color B", Color) = (0.9, 0.65, 0.40, 1)
        _ColorC("Sand Color C", Color) = (0.5, 0.25, 0.18, 1)
        _FadeInTime("Fade In Time", Range(1.0, 30.0)) = 6.0
        _FadeOutTime("Fade Out Time", Range(1.0, 10.0)) = 2.5
        _MaxFade("Maximum Effect Fade", Range(0.0, 1.0)) = 0.9
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _ParticleSlowness;
            float _ParticleSize;
            float _TimeScale;
            float4 _ColorA;
            float4 _ColorB;
            float4 _ColorC;
            float _FadeInTime;
            float _FadeOutTime;
            float _MaxFade;

            float rand(float2 pix)
            {
                // Original particle speed logic
                return frac(sin(dot(pix.xy, float2(12.345, 67.89))) * 98765.1234 + _Time.w / _ParticleSlowness);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 base = tex2D(_MainTex, i.uv);

                // Generate static noise
                float staticNoise = rand(floor(i.uv * _ParticleSize * float2(160, 90)));

                // Assign colors based on thresholds
                float4 sandstorm = staticNoise < 0.97 ? _ColorA : staticNoise < 0.99 ? _ColorB : _ColorC;

                // Fade logic
                float fadeTime = fmod((_Time.w * _TimeScale), _FadeInTime + _FadeOutTime);
                float fadingOut = step(_FadeInTime, fadeTime);
                float fadeAmount = (1 - fadingOut) * (fadeTime / _FadeInTime)
                                + (fadingOut) * (1 - (fadeTime - _FadeInTime) / _FadeOutTime);

                // Blend the base texture with the sandstorm effect
                base = lerp(base, sandstorm, min(fadeAmount, _MaxFade));

                return base;
            }
            ENDCG
        }
    }
}
