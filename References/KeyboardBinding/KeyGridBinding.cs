using JsonParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;

namespace KeysBinding {
    internal class KeyGridBinding {
        //设置钩子
        bool is_hooked=false;
        private KeyEventHandler keyEventHandler;
        private KeyboardHook hook = new KeyboardHook();
        private KeysBinding keyBinding;
        private DataGridView keyBindingGrid;

        dynamic config;
        bool autoSave = false;
        private string ConfigPath = null;
        public dynamic __config {
            get {
                return __config;
            }
            set {/* pass*/ }
        }

        //===========================================================================================================================
        public KeyGridBinding(DataGridView keyBindingGrid,string ConfigPath,bool AutoSave=false) {
            this.keyBindingGrid = keyBindingGrid;
            this.ConfigPath = ConfigPath;
            autoSave = AutoSave;
            if (System.IO.File.Exists(ConfigPath)) {
                using (var reader = new System.IO.StreamReader(ConfigPath)) {
                    string json = reader.ReadToEnd();
                    try {
                        config = Json.Parse(json);
                    } catch {
                        config= Json.Parse("{}");
                    }
                }
                using (var writer = new System.IO.StreamWriter(ConfigPath)) {
                    writer.Write("{}");
                }
            } else {
                config= Json.Parse("{}");
                using (var writer = new System.IO.StreamWriter(ConfigPath)) {
                    string json =Json.Serialize(config);
                    writer.Write(json);
                }
            }
            addEvent();
        }
        public KeyGridBinding(DataGridView keyBindingGrid) {
            this.keyBindingGrid = keyBindingGrid;
            addEvent();
        }
        private void handleKey(object sender, KeyEventArgs e) {
            keyBinding.handleKeyFunction(Control.ModifierKeys, e.KeyCode);
        }
        public void startListen() {
            if (is_hooked) return;
            this.keyBinding = new KeysBinding();
            keyEventHandler = new KeyEventHandler(handleKey);
            hook.KeyDownEvent += keyEventHandler;
            hook.Start();
        }

        public void stopListen() {
            if (!is_hooked) return;
            if (keyEventHandler != null) {
                hook.KeyDownEvent -= keyEventHandler;
                keyEventHandler = null;
                hook.Stop();
            }
        }
        //===========================================================================================================================

        Dictionary<string, Action> GridBinding = new Dictionary<string, Action>();
        bool is_user_changed = false;


        public void initGridData() {
            keyBindingGrid.Rows.Clear();
            foreach (var e in GridBinding) {
                var tmp = keyBinding.getKey(e.Value);
                string modifier =
                    tmp.modifier == Keys.Shift ? "Shift" :
                    tmp.modifier == Keys.Control ? "Ctrl" :
                    tmp.modifier == Keys.Alt ? "Alt" : 
                    "None"
                ;
                string key = KeysBinding.reverseKeyNames[tmp.key];
                int index = keyBindingGrid.Rows.Add(e.Key, modifier, key);
            }
            if(autoSave) {
                saveConfig();
            }
        }

        public int bind(string name,Action action,Keys modifire,Keys key,bool Forced=false) {
            if (!GridBinding.ContainsKey(name)) {
                GridBinding.Add(name, action);
            } else {
                var origin = GridBinding[name];
                GridBinding[name] = action;
                if (origin != action) {
                    keyBinding.removeKeyFunction(origin);
                }

            }
            int code = keyBinding.registKeyFunction(modifire, key, action, Forced);
            if (config != null && code==100) {
                try {
                    var __ = config["keyGrid"][name]["modifire"];
                }catch{
                    setData(name,KeysBinding.reverseKeyNames[modifire], KeysBinding.reverseKeyNames[key]);
                }
            }
            return code;
        }
        public int bind(string name, Action action,string modifire,string key, bool Forced = false) {
            Keys m;
            switch (modifire) {
                case "Shift":
                    m = Keys.Shift;
                    break;
                case "Ctrl":
                    m = Keys.Control;
                    break;
                case "Alt":
                    m = Keys.Alt;
                    break;
                default:
                    m = Keys.None;
                    break;
            }
            Keys k;
            if (!KeysBinding.KeyNames.ContainsKey(key)) {
                return -1;
            } else {
                k = KeysBinding.KeyNames[key];
            }
            return bind(name, action, m, k, Forced);
        }

