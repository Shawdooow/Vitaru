#version 460

layout(location = 20) in vec2 vertex;
layout(location = 21) in vec2 pos;
layout(location = 22) in vec2 size;
layout(location = 23) in vec4 color;

uniform mat4 projection;

out vec2 texCoords;
out vec4 spriteColor;

mat4 identity()
{
    return mat4(
    1, 0, 0, 0,
    0, 1, 0, 0,
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
	spriteColor = color;

	mat4 model = identity();
	model *= translateRow(pos);

	gl_Position = projection * model * (vec4(size, 0, 1.0) * vec4(vertex, 0, 1.0));
}