using System;
using System.Collections.Generic;
using System.Dynamic; // 用于动态对象
using System.Globalization;
using System.Text;

namespace JsonParser {
    // Program.cs

    #region --- 核心解析器 ---

    /// <summary>
    /// 负责解析 JSON 字符串的核心类。
    /// </summary>
    public class JsonParser {
        private readonly string _json;
        private int _pos;

        public JsonParser(string json) {
            _json = json ?? throw new ArgumentNullException(nameof(json));
        }

        /// <summary>
        /// 开始解析并返回一个 C# 对象。
        /// </summary>
        public object Parse() {
            SkipWhitespace();
            var value = ParseValue();
            SkipWhitespace();
            if (_pos < _json.Length) {
                throw new InvalidOperationException($"Unexpected character at position {_pos}: '{CurrentChar}'");
            }
            return value;
        }

        private char CurrentChar => _json[_pos];
        private bool EndOfString => _pos >= _json.Length;

        private void SkipWhitespace() {
            while (!EndOfString && char.IsWhiteSpace(CurrentChar)) {
                _pos++;
            }
        }

        private char Consume() {
            if (EndOfString) throw new InvalidOperationException("Unexpected end of string.");
            return _json[_pos++];
        }

        private void Consume(char expected) {
            if (EndOfString || CurrentChar != expected) {
                throw new InvalidOperationException($"Expected '{expected}' at position {_pos}.");
            }
            _pos++;
        }

        private object ParseValue() {
            SkipWhitespace();
            if (EndOfString) throw new InvalidOperationException("Unexpected end of string while parsing value.");

            switch (CurrentChar) {
                case '{': return ParseObject();
                case '[': return ParseArray();
                case '"': return ParseString();
                case 't': return ParseTrue();
                case 'f': return ParseFalse();
                case 'n': return ParseNull();
                default:
                    if (char.IsDigit(CurrentChar) || CurrentChar == '-') {
                        return ParseNumber();
                    }
                    throw new InvalidOperationException($"Unexpected character '{CurrentChar}' at position {_pos}.");
            }
        }

        private Dictionary<string, object> ParseObject() {
            Consume('{');
            var dict = new Dictionary<string, object>();
            SkipWhitespace();

            if (CurrentChar == '}') {
                Consume('}');
                return dict;
            }

            while (true) {
                SkipWhitespace();
                var key = (string)ParseString();
                SkipWhitespace();
                Consume(':');
                SkipWhitespace();
                var value = ParseValue();
                dict[key] = value;
                SkipWhitespace();

                if (CurrentChar == '}') {
                    Consume('}');
                    break;
                }
                Consume(',');
            }
            return dict;
        }

        private List<object> ParseArray() {
            Consume('[');
            var list = new List<object>();
            SkipWhitespace();

            if (CurrentChar == ']') {
                Consume(']');
                return list;
            }

            while (true) {
                SkipWhitespace();
                list.Add(ParseValue());
                SkipWhitespace();

                if (CurrentChar == ']') {
                    Consume(']');
                    break;
                }
                Consume(',');
            }
            return list;
        }

        private string ParseString() {
            Consume('"');
            var sb = new StringBuilder();
            while (CurrentChar != '"') {
                if (CurrentChar == '\\') {
                    Consume('\\');
                    switch (Consume()) {
                        case '"': sb.Append('"'); break;
                        case '\\': sb.Append('\\'); break;
                        case '/': sb.Append('/'); break;
                        case 'b': sb.Append('\b'); break;
                        case 'f': sb.Append('\f'); break;
                        case 'n': sb.Append('\n'); break;
                        case 'r': sb.Append('\r'); break;
                        case 't': sb.Append('\t'); break;
                        case 'u':
                            var hex = new string(new[] { Consume(), Consume(), Consume(), Consume() });
                            sb.Append((char)int.Parse(hex, NumberStyles.HexNumber));
                            break;
                        default:
                            throw new InvalidOperationException($"Invalid escape sequence at position {_pos}.");
                    }
                } else {
                    sb.Append(Consume());
                }
            }
            Consume('"');
            return sb.ToString();
        }

