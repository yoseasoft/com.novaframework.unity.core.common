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

namespace NovaFramework.Editor
{
    /// <summary>
    /// 环境上下文的配置类
    /// </summary>
    internal static class ContextSettings
    {
        /// <summary>
        /// 通用模块的本地包名
        /// </summary>
        public const string LocalPackageNameOfCommonModule = @"com.novaframework.unity.core.common";

        /// <summary>
        /// Nova框架基础文件夹的本地安装路径
        /// </summary>
        public const string LocalInstallPathOfNovaFrameworkDataFolder = @"Assets/../NovaFrameworkData/";
        /// <summary>
        /// Nova框架仓库文件夹的本地安装路径
        /// </summary>
        public const string LocalInstallPathOfNovaFrameworkRepositoryFolder = @"Assets/../NovaFrameworkData/framework_repo/";
    }
}
