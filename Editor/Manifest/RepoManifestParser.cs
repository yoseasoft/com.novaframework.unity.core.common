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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace NovaFramework.Editor.Manifest
{
    /// <summary>
    /// 解析器对象类，专用于对仓库资源配置清单数据的解析及对象实例的构建
    /// </summary>
    static class RepoManifestParser
    {
        private const string ElementName_EnvironmentVariable = @"environment-variable";
        private const string ElementName_SystemPath = @"system-path";
        private const string ElementName_LocalPath = @"local-path";
        private const string ElementName_Package = @"package";
        private const string ElementName_GitRepository = @"git-repository";
        private const string ElementName_AssemblyDefinition = @"assembly-definition";
        private const string ElementName_LoadableStrategy = @"loadable-strategy";
        private const string ElementName_AssetSources = @"asset-sources";
        private const string ElementName_Dependencies = @"dependencies";
        private const string ElementName_Repulsions = @"repulsions";
        private const string ElementName_ReferencePackage = @"reference-package";

        private const string AttributeName_Pid = @"pid";
        private const string AttributeName_Name = @"name";
        private const string AttributeName_DisplayName = @"displayName";
        private const string AttributeName_Value = @"value";
        private const string AttributeName_DefaultValue = @"defaultValue";
        private const string AttributeName_Title = @"title";
        private const string AttributeName_Description = @"description";
        private const string AttributeName_Required = @"required";
        private const string AttributeName_Order = @"order";
        private const string AttributeName_Url = @"url";

        /// <summary>
        /// 对指定地址的文本资源中数据进行解析，并填充到清单对象中
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="manifest">清单对象实例</param>
        /// <returns>若解析数据成功返回true，否则返回false</returns>
        public static bool Parse(string url, RepoManifest manifest)
        {
            // 首先读取XML内容并替换变量
            string text = File.ReadAllText(url);
            if (string.IsNullOrEmpty(text))
            {
                return false;
            }

            XmlDocument document = new XmlDocument();
            document.LoadXml(text);

            XmlElement root = document.DocumentElement;
            XmlNodeList nodeList = root.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType)
                {
                    Logger.Info("目标节点为非法类型‘{0}’，无法正确解析其内部数据‘{1}’！", child.NodeType.ToString(), child.Value);
                    continue;
                }

                switch (child.Name)
                {
                    case ElementName_EnvironmentVariable:
                        if (!ParseTheNodeNamedEnvironmentVariable(child, manifest.variables)) return false;
                        break;
                    case ElementName_SystemPath:
                        if (!ParseTheNodeNamedSystemPath(child, manifest)) return false;
                        break;
                    case ElementName_Package:
                        if (!ParseTheNodeNamedPackage(child, manifest.modules)) return false;
                        break;
                    default:
                        Logger.Error("无法正确识别节点名称‘{0}’，解析该节点数据失败！", child.Name);
                        return false;
                }
            }

            // 解析完成后进行变量替换
            ReplaceVariables(manifest);

            return true;
        }

        static bool ParseTheNodeNamedEnvironmentVariable(XmlNode node, IList<VariableObject> list)
        {
            string name = GetXmlAttribute(node, AttributeName_Name);
            string value = GetXmlAttribute(node, AttributeName_Value);

            list.Add(new VariableObject()
            {
                key = name,
                value = value,
            });

            return true;
        }

        static bool ParseTheNodeNamedSystemPath(XmlNode node, RepoManifest manifest)
        {
            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType || !child.Name.Equals(ElementName_LocalPath))
                {
                    Logger.Info("目标节点的类型‘{0}’或名称‘{1}’为非法格式，解析该节点数据失败！", child.NodeType.ToString(), child.Name);
                    return false;
                }

                if (!ParseTheNodeNamedLocalPath(child, manifest.localPaths)) return false;
            }

            return true;
        }

        static bool ParseTheNodeNamedLocalPath(XmlNode node, IList<LocalPathObject> list)
        {
            string name = GetXmlAttribute(node, AttributeName_Name);
            string defaultValue = GetXmlAttribute(node, AttributeName_DefaultValue);
            string title = GetXmlAttribute(node, AttributeName_Title);
            bool required = GetXmlAttributeAsBool(node, AttributeName_Required);

            list.Add(new LocalPathObject()
            {
                name = name,
                defaultValue = defaultValue,
                title = title,
                required = required,
            });

            return true;
        }

        static bool ParseTheNodeNamedPackage(XmlNode node, IList<PackageObject> list)
        {
            PackageObject packageObject = new PackageObject();

            packageObject.pid = GetXmlAttributeAsInt(node, AttributeName_Pid);
            packageObject.name = GetXmlAttribute(node, AttributeName_Name);
            packageObject.displayName = GetXmlAttribute(node, AttributeName_DisplayName);
            packageObject.title = GetXmlAttribute(node, AttributeName_Title);
            packageObject.description = GetXmlAttribute(node, AttributeName_Description);
            packageObject.required = GetXmlAttributeAsBool(node, AttributeName_Required);

            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType)
                {
                    Logger.Info("目标节点为非法类型‘{0}’，解析模块配置数据失败！", child.NodeType.ToString());
                    continue;
                }

                switch (child.Name)
                {
                    case ElementName_GitRepository:
                        if (!ParseTheNodeNamedGitRepositoryOfPackage(child, packageObject)) return false;
                        break;
                    case ElementName_AssemblyDefinition:
                        if (!ParseTheNodeNamedAssemblyDefinitionOfPackage(child, packageObject)) return false;
                        break;
                    case ElementName_AssetSources:
                        if (!ParseTheNodeNamedAssetSourcesOfPackage(child, packageObject)) return false;
                        break;
                    case ElementName_Dependencies:
                        if (!ParseTheNodeNamedDependenciesOfPackage(child, packageObject)) return false;
                        break;
                    case ElementName_Repulsions:
                        if (!ParseTheNodeNamedRepulsionsOfPackage(child, packageObject)) return false;
                        break;
                    default:
                        Logger.Error("无法正确识别节点名称‘{0}’，解析该节点数据失败！", child.Name);
                        return false;
                }
            }

            list.Add(packageObject);

            return true;
        }

        static bool ParseTheNodeNamedGitRepositoryOfPackage(XmlNode node, PackageObject packageObject)
        {
            packageObject.gitRepositoryUrl = GetXmlAttribute(node, AttributeName_Url);

            return true;
        }

        static bool ParseTheNodeNamedAssemblyDefinitionOfPackage(XmlNode node, PackageObject packageObject)
        {
            AssemblyDefinitionObject assemblyDefinitionObject = new AssemblyDefinitionObject();

            assemblyDefinitionObject.name = GetXmlAttribute(node, AttributeName_Name);
            assemblyDefinitionObject.order = GetXmlAttributeAsInt(node, AttributeName_Order);

            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType || !child.Name.Equals(ElementName_LoadableStrategy))
                {
                    Logger.Info("目标节点的类型‘{0}’或名称‘{1}’为非法格式，解析该节点数据失败！", child.NodeType.ToString(), child.Name);
                    return false;
                }

                string innerTextValue = child.InnerText.Trim();
                if (!string.IsNullOrEmpty(innerTextValue))
                {
                    assemblyDefinitionObject.tags.Add(innerTextValue);
                }
            }

            packageObject.assemblyDefinitionObject = assemblyDefinitionObject;

            return true;
        }

        static bool ParseTheNodeNamedAssetSourcesOfPackage(XmlNode node, PackageObject packageObject)
        {
            AssetSourceObject assetSourceObject = new AssetSourceObject();

            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType)
                {
                    Logger.Info("目标节点为非法类型‘{0}’，解析资产源配置数据失败！", child.NodeType.ToString());
                    continue;
                }

                switch (child.Name)
                {
                    case ElementName_LocalPath:
                        if (!ParseTheNodeNamedLocalPath(child, assetSourceObject.localPaths)) return false;
                        break;
                    default:
                        Logger.Error("无法正确识别节点名称‘{0}’，解析该节点数据失败！", child.Name);
                        return false;
                }
            }

            packageObject.assetSourceObject = assetSourceObject;

            return true;
        }

        static bool ParseTheNodeNamedDependenciesOfPackage(XmlNode node, PackageObject packageObject)
        {
            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType)
                {
                    Logger.Info("目标节点为非法类型‘{0}’，解析模块依赖配置数据失败！", child.NodeType.ToString());
                    continue;
                }

                switch (child.Name)
                {
                    case ElementName_ReferencePackage:
                        string innerTextValue = child.InnerText.Trim();
                        if (!string.IsNullOrEmpty(innerTextValue))
                        {
                            packageObject.dependencies.Add(innerTextValue);
                        }
                        break;
                    default:
                        Logger.Error("无法正确识别节点名称‘{0}’，解析该节点数据失败！", child.Name);
                        return false;
                }
            }

            return true;
        }

        static bool ParseTheNodeNamedRepulsionsOfPackage(XmlNode node, PackageObject packageObject)
        {
            XmlNodeList nodeList = node.ChildNodes;
            for (int n = 0; null != nodeList && n < nodeList.Count; ++n)
            {
                XmlNode child = nodeList[n];

                if (XmlNodeType.Element != child.NodeType)
                {
                    Logger.Info("目标节点为非法类型‘{0}’，解析模块排斥配置数据失败！", child.NodeType.ToString());
                    continue;
                }

                switch (child.Name)
                {
                    case ElementName_ReferencePackage:
                        string innerTextValue = child.InnerText.Trim();
                        if (!string.IsNullOrEmpty(innerTextValue))
                        {
                            packageObject.repulsions.Add(innerTextValue);
                        }
                        break;
                    default:
                        Logger.Error("无法正确识别节点名称‘{0}’，解析该节点数据失败！", child.Name);
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 对清单对象中的所有字符串字段进行变量替换
        /// </summary>
        /// <param name="manifest">清单对象实例</param>
        public static void ReplaceVariables(RepoManifest manifest)
        {
            if (manifest?.variables == null || manifest.variables.Count == 0)
                return;

            // 构建变量字典
            Dictionary<string, string> variableDict = new Dictionary<string, string>();
            foreach (var variable in manifest.variables)
            {
                if (!string.IsNullOrEmpty(variable.key) && variable.value != null)
                {
                    variableDict[variable.key] = variable.value;
                }
            }

            if (variableDict.Count == 0)
                return;

            // 替换本地路径中的默认值
            if (manifest.localPaths != null)
            {
                foreach (var localPath in manifest.localPaths)
                {
                    localPath.defaultValue = ReplaceVariablesInString(localPath.defaultValue, variableDict);
                    localPath.title = ReplaceVariablesInString(localPath.title, variableDict);
                }
            }

            // 替换模块中的相关字段
            if (manifest.modules != null)
            {
                foreach (var module in manifest.modules)
                {
                    module.name = ReplaceVariablesInString(module.name, variableDict);
                    module.displayName = ReplaceVariablesInString(module.displayName, variableDict);
                    module.title = ReplaceVariablesInString(module.title, variableDict);
                    module.description = ReplaceVariablesInString(module.description, variableDict);
                    module.gitRepositoryUrl = ReplaceVariablesInString(module.gitRepositoryUrl, variableDict);

                    // 替换资产源中的本地路径
                    if (module.assetSourceObject?.localPaths != null)
                    {
                        foreach (var localPath in module.assetSourceObject.localPaths)
                        {
                            localPath.defaultValue = ReplaceVariablesInString(localPath.defaultValue, variableDict);
                            localPath.title = ReplaceVariablesInString(localPath.title, variableDict);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 在字符串中替换变量占位符
        /// </summary>
        /// <param name="input">输入字符串</param>
        /// <param name="variables">变量字典</param>
        /// <returns>替换后的字符串</returns>
        private static string ReplaceVariablesInString(string input, Dictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(input) || variables == null || variables.Count == 0)
                return input;

            string result = input;
            foreach (var kvp in variables)
            {
                string placeholder = "%" + kvp.Key + "%";
                result = result.Replace(placeholder, kvp.Value);
            }

            return result;
        }

        /// <summary>
        /// 获取XML属性值，如果不存在则返回空字符串
        /// </summary>
        /// <param name="node">XML节点</param>
        /// <param name="attributeName">属性名</param>
        /// <returns>返回属性值或空字符串</returns>
        private static string GetXmlAttribute(XmlNode node, string attributeName)
        {
            return node.Attributes?[attributeName]?.Value ?? string.Empty;
        }

        /// <summary>
        /// 获取XML属性值并转换为布尔值
        /// </summary>
        /// <param name="node">XML节点</param>
        /// <param name="attributeName">属性名</param>
        /// <returns>返回布尔值</returns>
        private static bool GetXmlAttributeAsBool(XmlNode node, string attributeName)
        {
            string attrValue = GetXmlAttribute(node, attributeName);
            return string.Equals(attrValue, "true", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 获取XML属性值并转换为整型值
        /// </summary>
        /// <param name="node">XML节点</param>
        /// <param name="attributeName">属性名</param>
        /// <returns>返回整型值</returns>
        private static int GetXmlAttributeAsInt(XmlNode node, string attributeName)
        {
            string attrValue = GetXmlAttribute(node, attributeName);
            return int.Parse(attrValue);
        }
    }
}
