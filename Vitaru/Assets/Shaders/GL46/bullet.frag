#version 460

uniform highp int white;
uniform sampler2D spriteTexture;
uniform int shade;
uniform float intensity;

in vec2 texCoords;
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
	vec4 color = texture(spriteTexture, texCoords);
	color.w *= bColor.w;

	if (white == 0)
		color.xyz *= bColor.xyz;

	final = color;
}