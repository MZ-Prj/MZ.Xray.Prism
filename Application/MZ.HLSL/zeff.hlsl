sampler2D ZeffTexture : register(s0);

float ZeffMin : register(c0);
float ZeffMax : register(c1);

float4 Color : register(c2);

float4 main(float2 uv : TEXCOORD0) : COLOR
{
    float zeff = tex2D(ZeffTexture, uv).r;
    
    if (zeff > ZeffMin && zeff < ZeffMax)
    {
         return Color;
    }
    else
    {
         return float4(0.0, 0.0, 0.0, 0.0);
    }
}