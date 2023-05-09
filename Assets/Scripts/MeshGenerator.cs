using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator 
{

    //Hàm tạo mesh (đang nghiên cứu thêm)

    //heightMultiplier để điều chính độ cao
    //heightCurve để điều chỉnh biểu đồ độ cao của từng heightMap[x, y] (ví dụ để hiểu rõ)
    //levelOfDetail độ chi tiết
    public static MeshData GenerateTerrainMesh(float [,] heightMap,MeshSettings meshSettings, /*float heightMultiplier, AnimationCurve heightCurve*/ int levelOfDetail/*bool useFlatShading*/){
        //AnimationCurve heightCurveS = new AnimationCurve (heightCurve.keys); // biến cho endlessmode (nghiên cứu bỏ sau)
        int width = heightMap.GetLength (0);
        int height = heightMap.GetLength (1);
        //Top left để xử lý cho mesh render ở trung tâm màn hình
        //   X  X  X
        //  -1  0  1
        // float topLeftX = (width - 1) / -2f;
        // float topLeftZ = (height - 1) / -2f;
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1)/ 2f;
        //biến điều chỉnh độ chi tiết
        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
        // tính số vertices mỗi dòng
        int verticesPerline = (width-1) / meshSimplificationIncrement + 1;   
        //khởi tạo một struct lưu các dữ liệu của mesh
        MeshData meshData = new MeshData (verticesPerline, verticesPerline,meshSettings.useFlatShading); // struct lưu meshData ở dưới
        int vertexIndex = 0; // lưu lại vị trí của vertex
        //Vòng lặp qua các điểm heightmap
        for (int y = 0; y < height; y+= meshSimplificationIncrement){
            for(int x = 0 ; x < width; x+=meshSimplificationIncrement ){
                //Gán vertices ở vị trí vertexIndex một vector3 có tham số x là x (của vòng lặp), y (ảnh hưởng độ cao) là heightmap vị trí x,y(của vòng lặp) và z là y (của vòng lặp)
                //Nếu ta nhân tọa độ y của vector với heightMultiplier thì dẫn tới sự thay đổi độ nhô của mesh
                //HeightCurveS để xác định xem độ ảnh hưởng của heightMultiplier so với từng heightMap[x, y]
                meshData.vertices[vertexIndex] = new Vector3((topLeftX+x) * meshSettings.scale, heightMap[x, y], (topLeftZ - y) * meshSettings.scale);
                meshData.uvs[vertexIndex] = new Vector2(x/(float)width,y/(float)height);
                if(x <width - 1 && y < height - 1){
                    // 0 1 2
                    // 3 4 5
                    // 6 7 8
                    //chỉ xét 0 1 3 4 không cần xét 2 5 6 7 8

                    //  i   i+1
                    //
                    // i+w  i+w+1
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerline + 1, vertexIndex +verticesPerline);
                    meshData.AddTriangle(vertexIndex +verticesPerline + 1, vertexIndex, vertexIndex + 1); // sao lai tinh hai lan
                }
                vertexIndex++;
            }
        }
        meshData.Finialize();
        return meshData;
    }
}
// struct lưu các giá trị của mesh
public class MeshData {
    public Vector3[] vertices; // mảng lưu các vertices (3 chiều vì là 3D (x,y,z))
    public int[] triangles; // mảng lưu các triangles

    
    public Vector2[] uvs; // mảng lưu tọa độ uv để gán texture lên mesh để cho các vertex biết là nó đang ở đâu so với các vị trí còn lại trên bản đồ theo tỉ lệ trục x và y (từ 0 đến 1)
    int triangleIndex; // thứ tự của mảng triangles

    Vector3[] borderVertices;
    int[] borderTriangles;
    int borderTriangleIndex;
    bool useFlatShading;
    // normalize
    Vector3[] bakedNormals;

    public MeshData(int meshWidth, int meshHeight, bool useFlatShading){
        this.useFlatShading = useFlatShading;
        vertices = new Vector3[meshWidth*meshHeight];
        uvs = new Vector2[meshWidth* meshHeight];
        triangles = new int[(meshWidth-1)*(meshHeight-1)*6];
    }
    //Hàm gán các vị trí của Vertices vào mảng triangles
    public void AddTriangle(int a, int b, int c){
        triangles[triangleIndex] = a;
        triangles[triangleIndex+1] = b;
        triangles[triangleIndex+2] = c;
        triangleIndex += 3;
    }

    Vector3[] CalulateNormals() {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for(int i = 0 ; i < triangleCount; i ++){
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex+1];
            int vertexIndexC = triangles[normalTriangleIndex+2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA,vertexIndexB,vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        
        }

        for(int i = 0; i<vertexNormals.Length; i++){
            vertexNormals [i].Normalize();
        }
        return vertexNormals;
    }
    //tính trọng tâm tam giác
    Vector3 SurfaceNormalFromIndices(int a, int b, int c){
        Vector3 pA = vertices[a];
        Vector3 pB = vertices[b];
        Vector3 pC = vertices[c];

        Vector3 sideAB = pB - pA;
        Vector3 sideAC = pC - pA;
        return Vector3.Cross(sideAB,sideAC).normalized;
    }
    public void Finialize(){
        if(useFlatShading){
            FlatShading();
        }else{
            BakedNormals();
        }
    }
    void BakedNormals(){
        bakedNormals = CalulateNormals();
    }
    void FlatShading (){
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShidesUVs = new Vector2[triangles.Length];
        for( int i = 0; i < triangles.Length; i ++){
            flatShadedVertices [i] = vertices[triangles[i]];
            flatShidesUVs [i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShidesUVs;
    }

    // Hàm tạo một đối tượng mesh từ các tham số ta đã xử lí ở trên
    public Mesh CreateMesh() {
        Mesh mesh = new Mesh(); // khai báo đối tượng mesh của Unity
        mesh.vertices = vertices; // gán Vertices đã tính toán
        mesh.triangles = triangles;// gán triangles
        mesh.uv = uvs; // gán uv

        //Phần xử lý shading
        if(useFlatShading){
            mesh.RecalculateNormals();
        } else{
            mesh.normals = bakedNormals;
        }
        //-----
        //mesh.RecalculateNormals();
        return mesh;
    }
    //------
}