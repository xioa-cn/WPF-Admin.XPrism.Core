using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Text;
using Microsoft.International.Converters.PinYinConverter;

namespace PersonalityComponentModules.Views;

public partial class PinYinView : Page {
    public PinYinView() {
        InitializeComponent();
        
        // 使用Loaded事件确保所有控件都已初始化
        this.Loaded += (s, e) => {
            UpdatePinyin(InputTextBox.Text);
        };
    }

    private void InputTextBox_TextChanged(object sender, TextChangedEventArgs e) {
        // 确保PinyinPanel已初始化
        if (PinyinPanel != null) {
            UpdatePinyin(InputTextBox.Text);
        }
    }

    private void UpdatePinyin(string text) {
        // 添加空检查
        if (PinyinPanel == null)
            return;
            
        PinyinPanel.Children.Clear();
        
        if (string.IsNullOrEmpty(text))
            return;

        // 分割文本为汉字和非汉字部分
        var segments = SplitTextIntoSegments(text);
        
        foreach (var segment in segments)
        {
            if (IsChineseCharacter(segment))
            {
                // 为每个汉字创建带拼音的控件
                foreach (char c in segment)
                {
                    var charContainer = CreatePinyinCharControl(c);
                    PinyinPanel.Children.Add(charContainer);
                }
            }
            else
            {
                // 非汉字部分直接显示
                var textBlock = new TextBlock
                {
                    Text = segment,
                    FontSize = 16,
                    Foreground = FindResource("Text.Primary.Brush") as System.Windows.Media.Brush,
                    VerticalAlignment = VerticalAlignment.Bottom,
                    Margin = new Thickness(0, 0, 0, 0)
                };
                PinyinPanel.Children.Add(textBlock);
            }
        }
    }

    private StackPanel CreatePinyinCharControl(char character) {
        // 获取带声调的拼音
        string pinyinWithTone = GetPinyinWithTone(character);
        
        // 创建容器
        var container = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(2, 0, 2, 0),
            MinWidth = 20 // 设置最小宽度，确保有足够空间显示
        };
        
        // 添加拼音
        var pinyinText = new TextBlock
        {
            Text = pinyinWithTone,
            Style = FindResource("PinyinTextStyle") as Style
        };
        container.Children.Add(pinyinText);
        
        // 添加汉字
        var charText = new TextBlock
        {
            Text = character.ToString(),
            Style = FindResource("ChineseTextStyle") as Style
        };
        container.Children.Add(charText);
        
        return container;
    }

    // 获取带声调的拼音
    private string GetPinyinWithTone(char c) {
        try {
            // 检查是否是汉字
            if (Regex.IsMatch(c.ToString(), @"[\u4e00-\u9fa5]")) {
                // 使用微软拼音转换库
                ChineseChar chineseChar = new ChineseChar(c);
                
                // 获取所有可能的拼音
                var pinyins = chineseChar.Pinyins;
                
                // 检查集合是否有元素
                if (pinyins != null && pinyins.Count > 0) {
                    // 获取第一个非空拼音
                    string pinyin = null;
                    foreach (string p in pinyins) {
                        if (!string.IsNullOrEmpty(p)) {
                            pinyin = p;
                            break;
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(pinyin)) {
                        // 微软拼音库返回的格式通常是大写带数字声调，如"ZHONG1"
                        pinyin = pinyin.Trim();
                        
                        // 提取声调数字（通常在末尾）
                        int toneNumber = 0;
                        if (pinyin.Length > 0 && char.IsDigit(pinyin[pinyin.Length - 1])) {
                            toneNumber = pinyin[pinyin.Length - 1] - '0';
                            pinyin = pinyin.Substring(0, pinyin.Length - 1);
                        }
                        
                        // 转换为小写
                        pinyin = pinyin.ToLower();
                        
                        // 添加声调符号
                        if (toneNumber >= 1 && toneNumber <= 4) {
                            return AddToneMarks(pinyin, toneNumber);
                        }
                        
                        return pinyin;
                    }
                }
            }
            
            // 非汉字或获取拼音失败，返回空字符串
            return "";
        }
        catch (Exception ex) {
            Console.WriteLine($"获取拼音出错: {ex.Message}");
            return "";
        }
    }

    // 添加声调标记
    private string AddToneMarks(string pinyin, int toneNumber) {
        if (string.IsNullOrEmpty(pinyin) || toneNumber < 1 || toneNumber > 4)
            return pinyin;
        
        // 声调字符映射
        char[] vowels = new char[] { 'a', 'e', 'i', 'o', 'u', 'ü' };
        string[][] toneMarks = new string[][] {
            new string[] { "ā", "ē", "ī", "ō", "ū", "ǖ" },
            new string[] { "á", "é", "í", "ó", "ú", "ǘ" },
            new string[] { "ǎ", "ě", "ǐ", "ǒ", "ǔ", "ǚ" },
            new string[] { "à", "è", "ì", "ò", "ù", "ǜ" }
        };
        
        // 替换 v 为 ü
        pinyin = pinyin.Replace('v', 'ü');
        
        // 查找需要添加声调的元音字母
        int vowelIndex = -1;
        
        // 特殊规则：如果有a或e，则声调标在a或e上
        if (pinyin.Contains('a')) {
            vowelIndex = pinyin.IndexOf('a');
        }
        else if (pinyin.Contains('e')) {
            vowelIndex = pinyin.IndexOf('e');
        }
        // 特殊规则：如果有ou，则声调标在o上
        else if (pinyin.Contains("ou")) {
            vowelIndex = pinyin.IndexOf('o');
        }
        // 否则，声调标在最后一个元音上
        else {
            for (int i = pinyin.Length - 1; i >= 0; i--) {
                if (Array.IndexOf(vowels, pinyin[i]) != -1) {
                    vowelIndex = i;
                    break;
                }
            }
        }
        
        // 如果找到了需要添加声调的元音
        if (vowelIndex != -1) {
            char vowel = pinyin[vowelIndex];
            int vowelArrayIndex = Array.IndexOf(vowels, vowel);
            if (vowelArrayIndex != -1 && toneNumber >= 1 && toneNumber <= 4) {
                // 替换为带声调的元音
                string toneMark = toneMarks[toneNumber - 1][vowelArrayIndex];
                pinyin = pinyin.Remove(vowelIndex, 1).Insert(vowelIndex, toneMark);
            }
        }
        
        return pinyin.ToLower();
    }

    private bool IsChineseCharacter(string text) {
        // 检查是否包含汉字
        return Regex.IsMatch(text, @"[\u4e00-\u9fa5]");
    }

    private List<string> SplitTextIntoSegments(string text) {
        var result = new List<string>();
        var currentSegment = "";
        var isCurrentChinese = false;
        
        foreach (char c in text)
        {
            bool isChinese = Regex.IsMatch(c.ToString(), @"[\u4e00-\u9fa5]");
            
            if (string.IsNullOrEmpty(currentSegment))
            {
                // 第一个字符
                currentSegment += c;
                isCurrentChinese = isChinese;
            }
            else if (isChinese == isCurrentChinese)
            {
                // 与当前段落类型相同
                currentSegment += c;
            }
            else
            {
                // 类型变化，添加当前段落并开始新段落
                result.Add(currentSegment);
                currentSegment = c.ToString();
                isCurrentChinese = isChinese;
            }
        }
        
        // 添加最后一个段落
        if (!string.IsNullOrEmpty(currentSegment))
            result.Add(currentSegment);
            
        return result;
    }
}