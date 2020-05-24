#version 460

uniform sampler2D spriteTexture;
uniform float alpha;
uniform vec3 spriteColor;
uniform float startAngle;
uniform float endAngle;

in vec2 texCoords;
out vec4 color;

const float PI = 3.1415926535897932384626433832795;

void main()
{	
	float angle = atan(-texCoords.y + 0.5, texCoords.x - 0.5) - PI / 2;

	if (angle < 0)
	{
		angle += PI * 2;
	}

	if (angle >= startAngle && angle <= endAngle)
	{
		color = texture(spriteTexture, texCoords);
		color.w *= alpha;
		color.xyz *= spriteColor;
	}
	else
		discard;
}