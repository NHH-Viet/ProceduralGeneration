using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size)
    {
        //tạo mảng 2D
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //i và j tượng trưng cho tọa độ của falloff map, và giờ ta gán giá trị nó khoảng từ -1 đến 1
                float x = i/(float)size*2-1;
                float y = j/(float)size*2-1;
                //kiểm tra điểm nào gần viền hơn
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i,j] = Evaluate(value);
            }
        }
        return map;
    }

    //Hàm thay đổi độ dày từ trung tâm ra
    static float Evaluate(float value){
        float a = 3;
        float b = 2.2f;
        // công thức f(x) = x mũ a / x mũ a + (b-bx)mũ a
        return Mathf.Pow(value,a)/(Mathf.Pow(value,a)+ Mathf.Pow(b-b*value,a));
    }
}
