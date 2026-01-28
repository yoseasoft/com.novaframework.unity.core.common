/// -------------------------------------------------------------------------------
/// Copyright (C) 2024 - 2025, Hurley, Independent Studio.
/// Copyright (C) 2025 - 2026, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
///
/// Permission is hereby granted, free of charge, to any person obtaining a copy
/// of this software and associated documentation files (the "Software"), to deal
/// in the Software without restriction, including without limitation the rights
/// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
/// copies of the Software, and to permit persons to whom the Software is
/// furnished to do so, subject to the following conditions:
///
/// The above copyright notice and this permission notice shall be included in
/// all copies or substantial portions of the Software.
///
/// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
/// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
/// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
/// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
/// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
/// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
/// THE SOFTWARE.
/// -------------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace NovaFramework.Editor
{
    /// <summary>
    /// 资源数据库辅助工具类，用于提供本地资产编辑的功能接口
    /// </summary>
    public static class AssetDatabaseUtils
    {
        /// <summary>
        /// 创建脚本对象资产数据
        /// </summary>
        /// <typeparam name="T">脚本对象类型</typeparam>
        /// <param name="path">资产存放路径</param>
        /// <param name="initCallback">初始化回调句柄</param>
        public static void CreateScriptableObjectAsset<T>(string path, Action<T> initCallback = null) where T : ScriptableObject
        {
            T scriptableObject = ScriptableObject.CreateInstance<T>();
            initCallback?.Invoke(scriptableObject);

            // 创建并保存资产
            AssetDatabase.CreateAsset(scriptableObject, path);
            // 保存所有资产的改动到磁盘上
            AssetDatabase.SaveAssets();
            // 刷新资源视图，使新创建的资产立即可见
            AssetDatabase.Refresh();
        }
    }
}
