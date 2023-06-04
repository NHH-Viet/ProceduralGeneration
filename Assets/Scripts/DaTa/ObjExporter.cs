using UnityEngine;
using System.Text;
public class ObjExporter
{
    /*public static string MeshToObj(Mesh mesh)
     {
         string objData = "g ExportedMesh\n";

         for (int i = 0; i < mesh.vertices.Length; i++)
         {
             objData += "v " + mesh.vertices[i].x + " " + mesh.vertices[i].y + " " + mesh.vertices[i].z + "\n";
             objData += "vn " + mesh.normals[i].x + " " + mesh.normals[i].y + " " + mesh.normals[i].z + "\n";
             objData += "vt " + mesh.uv[i].x + " " + mesh.uv[i].y + "\n";
         }

         int[] triangles = mesh.triangles;
         for (int i = 0; i < triangles.Length; i += 3)
         {
             int vertexIndex1 = triangles[i] + 1;
             int vertexIndex2 = triangles[i + 1] + 1;
             int vertexIndex3 = triangles[i + 2] + 1;

             objData += "f " + vertexIndex1 + "/" + vertexIndex1 + "/" + vertexIndex1 + " "
                               + vertexIndex2 + "/" + vertexIndex2 + "/" + vertexIndex2 + " "
                               + vertexIndex3 + "/" + vertexIndex3 + "/" + vertexIndex3 + "\n";
         }


         return objData;
     }*/
    public static string MeshToObj(Mesh mesh)
    {
        StringBuilder objData = new StringBuilder(mesh.vertices.Length * 100); // Adjust the capacity based on your mesh size

        objData.Append("g ExportedMesh\n");

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            objData.Append("v ").Append(mesh.vertices[i].x).Append(" ").Append(mesh.vertices[i].y).Append(" ").Append(mesh.vertices[i].z).Append("\n");
            objData.Append("vn ").Append(mesh.normals[i].x).Append(" ").Append(mesh.normals[i].y).Append(" ").Append(mesh.normals[i].z).Append("\n");
            objData.Append("vt ").Append(mesh.uv[i].x).Append(" ").Append(mesh.uv[i].y).Append("\n");
        }

        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int vertexIndex1 = triangles[i] + 1;
            int vertexIndex2 = triangles[i + 1] + 1;
            int vertexIndex3 = triangles[i + 2] + 1;

            objData.Append("f ").Append(vertexIndex1).Append("/").Append(vertexIndex1).Append("/").Append(vertexIndex1)
                   .Append(" ").Append(vertexIndex2).Append("/").Append(vertexIndex2).Append("/").Append(vertexIndex2)
                   .Append(" ").Append(vertexIndex3).Append("/").Append(vertexIndex3).Append("/").Append(vertexIndex3)
                   .Append("\n");
        }

        return objData.ToString();
    }

}