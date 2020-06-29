#version 460

struct Particle
{
	//Raw data
	vec4 Position;
	vec4 Rotation;

	//computed data for drawing
	mat4 Transform;
};

layout(std430, binding = 2) buffer particleBuffer
{
	Particle[] particles;
};