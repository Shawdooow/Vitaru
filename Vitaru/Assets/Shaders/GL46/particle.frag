#version 460

uniform sampler2D spriteTexture;
uniform int shade;
uniform float intensity;

uniform highp int index;

struct Particle
{
	vec4 StartPosition;
	vec4 EndPosition;
    vec4 Color;
    mat4 model;
};

layout(std430, binding = 2) buffer particleBuffer
{
	Particle[] particles;
};

in vec2 texCoords;
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
	float red;
	float green;
	float blue;
	float gray;

	vec4 color = texture(spriteTexture, texCoords);
	color.w *= particles[index].Color.w;
	color.xyz *= particles[index].Color.xyz;

	switch(shade)
	{
		default:
		final = color;
		break;
		//Gray
		case 1:
		gray = dot(color.xyz, vec3(r, g, b));

		red = scale(intensity, 0, 1, color.x, gray);
		green = scale(intensity, 0, 1, color.y, gray);
		blue = scale(intensity, 0, 1, color.z, gray);

		final = vec4(red, green, blue, color.w);
		break;
		//Red
		case 2:
		gray = dot(color.yz, vec2(g, b));

		green = scale(intensity, 0, 1, color.y, gray);
		blue = scale(intensity, 0, 1, color.z, gray);

		final = vec4(color.x, green, blue, color.w);
		break;
		//Green
		case 3:
		gray = dot(color.xz, vec2(r, b));
		final = vec4(gray, color.y, gray, color.w);
		break;
		//Blue
		case 4:
		gray = dot(color.xy, vec2(r, g));
		final = vec4(vec2(gray), color.z, color.w);
		break;
	}
}