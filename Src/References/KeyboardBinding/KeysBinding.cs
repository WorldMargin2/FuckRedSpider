using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace KeysBinding {


    internal class KeysBinding {
        public static readonly Dictionary<String, Keys> KeyNames = new Dictionary<string, Keys> { };

        public static readonly Dictionary<Keys, String> reverseKeyNames;


        static KeysBinding() {
            //初始化按键名称字典
            foreach (Keys key in Enum.GetValues(typeof(Keys))) {
                String name = key.ToString();
                if (!KeyNames.ContainsKey(name)) {
                    KeyNames.Add(name, key);
                }
            }
            reverseKeyNames = KeyNames.ToDictionary(kv => kv.Value, kv => kv.Key);
        }


        //错误管理

        private List<string> errorCode = new List<string> {
            "主控键与副键相同",
            "主控键错误(Ctrl,Alt,Shift)",
            "副键错误",
            "键组已注册",
            "键组未注册",
            "函数未注册"
        };

        public string getError(int err) {
            if (err == 100) return "";
            return errorCode[err];
        }
        //注册键组

        //  键组结构体
        public struct KeyFunction {
            public KeyFunction(Keys modifier, Keys key) {
                this.modifier = modifier;
                this.key = key;
            }
            public Keys modifier;
            public Keys key;
        }
        //  函数-键组字典
        private Dictionary<Action, KeyFunction> registedKeys = new Dictionary<Action, KeyFunction> { };
        //  键组-函数字典（Ctrl, Alt, Shift，None）

        private Dictionary<Keys, Dictionary<Keys, Action>> registedFunctions = new Dictionary<Keys, Dictionary<Keys, Action>> {
            {Keys.Control,new Dictionary<Keys, Action>{ } },
            {Keys.Alt,new Dictionary<Keys, Action>{ } },
            {Keys.Shift,new Dictionary<Keys, Action>{ } },
            {Keys.None,new Dictionary<Keys, Action>{ } },
        };

        //键组管理


        public KeyFunction getKey(Action function) {
            if (!registedKeys.ContainsKey(function)) {
                return new KeyFunction(Keys.None, Keys.None);
            }
            return registedKeys[function];
        }

        public int registKeyFunction(Keys modifier, Keys key, Action function, bool Forced = false) {
            if (modifier == key) {
                return 0;
            } else if (!registedFunctions.ContainsKey(modifier)) {
                return 1;
            } else if (key != Keys.None && !reverseKeyNames.ContainsKey(key) || (key == Keys.Control) || (key == Keys.Alt)|| (key==Keys.Shift)) {
                return 2;
            } else if (registedFunctions[modifier].ContainsKey(key) && !Forced) {
                return 3;
            }

            if (Forced) {
                registedFunctions[modifier].Remove(key);
            }
            registedFunctions[modifier].Add(key, function);
            if (registedKeys.ContainsKey(function)) {
                KeyFunction tmp = registedKeys[function];
                registedFunctions[tmp.modifier].Remove(tmp.key);
            }
            registedKeys[function] = new KeyFunction(modifier, key);


            return 100;
        }

        //========================================================================
        public int removeKeyFunction(Keys modifier, Keys key) {
            if (modifier == key) {
                return 0;
            } else if (!registedFunctions.ContainsKey(modifier)) {
                return 1;
            } else if (key != Keys.None && registedFunctions[modifier].ContainsKey(key)) {
                return 2;
            } else if (!registedFunctions[modifier].ContainsKey(key)) {
                return 4;
            }

            Action f = registedFunctions[modifier][key];
            registedFunctions[modifier].Remove(key);
            registedKeys.Remove(f);

            return 100;
        }

        //  重载
        public int removeKeyFunction(Action function) {
            if (!registedKeys.ContainsKey(function)) return 5;
            KeyFunction f = registedKeys[function];
            registedKeys.Remove(function);
            registedFunctions[f.modifier].Remove(f.key);
            return 100;
        }
        //========================================================================


        //事件处理
        public bool handleKeyFunction(Keys modifier, Keys key) {
            if (!registedFunctions[modifier].ContainsKey(key)) {
                return false;
            }
            registedFunctions[modifier][key]();
            //MessageBox.Show(modifier.ToString(),key.ToString());
            return true;
        }
    }
}
