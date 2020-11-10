#version 460

layout(location = 10) in vec2 vertex;
layout(location = 11) in float lifetime;
layout(location = 12) in vec4 position;
layout(location = 13) in vec4 color;

uniform mat4 projection;
uniform float size;

out vec2 texCoords;
out vec4 pColor;

mat4 identity()
{
    return mat4(
    1, 0, 0, 0,
    0, 1, 0, 0,
    0, 0, 1, 0,
    0, 0, 0, 1);
}

mat4 scale(float s)
{
	return mat4(s, 0, 0, 0,
	            0, s, 0, 0,
	            0, 0, s, 0,
	            0, 0, 0, 1);
}

mat4 rotateZ(float rad)
{
	return mat4(cos(rad), -sin(rad), 0, 0,
	            sin(rad), cos(rad), 0, 0,
	            0, 0, 1.0, 0,
	            0, 0, 0, 1.0);
}

mat4 translateRow(vec2 p)
{
	return mat4(1, 0, 0, 0,
	            0, 1, 0, 0,
	            0, 0, 1, 0,
	            p.x, p.y, 0, 1);
}

float scale(float value, float inputMin, float inputMax, float outputMin, float outputMax)
{
    float s = (outputMax - outputMin) / (inputMax - inputMin);
    return outputMin + (value - inputMin) * s;
}

in int gl_InstanceID;
void main()
{
	texCoords = vec2(vertex.x + 0.5, vertex.y + 0.5);
	int clockwise = gl_InstanceID % 2;

	float easing = 1 - pow(1 - (lifetime / 3 * 2), 3);
	float fade = 1 - pow(1 - lifetime, 3);

	float x = scale(easing, 0, 1, position.x, position.z);
	float y = scale(easing, 0, 1, position.y, position.w);

	vec2 pos = vec2(x, y);

	float dir = 4;

	if (clockwise == 0)
		dir = -dir;

	mat4 model = identity();
	model *= translateRow(pos);
	model *= rotateZ(scale(easing, 0, 1, 0, dir));
	model *= scale(size);

	gl_Position = projection * model * (vec4(vec2(color.w), 0, 1.0) * vec4(vertex, 0, 1.0));

	pColor = vec4(color.xyz, 1 - fade);
}