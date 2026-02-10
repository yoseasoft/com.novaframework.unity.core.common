/// -------------------------------------------------------------------------------
/// Copyright (C) 2023, Guangzhou Shiyue Network Technology Co., Ltd.
/// Copyright (C) 2025, Hainan Yuanyou Information Technology Co., Ltd. Guangzhou Branch
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
using System.Linq;
using System.Reflection;
using NovaFramework.Editor.Manifest;
using UnityEngine;

namespace NovaFramework.Editor.Preference
{
    /// <summary>
    /// 模块安装调用器，通过反射调用所有ModuleInstallHandler和InstallationStep实现的Install方法
    /// </summary>
    public class AutoInstallInvoker
    {
        /// <summary>
        /// 调用所有实现了IModuleInstallHandler的类的Install方法
        /// </summary>
        public static void InvokeAllInstall()
        {
            var interfaceHandlers = GetModuleInstallHandlersFromInterface();
            
            // 调用接口实现的Install方法
            foreach (var handler in interfaceHandlers)
            {
                try
                {
                    Debug.Log($"正在执行模块安装 (接口): {handler.GetType().Name}");
                    handler.Install(() => {
                        Debug.Log($"模块安装完成 (接口): {handler.GetType().Name}");
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogError($"执行模块安装失败 {handler.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 调用所有实现了IModuleInstallHandler的类的Uninstall方法
        /// </summary>
        public static void InvokeAllUninstall()
        {
            var interfaceHandlers = GetModuleInstallHandlersFromInterface();
            
            // 调用接口实现的Uninstall方法
            foreach (var handler in interfaceHandlers)
            {
                try
                {
                    Debug.Log($"正在执行模块卸载 (接口): {handler.GetType().Name}");
                    handler.Uninstall(() => {
                        Debug.Log($"模块卸载完成 (接口): {handler.GetType().Name}");
                    });
                }
                catch (Exception ex)
                {
                    Debug.LogError($"执行模块卸载失败 {handler.GetType().Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 获取所有IModuleInstallHandler的实例
        /// </summary>
        /// <returns>IModuleInstallHandler实例列表</returns>
        private static List<InstallationStep> GetModuleInstallHandlersFromInterface()
        {
            var handlers = new List<InstallationStep>();
            
            try
            {
                // 加载仓库清单数据
                RepoManifest.Instance.LoadData();
                var packageObjects = RepoManifest.Instance.modules;
                
                if (packageObjects == null || packageObjects.Count == 0)
                {
                    Debug.LogWarning("未找到任何包配置信息");
                    return handlers;
                }
                
                // 按PID排序包对象
                var sortedPackages = packageObjects.OrderBy(p => p.pid).ToList();
                
                // 在自动安装阶段，只处理必需包(required=true)及其依赖
                var requiredPackageNames = new HashSet<string>();
                
                // 先添加所有必需包
                foreach (var package in sortedPackages)
                {
                    if (package.required)
                    {
                        requiredPackageNames.Add(package.name);
                    }
                }
                
                // 添加必需包的依赖
                foreach (var packageName in requiredPackageNames.ToList())
                {
                    var package = sortedPackages.FirstOrDefault(p => p.name == packageName);
                    if (package?.dependencies != null)
                    {
                        foreach (var dep in package.dependencies)
                        {
                            requiredPackageNames.Add(dep);
                        }
                    }
                }
                
                foreach (var package in sortedPackages)
                {
                    // 检查包是否为必需包或其依赖
                    if (!requiredPackageNames.Contains(package.name))
                    {
                        continue;
                    }
                    
                    // 检查包是否有安装配置
                    if (package.installationObject?.importModules == null)
                    {
                        continue;
                    }
                    
                    // 遍历该包的所有import-strategy配置
                    foreach (var importModule in package.installationObject.importModules)
                    {
                        // 检查installable属性是否为true
                        if (!importModule.installable)
                        {
                            continue;
                        }
                        
                        // 通过name获取程序集
                        string assemblyName = importModule.name;
                        if (string.IsNullOrEmpty(assemblyName))
                        {
                            continue;
                        }
                        
                        // 查找对应的程序集
                        var assembly = AppDomain.CurrentDomain.GetAssemblies()
                            .FirstOrDefault(a => a.GetName().Name == assemblyName);
                        
                        if (assembly == null)
                        {
                            Debug.LogWarning($"未找到程序集: {assemblyName}");
                            continue;
                        }
                        
                        try
                        {
                            // 查找该程序集中实现了InstallationStep的类型
                            var types = assembly.GetTypes()
                                .Where(x => typeof(InstallationStep).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
                            
                            foreach (var type in types)
                            {
                                var instance = Activator.CreateInstance(type) as InstallationStep;
                                if (instance != null)
                                {
                                    handlers.Add(instance);
                                    Debug.Log($"找到安装处理器: {type.FullName} (来自包: {package.name})");
                                }
                            }
                        }
                        catch (ReflectionTypeLoadException)
                        {
                            Debug.LogWarning($"无法加载程序集中的类型: {assemblyName}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"获取IModuleInstallHandler实例时出错: {ex.Message}");
            }
            
            return handlers;
        }
    }
}