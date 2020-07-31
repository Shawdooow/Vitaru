#version 460

layout (location = 0) in vec2 pos;

uniform mat4 projection;
uniform int index;
uniform vec2 size;

layout(std430, binding = 2) buffer particleBuffer
{
	mat4[] models;
};

out vec2 texCoords;

void main()
{
	texCoords = vec2(pos.x + 0.5, pos.y + 0.5);
	gl_Position = projection * models[index] * (vec4(size, 0, 1.0) * vec4(pos, 0, 1.0));
}