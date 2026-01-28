/// -------------------------------------------------------------------------------
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
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NovaFramework.Editor
{
    /// <summary>
    /// 插件包的`Manifest`文件操作的辅助工具类，用于对该文件内部的插件包引用信息进行增删改查处理
    /// </summary>
    public static class PackageManifestUtils
    {
        /// <summary>
        /// 添加新的本地插件包的引用关系到`manifest.json`文件中
        /// </summary>
        /// <param name="packageName">包名称</param>
        /// <param name="packageUrl">Git仓库URL</param>
        public static void AddPackageToManifest(string packageName, string packageUrl)
        {
            try
            {
                // 获取manifest.json文件路径
                string manifestPath = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "Packages", "manifest.json");

                if (!File.Exists(manifestPath))
                {
                    Logger.Error($"未找到manifest.json文件: {manifestPath}");
                    return;
                }

                // 读取现有的manifest.json内容
                string jsonContent = File.ReadAllText(manifestPath);

                // 查找 "dependencies" 的位置
                string dependenciesMarker = "\"dependencies\"";
                int dependenciesIndex = jsonContent.IndexOf(dependenciesMarker);

                if (dependenciesIndex == -1)
                {
                    Logger.Error("未在manifest.json中找到dependencies部分");
                    return;
                }

                // 从dependencies标记后查找冒号和左大括号
                int colonIndex = jsonContent.IndexOf(":", dependenciesIndex);
                if (colonIndex == -1)
                {
                    Logger.Error("未在manifest.json中找到dependencies后的冒号");
                    return;
                }

                int openBraceIndex = jsonContent.IndexOf('{', colonIndex);
                if (openBraceIndex == -1)
                {
                    Logger.Error("未在manifest.json中找到dependencies后的左大括号");
                    return;
                }

                // 找到dependencies对象的开始位置（左大括号之后）
                int dependenciesObjStart = openBraceIndex + 1;

                // 提取dependencies对象部分
                int dependenciesObjEnd = FindMatchingBrace(jsonContent, openBraceIndex);
                if (dependenciesObjEnd == -1)
                {
                    Logger.Error("manifest.json中dependencies部分格式错误");
                    return;
                }

                string dependenciesStr = jsonContent.Substring(dependenciesObjStart, dependenciesObjEnd - dependenciesObjStart - 1).Trim(); // -1 to exclude the closing brace

                // 检查包是否已存在
                if (dependenciesStr.Contains($"\"{packageName}\":"))
                {
                    Logger.Info($"包 {packageName} 已存在于manifest.json中");
                    return;
                }

                // 准备新的dependencies内容
                string newDependenciesContent;
                if (dependenciesStr.Length > 0 && !string.IsNullOrWhiteSpace(dependenciesStr)) // 如果dependencies不为空
                {
                    // 移除尾随的空格和可能的逗号
                    dependenciesStr = dependenciesStr.TrimEnd(' ', '\t', '\n', '\r', ',');
                    newDependenciesContent = $"{dependenciesStr},\n    \"{packageName}\": \"{packageUrl}\"";
                }
                else // 如果dependencies为空
                {
                    newDependenciesContent = $"    \"{packageName}\": \"{packageUrl}\"";
                }

                // 替换原内容
                string newJsonContent = jsonContent.Substring(0, dependenciesObjStart) +
                                       newDependenciesContent +
                                       jsonContent.Substring(dependenciesObjEnd - 1); // -1 to include the closing brace

                File.WriteAllText(manifestPath, newJsonContent);

                Logger.Info($"成功添加包 {packageName} 到manifest.json: {packageUrl}");

                // 注释掉此处的Client.Resolve()调用，统一在所有包都添加完毕后调用
                // UnityEditor.PackageManager.Client.Resolve();
            }
            catch (Exception e)
            {
                Logger.Error($"添加包到manifest.json时出错: {e.Message}");
            }
        }

        /// <summary>
        /// 从`manifest.json`文件中移除指定的插件包
        /// </summary>
        /// <param name="packageName">包名称</param>
        public static void RemovePackageFromManifest(string packageName)
        {
            try
            {
                // 获取manifest.json文件路径
                string manifestPath = Path.Combine(Directory.GetParent(Application.dataPath).ToString(), "Packages", "manifest.json");

                if (!File.Exists(manifestPath))
                {
                    Logger.Error($"未找到manifest.json文件: {manifestPath}");
                    return;
                }

                // 读取现有的manifest.json内容
                string jsonContent = File.ReadAllText(manifestPath);

                // 查找 "dependencies" 的位置
                string dependenciesMarker = "\"dependencies\"";
                int dependenciesIndex = jsonContent.IndexOf(dependenciesMarker);

                if (dependenciesIndex == -1)
                {
                    Logger.Error("未在manifest.json中找到dependencies部分");
                    return;
                }

                // 从dependencies标记后查找冒号和左大括号
                int colonIndex = jsonContent.IndexOf(":", dependenciesIndex);
                if (colonIndex == -1)
                {
                    Logger.Error("未在manifest.json中找到dependencies后的冒号");
                    return;
                }

                int openBraceIndex = jsonContent.IndexOf('{', colonIndex);
                if (openBraceIndex == -1)
                {
                    Logger.Error("未在manifest.json中找到dependencies后的左大括号");
                    return;
                }

                // 找到dependencies对象的开始位置（左大括号之后）
                int dependenciesObjStart = openBraceIndex + 1;

                // 提取dependencies对象部分
                int dependenciesObjEnd = FindMatchingBrace(jsonContent, openBraceIndex);
                if (dependenciesObjEnd == -1)
                {
                    Logger.Error("manifest.json中dependencies部分格式错误");
                    return;
                }

                string dependenciesStr = jsonContent.Substring(dependenciesObjStart, dependenciesObjEnd - dependenciesObjStart - 1).Trim(); // -1 to exclude the closing brace

                // 检查包是否存在
                if (!dependenciesStr.Contains($"\"{packageName}\":"))
                {
                    Logger.Info($"包 {packageName} 不存在于manifest.json中");
                    return;
                }

                // 分割dependencies字符串为单独的行，然后逐个处理
                string[] lines = dependenciesStr.Split('\n');
                IList<string> newLines = new List<string>();
                bool foundPackage = false;

                foreach (string line in lines)
                {
                    if (line.Trim().StartsWith($"\"{packageName}\":"))
                    {
                        foundPackage = true; // 标记找到了要删除的包
                        continue; // 跳过这一行（相当于删除）
                    }

                    // 如果不是最后一行，保留逗号
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        newLines.Add(line);
                    }
                }

                // 如果没找到要删除的包，直接返回
                if (!foundPackage)
                {
                    Logger.Info($"包 {packageName} 不存在于manifest.json中");
                    return;
                }

                // 重新构建dependencies内容
                string newDependenciesStr = string.Join("\n", newLines);
                newDependenciesStr = newDependenciesStr.TrimEnd(',', ' ', '\t', '\n', '\r');

                // 替换原内容
                string newJsonContent = jsonContent.Substring(0, dependenciesObjStart) +
                                       newDependenciesStr +
                                       jsonContent.Substring(dependenciesObjEnd - 1); // -1 to include the closing brace

                File.WriteAllText(manifestPath, newJsonContent);

                Logger.Info($"成功从manifest.json移除包 {packageName}");

                // 注释掉此处的Client.Resolve()调用，统一在所有包都处理完毕后调用
                // UnityEditor.PackageManager.Client.Resolve();
            }
            catch (Exception e)
            {
                Logger.Error($"从manifest.json移除包时出错: {e.Message}");
            }
        }

        // 辅助方法：找到匹配的大括号
        private static int FindMatchingBrace(string json, int startIndex)
        {
            int braceCount = 0;
            char prevChar = '\0';
            bool isInString = false;

            for (int n = startIndex; n < json.Length; n++)
            {
                char c = json[n];

                // 检查是否在字符串内（需要考虑转义字符）
                if (c == '"' && prevChar != '\\')
                {
                    isInString = !isInString;
                }

                if (!isInString)
                {
                    if (c == '{')
                    {
                        braceCount++;
                    }
                    else if (c == '}')
                    {
                        braceCount--;
                        if (braceCount == 0)
                        {
                            return n + 1; // 返回匹配的右大括号之后的位置
                        }
                    }
                }

                prevChar = c;
            }

            return -1; // 未找到匹配的大括号
        }
    }
}
