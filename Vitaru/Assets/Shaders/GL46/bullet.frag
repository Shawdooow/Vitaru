#version 460

uniform sampler2D circleTexture;
uniform sampler2D glowTexture;

in vec2 texCoords;
in vec4 cColor;
in vec4 gColor;
flat in int white;

out vec4 final;

void main()
{	
	vec4 color;

	if (white == 0)
	{
		color = texture(glowTexture, texCoords) * gColor;
	}
	else
	{
		color = texture(circleTexture, texCoords) * cColor;
	}

	final = color;
}