sampler2D inputTexture : register(s0);

cbuffer ShaderParams : register(b0)
{
    float2 textureSize;       
    float sharpnessLevel;     
    float brightnessLevel;    
    float contrastLevel;      
    float filterMode;  
}

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 texelSize = 1.0 / textureSize;
    float4 center = tex2D(inputTexture, uv);
    float4 up     = tex2D(inputTexture, uv + float2(0.0, -texelSize.y));
    float4 down   = tex2D(inputTexture, uv + float2(0.0,  texelSize.y));
    float4 left   = tex2D(inputTexture, uv + float2(-texelSize.x, 0.0));
    float4 right  = tex2D(inputTexture, uv + float2( texelSize.x, 0.0));
    
    float4 sharpened = center * (1.0 + 4.0 * sharpnessLevel)  
                     - (up + down + left + right) * sharpnessLevel;

    sharpened.rgb += brightnessLevel;

    float meanBrightness = dot(center.rgb, float3(0.299, 0.587, 0.114));
    sharpened.rgb = (sharpened.rgb - meanBrightness) * contrastLevel + meanBrightness;

    float gray = dot(sharpened.rgb, float3(0.299, 0.587, 0.114));
    float4 grayColor = float4(gray, gray, gray, sharpened.a);

    if (filterMode == 0)  
    { 
        return grayColor; 
    } 
    else if (filterMode == 1)  
    { 
        return saturate(sharpened);  
    } 
    else if (filterMode == 2 || filterMode == 3 || filterMode == 4)  
    { 
        float maxChannel = max(max(sharpened.r, sharpened.g), sharpened.b);
        bool isRedMax   = (filterMode == 2) && (sharpened.r == maxChannel);
        bool isGreenMax = (filterMode == 3) && (sharpened.g == maxChannel);
        bool isBlueMax  = (filterMode == 4) && (sharpened.b == maxChannel);

        return (isRedMax || isGreenMax || isBlueMax) ? saturate(sharpened) : grayColor;
    }

    return saturate(sharpened);
}