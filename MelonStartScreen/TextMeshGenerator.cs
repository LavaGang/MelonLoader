using System;
using System.Linq;
using UnityEngine;

namespace MelonLoader.MelonStartScreen
{
    internal static class TextMeshGenerator
    {
        public static Mesh Generate(string text, TextGenerationSettings settings)
        {
            TextGenerator generator = new TextGenerator();
            generator.Populate(text, settings);

            Mesh mesh = new Mesh();
            UIVertex[] vertices = generator.GetVerticesArray();
            mesh.vertices = vertices.Select(v => v.position).ToArray();
            if (GL.sRGBWrite)
                mesh.colors = vertices.Select(v => {
                    Color color = v.color;
                    color.r = (float)Math.Pow(color.r, 2.2);
                    color.g = (float)Math.Pow(color.g, 2.2);
                    color.b = (float)Math.Pow(color.b, 2.2);
                    return color;
                }).ToArray();
            else
                mesh.colors32 = vertices.Select(v => v.color).ToArray();
            mesh.normals = vertices.Select(v => v.normal).ToArray();
            mesh.tangents = vertices.Select(v => v.tangent).ToArray();
            mesh.uv = vertices.Select(v => v.uv0).ToArray();

            int characterCount = generator.vertexCount / 4;
            int[] indices = new int[characterCount * 6];
            for (var i = 0; i < characterCount; ++i)
            {
                int vertIndexStart = i * 4;
                int trianglesIndexStart = i * 6;
                indices[trianglesIndexStart++] = vertIndexStart;
                indices[trianglesIndexStart++] = vertIndexStart + 1;
                indices[trianglesIndexStart++] = vertIndexStart + 2;
                indices[trianglesIndexStart++] = vertIndexStart;
                indices[trianglesIndexStart++] = vertIndexStart + 2;
                indices[trianglesIndexStart] = vertIndexStart + 3;
            }
            mesh.triangles = indices;
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}
