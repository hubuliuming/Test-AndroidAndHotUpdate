/****************************************************
    文件：Hello.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using System.Collections;
using UnityEngine;

public class Hello
{
    public static void Run()
    {
        Debug.Log("Hello, HybridCLR");
        GameObject go = new GameObject("Test1");
        go.AddComponent<Print>();
    }
}