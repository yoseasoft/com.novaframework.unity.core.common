/// -------------------------------------------------------------------------------
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

using System.Collections.Generic;

namespace NovaFramework.Editor.Manifest
{
    /// <summary>
    /// 仓库资源配置清单管理对象类
    /// </summary>
    public sealed class RepoManifest : Singleton<RepoManifest>
    {
        public List<VariableObject> variables;
        public List<LocalPathObject> localPaths;
        public List<PackageObject> modules;

        /// <summary>
        /// 对象类初始化回调接口
        /// </summary>
        protected override void OnInitialize()
        {
            variables = new List<VariableObject>();
            localPaths = new List<LocalPathObject>();
            modules = new List<PackageObject>();

            string url = PersistencePath.AbsolutePathOfRepositoryManifestFile;
            if (!RepoManifestParser.Parse(url, this))
            {
                Logger.Error("仓库资源配置清单解析失败，请检测目标文件‘{0}’格式是否正确后再重新加载数据！", url);
                Clear();
                return;
            }
        }

        internal void LoadData()
        {
            Clear();
            
            string url = PersistencePath.AbsolutePathOfRepositoryManifestFile;
            if (!RepoManifestParser.Parse(url, this))
            {
                Logger.Error("仓库资源配置清单解析失败，请检测目标文件‘{0}’格式是否正确后再重新加载数据！", url);
                Clear();
            }
        } 
        
        /// <summary>
        /// 对象类清理回调接口
        /// </summary>
        protected override void OnCleanup()
        {
            Clear();

            variables = null;
            localPaths = null;
            modules = null;
        }

        /// <summary>
        /// 清理清单所有数据
        /// </summary>
        internal void Clear()
        {
            variables.Clear();
            localPaths.Clear();
            modules.Clear();
        }
    }
}
