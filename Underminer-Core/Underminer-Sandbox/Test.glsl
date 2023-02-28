#shader vertex

#version 410 core
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec2 aTexCoords;
       
layout (location = 0) out vec3 color;
layout (location = 1) out vec2 texCoords;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
    color = aColor;
    texCoords = aTexCoords;
}




#shader fragment

#version 410 core

layout (location = 0) in vec3 color;
layout (location = 1) in vec2 texCoords;

out vec4 FragColor;

uniform sampler2D mainTex;

void main()
{
    FragColor = texture(mainTex, texCoords);
}