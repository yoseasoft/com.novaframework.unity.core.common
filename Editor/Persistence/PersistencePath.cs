/// -------------------------------------------------------------------------------
/// NovaFramework By UnityEngine
///
/// Copyright (C) 2025, Hurley, Independent Studio.
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

using UnityEditor.Compilation;

using Assembly = System.Reflection.Assembly;

namespace NovaFramework.Editor
{
    /// <summary>
    /// 持久化数据的路径管理辅助工具类
    /// </summary>
    public static class PersistencePath
    {
        /// <summary>
        /// 通用模块的本地包名
        /// </summary>
        public const string LocalPackageNameOfCommonModule = @"com.novaframework.unity.core.common";

        /// <summary>
        /// 通用模块的本地包名
        /// </summary>
        public const string CommonEditorAssemblyName = @"NovaEditor.Common";
        
        /// <summary>
        /// Nova框架基础文件夹的本地安装路径
        /// </summary>
        public const string LocalInstallPathOfNovaFrameworkDataFolder = @"Assets/../NovaFrameworkData/";
        /// <summary>
        /// Nova框架仓库文件夹的本地安装路径
        /// </summary>
        public const string LocalInstallPathOfNovaFrameworkRepositoryFolder = @"Assets/../NovaFrameworkData/framework_repo/";
        
        /// <summary>
        /// 获取指定程序集返回的模块文件夹路径
        /// </summary>
        /// <param name="module">模块名称</param>
        /// <returns>返回模块的当前文件路径</returns>
        public static string FindCurrentModuleFolder(string assemblyName)
        {
            string asmdefPath = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            if (string.IsNullOrEmpty(asmdefPath))
            {
                Logger.Warn($"程序集 {assemblyName} 的asmdef路径为空");
                return null;
            }

            string moduleFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(asmdefPath));
            if (!asmdefPath.StartsWith("Assets/"))
            {
                //默认存放在LocalInstallPathOfNovaFrameworkRepositoryFolder
                moduleFolderPath = Path.GetDirectoryName(Path.GetDirectoryName(LocalInstallPathOfNovaFrameworkRepositoryFolder + asmdefPath.Substring("Packages".Length + 1))).Replace("\\","/");
            }

            return moduleFolderPath;
        }
        
        /// <summary>
        /// 查找工程中程序集是否存在，找到一个符合的就返回true，否则返回false
        /// </summary>
        /// <param name="module">模块名称</param>
        /// <returns>返回有效的模块文件夹路径</returns>
        public static bool IsModuleAssemblyExists(string module)
        {
            return GetAssembliesFromModule(module).Count > 0;
        }

        public static List<Assembly> GetAssembliesFromModule(string module)
        {
            Manifest.PackageObject packageObject = Manifest.RepoManifest.Instance.modules.Find(m => m.name == module);
            if (null == packageObject?.outputAssembliesObject || 0 == packageObject.outputAssembliesObject.localAssemblies.Count)
            {
                Logger.Warn($"模块 {module} 没有outputAssembliesObject");
                return new List<Assembly>();
            }

            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            List<Assembly> result = new List<Assembly>();

            foreach (Manifest.ImportModuleObject localAssembly in packageObject.outputAssembliesObject.localAssemblies)
            {
                for (int n = 0; n < allAssemblies.Length; ++n)
                {
                    if (allAssemblies[n].GetName().Name == localAssembly.name)
                    {
                        result.Add(allAssemblies[n]);
                        break;
                    }
                }
            }

            return result;
        }
        
        /// <summary>
        /// 根据模块包名获取其在工程中的文件夹路径
        /// </summary>
        /// <param name="module">模块包名</param>
        /// <returns>返回模块文件夹路径，未找到则返回null</returns>
        public static string FindModuleFolderByPackageName(string module)
        {
            Manifest.PackageObject packageObject = Manifest.RepoManifest.Instance.modules.Find(m => m.name == module);
            if (null == packageObject?.outputAssembliesObject || 0 == packageObject.outputAssembliesObject.localAssemblies.Count)
            {
                return null;
            }

            return FindCurrentModuleFolder(packageObject.outputAssembliesObject.localAssemblies[0].name);
        }

        /// <summary>
        /// 仓库资源清单文件的绝对路径
        /// </summary>
        internal static readonly string AbsolutePathOfRepositoryManifestFile = Path.Combine(FindCurrentModuleFolder(CommonEditorAssemblyName), "Editor Default Resources/Config/repo_manifest.xml").Replace("\\", "/");
    }
}