        public int bind(string name, Action action, string modifire,Keys key, bool Forced = false) {
            Keys m;
            switch (modifire) {
                case "Shift":
                    m = Keys.Shift;
                    break;
                case "Ctrl":
                    m = Keys.Control;
                    break;
                case "Alt":
                    m = Keys.Alt;
                    break;
                default:
                    m = Keys.None;
                    break;
            }
            return bind(name, action, m, key, Forced);
        }

        public int bind(string name, Action action, Keys modifire, string key, bool Forced = false) {
            Keys k;
            if (!KeysBinding.KeyNames.ContainsKey(key)) {
                return -1;
            } else {
                k = KeysBinding.KeyNames[key];
            }
            return bind(name, action, modifire, k, Forced);
        }

        public int unbind(string name) {
            Action action;
            GridBinding.TryGetValue(name, out action);
            return keyBinding.removeKeyFunction(action);
        }


        //===========================================================================================================================



        private void keyBindingGrid_KeyDown(object sender, KeyEventArgs e) {
            is_user_changed = false;
            e.Handled = true;//阻止事件传递
            if (keyBindingGrid.CurrentCell.ColumnIndex != 2) return;//判断是否为副键列
            if (Control.ModifierKeys == Keys.Alt || Control.ModifierKeys == Keys.Control || Control.ModifierKeys == Keys.Shift) {

                return;//这些应该在主键设置
            }

            string name = keyBindingGrid.CurrentRow.Cells[0].Value.ToString();//获取行头
            Action action = GridBinding[name];//通过行头获取绑定的事件函数
            var tmp = keyBinding.getKey(action);//获取原键组
            if (e.KeyCode == tmp.key) return;//判断是否和原键组相同

            string err = keyBinding.getError(keyBinding.registKeyFunction(tmp.modifier, e.KeyCode, action));//获取错误信息
            if (err != "") {
                MessageBox.Show(err);//显示错误信息
                return;
            }
            if (config != null) {
                setData(name, null, KeysBinding.reverseKeyNames[e.KeyCode],autoSave);
            }

            keyBindingGrid.CurrentCell.Value = KeysBinding.reverseKeyNames[e.KeyCode];//更新新键

            e.Handled = true;//阻止事件传递
        }

        private void keyBindingGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e) {
            //判断是否由程序更改
            if (!is_user_changed) return;
            is_user_changed = false;
            if (keyBindingGrid.CurrentCell.ColumnIndex != 1) return;
            String name = keyBindingGrid.CurrentRow.Cells[0].Value.ToString();
            Action action = GridBinding[name];
            var tmp = keyBinding.getKey(action);
            Keys m;
            switch (keyBindingGrid.CurrentCell.Value) {
                case "Shift":
                    m = Keys.Shift;
                    break;
                case "Ctrl":
                    m = Keys.Control;
                    break;
                case "Alt":
                    m = Keys.Alt;
                    break;
                default:
                    m = Keys.None;
                    break;
            }

            if (m == tmp.modifier) return;
            string err = keyBinding.getError(keyBinding.registKeyFunction(m, tmp.key, action));
            if (err != "") {
                MessageBox.Show(err);
                return;
            } else {
                if(config!=null) {
                    //keygrid.[name].modifire
                    //获取名称
                    setData(name, (string)keyBindingGrid.CurrentCell.Value, null,autoSave);
                }
            }

        }

