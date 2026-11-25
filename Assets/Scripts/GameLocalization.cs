using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace PinPop {
 // Existing Turkish copy is the stable key. Only presentation is translated;
 // gameplay identifiers, resource paths and saved data never depend on language.
 public static class GameLocalization {
  [Serializable] public sealed class Entry {public string source,en,enOne;}
  [Serializable] sealed class Catalog {public Entry[] entries;}
  [Serializable] public sealed class LanguageChoice {public string code,nativeName;}
  [Serializable] sealed class LanguageChoices {public LanguageChoice[] languages;}
  static LanguageChoice[] choices;
  public static IReadOnlyList<LanguageChoice> SupportedLanguages {get{if(choices==null){var asset=Resources.Load<TextAsset>("Localization/languages");if(asset==null)throw new InvalidOperationException("Missing language choices");choices=JsonUtility.FromJson<LanguageChoices>(asset.text).languages;var codes=new HashSet<string>();foreach(var choice in choices)if(string.IsNullOrEmpty(choice.code)||string.IsNullOrEmpty(choice.nativeName)||!codes.Add(choice.code))throw new InvalidOperationException("Invalid language choice");if(!codes.Contains("en"))throw new InvalidOperationException("English fallback is required");}return choices;}}
  public static bool Supports(string code){foreach(var choice in SupportedLanguages)if(choice.code==code)return true;return false;}
  public static string NativeLanguageName(string code){foreach(var choice in SupportedLanguages)if(choice.code==code)return choice.nativeName;return "English";}
  public static bool IsNativeLanguageName(string value){foreach(var choice in SupportedLanguages)if(choice.nativeName==value)return true;return false;}
  static Dictionary<string,string> english,singular;
  static HashSet<string> englishValues;
  static string language;
  public const string PreferenceKey="PinPop.Language";
  public static event Action Changed;
  public static string PreferenceCode {get{string value=PlayerPrefs.GetString(PreferenceKey,"auto");return Supports(value)?value:"auto";}}
  public static void SetPreference(string value){if(value!="auto"&&!Supports(value))throw new ArgumentException("Unsupported language preference",nameof(value));if(PreferenceCode==value)return;PlayerPrefs.SetString(PreferenceKey,value);PlayerPrefs.Save();language=null;Changed?.Invoke();}
  public static string Resolve(SystemLanguage device)=>device==SystemLanguage.Turkish?"tr":"en";
  public static string LanguageCode {get {
   if(language==null){language=PreferenceCode; if(language!="auto")return language;language=Resolve(Application.systemLanguage);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    string review=Environment.GetEnvironmentVariable("PINPOP_REVIEW_LANGUAGE");if(review=="en"||review=="tr")language=review;
#endif
   }return language;
  }}
  public static bool Turkish=>LanguageCode=="tr";
  public static CultureInfo Culture=>CultureInfo.GetCultureInfo(Turkish?"tr-TR":"en-US");
  public static IReadOnlyDictionary<string,string> English {get{Load();return english;}}
#if UNITY_EDITOR || DEVELOPMENT_BUILD
  public static readonly HashSet<string> UntranslatedSources=new HashSet<string>();
  public static bool IsCatalogOutput(string value){
   Load();if(string.IsNullOrWhiteSpace(value)||english.ContainsKey(value)||englishValues.Contains(value))return true;
   if(System.Text.RegularExpressions.Regex.IsMatch(value,@"^[\d\s.,/×+−→★☆<>:=KMb–·]+$"))return true;
   foreach(var row in english)foreach(string template in new[]{row.Key,row.Value,singular.TryGetValue(row.Key,out string one)?one:row.Value})if(template.Contains("{0}")){
    string pattern=System.Text.RegularExpressions.Regex.Escape(template);foreach(System.Text.RegularExpressions.Match field in System.Text.RegularExpressions.Regex.Matches(template,@"\{\d+\}"))pattern=pattern.Replace(System.Text.RegularExpressions.Regex.Escape(field.Value),".*");
    if(System.Text.RegularExpressions.Regex.IsMatch(value,"^"+pattern+"$",System.Text.RegularExpressions.RegexOptions.Singleline))return true;
   }
   var lines=value.Split('\n');if(lines.Length>1){foreach(string line in lines)if(!IsCatalogOutput(line))return false;return true;}return false;
  }
#endif
  static void Load(){if(english!=null)return;var asset=Resources.Load<TextAsset>("Localization/strings");if(asset==null)throw new InvalidOperationException("Missing PinPop localization catalog");var catalog=JsonUtility.FromJson<Catalog>(asset.text);english=new Dictionary<string,string>(StringComparer.Ordinal);singular=new Dictionary<string,string>(StringComparer.Ordinal);englishValues=new HashSet<string>(StringComparer.Ordinal);foreach(var entry in catalog.entries){if(string.IsNullOrEmpty(entry.source)||string.IsNullOrEmpty(entry.en)||english.ContainsKey(entry.source))throw new InvalidOperationException("Invalid or duplicate localization entry: "+entry.source);english.Add(entry.source,entry.en);englishValues.Add(entry.en);if(!string.IsNullOrEmpty(entry.enOne)){singular.Add(entry.source,entry.enOne);englishValues.Add(entry.enOne);}}}
  public static string Text(string source){
   if(string.IsNullOrEmpty(source))return source??"";Load();
   if(english.TryGetValue(source,out string translated))return Turkish?source:translated;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
   // Ignore numbers and symbols, but report unregistered copy in either language.
   if(!englishValues.Contains(source)&&Array.Exists(source.ToCharArray(),char.IsLetter))UntranslatedSources.Add(source);
#endif
   return source;
  }
  public static string Format(string source,params object[] values){string template=Text(source);bool one=values.Length>0&&(values[0] is int number?number==1:values[0] is string count&&count=="1");if(!Turkish&&one&&singular.TryGetValue(source,out string single))template=single;var arguments=new object[values.Length];for(int i=0;i<values.Length;i++)arguments[i]=values[i] is string text?Text(text):values[i];return string.Format(Culture,template,arguments);}
 }
}
