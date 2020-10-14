#version 460

layout(location = 20) in vec2 vertex;
layout(location = 21) in vec2 pos;
layout(location = 22) in vec3 size;
layout(location = 23) in vec4 color;

uniform mat4 projection;

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

mat4 scale(vec3 s)
{
	return mat4(s.x, 0, 0, 0,
	            0, s.y, 0, 0,
	            0, 0, s.z, 0,
	            0, 0, 0, 1);
}

mat4 rotateZ(float rad)
{
	return mat4(cos(rad), -sin(rad), 0, 0,
	            sin(rad), cos(rad), 0, 0,
	            0, 0, 1.0, 0,
	            0, 0, 0, 1.0);
}

mat4 translateColumn(vec2 p)
{
	return mat4(1, 0, 0, p.x,
	            0, 1, 0, p.y,
	            0, 0, 1, 0,
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

	mat4 model = identity();
	model *= translateRow(pos);
	//model *= rotateZ();
	model *= scale(vec3(size, 1));

	gl_Position = projection * model * (vec4(vec2(size.z), 0, 1.0) * vec4(vertex, 0, 1.0));

	pColor = color;
}