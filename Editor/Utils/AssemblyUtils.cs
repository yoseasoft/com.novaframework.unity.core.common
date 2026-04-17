/// -------------------------------------------------------------------------------
/// NovaFramework By UnityEngine
///
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NovaFramework.Editor
{
    /// <summary>
    /// 程序集的辅助工具类，用于提供程序集的访问操作
    /// </summary>
    public static class AssemblyUtils
    {
        /// <summary>
        /// 通过名称查找程序集
        /// </summary>
        /// <param name="name">程序集名称</param>
        /// <returns>返回名称对应的程序集实例，若不存在则返回null</returns>
        static Assembly FindAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly =>
                assembly.GetName().Name == name);
        }

        /// <summary>
        /// 从指定名称的程序集中查找所有指定类型的对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="name">程序集名称</param>
        /// <param name="inherited">继承标识</param>
        /// <returns>返回查找到的所有对象类型列表</returns>
        public static IReadOnlyList<Type> FindAllTypesFromAssembly<T>(string name, bool inherited = false) where T : class
        {
            Assembly assembly = FindAssemblyByName(name);
            Logger.Assert(null != assembly, "当前检索的目标程序集‘{0}’不存在，查找其内部的对象类型失败！", name);

            List<Type> result = new List<Type>();

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                if (inherited)
                {
                    if (typeof(T).IsAssignableFrom(type))
                    {
                        result.Add(type);
                    }
                }
                else
                {
                    if (typeof(T) == type)
                    {
                        result.Add(type);
                    }
                }
            }

            return result;
        }
    }
}
