#version 460

layout(location = 10) in vec2 vertex;
layout(location = 11) in vec2 pos;
layout(location = 12) in float size;
layout(location = 13) in vec4 circleColor;
layout(location = 14) in vec4 glowColor;

uniform mat4 projection;
uniform vec2 scale;

out vec2 texCoords;
out vec4 cColor;
out vec4 gColor;
flat out int white;

mat4 identity()
{
    return mat4(
    1, 0, 0, 0,
    0, 1, 0, 0,
    0, 0, 1, 0,
    0, 0, 0, 1);
}

mat4 sca(float s)
{
	return mat4(s, 0, 0, 0,
	            0, s, 0, 0,
	            0, 0, s, 0,
	            0, 0, 0, 1);
}

mat4 translateRow(vec2 p)
{
	return mat4(1, 0, 0, 0,
	            0, 1, 0, 0,
	            0, 0, 1, 0,
	            p.x, p.y, 0, 1);
}

void main()
{
	texCoords = vec2(vertex.x + 0.5, vertex.y + 0.5);
	white = gl_InstanceID % 2;

	float s = 1;

	if (white == 0)
		s = 3;

	mat4 model = identity();
	model *= translateRow(pos * scale);
	model *= sca(s * min(scale.x, scale.y));

	gl_Position = projection * model * (vec4(size, size, 0, 1.0) * vec4(vertex, 0, 1.0));

	cColor = circleColor;
	gColor = glowColor;
}