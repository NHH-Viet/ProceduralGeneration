using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;

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
    public delegate void ProgressHandler(float progress);
    public static event ProgressHandler OnProgress;
    /*public static string MeshToObj(Mesh mesh)
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
    }*/
    public static async Task SaveMeshAsync(Mesh mesh, string filePath, CancellationToken cancellationToken)
    {
        StringBuilder objData = new StringBuilder(mesh.vertices.Length * 100);
        objData.Append("g ExportedMesh\n");

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            // Check for cancellation request
            if (cancellationToken.IsCancellationRequested)
            {
                // Clean up any resources and throw an OperationCanceledException
                // or return early, depending on your requirements
                //CleanupResources();
                cancellationToken.ThrowIfCancellationRequested();
            }
            objData.Append("v ").Append(mesh.vertices[i].x).Append(" ").Append(mesh.vertices[i].y).Append(" ").Append(mesh.vertices[i].z).Append("\n");
            objData.Append("vn ").Append(mesh.normals[i].x).Append(" ").Append(mesh.normals[i].y).Append(" ").Append(mesh.normals[i].z).Append("\n");
            objData.Append("vt ").Append(mesh.uv[i].x).Append(" ").Append(mesh.uv[i].y).Append("\n");

            float progress = (float)(i + 1) / mesh.vertices.Length;
            OnProgress?.Invoke(progress); // Report progress
            await Task.Yield(); // Allow other tasks to execute
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

        await WriteTextToFileAsync(filePath, objData.ToString());
    }
    private static async Task WriteTextToFileAsync(string filePath, string text)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            await writer.WriteAsync(text);
        }
    }

    public static async Task iSaveMeshAsync(Mesh mesh, string filePath, CancellationToken cancellationToken)
    {
        StringBuilder objData = new StringBuilder(mesh.vertices.Length * 100);
        objData.Append("g ExportedMesh\n");

        const int batchSize = 1000; // Adjust the batch size as per your requirements

        for (int batchStart = 0; batchStart < mesh.vertices.Length; batchStart += batchSize)
        {
            int batchEnd = Mathf.Min(batchStart + batchSize, mesh.vertices.Length);

            for (int i = batchStart; i < batchEnd; i++)
            {
                // Check for cancellation request
                if (cancellationToken.IsCancellationRequested)
                {
                    // Clean up any resources and throw an OperationCanceledException
                    // or return early, depending on your requirements
                    //CleanupResources();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                // Process vertices and append to objData StringBuilder
                objData.Append("v ").Append(mesh.vertices[i].x).Append(" ").Append(mesh.vertices[i].y).Append(" ").Append(mesh.vertices[i].z).Append("\n");
                objData.Append("vn ").Append(mesh.normals[i].x).Append(" ").Append(mesh.normals[i].y).Append(" ").Append(mesh.normals[i].z).Append("\n");
                objData.Append("vt ").Append(mesh.uv[i].x).Append(" ").Append(mesh.uv[i].y).Append("\n");

                float progress = (float)(i + 1) / mesh.vertices.Length;
                OnProgress?.Invoke(progress); // Report progress
                await Task.Yield(); // Allow other tasks to execute
            }

            // Append the processed batch to the file
            await AppendTextToFileAsync(filePath, objData.ToString());

            // Clear the StringBuilder for the next batch
            objData.Clear();
        }
    }

    private static async Task AppendTextToFileAsync(string filePath, string text)
    {
        using (StreamWriter writer = new StreamWriter(filePath, true)) // Open the file in append mode
        {
            await writer.WriteAsync(text);
        }
    }

}