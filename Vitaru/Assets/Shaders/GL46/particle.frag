#version 460

uniform sampler2D spriteTexture;

in vec2 texCoords;
in vec4 pColor;

out vec4 final;

void main()
{
	final = texture(spriteTexture, texCoords) * pColor;
}