        private decimal ParseNumber() {
            int start = _pos;
            if (CurrentChar == '-') Consume();

            if (CurrentChar == '0') {
                Consume();
                if (char.IsDigit(CurrentChar))
                    throw new InvalidOperationException($"Invalid number format at position {_pos}.");
            } else {
                while (!EndOfString && char.IsDigit(CurrentChar)) Consume();
            }

            if (CurrentChar == '.') {
                Consume();
                while (!EndOfString && char.IsDigit(CurrentChar)) Consume();
            }

            if (CurrentChar == 'e' || CurrentChar == 'E') {
                Consume();
                if (CurrentChar == '+' || CurrentChar == '-') Consume();
                while (!EndOfString && char.IsDigit(CurrentChar)) Consume();
            }

            string numberStr = _json.Substring(start, _pos - start);
            if (decimal.TryParse(numberStr, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value)) {
                return value;
            }
            throw new InvalidOperationException($"Invalid number format: '{numberStr}'");
        }

        private bool ParseTrue() {
            Consume('t'); Consume('r'); Consume('u'); Consume('e');
            return true;
        }

        private bool ParseFalse() {
            Consume('f'); Consume('a'); Consume('l'); Consume('s'); Consume('e');
            return false;
        }

        private object ParseNull() {
            Consume('n'); Consume('u'); Consume('l'); Consume('l');
            return null;
        }
    }

    #endregion


    #region --- 公共 API ---

    /// <summary>
    /// 提供简洁的 JSON 解析入口。
    /// </summary>
    public static class Json {
        /// <summary>
        /// 将 JSON 字符串解析为 C# 对象。
        /// 返回类型将是 Dictionary<string, object>, List<object>, string, decimal, bool 或 null。
        /// </summary>
        public static object Parse(string json) {
            var parser = new JsonParser(json);
            return parser.Parse();
        }

        public static string Serialize(object obj) {
            var serializer = new JsonSerializer();
            return serializer.Serialize(obj);
        }

        /// <summary>
        /// 将 JSON 字符串解析为动态对象，允许使用类似 obj.property 的语法访问。
        /// </summary>
        public static dynamic ParseDynamic(string json) {
            return ConvertToDynamic(Parse(json));
        }

        /// <summary>
        /// 递归地将解析出的字典和列表转换为动态对象。
        /// </summary>
        private static object ConvertToDynamic(object obj) {
            if (obj == null) return null;
            if (obj is string || obj is decimal || obj is bool) return obj;

            if (obj is Dictionary<string, object> dict) {
                var expando = new ExpandoObject();
                var expandoDict = (IDictionary<string, object>)expando;
                foreach (var kvp in dict) {
                    expandoDict[kvp.Key] = ConvertToDynamic(kvp.Value);
                }
                return expando;
            }

            if (obj is List<object> list) {
                var dynamicList = new List<object>();
                foreach (var item in list) {
                    dynamicList.Add(ConvertToDynamic(item));
                }
                return dynamicList;
            }

            throw new InvalidOperationException("Unsupported type for dynamic conversion.");
        }
    }

    #endregion


    #region --- json序列化器 ---
    public class JsonSerializer {
        public string Serialize(object value) {
            if (value == null) return "null";

            switch (value) {
                case Dictionary<string, object> dict:
                    return SerializeObject(dict);
                case List<object> list:
                    return SerializeArray(list);
                case string str:
                    return SerializeString(str);
                case bool b:
                    return b ? "true" : "false";
                // 支持多种数字类型
                case decimal _:
                case double _:
                case float _:
                case int _:
                case long _:
                    // 使用 InvariantCulture 确保小数点永远是 '.'
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
                default:
                    throw new InvalidOperationException($"Type '{value.GetType().FullName}' is not supported for JSON serialization.");
            }
        }

        private string SerializeObject(Dictionary<string, object> dict) {
            var sb = new StringBuilder();
            sb.Append("{");
            bool first = true;
            foreach (var kvp in dict) {
                if (!first) sb.Append(", ");
                sb.Append(SerializeString(kvp.Key));
                sb.Append(":");
                sb.Append(Serialize(kvp.Value));
                first = false;
            }
            sb.Append("}");
            return sb.ToString();
        }

        private string SerializeArray(List<object> list) {
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < list.Count; i++) {
                if (i > 0) sb.Append(", ");
                sb.Append(Serialize(list[i]));
            }
            sb.Append("]");
            return sb.ToString();
        }

        private string SerializeString(string str) {
            var sb = new StringBuilder();
            sb.Append("\"");
            foreach (char c in str) {
                switch (c) {
                    case '"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        if (c < '\x20') {
                            sb.Append($"\\u{(int)c:x4}");
                        } else {
                            sb.Append(c);
                        }
                        break;
                }
            }
            sb.Append("\"");
            return sb.ToString();
        }
    }


    #endregion

}
