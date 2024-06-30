/****************************************************
    文件：GameObjectExtension.cs
    作者：Y
    邮箱: 916111418@qq.com
    日期：#CreateTime#
    功能：Nothing
*****************************************************/

using UnityEngine;

namespace YFramework.Extension
{
    public static class GameObjectExtension  
    {
        public static void ReSetShader(this GameObject obj)
        {
            var renderer = obj.GetComponent<Renderer>();
            renderer.material.shader = Shader.Find(renderer.material.shader.name);
        }
    }
}