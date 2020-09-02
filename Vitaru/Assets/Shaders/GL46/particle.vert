#version 460

layout(location = 0) in vec2 vertex;
layout(location = 1) in float lifetime;
//TODO: vec4 positions
layout(location = 2) in vec2 startPos;
layout(location = 3) in vec2 endPos;
layout(location = 4) in vec4 color;

uniform mat4 projection;
uniform mat4 parent;

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

float scale(float value, float inputMin, float inputMax, float outputMin, float outputMax)
{
    float scale = (outputMax - outputMin) / (inputMax - inputMin);
    return outputMin + (value - inputMin) * scale;
}

void main()
{
	texCoords = vec2(vertex.x + 0.5, vertex.y + 0.5);

	float particleSize = color.w;

	vec2 pos = startPos;

	mat4 model = identity();
	model *= translateRow(pos);
	model *= parent;

	gl_Position = projection * model * (vec4(vec2(color.w), 0, 1.0) * vec4(vertex, 0, 1.0));

	pColor = vec4(color.xyz, 1);
}

//return --time * time * time + 1;

//drawTransform *= draw.ScaleTransform;
//drawTransform *= draw.RotationTransform;
//drawTransform *= draw.TranslationTransform;
//drawTransform *= parentTransform;