        private void keyBindingGrid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            is_user_changed = true;
        }

        private void keyBindingGrid_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (!is_user_changed) return;
            if (keyBindingGrid.CurrentCell == null || keyBindingGrid.CurrentCell.ColumnIndex != 1) return;
            keyBindingGrid.EndEdit();
            is_user_changed = false;
        }

        private void addEvent() {
            this.keyBindingGrid.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.keyBindingGrid_CellMouseClick);
            this.keyBindingGrid.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.keyBindingGrid_CellMouseLeave);
            this.keyBindingGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.keyBindingGrid_CellValueChanged);
            this.keyBindingGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.keyBindingGrid_KeyDown);
        }

        //==============================================配置保存====================================================================

        private void setData(string name,string modifire=null,string key=null,bool save=false) {
             #region ---- 设置值 ----
            if (key != null) {
                try {
                    if (config["keyGrid"] is Dictionary<string, object> data) {
                        try {
                            if (config["keyGrid"].ContainsKey($"{name}") && config["keyGrid"][$"{name}"] is Dictionary<string,object>) {
                                var keys = (Dictionary<string, object>)data[$"{name}"];
                                keys["key"] = key;
                            } else {
                                var keys = new Dictionary<string, object>();
                                keys["key"] = key;
                                data[$"{name}"] = keys;
                            }
                        } catch {
                            var keys = new Dictionary<string, object>();
                            keys["key"] = key;
                            data[$"{name}"] = keys;
                        }
                    } else {
                        config["keyGrid"] = new Dictionary<string, object>();
                        var keys = new Dictionary<string, object>();
                        keys["key"] = key;
                        config["keyGrid"][$"{name}"] = keys;
                    }
                } catch {
                    config["keyGrid"] = new Dictionary<string, object>();
                    var keys = new Dictionary<string, object>();
                    keys["key"] = key;
                    config["keyGrid"][$"{name}"] = keys;
                }
            }
            if(modifire != null) {
                try {
                    if (config["keyGrid"] is Dictionary<string, object> data) {
                        try {
                            if (config["keyGrid"].ContainsKey($"{name}")) {
                                var keys = (Dictionary<string, object>)data[$"{name}"];
                                keys["modifire"] = modifire;
                            } else {
                                var keys = new Dictionary<string, object>();
                                keys["modifire"] = modifire;
                                data[$"{name}"] = keys;
                            }
                        } catch {
                            var keys = new Dictionary<string, object>();
                            keys["modifire"] = modifire;
                            data[$"{name}"] = keys;
                        }
                    } else {
                        config["keyGrid"] = new Dictionary<string, object>();
                        var keys = new Dictionary<string, object>();
                        keys["modifire"] = modifire;
                        config["keyGrid"][$"{name}"] = keys;
                    }
                } catch {
                    config["keyGrid"] = new Dictionary<string, object>();
                    var keys = new Dictionary<string, object>();
                    keys["modifire"] = modifire;
                    config["keyGrid"][$"{name}"] = keys;
                }
            }
            #endregion
            if (save) { saveConfig(); }
        }


        public void saveConfig() {
            if (ConfigPath == null) return;
            using (var writer = new System.IO.StreamWriter(ConfigPath)) {
                string json = Json.Serialize(config);
                writer.Write(json);
            }
        }

        public void Load() {
            if (config == null) return;
            var tmpDict = new Dictionary<string, Action>(GridBinding);
            foreach (var e in tmpDict) {
                string name = e.Key;
                Action action = e.Value;
                try {
                    if(config["keyGrid"] is Dictionary<string, object> data) {
                        try {
                            if (data.ContainsKey($"{name}")) {
                                var keys = (Dictionary<string, object>)data[$"{name}"];
                                string modifire_str = (string)(keys["modifire"]);
                                string key_str = (string)(keys["key"]);
                                bind(name, action, modifire_str, key_str, true);
                            }
                        } catch{
                            
                        }
                    }
                } catch{
                    config["keyGrid"] = new Dictionary<string, object>();
                }
            }
            initGridData();
        }

        public void loadFromFile(string path) {
            if (System.IO.File.Exists(path)) {
                using (var reader = new System.IO.StreamReader(path)) {
                    string json = reader.ReadToEnd();
                    try {
                        config = Json.Parse(json);
                    } catch {
                        config= Json.Parse("{}"); 
                    }
                }
                Load();
                
            }
        }

        public void loadFromFile() {
            if (ConfigPath == null) return;
            loadFromFile(ConfigPath);
        }
        public void saveToFile(string path) {
            using (var writer = new System.IO.StreamWriter(path)) {
                string json = Json.Serialize(config);
                writer.Write(json);
            }
        }
        public void saveToFile() {
            if (ConfigPath == null) return;
            saveToFile(ConfigPath);
        }

        public void setAutoSave(bool autoSave) {
            if(config==null && ConfigPath=="") return;
            this.autoSave = autoSave;
        }


        //===========================================================================================================================

    }

}
