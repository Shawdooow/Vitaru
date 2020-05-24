#version 460

uniform sampler2D spriteTexture;
uniform float alpha;
uniform vec3 spriteColor;

in vec2 texCoords;
out vec4 color;

void main()
{	
	color = texture(spriteTexture, texCoords);
	color.w *= alpha;
	color.xyz *= spriteColor;
}