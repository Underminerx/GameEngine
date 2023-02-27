#shader vertex
#version 410 core
layout(location = 0) in vec3 aPos;
layout(location = 1) in vec3 aColor;
layout(location = 0) out vec3 color;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
    color = aColor;
}


#shader fragment
#version 410 core
out vec4 FragColor;
layout(location = 0) in vec3 color;

void main()
{
    FragColor = vec4(color, 1.0f);
}