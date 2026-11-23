using UnityEngine;
namespace PinPop {
 public static class PinPopPalette {
  static Color Hex(string value){ColorUtility.TryParseHtmlString("#"+value,out var color);return color;}
  public const int Count=8;
  public static readonly Color[] Colors={Hex("DF5F84"),Hex("32A6BA"),Hex("8C6AC4"),Hex("E8BC32"),Hex("E98A3D"),Hex("55AA66"),Hex("397AC7"),Hex("D84F48")};
  public static readonly string[] Names={"PEMBE","TURKUAZ","MOR","SARI","TURUNCU","YEŞİL","MAVİ","KIRMIZI"};
  public static readonly string[] Characters={"rabbit","penguin","cat","lion","bear","frog","elephant","fox"};
 }
}
