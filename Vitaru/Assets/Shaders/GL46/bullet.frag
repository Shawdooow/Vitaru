#version 460

uniform sampler2D spriteTexture;
uniform int shade;

in vec2 texCoords;
in vec4 pColor;

out vec4 final;

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
		final = vec4(gray, color.y, gray, color.w);
		break;
		//Blue
		case 4:
		gray = dot(color.xy, vec2(r, g));
		final = vec4(vec2(gray), color.z, color.w);
		break;
	}
}