#version 460

uniform sampler2D spriteTexture;
uniform float alpha;
uniform vec3 spriteColor;
uniform int shade;
uniform float intensity;

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
	color.w *= alpha;
	color.xyz *= spriteColor;

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

		red = scale(intensity, 0, 1, color.x, gray);
		blue = scale(intensity, 0, 1, color.z, gray);

		final = vec4(red, color.y, blue, color.w);
		break;
		//Blue
		case 4:
		gray = dot(color.xy, vec2(r, g));

		red = scale(intensity, 0, 1, color.x, gray);
		green = scale(intensity, 0, 1, color.y, gray);

		final = vec4(red, green, color.z, color.w);
		break;
	}
}