#version 460

layout(location = 0) uniform sampler2D circleTexture;
layout(location = 1) uniform sampler2D glowTexture;
//uniform int shade;
//uniform float intensity;

in vec2 texCoords;
flat in int white;
in vec4 bColor;

out vec4 final;

const float r = 0.299;
const float g = 0.587;
const float b = 0.114;

float scale(float value, float inputMin, float inputMax, float outputMin, float outputMax)
{
    float scale = (outputMax - outputMin) / (inputMax - inputMin);
    return outputMin + (value - inputMin) * scale;
}

void main()
{	
	vec4 color;

	if (white == 0)
	{
		color = texture(glowTexture, texCoords);
		color.xyz *= bColor.xyz;
		color.w *= 0.5;
	}
	else
	{
		color = texture(circleTexture, texCoords);
	}

	color.w *= bColor.w;

	final = color;
}