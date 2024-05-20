/****************************************************
    文件：Print.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Print : MonoBehaviour
{
    void Start()
    {
        Debug.Log($"[Print] GameObject:{name}");   
        Debug.Log("hhh");
        //Debug.Log("newnewnew");
        
        Debug.Log("heollniahaoahao");
    }
}


// AOT 中已经用过List<float>泛型
class Foo
{
    public void Run()
    {
        var arr = new List<float>();
    }
}

// 热更新中可以使用 List<float>
class HotUpdateGenericDemos
{
    public void Run()
    {
        var arr = new List<float>();
    }
}
