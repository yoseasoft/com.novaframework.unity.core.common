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

using UnityEngine;

namespace NovaFramework
{
    /// <summary>
    /// 程序日志接口类，用于在程序中提供日志输出接口函数
    /// </summary>
    public static class Logger
    {
        private delegate void ObjectMessageOutputCallback(object message);
        private delegate void StringMessageOutputCallback(string message);
        private delegate void FormatMessageOutputCallback(string format, params object[] args);

        private static ObjectMessageOutputCallback logObject;
        private static StringMessageOutputCallback logString;
        private static FormatMessageOutputCallback logFormat;

        /// <summary>
        /// 普通信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息对象</param>
        public static void Info(object message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// 普通信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// 普通信息输出日志接口函数
        /// </summary>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Info(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        /// <summary>
        /// 警告信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息对象</param>
        public static void Warn(object message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// 警告信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Warn(string message)
        {
            Debug.LogWarning(message);
        }

        /// <summary>
        /// 警告信息输出日志接口函数
        /// </summary>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Warn(string format, params object[] args)
        {
            Debug.LogWarningFormat(format, args);
        }

        /// <summary>
        /// 错误信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息对象</param>
        public static void Error(object message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// 错误信息输出日志接口函数
        /// </summary>
        /// <param name="message">信息内容</param>
        public static void Error(string message)
        {
            Debug.LogError(message);
        }

        /// <summary>
        /// 错误信息输出日志接口函数
        /// </summary>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Error(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        /// 断言调试接口函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        public static void Assert(bool condition)
        {
            Debug.Assert(condition);
        }

        /// <summary>
        /// 断言调试接口函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">信息对象</param>
        public static void Assert(bool condition, object message)
        {
            Debug.Assert(condition, message);
        }

        /// <summary>
        /// 断言调试接口函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="message">信息内容</param>
        public static void Assert(bool condition, string message)
        {
            Debug.Assert(condition, message);
        }

        /// <summary>
        /// 断言调试接口函数
        /// </summary>
        /// <param name="condition">断言条件</param>
        /// <param name="format">格式化文本</param>
        /// <param name="args">参数列表</param>
        public static void Assert(bool condition, string format, params object[] args)
        {
            Debug.AssertFormat(condition, format, args);
        }
    }
}
