﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace URL_Parser.Tests.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://cmsbestpractices.com/UrlParserTests/test1.html")]
        public string EmptyHtmlPagePath {
            get {
                return ((string)(this["EmptyHtmlPagePath"]));
            }
            set {
                this["EmptyHtmlPagePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://cmsbestpractices.com/UrlParserTests/test.html")]
        public string TestHtmlPagePath {
            get {
                return ((string)(this["TestHtmlPagePath"]));
            }
            set {
                this["TestHtmlPagePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://cmsbestpractices.com/UrlParserTests/testStyles.css")]
        public string TestCssFilePath {
            get {
                return ((string)(this["TestCssFilePath"]));
            }
            set {
                this["TestCssFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://cmsbestpractices.com/UrlParserTests/testScripts.js")]
        public string TestJsFilePath {
            get {
                return ((string)(this["TestJsFilePath"]));
            }
            set {
                this["TestJsFilePath"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("http://cmsbestpractices.com")]
        public string TestRequestUrl {
            get {
                return ((string)(this["TestRequestUrl"]));
            }
            set {
                this["TestRequestUrl"] = value;
            }
        }
    }
}