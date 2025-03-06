using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace WPFAdmin.Test;

public class CrackPassword {
    [Fact]
    public void Crack() {
        var res = GetPassword([999999998, 999999997, 999999999]);
    }

    public string GetPassword(int[] password) {
        return string.Join("",
            password.Select(x => x.ToString()).OrderBy(x => x,
                Comparer<string>.Create((a, b) =>
                    string.Compare(a + b, b + a))));


        // // 将整数数组转换为字符串数组
        // string[] strArr = Array.ConvertAll(password, x => x.ToString());
        //
        // // 自定义排序规则：比较两个字符串拼接后的大小
        // Array.Sort(strArr, (x, y) => 
        //     string.Compare(x + y, y + x));
        //
        // // 拼接结果
        // return string.Concat(strArr);
    }

    [Fact]
    public void SumEvent() {
       var res = SumEvent1([1, 2, 3, 4], [[1, 0], [-3, 1], [-4, 0], [2, 3]]);
    }

    public int[] SumEvent1(int[] nums, int[][] queries) {
        int[] result = new int[queries.Length];
    
        for (int q = 0; q < queries.Length; q++) {
            // 将查询值加到指定索引的元素上
            nums[queries[q][1]] += queries[q][0];
        
            // 计算修改后数组中所有偶数值的和
            int sum = 0;
            for (int i = 0; i < nums.Length; i++) {
                if (nums[i] % 2 == 0) { // 检查是否为偶数
                    sum += nums[i];
                }
            }
        
            result[q] = sum;
        }
    
        return result;
       
    }

    [Fact]
    public void Test() {
      var result =  MostCommonWord("Bob!", new[] { "hit" });
    }
    
    
    public string MostCommonWord(string paragraph, string[] banned)
    {
        // 将段落转换为小写
        paragraph = paragraph.ToLower();
            
        // 使用正则表达式替换所有非字母字符为空格
        paragraph = Regex.Replace(paragraph, "[^a-z!]", " ");
            
        // 将段落分割成单词数组
        string[] words = paragraph.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
        // 创建一个HashSet存储禁用词，便于快速查找
        HashSet<string> bannedSet = new HashSet<string>(banned);
            
        // 创建字典统计每个单词的出现次数
        Dictionary<string, int> wordCount = new Dictionary<string, int>();
            
        // 统计每个非禁用词的出现次数
        foreach (string word in words)
        {
            if (!bannedSet.Contains(word))
            {
                if (wordCount.ContainsKey(word))
                {
                    wordCount[word]++;
                }
                else
                {
                    wordCount[word] = 1;
                }
            }
        }
            
        // 找出出现频率最高的单词
        string mostCommonWord = "";
        int maxCount = 0;
            
        foreach (var pair in wordCount)
        {
            if (pair.Value > maxCount)
            {
                mostCommonWord = pair.Key;
                maxCount = pair.Value;
            }
        }
            
        return mostCommonWord;
    }
    
    
    
   
}