#version 460

uniform sampler2D circleTexture;
uniform sampler2D glowTexture;

in vec2 texCoords;
flat in int white;
in vec4 bColor;

out vec4 final;

void main()
{	
	vec4 color;

	if (white == 0)
	{
		color = texture(glowTexture, texCoords) * bColor;
	}
	else
	{
		color = texture(circleTexture, texCoords);
		color.w *= bColor.w;
	}

	final = color;
}