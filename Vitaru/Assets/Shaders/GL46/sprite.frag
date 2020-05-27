#version 460

uniform sampler2D spriteTexture;
uniform float alpha;
uniform vec3 spriteColor;
uniform int shade;

in vec2 texCoords;
out vec4 final;

const float r = 0.299;
const float g = 0.587;
const float b = 0.114;

void main()
{	
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
		final = vec4(vec3(gray), color.w);
		break;
		//Red
		case 2:
		gray = dot(color.yz, vec2(g, b));
		final = vec4(color.x, vec2(gray), color.w);
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