#version 460

uniform sampler2D buffTexture;
uniform int shade;

in vec2 texCoords;
out vec4 final;

void main()
{	
	if (shade == 1)
	{
		float gray = dot(texture(buffTexture, texCoords).xyz, vec3(0.299, 0.587, 0.114));
		final = vec4(vec3(gray), 1.0);
	}
	else if (shade == 2)
	{
		vec4 color = texture(buffTexture, texCoords);
		float gray = dot(color.yz, vec2(0.587, 0.114));
		final = vec4(color.x, vec2(gray), 1.0);
	}
	else
		final = texture(buffTexture, texCoords);
}