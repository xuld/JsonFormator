/*************************************************************************
 *
 * Copyright (c) 2009-2014 Xuld. All rights reserved.
 * 
 * Project Url: http://work.xuld.net/circus
 * 
 * This source code is part of the Project Circus.
 * 
 * This code is licensed under The Circus License.
 * See the file License.html for the license details.
 * 
 * 
 * You must not remove this notice, or any other, from this software.
 *
 * 
 *************************************************************************/




using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace Circus.Json {

    #region JSON => 对象

    /// <summary>
    /// 表示一个 JSON 读取器。
    /// </summary>
    public sealed class JsonReader : IDisposable {

        #region 属性

        /// <summary>
        /// 获取当前的输入源。
        /// </summary>
        public readonly TextReader input;

        /// <summary>
        /// 获取或设置当前行号。
        /// </summary>
        public int currentEndLine;

        /// <summary>
        /// 获取或设置当前列号。
        /// </summary>
        public int currentEndColumn;

        /// <summary>
        /// 判断或设置是否启用严格模式。
        /// </summary>
        /// <remarks>
        /// 严格模式下将完全遵循 JSON 语法规范，如属性名必须使用双引号包围。
        /// </remarks>
        public bool strictMode;

        /// <summary>
        /// 初始化 <see cref="JsonReader"/> 类的新实例。
        /// </summary>
        /// <param name="input">JSON 源码的输入源。</param>
        public JsonReader(TextReader input) {
            this.input = input;
            currentEndLine = currentEndColumn = 1;
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            input.Dispose();
        }

        private string getCurrentToken() {
            switch (currentTokenType) {
                case JsonTokenType.objectStart:
                    return "{";
                case JsonTokenType.objectEnd:
                    return "}";
                case JsonTokenType.arrayEnd:
                    return "]";
                case JsonTokenType.arrayStart:
                    return "[";
                case JsonTokenType.colon:
                    return ":";
                case JsonTokenType.comma:
                    return ",";
                case JsonTokenType.hexLiteral:
                    return "0x" + currentBuffer.ToString();
                case JsonTokenType.stringLiteral:
                    return Json.stringify(currentBuffer.ToString());
                case JsonTokenType.eof:
                    return "文件尾";
                default:
                    return currentBuffer.ToString();
            }
        }

        /// <summary>
        /// 触发解析发生错误的异常。
        /// </summary>
        /// <param name="message">错误相关信息。</param>
        private void reportError(string message) {
            throw new JsonParseException(message, currentStartLine, currentStartColumn, currentEndLine, currentEndColumn);
        }

        #endregion

        #region 词法分析

        /// <summary>
        /// 指示当前已读取新行。
        /// </summary>
        private void newLine() {
            currentEndLine++;
            currentEndColumn = 1;
        }

        /// <summary>
        /// 存储当前标记相关数据的缓存。
        /// </summary>
        public StringBuilder currentBuffer = new StringBuilder();

        /// <summary>
        /// 获取当前标记的开始行号。
        /// </summary>
        public int currentStartLine;

        /// <summary>
        /// 获取当前标记的开始列号。
        /// </summary>
        public int currentStartColumn;

        /// <summary>
        /// 扫描下一个标记。
        /// </summary>
        private JsonTokenType scan() {

        skip:

            // 更新当前位置。
            currentStartLine = currentEndLine;
            currentStartColumn = currentEndColumn;

            int c = input.Read();
            currentEndColumn++;
            switch (c) {

                #region 空白字符

                case ' ':
                case '\t':
                    goto skip;
                case '\r':
                    if (input.Peek() == '\n') {
                        input.Read();
                    }
                    goto case '\n';
                case '\n':
                    newLine();
                    goto skip;

                #endregion

                #region 字符

                case ',':
                    return JsonTokenType.comma;
                case ':':
                    return JsonTokenType.colon;
                case '{':
                    return JsonTokenType.objectStart;
                case '[':
                    return JsonTokenType.arrayStart;
                case '}':
                    return JsonTokenType.objectEnd;
                case ']':
                    return JsonTokenType.arrayEnd;
                //case '(':
                //    return JsonTokenType.leftParam;
                //case ')':
                //    return JsonTokenType.rightParam;

                #endregion

                #region 数字和字符串

                case '-':
                    if (input.Peek() == 'I') {
                        readIdentifier(c);
                        if (!hasRead("-Infinity")) {
                            reportError(String.Format("无效的数字；应输入“-Infinity”；实际为“{0}”", currentBuffer.ToString()));
                            currentBuffer.Length = 0;
                            currentBuffer.Append("-Infinity");
                        }
                        return JsonTokenType.negInfinity;
                    }
                    return readNumber(c);
                case '.':
                    if (strictMode) {
                        break;
                    }
                    return readNumber(c);

                case '\"':
                    readString(c);
                    return JsonTokenType.stringLiteral;
                case '\'':
                    if (strictMode) {
                        break;
                    }
                    goto case '\"';

                #endregion

                #region 特殊符号

                case '/':
                    if (strictMode) {
                        break;
                    }
                    switch (input.Peek()) {
                        case '/':
                            input.Read(); // 读取 /
                            currentEndColumn++;
                            skipSingelineComment();
                            goto skip;
                        case '*':
                            input.Read(); // 读取 *
                            currentEndColumn++;
                            skipMultiLineComment();
                            goto skip;
                    }
                    break;

                case -1:
                    return JsonTokenType.eof;

                #endregion

                default:

                    // 数字。
                    if (c >= '0' && c <= '9') {
                        return readNumber(c);
                    }

                    // 标识符或其它关键字。
                    if (char.IsLetterOrDigit((char)c)) {
                        readIdentifier(c);

                        if (hasRead("null")) {
                            return JsonTokenType.@null;
                        }

                        if (hasRead("true")) {
                            return JsonTokenType.@true;
                        }

                        if (hasRead("false")) {
                            return JsonTokenType.@false;
                        }

                        if (!strictMode) {

                            if (hasRead("undefined")) {
                                return JsonTokenType.undefined;
                            }

                            if (hasRead("NaN")) {
                                return JsonTokenType.nan;
                            }

                            if (hasRead("Infinity")) {
                                return JsonTokenType.infinity;
                            }

                            //if (hasRead("new")) {
                            //    return JsonTokenType.@new;
                            //}

                        }

                        return JsonTokenType.identifier;

                    }

                    // 特殊空格。
                    if (c > 255 && char.IsWhiteSpace((char)c)) {
                        goto skip;
                    }

                    break;

            }

            // 如果执行到此处，说明是非法字符。
            reportError(String.Format("无效的字符：“{0}”", (char)c));
            return JsonTokenType.eof;

        }

        private bool hasRead(string content) {
            if (currentBuffer.Length != content.Length) {
                return false;
            }
            for (int i = 0; i < content.Length; i++) {
                if (currentBuffer[i] != content[i]) {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 读取一个字符串。
        /// </summary>
        /// <param name="quoteChar">当前引号。</param>
        private void readString(int quoteChar) {
            currentBuffer.Length = 0;
            while (true) {
                int c = input.Read();
                currentEndColumn++;

                if (c == quoteChar) {
                    return;
                }

                switch (c) {
                    case '\\':
                        c = input.Read();
                        currentEndColumn++;

                        switch (c) {
                            case 'b':
                                c = '\b';
                                break;
                            case 't':
                                c = '\t';
                                break;
                            case 'n':
                                c = '\n';
                                break;
                            case 'f':
                                c = '\f';
                                break;
                            case 'r':
                                c = '\r';
                                break;
                            case 'x':
                                if (strictMode) {
                                    reportError("严格模式：无效的转义字符；应使用“\\u”代替“\\x”");
                                }
                                c = (readHexDight() << 4) | readHexDight();
                                break;
                            case 'u':
                                c = (readHexDight() << 12) | (readHexDight() << 8) | (readHexDight() << 4) | readHexDight();
                                break;
                            case -1:
                                c = '\\';
                                continue;
                        }
                        break;

                    case '\r':
                        if (input.Peek() == '\n') {
                            input.Read();
                        }
                        goto case '\n';
                    case '\n':
                        if (strictMode) {
                            reportError("严格模式：字符串未关闭；字符串内发现换行符");
                        }
                        newLine();
                        continue;

                    case -1:
                        reportError(String.Format("字符串未关闭；应输入“{0}”；文件提前结束", (char)quoteChar));
                        return;

                }
                currentBuffer.Append((char)c);
            }

        }

        /// <summary>
        /// 读取紧跟的一个十六进制数字。
        /// </summary>
        /// <returns></returns>
        private int readHexDight() {
            int c = input.Read();
            currentEndColumn++;

            if (c >= '0' && c <= '9') {
                return c - '0';
            }

            c |= 0x20;

            if (c >= 'a' && c <= 'f') {
                return 10 + c - 'a';
            }

            if (c == -1) {
                reportError("无效的 Unicode 转义字符；文件提前结束");
            } else {
                reportError(String.Format("无效的 Unicode 转义字符：“{0}”", (char)c));
            }

            return c;

        }

        /// <summary>
        /// 读取一个数字。
        /// </summary>
        /// <param name="c">当前字符。</param>
        /// <returns></returns>
        private JsonTokenType readNumber(int c) {

            // c 可能是 . 或 - 或数字。
            currentBuffer.Length = 1;
            currentBuffer[0] = (char)c;

            readDecimalDights();
            if (currentBuffer.Length == 1 && (c == '-' || c == '.')) {
                reportError(String.Format("无效的数字；“{0}”不是数字", (char)input.Peek()));
                return JsonTokenType.floatLiteral;
            }

            JsonTokenType result = JsonTokenType.floatLiteral;

            if (c != '.') {

                switch (input.Peek()) {
                    case '.':
                        input.Read();
                        currentEndColumn++;
                        currentBuffer.Append('.');
                        readDecimalDights();
                        result = JsonTokenType.floatLiteral;
                        break;
                    case 'x':
                    case 'X':
                        if (strictMode) {
                            goto default;
                        }
                        int prefixLength = c == '-' ? 2 : 1;
                        if (currentBuffer.Length == prefixLength && currentBuffer[prefixLength - 1] == '0') {
                            input.Read();
                            currentEndColumn++;
                            currentBuffer.Length = 0;
                            readHexDights();
                            if (currentBuffer.Length == prefixLength) {
                                reportError(String.Format("无效的十六进制数字；“{0}”不是十六进制数字", (char)input.Peek()));
                            }
                            return JsonTokenType.hexLiteral;
                        }
                        goto default;
                    default:
                        result = JsonTokenType.intLiteral;
                        break;
                }

            }

            // 读取科学计数法。
            if (!strictMode && (input.Peek() == 'e' || input.Peek() == 'E')) {
                input.Read();
                currentEndColumn++;
                currentBuffer.Append('e');
                result = JsonTokenType.floatLiteral;
                if (input.Peek() == '+' || input.Peek() == '-') {
                    currentBuffer.Append((char)input.Read());
                    currentEndColumn++;
                }
                int col = currentEndColumn;
                readDecimalDights();
                if (col == currentEndColumn) {
                    reportError(String.Format("无效的科学计数法表达式；“{0}”不是数字", (char)input.Peek()));
                }
            }

            return result;

        }

        /// <summary>
        /// 读取紧跟的全部十进制数字。
        /// </summary>
        private void readDecimalDights() {
            int c;
            while (true) {
                c = input.Peek();

                if (c < '0' || c > '9') {
                    return;
                }

                input.Read();
                currentEndColumn++;
                currentBuffer.Append((char)c);
            }
        }

        /// <summary>
        /// 读取紧跟的全部十进制数字。
        /// </summary>
        private void readHexDights() {
            int c;
            while (true) {
                c = input.Peek();

                if (c < '0' || c > '9') {
                    c |= 0x20;
                    if (c < 'a' || c > 'z') {
                        return;
                    }
                }

                input.Read();
                currentEndColumn++;
                currentBuffer.Append((char)c);
                continue;
            }
        }

        /// <summary>
        /// 读取一个标识符。
        /// </summary>
        /// <param name="c"></param>
        private void readIdentifier(int c) {
            currentBuffer.Length = 1;
            currentBuffer[0] = (char)c;

            while (char.IsLetterOrDigit((char)input.Peek())) {
                currentBuffer.Append((char)input.Read());
                currentEndColumn++;
            }
        }

        /// <summary>
        /// 忽略多行注释。
        /// </summary>
        private void skipMultiLineComment() {
            while (true) {
                int c = input.Read();
                currentEndColumn++;
                switch (c) {
                    case '\r':
                        if (input.Peek() == '\n') {
                            input.Read();
                        }
                        goto case '\n';
                    case '\n':
                        newLine();
                        break;
                    case '*':
                        if (input.Peek() == '/') {
                            input.Read();
                            return;
                        }
                        break;
                    case -1:
                        reportError("多行注释未关闭；文件提前结束");
                        return;
                }
            }
        }

        /// <summary>
        /// 忽略单行注释。
        /// </summary>
        private void skipSingelineComment() {
            while (true) {
                int c = input.Read();
                switch (c) {
                    case '\r':
                        if (input.Peek() == '\n') {
                            input.Read();
                        }
                        goto case '\n';
                    case '\n':
                        newLine();
                        return;
                    case -1:
                        return;
                }
            }
        }

        #endregion

        #region 语法分析

        /// <summary>
        /// 获取当前标记。
        /// </summary>
        public JsonTokenType currentTokenType;

        /// <summary>
        /// 读取下一个标记。
        /// </summary>
        /// <returns></returns>
        public JsonTokenType readRaw() {
            return currentTokenType = scan();
        }

        /// <summary>
        /// 读取数据的开头部分。
        /// </summary>
        public bool readValueStart() {
            if (readRaw() == JsonTokenType.eof) {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 读取数据的结尾部分。
        /// </summary>
        public void readValueEnd() {
            if (currentTokenType != JsonTokenType.eof) {
                reportError(String.Format("读取 JSON 结束后发现多余的字符:“{0}”", getCurrentToken()));
            }
        }

        /// <summary>
        /// 读取对象项的开头部分。
        /// </summary>
        /// <returns>返回属性名，如果不存在项返回 null。</returns>
        public bool readObjectItemStart(out string propertyName) {
            switch (readRaw()) { // 读取 { 或 ,
                case JsonTokenType.stringLiteral:
                    propertyName = currentBuffer.ToString();

                    // 读取 :
                    if (readRaw() == JsonTokenType.colon) {
                        readRaw();
                    } else {
                        reportError(String.Format("应输入“:”；实际为“{0}”", getCurrentToken()));
                    }

                    return true;
                case JsonTokenType.eof:
                    reportError("应输入“}”；文件提前结束");
                    goto case JsonTokenType.objectEnd;
                case JsonTokenType.objectEnd:
                    readRaw();
                    propertyName = null;
                    return false;
                case JsonTokenType.identifier:
                case JsonTokenType.intLiteral:
                case JsonTokenType.floatLiteral:
                case JsonTokenType.hexLiteral:
                case JsonTokenType.nan:
                case JsonTokenType.infinity:
                //case JsonTokenType.@new:
                case JsonTokenType.@true:
                case JsonTokenType.@false:
                case JsonTokenType.@null:
                case JsonTokenType.undefined:
                    if (strictMode) {
                        reportError("严格模式：属性名应该使用双引号包围");
                    }
                    if (currentTokenType == JsonTokenType.hexLiteral) {
                        currentBuffer.Insert(0, "0x");
                    }
                    goto case JsonTokenType.stringLiteral;
                default:
                    reportError(String.Format("应输入属性名；实际为“{0}”", getCurrentToken()));
                    propertyName = String.Empty;
                    return true;
            }
        }

        /// <summary>
        /// 读取对象项的结尾部分。
        /// </summary>
        /// <returns>指示是否读到末尾。</returns>
        public bool readObjectItemEnd() {
            switch (currentTokenType) {
                case JsonTokenType.objectEnd:
                    readRaw();
                    return true;
                case JsonTokenType.comma:
                    return false;
                case JsonTokenType.eof:
                    reportError("应输入“}”；文件提前结束");
                    return true;
                default:
                    reportError(String.Format("应输入“}”；实际为“{0}”", getCurrentToken()));
                    return false;
            }
        }

        /// <summary>
        /// 读取数组项的开头部分。
        /// </summary>
        /// <returns></returns>
        public bool readArrayItemStart(out bool foundUndefined) {
            foundUndefined = false;
            switch (readRaw()) { // 读取 [ 或 ,
                case JsonTokenType.arrayEnd:
                    readRaw();
                    return false;
                case JsonTokenType.eof:
                    reportError(String.Format("应输入“]”；实际为“{0}”", getCurrentToken()));
                    return false;
                case JsonTokenType.comma:
                    if (strictMode) {
                        reportError("严格模式：发现多余的“,”");
                    }
                    foundUndefined = true;
                    return true;
                default:
                    return true;
            }
        }

        /// <summary>
        /// 读取数组项的结尾部分。
        /// </summary>
        /// <returns></returns>
        public bool readArrayItemEnd() {
            switch (currentTokenType) {
                case JsonTokenType.arrayEnd:
                    readRaw();
                    return true;
                case JsonTokenType.comma:
                    return false;
                case JsonTokenType.eof:
                    reportError("应输入“]”；文件提前结束");
                    goto case JsonTokenType.arrayEnd;
                default:
                    reportError(String.Format("应输入“]”；实际为“{0}”", getCurrentToken()));
                    return false;
            }
        }

        /// <summary>
        /// 读取一个字符串。
        /// </summary>
        /// <returns></returns>
        public string readString() {
            var result = currentBuffer.ToString();
            readRaw();
            return result;
        }

        /// <summary>
        /// 读取一个字面量。
        /// </summary>
        public void readLiteral() {
            readRaw();
        }

        /// <summary>
        /// 读取一个整数。
        /// </summary>
        public int readInt() {
            var input = currentBuffer.ToString();
            readRaw();

            int result;
            if (int.TryParse(input, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out result)) {
                return result;
            }

            return 0;
        }

        /// <summary>
        /// 读取一个整数，如果表示不下则读取小数。
        /// </summary>
        /// <returns></returns>
        public object readIntOrFloat() {
            var input = currentBuffer.ToString();
            readRaw();

            int result;
            return int.TryParse(input, out result) ? (object)result : double.Parse(input, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 读取一个小数。
        /// </summary>
        public double readFloat() {
            var input = currentBuffer.ToString();
            readRaw();
            return double.Parse(input, System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 读取一个十六进制整数。
        /// </summary>
        public int readHexInt() {
            string input = currentBuffer.ToString();
            readRaw();

            int result;
            if (int.TryParse(input, System.Globalization.NumberStyles.HexNumber, System.Globalization.CultureInfo.InvariantCulture, out result)) {
                return result;
            }

            reportError(String.Format("十六进制整数“{0}”太大；十六进制整数必须在 {1} 和 {2} 之间", currentBuffer.ToString(), int.MinValue, int.MaxValue));
            return 0;
        }

        /// <summary>
        /// 读取文件末尾。
        /// </summary>
        public void readEOF() {
            if (currentTokenType == JsonTokenType.eof) {
                reportError("应输入值；文件提前结束");
            } else {
                reportError(String.Format("应输入值；实际为“{0}”", getCurrentToken()));
            }
        }

        #endregion

    }

    /// <summary>
    /// 表示 JSON 标记类型。
    /// </summary>
    public enum JsonTokenType {

        /// <summary>
        /// 已读取完毕。
        /// </summary>
        eof,

        /// <summary>
        /// {
        /// </summary>
        objectStart,

        /// <summary>
        /// }
        /// </summary>
        objectEnd,

        /// <summary>
        /// [
        /// </summary>
        arrayStart,

        /// <summary>
        /// ]
        /// </summary>
        arrayEnd,

        ///// <summary>
        ///// (
        ///// </summary>
        //leftParam,

        ///// <summary>
        ///// )
        ///// </summary>
        //rightParam,

        /// <summary>
        /// :
        /// </summary>
        colon,

        /// <summary>
        /// ,
        /// </summary>
        comma,

        ///// <summary>
        ///// new
        ///// </summary>
        //@new,

        /// <summary>
        /// 属性名。
        /// </summary>
        identifier,

        /// <summary>
        /// 整数字面量。
        /// </summary>
        intLiteral,

        /// <summary>
        /// 十六进制字面量。
        /// </summary>
        hexLiteral,

        /// <summary>
        /// 小数字面量。
        /// </summary>
        floatLiteral,

        /// <summary>
        /// 字符串字面量。
        /// </summary>
        stringLiteral,

        /// <summary>
        /// true。
        /// </summary>
        @true,

        /// <summary>
        /// false。
        /// </summary>
        @false,

        /// <summary>
        /// null。
        /// </summary>
        @null,

        /// <summary>
        /// undefined。
        /// </summary>
        undefined,

        /// <summary>
        /// NaN。
        /// </summary>
        nan,

        /// <summary>
        /// Infinity。
        /// </summary>
        infinity,

        /// <summary>
        /// -Infinity。
        /// </summary>
        negInfinity,

    }

    /// <summary>
    /// 表示由于 JSON 字符串格式错误引发的异常。
    /// </summary>
    public class JsonParseException : FormatException {

        /// <summary>
        /// 获取当前发生错误的起始行号。
        /// </summary>
        public int startLine;

        /// <summary>
        /// 获取当前发生错误的起始列号。
        /// </summary>
        public int startColumn;

        /// <summary>
        /// 获取当前发生错误的结束行号。
        /// </summary>
        public int endLine;

        /// <summary>
        /// 获取当前发生错误的结束列号。
        /// </summary>
        public int endColumn;

        /// <summary>
        /// 使用指定错误信息初始化 JsonException 类的新实例。
        /// </summary>
        /// <param name="message">描述该错误的 System.String。message 的内容被设计为人可理解的形式。此构造函数的调用方需要确保此字符串已针对当前系统区域性进行了本地化</param>
        /// <param name="startLine">当前发生错误的起始行号。</param>
        /// <param name="startColumn">当前发生错误的起始列号。</param>
        /// <param name="endLine">当前发生错误的结束行号。</param>
        /// <param name="endColumn">当前发生错误的结束列号。</param>
        public JsonParseException(string message, int startLine, int startColumn, int endLine, int endColumn)
            : base(message) {
            this.startLine = startLine;
            this.startColumn = startColumn;
            this.endLine = endLine;
            this.endColumn = endColumn;
        }

    }

    /// <summary>
    /// 表示一个 JSON 对象。
    /// </summary>
    public sealed class JsonObject : Dictionary<string, object>, IEnumerable<KeyValuePair<string, object>> {

        /// <summary>
        /// 初始化 <see cref="JsonObject"/> 类的新实例，该实例为空并且具有默认初始容量。
        /// </summary>
        public JsonObject()
            : base() { }

        /// <summary>
        /// 初始化 <see cref="JsonObject"/> 类的新实例，该实例为空并且具有指定的初始容量。
        /// </summary>
        /// <param name="capacity">新列表最初可以存储的元素数。</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
        public JsonObject(int capacity = 2)
            : base(capacity) { }

        /// <summary>
        /// 获取或设置指定键的值。
        /// </summary>
        public new object this[string key] {
            get {
                object result;
                return base.TryGetValue(key, out result) ? result : null;
            }
            set {
                base[key] = value;
            }
        }

        /// <summary>
        /// 将指定的键和值添加到字典中。
        /// </summary>
        /// <param name="key">要添加的元素的键。</param>
        /// <param name="value">要添加的元素的值。 对于引用类型，该值可以为 null。</param>
        public new void Add(string key, object value) {
            base[key] = value;
        }

        /// <summary>
        /// 获取当前 JSON 值的 JSON 字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Json.stringify(this);
        }

    }

    /// <summary>
    /// 表示一个 JSON 对象。
    /// </summary>
    public sealed class JsonArray : List<object>, IEnumerable<object> {

        /// <summary>
        /// 初始化 <see cref="JsonArray"/> 类的新实例，该实例为空并且具有默认初始容量。
        /// </summary>
        public JsonArray()
            : base() { }

        /// <summary>
        /// 初始化 <see cref="JsonArray"/> 类的新实例，该实例为空并且具有指定的初始容量。
        /// </summary>
        /// <param name="capacity">新列表最初可以存储的元素数。</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="capacity"/> 小于 0。</exception>
        public JsonArray(int capacity)
            : base(capacity) { }

        /// <summary>
        /// 将元素插入 <see cref="T:System.Collections.Generic.List`1"/> 的指定索引处。
        /// </summary>
        /// <param name="index">从零开始的索引，应在该位置插入 <paramref name="item"/>。</param>
        /// <param name="item">要插入的对象。 对于引用类型，该值可以为 null。</param>
        public new void Insert(int index, object item) {
            if (index >= Count) {
                for (int i = Count; i < index; i++) {
                    Add(JsonUndefined.value);
                }
                Add(JsonUndefined.value);
                return;
            }
            base.Insert(index, item);
        }

        /// <summary>
        /// 移除 <see cref="T:System.Collections.Generic.List`1"/> 的指定索引处的元素。
        /// </summary>
        /// <param name="index">要移除的元素的从零开始的索引。</param>
        public new bool RemoveAt(int index) {
            if (index < 0 || index >= Count) {
                return false;
            }
            base.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 获取或设置指定索引的值。
        /// </summary>
        public new object this[int index] {
            get {
                if (index < 0 || index >= Count) {
                    return null;
                }
                return base[index];
            }
            set {
                if (index >= Count) {
                    Insert(index, value);
                } else {
                    base[index] = value;
                }
            }
        }

        /// <summary>
        /// 获取当前 JSON 值的 JSON 字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return Json.stringify(this);
        }

    }

    /// <summary>
    /// 表示一个 undeined。
    /// </summary>
    public sealed class JsonUndefined : IJsonConverter {

        private JsonUndefined() { }

        /// <summary>
        /// 获取默认的 null 实例。
        /// </summary>
        public static readonly JsonUndefined value = new JsonUndefined();

        /// <summary>
        /// 获取当前 JSON 值的 JSON 字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            return "undefined";
        }

        /// <summary>
        /// 从指定 JSON 读取器读取对象。
        /// </summary>
        /// <param name="reader">要读取的读取器。</param>
        /// <param name="type">要读取的类型。</param>
        /// <returns>返回读取的对象。</returns>
        object IJsonConverter.read(JsonReader reader, Type type) {
            if (reader.currentTokenType == JsonTokenType.undefined) {
                reader.readLiteral();
                return this;
            }
            Json.read(reader);
            return this;
        }

        /// <summary>
        /// 将指定的对象写入到输出器。
        /// </summary>
        /// <param name="writer">写入的输出器。</param>
        /// <param name="value">要写入的对象。</param>
        void IJsonConverter.write(JsonWriter writer, object value) {
            writer.writeUndefined();
        }

    }

    #endregion

    #region 对象 => JSON

    /// <summary>
    /// 表示一个 JSON 编写器。
    /// </summary>
    public sealed class JsonWriter : IDisposable {

        #region 属性

        /// <summary>
        /// 获取当前的编写目标。
        /// </summary>
        public readonly TextWriter output;

        /// <summary>
        /// 获取或设置当前编写器使用的格式化字符串。如果不需要格式化则为 null。
        /// </summary>
        public string indentString;

        /// <summary>
        /// 获取或设置当前编写器使用的新行字符串。如果不需要换行则为 null。
        /// </summary>
        public string newLine;

        /// <summary>
        /// 获取或设置当前的缩进等级。
        /// </summary>
        public int indent;

        /// <summary>
        /// 初始化 <see cref="JsonReader"/> 类的新实例。默认设置为不格式化。
        /// </summary>
        /// <param name="output">JSON 源码的输出目标。</param>
        public JsonWriter(TextWriter output) {
            this.output = output;
        }

        /// <summary>
        /// 初始化 <see cref="JsonReader"/> 类的新实例。
        /// </summary>
        /// <param name="output">JSON 源码的输出目标。</param>
        /// <param name="format">指示是否格式化输出字符串。</param>
        public JsonWriter(TextWriter output, bool format) {
            this.output = output;
            if (format) {
                indentString = "\t";
                newLine = "\r\n";
            }
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose() {
            output.Dispose();
        }

        /// <summary>
        /// 用于反射写入对象时检测循环引用。
        /// </summary>
        internal HashSet<object> writtenObjects;

        #endregion

        #region 写入

        /// <summary>
        /// 写入逗号。
        /// </summary>
        public void writeComma() {
            output.Write(',');
        }

        /// <summary>
        /// 写入新行和缩进。
        /// </summary>
        public void writeLine() {
            if (newLine != null) {
                output.Write(newLine);
                for (int i = 0; i < indent; i++) {
                    output.Write(indentString);
                }
            }
        }

        /// <summary>
        /// 写入原始内容。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeRaw(string value) {
            output.Write(value);
        }

        /// <summary>
        /// 写入原始内容。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeRaw(char value) {
            output.Write(value);
        }

        /// <summary>
        /// 写入一个空。
        /// </summary>
        public void writeNull() {
            output.Write("null");
        }

        /// <summary>
        /// 写入一个 undefined。
        /// </summary>
        public void writeUndefined() {
            output.Write("undefined");
        }

        /// <summary>
        /// 写入一个属性名。
        /// </summary>
        /// <param name="propertyName">要写入的属性名。</param>
        public void writePropertyName(string propertyName) {
            writeString(propertyName);
            output.Write(indentString == null ? ":" : ": ");
        }

        /// <summary>
        /// 写入一个新值。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeString(string value) {
            output.Write('\"');
            Json.writeEscapedJavaScriptString(output, value, '\"');
            output.Write('\"');
        }

        /// <summary>
        /// 写入一个新值。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeBool(bool value) {
            output.Write(value ? "true" : "false");
        }

        /// <summary>
        /// 写入一个新值。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeInt(int value) {
            output.Write(value);
        }

        /// <summary>
        /// 写入一个新值。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeFloat(float value) {
            output.Write(value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
        }

        /// <summary>
        /// 写入一个新值。
        /// </summary>
        /// <param name="value">要写入的内容。</param>
        public void writeFloat(double value) {
            output.Write(value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
        }

        /// <summary>
        /// 写入对象开始标记。
        /// </summary>
        public void writeObjectStart() {
            output.Write('{');
        }

        /// <summary>
        /// 写入对象开始标记。
        /// </summary>
        public void writeObjectEnd() {
            output.Write('}');
        }

        /// <summary>
        /// 写入对象开始标记。
        /// </summary>
        public void writeArrayStart() {
            output.Write('[');
        }

        /// <summary>
        /// 写入对象开始标记。
        /// </summary>
        public void writeArrayEnd() {
            output.Write(']');
        }

        #endregion

    }

    #endregion

    #region 转换器

    /// <summary>
    /// 提供格式化和解析 JSON 的相关方法。
    /// </summary>
    public static class Json {

        #region 转换器

        /// <summary>
        /// 获取所有转换器。
        /// </summary>
        public static readonly Dictionary<Type, IJsonConverter> converters = new Dictionary<Type, IJsonConverter>() {
            { typeof(string), new StringConverter()},
            
            { typeof(int), new IntJsonConverter()},
            { typeof(bool), new BoolJsonConverter()},
            { typeof(double), new DoubleJsonConverter()},
            
            { typeof(float), new FloatJsonConverter()},
            { typeof(char), new CharJsonConverter()},
            { typeof(object), new DefaultJsonConverter()},

            { typeof(uint), ConvertibleJsonConverter.instance },
            { typeof(long), ConvertibleJsonConverter.instance},
            { typeof(ulong), ConvertibleJsonConverter.instance},
            { typeof(short), ConvertibleJsonConverter.instance},
            { typeof(ushort), ConvertibleJsonConverter.instance},
            { typeof(byte),  ConvertibleJsonConverter.instance},
            { typeof(sbyte), ConvertibleJsonConverter.instance},
            { typeof(decimal),ConvertibleJsonConverter.instance},
            
            { typeof(JsonUndefined), JsonUndefined.value},
            { typeof(JsonArray), new ListBasedJsonConverter() },
            { typeof(JsonObject), new DictionaryBasedJsonConverter()},

            { typeof(DataTable), new DataTableJsonConverter()},
            { typeof(DataSet), new DataSetJsonConverter()},
            { typeof(DBNull), new DBNullJsonConverter()},
            { typeof(DateTime), new DateTimeJsonConverter()},
            { typeof(Guid), new GuidJsonConverter()},
            { typeof(Version), new VersionJsonConverter()},
            
            {typeof(IDictionary), new DictionaryBasedJsonConverter()},
            {typeof(IList), new ListBasedJsonConverter()},
            {typeof(IEnumerable), new ListBasedJsonConverter()},
            {typeof(Array), new ArrayJsonConverter()},
            {typeof(Enum), new EnumJsonConverter()},
            
        };

        /// <summary>
        /// 获取指定类型的转换器。
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IJsonConverter getConverter(Type type) {

            // 查找直接使用的转换器。
            IJsonConverter result;
            if (converters.TryGetValue(type, out result)) {
                return result;
            }

            Type changedType;

            // 查找基类转换器。
            if (typeof(IEnumerable).IsAssignableFrom(type)) {
                if (typeof(IDictionary).IsAssignableFrom(type)) {
                    changedType = typeof(JsonObject);
                } else if (type.IsArray) {
                    changedType = typeof(Array);
                } else {
                    changedType = typeof(JsonArray);
                }
            } else if (typeof(Enum).IsAssignableFrom(type)) {
                changedType = typeof(Enum);
            } else {
                changedType = typeof(object);
            }

            result = converters[changedType];
            converters.Add(type, result);
            return result;

        }

        #endregion

        #region JSON => 对象

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="jsonString">要解析的 JSON 字符串。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object parse(string jsonString) {
            return jsonString == null ? null : parse(new JsonReader(new StringReader(jsonString)));
        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="jsonString">要解析的 JSON 字符串。</param>
        /// <param name="strictMode">是否使用严格模式。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object parse(string jsonString, bool strictMode) {
            return jsonString == null ? null : parse(new JsonReader(new StringReader(jsonString)) {
                strictMode = strictMode
            });
        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="reader">要读取的对象。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object parse(JsonReader reader) {
            object result;
            if (reader.readValueStart()) {
                result = read(reader);
                reader.readValueEnd();
            } else {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 从指定的读取器读取一个对象。
        /// </summary>
        /// <param name="reader">要读取的读取器。</param>
        /// <returns>返回读取的对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object read(JsonReader reader) {

            switch (reader.currentTokenType) {

                #region 解析 Object
                case JsonTokenType.objectStart:
                    var obj = new JsonObject();
                    string propertyName;
                    while (reader.readObjectItemStart(out propertyName)) {
                        obj[propertyName] = read(reader);
                        if (reader.readObjectItemEnd()) {
                            break;
                        }
                    }
                    return obj;
                #endregion

                #region 解析 Array
                case JsonTokenType.arrayStart:
                    var arr = new JsonArray();
                    bool foundUndefined;
                    while (reader.readArrayItemStart(out foundUndefined)) {
                        arr.Add(foundUndefined ? JsonUndefined.value : read(reader));
                        if (reader.readArrayItemEnd()) {
                            break;
                        }
                    }
                    return arr;
                #endregion

                #region 其它常量

                case JsonTokenType.stringLiteral:
                    return reader.readString();
                case JsonTokenType.undefined:
                    reader.readLiteral();
                    return JsonUndefined.value;
                case JsonTokenType.@null:
                    reader.readLiteral();
                    return null;
                case JsonTokenType.@true:
                    reader.readLiteral();
                    return true;
                case JsonTokenType.@false:
                    reader.readLiteral();
                    return false;

                #endregion

                #region 解析数字

                case JsonTokenType.intLiteral:
                    return reader.readIntOrFloat();
                case JsonTokenType.floatLiteral:
                    return reader.readFloat();
                case JsonTokenType.hexLiteral:
                    return reader.readHexInt();
                case JsonTokenType.nan:
                    reader.readLiteral();
                    return double.NaN;
                case JsonTokenType.infinity:
                    reader.readLiteral();
                    return double.NegativeInfinity;
                case JsonTokenType.negInfinity:
                    reader.readLiteral();
                    return double.NegativeInfinity;

                #endregion

                default:
                    reader.readEOF();
                    return JsonUndefined.value;
            }

        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="jsonString">要解析的 JSON 字符串。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static T parse<T>(string jsonString) {
            return (T)parse(jsonString, typeof(T));
        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="jsonString">要解析的 JSON 字符串。</param>
        /// <param name="strictMode">是否使用严格模式。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static T parse<T>(string jsonString, bool strictMode) {
            return (T)parse(jsonString, typeof(T), strictMode);
        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="jsonString">要解析的 JSON 字符串。</param>
        /// <param name="targetType">要转换的类型。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object parse(string jsonString, Type targetType) {
            return jsonString == null ? null : parse(new JsonReader(new StringReader(jsonString)), targetType);
        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="jsonString">要解析的 JSON 字符串。</param>
        /// <param name="targetType">要转换的类型。</param>
        /// <param name="strictMode">是否使用严格模式。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object parse(string jsonString, Type targetType, bool strictMode) {
            return jsonString == null ? null : parse(new JsonReader(new StringReader(jsonString)) {
                strictMode = strictMode
            }, targetType);
        }

        /// <summary>
        /// 将指定的 JSON 字符串转为对象。
        /// </summary>
        /// <param name="reader">要读取的对象。</param>
        /// <param name="targetType">要转换的类型。</param>
        /// <returns>返回一个 JSON 对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object parse(JsonReader reader, Type targetType) {
            object result;
            if (reader.readValueStart()) {
                result = read(reader, targetType);
                reader.readValueEnd();
            } else {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// 从指定的读取器读取一个对象为指定类型。
        /// </summary>
        /// <param name="reader">要读取的读取器。</param>
        /// <param name="targetType">要转换的类型。</param>
        /// <returns>返回读取的对象。</returns>
        /// <exception cref="JsonParseException">解析错误时发生的异常。</exception>
        public static object read(JsonReader reader, Type targetType) {
            return getConverter(targetType).read(reader, targetType);
        }

        #endregion

        #region 对象 => JSON

        /// <summary>
        /// 将一个对象转为 JSON 字符串。
        /// </summary>
        /// <param name="value">要转换的对象。</param>
        /// <returns>JSON 字符串。</returns>
        public static string stringify(object value) {
            var sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(new StringWriter(sb));
            write(writer, value);
            return sb.ToString();
        }

        /// <summary>
        /// 将一个对象转为 JSON 字符串。
        /// </summary>
        /// <param name="value">要转换的对象。</param>
        /// <param name="indent">指示是否格式化输出的 JSON 字符串。</param>
        /// <returns>JSON 字符串。</returns>
        public static string stringify(object value, bool indent) {
            var sb = new StringBuilder();
            JsonWriter writer = new JsonWriter(new StringWriter(sb), indent);
            write(writer, value);
            return sb.ToString();
        }

        /// <summary>
        /// 将特定的对象写入输出器。
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        public static void write(JsonWriter writer, object value) {

            // 处理 null。
            if (value == null) {
                writer.writeNull();
                return;
            }

            // 找到对应类型的转换器。
            IJsonConverter converter;
            if (!converters.TryGetValue(value.GetType(), out converter)) {
                Type changedType;
                if (value is IEnumerable<KeyValuePair<string, object>>) {
                    changedType = typeof(JsonObject);
                } else if (value is IEnumerable) {
                    changedType = typeof(JsonArray);
                } else if (value is Enum) {
                    changedType = typeof(Enum);
                } else {
                    changedType = typeof(object);
                }
                converters[value.GetType()] = converter = converters[changedType];
            }

            converter.write(writer, value);

        }

        /// <summary>
        /// 将一个对象转为 JSON 字符串。
        /// </summary>
        /// <param name="value">要转换的对象。</param>
        /// <param name="delimiter">格式化字符串时使用的引号。</param>
        /// <returns>JSON 字符串。</returns>
        public static string stringify(string value, char delimiter = '"') {
            if (value == null) {
                return "null";
            }
            StringWriter sw = new StringWriter(new StringBuilder(value.Length + 2));
            sw.Write(delimiter);
            writeEscapedJavaScriptString(sw, value, delimiter);
            sw.Write(delimiter);
            return sw.ToString();
        }

        internal static void writeEscapedJavaScriptString(TextWriter writer, string value, char delimiter) {

            for (int i = 0; i < value.Length; i++) {
                var c = value[i];

                // 最大的转义字符是 '\u2029'。
                // 加速处理中文字符。
                if (c <= '\u2029') {

                    // 加速处理非英文字符。
                    if (c >= '\u0085') { // 133
                        switch (c) {
                            case '\u0085': // 换行
                                writer.Write(@"\u0085");
                                continue;
                            case '\u2028': // 新行
                                writer.Write(@"\u2028");
                                continue;
                            case '\u2029': // 段落分隔符
                                writer.Write(@"\u2029");
                                continue;
                        }
                    } else if (c == '\\') { // 92
                        writer.Write('\\');
                    } else if (c <= 39) {
                        switch (c) {
                            case '\t':
                                writer.Write('\\');
                                c = 't';
                                break;
                            case '\n':
                                writer.Write('\\');
                                c = 'n';
                                break;
                            case '\r':
                                writer.Write('\\');
                                c = 'r';
                                break;
                            case '\f':
                                writer.Write('\\');
                                c = 'f';
                                break;
                            case '\b':
                                writer.Write('\\');
                                c = 'b';
                                break;
                            default:

                                // 写入分隔符。
                                if (c == delimiter) {
                                    writer.Write('\\');
                                }

                                if (c < ' ') {
                                    writer.Write('\\');
                                    writer.Write('u');

                                    writer.Write(intToHex(c >> 12));
                                    writer.Write(intToHex(c >> 8));
                                    writer.Write(intToHex(c >> 4));
                                    writer.Write(intToHex(c));

                                    continue;
                                }

                                break;
                        }
                    }
                }

                writer.Write(c);
            }

        }

        private static char intToHex(int n) {
            n &= 0x000f;
            return (char)(n <= 9 ? n + '0' : (n - 10 + 'a'));
        }

        /// <summary>
        /// 将一个对象转为 JSON 字符串。
        /// </summary>
        /// <param name="value">要转换的对象。</param>
        /// <returns>JSON 字符串。</returns>
        public static string stringify(int value) {
            return value.ToString();
        }

        /// <summary>
        /// 将一个对象转为 JSON 字符串。
        /// </summary>
        /// <param name="value">要转换的对象。</param>
        /// <returns>JSON 字符串。</returns>
        public static string stringify(double value) {
            return value.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// 将一个对象转为 JSON 字符串。
        /// </summary>
        /// <param name="value">要转换的对象。</param>
        /// <returns>JSON 字符串。</returns>
        public static string stringify(bool value) {
            return value ? "true" : "false";
        }

        #endregion

    }

    /// <summary>
    /// 用于将特定类型和 JSON 之间互相转换。
    /// </summary>
    public interface IJsonConverter {

        /// <summary>
        /// 从指定 JSON 读取器读取对象。
        /// </summary>
        /// <param name="reader">要读取的读取器。</param>
        /// <param name="type">要读取的类型。</param>
        /// <returns>返回读取的对象。</returns>
        object read(JsonReader reader, Type type);

        /// <summary>
        /// 将指定的对象写入到输出器。
        /// </summary>
        /// <param name="writer">写入的输出器。</param>
        /// <param name="value">要写入的对象。</param>
        void write(JsonWriter writer, object value);

    }

    sealed class DefaultJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {

            // 如果期待类型为 JSON 本身，则直接读取。
            if (type == typeof(object)) {
                return Json.read(reader);
            }

            // 引用类型读取为默认值。
            if (!type.IsValueType && reader.currentTokenType == JsonTokenType.@null) {
                reader.readLiteral();
                return null;
            }

            // 使用反射方式读取。
            var r = Activator.CreateInstance(type);
            if (reader.currentTokenType == JsonTokenType.objectStart) {
                string propertyName;
                while (reader.readObjectItemStart(out propertyName)) {
                    var prop = type.GetProperty(propertyName);
                    prop.SetValue(r, read(reader, prop.PropertyType), null);
                    if (reader.readObjectItemEnd()) {
                        break;
                    }
                }
            } else {
                Json.read(reader);
            }

            return r;
        }

        public void write(JsonWriter writer, object value) {

            // 避免出现递归调用。
            if (writer.writtenObjects == null) {
                writer.writtenObjects = new HashSet<object>();
            }
            if (writer.writtenObjects.Contains(value)) {
                throw new InvalidOperationException("要格式化的对象类型“" + value.GetType().FullName + "”存在循环引用");
            }
            writer.writtenObjects.Add(value);
            try {
                writer.writeObjectStart();
                var properties = value.GetType().GetProperties();
                writer.indent++;
                for (int i = 0; i < properties.Length; i++) {
                    if (i > 0) {
                        writer.writeComma();
                    }
                    writer.writeLine();

                    var p = properties[i];
                    writer.writePropertyName(p.Name);

                    Json.write(writer, p.GetValue(value, null));
                }
                writer.indent--;
                if (properties.Length > 0) {
                    writer.writeLine();
                }
                writer.writeObjectEnd();
            } finally {
                writer.writtenObjects.Remove(value);
            }
        }

    }


    sealed class StringConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            switch (reader.currentTokenType) {
                case JsonTokenType.stringLiteral:
                    return reader.readString();
                case JsonTokenType.@null:
                    reader.readLiteral();
                    return null;
                case JsonTokenType.undefined:
                    reader.readLiteral();
                    return String.Empty;
                default:
                    Json.read(reader);
                    return null;
            }
        }

        public void write(JsonWriter writer, object value) {
            writer.writeString((string)value);
        }

    }

    sealed class IntJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            switch (reader.currentTokenType) {
                case JsonTokenType.intLiteral:
                    return reader.readInt();
                case JsonTokenType.hexLiteral:
                    return reader.readHexInt();
                case JsonTokenType.floatLiteral:
                    return (int)reader.readFloat();
                default:
                    Json.read(reader);
                    return 0;
            }
        }

        public void write(JsonWriter writer, object value) {
            writer.writeInt((int)value);
        }

    }

    sealed class DoubleJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            switch (reader.currentTokenType) {
                case JsonTokenType.intLiteral:
                    return (double)reader.readInt();
                case JsonTokenType.hexLiteral:
                    return (double)reader.readHexInt();
                case JsonTokenType.floatLiteral:
                    return reader.readFloat();
                case JsonTokenType.nan:
                    reader.readLiteral();
                    return double.NaN;
                case JsonTokenType.infinity:
                    reader.readLiteral();
                    return double.PositiveInfinity;
                case JsonTokenType.negInfinity:
                    reader.readLiteral();
                    return double.NegativeInfinity;
                default:
                    Json.read(reader);
                    return 0d;
            }
        }

        public void write(JsonWriter writer, object value) {
            writer.writeFloat((double)value);
        }

    }

    sealed class FloatJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            switch (reader.currentTokenType) {
                case JsonTokenType.intLiteral:
                    return (float)reader.readInt();
                case JsonTokenType.hexLiteral:
                    return (float)reader.readHexInt();
                case JsonTokenType.floatLiteral:
                    return (float)reader.readFloat();
                case JsonTokenType.nan:
                    reader.readLiteral();
                    return float.NaN;
                case JsonTokenType.infinity:
                    reader.readLiteral();
                    return float.NegativeInfinity;
                case JsonTokenType.negInfinity:
                    reader.readLiteral();
                    return float.NegativeInfinity;
                default:
                    Json.read(reader);
                    return 0f;
            }
        }

        public void write(JsonWriter writer, object value) {
            writer.writeFloat((float)value);
        }

    }

    sealed class ConvertibleJsonConverter : IJsonConverter {

        public static readonly ConvertibleJsonConverter instance = new ConvertibleJsonConverter();

        public object read(JsonReader reader, Type type) {
            object val = Json.read(reader);
            IConvertible value = val as IConvertible;
            if (value != null) {
                return value.ToType(type, System.Globalization.CultureInfo.InvariantCulture);
            }
            return Activator.CreateInstance(type);
        }

        public void write(JsonWriter writer, object value) {
            IConvertible cont = (IConvertible)value;
            switch (cont.GetTypeCode()) {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.Int16:
                    writer.writeInt(cont.ToInt32(System.Globalization.CultureInfo.InvariantCulture));
                    break;
                default:
                    writer.writeFloat(cont.ToDouble(System.Globalization.CultureInfo.InvariantCulture));
                    break;
            }

        }

    }

    sealed class BoolJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            switch (reader.currentTokenType) {
                case JsonTokenType.@true:
                    reader.readLiteral();
                    return true;
                case JsonTokenType.@false:
                    reader.readLiteral();
                    return false;
                default:
                    Json.read(reader);
                    return false;
            }
        }

        public void write(JsonWriter writer, object value) {
            writer.writeBool((bool)value);
        }

    }

    sealed class CharJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            if (reader.currentTokenType == JsonTokenType.stringLiteral) {
                var c = reader.readString();
                return c.Length >= 1 ? c[0] : '\0';
            }
            Json.read(reader);
            return '\0';
        }

        public void write(JsonWriter writer, object value) {
            writer.writeString(((char)value).ToString());
        }

    }

    sealed class DBNullJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            if (reader.currentTokenType == JsonTokenType.@null) {
                reader.readLiteral();
                return DBNull.Value;
            }
            Json.read(reader);
            return DBNull.Value;
        }

        public void write(JsonWriter writer, object value) {
            writer.writeNull();
        }

    }

    /// <summary>
    /// 表示所有可枚举对象使用的迭代对象。
    /// </summary>
    abstract class EnumerableJsonConverterBase {

        protected void readItems(JsonReader reader, Type elementType, IList arr) {
            if (reader.currentTokenType == JsonTokenType.arrayStart) {
                bool foundUndefined;
                while (reader.readArrayItemStart(out foundUndefined)) {
                    arr.Add(foundUndefined ? Activator.CreateInstance(elementType) : Json.read(reader, elementType));
                    if (reader.readArrayItemEnd()) {
                        break;
                    }
                }
            } else {
                Json.read(reader);
            }
        }

        public void write(JsonWriter writer, object value) {
            writer.writeArrayStart();
            bool hasValue = false;
            writer.indent++;
            foreach (var t in (IEnumerable)value) {
                if (hasValue) {
                    writer.writeComma();
                } else {
                    hasValue = true;
                }
                writer.writeLine();
                Json.write(writer, t);
            }
            writer.indent--;
            if (hasValue) {
                writer.writeLine();
            }
            writer.writeArrayEnd();
        }

    }

    sealed class EnumerableJsonConverter : EnumerableJsonConverterBase, IJsonConverter {

        public object read(JsonReader reader, Type type) {
            Type elementType;
            var it = type.GetInterface(typeof(IList<>).FullName);
            if (it != null) {
                elementType = it.GetGenericArguments()[0];
            } else {
                elementType = typeof(object);
            }
            var arr = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            readItems(reader, elementType, arr);
            return arr;
        }

    }

    sealed class ArrayJsonConverter : EnumerableJsonConverterBase, IJsonConverter {

        public object read(JsonReader reader, Type type) {
            Type elementType = type.GetElementType();
            var arr = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
            readItems(reader, elementType, arr);
            var result = Array.CreateInstance(elementType, arr.Count);
            arr.CopyTo(result, 0);
            return result;
        }

    }

    sealed class ListBasedJsonConverter : EnumerableJsonConverterBase, IJsonConverter {

        public object read(JsonReader reader, Type type) {

            Type elementType;
            var it = type.GetInterface(typeof(IList<>).FullName);
            if (it != null) {
                elementType = it.GetGenericArguments()[0];
            } else {
                elementType = typeof(object);
            }

            var arr = (IList)Activator.CreateInstance(type);
            readItems(reader, elementType, arr);
            return arr;

        }

    }

    sealed class DictionaryBasedJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {

            Type elementType;

            var it = type.GetInterface(typeof(IDictionary<,>).FullName);
            if (it != null) {
                elementType = it.GetGenericArguments()[1];
            } else {
                elementType = typeof(object);
            }

            var obj = (IDictionary)Activator.CreateInstance(type);
            string propertyName;
            while (reader.readObjectItemStart(out propertyName)) {
                obj[propertyName] = Json.read(reader, elementType);
                if (reader.readObjectItemEnd()) {
                    break;
                }
            }
            return obj;

        }

        public void write(JsonWriter writer, object value) {
            writer.writeObjectStart();
            bool hasValue = false;
            writer.indent++;
            IDictionaryEnumerator de = ((IDictionary)value).GetEnumerator();
            while (de.MoveNext()) {
                if (hasValue) {
                    writer.writeComma();
                } else {
                    hasValue = true;
                }
                writer.writeLine();
                writer.writePropertyName(de.Key.ToString());
                Json.write(writer, de.Value);
            }
            writer.indent--;
            if (hasValue) {
                writer.writeLine();
            }
            writer.writeObjectEnd();
        }

    }

    sealed class DataSetJsonConverter : EnumerableJsonConverterBase, IJsonConverter {

        public object read(JsonReader reader, Type type) {
            var ds = new DataSet();
            if (reader.currentTokenType == JsonTokenType.arrayStart) {
                bool foundUndefined;
                while (reader.readArrayItemStart(out foundUndefined)) {
                    ds.Tables.Add(foundUndefined ? new DataTable() : (DataTable)Json.read(reader, typeof(DataTable)));
                    if (reader.readArrayItemEnd()) {
                        break;
                    }
                }
            } else {
                Json.read(reader);
            }
            return ds;
        }

        public new void write(JsonWriter writer, object value) {
            base.write(writer, ((DataSet)value).Tables);
        }

    }

    sealed class DataTableJsonConverter : IJsonConverter {

        public object read(JsonReader reader, Type type) {
            var dt = new DataTable();

            // 读取每一行数据。
            if (reader.currentTokenType == JsonTokenType.arrayStart) {
                bool foundUndefined;
                while (reader.readArrayItemStart(out foundUndefined)) {
                    var dr = dt.NewRow();
                    if (!foundUndefined) {
                        if (reader.currentTokenType == JsonTokenType.objectStart) {

                            // 读取每一列数据。
                            string propertyName;
                            while (reader.readObjectItemStart(out propertyName)) {

                                var value = Json.read(reader);

                                if (!dt.Columns.Contains(propertyName)) {
                                    var actualType = value == null ? typeof(object) : value.GetType();
                                    dt.Columns.Add(new DataColumn(propertyName, actualType));
                                }

                                dr[propertyName] = value ?? DBNull.Value;

                                if (reader.readObjectItemEnd()) {
                                    break;
                                }
                            }

                        } else {
                            Json.read(reader);
                        }
                    }

                    dr.EndEdit();
                    dt.Rows.Add(dr);

                    if (reader.readArrayItemEnd()) {
                        break;
                    }
                }
            } else {
                Json.read(reader);
            }

            return dt;
        }

        public void write(JsonWriter writer, object value) {
            var dt = (DataTable)value;
            writer.writeArrayStart();
            writer.indent++;
            for (int i = 0; i < dt.Rows.Count; i++) {
                if (i > 0) {
                    writer.writeComma();
                }
                writer.writeLine();

                writer.writeObjectStart();
                writer.indent++;
                for (int j = 0; j < dt.Columns.Count; j++) {
                    if (j > 0) {
                        writer.writeComma();
                    }
                    writer.writeLine();
                    writer.writePropertyName(dt.Columns[j].ColumnName);
                    Json.write(writer, dt.Rows[i][j]);
                }
                writer.indent--;
                if (dt.Columns.Count > 0) {
                    writer.writeLine();
                }
                writer.writeObjectEnd();
            }
            writer.indent--;
            if (dt.Rows.Count > 0) {
                writer.writeLine();
            }
            writer.writeArrayEnd();
        }

    }

    abstract class StringConverterBase {

        public object read(JsonReader reader, Type type) {
            if (reader.currentTokenType == JsonTokenType.stringLiteral) {
                return read(reader.readString(), type);
            }
            Json.read(reader);
            return null;
        }

        protected abstract object read(string value, Type type);

        public void write(JsonWriter writer, object value) {
            writer.writeString(value.ToString());
        }

    }

    sealed class DateTimeJsonConverter : StringConverterBase, IJsonConverter {
        
        protected override object read(string value, Type type) {
            DateTime dt;
            if (DateTime.TryParse(value, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out dt)) {
                return dt;
            }
            return DateTime.MinValue;
        }

        public new void write(JsonWriter writer, object value) {
            var dt = (DateTime)value;
            writer.writeString(dt.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", System.Globalization.CultureInfo.InvariantCulture));
        }

    }

    sealed class GuidJsonConverter : StringConverterBase, IJsonConverter {

        protected override object read(string value, Type type) {
            Guid result;
            return Guid.TryParse(value, out result) ? result : Guid.Empty;
        }

    }

    sealed class VersionJsonConverter : StringConverterBase, IJsonConverter {

        protected override object read(string value, Type type) {
            Version result;
            return Version.TryParse(value, out result) ? result : null;
        }

    }

    sealed class EnumJsonConverter : StringConverterBase, IJsonConverter {

        protected override object read(string value, Type type) {
            try {
                return Enum.Parse(type, value);
            } catch {
                return Activator.CreateInstance(type);
            }
        }

    }

    #endregion

}