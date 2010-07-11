texture Texture0;
sampler2D sampler0 = sampler_state
{
   Texture = (Texture0);
};

texture TextureMask;
sampler2D samplerMask = sampler_state
{
   Texture = (TextureMask);
};
// TODO: add effect parameters here.

struct VertexShaderInput
{
    float4 Position : POSITION0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

float4 PixelShader(float2 texCoord: TEXCOORD0) : COLOR
{
	float4 input = tex2D(sampler0, texCoord);
	float4 mask = tex2D(samplerMask, texCoord);
	return float4(input.rgb, mask.r * input.a);
}
technique
{
	pass P0
	{
		 AlphaBlendEnable = true;
        SrcBlend = SrcAlpha;
        DestBlend = InvSrcAlpha;
		PixelShader = compile ps_2_0 PixelShader();
	}
}
